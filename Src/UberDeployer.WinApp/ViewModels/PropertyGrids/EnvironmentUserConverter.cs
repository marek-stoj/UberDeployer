using System;
using System.ComponentModel;
using System.Globalization;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  public class EnvironmentUserConverter : ExpandableObjectConverter
  {
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
    {
      if (destType == typeof(string) && value is EnvironmentUserInPropertyGridVieModel)
      {
        var environmentUser = (EnvironmentUserInPropertyGridVieModel)value;

        return string.Format("{0}", environmentUser.UserName);
      }

      return base.ConvertTo(context, culture, value, destType);
    }
  }
}
