using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace uPV.ImageView.MediaBrowser.MediaFileStore.DicomDir
{
    internal sealed class DicomDirFileSet : MediaFileSet
    {
        public DicomDirFileSet(string fileSetId) : base(fileSetId)
        {
        }

        public override ISopDataSource CreateSopDataSource(SopInstance sop)
        {
            var instance = sop as LocalInstance;
            if (instance != null)
            {
                return new LocalSopDataSource(instance.Filename);
            }

            return base.CreateSopDataSource(sop);
        }

        public static DicomDirFileSet Load(string filename)
        {
            var dicomDirectory = new DicomDirectory(String.Empty);

            dicomDirectory.Load(filename);

            var directoryName = Path.GetDirectoryName(filename);

            DicomDirFileSet tmp = new DicomDirFileSet(dicomDirectory.FileSetId);

            foreach (var item in dicomDirectory.RootDirectoryRecordCollection)
            {
                if (item.DirectoryRecordType != DirectoryRecordType.Patient)
                {
                    throw new InvalidOperationException();
                }

                foreach (var subItem in item.LowerLevelDirectoryRecordCollection)
                {
                    if (subItem.DirectoryRecordType == DirectoryRecordType.Study)
                    {
                        try
                        {
                            tmp.Studies.Add(new LocalStudy(subItem, tmp, directoryName));
                        }
                        catch (Exception)
                        {
                            // TODO 
                            Platform.Log(LogLevel.Warn, "invaild data when load dicom dir");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            return tmp;
        }

        public static string RelativePath(string dirName, DicomAttribute attribute)
        {
            var path = dirName;

            for (int i = 0; i < attribute.Count; i++)
            {
                path = Path.Combine(path, attribute.GetString(i, string.Empty));
            }

            return path;
        }

        #region Private Class 

        internal sealed class LocalInstance : SopInstance
        {
            private DicomFile _dicomFile;
            private readonly string _filename;

            public LocalInstance(DirectoryRecordSequenceItem item, LocalSeries parent, string dirName)
                : base(item[DicomTags.ReferencedSopInstanceUidInFile], parent)
            {
                _filename = RelativePath(dirName, item[DicomTags.ReferencedFileId]);

                if (!File.Exists(_filename))
                    throw new FileNotFoundException();
            }

            public DicomFile DicomFile
            {
                get
                {
                    if (_dicomFile == null)
                    {
                        _dicomFile = new DicomFile(_filename);
                        _dicomFile.Load(DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences);
                    }

                    return _dicomFile;
                }
            }

            public override string SopClassUid
            {
                get { return DicomFile.SopClass.Uid; }
            }

            public override int InstanceNumber
            {
                get { return DicomFile.DataSet[DicomTags.IndicationNumber].GetInt32(0, 0); }
            }

            public override string Filename
            {
                get { return _filename; }
            }
        }

        internal sealed class LocalSeries : Series
        {
            public LocalSeries(DirectoryRecordSequenceItem item, LocalStudy parent, string dirName) :
                base(item[DicomTags.SeriesInstanceUid], parent)
            {
                foreach (var subItem in item.LowerLevelDirectoryRecordCollection)
                {
                    if (subItem.DirectoryRecordType != DirectoryRecordType.Image)
                    {
                        continue;
                    }

                    Sops.Add(new LocalInstance(subItem, this, dirName));
                }
            }

            private DicomFile DicomFile
            {
                get
                {
                    foreach (var sop in Sops)
                    {
                        var tmp = sop as LocalInstance;
                        if (tmp != null)
                            return tmp.DicomFile;
                    }

                    throw new InvalidOperationException();
                }
            }

            public override string Modality
            {
                get { return DicomFile.DataSet[DicomTags.Modality]; }
            }

            public override string SeriesDescription
            {
                get { return DicomFile.DataSet[DicomTags.SegmentDescription]; }
            }

            public override int SeriesNumber
            {
                get { return DicomFile.DataSet[DicomTags.SeriesNumber].GetInt32(0, 0); }
            }

            public override int? NumberOfSeriesRelatedInstances
            {
                get { return Sops.Count; }
            }
        }

        internal sealed class LocalStudy : Study
        {
            public LocalStudy(DirectoryRecordSequenceItem item, MediaFileSet parentMediaFileSet, string dirName)
                : base(GetUID(item), parentMediaFileSet)
            {
                foreach (var subItem in item.LowerLevelDirectoryRecordCollection)
                {
                    if (subItem.DirectoryRecordType != DirectoryRecordType.Series)
                    {
                        continue;
                    }

                    Series.Add(new LocalSeries(subItem, this, dirName));
                }
            }

            private DicomFile DicomFile
            {
                get
                {
                    foreach (var series in Series)
                    {
                        foreach (var sop in series.Sops)
                        {
                            var tmp = sop as LocalInstance;
                            if (tmp != null)
                                return tmp.DicomFile;
                        }
                    }

                    throw new InvalidOperationException();
                }
            }

            public override string[] SopClassesInStudy
            {
                get
                {
                    return (from series in Series
                            from sop in series.Sops
                            select sop.SopClassUid).Distinct().ToArray();
                }
            }

            public override string[] ModalitiesInStudy
            {
                get
                {
                    return Series.Select(series => series.Modality).Distinct().ToArray();
                }
            }

            public override string StudyDescription
            {
                get
                {
                    return DicomFile.DataSet[DicomTags.StudyDescription];
                }
            }

            public override string StudyId
            {
                get { return DicomFile.DataSet[DicomTags.StudyId]; }
            }

            public override string StudyDate
            {
                get { return DicomFile.DataSet[DicomTags.StudyDate]; }
            }

            public override string StudyTime
            {
                get { return DicomFile.DataSet[DicomTags.StudyTime]; }
            }

            public override string AccessionNumber
            {
                get { return DicomFile.DataSet[DicomTags.AccessionNumber]; }
            }

            public override string ReferringPhysiciansName
            {
                get { return DicomFile.DataSet[DicomTags.ReferringPhysiciansName]; }
            }

            public override string PatientsAge
            {
                get { return DicomFile.DataSet[DicomTags.PatientsAge]; }
            }

            public override string PatientSpeciesDescription
            {
                get { return DicomFile.DataSet[DicomTags.PatientSpeciesDescription]; }
            }

            public override string PatientSpeciesCodeSequenceCodingSchemeDesignator
            {
                get { return DicomFile.DataSet[DicomTags.StudyId]; }
            }

            public override string PatientSpeciesCodeSequenceCodeValue
            {
                get { return DicomFile.DataSet[DicomTags.StudyId]; }
            }

            public override string PatientSpeciesCodeSequenceCodeMeaning
            {
                get { return DicomFile.DataSet[DicomTags.StudyId]; }
            }

            public override string PatientBreedDescription
            {
                get { return DicomFile.DataSet[DicomTags.PatientBreedDescription]; }
            }

            public override string PatientBreedCodeSequenceCodingSchemeDesignator
            {
                get { return DicomFile.DataSet[DicomTags.StudyId]; }
            }

            public override string PatientBreedCodeSequenceCodeValue
            {
                get { return DicomFile.DataSet[DicomTags.StudyId]; }
            }

            public override string PatientBreedCodeSequenceCodeMeaning
            {
                get { return DicomFile.DataSet[DicomTags.StudyId]; }
            }

            public override string ResponsiblePerson
            {
                get { return DicomFile.DataSet[DicomTags.ResponsiblePerson]; }
            }

            public override string ResponsiblePersonRole
            {
                get { return DicomFile.DataSet[DicomTags.ResponsiblePersonRole]; }
            }

            public override string ResponsibleOrganization
            {
                get { return DicomFile.DataSet[DicomTags.ResponsibleOrganization]; }
            }

            public override int? NumberOfStudyRelatedSeries
            {
                get { return Series.Count; }
            }

            public override int? NumberOfStudyRelatedInstances
            {
                get { return Series.Sum(series => series.Sops.Count); }
            }

            public override string PatientId
            {
                get { return DicomFile.DataSet[DicomTags.PatientId]; }
            }

            public override string PatientsName
            {
                get { return DicomFile.DataSet[DicomTags.PatientsName]; }
            }

            public override string PatientsBirthDate
            {
                get { return DicomFile.DataSet[DicomTags.PatientsBirthDate]; }
            }

            public override string PatientsBirthTime
            {
                get { return DicomFile.DataSet[DicomTags.PatientsBirthTime]; }
            }

            public override string PatientsSex
            {
                get { return DicomFile.DataSet[DicomTags.PatientsSex]; }
            }

            public override string SpecificCharacterSet
            {
                get { return DicomFile.DataSet[DicomTags.SpecificCharacterSet]; }
            }

            private static string GetUID(IDicomAttributeProvider provider)
            {
                string str = provider[DicomTags.StudyInstanceUid];

                return str;
            }
        }

        #endregion
    }
}
