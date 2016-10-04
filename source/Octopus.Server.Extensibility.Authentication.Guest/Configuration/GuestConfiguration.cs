using Octopus.Server.Extensibility.HostServices.Model;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class GuestConfiguration : IId
    {
        public string Id { get; set; }

        public bool IsEnabled { get; set; }
    }
}