using Hackathon24.Models;

namespace Hackathon24.Helpers
{
    public class ScopeTagHelpers
    {
        //  Simulates getting scope tags for the User
        public static List<string>? GetUserScopeTags(HttpContext context)
        {
            var scopeTags = context.User.Claims.FirstOrDefault(c => c.Type == "ScopeTags")?.Value;
            return scopeTags?.Split(';').ToList();
        }

        // Returns list of scope tags that intersect the scope tags on customer object and the user
        public static List<string> GetIntersectingScopeTags(List<string> scopeTags, HttpContext context)
        {
            var userScopeTags = GetUserScopeTags(context);
            if (userScopeTags == null)
            {
                return new List<string>();
            }
            return scopeTags.Intersect(userScopeTags).ToList();
        }
    }
}
