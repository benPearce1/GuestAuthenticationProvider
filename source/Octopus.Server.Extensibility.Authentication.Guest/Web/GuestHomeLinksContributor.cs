using System.Collections.Generic;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;
using Octopus.Server.Extensibility.HostServices.Web;

namespace Octopus.Server.Extensibility.Authentication.Guest.Web
{
    public class GuestHomeLinksContributor : IHomeLinksContributor
    {
        readonly IGuestConfigurationStore configurationStore;

        public GuestHomeLinksContributor(IGuestConfigurationStore configurationStore)
        {
            this.configurationStore = configurationStore;
        }

        public IDictionary<string, string> GetLinksToContribute()
        {
            var linksToContribute = new Dictionary<string, string>();

            if (configurationStore.GetIsEnabled())
            {
                linksToContribute.Add("Authenticate_OctopusGuest", "~" + GuestApi.ApiUsersAuthenticate);
            }

            return linksToContribute;
        }
    }
}