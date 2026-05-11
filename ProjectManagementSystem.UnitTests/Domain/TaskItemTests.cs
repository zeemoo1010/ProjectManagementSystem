using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Domain.Enums;
using DomainTaskStatus = ProjectManagementSystem.Domain.Enums.TaskStatus;
using Xunit;

namespace ProjectManagementSystem.UnitTests.Domain
{
    public class TaskItemTests
    {
        [Fact]
        public void IsOverdue_ignores_completed_tasks()
        {
            var task = new TaskItem("Design endpoint", null, Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 4, 1), TaskPriority.High);

            task.ChangeStatus(DomainTaskStatus.Completed);

            Assert.False(task.IsOverdue(new DateOnly(2026, 4, 28)));
        }

        [Fact]
        public void Changing_status_to_completed_sets_completion_timestamp()
        {
            var task = new TaskItem("Design endpoint", null, Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 4, 30), TaskPriority.High);

            task.ChangeStatus(DomainTaskStatus.Completed);

            Assert.NotNull(task.CompletedAt);
        }
    }
}
