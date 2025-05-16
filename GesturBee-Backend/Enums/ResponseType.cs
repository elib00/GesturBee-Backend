namespace GesturBee_Backend.Enums
{
    public enum ResponseType
    {
        ValidUser,
        InvalidUser,
        UserAlreadyExists,
        SuccessfulRetrievalOfResource,
        UserCreated,
        MissingInput,
        UserNotFound,

        //for pw reset token
        ValidToken,
        InvalidToken,
        TokenMissingRequiredClaim,

        //for pw
        PasswordResetSuccessful,
        ResetPasswordMismatch,
        
        //for profile
        ProfileNotFound,
        ProfileUpdated,
        
        //for external auth
        InvalidGoogleToken,
        InvalidFacebookToken,

        //for classroom
        StudentNotFound, 
        ClassroomNotFound,
        TeacherNotFound,
        ClassNameAlreadyTaken,
        StudentAddedToClassroom,
        StudentAlreadyInvited,
        StudentInviteSuccessful,
        ClassCreated,
    }
}
