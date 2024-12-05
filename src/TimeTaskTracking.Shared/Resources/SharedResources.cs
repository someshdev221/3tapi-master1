using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using FluentValidation.Results;
using System.IdentityModel.Tokens.Jwt;
using TimeTaskTracking.Models.Entities;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using TimeTaskTracking.Shared.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using TimeTaskTracking.Models.Entities.Project;


namespace TimeTaskTracking.Shared.Resources;

public static class SharedResources
{
    //Failure messages
    public static string UserNotFound = "No user found";
    public static string TeamAdminIdNotFound = "Error while updating employee manager!";
    public static string NoDataFound = "No data found";
    public static string NoManagerFound = "No manager found";
    public static string InvalidManager = "Failed to activate the user since neither any HOD or Project Manager found for the Department";
    public static string IncorrectEmail = "Please check your email!";
    public static string IncorrectPassword = "Please check your password!";
    public static string EmailAlreadyExist = "Email already exist!";
    public static string PhoneNumberAlreadyExist = "PhoneNumber already exist!";
    public static string EmployeeNumberAlreadyExist = "EmployeeNumber already exist!";
    public static string EmailExistUserInActive = "Email address already exist but the account is currently inactive.";
    public static string TechnicalIssue = "Can't process your request right now!";
    public static string OldPasswordIsIncorrect = "Old password is incorrect!";
    public static string ErrorWhileChangingPassword = "Error while changing password!";
    public static string InValidForgotPasswordLink = "Your forgot password link has not valid!";
    public static string ExpiredForgotPasswordLink = "Your forgot password link has been expired!";
    public static string ErrorWhileResetPassword = "Error while reset password!";
    public static string NoRecordFound = "No details found!";
    public static string NoProjectFound = "No project found!";
    public static string InternalServerError = "Internal Server Error!";
    public static string UnableToChangeDepartment = "Unable to change Department";
    public static string UnableToChangeEmpNo = "Unable to change EmployeeNumber";
    public static string NoBatchFound = "No badge details found!";
    public static string TeamAdminIdOrIsActiveRequired = "Please provide project manager or active status!";
    public static string DepartmentNotFound = "Department not found.";
    public static string IsActiveStatusValidForEmployee = "Status must be InActive or Active";
    public static string TeamAdminIdAndIsActiveRequired = "Please provide project manager or active status!";
    public static string OnlyOneFieldAllowedTeamAdminIdAndIsActive = "Only one field is allowed: either 'project manager' or 'active status'"!;
    public static string NoTeamFound = "No team found";
    public static string DateError = "Approval date must be less than or equal to the deadline date.";
    public static string ProjectIsNotAssigned = "Project is not assigned to you.";
    public static string EmployeeNotFound = "Employee not found. Please check this employee.";
    public static string EmployeeNotFoundForTheDeparment = "Please Provide valid employee ID as either employees does not exists or does not belong to your department";
    public static string InvalidDepartment = "Please Provide valid employee ID as employee does not belong to your department";
    public static string ProjectNotFoundForTheDeparment = "Please Provide valid Project ID as either Project does not exists or does not belong to your department";
    public static string NoEmployeeFoundUnderTheManager = "The Employee you have provided are either not exists or not assigned to you";
    public static string ProjectSummaryNotFound = "Projects detail summary not found.";
    public static string FailedToSendWarningMails = "Failed to send warning mails.";
    public static string TeamMembersNotFound = "Team members not found.";
    public static string NoUserFound = "No user found.";
    public static string PleaseProvideEmployee = "Please provide atleast one employee.";
    public static string DescriptionIsRequired = "Description cannot be empty.";
    public static string EmployeeListIsRequired = "Employee List can not be null.";
    public static string ClientInUse = "Failed to Delete Client. Client is currently linked to projects";
    public static string ErrorWhileSavingToDo = "Error while saving to do.";
    public static string EmployeeNotAssignedToYou = "Employee is not assigned to you";
    public static string ValueIsNotValid = "Field can be Only Empty, 0 or 1";
    public static string CannotAssignToManager = "You can not submit 'to do' to Manager";
    public static string YouCanNotSubmitForOthers = "You are not authorized to submit 'to do' for other Employees";
    public static string InvalidEmployee = "Employee not found. Please provide a valid Employee";
    public static string ToDoCannotBeEmptyOrNull = "ToDo can not be empty or null";
    public static string AssignedToIdCannotBeEmptyOrNull = "'AssignedToId' cannot be empty or null.";
    public static string ToDoDeletedSuccessfully = "To Do deleted successfully.";
    public static string SomethingWentWrongWhileDeletingToDo = "'Something went wrong while deleting To Do.";
    public static string NoToDoListFound = "To Do not found.";
    public static string PriorityCanOnlyBe0Or1 = "'Priority' can only be 0 or 1";
    public static string IsActiveMustBeEitherTrueOrFalse = "'IsActive' must be either true or false.";
    public static string ManagerDoesNotExists = "Manager doesn't not exits.";
    public static string UnAuthorizedAccess = "You are not Authorized to Change the Payment Status.";
    public static string EitherModuleOrPaymentStatusCanBeUpdated = "Either PaymentStatus or ModuleStatus can be updated at once.";
    public static string ModuleOrPaymentStatusIsRequired = "Please Provide PaymentStatus or ModuleStatus while updating.";
    public static string ProjectModulePaymentStatusUpdated = "Module's payment status updated successfully";
    public static string ProjectModuleStatusUpdated = "Module's status updated successfully";
    public static string InvalidDate = "You can not update the attendance for the future dates.";
    public static string EmployeeStatusNotFoundForTheEmployee = "You can not update the attendance Status since employee status not found.";
    public static string DatesAreRequired = "Please Provide valid Date Range";
    public static string PleaseProvideTeamLeadId = "Please Provide Team Lead Id";
    public static string ProjectIsAlreadyAssigned = "Project is already assigned";
    public static string TeamLeadIdIsRequired = "Team Lead Id is required. Please provide team lead id and try again.";
    public static string InvoiceDeletedSuccessfully = "Invoice deleted successfully";
    public static string NoProjectFoundforTheManager = "The Projects that are selected are either not exists or not assigned to you.";
    public static string UnableToFindTheManager = "Unable to find the manager for the user.";
    public static string DepartmentIdIsRequired = "Department id is required";
    public static string TeamAdminIdIsRequired = "Team admin id is required";
    public static string DomainsNotFound = "Domains not found";
    public static string ProjectNotBelongsToYourDepartment = "Project not belongs to your department";
    public static string ProjectBillingHistoryNotFound = "Project billing history not found";
    public static string NoAttendanceLogFound = "No attendance log found";
    public static string InvalidBadgeId = "The user is not assigned with the provided user badge id. Please check the user badge id and try again";
    public static string InValidTeamLeadId = "Invalid team lead id. The team lead does not belong to your department";
    public static string TeamLeadNotFound = "Team lead not found. Please check the team lead id and try again.";
    public static string NotAValidTeamLeadId = "Invalid team lead Id. The team lead id provided does not belong to team lead role. Please Provide a valid team lead Id";
    public static string CanNotAssignToMultipleDepartments = "Project can not assign to multipe departments since the project is not multi department";
    public static string ProjectDepartmentsNotFound = "Project departments not found";
    public static string PleaseUnlockTheInvoice = "Please unlock the invoice in order to change the payment status";
    public static string CantDeleteModuleId = "You can not delete the module since the module is in use.";
    public static string InvalidTeamAdminId = "The team admin id provided is invalid. please provide a valid team admin id and try again";
    public static string PleaseSelectAProfile = "Please select a valid profile since the project belongs to billing category";
    public static string ProjectStatusUpdatedSuccessfully = "Project status updated successfully";
    public static string FailedToUpdateProjectStatus = "Failed To Update Project Status";
    public static string UnableToUpdateTheManager = "The selected employee is already a project manager and cannot be assigned to another project manager.";
    public static string ProjectIdMustBeGreaterThan0 = "Project id must be greater than 0";
    // Failure messages
    public static string ErrorWhileSaveUpworkProfile = "Error while saving profile!";
    public static string NoEmployeeAssignUnderThisProject = "No employee assign under this project!";
    public static string FailedToResetPassword = "Failed to reset password!";
    public static string SkillsFieldIsRequired = "Skills field is required!";
    public static string SkillSetsAreRequired = "Skill sets are required";
    public static string ApplicationDomainIsRequired = "Application Domain is Required";
    public static string PleaseProvideAtLeast3SkillSets = "Please provide atleast 3 skill sets";
    public static string ToolAlreadyExists = "Tool already exists";
    public static string PasswordDoesNotMatchWithConfirmpassword = "Password does not match with confirmpassword";
    public static string ErrorWhileUpdateUpworkProfile = "Error while updating profile!";
    public static string ErrorWhileDeleteEmployeeStatus = "Error while Deleting status!";
    public static string ErrorWhileDeleteUpworkProfile = "Error while deleting profile!";
    public static string ProjectNotFound = "Project not found!";
    public static string HiringStatus = "HiringStatus must be either Agency, Freelancer, Direct or Other Platform!";
    public static string BillingStatus = "BillingStatus must be either Fixed, Hourly or Non-Billable!";
    public static string ProjectStatus = "ProjectStatus must be either Hold, Open, or Complete!";
    public static string ErrorWhileSaveProject = "Error while saving project!";
    public static string ErrorWhileUpdateProject = "Error while updating project!";
    public static string ErrorWhileDeleteProject = "Error while deleting project!";
    public static string ErrorWhileUpdateStatus = "Error while update Status!";
    public static string ErrorWhileFetchingData = "Error while fetching data!";
    public static string ErrorWhileEditStatus = "Unable to update employee status!";
    public static string ErrorWhileAddStatus = "Unable to add employee status!";
    public static string ErrorWhileEditLeaveStatus = "Error while updated status!";
    public static string ErrorWhileFetchModuleIdData = "Error while fetching moduleId data!";
    public static string SomethingWentWrong = "Some thing went wrong. Please try again";
    public static string FailedToAssignAward = "Some thing went wrong while assigning award. Please try again";
    public static string ClientNotFound = "No client found";
    public static string NoEmployeeFound = "No employee found!";
    public static string ErrorWhileSaving = "Error while adding client";
    public static string ErrorWhileUpdating = "Error while updating employee status";
    public static string FailedToDeleteClient = "Error while deleting client";
    public static string FailedToDeleteEmployee = "Error while deleting Employee";
    public static string EmployeeStatusNull = "Bad Request: Employee status is null!";
    public static string IdRequired = "Id is required!.";
    public static string InvalidModuleIdFormat = "Invalid ModuleId format!.";
    public static string ModuleIdNotFound = "ModuleId not found!.";
    public static string InvalidApplicationUsersIdFormat = "Invalid applicationUsersId format!.";
    public static string ApplicationUsersIdNotFound = "ApplicationUsersId not found!.";
    public static string AllowedHoursExceeded = "Allowed Hours {0}:{1}";
    public static string DateRequired = "Date is required!.";
    public static string UpdateStatusWithin7Days = "Update status within 7 days!.";
    public static string HoursRequired = "UpworkHours, FixedHours, and NonBillableHours are required!.";
    public static string ModuleIdRequired = "ModuleId is required!.";
    public static string IdNotFound = "Id not found!.";
    public static string UpdatedLeaveStatus = "Updated leave status!.";
    public static string FailedToUpdateLeaveStatus = "Failed to update leave status!.";
    public static string AddStatusWithin7Days = "Add Status within the last 7 days! ";
    public static string StatusAddedSuccessfully = "Status added successfully!.";
    public static string ProjectModuleNotFound = "Project module not found!";
    public static string DataNotFound = "Data not found!.";
    public static string StatusNotFound = "Employee status not found!.";
    public static string BadRequestInvalidDateFormat = "Bad Request: Invalid date format!.";
    public static string DeleteStatusWithin7Days = "Delete status within the last 7 days!.";
    public static string DateAndUserProfileIDRequired = "Date and user id are required!.";
    public static string EmployeeStatusNotFound = "Employee status not found!.";
    public static string ModuleIDRequired = "Module id is required.";
    public static string UserProfileNotFound = "User profile not found!.";
    public static string UserNotFoundPleaseCheckYourDetailsAndTryAgain = "User not found. Please check your details and try again!.";
    public static string DateStatusMessage = "You cannot add or update status for future dates.";
    public static string InvalidRequest = "Invalid request!.";
    public static string FailedToAddUserProject = "Failed to add user project!.";
    public static string FailedToUpdateUserProject = "Failed to update user project!.";
    public static string UserProjectNotFound = "User project not found!.";
    public static string DeleteUserProjectSuccessfully = "Deleted user project successfully!.";
    public static string FailedToAddUserTool = "Failed to add user Tool!";
    public static string FailedToUpdateUserTool = "Failed to update user Tool!";
    public static string FailedToAddUser = "Failed to add user!";
    public static string PaymentStatus = "PaymentStatus must be either Pending, Hold, Complete, UpworkHourly or NonBillable!";
    public static string ModuleStatus = "ModuleStatus must be either Open,Hold or Done!";
    public static string ErrorWhileSaveProjectModule = "Error while saving project module!";
    public static string ErrorWhileUpdateProjectModule = "Error while update project module!";
    public static string ErrorWhileDeleteProjectModule = "Error while delete project module!";
    public static string AlreadyTeamMember = "Employee is already member of team!";
    public static string ErrorWhileAssignProject = "Error while assign project to team!";
    public static string UserNotFoundWithRole = "Users not found with this role!";
    public static string ErrorWhileGettingUsers = "Error while getting users!";
    public static string ProvideEmployeeList = "Please provide employee list to add in project!";
    public static string ErrorWhileGettingMembers = "Error while getting members in this project!";
    public static string ProjectMembersNotFound = "Project members are not found!";
    public static string HoursDetails = "UpworkHours, FixedHours, and NonBillableHours are required!";
    public static string ProfileNotFound = "Profile not found!";
    public static string UserInactiveMessage = "Your account is currently inactive.Please ask your manager to activate";
    public static string ProjectNotExist = "Project does not exist!";
    public static string ClientsNotFound = "No clients found!";
    public static string ProjectsNotFound = "No projects found!";
    public static string ProjectModulesNotFound = "Project modules not found!";
    public static string UpworkProfilesNotFound = "Upwork profiles not found!";
    public static string NoTechnologyFound = "No technology found!";
    public static string ErrorLeaveStatusMessage = "Error while updating leave!";
    public static string CantDeleteYourself = "You cannot delete your own account.";
    public static string ErrorWhileAssignProjectToEmployee = "Error when assigning certain employee to a project!";
    public static string ErrorWhileAssignEmployeeToTeam = "Error when assigning certain employee to team!";
    public static string NameAlreadyExist = "Name already exist!";
    public static string ToDateMustGreater = "FromDate can not be a future date!.";
    public static string ProjectModuleNotExist = "ProjectModule does not exist!";
    public static string ProjectModuleIdNotExist = "ProjectModule does not exist in that project!";
    public static string UpworkProfileNotExist = "Upwork profile does not exist!";
    public static string ClientNotExist = "Client does not exist!";
    public static string AlreadyAssignProjectToEmployee = "Project is already assigned to employee!";
    public static string AlreadyAssignEmployeeToTeam = "Employee is already team member!";
    public static string DepartmentsDoesNotMatched = "The Employee, Team Admin, and Team Lead do not belong to the same department.";
    public static string ProjectAndUserDepartmentsDoesNotMatched = "The Employee, Team Admin, and Project do not belong to the same department.";
    public static string LeaveAlreadyAppliedMessage = "You have been marked as on leave. Please contact your team leader for further assistance.";
    public static string HalfDayLeaveAlreadyAppliedMessage = "You have been marked as on half day leave, so you can add or update status for 4 hrs only. Please contact your team leader for further assistance.";
    public static string AbsentAlreadyAppliedMessage = "You have been marked as absent, so you can not add or update status. Please contact your team leader for further assistance.";
    public static string EmployeeStatusHoursLimit = "You can add up to 24 hours per day.";
    public static string NoAccessToEditUser = "You can not edit or delete this user.";
    public static string ErrorWhileDeletingBadges = "Error while deleting badges!";
    public static string ProvideValidEmployeeActiveStatus = "Please provide a valid employee status!";
    public static string EmployeeDashboardDetailsNotFound = "No details available for the employee dashboard for this month.";
    public static string ThisProjectIsAlreadyAssignedToTheUser = "This project is already assigned to the user.";
    public static string FailedToUploadFile = "Failed to upload file.";
    public static string NoFileFound = "No file found.";
    public static string FileSizeExceedsThe10MBLimit = "The uploaded file exceeds the maximum allowed size of 10 MB.";
    public static string FileSizeExceedsThe5MBLimit = "The uploaded file exceeds the maximum allowed size of 5 MB.";
    public static string OnlyJpegJpgOrPngFormatPhotosAreAllowed = "Only JPEG, JPG, or PNG format photos are allowed.";
    public static string InvalidFileFormat = "Invalid file format. Only JPG, PNG, PDF, DOCX, and XLSX files are allowed.";
    public static string ProjectsNotExist = "Project does not exist!";
    public static string DoNotAddBillableHours = "Payment status is non-billable; you do not add billable hours!";
    public static string TraineeFeedbackFormNotFound = "Feedback form not found!.";
    public static string TraineeFeedbackAlreadySubmitted = "Feedback has already been submitted for this assessment month!.";
    public static string ErrorWhileDeletingFeedbackForm = "Error while deleting feedbackForm!";
    public static string ErrorWhileUpdatingUserProfile = "Error while updating user profile. Please check your account detials!";
    public static string ErrorWhileUpdatingCanEditStatus = "Error while updating CanEditStatus!";
    public static string ModuleNotFound = "Module not found. Please Check and try again";
    public static string FailedToUpdateEmail = "Some thing went wrong while updating user email. Please try again";
    public static string InvoiceNotFound = "Invoice not found";
    public static string ReportNotFound = "No Report Found";


