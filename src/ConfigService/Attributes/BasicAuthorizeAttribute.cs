using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConfigEditor.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class BasicAuthorizeAttribute : TypeFilterAttribute {
        public BasicAuthorizeAttribute(Type type) : base(type) {
        }
    }

    public class BasicAuthorizeFilter : IAuthorizationFilter {
        public void OnAuthorization(AuthorizationFilterContext context) {
            string au = context.HttpContext.Request.Headers["Authorization"];
            if (au != null && au.StartsWith("Basic ")) {
                var encodedUsernamePassword = au.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                try {
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                    var username = decodedUsernamePassword.Split(':', 2)[0];
                    var password = decodedUsernamePassword.Split(':', 2)[1];
                    if (IsAuthorized(username, password)) {
                        return;
                    }
                } catch (Exception _) {
                    context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic";
                    context.Result = new UnauthorizedResult();
                }
            }
            // Return authentication type (causes browser to show login dialog)
            context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Result = new UnauthorizedResult();
        }
        public bool IsAuthorized(string username, string password) {
            // Check that username and password are correct
            return username.Equals("admin", StringComparison.InvariantCultureIgnoreCase)
                   && password.Equals("admin");
        }
    }
}