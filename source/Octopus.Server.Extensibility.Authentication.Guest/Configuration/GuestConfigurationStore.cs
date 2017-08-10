using System;
using System.Collections.Generic;
using System.Threading;
using Octopus.Configuration;
using Octopus.Data.Model.User;
using Octopus.Data.Storage.Configuration;
using Octopus.Data.Storage.User;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class GuestConfigurationStore : IGuestConfigurationStore, IHasConfigurationSettings
    {
        public static string SingletonId = "authentication-guest";

        readonly ILog log;
        readonly IKeyValueStore settings;
        readonly IConfigurationStore configurationStore;
        readonly IUserStore userStore;

        public GuestConfigurationStore(
            ILog log,
            IKeyValueStore settings,
            IConfigurationStore configurationStore,
            IUserStore userStore)
        {
            this.log = log;
            this.settings = settings;
            this.configurationStore = configurationStore;
            this.userStore = userStore;
        }

        public bool GetIsEnabled()
        {
            var doc = configurationStore.Get<GuestConfiguration>(SingletonId);
            if (doc != null)
                return doc.IsEnabled;

            doc = MoveSettingsToDatabase();

            return doc.IsEnabled;
        }

        public void SetIsEnabled(bool isEnabled)
        {
            var doc = configurationStore.Get<GuestConfiguration>(SingletonId) ?? MoveSettingsToDatabase();
            doc.IsEnabled = isEnabled;
            EnsureGuestUserIsInCorrectState(isEnabled);
            configurationStore.Update(doc);
        }

        GuestConfiguration MoveSettingsToDatabase()
        {
            log.Info("Moving Octopus.WebPortal.GuestLoginEnabled from config file to DB");

            var guestLoginEnabled = settings.Get("Octopus.WebPortal.GuestLoginEnabled", false);

            var doc = new GuestConfiguration("Guest", "Octopus Deploy")
            {
                IsEnabled = guestLoginEnabled
            };

            configurationStore.Create(doc);

            settings.Remove("Octopus.WebPortal.GuestLoginEnabled");
            settings.Save();

            EnsureGuestUserIsInCorrectState(guestLoginEnabled);

            return doc;
        }

        public string ConfigurationSetName => "Guest Login";
        public IEnumerable<ConfigurationValue> GetConfigurationValues()
        {
            yield return new ConfigurationValue("Octopus.WebPortal.GuestLoginEnabled", GetIsEnabled().ToString(), GetIsEnabled(), "Is Enabled");
        }

        void EnsureGuestUserIsInCorrectState(bool isEnabled)
        {
            var user = userStore.GetByUsername(User.GuestLogin);

            // if there isn't a guest user and we're enabling then create the user
            if (user == null && isEnabled)
            {
                log.Info("Creating guest user");
                var userResult = userStore.CreateOrUpdate(
                    User.GuestLogin, 
                    "Guest",
                    null,
                    null,
                    null,
                    false,
                    CancellationToken.None);
                if (!userResult.Succeeded)
                {
                    log.Error("Error creating guest account: " + userResult.FailureReason);
                    return;
                }
                user = userResult.User;

                // When the special guest login mode is enabled, no password is actually needed for the guest. 
                // But we give them a default password anyway just in case someone disables guest login and then re-enables the 
                // account
                var randomMilliseconds = new Random(DateTimeOffset.UtcNow.Millisecond).Next(100000);
                var pwd = DateTimeOffset.UtcNow.AddMilliseconds(randomMilliseconds).ToString();
                user.SetPassword(pwd);
            }

            // if we're enabling then by now the user must exist
            if (isEnabled)
            {
                userStore.EnableUser(user.Id);
            }
            else if (user != null)  // if we're ensuring is disabled user may never have existed
            {
                userStore.DisableUser(user.Id);
            }
        }
    }
}