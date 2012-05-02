using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace UberDeployer.Core.Domain.UI
{
  // TODO IMM HI: that's for UI!
  public class EnvironmentUsersCollectionConverter : ExpandableObjectConverter
  {
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
    {
      if (destType == typeof(string) && value is EnvironmentUsersCollection)
      {
        var environmentUsersCollection = (EnvironmentUsersCollection)value;

        return
          string.Join(
            ", ",
            environmentUsersCollection.Cast<EnvironmentUser>()
              .Select(eu => eu.Id)
              .ToArray());
      }

      return base.ConvertTo(context, culture, value, destType);
    }
  }
}
