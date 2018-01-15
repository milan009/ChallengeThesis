using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.EFDB
{
    public class ChallengeProgress : IChallengeProgress
    {
        [Key]
        public Guid Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ChallengeId { get; set; }
        public Challenge Challenge { get; set; }

        public ProgressStatus Status { get; set; }

        public ICollection<TaskProgress> TasksProgresses { get; set; }

        public DateTime LastModified { get; set; }

        public static explicit operator DTO.ChallengeProgress(ChallengeProgress progress)
        {
            return new DTO.ChallengeProgress
            {
                ChallengeId = progress.ChallengeId,
                Id = progress.Id,
                LastModified = progress.LastModified,
                Status = progress.Status,
                UserId = progress.UserId
            };
        }

        public static explicit operator ChallengeProgress(DTO.ChallengeProgress progress)
        {
            return new ChallengeProgress
            {
                ChallengeId = progress.ChallengeId,
                Id = progress.Id,
                LastModified = progress.LastModified,
                Status = progress.Status,
                UserId = progress.UserId,
            };
        }
    }
}
