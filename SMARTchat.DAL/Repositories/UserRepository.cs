using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SMARTchat.DAL.EF;
using SMARTchat.DAL.Entities;
using SMARTchat.DAL.Interfaces;

namespace SMARTchat.DAL.Repositories
{
    public class UserRepository : IRepository<ApplicationUser>
    {
        private readonly ApplicationDbContext _dbcontext;

        public UserRepository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _dbcontext.Users;
        }

        public ApplicationUser Get(string id)
        {
            return _dbcontext.Users.Find(id);
        }

        public void Create(ApplicationUser item)
        {
            _dbcontext.Users.Add(item);
        }

        public void Update(ApplicationUser item)
        {
            _dbcontext.Entry(item).State = EntityState.Modified;
        }

        public IEnumerable<ApplicationUser> Find(Func<ApplicationUser, bool> predicate)
        {
            return _dbcontext.Users.Include(u => u.Channels).Where(predicate).ToList();
        }

        public void Delete(string id)
        {
            var user = _dbcontext.Users.Find(id);
            if (user != null)
                _dbcontext.Users.Remove(user);
        }
    }
}
