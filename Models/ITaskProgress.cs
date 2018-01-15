using System;

namespace Models
{
    public interface ITaskProgress
    {
        Guid Id { get; set; }
        Guid ChallengeProgressId { get; set; }
        int TaskId { get; set; }

        ProgressStatus Status { get; set; }
        DateTime LastModified { get; set; }
    }
}
