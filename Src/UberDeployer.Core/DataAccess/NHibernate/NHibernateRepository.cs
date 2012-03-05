using System;
using NHibernate;

namespace UberDeployer.Core.DataAccess.NHibernate
{
  // TODO IMM HI: what about UoW and transactions?
  public abstract class NHibernateRepository
  {
    private readonly ISessionFactory _sessionFactory;

    protected NHibernateRepository(ISessionFactory sessionFactory)
    {
      if (sessionFactory == null)
      {
        throw new ArgumentNullException("sessionFactory");
      }

      _sessionFactory = sessionFactory;
    }

    protected ISession OpenSession()
    {
      return _sessionFactory.OpenSession();
    }
  }
}
