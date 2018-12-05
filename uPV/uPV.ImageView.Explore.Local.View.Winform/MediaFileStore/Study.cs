using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Dicom.Iod;

namespace uPV.ImageView.MediaBrowser.MediaFileStore
{
    public abstract class Study : IKeyProvider, IStudyRootData
    {
        private readonly MediaFileSet _parentFileSet;

        public string FileSetId
        {
            get { return _parentFileSet.FileSetId; }
        }

        public string StudyInstanceUid { get; }

        public MediaFileSet MediaFileSet
        {
            get { return _parentFileSet; }
        }

        public SeriesCollection Series { get; }

        #region abstract 

        public abstract string[] SopClassesInStudy { get; }

        public abstract string[] ModalitiesInStudy { get; }

        public abstract string SpecificCharacterSet { get; }

        public abstract string StudyDescription { get; }

        public abstract string StudyId { get; }

        public abstract string StudyDate { get; }

        public abstract string StudyTime { get; }

        public abstract string AccessionNumber { get; }

        public abstract string ReferringPhysiciansName { get; }

        public abstract int? NumberOfStudyRelatedSeries { get; }

        public abstract int? NumberOfStudyRelatedInstances { get; }

        public abstract string PatientId { get; }

        public abstract string PatientsName { get; }

        public abstract string PatientsBirthDate { get; }

        public abstract string PatientsBirthTime { get; }

        public abstract string PatientsSex { get; }

        public abstract string PatientsAge { get; }

        public abstract string PatientSpeciesDescription { get; }

        public abstract string PatientSpeciesCodeSequenceCodingSchemeDesignator { get; }

        public abstract string PatientSpeciesCodeSequenceCodeValue { get; }

        public abstract string PatientSpeciesCodeSequenceCodeMeaning { get; }

        public abstract string PatientBreedDescription { get; }

        public abstract string PatientBreedCodeSequenceCodingSchemeDesignator { get; }

        public abstract string PatientBreedCodeSequenceCodeValue { get; }

        public abstract string PatientBreedCodeSequenceCodeMeaning { get; }

        public abstract string ResponsiblePerson { get; }

        public abstract string ResponsiblePersonRole { get; }

        public abstract string ResponsibleOrganization { get; }

        #endregion

        public string Key
        {
            get { return StudyInstanceUid; }
        }
        protected Study(string studyInstanceUid, MediaFileSet parentMediaFileSet)
        {
            StudyInstanceUid = studyInstanceUid;
            Series = new SeriesCollection();
            _parentFileSet = parentMediaFileSet;
        }

        public sealed class SeriesCollection : CollectionBase<Series>
        {
            
        }
        
    }
}
