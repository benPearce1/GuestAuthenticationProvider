using Octopus.Node.Extensibility.Authentication.Extensions;

namespace Octopus.Server.Extensibility.Authentication.Guest.GuestAuth
{
    public interface IGuestCredentialValidator : IDoesBasicAuthentication
    {
    }
}