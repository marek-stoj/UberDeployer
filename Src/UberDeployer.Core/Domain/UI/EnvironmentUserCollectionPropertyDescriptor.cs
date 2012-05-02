using System;
using System.ComponentModel;

namespace UberDeployer.Core.Domain.UI
{
  // TODO IMM HI: that's for UI!
  public class EnvironmentUserCollectionPropertyDescriptor : PropertyDescriptor
  {
    private readonly EnvironmentUsersCollection _environmentUsersCollection;
    private readonly int _index;

    public EnvironmentUserCollectionPropertyDescriptor(EnvironmentUsersCollection environmentUsersCollection, int index)
      : base(CreatePropertyDescriptorNameSafe(environmentUsersCollection, index), null)
    {
      if (environmentUsersCollection == null)
      {
        throw new ArgumentNullException("environmentUsersCollection");
      }

      _environmentUsersCollection = environmentUsersCollection;
      _index = index;
    }

    #region Overrides of PropertyDescriptor

    public override bool CanResetValue(object component)
    {
      return false;
    }

    public override object GetValue(object component)
    {
      return _environmentUsersCollection[_index];
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
      get { return _environmentUsersCollection.GetType(); }
    }

    public override bool IsReadOnly
    {
      get { return true; }
    }

    public override Type PropertyType
    {
      get { return _environmentUsersCollection[_index].GetType(); }
    }

    #endregion

    #region Private helper methods

    private static string CreatePropertyDescriptorNameSafe(EnvironmentUsersCollection environmentUsersCollection, int index)
    {
      if (environmentUsersCollection == null || index < 0 || index >= environmentUsersCollection.Count)
      {
        return "";
      }

      return environmentUsersCollection[index].Id;
    }

    #endregion
  }
}
