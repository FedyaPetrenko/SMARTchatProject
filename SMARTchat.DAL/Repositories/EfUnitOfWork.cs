using System;
using SMARTchat.DAL.EF;
using SMARTchat.DAL.Entities;
using SMARTchat.DAL.Interfaces;

namespace SMARTchat.DAL.Repositories
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private ChannelRepository _channelRepository;
        private MessageRepository _messageRepository;
        private UserRepository _userRepository;

        public EfUnitOfWork(string connectingString)
        {
            _dbContext = new ApplicationDbContext(connectingString);
        }

        public IRepository<Channel> Channels
            => _channelRepository ?? (_channelRepository = new ChannelRepository(_dbContext));

        public IRepository<Message> Messages
            => _messageRepository ?? (_messageRepository = new MessageRepository(_dbContext));

        public IRepository<ApplicationUser> Users
            => _userRepository ?? (_userRepository = new UserRepository(_dbContext));

        public void Save()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (Exception exception)
            {
                var i = exception.Message;
            }
        }

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
