using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class GuestConfiguration : ExtensionConfigurationDocument
    {
        protected GuestConfiguration()
        {
        }

        public GuestConfiguration(string name, string extensionAuthor) : base(name, extensionAuthor)
        {
            Id = GuestConfigurationStore.SingletonId;
        }

        public bool IsEnabled { get; set; }
    }
}