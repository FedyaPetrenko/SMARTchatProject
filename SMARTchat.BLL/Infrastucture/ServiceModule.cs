using Ninject.Modules;
using SMARTchat.DAL.EF;
using SMARTchat.DAL.Interfaces;
using SMARTchat.DAL.Repositories;

namespace SMARTchat.BLL.Infrastucture
{
    public class ServiceModule : NinjectModule
    {
        private readonly string _connectionString;

        public ServiceModule(string connection)
        {
            _connectionString = connection;
        }

        public override void Load()
        {
            Bind<IUnitOfWork>().To<EfUnitOfWork>().InSingletonScope().WithConstructorArgument(_connectionString);
            Bind<ApplicationDbContext>().ToSelf().InSingletonScope().WithConstructorArgument(_connectionString);
        }
    }
}
