using System;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.StudyManagement;
using uPV.ImageView.MediaBrowser.MediaFileStore;

namespace uPV.ImageView.MediaBrowser
{
    [ExtensionOf(typeof(StudyFinderExtensionPoint))]
    internal sealed class DicomDirStudyFinder : StudyFinder
    {
        public DicomDirStudyFinder() : base("MediaFileSet")
        {
        }

        public override StudyItemList Query(QueryParameters queryParams,
            IApplicationEntity targetServer)
        {
            var studyItems = new StudyItemList();
            var tmp = targetServer as MediaFileSet;

            try
            {
                if (tmp != null)
                {
                    studyItems.AddRange(tmp.Studies.Select(study => new StudyItem(study, tmp)));
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e.Message);
                
            }
            

            return studyItems;
        }
    }
}
