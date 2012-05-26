using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;

namespace _3DDesktop
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double _angle = 0;        

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                _angle -= 2;
                //camera.Position = new Point3D(camera.Position.X - 10, camera.Position.Y, camera.Position.Z);
            }
            if (e.Key == Key.Right)
            {
                _angle += 2;
                //camera.Position = new Point3D(camera.Position.X + 10, camera.Position.Y, camera.Position.Z);
            }
            Transform3DGroup transform3DGroup = new Transform3DGroup();
            RotateTransform3D rotateTransform3D_1 = new RotateTransform3D();
            AxisAngleRotation3D axisAngleRotation3D_1 = new AxisAngleRotation3D(new Vector3D(0, 1, 0), _angle );
            rotateTransform3D_1.Rotation = axisAngleRotation3D_1;          
            transform3DGroup.Children.Add(rotateTransform3D_1);             
            camera.Transform = transform3DGroup;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string wallpaper = DesktopHelper.GetCurrentWallpaper();

            BitmapImage bitmapImage = new BitmapImage(new Uri(wallpaper, UriKind.Absolute));

            int width = (int)(bitmapImage.Width / 2);
            int height = (int)(bitmapImage.Height / 2);
            CreateBackFace(width, height, bitmapImage);
            CreateBottomFace(width, height);
            CreateTopFace(width, height);

            IconManagement iconManagement = new IconManagement();
            Canvas canvas = new Canvas();
            iconManagement.DrawIconOnBackground(canvas);
        }

        private void CreateBackFace(int width, int height, BitmapImage bitmapImage)
        {;

            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions.Add(new Point3D(-width, -height, -10));
            mesh.Positions.Add(new Point3D(width, -height, -10));
            mesh.Positions.Add(new Point3D(width, height, -10));
            mesh.Positions.Add(new Point3D(-width, height, -10));

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            // These are the lines you need to allow an image to be painted onto the 3d model
            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(1, 0));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(0, 1));

            ImageBrush imageBrush = new ImageBrush(bitmapImage);

            GeometryModel3D geometry = new GeometryModel3D(mesh, new DiffuseMaterial(imageBrush));
            //GeometryModel3D geometry = new GeometryModel3D(mesh, new DiffuseMaterial(new SolidColorBrush(Colors.Red)));

            group.Children.Add(geometry);
        }

        private void CreateBottomFace(int width, int height)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions.Add(new Point3D(-width, height, -10));
            mesh.Positions.Add(new Point3D(width, height, -10));
            mesh.Positions.Add(new Point3D(width, height, +1000));
            mesh.Positions.Add(new Point3D(-width, height, +1000));

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            // These are the lines you need to allow an image to be painted onto the 3d model
            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(1, 0));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(0, 1));

            GeometryModel3D geometry = new GeometryModel3D(mesh, new DiffuseMaterial(new SolidColorBrush(Colors.Red)));

            group.Children.Add(geometry);
        }

        private void CreateTopFace(int width, int height)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions.Add(new Point3D(-width, -height, -10));
            mesh.Positions.Add(new Point3D(width, -height, -10));
            mesh.Positions.Add(new Point3D(width, -height, +1000));
            mesh.Positions.Add(new Point3D(-width, -height, +1000));

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(2);

            // These are the lines you need to allow an image to be painted onto the 3d model
            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(1, 0));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(0, 1));

            GeometryModel3D geometry = new GeometryModel3D(mesh, new DiffuseMaterial(new SolidColorBrush(Colors.Red)));

            group.Children.Add(geometry);
        }
    }
}
