using System.ComponentModel;
using System.Drawing.Design;

namespace UberDeployer.Core.Domain
{
  // TODO IMM HI: that's for UI
  // TODO IMM HI: remove reference to System.Drawing
  public class ReadOnlyUITypeEditor : UITypeEditor
  {
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.None;
    }
  }
}
