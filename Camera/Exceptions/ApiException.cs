using System;

namespace Camera.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string content)
            : base(content)
        {

        }
    }

}