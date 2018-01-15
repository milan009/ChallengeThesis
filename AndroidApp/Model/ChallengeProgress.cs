using System;
using Models;
using SQLite;

namespace OdborkyApp.Model
{
    internal class ChallengeProgress : Models.IChallengeProgress
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public int ChallengeId { get; set; }
        public ProgressStatus Status { get; set; }
        public DateTime LastModified { get; set; }
    }
}