using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace uPV.ImageView.MediaBrowser.MediaFileStore.DicomDir
{
    [ExtensionOf(typeof(MediaFileSetReaderExtensionPoint))]
    internal class DicomDirReader : IMediaFileSetReader
    {
        public MediaFileSet Read(string filename)
        {
            DicomFile file = new DicomFile(filename);

            file.Load(DicomTags.SopClassUid, DicomReadOptions.Default);

            if (file.MediaStorageSopClassUid == SopClass.MediaStorageDirectoryStorageUid)
            {
                return DicomDirFileSet.Load(filename);
            }
            else
            {
                if (file.DataSet[DicomTags.SopClassUid] != SopClass.MediaStorageDirectoryStorageUid)
                {
                    return null;
                }

                return DicomDirFileSet.Load(filename);
            }
            
        }

        public bool TryRead(string filename, out MediaFileSet mediaFileSet)
        {
            try
            {
                mediaFileSet = Read(filename);
            }
            catch (Exception)
            {
                mediaFileSet = null;
            }

            return mediaFileSet != null;
        }
    }
}
