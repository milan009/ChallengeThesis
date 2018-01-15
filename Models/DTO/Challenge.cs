using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Models.DTO
{
    [Serializable]
    public class Challenge
    {
        [XmlAttribute("id")]
        public int Id;
        
        [XmlArray("Names")]
        public GenderName[] Names;
        public string Description;

        [XmlElement("Image")]
        public Image ImageUri;

        public ChallengeCategoryEnum Category;

        public ChallengeRequirements BasicRequirements;
        public ChallengeRequirements ExtraRequirements;

        public List<ChallengeTask> BasicTasks;
        public List<ChallengeTask> ExtraTasks;
    }

    [Serializable]
    public struct ChallengeCategory
    {
        [XmlElement("Name")]
        public ChallengeCategoryEnum Enum;

        [XmlAttribute("id")]
        public int Id;
    }

    [Serializable]
    public class Image : IXmlSerializable
    {
        public string Uri;
        public string LocalPath;

        public Image()
        {
            
        }

        public Image(string uriString, string localPath = null)
        {
            Uri = uriString;
            LocalPath = localPath;
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            Uri = reader.GetAttribute("uri");
            LocalPath = reader.GetAttribute("localPath");
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("uri", Uri);
            writer.WriteAttributeString("localPath", LocalPath);
        }
    }

    [Serializable]
    public enum ChallengeCategoryEnum
    {
        Art,
        Technology,
        Camping,
        Social,
        NaturalScience,
        Watering,
        Service,
        Sports,
        Religion,
        Scouting  
    }

    [Serializable]
    public struct GenderName
    {
        [XmlText]
        public string Name;
        [XmlAttribute("gender")]
        public Gender Gender;
    }

    [Serializable]
    public enum Gender
    {
        Male, Female
    }
}