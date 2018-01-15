using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Models.DTO;

namespace Models.EFDB
{
    public class Challenge
    {
        [Key]
        public int Id { get; set; }
        
        public string MaleName { get; set; }
        public string FemaleName { get; set; }
        public string Description { get; set; }

        public string ImageUri { get; set; }

        public int CategoryId { get; set; }

        public string BasicRequirements { get; set; }
        public string ExtraRequirements { get; set; }
 
        public List<ChallengeTask> Tasks { get; set; }
        public DateTime LastModified { get; set; }

        public static explicit operator DTO.Challenge(Challenge challenge)
        {
            var names = new GenderName[]
            {
                new GenderName{Gender = Gender.Male, Name = challenge.MaleName},
                new GenderName{Gender = Gender.Female, Name = challenge.FemaleName}
            };

            var bReqs = challenge.BasicRequirements.Split(',').Select(r => int.Parse(r)).ToArray();
            var basicRequirements = new ChallengeRequirements
            {
                Cubs = bReqs[0],
                Scouts = bReqs[1],
                Guides = bReqs[2],
            };
   
            var eReqs = challenge.ExtraRequirements.Split(',').Select(r => int.Parse(r)).ToArray();
            var extraRequirements = new ChallengeRequirements
            {
                Cubs = eReqs[0],
                Scouts = eReqs[1],
                Guides = eReqs[2],
            };

            return new DTO.Challenge
            {
                Id = challenge.Id,
                Category = (ChallengeCategoryEnum)challenge.CategoryId,

                Description = challenge.Description,
                Names = names,
                ImageUri = new Image(challenge.ImageUri),

                BasicRequirements = basicRequirements,
                ExtraRequirements = extraRequirements,

                BasicTasks = challenge.Tasks.Where(t => !t.Extra).Select(t => (DTO.ChallengeTask)t).ToList(),
                ExtraTasks = challenge.Tasks.Where(t => t.Extra).Select(t => (DTO.ChallengeTask)t).ToList(),
            };
        }

        public static explicit operator Challenge(DTO.Challenge challenge)
        {
            var createdChallenge = new Challenge
            {
                Id = challenge.Id,
                MaleName = challenge.Names[0].Name,
                FemaleName = challenge.Names[1].Name,
                Description = challenge.Description,

                CategoryId = (int)challenge.Category,
                ImageUri = challenge.ImageUri.Uri,
                BasicRequirements = challenge.BasicRequirements.ToString(),
                ExtraRequirements = challenge.ExtraRequirements.ToString(),

                LastModified = DateTime.UtcNow,
            };

            var bTasks = challenge.BasicTasks.Select(task => new ChallengeTask
            {
                Id = task.Id,
                Challenge = createdChallenge,
                ChallengeId = createdChallenge.Id,
                Name = task.Name,
                Description = task.Description,
                Competences = task.Competences.Aggregate("", (a, s) => a += ";" + s, res => res),
                Extra = false,
            }).ToList();

            var eTasks = challenge.ExtraTasks.Select(task => new ChallengeTask
            {
                Id = task.Id,
                Challenge = createdChallenge,
                ChallengeId = createdChallenge.Id,
                Name = task.Name,
                Description = task.Description,

                Extra = true,
            }).ToList();

            createdChallenge.Tasks = new List<ChallengeTask>(bTasks);
            createdChallenge.Tasks.AddRange(eTasks);

            return createdChallenge;
        }
    }
}