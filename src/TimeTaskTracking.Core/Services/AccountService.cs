using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Infrastructure;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;
using TimeTaskTracking.Shared.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Win32;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace TimeTaskTracking.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AccountService> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public AccountService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IEmailService emailService, IWebHostEnvironment webHostEnvironment, ILogger<AccountService> logger, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public async Task<ResponseModel<TokenDto>> UserLogin(LoginViewModel loginViewModel)
        {
            var result = new ResponseModel<TokenDto>();
            var userDetail = await _unitOfWork.Account.EmailExists(loginViewModel.Email);

            if (userDetail != null)
            {
                // Verify the password
                if (SharedResources.VerifyHashedPassword(userDetail.PasswordHash, loginViewModel.Password))
                {
                    // Check if user is active
                    if (userDetail.IsActive == 0 || userDetail.IsActive == 2)
                    {
                        result.Message.Add(SharedResources.UserInactiveMessage);
                        return result;
                    }

                    // Retrieve roles associated with the user
                    var userRoles = await _unitOfWork.Account.GetUserRolesAsync(userDetail.Id);

                    // Set role to Admin if user has the Admin role
                    string role = userRoles.Contains("2b8849ea-016a-40fb-8d20-8e6f9bbb00d1") ? "Admin" : userDetail.RoleName ?? string.Empty;

                    // Check if the user is also a Team Lead and override the role if applicable
                    var employeeCountIfTeamLead = await _unitOfWork.Employee.EmployeeCountInTeam(userDetail.Id);
                    if (employeeCountIfTeamLead != 0)
                        role = "Team Lead";

                    // Create claims
                    var claims = new ClaimsIdentity(new Claim[]
                    {
                new Claim("id", userDetail.Id ?? string.Empty),
                new Claim(ClaimTypes.Name, userDetail.FirstName + " " + userDetail.LastName ?? string.Empty),
                new Claim(ClaimTypes.Email, userDetail.Email ?? string.Empty),
                new Claim("designation", userDetail.Designation ?? string.Empty),
                new Claim("departmentId", userDetail.DepartmentId == 0 ? string.Empty : userDetail.DepartmentId.ToString()),
                new Claim(ClaimTypes.Role, role)  // Assign role as Admin or Team Lead
                    });

                    // Generate the token
                    TokenDto tokenModel = new();
                    tokenModel.Token = GenrateJwtToken(userDetail, claims, 1440);  // 1440 minutes = 24 hours
                    result.Message.Add(SharedResources.LoggedIn);
                    result.Model = tokenModel;

                    // Check and delete To-Do list if exists
                    var today = DateTime.Today;
                    var currentDateEndTime = today.AddDays(1).AddTicks(-1);
                    var checkToDoListExists = await _unitOfWork.CommonRepository.ToDoListExistsForTheDate(currentDateEndTime);
                    if (checkToDoListExists != null)
                    {
                        await _unitOfWork.CommonRepository.DeleteToDoListAsync(currentDateEndTime);
                    }
                }
                else
                {
                    result.Message.Add(SharedResources.IncorrectPassword);
                }
            }
            else
            {
                result.Message.Add(SharedResources.IncorrectEmail);
            }

            return result;
        }


        public async Task<ResponseModel<string>> RegisterUser(RegisterDto register)
        {
            var result = new ResponseModel<string>();
            var userInfo = await _unitOfWork.Account.EmailExists(register.Email);
            if (userInfo != null)
            {
                if (userInfo.IsActive == 0 || userInfo.IsActive == 2)
                    result.Message.Add(SharedResources.EmailExistUserInActive);
                else if (userInfo.IsActive == 1)
                    result.Message.Add(SharedResources.EmailAlreadyExist);
                return result;
            }
            var phoneNumberExists = await _unitOfWork.Account.PhoneNumberExists(register.PhoneNumber);
            if (phoneNumberExists != null)
            {
                result.Message.Add(SharedResources.PhoneNumberAlreadyExist);
                return result;
            }
            var employeeNumberExists = await _unitOfWork.Account.EmployeeNumberExists(register.EmployeeNumber);
            if (employeeNumberExists != null)
            {
                result.Message.Add(SharedResources.EmployeeNumberAlreadyExist);
                return result;
            }

            // Fetch valid designations  from the database
            var validDesignations = await _unitOfWork.Account.GetAllDesignations();

            if (!validDesignations.Contains(register.Designation))
            {
                result.Message.Add(SharedResources.PleaseProvideAValidDesignation);
                return result;
            }
            // Hash the password 
            string hashedPassword = SharedResources.HashPassword(register.Password);
            var mappedModel = _mapper.Map<Register>(register);

            if (register?.ProfileImage != null)
            {
                mappedModel.ProfileImageName = await SharedResources.SaveProfileImage(register.ProfileImage, _webHostEnvironment, _logger);
            }

            var user = await _unitOfWork.Account.RegisterUser(mappedModel, hashedPassword);
            if (user?.Id != null)
            {
                result.Message.Add(SharedResources.UserRegistered);
                result.Model = user.Id;
            }
            else
                result.Message.Add(SharedResources.TechnicalIssue);
            return result;
        }

        public async Task<ResponseModel<RegisterDto>> CheckUserExist(string email)
        {
            var result = new ResponseModel<RegisterDto>();
            var userDetail = await _unitOfWork.Account.EmailExists(email);
            if (!string.IsNullOrEmpty(userDetail?.Email))
                result.Model = _mapper.Map<RegisterDto>(userDetail);
            else
                result.Message.Add(SharedResources.UserNotFound);
            return result;
        }

        public async Task<ResponseModel<bool>> GetDepartmentById(int departmentId)
        {
            var response = new ResponseModel<bool>();
            var checkDepartmentExists = await _unitOfWork.Employee.GetDepartmentExistById(departmentId);
            if (checkDepartmentExists)
            {
                response.Model = checkDepartmentExists;
            }
            else
                response.Message.Add(SharedResources.DepartmentNotFound);
            return response;
        }



        public async Task<ResponseModel<bool>> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            var result = new ResponseModel<bool>();
            var exists = await _unitOfWork.Account.EmailExists(changePasswordViewModel.Email);
            if (!string.IsNullOrEmpty(exists?.Email))
            {
                var validPassword = SharedResources.StrongPasswordValidation(changePasswordViewModel.NewPassword);
                if (!string.IsNullOrEmpty(validPassword))
                {
                    result.Message.Add(validPassword);
                    return result;
                }
                bool isOldPasswordValid = SharedResources.VerifyHashedPassword(exists.PasswordHash, changePasswordViewModel.OldPassward);
                if (isOldPasswordValid)
                {
                    changePasswordViewModel.NewPassword = SharedResources.HashPassword(changePasswordViewModel.NewPassword);
                    changePasswordViewModel.Id = exists.Id;

                    var response = await _unitOfWork.Account.ChangePassword(changePasswordViewModel);
                    if (response == null)
                        result.Message.Add(SharedResources.ErrorWhileChangingPassword);
                    else
                        result.Message.Add(SharedResources.PasswordChangedSuccessfully);
                    result.Model = true;
                }
                else
                    result.Message.Add(SharedResources.OldPasswordIsIncorrect);
            }
            else
                result.Message.Add(SharedResources.UserNotFound);
            return result;
        }
        public async Task<ResponseModel<bool>> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            var result = new ResponseModel<bool>();
            var user = await _unitOfWork.Account.EmailExists(forgotPasswordViewModel.Email);
            if (string.IsNullOrEmpty(user?.Email))
                result.Message.Add(SharedResources.UserNotFound);
            else
            {
                var claims = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("id", user.Id ?? string.Empty),
                    new Claim(ClaimTypes.Name, user.FirstName+" "+ user.LastName?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
                    });

                var tokenString = GenrateJwtToken(user, claims, 1);  //Token Validate for 5 minutes
                if (!forgotPasswordViewModel.DomainName.EndsWith("/"))
                {
                    forgotPasswordViewModel.DomainName += "/";
                }
                var callbackUrl = $"{forgotPasswordViewModel.DomainName}resetpassword?request={Uri.EscapeDataString(tokenString)}";
                var emailTemplate = await SharedResources.GetForgotEmailTemplate();

                emailTemplate = emailTemplate.Replace("@callbackUrl", callbackUrl);
                //var currentDirectory = Directory.GetCurrentDirectory();
                //var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                //var filePath = Path.Combine(currentDirectory, "ViewModels", "Utility", "ForgotEmailTemplate.html");
                //string emailTemplate;
                //using (StreamReader reader = new StreamReader(filePath))
                //{
                //    emailTemplate = reader.ReadToEnd();
                //}
                //var abc = emailTemplate;
                //emailTemplate = abc.Replace("@callbackUrl", callbackUrl);
                _emailService.SendMail(user.Email, user.FirstName + " " + user.LastName, "", "",
                    emailTemplate, "Reset Password");
                //result.Message.Add(currentDirectory);
                result.Message.Add(SharedResources.EmailSent);
                result.Model = true;
            }
            return result;
        }

        public async Task<ResponseModel<dynamic>> ValidateToken(TokenViewModel model)
        {
            var response = new ResponseModel<dynamic>();
            var validResponse = GetPrincipalFromExpiredToken(model.Token);
            if (string.IsNullOrEmpty(validResponse.Email))
                response.Message.Add(validResponse.Message);
            else
                response.Model = new { Id = validResponse.Id, Email = validResponse.Email };
            return response;
        }

        public async Task<ResponseModel<bool>> ResetPassword(ResetPasswordViewModel model)
        {
            var result = new ResponseModel<bool>();
            var validToken = GetPrincipalFromExpiredToken(model.Token);
            if (!string.IsNullOrEmpty(validToken?.Email))
            {
                var validPassword = SharedResources.StrongPasswordValidation(model.Password);
                if (!string.IsNullOrEmpty(validPassword))
                {
                    result.Message.Add(validPassword);
                    return result;
                }
                string hashedPassword = SharedResources.HashPassword(model.Password);
                var changePassword = new ChangePasswordViewModel()
                {
                    Email = validToken.Email,
                    NewPassword = hashedPassword,
                    Id = validToken.Id
                };
                var response = await _unitOfWork.Account.ChangePassword(changePassword);
                if (response != null)
                {
                    result.Message.Add(SharedResources.PasswordResetSuccess);
                    result.Model = true;
                }
                else
                    result.Message.Add(SharedResources.ErrorWhileResetPassword);
            }
            else
                result.Message.Add(validToken.Message);
            return result;
        }

        public string GenrateJwtToken(Register userDetail, ClaimsIdentity claims, double tokenTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["TokenKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(tokenTime), // Token expiration time
                Issuer = _configuration["JWTSettings:Issuer"],
                Audience = _configuration["JWTSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ValidTokenResponseViewModel GetPrincipalFromExpiredToken(string token)
        {
            var result = new ValidTokenResponseViewModel();
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["TokenKey"])),
                    ValidateLifetime = true // Disabling token expiration validation
                };

                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    SecurityToken securityToken;

                    if (string.IsNullOrEmpty(token) || token.Split('.').Length != 3)
                    {
                        result.Message = SharedResources.InValidForgotPasswordLink;
                        return result;
                    }
                    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                    var jwtSecurityToken = securityToken as JwtSecurityToken;

                    if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result.Message = SharedResources.InValidForgotPasswordLink;
                        return result;
                    }

                    var emailClaim = principal.FindFirst(ClaimTypes.Email);
                    if (emailClaim == null)
                        result.Message = SharedResources.InValidForgotPasswordLink;
                    var idClaim = principal.FindFirst("id");
                    if (idClaim == null)
                        result.Message = SharedResources.InValidForgotPasswordLink;

                    result.Email = emailClaim?.Value;
                    result.Id = idClaim?.Value;
                }
                catch (SecurityTokenExpiredException)
                {
                    result.Message = SharedResources.ExpiredForgotPasswordLink;
                }
                catch (SecurityTokenException)
                {
                    result.Message = SharedResources.InValidForgotPasswordLink;
                }
            }
            catch (Exception)
            {
                result.Message = SharedResources.InValidForgotPasswordLink;
            }
            return result;
        }

        public async Task<ResponseModel<string>> ResetEmployeePassword(ResetEmployeePasswordViewModel resetPasswordViewModel)
        {
            var result = new ResponseModel<string>();
            var claims = await SharedResources.GetDepartmentFromClaims(_contextAccessor.HttpContext, 0, string.Empty);
            var userDetail = await _unitOfWork.Account.EmailExists(resetPasswordViewModel.Email);

            if (claims.Role == "Project Manager")
            {
                var employeeDetails = await _unitOfWork.Employee.GetEmployeeListByManager(claims.UserId, claims.DepartmentId);
                var employeeIdList = employeeDetails.Select(x => x.Id).ToList();
                if (employeeIdList != null)
                {
                    var filteredEmployeeDetails = employeeIdList.Contains(userDetail.Id);
                    if (filteredEmployeeDetails == false)
                    {
                        result.Message.Add(SharedResources.EmployeeNotAssignedToYou);
                        return result;
                    }
                }
            }
            if (userDetail == null)
            {
                result.Message.Add(SharedResources.UserNotFound);
                return result;
            }
            var validPassword = SharedResources.StrongPasswordValidation(resetPasswordViewModel.Password);
            if (!string.IsNullOrEmpty(validPassword))
            {
                result.Message.Add(validPassword);
                return result;
            }

            if (resetPasswordViewModel.Password != resetPasswordViewModel.ConfirmPassword)
            {
                result.Message.Add(SharedResources.PasswordDoesNotMatchWithConfirmpassword);
            }

            var hashedPasseword = SharedResources.HashPassword(resetPasswordViewModel.Password);
            var updateResult = await _unitOfWork.Account.ResetEmployeePassword(resetPasswordViewModel.Id, resetPasswordViewModel.Email, hashedPasseword);
            if (updateResult != null)
            {
                result.Message.Add(SharedResources.PasswordResetSuccess);
                result.Model = updateResult.Email;
            }
            else
            {
                result.Message.Add(SharedResources.FailedToResetPassword);
            }
            return result;
        }
    }
}
