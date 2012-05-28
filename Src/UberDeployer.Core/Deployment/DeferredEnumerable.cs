using System;
using System.Collections;
using System.Collections.Generic;

namespace UberDeployer.Core.Deployment
{
  public class DeferredEnumerable<T> : IEnumerable<T>
  {
    private readonly Func<IEnumerable<T>> _enumerableFunc;

    #region Constructor(s)

    public DeferredEnumerable(Func<IEnumerable<T>> enumerableFunc)
    {
      if (enumerableFunc == null)
      {
        throw new ArgumentNullException("enumerableFunc");
      }

      _enumerableFunc = enumerableFunc;
    }

    #endregion

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator()
    {
      foreach (T item in _enumerableFunc())
      {
        yield return item;
      }
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
  }
}
