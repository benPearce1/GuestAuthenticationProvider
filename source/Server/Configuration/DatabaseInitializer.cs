using Octopus.Configuration;
using Octopus.Data.Storage.Configuration;
using Octopus.Diagnostics;
using Octopus.Node.Extensibility.Extensions.Infrastructure;
using Octopus.Server.Extensibility.Authentication.Guest.GuestAuth;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class DatabaseInitializer : ExecuteWhenDatabaseInitializes
    {
        readonly ILog log;
        readonly IKeyValueStore settings;
        readonly IConfigurationStore configurationStore;
        readonly IGuestUserStateChecker guestUserStateChecker;

        private bool cleanupRequired = false;

        public DatabaseInitializer(ILog log, IKeyValueStore settings, IConfigurationStore configurationStore, IGuestUserStateChecker guestUserStateChecker)
        {
            this.log = log;
            this.settings = settings;
            this.configurationStore = configurationStore;
            this.guestUserStateChecker = guestUserStateChecker;
        }

        public override void Execute()
        {
            var doc = configurationStore.Get<GuestConfiguration>(GuestConfigurationStore.SingletonId);
            if (doc != null)
                return;

            log.Info("Moving Octopus.WebPortal.GuestLoginEnabled from config file to DB");

            var guestLoginEnabled = settings.Get("Octopus.WebPortal.GuestLoginEnabled", false);

            doc = new GuestConfiguration("Guest", "Octopus Deploy")
            {
                IsEnabled = guestLoginEnabled
            };

            configurationStore.Create(doc);

            guestUserStateChecker.EnsureGuestUserIsInCorrectState(guestLoginEnabled);

            cleanupRequired = true;
        }

        public override void PostExecute()
        {
            if (!cleanupRequired)
                return;

            settings.Remove("Octopus.WebPortal.GuestLoginEnabled");
            settings.Save();
        }
    }
}