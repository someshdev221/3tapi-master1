
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Services.IServices;

public interface IAccountService
{
    Task<ResponseModel<TokenDto>> UserLogin(LoginViewModel loginViewModel);
    Task<ResponseModel<string>> RegisterUser(RegisterDto register);
    Task<ResponseModel<RegisterDto>> CheckUserExist(string email);
    Task<ResponseModel<bool>> ForgotPassword(ForgotPasswordViewModel model);
    Task<ResponseModel<dynamic>> ValidateToken(TokenViewModel model);
    Task<ResponseModel<bool>> ResetPassword(ResetPasswordViewModel model);
    Task<ResponseModel<bool>> ChangePassword(ChangePasswordViewModel changePasswordViewModel);
    Task<ResponseModel<bool>> GetDepartmentById(int deparmentId);

    Task<ResponseModel<string>> ResetEmployeePassword(ResetEmployeePasswordViewModel resetPasswordViewModel);
}
