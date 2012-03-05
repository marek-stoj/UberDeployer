using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace UberDeployer.Core.Domain
{
  // TODO IMM HI: that's for UI!
  [ReadOnly(true)]
  public class EnvironmentUsersCollection : CollectionBase, ICustomTypeDescriptor
  {
    #region Constructor(s)

    public EnvironmentUsersCollection(IEnumerable<EnvironmentUser> environmentUsers)
    {
      if (environmentUsers == null)
      {
        throw new ArgumentNullException("environmentUsers");
      }

      foreach (EnvironmentUser environmentUser in environmentUsers)
      {
        List.Add(environmentUser);
      }
    }

    #endregion

    #region ICustomTypeDescriptor members

    public AttributeCollection GetAttributes()
    {
      return TypeDescriptor.GetAttributes(this, true);
    }

    public string GetClassName()
    {
      return TypeDescriptor.GetClassName(this, true);
    }

    public string GetComponentName()
    {
      return TypeDescriptor.GetComponentName(this, true);
    }

    public TypeConverter GetConverter()
    {
      return TypeDescriptor.GetConverter(this, true);
    }

    public EventDescriptor GetDefaultEvent()
    {
      return TypeDescriptor.GetDefaultEvent(this, true);
    }

    public PropertyDescriptor GetDefaultProperty()
    {
      return TypeDescriptor.GetDefaultProperty(this, true);
    }

    public object GetEditor(Type editorBaseType)
    {
      return TypeDescriptor.GetEditor(this, editorBaseType, true);
    }

    public EventDescriptorCollection GetEvents(Attribute[] attributes)
    {
      return TypeDescriptor.GetEvents(this, attributes, true);
    }

    public EventDescriptorCollection GetEvents()
    {
      return TypeDescriptor.GetEvents(this, true);
    }

    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
      return GetProperties();
    }

    public PropertyDescriptorCollection GetProperties()
    {
      var propertyDescriptorCollection = new PropertyDescriptorCollection(null);

      for (int i = 0; i < List.Count; i++)
      {
        var environmentUserCollectionPropertyDescriptor =
          new EnvironmentUserCollectionPropertyDescriptor(this, i);
        
        propertyDescriptorCollection.Add(environmentUserCollectionPropertyDescriptor);
      }

      return propertyDescriptorCollection;
    }

    public object GetPropertyOwner(PropertyDescriptor pd)
    {
      return this;
    }

    #endregion

    #region Properties

    public EnvironmentUser this[int index]
    {
      get { return (EnvironmentUser)List[index]; }
    }

    #endregion
  }
}
