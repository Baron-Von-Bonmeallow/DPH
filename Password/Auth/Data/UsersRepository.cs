using System.Collections.Concurrent;
using Auth.Models;
namespace Auth.Data
{
    public class UsersRepository: IUsersRepository
    {
        private readonly ServerContext _context;
        public UsersRepository(ServerContext context)
        {
            _context = context;
        }
        public Users? GetById(int id)
        {
            return _context.Users.Find(id);
        }
        public Users Register(Users user) 
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
        public void SettingsChange(int id,DateFormat Dnew)
        {
            _context.Users.Find(id)!.DFormat = Dnew;
        }
        public Users? GetbyName(string name)
        {
            return _context.Users.FirstOrDefault(u => u.UserName == name);
        }
    }
}
