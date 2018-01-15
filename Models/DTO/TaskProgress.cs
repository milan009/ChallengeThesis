using System;

namespace Models.DTO
{ 
    public class TaskProgress : ITaskProgress
    {
        public Guid Id { get; set; }
        public Guid ChallengeProgressId { get; set; }
        public int TaskId { get; set; }

        public ProgressStatus Status { get; set; }
        public DateTime LastModified { get; set; }
    }
}