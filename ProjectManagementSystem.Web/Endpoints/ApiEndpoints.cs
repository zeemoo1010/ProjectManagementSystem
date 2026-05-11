using System.Security.Claims;
using ProjectManagementSystem.Application.Common;
using ProjectManagementSystem.Application.DTOs;
using ProjectManagementSystem.Application.Interfaces;
using ProjectManagementSystem.Domain.Enums;

namespace ProjectManagementSystem.API.Endpoints
{
    public static class ApiEndpoints
    {
        public static WebApplication MapProjectManagementEndpoints(this WebApplication app)
        {
            app.MapHealthChecks("/health");

            app.MapGet("/", () => Results.Ok(ApiResponse<object>.Success(new
            {
                name = "ProjectManagementSystemAPI",
                status = "running",
                docs = "/swagger",
                endpoints = new[] { "/auth/login", "/users", "/projects", "/tasks", "/comments", "/reports/summary", "/health" }
            }))).AllowAnonymous().WithTags("System");

            app.MapPost("/auth/login", async (LoginRequest request, IAuthService auth, CancellationToken cancellationToken) =>
            {
                var result = await auth.LoginAsync(request, cancellationToken);
                return result.Succeeded
                    ? Results.Ok(result.ToApiResponse())
                    : Results.Unauthorized();
            }).AllowAnonymous().WithTags("Authentication").WithName("Login");

            MapUsers(app);
            MapProjects(app);
            MapTasks(app);
            MapComments(app);
            MapReports(app);

            return app;
        }

