using System.Collections.Generic;
using NUnit.Framework;

namespace UberDeployer.Common.Tests
{
  [TestFixture]
  public class CollectionsExtensionsTests
  {
    [Test]
    public void Test_BinarySearch_1()
    {
      var list = new List<int> { 2, 4, 6 };
      int itemToSearchFor;
      int index;

      // ReSharper disable AccessToModifiedClosure
      itemToSearchFor = 1;
      index = list.BinarySearch(i => Comparer<int>.Default.Compare(i, itemToSearchFor));
      Assert.AreEqual(0, ~index);

      itemToSearchFor = 2;
      index = list.BinarySearch(i => Comparer<int>.Default.Compare(i, itemToSearchFor));
      Assert.AreEqual(0, index);

      itemToSearchFor = 3;
      index = list.BinarySearch(i => Comparer<int>.Default.Compare(i, itemToSearchFor));
      Assert.AreEqual(1, ~index);

      itemToSearchFor = 4;
      index = list.BinarySearch(i => Comparer<int>.Default.Compare(i, itemToSearchFor));
      Assert.AreEqual(1, index);

      itemToSearchFor = 5;
      index = list.BinarySearch(i => Comparer<int>.Default.Compare(i, itemToSearchFor));
      Assert.AreEqual(2, ~index);

      itemToSearchFor = 6;
      index = list.BinarySearch(i => Comparer<int>.Default.Compare(i, itemToSearchFor));
      Assert.AreEqual(2, index);
      // ReSharper restore AccessToModifiedClosure
    }

    [Test]
    public void Test_BinarySearch_2()
    {
      var list = new List<int> { 10 };
      int itemToSearchFor;
      int index;

      itemToSearchFor = 10;
      index = list.BinarySearch(i => Comparer<int>.Default.Compare(i, itemToSearchFor));
      Assert.AreEqual(0, index);
    }

    [Test]
    public void Test_BinarySearch_3()
    {
      var list = new List<int>();
      int itemToSearchFor;
      int index;

      itemToSearchFor = 10;
      index = list.BinarySearch(i => Comparer<int>.Default.Compare(i, itemToSearchFor));
      Assert.AreEqual(-1, index);
      Assert.AreEqual(0, ~index);
    }    
  }
}
