using ProjectManagementSystem.Application.Common;
using ProjectManagementSystem.Application.DTOs;
using ProjectManagementSystem.Application.Interfaces;
using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Domain.Enums;
using ProjectManagementSystem.Domain.Interfaces;

namespace ProjectManagementSystem.Application.Services
{
    public sealed class ProjectService(IProjectRepository projects, IUserRepository users) : IProjectService
    {
        public async Task<IReadOnlyCollection<ProjectDto>> ListAsync(ProjectStatus? status = null, CancellationToken cancellationToken = default)
        {
            var allProjects = await projects.ListAsync(status, cancellationToken);
            return allProjects.Where(project => !project.IsDeleted).Select(project => project.ToDto()).ToArray();
        }

        public async Task<ProjectDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var project = await projects.GetByIdAsync(id, cancellationToken);
            return project is null || project.IsDeleted ? null : project.ToDto();
        }

        public async Task<Result<ProjectDto>> CreateAsync(CreateProjectRequest request, Guid? actorId = null, CancellationToken cancellationToken = default)
        {
            var manager = await users.GetByIdAsync(request.ManagerId, cancellationToken);
            if (manager is null || !manager.IsActive)
                return Result<ProjectDto>.Failure("Project manager was not found or is inactive.");

            var project = new Project(request.Name, request.Description, request.TenantId, request.ManagerId, request.StartDate, request.EndDate);
            project.Activate();
            if (actorId.HasValue)
                project.RecordCreatedBy(actorId.Value);

            await projects.AddAsync(project, cancellationToken);
            return Result<ProjectDto>.Success(project.ToDto());
        }

        public async Task<Result<ProjectDto>> UpdateAsync(Guid id, UpdateProjectRequest request, Guid? actorId = null, CancellationToken cancellationToken = default)
        {
            var project = await projects.GetByIdAsync(id, cancellationToken);
            if (project is null || project.IsDeleted)
                return Result<ProjectDto>.Failure("Project was not found.");

            project.Update(request.Name, request.Description, request.ManagerId, request.StartDate, request.EndDate);
            ApplyStatus(project, request.Status);

            if (actorId.HasValue)
                project.RecordModifiedBy(actorId.Value);

            await projects.UpdateAsync(project, cancellationToken);
            return Result<ProjectDto>.Success(project.ToDto());
        }

        private static void ApplyStatus(Project project, ProjectStatus status)
        {
            if (status == ProjectStatus.Active)
                project.Activate();
            else if (status == ProjectStatus.Completed)
                project.Complete();
            else if (status == ProjectStatus.Archived)
                project.Archive();
        }
    }
}
