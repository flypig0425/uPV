using System.Linq;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.StudyManagement;
using uPV.ImageView.MediaBrowser.MediaFileStore;

namespace uPV.ImageView.MediaBrowser
{
    [ExtensionOf(typeof(StudyLoaderExtensionPoint))]
    internal sealed class DicomDirStudyLoader : StudyLoader
    {
        private Queue<SopInstance> _sops;

        public DicomDirStudyLoader() : base("MediaFileSet")
        {
            int? frameLookAhead = PreLoadingSettings.Default.FrameLookAheadCount;
            if (PreLoadingSettings.Default.LoadAllFrames)
                frameLookAhead = null;

            var coreStrategy = new SimpleCorePrefetchingStrategy(frame =>
                    frame.ParentImageSop.DataSource is LocalSopDataSource);
            PrefetchingStrategy = new WeightedWindowPrefetchingStrategy(coreStrategy, "MediaFileSet", 
                "Simple prefetcing strategy for local images.")
            {
                Enabled = PreLoadingSettings.Default.Enabled,
                RetrievalThreadConcurrency = PreLoadingSettings.Default.Concurrency,
                FrameLookAheadCount = frameLookAhead,
                SelectedImageBoxWeight = PreLoadingSettings.Default.SelectedImageBoxWeight,
                UnselectedImageBoxWeight = PreLoadingSettings.Default.UnselectedImageBoxWeight,
                DecompressionThreadConcurrency = 0
            };
        }

        protected override int OnStart(StudyLoaderArgs studyLoaderArgs)
        {
            _sops = null;
            MediaFileSet server = studyLoaderArgs.Server as MediaFileSet;

            if (server != null)
            {
                _sops = new Queue<SopInstance>();
                foreach (var study in server.Studies)
                {
                    if (studyLoaderArgs.StudyInstanceUid != study.StudyInstanceUid)
                        continue;

                    foreach (var series in study.Series)
                    {
                        foreach (var sop in series.Sops)
                        {
                            _sops.Enqueue(sop);
                        }
                    }
                }

                return _sops.Count;
            }

            return 0;
        }

        protected override SopDataSource LoadNextSopDataSource()
        {
            if (_sops != null && _sops.Count > 0)
            {
                var tmp = _sops.Dequeue();
                return new LocalSopDataSource(tmp.Filename);
            }

            return null;
        }
    }
}