    public static string NoDocumentsUploaded = "No documents uploaded.";
    public static string NoBillingDetailsFound = "No billing details found";
    public static string NoEmployeeDetailsFound = "No employee details found";
    public static string NoTeamMembersFound = "No team members found";
    public static string attendanceReportsDoesNotExists = "Attendance reports does not exists!";
    public static string ErrorWhileAssignTeamLeadToTeamLead = "You can not assign team lead to another team lead!";
    public static string ProvideProjectToAssignMember = "Please provide project!";
    public static string ProvideMemberToProjectAssign = "Please provide employee to assign project!";
    public static string ProvideEmployeeToAssignTeam = "Please provide employee to assign team!";
    public static string ProvideTeamLeadToAssignTeam = "Please provide team lead to assign employee!";
    public static string FileUploadedSuccessfully = "File uploaded successfully";
    public static string AssignedProjectToEmployee = "Assigned project to employee";
    public static string AssignedEmployeeToTeam = "Assigned employee to team";
    public static string ErrorNotAuthorizeToAssignProjectToEmployee = "You are not authorize to assign project to this member.";
    public static string FailedToAddMonthlyFeedback = "Something went wrong while added MonthlyFeedback. Please try again";
    public static string PleaseProvideAValidDesignation = "Please provide a valid designation";
    public static string EmployeeNumberIsRequired = "EmployeeNumber is required";
    public static string TraineeNotUnderTeamLead = "Trainee does not exist under TeamLead!";
    public static string TraineeNotUnderManager = "Trainee does not exist under Manager!";
    public static string FailedToGetStipendOrTrainee = "Feedback can only be getting for employees with designation 'Stipend' or 'Trainee'!";
    public static string FailedToAddOrUpdateFeedbackForNonStipendOrTrainee = "Feedback can only be added or updated for employees with the designation 'Stipend' or 'Trainee'!";
    public static string ApiKeyNotProvided = "API Key was not provided.";
    public static string UnauthorizedClient = "Unauthorized client.";
    public static string FailedToUpdateMonthlyFeedback = "Something went wrong while updated MonthlyFeedback. Please try again";
    public static string FailedToUpdateStipendOrTrainee = "Feedback can only be updated for employees with designation 'Stipend' or 'Trainee'!";
    public static string CantUpdateTheFeedBackForm = "Failed to update the feed back form since the feed back form for the Month Already Exists";
    public static string FeedbackFormNotFound = "FeedbackForm not found!.";
    public static string AwardsNotFound = "Awards not found.";
    public static string NewRequestsNotFound = "New requests not found.";
    public static string InvalidProfileType = "Invalid profile type.";
    public static string InvalidPerformanceType = "Invalid performance type.";
    public static string InvalidStartSalaryType = "Invalid startSalary type.";
    public static string DateMustNotBeFutureDate = "The selected date cannot be in the future. Please choose a valid date.";
    public static string NoTraineesFound = "No trainees found.";
    public static string UserToolsNotFound = "User tools not found.";
    public static string TeamsSummaryNotFound = "Teams summary not found.";
    public static string TeamsProductivitySummaryNotFound = "Teams productivity summary not found.";
    public static string InvalidAttendanceStatus = "Invalid attendance status.";
    public static string NoEmployeesFound = "No employees found";
    public static string EmployeeIsNotUnderThisTeamLead = "Employee  is not under this team lead";
    public static string ErrorWhileUpdatingAttendanceStatus = "Error while updating attendance status";
    public static string ToDateNotAboveTodayDate = "To Date can't be greater than today's date.";
    public static string ProvideDepartmentOrTeamAdmin = "Could you please provide the department or TeamAdmin";
    public static string PaymentStatusInvoice = "PaymentStatus must be either Paid, InProcess, SendToClient or Hold!";
    public static string ErrorWhileSaveInvoice = "Error while saveing invoice!";
    public static string ErrorWhileUpdaingInvoice = "Error while updating invoice!";
    public static string InvoicesNotFound = "Invoices not found!";
    public static string ProjectModulesNotFoundForInvoice = "Project module details not found for invoice!";
    public static string UnauthorizedAccessToDeleteTheInvoice = "Unauthorized access to delete the invoice";
    public static string PaymentStatusOrLockStatusRequired = "Please provide Payment Status or Lock Status!";
    public static string OnlyOneFieldAllowedPaymentStatusOrLockStatus = "Only one field is allowed: either 'Payment Status' or 'Lock Status'"!;
    public static string CantUpdateLockStatus = "You can't change lock status of invoice!";
    public static string ClientIsAssociatedWithproject = "You can't delete this client it is associated with the project";

