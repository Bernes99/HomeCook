namespace HomeCook.Data.CustomException
{
    public class AuthException: Exception
    {
        public const string InvalidCode = "Invalid Code",
            TooManyLoginAttempts = "Too Many Login Attempts",
            UserHasNoInstitutionAssigned = "User has no Institution assigned.",
            UserAlreadyExist = "User Already Exist.",
            InvalidLoginAttempt = "Invalid Login Attempt",
            InvalidRefreshToken = "Invalid Refresh Token";

        public AuthException(string errorMessage): base(String.Format(errorMessage))
        {

        }
    }
}
