Econmed.ImageViewer.Layout.HangingProtocols
===========================================

The [Econmed.ImageViewer.Layout.HangingProtocols plugin][plugin-page] allows one to create rules which control the way of how the [ClearCanvas DICOM Viewer](download-dicom-viewer) will lay out its display sets.

Installation
------------

1. [Download][download-plugin] the plugin
2. Extract it anywhere 
3. Run the install.bat to compile and install the plugin into your [ClearCanvas DICOM Viewer](download-dicom-viewer) installation.

Usage
-----

1. Open a study
2. Press `tab` or `shift` + `tab` to page up or down the available layouts.

Configuration
-------------

1. Open the `Settings Editor` at `Tools` -> `Utilities` -> `Settings Editor`
2. Search for `Econmed.ImageViewer.Layout.HangingProtocols`
3. Edit the `LayoutSettingsXml`
4. Save your changes

This is an example of a simple hanging protocol that will be applied to all studies containing any CR images and will put the first CR image of the primary and prior studies into a 1x1 grid and the remaining images of the primary study into 2x2 grids:   

    <hanging-protocols>
        <hanging-protocol residual-workspace-rows="2" residual-workspace-columns="2" >
            <study-filter>
                <condition>
                    <contains test="$ModalitiesInStudy" pattern="CR" />
                </condition>
            </study-filter>
            <image-filter name="CR">
                <condition>
                    <equal test="$Modality" refValue="CR" />
                </condition>
            </image-filter>
            <workspace rows="1" columns="1">            
                <image-box filter="CR" />
            </workspace>
            <workspace rows="1" columns="1">            
                <image-box filter="CR" prior-study="true" />
            </workspace>
        </hanging-protocol>
    </hanging-protocols>            
    
The `condition` elements work like descriped at `Rule Condition Operators` within the [ClearCanvas ImageServer User's Guide][rules-engine].

[plugin-page]: https://github.com/econmed/Econmed.ImageViewer.Layout.HangingProtocols
[download-plugin]: https://github.com/econmed/Econmed.ImageViewer.Layout.HangingProtocols/archive/master.zip
[download-dicom-viewer]: https://github.com/ClearCanvas/ClearCanvas/releases
[rules-engine]: http://www.clearcanvas.ca/Portals/0/ClearCanvasFiles/Documentation/UsersGuide/ImageServer/2_0/index.html?rules_engine.htm