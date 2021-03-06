﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Econmed.ImageViewer.Layout.HangingProtocols {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class LayoutSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static LayoutSettings defaultInstance = ((LayoutSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new LayoutSettings())));
        
        public static LayoutSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<hanging-protocols>\r\n  <hanging-protocol" +
            ">\r\n    <study-filter>\r\n      <condition>\r\n        <contains test=\"$ModalitiesInS" +
            "tudy\" pattern=\"MG\" />\r\n      </condition>\r\n    </study-filter>\r\n    <image-filte" +
            "r name=\"MG-Left-CC\">\r\n      <condition>\r\n        <and>\r\n          <equal test=\"$" +
            "PresentationIntentType\" refValue=\"FOR PRESENTATION\" />\r\n          <equal test=\"$" +
            "Modality\" refValue=\"MG\" />\r\n          <equal test=\"$ImageLaterality\" refValue=\"L" +
            "\" />\r\n          <or>\r\n            <equal test=\"$ViewPosition\" refValue=\"CC\" />\r\n" +
            "            <equal test=\"$PatientOrientation\" refValue=\"P\\L\" />\r\n            <eq" +
            "ual test=\"$PatientOrientation\" refValue=\"A\\R\" />\r\n          </or>\r\n        </and" +
            ">\r\n      </condition>\r\n    </image-filter>\r\n    <image-filter name=\"MG-Right-CC\"" +
            ">\r\n      <condition>\r\n        <and>\r\n          <equal test=\"$PresentationIntentT" +
            "ype\" refValue=\"FOR PRESENTATION\" />\r\n          <equal test=\"$Modality\" refValue=" +
            "\"MG\" />\r\n          <equal test=\"$ImageLaterality\" refValue=\"R\" />\r\n          <or" +
            ">\r\n            <equal test=\"$ViewPosition\" refValue=\"CC\" />\r\n            <equal " +
            "test=\"$PatientOrientation\" refValue=\"P\\L\" />\r\n            <equal test=\"$PatientO" +
            "rientation\" refValue=\"A\\R\" />\r\n          </or>\r\n        </and>\r\n      </conditio" +
            "n>\r\n    </image-filter>\r\n    <image-filter name=\"MG-Left-MLO\">\r\n      <condition" +
            ">\r\n        <and>\r\n          <equal test=\"$PresentationIntentType\" refValue=\"FOR " +
            "PRESENTATION\" />\r\n          <equal test=\"$Modality\" refValue=\"MG\" />\r\n          " +
            "<equal test=\"$ImageLaterality\" refValue=\"L\" />\r\n          <or>\r\n            <equ" +
            "al test=\"$ViewPosition\" refValue=\"MLO\" />\r\n            <not>\r\n              <or>" +
            "\r\n                <equal test=\"$PatientOrientation\" refValue=\"P\\L\" />\r\n         " +
            "       <equal test=\"$PatientOrientation\" refValue=\"A\\R\" />\r\n              </or>\r" +
            "\n            </not>\r\n          </or>\r\n        </and>\r\n      </condition>\r\n    </" +
            "image-filter>\r\n    <image-filter name=\"MG-Right-MLO\">\r\n      <condition>\r\n      " +
            "  <and>\r\n          <equal test=\"$PresentationIntentType\" refValue=\"FOR PRESENTAT" +
            "ION\" />\r\n          <equal test=\"$Modality\" refValue=\"MG\" />\r\n          <equal te" +
            "st=\"$ImageLaterality\" refValue=\"R\" />\r\n          <or>\r\n            <equal test=\"" +
            "$ViewPosition\" refValue=\"MLO\" />\r\n            <not>\r\n              <or>\r\n       " +
            "         <equal test=\"$PatientOrientation\" refValue=\"P\\L\" />\r\n                <e" +
            "qual test=\"$PatientOrientation\" refValue=\"A\\R\" />\r\n              </or>\r\n        " +
            "    </not>\r\n          </or>\r\n        </and>\r\n      </condition>\r\n    </image-fil" +
            "ter>\r\n    <workspace rows=\"2\" columns=\"2\">\r\n      <image-box filter=\"MG-Right-CC" +
            "\" />\r\n      <image-box filter=\"MG-Left-CC\" />\r\n      <image-box filter=\"MG-Right" +
            "-MLO\" />\r\n      <image-box filter=\"MG-Left-MLO\" />\r\n    </workspace>\r\n    <works" +
            "pace rows=\"1\" columns=\"2\">\r\n      <image-box filter=\"MG-Right-CC\" />\r\n      <ima" +
            "ge-box filter=\"MG-Left-CC\" />\r\n    </workspace>\r\n    <workspace rows=\"1\" columns" +
            "=\"2\">\r\n      <image-box filter=\"MG-Right-MLO\" />\r\n      <image-box filter=\"MG-Le" +
            "ft-MLO\" />\r\n    </workspace>\r\n    <workspace rows=\"2\" columns=\"2\">\r\n      <image" +
            "-box filter=\"MG-Right-CC\" prior-study=\"true\" />\r\n      <image-box filter=\"MG-Lef" +
            "t-CC\" prior-study=\"true\" />\r\n      <image-box filter=\"MG-Right-CC\" />\r\n      <im" +
            "age-box filter=\"MG-Left-CC\" />\r\n    </workspace>\r\n    <workspace rows=\"2\" column" +
            "s=\"2\">\r\n      <image-box filter=\"MG-Right-MLO\" prior-study=\"true\" />\r\n      <ima" +
            "ge-box filter=\"MG-Left-MLO\" prior-study=\"true\" />\r\n      <image-box filter=\"MG-R" +
            "ight-MLO\" />\r\n      <image-box filter=\"MG-Left-MLO\" />\r\n    </workspace>\r\n  </ha" +
            "nging-protocol>\r\n</hanging-protocols>")]
        public global::System.Xml.XmlDocument LayoutSettingsXml {
            get {
                return ((global::System.Xml.XmlDocument)(this["LayoutSettingsXml"]));
            }
            set {
                this["LayoutSettingsXml"] = value;
            }
        }
    }
}
