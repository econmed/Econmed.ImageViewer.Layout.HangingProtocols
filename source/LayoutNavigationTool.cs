using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer;
using System;

namespace Econmed.ImageViewer.Layout.HangingProtocols
{
    [MenuAction("previous", "global-menus/MenuTools/MenuStandard/MenuPreviousDisplaySets", "PreviousDisplaySets")]
    [ButtonAction("previous", "global-toolbars/ToolbarStandard/ToolbarPreviousDisplaySets", "PreviousDisplaySets", KeyStroke = XKeys.Shift | XKeys.Tab)]
    [Tooltip("previous", "TooltipPreviousDisplaySets")]
    [IconSet("previous", "Icons.PreviousSmall.png", "Icons.PreviousMedium.png", "Icons.PreviousLarge.png")]
    [GroupHint("previous", "Tools.Navigation.DisplaySets.Previous.DisplaySets")]
    [EnabledStateObserver("previous", "PreviousEnabled", "PreviousEnabledChanged")]
    [MenuAction("next", "global-menus/MenuTools/MenuStandard/MenuNextDisplaySets", "NextDisplaySets")]
    [ButtonAction("next", "global-toolbars/ToolbarStandard/ToolbarNextDisplaySets", "NextDisplaySets", KeyStroke = XKeys.Tab)]
    [Tooltip("next", "TooltipNextDisplaySets")]
    [IconSet("next", "Icons.NextSmall.png", "Icons.NextMedium.png", "Icons.NextLarge.png")]
    [GroupHint("next", "Tools.Navigation.DisplaySets.Next.DisplaySets")]
    [EnabledStateObserver("next", "NextEnabled", "NextEnabledChanged")]
    [ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public class LayoutNavigationTool : Tool<IImageViewerToolContext>
    {
        private bool nextEnabled = true;
        public event EventHandler NextEnabledChanged;
        public bool NextEnabled
        {
            get { return nextEnabled; }
            protected set
            {
                if (nextEnabled != value)
                {
                    nextEnabled = value;
                    EventsHelper.Fire(NextEnabledChanged, this, EventArgs.Empty);
                }
            }
        }

        private bool previousEnabled = true;
        public event EventHandler PreviousEnabledChanged;
        public bool PreviousEnabled
        {
            get { return previousEnabled; }
            protected set
            {
                if (previousEnabled != value)
                {
                    previousEnabled = value;
                    EventsHelper.Fire(PreviousEnabledChanged, this, EventArgs.Empty);
                }
            }
        }

        private void OnStudyLoaded(object sender, StudyLoadedEventArgs e)
        {
            Undoable("PreviousLayout", () =>
            {
                var hook = GetLayoutHook();
                if (null == hook || hook.UpdateLayouts(this.Context.Viewer))
                {
                    return false;
                }
                Context.Viewer.PhysicalWorkspace.Draw();
                Context.Viewer.PhysicalWorkspace.SelectDefaultImageBox();
                return true;
            });
            UpdateEnabled();
        }

        private void UpdateEnabled()
        {
            var hook = GetLayoutHook();
            if (null == hook)
            {
                NextEnabled = PreviousEnabled = false;
                return;
            }
            NextEnabled = hook.NextLayoutAvailable;
            PreviousEnabled = hook.PreviousLayoutAvailable;
        }

        public override void Initialize()
        {
            base.Initialize();
            UpdateEnabled();
            base.Context.Viewer.EventBroker.StudyLoaded += OnStudyLoaded;
        }

        protected override void Dispose(bool disposing)
        {
            base.Context.Viewer.EventBroker.StudyLoaded -= OnStudyLoaded;
            base.Dispose(disposing);
        }

        LayoutHook GetLayoutHook()
        {
            var layoutManager = this.Context.Viewer.LayoutManager as ClearCanvas.ImageViewer.Layout.Basic.LayoutManager;
            if (null == layoutManager) { return null; }
            return layoutManager.LayoutHook as LayoutHook;
        }

        public void NextDisplaySets()
        {
            Undoable("NextLayout", () =>
            {
                var hook = GetLayoutHook();
                if (null == hook) { return false; }
                hook.ApplyNextLayout(this.Context.Viewer);
                Context.Viewer.PhysicalWorkspace.Draw();
                Context.Viewer.PhysicalWorkspace.SelectDefaultImageBox();
                return true;
            });
            UpdateEnabled();
        }

        public void PreviousDisplaySets()
        {
            Undoable("PreviousLayout", () =>
            {
                var hook = GetLayoutHook();
                if (null == hook) { return false; }
                hook.ApplyPreviousLayout(this.Context.Viewer);
                Context.Viewer.PhysicalWorkspace.Draw();
                Context.Viewer.PhysicalWorkspace.SelectDefaultImageBox();
                return true;
            });
            UpdateEnabled();
        }

        public void Undoable(string name, Func<bool> action)
        {
            IPhysicalWorkspace workspace = Context.Viewer.PhysicalWorkspace;
            MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(workspace);
            memorableCommand.BeginState = workspace.CreateMemento();
            if (!action()) { return; }
            memorableCommand.EndState = workspace.CreateMemento();
            DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(workspace);
            historyCommand.Name = name;
            historyCommand.Enqueue(memorableCommand);
            Context.Viewer.CommandHistory.AddCommand(historyCommand);
        }
    }
}
