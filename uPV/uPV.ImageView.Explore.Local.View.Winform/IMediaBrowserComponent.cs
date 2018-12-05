using System;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace uPV.ImageView.MediaBrowser
{
    public interface IMediaBrowserComponent : IApplicationComponent
    {
        ITable StudyTable { get; }

        ISelection Selection { get; set; }

        ActionModelNode ToolbarActionModel { get; }

        ActionModelNode ContextMenuActionModel { get; }

        event EventHandler SelectionChanged;

        bool Load(string filename);

        void LoadFolder(string folder);
    }
}
