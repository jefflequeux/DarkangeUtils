using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Image=System.Windows.Controls.Image;
using Label=System.Windows.Controls.Label;
using System.Windows.Controls;
using System.Windows.Documents;

namespace _3DDesktop
{
    public class DesktopIcon
    {
        public enum StateEnum
        {
            None,
            Pile,
            Exploded,
            CoverFlow
        } ;

        
        public string Name{ get; set; }
        public FileInfo FileName{ get; set; }
        public System.Windows.Controls.Image ImageIcon{ get; set; }
        public Label Label { get; set; }
        public Point Location { get; set; }
        public Point LocationSave { get; set; }
        public bool IsSelected{ get; set; }
        public StateEnum State { get; set; }
        public int PositionOnPile { get; set; }
        public System.Drawing.Image Image { get; set; }
        public int PileId { get; set; }

        public DiffuseMaterial Dmaterial
        {
            get { return dmaterial; }
        }

        public DiffuseMaterial DmaterialSelect
        {
            get { return dmaterialSelect; }
        }

        public GeometryModel3D DesktopGroundIcon
        {
            get { return _desktopGroundIcon; }
        }

        private GeometryModel3D _desktopGroundIcon;
        private DiffuseMaterial dmaterial;
        private DiffuseMaterial dmaterialSelect;

        private Point3D location3dSave;
        private Point3D location3d;
        private Point3D newPosition;

