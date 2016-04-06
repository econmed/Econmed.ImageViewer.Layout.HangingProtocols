using System.Configuration;

namespace Econmed.ImageViewer.Layout.HangingProtocols
{
    [SettingsGroupDescription("Stores the user's hanging protocols.")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class LayoutSettings
    {
    }
}
