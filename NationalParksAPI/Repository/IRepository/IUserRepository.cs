using NationalParksAPI.Models;

namespace NationalParksAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        AuthenticationModel Authenticate(string username, string password);
        AuthenticationModel Register(string username, string password);
    }
}
