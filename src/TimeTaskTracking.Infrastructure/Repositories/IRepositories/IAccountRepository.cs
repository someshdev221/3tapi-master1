using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories.IRepositories;

public interface IAccountRepository
{
    Task<Register> EmailExists(string email);
    Task<List<string>>GetUserRolesAsync (string userId);
    Task<LoginResponseViewModel> UserLogin(LoginViewModel model, string passwordHash);
    Task<Register> RegisterUser(Register register, string hashedPassword);
    Task<IEnumerable<string>> GetAllDesignations();
    Task<ChangePasswordViewModel> ChangePassword(ChangePasswordViewModel model);
    Task<Register> PhoneNumberExists(string phoneNumber);
    Task<Register> EmployeeNumberExists(string employeeNumber);
    Task<Register> ResetEmployeePassword(string id, string email, string hashPassword);
}
