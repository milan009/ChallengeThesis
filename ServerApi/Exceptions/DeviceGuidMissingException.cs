using System;

namespace ServerApi.Exceptions
{
    public class DeviceGuidMissingException : Exception
    {
        public DeviceGuidMissingException() : 
            base("The required \"DeviceGuid\" header was not found in the request!") {}
    }
}