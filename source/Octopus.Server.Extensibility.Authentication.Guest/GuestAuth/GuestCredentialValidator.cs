using System;
using Octopus.Data.Model.User;
using Octopus.Data.Storage.User;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Guest.GuestAuth
{
    public class GuestCredentialValidator : IGuestCredentialValidator
    {
        readonly ILog log;
        readonly IUserStore userStore;
        readonly IGuestConfigurationStore configurationStore;

        public GuestCredentialValidator(
            ILog log,
            IUserStore userStore,
            IGuestConfigurationStore configurationStore)
        {
            this.log = log;
            this.userStore = userStore;
            this.configurationStore = configurationStore;
        }

        public IUser ValidateCredentials(string username, string password)
        {
            if ((!configurationStore.GetIsEnabled() || !string.Equals(username, User.GuestLogin, StringComparison.InvariantCultureIgnoreCase)))
                return null;

            var user = userStore.GetByUsername(username);

            if (user == null)
            {
                log.Warn("Guest login is enabled, but the guest user acccount could not be found so the login request was rejected. Please restart the Octopus server.");
            }
            else if (user.IsActive == false)
            {
                log.Warn("Guest login is enabled, but the guest acccount is disabled so the login request was rejected. Please re-enable the guest account if you want guest logins to work.");
            }
            else
            {
                return user;
            }

            return null;
        }
    }
}