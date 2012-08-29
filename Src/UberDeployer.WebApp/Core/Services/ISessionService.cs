using System;

namespace UberDeployer.WebApp.Core.Services
{
  public interface ISessionService
  {
    Guid UniqueClientId { get; }
  }
}
