using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;

namespace Econmed.ImageViewer.Layout.HangingProtocols
{
    [Cloneable]
    internal sealed class MessagePresentationImage : BasicPresentationImage
    {
        public static IDisplaySet CreateDisplaySet(string text)
        {
            var displaySet = new DisplaySet();
            displaySet.PresentationImages.Add(new MessagePresentationImage(text));
            return displaySet;
        }

        private readonly string text;

        public MessagePresentationImage(string text)
            : base(new GrayscaleImageGraphic(1, 1))
        {
            this.text = text;
            CompositeImageGraphic.Graphics.Add(new MessageGraphic { Text = text, Color = Color.WhiteSmoke });
        }

        protected override IAnnotationLayout CreateAnnotationLayout()
        {
            return new AnnotationLayout();
        }

        public override IPresentationImage CreateFreshCopy()
        {
            return new MessagePresentationImage(text);
        }

        public override Size SceneSize
        {
            get { return new Size(100, 100); }
        }

        [Cloneable(true)]
        private class MessageGraphic : InvariantTextPrimitive
        {
            protected override SpatialTransform CreateSpatialTransform()
            {
                return new InvariantSpatialTransform(this);
            }

            public override void OnDrawing()
            {
                if (base.ParentPresentationImage != null)
                {
                    CoordinateSystem = CoordinateSystem.Destination;
                    try
                    {
                        var clientRectangle = ParentPresentationImage.ClientRectangle;
                        Location = new PointF(clientRectangle.Width / 2f, clientRectangle.Height / 2f);
                    }
                    finally
                    {
                        ResetCoordinateSystem();
                    }
                }
                base.OnDrawing();
            }
        }
    }
}

