using System;
using System.Xml.Serialization;

namespace Models.DTO
{
    [Serializable]
    public class ChallengeTask
    {
        public int Id;
        public string Name, Description;

        [XmlElement("Category")]
        public TaskCategory Category;

        [XmlArrayItem(ElementName = "Competence", Type = typeof(string))]
        [XmlArray]
        public string[] Competences;

        [Serializable]
        public enum TaskCategory
        {
            Basic, Extra
        }
    }

}
