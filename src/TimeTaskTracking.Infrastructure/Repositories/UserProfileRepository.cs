using Microsoft.Extensions.Configuration;
using Serilog.Context;
using System.Data;
using TimeTaskTracking.Infrastructure.Extensions;
using TimeTaskTracking.Infrastructure.Repositories.IRepositories;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ExecuteProcedure _exec;
        private readonly IConfiguration _configuration;

        public UserProfileRepository(IConfiguration configuration)
        {
            _exec = new ExecuteProcedure(configuration);

        }
        public async Task<UserProfile> GetUserProfileDetailAsync(string userProfileId)
        {
            var obj = await _exec.Get_DataSetAsync("GetAspNetUsers",
                new string[] { "@UserId", "@OptId" },
                new string[] { userProfileId, "1" });

            if (obj == null || obj.Tables.Count == 0 || obj.Tables[0].Rows.Count == 0)
                return null;

            return await _exec.DataTableToModelAsync<UserProfile>(obj.Tables[0]);
        }
        public async Task<UserProfileView> GetUserProfileByIdAsync(string userProfileId)
        {
            try
            {
                var dataTable = await _exec.Get_DataTableAsync("GetAspNetUsers",
                    new string[] { "@UserId", "@OptId" },
                    new string[] { userProfileId, "4" });
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<UserProfileView>(dataTable);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<UpdateUserProject> UpdateProfileAsync(UpdateUserProject updateProfile)
        {
            try
            {
                var dataTable = await _exec.Get_DataTableAsync("ExecuteAspNetUser",
                    new string[] {
                "@UserId", "@FirstName", "@LastName", "@Email", "@SkypeMail", "@DepartmentId",
                "@Designation", "@Skills", "@JoiningDate", "@PhoneNumber", "@EmployeeNumber", "@ProfileImage", "@Address", "@OptId"
                    },
                    new string[] {
                updateProfile.Id, updateProfile.FirstName, updateProfile.LastName,
                updateProfile.Email, updateProfile.SkypeMail, updateProfile.DepartmentId,
                updateProfile.Designation, updateProfile.Skills, updateProfile.JoiningDate.ToString("yyyy-MM-dd"),
                updateProfile.PhoneNumber, updateProfile.EmployeeNumber, updateProfile.ProfileImageName,updateProfile.Address, "1"
                    });

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<UpdateUserProject>(dataTable);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool?> AddNewSkill(string skill)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("AddSkillIs",
                    new string[] { "@Skill" },
                    new string[] { skill });

                if (obj?.Rows.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<int> GetSkillIdByNameAsync(string skill)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetSkillIdByName",
                    new string[] { "@SkillName" },
                    new string[] { skill });

                if (obj?.Rows.Count > 0)
                {
                    int skillId = Convert.ToInt32(obj.Rows[0][0]);
                    return skillId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        
        public async Task<UserProject> GetUserProjectByUserIdAsync(int id)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetAspNetUsers",
                    new string[] { "@Id", "@OptId" },
                    new string[] { id.ToString(), "5" });

                if (obj != null && obj.Rows.Count > 0)
                {
                    var userProjects = _exec.DataTableToList<UserProject>(obj);
                    return userProjects.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogContext.PushProperty("RequestBody", id);
                return null;
            }
        }

        public async Task<string> UpdateUserEmailAsync(UpdateUserEmailViewModel model, string EmployeeId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("UpdateUserEmail",
                    new string[] { "@Email", "EmployeeId" },
                    new string[] { model.Email, EmployeeId });

                if (obj != null && obj.Rows.Count > 0)
                {
                    var userDetails = await _exec.DataTableToModelAsync<Register>(obj);
                    return userDetails.Id;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<AddUserProject> AddUserProjectAsync(AddUserProject userProject)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteAspNetUser",
                    new string[] { "@UserWorkedProjectDescription", "@SvnUrl", "@LiveUrl", "@UserWorkedProjectsLocalUrl", "@ApplicationUserId", "@UserWorkedProjectsId", "@UserWorkedProjectsTechnology", "@Feature", "@OptId" },
                    new string[] { userProject.Description, userProject.SvnUrl, userProject.LiveUrl, userProject.LocalUrl, userProject.ApplicationUsersId, userProject.ProjectsId.ToString(), userProject.Technology, userProject.Feature.ToString(), "2" });

                if (obj?.Rows?.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<AddUserProject>(obj);
                }

                return null;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        //Check if UserProjectExists
        public async Task<bool> UserProjectExistsAsync(string userId, int projectId, int workedProjectId)
        {
            var obj = await _exec.Get_DataTableAsync("ExecuteAspNetUser",
                new string[] { "@ApplicationUserId", "@UserWorkedProjectsId", "@OptId", "@UserWorkedProjectId" },
                new string[] { userId, projectId.ToString(), "10", Convert.ToString(workedProjectId) });

            return obj?.Rows?.Count > 0;
        }

        public async Task<AddUserProject> UpdateUserProjectAsync(AddUserProject updateUserWorkedProject)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteAspNetUser",
                    new string[] { "@UserWorkedProjectId", "@UserWorkedProjectDescription", "@SvnUrl", "@LiveUrl", "@UserWorkedProjectsLocalUrl", "@ApplicationUserId", "@UserWorkedProjectsId", "@UserWorkedProjectsTechnology", "@Feature", "@OptId" },
                    new string[] { Convert.ToString(updateUserWorkedProject.Id), Convert.ToString(updateUserWorkedProject.Description), Convert.ToString(updateUserWorkedProject.SvnUrl),
                        Convert.ToString(updateUserWorkedProject.LiveUrl), Convert.ToString(updateUserWorkedProject.LocalUrl), Convert.ToString(updateUserWorkedProject.ApplicationUsersId), Convert.ToString(updateUserWorkedProject.ProjectsId), Convert.ToString(updateUserWorkedProject.Technology),Convert.ToString(updateUserWorkedProject.Feature), "4" });

                if (obj?.Rows?.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<AddUserProject>(obj);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> DeleteUserProjectAsync(int userProjectID)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteAspNetUser",
                    new string[] { "@UserWorkedProjectId", "@OptId" },
                    new string[] { Convert.ToString(userProjectID), "6" });

                if (obj?.Rows?.Count > 0)
                {
                    var status = Convert.ToInt32(obj.Rows[0]["Status"]);
                    return status == 1;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<UserTools> GetUserToolByUserIdAsync(int id)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetAspNetUsers",
                    new string[] { "@Id", "@OptId" },
                    new string[] { id.ToString(), "6" });

                if (obj != null && obj.Rows.Count > 0)
                {
                    var userTools = _exec.DataTableToList<UserTools>(obj);
                    return userTools.FirstOrDefault(); // Return the first item or null
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogContext.PushProperty("RequestBody", id);
                return null;
            }
        }
        public async Task<UserTools> AddUserToolAsync(UserTools userTools)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteAspNetUser",
                    new string[] { "@UserToolsDescription", "@NetworkUrl", "@LocalUrl", "@DateTime", "@ApplicationUserId", "@UserToolsTechnology", "@OptId" },
                    new string[] { Convert.ToString(userTools.Description), Convert.ToString(userTools.NetworkUrl),
                    Convert.ToString(userTools.LocalUrl), DateTime.Now.ToString(), Convert.ToString(userTools.ApplicationUsersId), Convert.ToString(userTools.Technology), "3" });
                if (obj != null && obj.Rows.Count > 0)
                {
                    return _exec.DataTableToModel<UserTools>(obj);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<UserTools> GetUserToolByDescriptionAsync(string applicationUsersId, string description)
        {
            var obj = await _exec.Get_DataTableAsync("ExecuteAspNetUser",
                new string[] { "@ApplicationUsersId", "@UserToolsDescription", "@OptId" },
                new string[] { applicationUsersId, description, "12" }); 

            if (obj != null && obj.Rows.Count > 0)
            {
                return _exec.DataTableToModel<UserTools>(obj);
            }

            return null;
        }
        public async Task<UserTools> UpdateUserToolAsync(UserTools userTool)
        {
            try
            {
                var updatedUserTool = await _exec.Get_DataTableAsync("ExecuteAspNetUser",
                    new string[] { "@UserToolsId", "@UserToolsDescription", "@NetworkUrl", "@LocalUrl", "@ApplicationUserId", "@DateTime", "@UserToolsTechnology", "@OptId" },
                    new string[] { Convert.ToString(userTool.Id), Convert.ToString(userTool.Description), Convert.ToString(userTool.NetworkUrl),
             Convert.ToString(userTool.LocalUrl), Convert.ToString(userTool.ApplicationUsersId), DateTime.Now.ToString(), Convert.ToString(userTool.Technology), "5" });
                if (updatedUserTool != null)
                {
                    return _exec.DataTableToModel<UserTools>(updatedUserTool);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> DeleteUserToolAsync(int userToolID)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteAspNetUser",
                    new string[] { "@UserToolsId", "@OptId" },
                    new string[] { userToolID.ToString(), "7" });

                if (obj?.Rows?.Count > 0)
                {
                    var status = Convert.ToInt32(obj.Rows[0]["Status"]);
                    return status == 1;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<UserProfile> UpdateUserTeamAdminAsync(Guid userId, Guid teamAdminId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("ExecuteAspNetUser",
                    new string[] { "@UserId", "@TeamAdminId", "@OptId" },
                    new string[] { userId.ToString(), teamAdminId.ToString(), "8" });

                if (obj?.Rows?.Count > 0)
                    return _exec.DataTableToModel<UserProfile>(obj);

                return null;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        //public async Task<List<TeamAdminModel>> GetDepTeamAdminAsync(int depId)
        //{
        //    try
        //    {
        //        var obj = await _exec.Get_DataTableAsync("ExecuteAspNetUser",
        //            new string[] { "@DepartmentId", "@OptId" },
        //            new string[] { depId.ToString(), "9" });

        //        if (obj.Rows.Count > 0)
        //            return _exec.DataTableToList<TeamAdminModel>(obj);

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public async Task<CombinedUserProfile> GetAllUserProfileAsync(string userId)
        {
            try
            {
                var obj = await _exec.Get_DataSetAsync("GetCompleteUserProfile",
                    new string[] { "@UserId" },
                    new string[] { userId });

                CombinedUserProfile combinedUserProfile = new CombinedUserProfile();
                var userProfileTable = obj?.Tables[0];
                if (userProfileTable != null && userProfileTable.Rows.Count > 0)
                {
                    combinedUserProfile.UserProfile = await _exec.DataTableToModelAsync<UserProfile>(userProfileTable);
                }

                var userBadgesTable = obj?.Tables[1];
                if (userBadgesTable != null && userBadgesTable.Rows.Count > 0)
                {
                    combinedUserProfile.UserBadges = await _exec.DataTableToListAsync<UserBadges>(userBadgesTable);
                }

                return combinedUserProfile;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<List<UserProject>> GetUserProjectsAsync(string userId)
        {
            try
            {
                var obj = await _exec.Get_DataSetAsync("GetUserToolOrProject",
                    new string[] { "@UserId", "@OptId" },
                    new string[] { userId, "1" });

                var userProjectsTable = obj?.Tables[0];
                if (userProjectsTable != null && userProjectsTable.Rows.Count > 0)
                {
                    return await _exec.DataTableToListAsync<UserProject>(userProjectsTable);
                }

                return new List<UserProject>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<List<UserTools>> GetUserToolsAsync(string userId)
        {
            try
            {
                var obj = await _exec.Get_DataSetAsync("GetUserToolOrProject",
                    new string[] { "@UserId", "@OptId" },
                    new string[] { userId, "2" });

                var userToolsTable = obj?.Tables[0];
                if (userToolsTable != null && userToolsTable.Rows.Count > 0)
                {
                    return await _exec.DataTableToListAsync<UserTools>(userToolsTable);
                }

                return new List<UserTools>();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public async Task<List<DropDownResponse<int>>> GetProjectsByDepartmentIdAsync(int departmentId)
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetProjectsByDepartmentId",
                    new string[] { "@DepartmentId", "@Option" },
                    new string[] { departmentId.ToString(), "1" });

                if (obj?.Rows?.Count > 0)
                {
                    // If DataTableToList supports generics
                    return _exec.DataTableToList<DropDownResponse<int>>(obj);
                }

                // Manual mapping if DataTableToList does not support generics
                var result = new List<DropDownResponse<int>>();
                foreach (DataRow row in obj.Rows)
                {
                    result.Add(new DropDownResponse<int>
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["Name"].ToString()
                    });
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<List<DropDownResponse<int>>> GetTagsAsync()
        {
            try
            {
                var obj = await _exec.Get_DataTableAsync("GetProjectsByDepartmentId",
                    new string[] { "@DepartmentId", "@Option" },
                    new string[] { "0", "2" });

                if (obj?.Rows?.Count > 0)
                {
                    return obj.AsEnumerable().Select(row => new DropDownResponse<int>
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["Name"].ToString()
                    }).ToList();
                }
                return new List<DropDownResponse<int>>();
            }
            catch (Exception)
            {
                return new List<DropDownResponse<int>>();
            }
        }

        public async Task<UpdateUserProfileImage> UpdateProfileImageAsync(UpdateUserProfileImage updateProfileImage)
        {
            try
            {
                var dataTable = await _exec.Get_DataTableAsync("ExecuteUpdateProfileImage",
                    new string[] { "@UserId", "@ProfileImage", "@OptId" },
                    new string[] { updateProfileImage.Id, updateProfileImage.ProfileImageName, "1" });

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    return await _exec.DataTableToModelAsync<UpdateUserProfileImage>(dataTable);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public async Task<List<UserBadges>> GetUserBadgesAsync(string empId)
        //{
        //    try
        //    {
        //        var dataSet = await _exec.Get_DataSetAsync("GetEmployeeBadges",
        //            new string[] { "@EmpId" },
        //            new string[] { empId });

        //        if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
        //            return new List<UserBadges>();

        //        return await _exec.DataTableToListAsync<UserBadges>(dataSet.Tables[0]);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle the exception as needed
        //        throw new Exception("An error occurred while getting user badges.", ex);
        //    }
        //}
    }
}







