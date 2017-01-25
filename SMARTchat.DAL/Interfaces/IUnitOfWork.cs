using System;
using SMARTchat.DAL.Entities;

namespace SMARTchat.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Channel> Channels { get; }
        IRepository<Message> Messages { get; }
        IRepository<ApplicationUser> Users { get; }
        void Save();
    }
}
