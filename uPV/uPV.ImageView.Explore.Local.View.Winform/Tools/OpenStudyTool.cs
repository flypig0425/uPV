using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;

namespace uPV.ImageView.MediaBrowser.Tools
{
    //[EnabledStateObserver("open", "Enabled", "EnabledChanged")]
    [Tooltip("open", "TooltipOpenStudies")]
    [MenuAction("open", "explorermedia-contextmenu/MenuOpenStudies", "OpenStudies")]
    [ExtensionOf(typeof(MediaBrowserExplorerToolExtensionPoint))]
    [IconSet("open", "Icons.OpenToolSmall.png", "Icons.OpenToolMedium.png", "Icons.OpenToolLarge.png")]
    [ButtonAction("open", "explorermedia-toolbar/ToolbarOpenStudies", "OpenStudies")]
    public class OpenStudyTool : Tool<IMediaBrowserExplorerToolContext>
    {
        public void OpenStudies()
        {
            try
            {
                int numberOfSelectedStudies = Context.Component.Selection.Items.Length;

                if (numberOfSelectedStudies == 0)
                    return;

                var helper = new OpenStudyHelper
                {
                    WindowBehaviour = ViewerLaunchSettings.WindowBehaviour,
                    AllowEmptyViewer = ViewerLaunchSettings.AllowEmptyViewer
                };

                foreach (StudyItem study in Context.Component.Selection.Items)
                {
                    helper.AddStudy(study.StudyInstanceUid, study.Server);
                }

                helper.Title = ImageViewerComponent.CreateTitle(GetSelectedPatients());
                helper.OpenStudies();

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, Context.DesktopWindow);
            }
        }

        
        private IEnumerable<IPatientData> GetSelectedPatients()
        {
            return Context.Component.Selection.Items.Cast<IPatientData>();
        }
    }
}
