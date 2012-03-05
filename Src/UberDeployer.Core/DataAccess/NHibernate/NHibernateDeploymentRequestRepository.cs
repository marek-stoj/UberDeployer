using System;
using System.Collections.Generic;
using NHibernate;
using UberDeployer.Core.Deployment.Pipeline.Modules;

namespace UberDeployer.Core.DataAccess.NHibernate
{
  public class NHibernateDeploymentRequestRepository : NHibernateRepository, IDeploymentRequestRepository
  {
    #region Constructor(s)

    public NHibernateDeploymentRequestRepository(ISessionFactory sessionFactory)
      : base(sessionFactory)
    {
    }

    #endregion

    #region IDeploymentRequestRepository Members

    public void AddDeploymentRequest(DeploymentRequest deploymentRequest)
    {
      if (deploymentRequest == null)
      {
        throw new ArgumentNullException("deploymentRequest");
      }

      if (deploymentRequest.Id != -1)
      {
        throw new ArgumentException("Only new entities (with Id == -1) can be added.", "deploymentRequest");
      }

      using (var session = OpenSession())
      {
        session.Save(deploymentRequest);
      }
    }

    public IEnumerable<DeploymentRequest> GetDeploymentRequests(int startIndex, int maxCount)
    {
      using (var session = OpenSession())
      {
        IQueryOver<DeploymentRequest> deploymentRequests =
          session.QueryOver<DeploymentRequest>()
            .OrderBy(dr => dr.DateRequested).Desc
            .ThenBy(dr => dr.Id).Asc
            .Skip(startIndex)
            .Take(maxCount);

        return deploymentRequests.List();
      }
    }

    #endregion
  }
}
