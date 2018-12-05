using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Explorer;

namespace uPV.ImageView.MediaBrowser
{
    [ExtensionOf(typeof(HealthcareArtifactExplorerExtensionPoint))]
    public class MediaBrowserExplorer : IHealthcareArtifactExplorer
    {
        private MediaBrowserExplorerComponent _component;

        public string Name
        {
            get { return "Study List"; }
        }

        public bool IsAvailable
        {
            get { return true; }
        }

        public IApplicationComponent Component
        {
            get
            {
                if ((_component == null) && IsAvailable)
                    _component = new MediaBrowserExplorerComponent();

                return _component;
            }
        }
    }
}

