using Microsoft.AspNetCore.Identity;

namespace HomeCook.Data.CustomException
{
    public class AuthException: Exception
    {
        public const string InvalidCode = "Invalid Code",
            TooManyLoginAttempts = "Too Many Login Attempts",
            UserHasNoInstitutionAssigned = "User has no Institution assigned.",
            UserAlreadyExist = "User Already Exist.",
            InvalidLoginAttempt = "Invalid Login Attempt",
            UserDoesNotExist = "User Does Not Exist",
            BadRequest = "Bad Request",
            InvalidRefreshToken = "Invalid Refresh Token";

        readonly Dictionary<string, string> errors = new Dictionary<string, string>();
        public AuthException(string errorMessage): base(String.Format(errorMessage))
        {

        }

        public AuthException(IdentityError[] identityErrors)
        {
            foreach (var err in identityErrors)
            {
                errors.Add(err.Code, err.Description);
            }
        }
    }
}
