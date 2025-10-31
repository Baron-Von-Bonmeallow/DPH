using Auth.Models;
namespace Auth.Data
{
    public interface IUsersRepository
    {
        Users? GetById(int id);
        Users Register(Users users);
        Users? GetbyName(string name);
        void SettingsChange(int id, DateFormat Dnew);
    }
}
