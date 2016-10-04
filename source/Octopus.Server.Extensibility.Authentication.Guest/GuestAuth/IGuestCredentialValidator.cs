using Octopus.Server.Extensibility.HostServices.Model;

namespace Octopus.Server.Extensibility.Authentication.Guest.GuestAuth
{
    public interface IGuestCredentialValidator
    {
        IUser ValidateCredentials(string username, string password);
    }
}