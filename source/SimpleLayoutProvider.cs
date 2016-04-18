using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Layout.Basic;
using ClearCanvas.ImageViewer.StudyManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Econmed.ImageViewer.Layout.HangingProtocols
{
    public class SimpleLayoutProvider : ILayoutProvider
    {
        public bool CanHandle(Study study)
        {
            return true;
        }

        public IEnumerable<IImageSet> GetImageSets(ILogicalWorkspace logicalWorkspace)
        {
            string primaryStudyInstanceUid = logicalWorkspace.ImageViewer.StudyTree.Studies.First().StudyInstanceUid;
            return logicalWorkspace.ImageSets
                .SkipWhile(i => i.Descriptor.Uid != primaryStudyInstanceUid);
        }

        public IEnumerable<AppliedWorkspace> MakeLayouts(ILogicalWorkspace logicalWorkspace)
        {
            var imageSets = GetImageSets(logicalWorkspace);

            foreach (var imageSet in imageSets)
            {

                StoredLayout layout = LayoutSettingsHelper.MinimumLayout;
                foreach (IDisplaySet displaySet in imageSet.DisplaySets)
                {
                    if (displaySet.PresentationImages.Count <= 0) { continue; }
                    StoredLayout storedLayout = LayoutSettingsHelper.GetLayout(displaySet.PresentationImages[0] as IImageSopProvider);
                    layout.ImageBoxRows = Math.Max(layout.ImageBoxRows, storedLayout.ImageBoxRows);
                    layout.ImageBoxColumns = Math.Max(layout.ImageBoxColumns, storedLayout.ImageBoxColumns);
                    layout.TileRows = Math.Max(layout.TileRows, storedLayout.TileRows);
                    layout.TileColumns = Math.Max(layout.TileColumns, storedLayout.TileColumns);
                }
                var imageBoxCount = layout.ImageBoxColumns * layout.ImageBoxRows;
                var displaySetPerWorkspace = imageSet.DisplaySets.Select((item, index) => new { index, item })
                       .GroupBy(x => x.index / imageBoxCount)
                       .Select(x => x.Select(y => y.item));

                var imageBoxLayout = new ImageBoxLayout() { TileColumns = layout.TileColumns, TileRows = layout.TileRows };
                foreach (var displaySets in displaySetPerWorkspace)
                {
                    var workspaceDisplaySets = displaySets.Select((displaySet) => Tuple.Create(imageBoxLayout, displaySet)).ToList();
                    for (int i = workspaceDisplaySets.Count; i < imageBoxCount; i++)
                    {
                        workspaceDisplaySets.Add(Tuple.Create<ImageBoxLayout, IDisplaySet>(imageBoxLayout, null));
                    }
                    yield return new AppliedWorkspace()
                    {
                        WorkspaceLayout =
                            new WorkspaceLayout()
                            {
                                Columns = layout.ImageBoxColumns,
                                Rows = layout.ImageBoxRows
                            },
                        DisplaySets = workspaceDisplaySets
                    };
                }
            }
        }
    }
}
