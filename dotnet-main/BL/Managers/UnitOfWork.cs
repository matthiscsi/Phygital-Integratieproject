using Microsoft.EntityFrameworkCore.Storage;
using PIP.DAL.EF;

namespace PIP.BL.Managers;

public class UnitOfWork
{
    private readonly PhygitalDbContext _dbContext;
    private IDbContextTransaction _transaction;


    public UnitOfWork(PhygitalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void BeginTransaction()
    {
        _transaction = _dbContext.Database.BeginTransaction();
    }

    public void Commit()
    {
        _dbContext.SaveChanges();
        _dbContext.Database.CommitTransaction();
    }
}