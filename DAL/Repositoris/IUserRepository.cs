using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositoris
{
    public interface IUserRepository : IDisposable
    {
        IEnumerable<User> GetUsers();
        User GetUserByID(int UserId);
        void InsertUser(User User);
        void DeleteUser(int UserId);
        void UpdateUser(User User);
        void Save();
    }
}