    public static string CantChangePaymentStatusAlreadyPaid = "The payment status for this invoice cannot be changed because it has already been paid.";
    public static string InvoiceLockedCantUpdate = "Invoice is already locked, you can't update!";
    public static string UnlockInvoiceToChangePaymentStatus = "Please Unlock the Invoice in order to change the payment status.";
    public static string TeamLeadIDNotFound = "Team lead id not found!";
    public static string InvoiceLockNotDeleted = "Invoice is already locked, you can't Delete !";
    public static string ErrorWhileDeleteInvoice = "Error while deleteing invoice!";
    public static string SalesProsonsNotFound = "Sales persons not found!";
    public static string PleaseSelectMultipleDepartments = "Please provide atleast 2 departments since the project is multi department";
    public static string PaymentStatusInProcessToPaid = "You cannot change the PaymentStatus to Paid because the invoice has not been sent to the client yet!";
    public static string BDMPaymentStatusToPaidException = "You can't change PaymentStatus to paid of invoice!";
    public static string CantDeleteInvoice = "You can't delete this invoice because it has already been sent to the client or marked as paid!";
    public static string CantEditInvoice = "You can't edit this invoice because it has already been sent to the client or marked as paid!";
    public static string EmployeesNotFound = "Employees not found!";
    public static string ErrorWhileDeleting = "Failed to delete the Client since the client is assigned to projects!";
    public static string ErrorwhileDeletingThisUpworkProfile = "Failed to delete the UpworkProfile since the upworkprofile is assigned to projects!";


