namespace EffortlessQA.Client.Services
{
    public static class ApiConstants
    {
        public const string BaseUrl = "https://your-api-base-url.com/"; // Replace with your base URL

        public static class Endpoints
        {
            public const string Login = "login";
            public const string Register = "register";

            public const string GetDefects = "Defects";
            public const string CreateDefect = "Defects";
            public const string DeleteDefect = "Defects";

            public const string GetProjects = "Projects";
            public const string CreateProject = "Projects";

            // Add other endpoints as needed
        }
    }
}
