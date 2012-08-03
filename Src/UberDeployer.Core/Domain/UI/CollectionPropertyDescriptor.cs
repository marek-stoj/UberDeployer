using System;
using System.Collections;
using System.ComponentModel;

namespace UberDeployer.Core.Domain.UI
{
  public abstract class CollectionPropertyDescriptor<TCollection> : PropertyDescriptor
    where TCollection : MyCollectionBase
  {
    private readonly TCollection _collection;
    private readonly int _index;

    #region Constructor(s)

    protected CollectionPropertyDescriptor(TCollection collection, int index, string propertyDescriptorName)
      : base(propertyDescriptorName, null)
    {
      if (collection == null)
      {
        throw new ArgumentNullException("collection");
      }

      _collection = collection;
      _index = index;
    }

    #endregion

    #region Overrides of PropertyDescriptor

    public override bool CanResetValue(object component)
    {
      return false;
    }

    public override object GetValue(object component)
    {
      return ((IList)_collection)[_index];
    }

    public override void ResetValue(object component)
    {
      throw new NotSupportedException();
    }

    public override void SetValue(object component, object value)
    {
      throw new NotSupportedException();
    }

    public override bool ShouldSerializeValue(object component)
    {
      return true;
    }

    public override Type ComponentType
    {
      get { return _collection.GetType(); }
    }

    public override bool IsReadOnly
    {
      get { return true; }
    }

    public override Type PropertyType
    {
      get { return ((IList)_collection)[_index].GetType(); }
    }

    #endregion
  }
}
