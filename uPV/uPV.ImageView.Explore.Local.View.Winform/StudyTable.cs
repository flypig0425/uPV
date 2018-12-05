using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.StudyManagement;

namespace uPV.ImageView.MediaBrowser
{
    public class StudyTable : Table<StudyItem>
    {
        public const string ColumnNamePatientId = @"Patient ID";
        public const string ColumnNamePatientName = @"Patient Name";
        public const string ColumnNameAccessionNumber = @"Accession Number";
        public const string ColumnNameStudyDate = @"Study Date";
        public const string ColumnNameStudyDescription = @"Study Description";
        public const string ColumnNameModality = @"Modality";
        public const string ColumnNameReferringPhysician = @"Referring Physician";
        public const string ColumnNameNumberOfInstances = @"Instances";

        private TableColumn<StudyItem, string> ColumnPatientId { get; set; }
        private TableColumn<StudyItem, string> ColumnPatientName { get; set; }
        private TableColumn<StudyItem, string> ColumnAccessionNumber { get; set; }
        private TableColumn<StudyItem, string> ColumnStudyDate { get; set; }
        private TableColumn<StudyItem, string> ColumnStudyDescription { get; set; }
        private TableColumn<StudyItem, string> ColumnModality { get; set; }
        private TableColumn<StudyItem, string> ColumnReferringPhysician { get; set; }
        private TableColumn<StudyItem, string> ColumnNumberOfInstances { get; set; }

        public bool UseSinglePatientNameColumn
        {
            get { return ColumnPatientName.Visible; }
            set
            {
                ColumnPatientName.Visible = value;
            }
        }

        public void Initialize()
        {
            AddDefaultColumns();

            Sort(new TableSortParams(ColumnPatientName, true));
        }

        public void SetColumnVisibility(string columnHeading, bool visible)
        {
            var column = FindColumn(columnHeading);
            if (column == null)
                return;

            column.Visible = visible;
        }

        private void AddDefaultColumns()
        {
            ColumnPatientId = new TableColumn<StudyItem, string>(
                ColumnNamePatientId,
                SR.ColumnHeadingPatientId,
                item => item.PatientId,
                0.5f);

            Columns.Add(ColumnPatientId);

            ColumnPatientName = new TableColumn<StudyItem, string>(
                ColumnNamePatientName,
                SR.ColumnHeadingPatientName,
                item => item.PatientsName.FormattedName,
                0.6f);

            Columns.Add(ColumnPatientName);
            //Hide by default.
            ColumnPatientName.Visible = true;


            ColumnAccessionNumber = new TableColumn<StudyItem, string>(
                ColumnNameAccessionNumber,
                SR.ColumnHeadingAccessionNumber,
                item => item.AccessionNumber,
                0.40F);

            Columns.Add(ColumnAccessionNumber);

            ColumnStudyDate = new TableColumn<StudyItem, string>(
                ColumnNameStudyDate,
                SR.ColumnHeadingStudyDate,
                item => FormatDicomDT(item.StudyDate, item.StudyTime),
                null,
                0.5F,
                (one, two) => CompareDicomDT(one.StudyDate, one.StudyTime, two.StudyDate, two.StudyTime));

            Columns.Add(ColumnStudyDate);

            ColumnStudyDescription = new TableColumn<StudyItem, string>(
                ColumnNameStudyDescription,
                SR.ColumnHeadingStudyDescription,
                item => item.StudyDescription,
                0.7F);

            Columns.Add(ColumnStudyDescription);

            ColumnModality = new TableColumn<StudyItem, string>(
                ColumnNameModality,
                SR.ColumnHeadingModality,
                item => StringUtilities.Combine(SortModalities(item.ModalitiesInStudy), ", "),
                0.25f);

            Columns.Add(ColumnModality);

            ColumnReferringPhysician = new TableColumn<StudyItem, string>(
                ColumnNameReferringPhysician,
                SR.ColumnHeadingReferringPhysician,
                entry => entry.ReferringPhysiciansName.FormattedName,
                0.5f);

            Columns.Add(ColumnReferringPhysician);

            ColumnNumberOfInstances = new TableColumn<StudyItem, string>(
                ColumnNameNumberOfInstances,
                SR.ColumnHeadingNumberOfInstances,
                item => item.NumberOfStudyRelatedInstances.HasValue ? item.NumberOfStudyRelatedInstances.ToString() : "",
                null,
                0.2f,
                delegate (StudyItem entry1, StudyItem entry2)
                {
                    int? instances1 = entry1.NumberOfStudyRelatedInstances;
                    int? instances2 = entry2.NumberOfStudyRelatedInstances;

                    if (instances1 == null)
                    {
                        if (instances2 == null)
                            return 0;
                        return 1;
                    }
                    if (instances2 == null)
                    {
                        return -1;
                    }

                    return -instances1.Value.CompareTo(instances2.Value);
                });

            Columns.Add(ColumnNumberOfInstances);
            
        }

        protected static string FormatDicomDT(string dicomDate, string dicomTime)
        {
            if (string.IsNullOrEmpty(dicomTime)) return FormatDicomDA(dicomDate); // if time is not specified, just format the date

            DateTime dateTime;
            if (!DateTimeParser.ParseDateAndTime(string.Empty, dicomDate, dicomTime, out dateTime))
                return dicomDate + ' ' + dicomTime;

            return dateTime.ToString(Format.DateTimeFormat);
        }

        protected static int CompareDicomDT(string dicomDate1, string dicomTime1, string dicomDate2, string dicomTime2)
        {
            var result = CompareDicomDA(dicomDate1, dicomDate2);
            return result != 0 ? result : CompareDicomTM(dicomTime1, dicomTime2);
        }

        protected static int CompareDicomDA(string dicomDate1, string dicomDate2)
        {
            return string.Compare(dicomDate1, dicomDate2, StringComparison.InvariantCultureIgnoreCase);
        }

        protected static int CompareDicomTM(string dicomTime1, string dicomTime2)
        {
            return string.Compare(dicomTime1, dicomTime2, StringComparison.InvariantCultureIgnoreCase);
        }

        private static string FormatDicomDA(string dicomDate)
        {
            DateTime date;
            if (!DateParser.Parse(dicomDate, out date))
                return dicomDate;

            return date.ToString(Format.DateFormat);
        }

        private static string[] SortModalities(IEnumerable<string> modalities)
        {
            var list = new List<string>(modalities);
            list.Remove(@"DOC"); // the DOC modality is a special case and handled via the attachments icon
            list.Sort((x, y) =>
            {
                var result = GetModalityPriority(x).CompareTo(GetModalityPriority(y));
                if (result == 0)
                    result = string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);
                return result;
            });
            return list.ToArray();
        }

        private static int GetModalityPriority(string modality)
        {
            const int imageModality = 0; // sort all known image modalities to top
            const int unknownModality = 1; // unknown modalities may be images or may simply be other documents - sort after known images, but before known ancillary documents
            const int srModality = 2;
            const int koModality = 3;
            const int prModality = 4;

            switch (modality)
            {
                case @"SR":
                    return srModality;
                case @"KO":
                    return koModality;
                case @"PR":
                    return prModality;
                default:
                    return StandardModalities.Modalities.Contains(modality) ? imageModality : unknownModality;
            }
        }

        public TableColumnBase<StudyItem> FindColumn(string columnHeading)
        {
            return Columns.FirstOrDefault(column => column.Name == columnHeading);
        }
    }
}
