using System;
using System.Web;
using System.Web.SessionState;

namespace UberDeployer.WebApp.Core.Services
{
  public class SessionService : ISessionService
  {
    private const string SessionKey_UniqueClientId = "UniqueClientId";

    #region ISessionService Members

    public Guid UniqueClientId
    {
      get
      {
        Guid uniqueClientId;

        if (Session[SessionKey_UniqueClientId] == null)
        {
          uniqueClientId = Guid.NewGuid();
          Session[SessionKey_UniqueClientId] = uniqueClientId;
        }
        else
        {
          uniqueClientId = (Guid)Session[SessionKey_UniqueClientId];
        }

        return uniqueClientId;
      }
    }

    #endregion

    #region Properties

    private HttpSessionState Session
    {
      get { return HttpContext.Current.Session; }
    }

    #endregion
  }
}
