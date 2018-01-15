using System.ComponentModel.DataAnnotations;

namespace Models.EFDB
{
    public class ChallengeTask
    {
        [Key]
        public int Id { get; set; }
  //      [Key, Column(Order = 1)]
        public int ChallengeId { get; set; }
        public Challenge Challenge { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public bool Extra { get; set; }

        public string Competences { get; set; }

        public static explicit operator DTO.ChallengeTask(ChallengeTask task)
        {
            return new DTO.ChallengeTask
            {
                Id = task.Id,

                Name = task.Name,
                Description = task.Description,

                Category = task.Extra ? DTO.ChallengeTask.TaskCategory.Extra : DTO.ChallengeTask.TaskCategory.Basic,
                Competences = task.Extra ? null : task.Competences.Split(';'),
            };
        }
    }
}