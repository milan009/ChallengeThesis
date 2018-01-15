using System;

namespace Models.DTO
{
    [Serializable]
    public class ChallengeRequirements
    {
        public int Cubs, Scouts, Guides;

        public override string ToString()
        {
            return $"{Cubs},{Scouts},{Guides}";
        }
    }
}