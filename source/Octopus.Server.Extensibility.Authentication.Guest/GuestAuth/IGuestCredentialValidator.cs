using Octopus.Data.Model.User;

namespace Octopus.Server.Extensibility.Authentication.Guest.GuestAuth
{
    public interface IGuestCredentialValidator
    {
        IUser ValidateCredentials(string username, string password);
    }
}