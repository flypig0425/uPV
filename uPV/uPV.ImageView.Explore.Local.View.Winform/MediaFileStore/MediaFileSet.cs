using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace uPV.ImageView.MediaBrowser.MediaFileStore
{
    public interface IKeyProvider
    {
        string Key { get; }
    }

    public interface IMediaFileSetReader
    {
        MediaFileSet Read(string filename);

        bool TryRead(string filename, out MediaFileSet mediaFileSet);
    }

    [ExtensionPoint]
    public class MediaFileSetReaderExtensionPoint : ExtensionPoint<IMediaFileSetReader>
    {
        
    }

    public abstract class MediaFileSet : IKeyProvider, IDicomServiceNode
    {
        protected MediaFileSet(string fileSetId)
        {
            Studies = new StudyCollection();
            FileSetId = fileSetId;
        }

        protected void AddStudy(Study study)
        {
            Studies.Add(study);
        }

        public string FileSetId { get; }

        public StudyCollection Studies { get; }

        #region IKeyProvider

        public string Key
        {
            get { return FileSetId; }
        }

        #endregion

        #region IDicomServiceNode   

        #region IApplicationEntity

        public string Name
        {
            get { return FileSetId; }
        }

        public string AETitle
        {
            get { return FileSetId; }
        }

        public string Description
        {
            get { return string.Empty; }
        }

        public string Location
        {
            get { return string.Empty; }
        }

        public IScpParameters ScpParameters
        {
            get { return null; }
        }

        public IStreamingParameters StreamingParameters
        {
            get { return null; }
        }

        #endregion


        #region IServiceNode

        public bool IsSupported<T>() where T : class
        {
            if (typeof(T) == typeof(IStudyFinder) ||
                typeof(T) == typeof(IStudyLoader))
                return true;

            return false;
        }

        public void GetService<T>(Action<T> withService) where T : class
        {
            ServiceNode.WithService(GetService<T>(), withService);
        }

        public T GetService<T>() where T : class
        {
            if (typeof(T) == typeof(IStudyLoader))
            {
                return new DicomDirStudyLoader() as T;
            }

            if (typeof(T) == typeof(IStudyFinder))
            {
                return new DicomDirStudyFinder() as T;
            }

            throw new NotSupportedException(string.Format("Service node doesn't support service '{0}'.",
                typeof(T).FullName));

        }

        #endregion

        public bool IsLocal { get {return true;} }

        #endregion

        public virtual ISopDataSource CreateSopDataSource(SopInstance sop)
        {
            return new LocalSopDataSource(sop.Filename);
        }

        public sealed class StudyCollection : CollectionBase<Study>
        {
        }
    }
}
