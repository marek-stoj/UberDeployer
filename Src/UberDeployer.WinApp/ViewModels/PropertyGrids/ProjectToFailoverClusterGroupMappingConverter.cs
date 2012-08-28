using System;
using System.ComponentModel;
using System.Globalization;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  public class ProjectToFailoverClusterGroupMappingConverter : ExpandableObjectConverter
  {
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
    {
      if (destType == typeof(string) && value is ProjectToFailoverClusterGroupMappingInPropertyGridViewModel)
      {
        var environmentUser = (ProjectToFailoverClusterGroupMappingInPropertyGridViewModel)value;

        return string.Format("{0}", environmentUser.ClusterGroupName);
      }

      return base.ConvertTo(context, culture, value, destType);
    }
  }
}
