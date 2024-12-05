using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ExecuteProcedure _exec;
    public AccountRepository(IConfiguration configuration)
    {
        _exec = new ExecuteProcedure(configuration);
    }
    public async Task<Register> EmailExists(string email)
    {
        try
        {
            // Call a method, possibly from a data access library, to execute a database query.
            var obj = await _exec.Get_DataTableAsync("RegisterUser",
            new string[] { "@Email", "@OptID" },
            new string[] { Convert.ToString(email), "2" });

            if (obj?.Rows?.Count > 0)
                return await _exec.DataTableToModelAsync<Register>(obj);
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        try
        {
            // Execute the stored procedure and get a DataTable
            var obj = await _exec.Get_DataTableAsync("GetUserRolesByUserId",
                new string[] { "@UserId" },
                new string[] { Convert.ToString(userId) });

            // Check if the DataTable has rows
            if (obj?.Rows?.Count > 0)
            {
                // Create a list to hold the RoleIds
                var roleList = new List<string>();

                // Loop through the rows and add RoleIds to the list
                foreach (DataRow row in obj.Rows)
                {
                    // Assuming RoleId is in the first column
                    var roleId = row["RoleId"].ToString();
                    roleList.Add(roleId);
                }

                return roleList;
            }
            return null;
        }
        catch (Exception ex)
        {
            // Handle exception (logging can be added here if needed)
            return null;
        }
    }


    public async Task<LoginResponseViewModel> UserLogin(LoginViewModel model, string passwordHash)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("UserLogin",
            new string[] { "@Email", "@PasswordHash" },
            new string[] { Convert.ToString(model.Email), Convert.ToString(passwordHash) });

            return await _exec.DataTableToModelAsync<LoginResponseViewModel>(obj);
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<Register> RegisterUser(Register register, string hashedPassword)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("RegisterUser",
                new string[] { "@Name", "@LastName", "@Email", "@PhoneNumber", "@DepartmentId", "@Designation", "@EmployeeNumber", "@Address", "@ExperienceOnJoining", "@JoiningDate", "@Password", "@ProfileImageName", "@Id", "@OptID", "@skypeMail" },
                new string[] { Convert.ToString(register.FirstName), Convert.ToString(register.LastName), Convert.ToString(register.Email), Convert.ToString(register.PhoneNumber),
                           Convert.ToString(register.DepartmentId), Convert.ToString(register.Designation), Convert.ToString(register.EmployeeNumber), Convert.ToString(register.Address),
                           Convert.ToString(register.ExperienceOnJoining), Convert.ToString(register.JoiningDate), Convert.ToString(hashedPassword), register.ProfileImageName ?? string.Empty, Convert.ToString(Guid.NewGuid()), "1", register.SkypeMail});

            return await _exec.DataTableToModelAsync<Register>(obj);
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<ChangePasswordViewModel> ChangePassword(ChangePasswordViewModel model)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("RegisterUser",
            new string[] { "@Id", "@Password", "@OptId" },
            new string[] { Convert.ToString(model.Id), Convert.ToString(model.NewPassword), "3" });
            if (obj?.Rows?.Count > 0)
                return await _exec.DataTableToModelAsync<ChangePasswordViewModel>(obj);
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public  async Task<Register> PhoneNumberExists(string phoneNumber)
    {
        try
        {
            // Call a method, possibly from a data access library, to execute a database query.
            var obj = await _exec.Get_DataTableAsync("RegisterUser",
            new string[] { "@PhoneNumber", "@OptID" },
            new string[] { Convert.ToString(phoneNumber), "6" });

            if (obj?.Rows?.Count > 0)
                return await _exec.DataTableToModelAsync<Register>(obj);
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<Register> EmployeeNumberExists(string employeeNumber)
    {
        try
        {
            // Call a method, possibly from a data access library, to execute a database query.
            var obj = await _exec.Get_DataTableAsync("RegisterUser",
            new string[] { "@EmployeeNumber", "@OptID" },
            new string[] { Convert.ToString(employeeNumber), "7" });

            if (obj?.Rows?.Count > 0)
                return await _exec.DataTableToModelAsync<Register>(obj);
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<IEnumerable<string>> GetAllDesignations()
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("RegisterUser",
                new string[] { "@OptID" },
                new string[] { "8" });

            var designations = new List<string>();

            foreach (System.Data.DataRow row in obj.Rows)
            {
                designations.Add(Convert.ToString(row["Designation"]));
            }

            return designations;
        }
        catch (Exception ex)
        {
            // Handle exception as per your application's error handling strategy
            return null;
        }
    }
    public async Task<Register> ResetEmployeePassword(string id, string email, string hashPassword)
    {
        try
        {
            var obj = await _exec.Get_DataTableAsync("ResetEmployeePassword",
            new string[] { "@Id", "@Email", "@Password", "@OptID" },
             new string[] { Convert.ToString(id),Convert.ToString(email),Convert.ToString(hashPassword), "1" });
            if (obj?.Rows?.Count > 0)
            {
                return await _exec.DataTableToModelAsync<Register>(obj);
            }

            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

}
