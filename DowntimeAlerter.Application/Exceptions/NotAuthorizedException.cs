using System;
namespace DowntimeAlertereAlerter.Application.Exceptions
{
     public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(string userName, object key)
           : base($"{userName} is not authorized for entity {key}.")
        {
        }
    }
}