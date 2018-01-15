using System;
using System.ComponentModel.DataAnnotations;

namespace Models.EFDB
{
    public class TaskProgress
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ChallengeProgressId { get; set; }
        public ChallengeProgress ChallengeProgress { get; set; }

  //      public int TaskId { get; set; }
        public ChallengeTask Task { get; set; }

        public MembershipCategoryId MembershipCategory { get; set; }

        public ProgressStatus Status { get; set; }

        public DateTime LastModified { get; set; }

        public static explicit operator DTO.TaskProgress(TaskProgress progress)
        {
            return new DTO.TaskProgress
            {
                Id = progress.Id,
                TaskId = progress.Task.Id,
                ChallengeProgressId = progress.ChallengeProgressId,

                LastModified = progress.LastModified,
                Status = progress.Status,
            };
        }
    }
}