using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace UberDeployer.Core.Domain.UI
{
  // TODO IMM HI: that's for UI!
  public class WebServerMachineNamesCollectionConverter : ExpandableObjectConverter
  {
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
    {
      if (destType == typeof(string) && value is WebServerMachineNameCollection)
      {
        var webServerMachineNameCollection = (WebServerMachineNameCollection)value;

        return
          string.Join(
            ", ",
            webServerMachineNameCollection.Cast<string>()
              .ToArray());
      }

      return base.ConvertTo(context, culture, value, destType);
    }
  }
}
