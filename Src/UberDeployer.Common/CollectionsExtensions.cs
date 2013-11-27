using System;
using System.Collections.Generic;
using System.Linq;

namespace UberDeployer.Common
{
  public static class CollectionsExtensions
  {
    /// <remarks>
    /// Adapted from .NET 4.0 source code (ArraySortHelper`1.InternalBinarySearch()).
    /// </remarks>
    public static int BinarySearch<T>(this IList<T> list, Func<T, int> compareFunc)
    {
      if (list == null)
      {
        throw new ArgumentNullException("list");
      }

      if (compareFunc == null)
      {
        throw new ArgumentNullException("compareFunc");
      }

      int left = 0;
      int right = list.Count - 1;

      while (left <= right)
      {
        int middle = left + ((right - left) >> 1);
        int comparisonResult = compareFunc(list[middle]);

        if (comparisonResult == 0)
        {
          return middle;
        }

        if (comparisonResult < 0)
        {
          left = middle + 1;
        }
        else
        {
          right = middle - 1;
        }
      }

      return ~left;
    }

    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
      foreach (T element in collection)
      {
        action(element);
      }
    }

    public static T Second<T>(this IEnumerable<T> collection)
    {
      return collection.Skip(1).First();
    }
  }
}
