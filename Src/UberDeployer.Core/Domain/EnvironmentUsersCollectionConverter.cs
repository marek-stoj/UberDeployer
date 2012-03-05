using System;
using System.ComponentModel;

namespace UberDeployer.Core.Domain
{
  // TODO IMM HI: that's for UI!
  public class EnvironmentUsersCollectionConverter : ExpandableObjectConverter
  {
    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
    {
      if (destType == typeof(string) && value is EnvironmentUsersCollection)
      {
        return "Environment users collection.";
      }

      return base.ConvertTo(context, culture, value, destType);
    }
  }
}
