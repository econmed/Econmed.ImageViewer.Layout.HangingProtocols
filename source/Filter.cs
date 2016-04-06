using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom.Utilities.Rules.Specifications;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.StudyManagement;
using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace Econmed.ImageViewer.Layout.HangingProtocols
{
    public abstract class Filter<T>
    {
        [DefaultValue(null)]
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("condition")]
        public XmlElement Condition { get; set; }

        static XmlSpecificationCompiler GetSpecificationCompiler()
        {
            return new XmlSpecificationCompiler("dicom-patched", new DicomRuleSpecificationCompilerOperatorExtensionPoint());
        }

        static XmlElement EncapsulateWithElement(XmlElement node, string name)
        {
            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(name));
            doc.DocumentElement.AppendChild(doc.ImportNode(node, true));
            return doc.DocumentElement;
        }

        protected abstract object GetTestObject(T data);

        Func<T, TestResult> compiledFilter;

        public Func<T, TestResult> Compile()
        {
            if (null != compiledFilter) { return compiledFilter; }
            var specification = GetSpecificationCompiler()
                .Compile(EncapsulateWithElement(Condition, "condition"), true);
            return compiledFilter = data =>
            {
                return specification.Test(GetTestObject(data));
            };
        }

        public TestResult Test(T data)
        {
            return Compile()(data);
        }
    }

    [XmlType("image-filter")]
    public class ImageFilter : Filter<IPresentationImage>
    {
        protected override object GetTestObject(IPresentationImage data)
        {
            var imageSopProvider = data as IImageSopProvider;
            return imageSopProvider.ImageSop.DataSource;
        }
    }

    [XmlType("study-filter")]
    public class StudyFilter : Filter<Study>
    {
        protected override object GetTestObject(Study data)
        {
            return new StudyDicomAttributeProviderAdapter(data);
        }
    }
}
