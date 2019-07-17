namespace InstaSharper.Enums
{
    public enum InstaLoginResult
    {
        Success,
        BadPassword,
        InvalidUser,
        TwoFactorRequired,
        Exception,
        ChallengeRequired,
        LimitError,
        InactiveUser,
        CheckpointLoggedOut
    }
}