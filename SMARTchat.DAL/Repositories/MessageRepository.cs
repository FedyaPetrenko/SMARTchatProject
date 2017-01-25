using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SMARTchat.DAL.EF;
using SMARTchat.DAL.Entities;
using SMARTchat.DAL.Interfaces;

namespace SMARTchat.DAL.Repositories
{
    public class MessageRepository : IRepository<Message>
    {
        private readonly ApplicationDbContext _dbcontext;

        public MessageRepository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IEnumerable<Message> GetAll()
        {
            return _dbcontext.Messages;
        }

        public Message Get(string id)
        {
            return _dbcontext.Messages.Find(Int32.Parse(id));
        }

        public void Create(Message item)
        {
            _dbcontext.Messages.Add(item);
        }

        public void Update(Message item)
        {
            _dbcontext.Entry(item).State = EntityState.Modified;
        }

        public IEnumerable<Message> Find(Func<Message, bool> predicate)
        {
            return _dbcontext.Messages.Where(predicate).ToList();
        }

        public void Delete(string id)
        {
            var message = _dbcontext.Messages.Find(Int32.Parse(id));
            if (message != null)
                _dbcontext.Messages.Remove(message);
        }
    }
}