    //Success messages
    public static string LoggedIn = "Logged in successfully";
    public static string UserRegistered = "Registered success. Your account requires approval from your project manager, We'll notify you once your account has been approved.";
    public static string PasswordChangedSuccessfully = "Password changed successfully.";
    public static string EmailSent = "We've emailed you a link to reset your password.";
    public static string ValidToken = "Valid link.";
    public static string PasswordResetSuccess = "Your Password has been reset.";
    public static string TlStatus = "Success";
    public static string AttendanceStatus = "Attendance Status updated successfully.";
    public static string EmailSendMessage = "Email send successfully.";
    public static string DeleteMessage = "EmployeeStatus deleted successfully";
    public static string EmployeeStatusData = "Data fetched successfully";
    public static string LeaveStatusMessage = "Leave status updated.";
    public static string CheckStatusDate = "You can not add or update status for this date.";
    public static string DeleteStatusMessage = "Delete status with in  7 days";
    public static string ClientAddedSuccessFully = "Client added successfully.";
    public static string ClientRemoved = "Client deleted successfully.";
    public static string ClientDetailsUpdatedSuccessfully = "Client details updated successfully.";
    public static string EmployeeRemoved = "Employee deleted successfully.";
    public static string EmployeeRemovedFromList = "Employee removed successfully.";
    public static string EmployeeStatusUpdated = "Employee status updated successfully.";
    public static string EmployeeManagerAndStatusUpdated = "Employee manager and status updated successfully.";
    public static string StatusUpdatedSuccessfully = "EmployeeStatus updated successfully.";
    public static string TeamAssignedSuccess = "Team member added successfully.";
    public static string ProjectAssignedSuccess = "Project assigned successfully.";
    public static string StatusDeletedSuccessfully = "Status deleted successfully";
    public static string EmployeeStatusNotFoundOrDeleted = "Employee status not found or already deleted";
    public static string ProfileUpdatedSuccessfully = "Profile updated successfully";
    public static string UserProjectAddedSuccessfully = "User project added successfully.";
    public static string UserProjectUpdatedSuccessfully = "User project updated successfully.";
    public static string ToolAddedSuccessfully = "Tool added successfully";
    public static string ToolUpdatedSuccessfully = "Tool updated successfully";
    public static string DeletedUserToolSuccessfully = "Deleted user tool successfully";
    public static string UpdatedUserSuccessfully = "Updated User successfully";
    public static string SaveMessage = "Created successfully.";
    public static string UpdatedMessage = "Updated successfully.";
    public static string DeletedMessage = "Deleted successfully.";
    public static string ProjectsRetrievedSuccessfully = "Projects retrieved successfully.";
    public static string RetrievedSuccessfully = "Retrieved successfully.";
    public static string EmployeeDetailsUpdated = "Employee details updated successfully.";
    public static string SuccessfullyAddedAward = "Award assigned successfully.";
    public static string BadgesDeleteMessage = "Badges deleted successfully";
    public static string ProjectManagerAssigned = "Project Manager assigned successfully.";
    public static string SuccessfullyAddedMonthlyFeedback = "MonthlyFeedback added successfully.";
    public static string SuccessfullyUpdatedMonthlyFeedback = "MonthlyFeedback updated successfully.";
    public static string FeedbackFormDeleteMessage = "FeedbackForm deleted successfully";
    public static string AddFeedbackFormForSixMonths = "In order to submit an OnRollFeedback form, you need to submit at least 6 months of feedback.";
    public static string WarningEmailsSentSuccessfully = "Warning mails sent successfully.";
    public static string ToDoAddedSuccessfully = "To Do saved successfully.";
    public static string CanEditStatusUpdated = "CanEditStatus updated successfully.";
    public static string EmailUpdatedSuccessfully = "Email updated successfully.";
    public static string RoleNotAllowed = "Not a valid role to perform this action.";

