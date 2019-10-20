using DAL.Repositoris;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DbManager : IDisposable
    {
        private AppContext context = new AppContext();

        private GenericRepository<User> _user;

        private GenericRepository<Product> _Product;

        public GenericRepository<User> User
        {
            get
            {
                if (_user == null)
                {
                    _user = new GenericRepository<User>(context);
                }
                    return _user;
            }
        }

        public GenericRepository<Product> Product
        {
            get
            {
                if (_Product == null)
                {
                    _Product = new GenericRepository<Product>(context);
                }
                return _Product;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            context.SaveChanges();
        }
    }
}
