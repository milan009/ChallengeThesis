
using System;

namespace OdborkyApp.Model
{
    public enum ConfirmationType
    {
        Task, Challenge
    }

    internal class ConfirmationRequest
    {
        public int UserId { get; set; }
        public Guid TaskProgressId { get; set; }
        public Guid ChallengeProgressId { get; set; }
        public int TargetId { get; set; }
        public ConfirmationType Type { get; set; }
    }

    internal class Confirmation
    {
        public int SignerId { get; set; }
        public string Signature { get; set; }
        public ConfirmationRequest Request { get; set; }
    }
}