using Autofac;
using Octopus.Server.Extensibility.Authentication.Guest.Configuration;
using Octopus.Server.Extensibility.Authentication.Guest.GuestAuth;
using Octopus.Server.Extensibility.Authentication.Guest.Web;
using Octopus.Server.Extensibility.Extensions;
using Octopus.Server.Extensibility.Extensions.Contracts.Authentication;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Server.Extensibility.HostServices.Web;

namespace Octopus.Server.Extensibility.Authentication.Guest
{
    [OctopusPlugin("Guest", "Octopus Deploy")]
    public class GuestExtension : IOctopusExtension
    {
        public void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GuestHomeLinksContributor>().As<IHomeLinksContributor>().InstancePerDependency();

            builder.RegisterType<GuestConfigurationMapping>().As<IConfigurationDocumentMapper>().InstancePerDependency();

            builder.RegisterType<GuestConfigurationStore>()
                .As<IGuestConfigurationStore>()
                .As<IHasConfigurationSettings>()
                .InstancePerDependency();
            builder.RegisterType<GuestConfigureCommands>().As<IContributeToConfigureCommand>().InstancePerDependency();

            builder.RegisterType<GuestCredentialValidator>().As<IGuestCredentialValidator>().InstancePerDependency();

            builder.RegisterType<UserLoginAction>().AsSelf().InstancePerDependency();

            builder.RegisterType<GuestAuthenticationProvider>().As<IAuthenticationProvider>().InstancePerDependency();
        }
    }
}