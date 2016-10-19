using System;
using System.Globalization;
using Nancy;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;
using Octopus.Server.Extensibility.Authentication.Guest.GuestAuth;
using Octopus.Server.Extensibility.Authentication.HostServices;
using Octopus.Server.Extensibility.Authentication.Resources;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;
using Octopus.Server.Extensibility.HostServices.Web;
using Octopus.Time;

namespace Octopus.Server.Extensibility.Authentication.Guest.Web
{
    public class UserLoginAction : IApiAction
    {
        readonly IGuestConfigurationStore configurationStore;
        readonly IGuestCredentialValidator credentialValidator;
        readonly IAuthCookieCreator issuer;
        readonly IInvalidLoginTracker loginTracker;
        readonly ISleep sleep;
        readonly IApiActionModelBinder modelBinder;
        readonly IApiActionResponseCreator responseCreator;
        readonly IUserMapper userMapper;

        public UserLoginAction(
            IGuestConfigurationStore configurationStore,
            IGuestCredentialValidator credentialValidator,
            IAuthCookieCreator issuer,
            IInvalidLoginTracker loginTracker,
            ISleep sleep,
            IApiActionModelBinder modelBinder,
            IApiActionResponseCreator responseCreator,
            IUserMapper userMapper)
        {
            this.configurationStore = configurationStore;
            this.credentialValidator = credentialValidator;
            this.issuer = issuer;
            this.loginTracker = loginTracker;
            this.sleep = sleep;
            this.modelBinder = modelBinder;
            this.responseCreator = responseCreator;
            this.userMapper = userMapper;
        }

        public Response Execute(NancyContext context, IResponseFormatter response)
        {
            if (!configurationStore.GetIsEnabled())
                return responseCreator.AsStatusCode(HttpStatusCode.BadRequest);

            var model = modelBinder.BindAndValidate<LoginCommand>(context);

            var action = loginTracker.BeforeAttempt(model.Username, context.Request.UserHostAddress);
            if (action == InvalidLoginAction.Ban)
            {
                return responseCreator.BadRequest("You have had too many failed login attempts in a short period of time. Please try again later.");
            }

            var user = credentialValidator.ValidateCredentials(model.Username, model.Password);
            if (user == null || !user.IsActive || user.IsService)
            {
                loginTracker.RecordFailure(model.Username, context.Request.UserHostAddress);

                if (action == InvalidLoginAction.Slow)
                {
                    sleep.For(1000);
                }

                return responseCreator.BadRequest("Invalid username or password.");
            }

            loginTracker.RecordSucess(model.Username, context.Request.UserHostAddress);

            var cookie = issuer.CreateAuthCookie(context, user.IdentificationToken, model.RememberMe);

            return responseCreator.AsOctopusJson(response, userMapper.MapToResource(user))
                .WithCookie(cookie)
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Expires", DateTime.UtcNow.AddYears(1).ToString("R", DateTimeFormatInfo.InvariantInfo));
        }
    }
}