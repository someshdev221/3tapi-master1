using Microsoft.AspNetCore.Mvc;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Services.IServices;
using TimeTaskTracking.Shared.ViewModels;
using TimeTaskTracking.Validations;
using FluentValidation.Results;
using TimeTaskTracking.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using MimeKit.Encodings;
using FluentValidation;

namespace TimeTaskTracking.Controller;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    public readonly IAccountService _accountService;
    private readonly IValidator<RegisterDto> _validator;

    public AccountController(IAccountService accountService, IValidator<RegisterDto> validator)
    {
        _accountService = accountService;
        _validator = validator;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
    {
        var userDetail = await _accountService.UserLogin(loginViewModel);
        if (userDetail.Model == null)
            return NotFound(userDetail);
        return Ok(userDetail);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromForm] RegisterDto register)
    {
        if (!string.IsNullOrEmpty(register.Country))
        {
            // Bot detected! Block the registration attempt
            return BadRequest("Bot detected!");
        }
        var results = await _validator.ValidateAsync(register);
        if (!results.IsValid)
            return BadRequest(await SharedResources.FluentValidationResponse(results.Errors));

        var userDetail = await _accountService.RegisterUser(register);
        if (userDetail.Model == null)
            return BadRequest(userDetail);
        return Ok(userDetail);
    }
    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        var emailDetail = await _accountService.ForgotPassword(model);
        if (!emailDetail.Model)
            return NotFound(emailDetail);
        return Ok(emailDetail);
    }
    [HttpPost("ValidateToken")]
    public async Task<IActionResult> ValidateToken(TokenViewModel model)
    {
        var tokenValidateResult = await _accountService.ValidateToken(model);
        if (tokenValidateResult?.Model == null)
            return BadRequest(tokenValidateResult);
        return Ok(tokenValidateResult);
    }
    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        var passwordReset = await _accountService.ResetPassword(model);
        if (!passwordReset.Model)
            return BadRequest(passwordReset);
        return Ok(passwordReset);
    }

    [HttpPost("ChangePassword")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        var passwordReset = await _accountService.ChangePassword(model);
        if (!passwordReset.Model)
            return NotFound(passwordReset);
        return Ok(passwordReset);
    }

    [HttpPost("ResetEmployeePassword")]
    public async Task<IActionResult> ResetEmployeePassword([FromBody] ResetEmployeePasswordViewModel resetPasswordViewModel)
    {
        var result = await _accountService.ResetEmployeePassword(resetPasswordViewModel);
        if (result.Model == null)
            return BadRequest(result);
        return Ok(result);
    }
}
