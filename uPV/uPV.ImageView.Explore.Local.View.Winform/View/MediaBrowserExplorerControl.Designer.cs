namespace uPV.ImageView.MediaBrowser.View
{
    partial class MediaBrowserExplorerControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._studyTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _studyTableView
            // 
            this._studyTableView.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this._studyTableView.ColumnHeaderTooltip = null;
            this._studyTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._studyTableView.Location = new System.Drawing.Point(0, 0);
            this._studyTableView.Name = "_studyTableView";
            this._studyTableView.ReadOnly = false;
            this._studyTableView.Size = new System.Drawing.Size(608, 297);
            this._studyTableView.SortButtonTooltip = null;
            this._studyTableView.TabIndex = 0;
            // 
            // MediaBrowserExplorerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this._studyTableView);
            this.Name = "MediaBrowserExplorerControl";
            this.Size = new System.Drawing.Size(608, 297);
            this.ResumeLayout(false);

        }


        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _studyTableView;
    }
}
