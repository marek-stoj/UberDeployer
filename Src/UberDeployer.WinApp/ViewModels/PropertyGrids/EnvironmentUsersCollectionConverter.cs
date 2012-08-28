using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
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
            environmentUsersCollection.Cast<EnvironmentUserInPropertyGridVieModel>()
              .Select(eu => eu.Id)
              .ToArray());
      }

      return base.ConvertTo(context, culture, value, destType);
    }
  }
}
