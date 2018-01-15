using System;

namespace OdborkyApp.Model
{

    public class ParsedRegisterResponse
    {
        public UserDetails Item1 { get; set; }
        public Guid Item2 { get; set; }
        public string Item3 { get; set; }

        public ParsedRegisterResponse() { }

        public ParsedRegisterResponse(UserDetails userDetails, Guid deviceGuid, byte[] privateKey)
        {
            Item1 = userDetails;
            Item2 = deviceGuid;
            Item3 = Convert.ToBase64String(privateKey);
        }
    }
    
}