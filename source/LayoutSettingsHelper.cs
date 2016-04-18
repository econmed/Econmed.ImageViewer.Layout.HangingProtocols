using ClearCanvas.ImageViewer.Layout.Basic;
using ClearCanvas.ImageViewer.StudyManagement;
using System;
using System.Reflection;

namespace Econmed.ImageViewer.Layout.HangingProtocols
{
    public static class LayoutSettingsHelper
    {
        static Type layoutSettingsType = Assembly.GetAssembly(typeof(StoredLayout)).GetType("ClearCanvas.ImageViewer.Layout.Basic.LayoutSettings");
        static MethodInfo getMinimumLayoutMethod = layoutSettingsType.GetMethod("GetMinimumLayout", BindingFlags.Static | BindingFlags.Public);
        static PropertyInfo defaultInstanceProperty = layoutSettingsType.GetProperty("DefaultInstance", BindingFlags.Static | BindingFlags.Public);
        static object defaultInstance = defaultInstanceProperty.GetValue(null, new object[] { });
        static MethodInfo getLayoutMethod = layoutSettingsType.GetMethod("GetLayout", new Type[] { typeof(IImageSopProvider) });

        public static StoredLayout MinimumLayout { get { return (StoredLayout)getMinimumLayoutMethod.Invoke(null, new object[] { }); } }

        public static StoredLayout GetLayout(IImageSopProvider imageSopProvider)
        {
            return (StoredLayout)getLayoutMethod.Invoke(defaultInstance, new object[] { imageSopProvider });
        }
    }
}
