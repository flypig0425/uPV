using ClearCanvas.Dicom.Iod;

namespace uPV.ImageView.MediaBrowser.MediaFileStore
{
    public abstract class Series : IKeyProvider, ISeriesData
    {
        private readonly Study _parentStudy;

        protected Series(string seriesInstanceUid, Study parentStudy)
        {
            SeriesInstanceUid = seriesInstanceUid;
            Sops = new SopCollection();
            _parentStudy = parentStudy;
        }
        
        public string SeriesInstanceUid { get; }

        public Study Study
        {
            get { return _parentStudy; }
        }

        public string StudyInstanceUid
        {
            get { return _parentStudy.StudyInstanceUid; }
        }

        public SopCollection Sops { get; }

        public abstract string Modality { get; }

        public abstract string SeriesDescription { get; }

        public abstract int SeriesNumber { get; }

        public abstract int? NumberOfSeriesRelatedInstances { get; }

        public string Key
        {
            get { return SeriesInstanceUid; }
        }

        public sealed class SopCollection : CollectionBase<SopInstance>
        {
            
        }
    }
}
