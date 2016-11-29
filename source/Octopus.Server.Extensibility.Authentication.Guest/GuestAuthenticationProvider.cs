using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;
using Octopus.Server.Extensibility.Authentication.Resources;

namespace Octopus.Server.Extensibility.Authentication.Guest
{
    public class GuestAuthenticationProvider : IAuthenticationProvider
    {
        readonly IGuestConfigurationStore configurationStore;

        public GuestAuthenticationProvider(IGuestConfigurationStore configurationStore)
        {
            this.configurationStore = configurationStore;
        }

        public string IdentityProviderName => "Octopus - Guest";

        public bool IsEnabled => configurationStore.GetIsEnabled();

        public bool SupportsPasswordManagement => false;

        public string AuthenticateUri => GuestApi.ApiUsersAuthenticate;

        public AuthenticationProviderElement GetAuthenticationProviderElement(string requestDirectoryPath)
        {
            return GetAuthenticationProviderElement();
        }

        public AuthenticationProviderElement GetAuthenticationProviderElement()
        {
            var authenticationProviderElement = new AuthenticationProviderElement
            {
                Name = IdentityProviderName,
                IsGuestProvider = true,
                LinkHtml = "<div style=\"padding: 20px 20px 30px; text-align: center; \"><button class=\"btn btn-success\" type=\"button\" ng-click=\"signIn()\" focus-on=\"guest\" ng-disabled=\"isSubmitting.busy\">Sign in as a guest</button></div>"
            };
            authenticationProviderElement.Links.Add(AuthenticationProviderElement.FormsAuthenticateLinkName, "~" + AuthenticateUri);

            return authenticationProviderElement;
        }

        public string[] GetAuthenticationUrls()
        {
            return new[] { AuthenticateUri };
        }
    }
}