using Google;

public static class GoogleSignInHelper
{
    public static GoogleSignInUser User;

    static GoogleSignInHelper()
    {
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            WebClientId = "427950668681-70ccui7mhd4tombv9dog4a7u9lm62j6k.apps.googleusercontent.com",
            RequestAuthCode = true,
            RequestIdToken = true,
            UseGameSignIn = false
        };
    }

}