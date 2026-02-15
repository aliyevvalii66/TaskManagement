using AutoMapper;
using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.DTOs.User;
using DomainUser = TaskManagement.Domain.Entities.User;
using DomainProject = TaskManagement.Domain.Entities.Project;
using DomainTask = TaskManagement.Domain.Entities.Task;
using DomainTaskComment = TaskManagement.Domain.Entities.TaskComment;
using DomainProjectMember = TaskManagement.Domain.Entities.ProjectMember;
using DomainActivityLog = TaskManagement.Domain.Entities.ActivityLog;
namespace TaskManagement.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DomainUser, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, DomainUser>();
            CreateMap<UpdateUserDto, DomainUser>();

            CreateMap<DomainProject, ProjectDto>().ReverseMap();
            CreateMap<CreateProjectDto, DomainProject>();
            CreateMap<UpdateProjectDto, DomainProject>();

            CreateMap<DomainTask, TaskManagement.Application.DTOs.Task.TaskDto>().ReverseMap();
            CreateMap<TaskManagement.Application.DTOs.Task.CreateTaskDto, DomainTask>();
            CreateMap<TaskManagement.Application.DTOs.Task.UpdateTaskDto, DomainTask>();

            CreateMap<DomainTaskComment, TaskManagement.Application.DTOs.TaskComment.TaskCommentDto>().ReverseMap();
            CreateMap<TaskManagement.Application.DTOs.TaskComment.CreateTaskCommentDto, DomainTaskComment>();
            CreateMap<Application.DTOs.TaskComment.UpdateTaskCommentDto, DomainTaskComment>();

            CreateMap<DomainProjectMember, TaskManagement.Application.DTOs.ProjectMember.ProjectMemberDto>().ReverseMap();
            CreateMap<TaskManagement.Application.DTOs.ProjectMember.AddProjectMemberDto, DomainProjectMember>();

            CreateMap<DomainActivityLog, TaskManagement.Application.DTOs.ActivityLog.ActivityLogDto>().ReverseMap();
            CreateMap<TaskManagement.Application.DTOs.ActivityLog.CreateActivityLogDto, DomainActivityLog>();
        }
    }
}
