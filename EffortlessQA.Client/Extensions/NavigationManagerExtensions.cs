using Microsoft.AspNetCore.Components;

namespace EffortlessQA.Client.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static T GetQueryParameter<T>(this NavigationManager navigationManager, string key)
        {
            var uri = new Uri(navigationManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var value = query[key];

            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }
    }
}
