using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace uPV.ImageView.MediaBrowser.Tools
{
    [MenuAction("open", "global-menus/MenuFile/MenuOpenFiles", "Open")]
    [ButtonAction("open", "explorermedia-toolbar/ToolbarOpenFiles", "Open")]
    [IconSet("open", "Icons.OpenToolSmall.png", "Icons.OpenToolMedium.png", "Icons.OpenToolLarge.png")]
    [Tooltip("open", "TooltipOpenFiles")]
    [ExtensionOf(typeof(MediaBrowserExplorerToolExtensionPoint))]
    public class OpenFilesTool : Tool<IMediaBrowserExplorerToolContext>
    {
        public void Open()
        {
            var args = new SelectFolderDialogCreationArgs
            {
                AllowCreateNewFolder = false,
                Prompt = @"Open Dicom Files"
            };
            var result = base.Context.DesktopWindow.ShowSelectFolderDialogBox(args);

            if (result.Action == DialogBoxAction.Ok)
            {
                base.Context.Component.LoadFolder(result.FileName);
            }

            //FileDialogCreationArgs args = new FileDialogCreationArgs(string.Empty)
            //{
            //    Title = @"Open File Set",
            //    FileExtension = string.Empty
            //};
            //args.Filters.Add(new FileExtensionFilter("DICOMDIR; *.dcm", @"All Support File"));
            //args.Filters.Add(new FileExtensionFilter("*.*", @"All Files"));
            //FileDialogResult result = base.Context.DesktopWindow.ShowOpenFileDialogBox(args);

            //if (result.Action == DialogBoxAction.Ok)
            //{
            //    FileName = result.FileName;
            //}

            //base.Context.Component.Load(FileName);
        }
    }
}
