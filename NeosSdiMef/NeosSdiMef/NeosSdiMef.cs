using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;
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
using NeosSdiMef.CodingRules;

namespace NeosSdiMef
{
    /// <summary>
    /// A class detailing the margin's visual definition including both size and content.
    /// </summary>
    /// 
    public class NeosSdiMef : BaseMef, IWpfTextViewMargin
    {
        public const string MarginName = "NeosSdiMef";
        private bool _isDisposed = false;

        /// <summary>
        /// Creates a <see cref="NeosSdiMef"/> for a given <see cref="IWpfTextView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        public NeosSdiMef(IWpfTextView textView)
        {
            _textView = textView;

            this.Width = width;
            this.ClipToBounds = true;
            this.Background = new SolidColorBrush(Colors.White);

            _layer = _textView.GetAdornmentLayer("TextAdornmentNeos");

            textView.ViewportHeightChanged += new EventHandler(textView_ViewportHeightChanged);
            textView.LayoutChanged += new EventHandler<TextViewLayoutChangedEventArgs>(textView_LayoutChanged);

            _brush = new SolidColorBrush(Color.FromRgb(255, 155, 155));
            _pen = new Pen(new SolidColorBrush(Color.FromRgb(255, 100, 100)), 0.5);

            ParseFile();

        }

        /// <summary>
        /// Layout Update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textView_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            this.ParseFile();
        }

        /// <summary>
        /// Viewport size change Update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textView_ViewportHeightChanged(object sender, EventArgs e)
        {
            this.ParseFile();
        }

        /// <summary>
        /// Parse current file
        /// </summary>
        private void ParseFile()
        {
            string text = _textView.TextSnapshot.GetText();
            tree = SyntaxTree.ParseCompilationUnit(text);
            //this.RightMargin();
            //this.CheckRules();
            if (tree.Root.DescendentNodes().ToList().Count > 40)
            {
                Roslyn.Compilers.CSharp.ExpressionStatementSyntax exp = (tree.Root.DescendentNodes().ToList()[40] as Roslyn.Compilers.CSharp.ExpressionStatementSyntax);

                var lineSpan = tree.GetLineSpan(exp.FullSpan, usePreprocessorDirectives: false);
                int lineNumber = lineSpan.StartLinePosition.Line;
                var line = _textView.TextSnapshot.GetLineFromLineNumber(lineNumber);
                var textViewLine = _textView.GetTextViewLineContainingBufferPosition(line.Start);



                SnapshotSpan span = new SnapshotSpan(_textView.TextSnapshot, Span.FromBounds(line.Start.Position, line.End.Position));

                var tt = textViewLine.TextLines[0];
            }
        }

        /// <summary>
        /// Check for any coding rules
        /// </summary>
        private void CheckRules()
        {
            this.ParseProperty();
        }

        /// <summary>
        /// Create a margin on the right of the editor
        /// The margin will contain:
        ///     - Methods Accessors
        ///     - Error list
        ///     - Warning list
        /// </summary>
        private void RightMargin()
        {

            var root = (CompilationUnitSyntax)tree.Root;

            var _methods = root.DescendentNodes()
                        .OfType<MethodDeclarationSyntax>()
                        .ToList();

            int totalLines = _textView.TextSnapshot.LineCount;
            this.Children.Clear();

            foreach (var _method in _methods)
            {
                var lineSpan = tree.GetLineSpan(_method.Span, usePreprocessorDirectives: false);
                int lineNumber = lineSpan.StartLinePosition.Line;
                CreateMethodTick(_textView, totalLines, lineNumber, _method);
            }
        }

        /// <summary>
        /// Create a single ui element pointing to a method
        /// </summary>
        /// <param name="textView">Editor</param>
        /// <param name="totalLines">Number of lines in the file</param>
        /// <param name="lineNumber">Line number where the method start</param>
        /// <param name="method">current method</param>
        private void CreateMethodTick(IWpfTextView textView, int totalLines, int lineNumber, MethodDeclarationSyntax method)
        {
            if (textView.ViewportHeight == 0)
                return;

            double ratio = (double)lineNumber / (double)totalLines;
            double yPos = ratio * textView.ViewportHeight;
            double newYPos = Math.Ceiling(yPos);


            Grid rect = new Grid();
            rect.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(rect_MouseLeftButtonDown);
            string tooltipMessage = method.ToString();
            if(tooltipMessage.Replace(Environment.NewLine, "\r").Count(f => f == '\r') > 5)
            {
                tooltipMessage = ConvertStringArrayToString(tooltipMessage.Replace(Environment.NewLine, "\r").Split('\r'), 5);
                tooltipMessage += Environment.NewLine + "...";
            }
            rect.ToolTip = tooltipMessage;
            rect.Tag = method;
            rect.Background = new SolidColorBrush(Colors.LightGreen);
            rect.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            rect.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            rect.Margin = new System.Windows.Thickness(0, newYPos, 0, 0);
            rect.Height = textView.ViewportHeight / (double)totalLines; 
            if (rect.Height < height)
                rect.Height = height;
            rect.Width = width;
            this.Children.Add(rect);
        }

        /// <summary>
        /// Extract a part of a string array and convert it to a string
        /// </summary>
        /// <param name="array"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        static string ConvertStringArrayToString(string[] array, int limit)
        {
            StringBuilder builder = new StringBuilder();
            int _count = 0;
            foreach (string value in array)
            {
                if (_count == limit)
                    break;
                builder.AppendLine(value);
                _count++;
            }
            return builder.ToString();
        }

        /// <summary>
        /// When user click to a method tick, center screen to the clicked method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void rect_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MethodDeclarationSyntax method = ((Grid)sender).Tag as MethodDeclarationSyntax;
            
            // Retrieve method start line number
            int lineNumber = tree.GetLineSpan(method.Span, usePreprocessorDirectives: false).StartLinePosition.Line;

            var line = _textView.TextSnapshot.GetLineFromLineNumber(lineNumber);
            var textViewLine = _textView.GetTextViewLineContainingBufferPosition(line.Start);

            // Position caret to the given method
            _textView.Caret.MoveTo(textViewLine);
            
            // Scroll to display current caret
            _textView.Caret.EnsureVisible();

            //var span = textViewLine.GetTextElementSpan(line.Start);
            //_textView.ViewScroller.EnsureSpanVisible(span);
        }

        /// <summary>
        /// Dispose element
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(MarginName);
        }

        #region IWpfTextViewMargin Members

        /// <summary>
        /// The <see cref="Sytem.Windows.FrameworkElement"/> that implements the visual representation
        /// of the margin.
        /// </summary>
        public System.Windows.FrameworkElement VisualElement
        {
            // Since this margin implements Canvas, this is the object which renders
            // the margin.
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        #endregion

        #region ITextViewMargin Members

        public double MarginSize
        {
            // Since this is a horizontal margin, its width will be bound to the width of the text view.
            // Therefore, its size is its height.
            get
            {
                ThrowIfDisposed();
                return this.ActualWidth;
            }
        }

        public bool Enabled
        {
            // The margin should always be enabled
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        /// <summary>
        /// Returns an instance of the margin if this is the margin that has been requested.
        /// </summary>
        /// <param name="marginName">The name of the margin requested</param>
        /// <returns>An instance of NeosSdiMef or null</returns>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return (marginName == NeosSdiMef.MarginName) ? (IWpfTextViewMargin)this : null;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }
        #endregion

    }

}
