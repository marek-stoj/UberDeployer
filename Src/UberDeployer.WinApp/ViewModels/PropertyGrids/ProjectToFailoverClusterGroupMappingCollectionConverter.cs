using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  public class ProjectToFailoverClusterGroupMappingsCollectionConverter : ExpandableObjectConverter
  {
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
    {
      if (destType == typeof(string) && value is ProjectToFailoverClusterGroupMappingsCollection)
      {
        var collection = (ProjectToFailoverClusterGroupMappingsCollection)value;

        return
          string.Join(
            ", ",
            collection.Cast<ProjectToFailoverClusterGroupMappingInPropertyGridViewModel>()
              .Select(eu => eu.ProjectName)
              .ToArray());
      }

      return base.ConvertTo(context, culture, value, destType);
    }
  }
}
