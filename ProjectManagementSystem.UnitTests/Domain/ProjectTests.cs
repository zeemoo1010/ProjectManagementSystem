using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Domain.Enums;
using Xunit;

namespace ProjectManagementSystem.UnitTests.Domain
{
    public class ProjectTests
    {
        [Fact]
        public void Constructor_rejects_end_date_before_start_date()
        {
            var tenantId = Guid.NewGuid();
            var managerId = Guid.NewGuid();

            Assert.Throws<ArgumentException>(() =>
                new Project("Internal API", null, tenantId, managerId, new DateOnly(2026, 5, 10), new DateOnly(2026, 5, 1)));
        }

        [Fact]
        public void Project_can_move_through_lifecycle()
        {
            var project = new Project("Internal API", null, Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));

            project.Activate();
            Assert.Equal(ProjectStatus.Active, project.Status);

            project.Complete();
            Assert.Equal(ProjectStatus.Completed, project.Status);

            project.Archive();
            Assert.Equal(ProjectStatus.Archived, project.Status);
        }

        [Fact]
        public void AddTask_does_not_duplicate_same_task()
        {
            var project = new Project("Internal API", null, Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
            var task = new TaskItem("Build auth", null, project.Id, Guid.NewGuid(), new DateOnly(2026, 5, 15), TaskPriority.High);

            project.AddTask(task);
            project.AddTask(task);

            Assert.Single(project.Tasks);
        }
    }
}
