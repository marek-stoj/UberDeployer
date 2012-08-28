using System.ComponentModel;
using System.Drawing.Design;

namespace UberDeployer.WinApp.ViewModels.PropertyGrids
{
  public class ReadOnlyUITypeEditor : UITypeEditor
  {
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.None;
    }
  }
}