        /// <summary>
        /// Select Icon, change is color
        /// </summary>
        public bool Select(Point? newPt)
        {

            if (!IsSelected)
            {
                IsSelected = true;
                Label.Foreground =
                    new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                _desktopGroundIcon.Material = dmaterialSelect;
            }
            if (State == StateEnum.Pile)
            {
                foreach (var desktopIcon in IconManagementFty.Instance.ListOfDesktopIcon)
                {
                    if (desktopIcon != this && desktopIcon.State == StateEnum.Pile && desktopIcon.PileId == PileId && desktopIcon.PileId != -1)
                    {
                        desktopIcon.SelectSingle();
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Select Icon, change is color
        /// </summary>
        public void SelectSingle()
        {
            if (!IsSelected)
            {
                IsSelected = true;
                Label.Foreground =
                    new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                _desktopGroundIcon.Material = dmaterialSelect;
            }
        }

        /// <summary>
        /// Unselect Icon, change is color
        /// </summary>
        public void Unselect()
        {
            if (IsSelected)
            {
                IsSelected = false;
                Label.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                _desktopGroundIcon.Material = dmaterial;
            }
        }

        /// <summary>
        /// Move icon to position
        /// </summary>
        /// <param name="point"></param>
        public void MoveTo(Point point, int _positionOnPile)
        {
            PositionOnPile = _positionOnPile;
            Location = new Point(point.X - ImageIcon.Width + (PositionOnPile * 3), point.Y - ImageIcon.Height - (PositionOnPile * 3));
            ImageIcon.Margin = new Thickness(Location.X, Location.Y, 0, 0);
            Label.Visibility = Visibility.Hidden;
            Label.Margin = new Thickness(Location.X - 20, Location.Y + ImageIcon.Height, 0, 0);
            State = StateEnum.Pile;
        }

        /// <summary>
        /// Move icon to position
        /// </summary>
        /// <param name="point"></param>
        public void MoveTo3D(Point point3d, Point point, int _positionOnPile)
        {
            if (State != StateEnum.Pile)
            {
                int speed = 250;
                PositionOnPile = _positionOnPile;
                LocationSave = Location;

                Location = new Point(point.X - ImageIcon.Width + (PositionOnPile*3),
                                     point.Y - ImageIcon.Height - (PositionOnPile*3));
                ImageIcon.Margin = new Thickness(Location.X, Location.Y, 0, 0);
                Label.Visibility = Visibility.Hidden;
                Label.Margin = new Thickness(Location.X - 20, Location.Y + ImageIcon.Height, 0, 0);
                State = StateEnum.Pile;

                newPosition = new Point3D(point3d.X, 0.5 + (5*_positionOnPile), point3d.Y);
                Vector3D computedPos = newPosition - location3d;



                //TranslateTransform3D trans = new TranslateTransform3D(computedPos);
                //_desktopGroundIcon.Transform = trans;

                Transform3DGroup transGroup = new Transform3DGroup();
                Transform3D transform = new TranslateTransform3D(computedPos);
                transGroup.Children.Add(transform);
                _desktopGroundIcon.Transform = transGroup;

                DoubleAnimation AnimationX = new DoubleAnimation();
                AnimationX.From = 0;
                AnimationX.To = computedPos.X;
                AnimationX.Duration = new Duration(TimeSpan.FromMilliseconds(speed));
                transform.BeginAnimation(TranslateTransform3D.OffsetXProperty, AnimationX);

                DoubleAnimation AnimationY = new DoubleAnimation();
                AnimationY.From = 0;
                AnimationY.To = computedPos.Y;
                AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(speed));
                transform.BeginAnimation(TranslateTransform3D.OffsetYProperty, AnimationY);

                DoubleAnimation AnimationZ = new DoubleAnimation();
                AnimationZ.From = 0;
                AnimationZ.To = computedPos.Z;
                AnimationZ.Duration = new Duration(TimeSpan.FromMilliseconds(speed));
                transform.BeginAnimation(TranslateTransform3D.OffsetZProperty, AnimationZ);

                Timer timer = new Timer();
                timer.Interval = speed;
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            location3dSave = location3d;
            location3d = newPosition;
            ((Timer)sender).Stop();
        }

        

        public void DrawIcon3D(Model3DGroup group, double x, double y, double z, double size, ImageSource source)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            location3d = new Point3D(x, y, z);
            mesh.Positions.Add(new Point3D(x - (size / 2), y, z - (size / 2)));
            mesh.Positions.Add(new Point3D(x + (size / 2), y, z - (size / 2)));
            mesh.Positions.Add(new Point3D(x + (size / 2), y, z + (size / 2)));
            mesh.Positions.Add(new Point3D(x + -(size / 2), y, z + (size / 2)));

            mesh.TextureCoordinates.Add(new Point(0, 0)); //4
            mesh.TextureCoordinates.Add(new Point(1, 0)); //3
            mesh.TextureCoordinates.Add(new Point(1, 1)); //2
            mesh.TextureCoordinates.Add(new Point(0, 1)); //1

            // Bottom face
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(2);

            // Geometry creation
            dmaterial = new DiffuseMaterial();
            dmaterial.Brush = new ImageBrush(source);

            dmaterialSelect = new DiffuseMaterial();
            dmaterialSelect.Brush = new ImageBrush(Imaging.SelectImage(Image));

            _desktopGroundIcon = new GeometryModel3D(mesh, dmaterial);
            _desktopGroundIcon.Transform = new Transform3DGroup();

            //Point3D location3dText = new Point3D(x, y, z + size);
            //var _text = CreateTextLabel3D(this.Name, new SolidColorBrush(Colors.White), true, 6, location3dText, new Vector3D(-1, 0, 0), new Vector3D(0, 0, -1));
            //group.Children.Add(_text);

            group.Children.Add(_desktopGroundIcon);
        }

        /// <summary>
        /// Creates a ModelVisual3D containing a text label.
        /// </summary>
        /// <param name="text">The string</param>
        /// <param name="textColor">The color of the text.</param>
        /// <param name="bDoubleSided">Visible from both sides?</param>
        /// <param name="height">Height of the characters</param>
        /// <param name="center">The center of the label</param>
        /// <param name="over">Horizontal direction of the label</param>
        /// <param name="up">Vertical direction of the label</param>
        /// <returns>Suitable for adding to your Viewport3D</returns>
        public static GeometryModel3D CreateTextLabel3D(string text,
                                                    Brush textColor,
                                                    bool bDoubleSided,
                                                    double height,
                                                    Point3D center,
                                                    Vector3D over,
                                                    Vector3D up)
        {
            // First we need a textblock containing the text of our label
            TextBlock tb = new TextBlock(new Run(text));
            tb.Foreground = textColor;
            tb.FontFamily = new FontFamily("Arial");

            // Now use that TextBlock as the brush for a material
            DiffuseMaterial mat = new DiffuseMaterial();
            mat.Brush = new VisualBrush(tb);

            // We just assume the characters are square
            double width = text.Length * height;

            // Since the parameter coming in was the center of the label,
            // we need to find the four corners
            // p0 is the lower left corner
            // p1 is the upper left
            // p2 is the lower right
            // p3 is the upper right
            Point3D p0 = center - width / 2 * over - height / 2 * up;
            Point3D p1 = p0 + up * 1 * height;
            Point3D p2 = p0 + over * width;
            Point3D p3 = p0 + up * 1 * height + over * width;

            // Now build the geometry for the sign.  It's just a
            // rectangle made of two triangles, on each side.

            MeshGeometry3D mg = new MeshGeometry3D();
            mg.Positions = new Point3DCollection();
            mg.Positions.Add(p0);    // 0
            mg.Positions.Add(p1);    // 1
            mg.Positions.Add(p2);    // 2
            mg.Positions.Add(p3);    // 3

            if (bDoubleSided)
            {
                mg.Positions.Add(p0);    // 4
                mg.Positions.Add(p1);    // 5
                mg.Positions.Add(p2);    // 6
                mg.Positions.Add(p3);    // 7
            }

            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(3);
            mg.TriangleIndices.Add(1);
            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(2);
            mg.TriangleIndices.Add(3);

            if (bDoubleSided)
            {
                mg.TriangleIndices.Add(4);
                mg.TriangleIndices.Add(5);
                mg.TriangleIndices.Add(7);
                mg.TriangleIndices.Add(4);
                mg.TriangleIndices.Add(7);
                mg.TriangleIndices.Add(6);
            }

            // These texture coordinates basically stretch the
            // TextBox brush to cover the full side of the label.

            mg.TextureCoordinates.Add(new Point(0, 1));
            mg.TextureCoordinates.Add(new Point(0, 0));
            mg.TextureCoordinates.Add(new Point(1, 1));
            mg.TextureCoordinates.Add(new Point(1, 0));

            if (bDoubleSided)
            {
                mg.TextureCoordinates.Add(new Point(1, 1));
                mg.TextureCoordinates.Add(new Point(1, 0));
                mg.TextureCoordinates.Add(new Point(0, 1));
                mg.TextureCoordinates.Add(new Point(0, 0));
            }

            // And that's all.  Return the result.

            return  new GeometryModel3D(mg, mat); ;
        }
        public void ExplodeMoveTo3D()
        {
            int speed = 250;
            Vector3D computedPos = location3d - location3dSave;
            Location = LocationSave;

            Transform3DGroup transGroup = new Transform3DGroup();
            Transform3D transform = new TranslateTransform3D(computedPos);
            transGroup.Children.Add(transform);
            _desktopGroundIcon.Transform = transGroup;

            DoubleAnimation AnimationX = new DoubleAnimation();
            AnimationX.From = computedPos.X;
            AnimationX.To = 0;
            AnimationX.Duration = new Duration(TimeSpan.FromMilliseconds(speed));
            transform.BeginAnimation(TranslateTransform3D.OffsetXProperty, AnimationX);

            DoubleAnimation AnimationY = new DoubleAnimation();
            AnimationY.From = computedPos.Y;
            AnimationY.To = 0;
            AnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(speed));
            transform.BeginAnimation(TranslateTransform3D.OffsetYProperty, AnimationY);

            DoubleAnimation AnimationZ = new DoubleAnimation();
            AnimationZ.From = computedPos.Z;
            AnimationZ.To = 0;
            AnimationZ.Duration = new Duration(TimeSpan.FromMilliseconds(speed));
            transform.BeginAnimation(TranslateTransform3D.OffsetZProperty, AnimationZ);

            Timer timer = new Timer();
            timer.Interval = speed;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            State = StateEnum.None;
            PileId = -1;
        }
    }
}
