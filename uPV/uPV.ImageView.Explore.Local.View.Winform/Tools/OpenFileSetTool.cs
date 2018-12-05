using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace uPV.ImageView.MediaBrowser.Tools
{

    [MenuAction("open", "global-menus/MenuFile/MenuOpenFileSet", "Open")]
    [ButtonAction("open", "explorermedia-toolbar/ToolbarOpenFileSet", "Open")]
    [IconSet("open", "Icons.OpenToolSmall.png", "Icons.OpenToolMedium.png", "Icons.OpenToolLarge.png")]
    [Tooltip("open", "TooltipOpenFileSet")]
    [ExtensionOf(typeof(MediaBrowserExplorerToolExtensionPoint))]
    public class OpenFileSetTool : Tool<IMediaBrowserExplorerToolContext>
    {
        public string FileName { get; set; }

        public void Open()
        {
            FileDialogCreationArgs args = new FileDialogCreationArgs(string.Empty)
            {
                Title = @"Open File Set",
                FileExtension = string.Empty
            };
            args.Filters.Add(new FileExtensionFilter("DICOMDIR; *.dcm", @"All Support File"));
            args.Filters.Add(new FileExtensionFilter("*.*", @"All Files"));
            FileDialogResult result = base.Context.DesktopWindow.ShowOpenFileDialogBox(args);

            if (result.Action == DialogBoxAction.Ok)
            {
                FileName = result.FileName;
            }

            base.Context.Component.Load(FileName);
        }
    }
}
