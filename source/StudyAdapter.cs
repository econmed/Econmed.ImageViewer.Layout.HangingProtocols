using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using System;

namespace Econmed.ImageViewer.Layout.HangingProtocols
{
    public class StudyDicomAttributeProviderAdapter : IDicomAttributeProvider
    {
        Study study;

        public StudyDicomAttributeProviderAdapter(Study study)
        {
            this.study = study;
        }

        public DicomAttribute this[uint tag]
        {
            get
            {
                DicomAttribute attribute;
                if (TryGetAttribute(tag, out attribute)) { return attribute; }
                throw new ArgumentException(string.Format("DICOM tag {0:X8} is missing.", tag));
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public DicomAttribute this[DicomTag tag]
        {
            get
            {
                return this[tag.TagValue];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
        {
            return TryGetAttribute(tag.TagValue, out attribute);
        }

        public bool TryGetAttribute(uint tag, out DicomAttribute attribute)
        {
            attribute = DicomTagDictionary.GetDicomTag(tag).CreateDicomAttribute();
            switch (tag)
            {
                case DicomTags.ModalitiesInStudy:
                    attribute.Values = study.ModalitiesInStudy;
                    return true;
                case DicomTags.StudyDescription:
                    attribute.SetStringValue(study.StudyDescription);
                    return true;
                case DicomTags.AccessionNumber:
                    attribute.SetStringValue(study.AccessionNumber);
                    return true;
                case DicomTags.NumberOfStudyRelatedInstances:
                    attribute.SetInt32(0, study.NumberOfStudyRelatedInstances);
                    return true;
                case DicomTags.NumberOfStudyRelatedSeries:
                    attribute.SetInt32(0, study.NumberOfStudyRelatedSeries);
                    return true;
                case DicomTags.PatientsAge:
                    attribute.SetStringValue(study.PatientsAge);
                    return true;
                case DicomTags.ReferringPhysiciansName:
                    attribute.SetStringValue(study.ReferringPhysiciansName);
                    return true;
                case DicomTags.SopClassesInStudy:
                    attribute.Values = study.SopClassesInStudy;
                    return true;
                case DicomTags.StudyId:
                    attribute.SetStringValue(study.StudyId);
                    return true;
                case DicomTags.StudyDate:
                    attribute.SetStringValue(study.StudyDate);
                    return true;
                case DicomTags.StudyTime:
                    attribute.SetStringValue(study.StudyTime);
                    return true;
                default:
                    return false;
            }
        }
    }
}
