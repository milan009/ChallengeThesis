using System;

namespace Models
{
    public interface IChallengeProgress
    {
         Guid Id { get; set; }
         int UserId { get; set; }
         int ChallengeId { get; set; }

         ProgressStatus Status { get; set; }
         DateTime LastModified { get; set; }
    }
}
