using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace uPV.ImageView.MediaBrowser.View
{
    [ExtensionOf(typeof(MediaBrowserExplorerComponentViewExtensionPoint))]
    public class MediaBrowserExplorerComponentView : WinFormsView, IApplicationComponentView
    {
        private Control _control;
        private MediaBrowserExplorerComponent _component;

        public override object GuiElement {
            get
            {
                if (_control == null)
                {
                    _control = new MediaBrowserExplorerControl(_component);
                }
                return _control;
            }
        }

        public void SetComponent(IApplicationComponent component)
        {
            _component = component as MediaBrowserExplorerComponent;
        }
    }
}
