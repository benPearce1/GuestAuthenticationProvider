using System;
using Nancy;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;
using Octopus.Server.Extensibility.Authentication.Guest.Web;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;

namespace Octopus.Server.Extensibility.Authentication.Guest
{
    public class GuestApi : NancyModule
    {
        public const string ApiUsersAuthenticate = "/api/users/authenticate/guest";

        public GuestApi(
            Func<WhenEnabledActionInvoker<UserLoginAction, IGuestConfigurationStore>> userLoginActionFactory)
        {
            Post[ApiUsersAuthenticate] = o => userLoginActionFactory().Execute(Context, Response);
        }
    }
}