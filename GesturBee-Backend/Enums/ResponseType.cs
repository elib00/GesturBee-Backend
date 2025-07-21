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
        ClassNotFound,
        TeacherNotFound,
        ClassNameAlreadyTaken,
        StudentAddedToClassroom,
        StudentAlreadyInvited,
        EnrollmentRequestAlreadySent,
        StudentInviteSuccessful,
        EnrollmentRequestSuccessful,
        ClassCreated,
        EnrollmentAcceptanceSuccessful,
        EnrollmentRejectionSuccessful,
        InvitationAcceptanceSuccessful,
        InvitationRejectionSuccessful,
        StudentRemovalSuccessful,
        ExerciseNotFound,
        ExerciseContentCreated,
        NoNullValues,
        ResourceGroupCreated,
        ClassExerciseAlreadyExists,
        ClassExerciseSuccessfullyCreated,
        EnrollmentForbidden,
        IncorrectClassCode,

        //for roadmap
        LevelNotFound,
        LevelCompleted,
        ExerciseCreationSuccessful,
        ExerciseItemEditSuccessful,
        RoadmapProgressNotFound,
        RoadmapProgressEditSuccessful,
    }
}
