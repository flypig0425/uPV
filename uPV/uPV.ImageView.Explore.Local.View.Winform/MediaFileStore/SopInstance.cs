using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Dicom.Iod;

namespace uPV.ImageView.MediaBrowser.MediaFileStore
{
    public abstract class SopInstance : IKeyProvider, ISopInstanceData
    {
        protected SopInstance(string sopInstanceUid, Series parentSeries)
        {
            SopInstanceUid = sopInstanceUid;

            Series = parentSeries;
        }

        public string Key
        {
            get { return SopInstanceUid; }
        }

        public Series Series { get; }

        public abstract string SopClassUid { get; }

        public abstract int InstanceNumber { get; }

        public abstract string Filename { get; }

        public string StudyInstanceUid
        {
            get { return Series.Study.StudyInstanceUid; }
        }

        public string SeriesInstanceUid
        {
            get { return Series.SeriesInstanceUid; }
        }

        public string SopInstanceUid { get; }

        
    }
}
