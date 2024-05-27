using System;
using System.Threading.Tasks;

namespace WhiteLabel.Application.Interfaces.Generic
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();
        void Commit();
        Task CommitAsync();
        void Rollback();
    }
}