        private static void MapUsers(WebApplication app)
        {
            var users = app.MapGroup("/users").RequireAuthorization().WithTags("Users");

            users.MapGet("/", async (IUserService service, CancellationToken cancellationToken) =>
                Results.Ok(ApiResponse<IReadOnlyCollection<UserDto>>.Success(await service.ListAsync(cancellationToken))))
                .RequireAuthorization("AdminOnly");

            users.MapGet("/{id:guid}", async (Guid id, IUserService service, CancellationToken cancellationToken) =>
                await service.GetAsync(id, cancellationToken) is { } user
                    ? Results.Ok(ApiResponse<UserDto>.Success(user))
                    : Results.NotFound(ApiResponse<UserDto>.Failure("User was not found.")));

            users.MapPost("/", async (CreateUserRequest request, ClaimsPrincipal principal, IUserService service, CancellationToken cancellationToken) =>
                ToHttpResult(await service.CreateAsync(request, CurrentUserId(principal), cancellationToken), StatusCodes.Status201Created))
                .RequireAuthorization("AdminOnly");

            users.MapPut("/{id:guid}", async (Guid id, UpdateUserRequest request, ClaimsPrincipal principal, IUserService service, CancellationToken cancellationToken) =>
                ToHttpResult(await service.UpdateAsync(id, request, CurrentUserId(principal), cancellationToken)))
                .RequireAuthorization("AdminOnly");

            users.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal principal, IUserService service, CancellationToken cancellationToken) =>
            {
                var actorId = CurrentUserId(principal);
                return actorId.HasValue
                    ? ToHttpResult(await service.DeleteAsync(id, actorId.Value, cancellationToken))
                    : Results.Forbid();
            }).RequireAuthorization("AdminOnly");
        }

        private static void MapProjects(WebApplication app)
        {
            var projects = app.MapGroup("/projects").RequireAuthorization().WithTags("Projects");

            projects.MapGet("/", async (ProjectStatus? status, IProjectService service, CancellationToken cancellationToken) =>
                Results.Ok(ApiResponse<IReadOnlyCollection<ProjectDto>>.Success(await service.ListAsync(status, cancellationToken))));

            projects.MapGet("/{id:guid}", async (Guid id, IProjectService service, CancellationToken cancellationToken) =>
                await service.GetAsync(id, cancellationToken) is { } project
                    ? Results.Ok(ApiResponse<ProjectDto>.Success(project))
                    : Results.NotFound(ApiResponse<ProjectDto>.Failure("Project was not found.")));

            projects.MapPost("/", async (CreateProjectRequest request, ClaimsPrincipal principal, IProjectService service, CancellationToken cancellationToken) =>
                ToHttpResult(await service.CreateAsync(request, CurrentUserId(principal), cancellationToken), StatusCodes.Status201Created))
                .RequireAuthorization("ProjectWrite");

            projects.MapPut("/{id:guid}", async (Guid id, UpdateProjectRequest request, ClaimsPrincipal principal, IProjectService service, CancellationToken cancellationToken) =>
                ToHttpResult(await service.UpdateAsync(id, request, CurrentUserId(principal), cancellationToken)))
                .RequireAuthorization("ProjectWrite");
        }

        private static void MapTasks(WebApplication app)
        {
            var tasks = app.MapGroup("/tasks").RequireAuthorization().WithTags("Tasks");

            tasks.MapGet("/", async (Guid? projectId, Guid? userId, TaskPriority? priority, ITaskService service, CancellationToken cancellationToken) =>
                Results.Ok(ApiResponse<IReadOnlyCollection<TaskDto>>.Success(await service.ListAsync(projectId, userId, priority, cancellationToken))));

            tasks.MapGet("/{id:guid}", async (Guid id, ITaskService service, CancellationToken cancellationToken) =>
                await service.GetAsync(id, cancellationToken) is { } task
                    ? Results.Ok(ApiResponse<TaskDto>.Success(task))
                    : Results.NotFound(ApiResponse<TaskDto>.Failure("Task was not found.")));

            tasks.MapPost("/", async (CreateTaskRequest request, ClaimsPrincipal principal, ITaskService service, CancellationToken cancellationToken) =>
                ToHttpResult(await service.CreateAsync(request, CurrentUserId(principal), cancellationToken), StatusCodes.Status201Created))
                .RequireAuthorization("TaskWrite");

            tasks.MapPut("/{id:guid}", async (Guid id, UpdateTaskRequest request, ClaimsPrincipal principal, ITaskService service, CancellationToken cancellationToken) =>
                ToHttpResult(await service.UpdateAsync(id, request, CurrentUserId(principal), cancellationToken)))
                .RequireAuthorization("TaskWrite");
        }

        private static void MapComments(WebApplication app)
        {
            var comments = app.MapGroup("/comments").RequireAuthorization().WithTags("Comments");

            comments.MapGet("/{id:guid}", async (Guid id, ICommentService service, CancellationToken cancellationToken) =>
                await service.GetAsync(id, cancellationToken) is { } comment
                    ? Results.Ok(ApiResponse<CommentDto>.Success(comment))
                    : Results.NotFound(ApiResponse<CommentDto>.Failure("Comment was not found.")));

            comments.MapGet("/task/{taskItemId:guid}", async (Guid taskItemId, ICommentService service, CancellationToken cancellationToken) =>
                Results.Ok(ApiResponse<IReadOnlyCollection<CommentDto>>.Success(await service.ListByTaskAsync(taskItemId, cancellationToken))));

            comments.MapPost("/", async (CreateCommentRequest request, ClaimsPrincipal principal, ICommentService service, CancellationToken cancellationToken) =>
                ToHttpResult(await service.CreateAsync(request, CurrentUserId(principal), cancellationToken), StatusCodes.Status201Created))
                .RequireAuthorization("TaskWrite");

            comments.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal principal, ICommentService service, CancellationToken cancellationToken) =>
            {
                var actorId = CurrentUserId(principal);
                return actorId.HasValue
                    ? ToHttpResult(await service.DeleteAsync(id, actorId.Value, cancellationToken))
                    : Results.Forbid();
            }).RequireAuthorization("ProjectWrite");
        }

        private static void MapReports(WebApplication app)
        {
            app.MapGet("/reports/summary", async (IReportService service, CancellationToken cancellationToken) =>
                Results.Ok(ApiResponse<ReportSummary>.Success(await service.GetSummaryAsync(cancellationToken))))
                .RequireAuthorization("ReportsRead")
                .WithTags("Reports");
        }

        private static Guid? CurrentUserId(ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var userId) ? userId : null;
        }

        private static IResult ToHttpResult<T>(Result<T> result, int successStatusCode = StatusCodes.Status200OK)
        {
            if (!result.Succeeded)
                return Results.BadRequest(ApiResponse<T>.Failure(result.Error ?? "The operation failed."));

            return successStatusCode == StatusCodes.Status201Created
                ? Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status201Created)
                : Results.Ok(result.ToApiResponse());
        }
    }
}
