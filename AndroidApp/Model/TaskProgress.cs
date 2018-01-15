using System;
using Models;
using SQLite;

namespace OdborkyApp.Model
{
    internal class TaskProgress : ITaskProgress
    {
        [PrimaryKey, AutoIncrement]
        public Guid Id { get; set; }
        public Guid ChallengeProgressId { get; set; }
        public int TaskId { get; set; }
        public ProgressStatus Status { get; set; }
        public DateTime LastModified { get; set; }
    }
}