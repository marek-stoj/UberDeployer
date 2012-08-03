using System;
using System.ComponentModel;
using System.Globalization;

namespace UberDeployer.Core.Domain.UI
{
  // TODO IMM HI: that's for UI!
  public class ProjectToFailoverClusterGroupMappingConverter : ExpandableObjectConverter
  {
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
    {
      if (destType == typeof(string) && value is ProjectToFailoverClusterGroupMapping)
      {
        var environmentUser = (ProjectToFailoverClusterGroupMapping)value;

        return string.Format("{0}", environmentUser.ClusterGroupName);
      }

      return base.ConvertTo(context, culture, value, destType);
    }
  }
}
