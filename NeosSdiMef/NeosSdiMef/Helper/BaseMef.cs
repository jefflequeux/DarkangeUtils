using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Shapes;
using Roslyn.Compilers.CSharp;
using System.Collections.Generic;
using Roslyn.Compilers;
using System.Linq;
using System.Text;
using EnvDTE80;
using EnvDTE;
using NeosSdiMef.Extension;
using Microsoft.VisualStudio.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Reflection;
using System.Resources;
using System.Globalization;
using System.Windows.Input;

namespace NeosSdiMef
{
    public class BaseMef : Canvas
    {
        public IWpfTextView _textView;
        protected const int width = 5;
        protected const int height = 10;
        public SyntaxTree tree;
        public IAdornmentLayer _layer;

        protected Brush _brush;
        protected Pen _pen;
        protected List<Rect> rects = new List<Rect>();

        public delegate void FixErrorCallback();

        public void AddDecorationError(BasePropertyDeclarationSyntax _property, string textFull, string toolTipText, FixErrorCallback errorCallback)
        {
            var lineSpan = tree.GetLineSpan(_property.Span, usePreprocessorDirectives: false);
            int lineNumber = lineSpan.StartLinePosition.Line;
            var line = _textView.TextSnapshot.GetLineFromLineNumber(lineNumber);
            var textViewLine = _textView.GetTextViewLineContainingBufferPosition(line.Start);
            int startSpace = textFull.Length - textFull.TrimStart().Length;
            int endSpace = textFull.Length - textFull.TrimEnd().Length;



            SnapshotSpan span = new SnapshotSpan(_textView.TextSnapshot, Span.FromBounds(line.Start.Position + startSpace, line.End.Position - endSpace));
            Geometry g = _textView.TextViewLines.GetMarkerGeometry(span);
            if (g != null)
            {
                rects.Add(g.Bounds);

                GeometryDrawing drawing = new GeometryDrawing(_brush, _pen, g);
                drawing.Freeze();

                DrawingImage drawingImage = new DrawingImage(drawing);
                drawingImage.Freeze();

                Image image = new Image();
                image.Source = drawingImage;
                //image.Visibility = Visibility.Hidden;

                Canvas.SetLeft(image, g.Bounds.Left);
                Canvas.SetTop(image, g.Bounds.Top);
                _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, (t, ui) =>
                {
                    rects.Remove(g.Bounds);
                });

                DrawIcon(span, g.Bounds.Left - 30, g.Bounds.Top, toolTipText, errorCallback);
            }
        }

        private void DrawIcon(SnapshotSpan span, double x, double y, string toolTipText, FixErrorCallback errorCallback)
        {
            //draw a square with the created brush and pen
            System.Windows.Rect r = new System.Windows.Rect(0, 0, 16, 16);
            Geometry g = new System.Windows.Media.RectangleGeometry(r);
            rects.Add(g.Bounds);
            GeometryDrawing drawing = new GeometryDrawing(_brush, _pen, g);
            drawing.Freeze();

            DrawingImage drawingImage = new DrawingImage(drawing);
            drawingImage.Freeze();

            Image _image = new Image();
            _image.ToolTip = toolTipText;
            _image.Cursor = Cursors.Arrow;
            Helper.Imaging.GetImage(this, _image, "NeosSdiMef.Images.repair.png");
            _image.Stretch = Stretch.Fill;

            _image.MouseLeftButtonDown += new MouseButtonEventHandler(Error_MouseLeftButtonUp);
            _image.Tag = errorCallback;
            Canvas.SetLeft(_image, x);
            Canvas.SetTop(_image, y);

            _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, _image, (t, ui) =>
            {
                rects.Remove(g.Bounds);
            });

        }

        private void Error_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            FixErrorCallback errorCallback = image.Tag as FixErrorCallback;
            errorCallback();
        }

    }
}