    public static bool IsValidEnumValue<T>(T value, T[] allowedValues) where T : IComparable
    {
        return Array.Exists(allowedValues, element => element.CompareTo(value) == 0);
    }
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return false;
        }

        // Regular expression for basic email validation
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        return emailRegex.IsMatch(email);
    }
    public static string HashPassword(string password)
    {
        byte[] salt;
        byte[] buffer2;
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }
        using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
        {
            salt = bytes.Salt;
            buffer2 = bytes.GetBytes(0x20);
        }

        byte[] dst = new byte[0x31];
        Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
        Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
        return Convert.ToBase64String(dst);
    }
    //Decrypt Password
    public static bool VerifyHashedPassword(string hashedPassword, string password)
    {
        byte[] buffer4;
        if (hashedPassword == null)
        {
            return false;
        }
        if (password == null)
        {
            throw new ArgumentNullException("password");
        }
        byte[] src = Convert.FromBase64String(hashedPassword);
        if ((src.Length != 0x31) || (src[0] != 0))
        {
            return false;
        }
        byte[] dst = new byte[0x10];
        Buffer.BlockCopy(src, 1, dst, 0, 0x10);
        byte[] buffer3 = new byte[0x20];
        Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
        using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
        {
            buffer4 = bytes.GetBytes(0x20);
        }
        return ByteArraysEqual(buffer3, buffer4);
    }
    public static bool ByteArraysEqual(byte[] a1, byte[] a2)
    {
        if (a1 == a2)
            return true;

        if (a1 == null || a2 == null)
            return false;

        if (a1.Length != a2.Length)
            return false;

        for (int i = 0; i < a1.Length; i++)
        {
            if (a1[i] != a2[i])
                return false;
        }
        return true;
    }
    public static ResponseModel<List<EnumResponseStatus<Tkey>>> GetEnumData<T, Tkey>() where T : Enum
    {
        T[] statusValues = (T[])Enum.GetValues(typeof(T));
        var dataList = new List<EnumResponseStatus<Tkey>>();

        foreach (T status in statusValues)
        {
            if (typeof(Tkey) == typeof(int))
            {
                dataList.Add(new EnumResponseStatus<Tkey>
                {
                    Value = (Tkey)Convert.ChangeType(Convert.ToInt32(status), typeof(Tkey)),
                    Text = SharedResources.GetEnumDisplayName(status)
                });
            }
            else if (typeof(Tkey) == typeof(string))
            {
                dataList.Add(new EnumResponseStatus<Tkey>
                {
                    Value = (Tkey)Convert.ChangeType(status.ToString(), typeof(Tkey)),
                    Text = SharedResources.GetEnumDisplayName(status)
                });
            }
            else
            {
                throw new InvalidOperationException("Unsupported key type");
            }
        }

        return new ResponseModel<List<EnumResponseStatus<Tkey>>>()
        {
            Model = dataList
        };
    }
    //Method to get the display name of an enum value
    public static string GetEnumDisplayName(Enum enumValue)
    {
        Type enumType = enumValue.GetType();
        MemberInfo[] memberInfo = enumType.GetMember(enumValue.ToString());
        if (memberInfo != null && memberInfo.Length > 0)
        {
            var displayAttribute = memberInfo[0].GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                return displayAttribute.Name;
            }
        }
        return enumValue.ToString();
    }
    public static readonly Random _random = new Random();
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
    public static async Task<string> SaveProfileImage(IFormFile profileImage, IWebHostEnvironment webHostEnvironment, ILogger logger)
    {
        var webRootPath = webHostEnvironment.WebRootPath;
        var originalFileName = Path.GetFileNameWithoutExtension(profileImage.FileName);
        var extension = Path.GetExtension(profileImage.FileName);
        var randomString = SharedResources.GenerateRandomString(5);
        var newFileName = $"{originalFileName}-{randomString}{extension}";
        var directoryPath = Path.Combine(webRootPath, "images");
        var filePath = Path.Combine(directoryPath, newFileName);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            logger.LogInformation($"Created directory {directoryPath}");
        }
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await profileImage.CopyToAsync(stream);
            logger.LogInformation($"File {newFileName} saved successfully");
        }
        var fileUrl = $"/images/{newFileName}";
        logger.LogInformation($"File can be accessed via URL: {fileUrl}");
        return newFileName;
    }

    //Delete Profile Image
    public static void DeleteProfileImage(string imageName, IWebHostEnvironment webHostEnvironment, ILogger logger)
    {
        var webRootPath = webHostEnvironment.WebRootPath;
        var directoryPath = Path.Combine(webRootPath, "images");
        var filePath = Path.Combine(directoryPath, imageName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    //Delete Upload Files
    public static void DeleteProjectFiles(int projectId, string webRootPath)
    {
        var projectDirectory = Path.Combine(webRootPath, "images", "Documents", projectId.ToString());
        if (Directory.Exists(projectDirectory))
        {
            Directory.Delete(projectDirectory, true);
        }
    }

    public static async Task<int> GetWorkingDaysCount(int month, int year)
    {
        var currentDate = DateTime.UtcNow;
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        int totalDays;
        if (currentDate.Year == year && currentDate.Month == month)
        {
            endDate = currentDate;
            totalDays = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(offset => startDate.AddDays(offset))
                .Where(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                .Count();
        }
        else
        {
            totalDays = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(offset => startDate.AddDays(offset))
                .Where(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                .Count();
        }
        return totalDays;
    }

    public static async Task<UserIdentityModel> GetDepartmentFromClaims(HttpContext? context, int departmentId, string userId)
    {
        var departmentIdFromClaims = "";
        var claims = context.User.Claims;
        var user = context.User;
        var role = string.Empty;
        var teamAdminID = string.Empty;
        if (user.IsInRole("Team Lead") || user.IsInRole("Project Manager") || user.IsInRole("BDM") || user.IsInRole("Employee"))
        {
            departmentIdFromClaims = claims.FirstOrDefault(c => c.Type == "departmentId")?.Value;
            userId = claims.FirstOrDefault(c => c.Type == "id")!.Value;
            role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
        else if (user.IsInRole("HR") || user.IsInRole("Account") || user.IsInRole("HOD") || user.IsInRole("Admin"))
        {
            teamAdminID = claims.FirstOrDefault(c => c.Type == "id")!.Value;
            if (user.IsInRole("HOD"))
                departmentIdFromClaims = claims.FirstOrDefault(c => c.Type == "departmentId")?.Value;
            else
                departmentIdFromClaims = "0";
            role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
        var LoggedInUserId = claims.FirstOrDefault(c => c.Type == "id")!.Value;
        var result = new UserIdentityModel
        {
            UserId = userId,
            DepartmentId = Convert.ToInt32(departmentIdFromClaims) == 0 ? departmentId : Convert.ToInt32(departmentIdFromClaims),
            Role = role,
            LoggedInUserId = LoggedInUserId
        };
        return result;
    }

    public static async Task<UserIdentityModel> GetUserIdFromClaimsForHOD(HttpContext? context)
    {
        var departmentIdFromClaims = "";
        var claims = context.User.Claims;
        var user = context.User;
        var role = string.Empty;
        var userId = string.Empty;
         if ( user.IsInRole("HOD") || user.IsInRole("Admin") || user.IsInRole("HR"))
        {
            userId = claims.FirstOrDefault(c => c.Type == "id")!.Value;
            departmentIdFromClaims = claims.FirstOrDefault(c => c.Type == "departmentId")?.Value;
            role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
        var result = new UserIdentityModel
        {
            UserId = userId,
            DepartmentId = Convert.ToInt32(departmentIdFromClaims),
            Role = role
        };
        return result;
    }

    public static UserIdentityModel GetUserIdFromToken(HttpRequest request)
    {
        var token = request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        var userId = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var departmentId = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "departmentId")?.Value;

        return new UserIdentityModel { UserId = userId, DepartmentId = Convert.ToInt32(departmentId) };
    }

    public static async Task<ResponseModel<object>> FluentValidationResponse(List<ValidationFailure> validationFailure)
    {
        var messages = validationFailure.Select(a => a.ErrorMessage).ToList();
        return new ResponseModel<object> { Message = validationFailure.Select(a => a.ErrorMessage).ToList() };
    }
    public static async Task<string> GetForgotEmailTemplate()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        var filePath = Path.Combine(currentDirectory, "ViewModels", "Utility", "ForgotEmailTemplate.html");
        string emailTemplate;
        using (StreamReader reader = new StreamReader(filePath))
        {
            emailTemplate = reader.ReadToEnd();
        }
        return emailTemplate;
    }

    public static string GetAttendanceStatusDescription(string status)
    {
        switch (status)
        {
            case "P":
                return "Present";
            case "AB":
                return "Absent";
            case "L":
                return "Leave";
            case "HA":
                return "Half-Day";
            case "HL":
                return "Holiday Leave";
            default:
                return "Unknown Status";
        }
    }
    public static async Task<string> GetAttendenceUpdatedEmailTemplate()
    {
        try
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var filePath = Path.Combine(currentDirectory, "ViewModels", "Utility", "AttendenceStatusUpdatedTemplate.html");
            string emailTemplate;
            using (StreamReader reader = new StreamReader(filePath))
            {
                emailTemplate = reader.ReadToEnd();
            }
            return emailTemplate;
        }
        catch (Exception)
        {

            throw;
        }
        
    }

    public static async Task<DateValidationResult> ValidateReportDateFilter(DateTime? fromDate, DateTime? toDate)
    {
        toDate = toDate ?? DateTime.Today;
        fromDate = fromDate ?? new DateTime(toDate.Value.Year, toDate.Value.Month, 1);

        bool isValid = fromDate <= toDate;

        return new DateValidationResult
        {
            IsValid = isValid,
            FromDate = fromDate.Value,
            ToDate = toDate.Value
        };
    }
    public static string StrongPasswordValidation(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return "Password is required.";
        }

        if (password.Length < 8)
        {
            return "Password must be at least 8 characters long.";
        }

        if (!password.Any(char.IsUpper))
        {
            return "Password must contain at least one uppercase letter.";
        }

        if (!password.Any(char.IsLower))
        {
            return "Password must contain at least one lowercase letter.";
        }

        if (!password.Any(char.IsDigit))
        {
            return "Password must contain at least one digit.";
        }

        if (!Regex.IsMatch(password, @"[\W_]"))
        {
            return "Password must contain at least one special character.";
        }

        return string.Empty;
    }
    public static string GenerateInvoiceProjectID(string department, BillingType projectType, int projectCount)
    {
        var departmentCode = GetDepartmentCode(department);
        string prefix = "CS";
        string projectTypeCode = projectType switch
        {
            BillingType.Fixed => "FX",
            BillingType.Hourly => "HR",
            BillingType.NonBillable => "NB",
            _ => throw new ArgumentException("Invalid project type")
        };

        string year = DateTime.Now.ToString("yy");
        string month = DateTime.Now.ToString("MM");

        string projectCountString = projectCount.ToString("D2");

        return $"{prefix}{departmentCode}{projectTypeCode}{year}{month}{projectCountString}";
    }
    private static string GetDepartmentCode(string department)
    {
        return department switch
        {
            "Dot Net" => "MS",
            //"PHP" => "OS",
            //"Mobile" => "MO",
            //"Online Marketing" => "OM",
            //"IT" => "IT",
            "React" => "OS",
            "Angular" => "MO",
            "Blazor" => "OM",
            "Next" => "IT",
            _ => throw new ArgumentException("Invalid department")
        };
    }
}
