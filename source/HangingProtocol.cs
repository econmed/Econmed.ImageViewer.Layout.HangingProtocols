using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.StudyManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Econmed.ImageViewer.Layout.HangingProtocols
{
    [XmlType("hanging-protocol")]
    public class HangingProtocol
    {
        [DefaultValue(4)]
        [XmlAttribute("residual-workspace-rows")]
        public int ResidualWorkspaceRows { get; set; }

        [DefaultValue(4)]
        [XmlAttribute("residual-workspace-columns")]
        public int ResidualWorkspaceColumns { get; set; }

        [DefaultValue(true)]
        [XmlAttribute("show-residual-workspace")]
        public bool ShowResidualWorkspace { get; set; }

        [XmlElement("study-filter")]
        public StudyFilter StudyFilter { get; set; }

        [XmlElement("image-filter")]
        public List<ImageFilter> ImageFilters { get; set; }

        [XmlElement("workspace")]
        public List<WorkspaceLayout> Workspaces { get; set; }

        public HangingProtocol()
        {
            ResidualWorkspaceRows = 4;
            ResidualWorkspaceColumns = 4;
            ShowResidualWorkspace = true;
            StudyFilter = new StudyFilter();
            ImageFilters = new List<ImageFilter>();
            Workspaces = new List<WorkspaceLayout>();
        }

        public ImageFilter GetImageFilter(string name)
        {
            return ImageFilters.Find(f => f.Name == name);
        }
    }

    [XmlType("workspace")]
    public class WorkspaceLayout
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [DefaultValue(false)]
        [XmlAttribute("optional")]
        public bool Optional { get; set; }

        [XmlAttribute("rows")]
        public int Rows { get; set; }

        [XmlAttribute("columns")]
        public int Columns { get; set; }

        [XmlElement("image-box")]
        public List<ImageBoxLayout> ImageBoxes { get; set; }

        public WorkspaceLayout()
        {
            Name = SR.WorkspaceDefaultName;
            Optional = false;
            ImageBoxes = new List<ImageBoxLayout>();
        }

        public Boolean ShouldSerializeName()
        {
            return Name != SR.WorkspaceDefaultName;
        }

        public void ResetMyProperty()
        {
            Name = SR.WorkspaceDefaultName;
        }
    }

    [XmlType("image-box")]
    public class ImageBoxLayout
    {
        [DefaultValue(1)]
        [XmlAttribute("tile-rows")]
        public int TileRows { get; set; }

        [DefaultValue(1)]
        [XmlAttribute("tile-columns")]
        public int TileColumns { get; set; }

        [XmlAttribute("filter")]
        public string FilterName { get; set; }

        [DefaultValue(false)]
        [XmlAttribute("prior-study")]
        public bool PriorStudy { get; set; }

        public ImageBoxLayout()
        {
            TileRows = 1;
            TileColumns = 1;
            PriorStudy = false;
        }
    }

    public class AppliedWorkspace
    {
        public WorkspaceLayout WorkspaceLayout { get; set; }
        public List<Tuple<ImageBoxLayout, IDisplaySet>> DisplaySets { get; set; }

        public AppliedWorkspace()
        {
            DisplaySets = new List<Tuple<ImageBoxLayout, IDisplaySet>>();
        }
    }

    public class HangingProtocolLayoutProvider : ILayoutProvider
    {
        public bool CanHandle(Study study)
        {
            return hangingProtocolsContainer.Value.HangingProtocols.Any(h => h.StudyFilter.Test(study).Success);
        }

        [XmlType("hanging-protocols")]
        public class HangingProtocolsContainer
        {
            [XmlElement("hanging-protocol")]
            public List<HangingProtocol> HangingProtocols { get; set; }
        }

        Lazy<HangingProtocolsContainer> hangingProtocolsContainer = new Lazy<HangingProtocolsContainer>(() =>
        {
            try
            {
                var layoutSettingsXml = LayoutSettings.Default.LayoutSettingsXml;
                var serializer = new XmlSerializer(typeof(HangingProtocolsContainer));
                using (var reader = new XmlNodeReader(layoutSettingsXml.DocumentElement))
                {
                    return serializer.Deserialize(reader) as HangingProtocolsContainer;
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException(SR.MessageErrorWhileLoadingHangingProtocols, e);
            }
        });

        public IEnumerable<AppliedWorkspace> MakeLayouts(ILogicalWorkspace logicalWorkspace)
        {
            var primaryStudy = logicalWorkspace.ImageViewer.StudyTree.Studies.First();
            var hangingProtocol = hangingProtocolsContainer.Value.HangingProtocols.Find(h => h.StudyFilter.Test(primaryStudy).Success);
            if (null == hangingProtocol)
            {
                throw new ArgumentException(string.Format(SR.MessageNoMatchingHangingProtocol, primaryStudy.ParentPatient, primaryStudy));
            }

            var imageSets = GetImageSets(logicalWorkspace, hangingProtocol);
            var primaryImageSet = imageSets.First();
            var residualDisplaySets = primaryImageSet.DisplaySets.ToList();
            var studyHasPriors = imageSets.Count() > 1;
            var priorImageSets = studyHasPriors ? imageSets.Skip(1) : new List<IImageSet>() { null };

            foreach (var iw in priorImageSets.Zip(
                OneWorkspacesWithAndTheRestWithoutPrimary(hangingProtocol.Workspaces), (i, w) => new { Workspaces = w, priorImageSet = i }))
            {
                foreach (var workspace in iw.Workspaces)
                {
                    if (workspace.ImageBoxes.Count != workspace.Rows * workspace.Columns)
                    {
                        throw new ArgumentException(string.Format(SR.MessageAmountOfImageBoxesMismatch, workspace.ImageBoxes.Count, workspace.Rows, workspace.Columns, workspace.Name));
                    }
                    var appliedWorkspace = new AppliedWorkspace() { WorkspaceLayout = workspace };
                    bool atLeastOneMatchingDisplaySet = false;
                    if (!studyHasPriors && workspace.ImageBoxes.Any(i => i.PriorStudy))
                    {
                        continue;
                    }

                    foreach (var imageBox in workspace.ImageBoxes)
                    {
                        var imageSet = imageBox.PriorStudy ? iw.priorImageSet : primaryImageSet;
                        var displaySet = FindDisplaySet(hangingProtocol, imageSet, imageBox);
                        if (null != displaySet)
                        {
                            residualDisplaySets.Remove(displaySet);
                            atLeastOneMatchingDisplaySet = true;
                        }
                        else {
                            displaySet = MessagePresentationImage.CreateDisplaySet(
                                string.Format(SR.MessageNoMatchingDisplaySet, imageBox.FilterName, null != imageSet ? imageSet.Name : string.Empty, workspace.Name));
                        }
                        appliedWorkspace.DisplaySets.Add(new Tuple<ImageBoxLayout, IDisplaySet>(imageBox, displaySet));
                    }
                    if (!workspace.Optional || atLeastOneMatchingDisplaySet)
                    {
                        yield return appliedWorkspace;
                    }
                }
            }

            if (hangingProtocol.ShowResidualWorkspace && 0 != residualDisplaySets.Count)
            {
                var workspace = new WorkspaceLayout()
                {
                    Columns = hangingProtocol.ResidualWorkspaceColumns,
                    Rows = hangingProtocol.ResidualWorkspaceRows
                };
                var residualImageBoxCount = workspace.Columns * workspace.Rows;
                var imageBox = new ImageBoxLayout();
                var appliedWorkspace = new AppliedWorkspace() { WorkspaceLayout = workspace };
                var index = 0;

                var missingImageBoxCount = residualImageBoxCount - (residualDisplaySets.Count % residualImageBoxCount);
                for (int i = 0; i < missingImageBoxCount; i++)
                {
                    residualDisplaySets.Add(null);
                }

                foreach (var displaySet in residualDisplaySets)
                {
                    appliedWorkspace.DisplaySets.Add(new Tuple<ImageBoxLayout, IDisplaySet>(imageBox, displaySet));
                    index++;
                    if (index % residualImageBoxCount == 0)
                    {
                        yield return appliedWorkspace;
                        appliedWorkspace = new AppliedWorkspace() { WorkspaceLayout = workspace };
                    }
                }
            }
        }

        static IEnumerable<List<WorkspaceLayout>> OneWorkspacesWithAndTheRestWithoutPrimary(List<WorkspaceLayout> workspaces)
        {
            yield return workspaces;
            var priors = workspaces.Where(w => w.ImageBoxes.Any(i => i.PriorStudy)).ToList();
            while (true)
            {
                yield return priors;
            }
        }

        public Func<IImageSet, bool> ImageSetContains(Func<IPresentationImage, bool> imageFilter)
        {
            return imageSet =>
            {
                foreach (var displaySet in imageSet.DisplaySets)
                {
                    foreach (var presentationImage in displaySet.PresentationImages)
                    {
                        if (imageFilter(presentationImage)) { return true; }
                    }
                }
                return false;
            };
        }

        public IEnumerable<IImageSet> GetImageSets(ILogicalWorkspace logicalWorkspace, HangingProtocol hangingProtocol)
        {
            string primaryStudyInstanceUid = logicalWorkspace.ImageViewer.StudyTree.Studies.First().StudyInstanceUid;
            List<string> allStudyInstanceUids = logicalWorkspace.ImageViewer.StudyTree.Studies
                .Where(s => hangingProtocol.StudyFilter.Test(s).Success)
                .Select(s => s.StudyInstanceUid).ToList();
            return logicalWorkspace.ImageSets
                .SkipWhile(i => i.Descriptor.Uid != primaryStudyInstanceUid)
                .Where(i => allStudyInstanceUids.Contains(i.Descriptor.Uid));
        }

        public IDisplaySet FindDisplaySet(HangingProtocol hangingProtocol, IImageSet imageSet, ImageBoxLayout imageBox)
        {
            if (null == imageSet) { return null; }
            foreach (var displaySet in imageSet.DisplaySets)
            {
                foreach (var presentationImage in displaySet.PresentationImages)
                {
                    var filter = hangingProtocol.GetImageFilter(imageBox.FilterName);
                    if (null == filter ||
                        filter.Test(presentationImage).Success)
                    {
                        return displaySet;
                    }
                }
            }
            return null;
        }
    }
}
