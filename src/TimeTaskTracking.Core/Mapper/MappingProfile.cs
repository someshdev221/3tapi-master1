using AutoMapper;
using TimeTaskTracking.Core.Dtos;
using TimeTaskTracking.Core.Dtos.Project;
using TimeTaskTracking.Models.Entities;
using TimeTaskTracking.Models.Entities.Project;
using TimeTaskTracking.Shared.ViewModels;

namespace TimeTaskTracking.Core.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<LoginResponseViewModel, RegisterDto>().ReverseMap();
        CreateMap<Register, RegisterDto>().ForMember(dest => dest.ProfileImage, opt => opt.Ignore()).ReverseMap();
        //CreateMap<Register, RegisterDto>().ForMember(dest => dest., opt => opt.Ignore());
        //;
        CreateMap<AttendanceDetailsDTO, AttendanceDetails>().ReverseMap();
        CreateMap<EmployeeDetailsDTO, EmployeeDetails>().ReverseMap();
        CreateMap<UpworkProfileDto, UpworkProfile>().ReverseMap();
        CreateMap<UpworkDetailsProfileDto, UpworkProfile>().ReverseMap();
        CreateMap<ProjectDto, Project>().ReverseMap();
        CreateMap<ProjectModule, ProjectModuleDto>().ReverseMap();
        CreateMap<EmployeeStatusDto, EmployeeStatus>().ReverseMap();
        CreateMap<EstimatedHoursDto, EstimatedHour>().ReverseMap();
        CreateMap<ProjectsModule, ProjectsModuleDto>().ReverseMap();
        CreateMap<FeedbackDetailsDto, OnRollFeedbackDetailsViewModel>().ReverseMap();
        //Client 
        CreateMap<ClientDto, Client>().ReverseMap();
        CreateMap<EmployeeDto, EmployeeViewModel>().ReverseMap();
        CreateMap<EmployeeDetailDto, EmployeeDetailsViewModel>().ReverseMap();
        //UserProfile
        CreateMap<UserProfile, UserProfileDto>().ReverseMap();
        CreateMap<UpdateUserProject, UpdateUserProfileDto>().ReverseMap();
        CreateMap<UserProject, UserProjectDto>().ReverseMap();
        CreateMap<AddUserProject, UsersProjectDto>().ReverseMap();
        CreateMap<UpdateUserWorkedProject, UpdateUserWorkedProjectDto>().ReverseMap();
        CreateMap<UserTools, UserToolsDto>().ReverseMap();
        CreateMap<UserTools, UsersToolDto>().ReverseMap();
        CreateMap<UserProfile, UserProfileDto>().ReverseMap();
        CreateMap<TeamAdminModel, TeamAdminDto>().ReverseMap();
        CreateMap<CombinedUserProfile, CombinedUserProfileResponse>();
        CreateMap<EmployeeProfileDto, EmployeeProfileViewModel>().ReverseMap();
        CreateMap<ProjectResponseDto, Project>().ReverseMap();
        CreateMap<ProjectModuleResponseDto, ProjectModule>().ReverseMap();
        CreateMap<EmployeeStatusResponseDto, Models.Entities.EmployeeStatus>().ReverseMap();
        CreateMap<AddAssignProjectDto, Project>().ReverseMap();
        CreateMap<DepartmentEmployeeProjectRequestModel, EmployeeProjectRequestModel>().ReverseMap();
        CreateMap<FeedbackForm, MonthlyFeedbackFormDto>().ReverseMap();
        CreateMap<FeedbackForm, AddOnRollFeedbackFormDto>().ReverseMap();
        CreateMap<UserBadge, UserBadgeDto>().ReverseMap();
        CreateMap<TraineeViewModel, TraineeDto>().ReverseMap();
        CreateMap<ProjectModuleReport, ProjectPaymentStatus>().ReverseMap();
        CreateMap<ProjectProductivityDto, ProjectProductivity>().ReverseMap();
        CreateMap<UpdateUserProfileImage, UpdateUserProfileImageDto>().ForMember(dest => dest.ProfileImage, opt => opt.Ignore()).ReverseMap();
        CreateMap<Invoice, InvoiceDto>().ReverseMap();
        CreateMap<InvoiceUpdateDto, InvoiceUpdate>().ReverseMap();
        CreateMap<InvoicePostDto, InvoicePost>().ReverseMap();
        CreateMap<Register, EmployeeDto>().ReverseMap();
        CreateMap<ApplicationDomain, ApplicationDomainDto>().ReverseMap();
    }
}

