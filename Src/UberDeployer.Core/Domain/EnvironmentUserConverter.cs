using System;
using System.ComponentModel;

namespace UberDeployer.Core.Domain
{
  // TODO IMM HI: that's for UI!
  public class EnvironmentUserConverter : ExpandableObjectConverter
  {
    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
    {
      if (destType == typeof(string) && value is EnvironmentUser)
      {
        var environmentUser = (EnvironmentUser)value;

        return string.Format("{0}", environmentUser.UserName);
      }

      return base.ConvertTo(context, culture, value, destType);
    }
  }
}
