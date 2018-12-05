using System;
using System.Collections;
using System.Windows.Forms;
using ClearCanvas.Desktop;

namespace uPV.ImageView.MediaBrowser.View
{
    public partial class MediaBrowserExplorerControl : UserControl
    {
        private MediaBrowserExplorerComponent _studyBrowserComponent;
        private bool _selectionUpdating = false;

        public MediaBrowserExplorerControl(MediaBrowserExplorerComponent component)
        {
            _studyBrowserComponent = component;

            InitializeComponent();

            _studyBrowserComponent.StudyTableChanged += OnStudyBrowserComponentOnStudyTableChanged;
            _studyBrowserComponent.SelectedStudyChanged += OnStudyBrowserComponentSelectedStudyChanged;

            _studyTableView.Table = _studyBrowserComponent.StudyTable;
            _studyTableView.ToolbarModel = component.ToolbarActionModel;
            _studyTableView.MenuModel = component.ContextMenuActionModel;

            _studyTableView.SelectionChanged += new EventHandler(OnStudyTableViewSelectionChanged);
            _studyTableView.ItemDoubleClicked += new EventHandler(OnStudyTableViewDoubleClick);
            
        }

        private void OnStudyBrowserComponentOnStudyTableChanged(object sender, EventArgs e)
        {
            _studyTableView.Table = _studyBrowserComponent.StudyTable;
        }

        private void OnStudyBrowserComponentSelectedStudyChanged(object sender, EventArgs e)
        {
            if (_selectionUpdating) return;
            _selectionUpdating = true;
            try
            {
                _studyTableView.Selection = new Selection(_studyBrowserComponent.SelectedStudies);
            }
            finally
            {
                _selectionUpdating = false;
            }
        }

        private void OnStudyTableViewSelectionChanged(object sender, EventArgs e)
        {
            if (_selectionUpdating) return;
            _selectionUpdating = true;
            try
            {
                //The table view remembers the selection order, with the most recent being first.
                //We actually want that same order, but in reverse.
                _studyBrowserComponent.SetSelection(ReverseSelection(_studyTableView.Selection));
            }
            finally
            {
                _selectionUpdating = false;
            }
        }

        void OnStudyTableViewDoubleClick(object sender, EventArgs e)
        {
            _studyBrowserComponent.ItemDoubleClick();
        }

        private static ISelection ReverseSelection(ISelection selection)
        {
            ArrayList list = new ArrayList();

            if (selection != null && selection.Items != null)
            {
                foreach (object o in selection.Items)
                    list.Add(o);

                list.Reverse();
            }

            return new Selection(list);
        }
    }
}
