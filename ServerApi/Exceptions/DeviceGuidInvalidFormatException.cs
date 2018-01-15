using System;

namespace ServerApi.Exceptions
{
    public class DeviceGuidInvalidFormatException : Exception
    {
        public DeviceGuidInvalidFormatException() : 
            base("The value of \"DeviceGuid\" header is not a valid GUID!") {}
    }
}