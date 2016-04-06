using ClearCanvas.Common;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Layout;
using ClearCanvas.ImageViewer.StudyManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Econmed.ImageViewer.Layout.HangingProtocols
{
    public interface ILayoutProvider
    {
        bool CanHandle(Study study);
        IEnumerable<AppliedWorkspace> MakeLayouts(ILogicalWorkspace logicalWorkspace);
    }

    [ExtensionOf(typeof(HpLayoutHookExtensionPoint))]
    public class LayoutHook : IHpLayoutHook
    {
        int currentLayoutIndex = 0;
        List<AppliedWorkspace> layouts;
        ILayoutProvider layoutProvider = new HangingProtocolLayoutProvider();

        public bool HandleLayout(IHpLayoutHookContext context)
        {
            return UpdateLayouts(context.ImageViewer);
        }

        public bool UpdateLayouts(IImageViewer viewer)
        {
            if (!layoutProvider.CanHandle(viewer.StudyTree.Studies.First())) { return false; }
            layouts = new List<AppliedWorkspace>(layoutProvider.MakeLayouts(viewer.LogicalWorkspace));
            if (layouts.Count == 0) { return false; }
            ApplyLayout(viewer, layouts.ElementAt(0));
            return true;
        }

        public bool NextLayoutAvailable { get { return null != layouts && currentLayoutIndex < layouts.Count - 1; } }
        public bool PreviousLayoutAvailable { get { return currentLayoutIndex > 0; } }

        public void ApplyNextLayout(IImageViewer viewer)
        {
            if (NextLayoutAvailable)
            {
                currentLayoutIndex++;
            }
            else
            {
                currentLayoutIndex = 0;
            }
            ApplyLayout(viewer, layouts[currentLayoutIndex]);
        }

        public void ApplyPreviousLayout(IImageViewer viewer)
        {
            if (PreviousLayoutAvailable)
            {
                currentLayoutIndex--;
            }
            else
            {
                currentLayoutIndex = layouts.Count - 1;
            }
            ApplyLayout(viewer, layouts[currentLayoutIndex]);
        }

        public void ApplyLayout(IImageViewer viewer, AppliedWorkspace layout)
        {
            if (layout.DisplaySets.Count != layout.WorkspaceLayout.Rows * layout.WorkspaceLayout.Columns)
            {
                throw new ArgumentException(string.Format(SR.MessageAmountOfImageBoxesMismatch, layout.DisplaySets.Count, layout.WorkspaceLayout.Rows, layout.WorkspaceLayout.Columns));
            }
            IPhysicalWorkspace workspace = viewer.PhysicalWorkspace;
            workspace.SetImageBoxGrid(layout.WorkspaceLayout.Rows, layout.WorkspaceLayout.Columns);
            var imageBoxIndex = 0;
            foreach (var displaySetLayout in layout.DisplaySets)
            {
                var imageBox = workspace.ImageBoxes[imageBoxIndex];

                imageBox.SetTileGrid(
                    displaySetLayout.Item1.TileRows,
                    displaySetLayout.Item1.TileColumns);
                if (null != displaySetLayout.Item2)
                {
                    imageBox.DisplaySet = displaySetLayout.Item2.CreateFreshCopy();
                }
                imageBoxIndex++;
            }
        }
    }

}

