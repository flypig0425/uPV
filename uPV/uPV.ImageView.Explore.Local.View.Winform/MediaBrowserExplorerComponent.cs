using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;
using uPV.ImageView.MediaBrowser.MediaFileStore;

namespace uPV.ImageView.MediaBrowser
{
    public interface IMediaBrowserExplorerToolContext : IToolContext
    {
        IMediaBrowserComponent Component { get; }

        ClickHandlerDelegate DefaultActionHandler { get; set; }

        IDesktopWindow DesktopWindow { get; }
    }

    [ExtensionPoint]
    public sealed class MediaBrowserExplorerToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionPoint]
    public sealed class MediaBrowserExplorerComponentViewExtensionPoint : 
        ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(MediaBrowserExplorerComponentViewExtensionPoint))]
    public class MediaBrowserExplorerComponent : ApplicationComponent, IMediaBrowserComponent
    {
        protected class MediaBrowserExplorerToolContext : ToolContext, IMediaBrowserExplorerToolContext
        {
            private readonly MediaBrowserExplorerComponent _component;

            public MediaBrowserExplorerToolContext(MediaBrowserExplorerComponent component)
            {
                _component = component;
            }

            public IMediaBrowserComponent Component
            {
                get { return _component; }
            }

            public ClickHandlerDelegate DefaultActionHandler
            {
                get { return _component.DefaultActionHandler; }
                set { _component.DefaultActionHandler = value; }
            }

            public IDesktopWindow DesktopWindow
            {
                get
                {
                    return _component.Host.DesktopWindow;
                }
            }
        }

        private ToolSet _toolSet;
        private event EventHandler _selectionChanged;

        private ActionModelRoot _toolbarModel;
        private ActionModelRoot _contextMenuModel;

        private readonly StudyTable _dummyStudyTable;

        private event EventHandler _studyTableChanged;
        private event EventHandler _selectedStudyChangedEvent;

        private ClickHandlerDelegate _defaultActionHandler;

        public MediaBrowserExplorerComponent()
        {
            _dummyStudyTable = new StudyTable();
            _dummyStudyTable.Initialize();
        }

        public event EventHandler StudyTableChanged
        {
            add { _studyTableChanged += value; }
            remove { _studyTableChanged -= value; }
        }

        public event EventHandler SelectedStudyChanged
        {
            add { _selectedStudyChangedEvent += value; }
            remove { _selectedStudyChangedEvent -= value; }
        }


        protected ToolSet ToolSet
        {
            get { return _toolSet; }
            set { _toolSet = value; }
        }

        public ClickHandlerDelegate DefaultActionHandler
        {
            get { return _defaultActionHandler; }
            set { _defaultActionHandler = value; }
        }

        public ITable StudyTable
        {
            get { return _dummyStudyTable; }
        }

        public ISelection Selection { get; set; }

        public StudyItem SelectedStudy
        {
            get { return Selection == null ? null : Selection.Item as StudyItem; }
        }

        public ReadOnlyCollection<StudyItem> SelectedStudies
        {
            get
            {
                var selectedStudies = new List<StudyItem>();
                if (Selection != null)
                    selectedStudies.AddRange(Selection.Items.Cast<StudyItem>());

                return selectedStudies.AsReadOnly();
            }
        }

        public ActionModelNode ToolbarActionModel
        {
            get { return _toolbarModel; }
        }

        public event EventHandler SelectionChanged
        {
            add { _selectionChanged += value; }
            remove { _selectionChanged -= value; }
        }

        public bool Load(string filename)
        {
            var extensions = new MediaFileSetReaderExtensionPoint();
            MediaFileSet fileSet = null;

            foreach (IMediaFileSetReader item in extensions.CreateExtensions())
            {
                if (item.TryRead(filename, out fileSet))
                {
                    break;
                }   
            }

            if (fileSet == null)
                return false;

            _dummyStudyTable.Items.AddRange(ImageViewerComponent.FindStudy(new QueryParameters(), fileSet,
                "MediaFileSet"));
            
            return true;
        }

        public void LoadFolder(string path)
        {
            if (!Directory.Exists(path))
                return;

            List<string> files = new List<string>();
            foreach (var file in new DirectoryInfo(path).GetFiles())
            {
                files.Add(file.FullName);
            }

            try
            {
                new OpenFilesHelper(files) { WindowBehaviour = ViewerLaunchSettings.WindowBehaviour }.OpenFiles();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, @"Unable Open files", DesktopWindow);
            }
        }

        public void SetSelection(ISelection selection)
        {
            if (Equals(Selection, selection))
                return;

            Selection = selection;
            EventsHelper.Fire(_selectedStudyChangedEvent, this, EventArgs.Empty);
        }

        protected void OnSelectionChanged()
        {
            EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
        }

        public void DefaultAction()
        {
            if (this.DefaultActionHandler != null)
                this.DefaultActionHandler();
        }

        public void ItemDoubleClick()
        {
            if (_defaultActionHandler != null)
                _defaultActionHandler();
        }

        public IDesktopWindow DesktopWindow
        {
            get { return base.Host.DesktopWindow; }
        }

        public ActionModelNode ContextMenuActionModel
        {
            get
            {
                return _contextMenuModel;
            }
        }

        public override void Start()
        {
            base.Start();

            var tools = new ArrayList(new MediaBrowserExplorerToolExtensionPoint().CreateExtensions());
            ToolSet = new ToolSet(tools, new MediaBrowserExplorerToolContext(this));

            _toolbarModel = ActionModelRoot.CreateModel(GetType().FullName, "explorermedia-toolbar", _toolSet.Actions);
            _contextMenuModel = ActionModelRoot.CreateModel(GetType().FullName, "explorermedia-contextmenu", _toolSet.Actions);
        }

        public override void Stop()
        {
            base.Stop();
            ToolSet.Dispose();
            ToolSet = null;
        }
    }
}
