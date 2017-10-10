using Octopus.Node.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Guest.Configuration
{
    public class GuestConfiguration : ExtensionConfigurationDocument
    {
        protected GuestConfiguration()
        {
        }

        public GuestConfiguration(string name, string extensionAuthor) : base(name, extensionAuthor)
        {
        }

        public bool IsEnabled { get; set; }
    }
}