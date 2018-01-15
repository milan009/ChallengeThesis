using System;

namespace Models.DTO
{
    public class ChallengeProgress : IChallengeProgress
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public int ChallengeId { get; set; }

        public ProgressStatus Status { get; set; }
        public DateTime LastModified { get; set; }
    }
}
