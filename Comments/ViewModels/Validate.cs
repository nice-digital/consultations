using Microsoft.AspNetCore.Mvc;

namespace Comments.ViewModels
{
    public class Validate
    {
        public Validate(bool valid, bool unauthenticated = false, bool notFound = false, string message = null, bool unauthorised = false)
        {
            Valid = valid;
            Unauthenticated = unauthenticated;
            NotFound = notFound;
            Message = message;
            Unauthorised = unauthorised;
        }

        public bool Valid { get; private set; }
        public bool Unauthenticated { get; private set; }
        public bool NotFound { get; private set; }
        public string Message { get; private set; }
		public bool Unauthorised { get; private set; }
    }
}
