using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WpfApp1.Model;

namespace PZ2.Common
{
    public class Objects3D
    {
        const double HEIGHT = 0.06;

        public static GeometryModel3D Create3Delement(PowerEntity entity, Model3DGroup model3DGroup, double size = HEIGHT)
        {
            double entityZ = 0.04;
            var mesh = new MeshGeometry3D();
            Point3DCollection PositionCollection = new Point3DCollection();
            PositionCollection.Add(new Point3D(entity.X, entity.Y, entityZ));//donji deo
            PositionCollection.Add(new Point3D(entity.X + size, entity.Y, entityZ));
            PositionCollection.Add(new Point3D(entity.X + size, entity.Y + size, entityZ));
            PositionCollection.Add(new Point3D(entity.X, entity.Y + size, entityZ));
            PositionCollection.Add(new Point3D(entity.X, entity.Y, entityZ + size));//gornji deo
            PositionCollection.Add(new Point3D(entity.X + size, entity.Y, entityZ + size));
            PositionCollection.Add(new Point3D(entity.X + size, entity.Y + size, entityZ + size));
            PositionCollection.Add(new Point3D(entity.X, entity.Y + size, entityZ + size));

            mesh.Positions = PositionCollection;
            foreach (var item in model3DGroup.Children)
            {
                while (mesh.Bounds.IntersectsWith(item.Bounds))
                {
                    for (var i = 0; i < mesh.Positions.Count; i++)
                    {
                        mesh.Positions[i] = new Point3D(mesh.Positions[i].X, mesh.Positions[i].Y, mesh.Positions[i].Z + HEIGHT);//slaganje jedan na drugi po Z osi
                    }
                }
            }
            Int32Collection TriangleCollection = new Int32Collection();

            TriangleCollection.Add(1);
            TriangleCollection.Add(0);
            TriangleCollection.Add(3);
            TriangleCollection.Add(1);
            TriangleCollection.Add(3);
            TriangleCollection.Add(2);

            TriangleCollection.Add(5);
            TriangleCollection.Add(6);
            TriangleCollection.Add(7);
            TriangleCollection.Add(5);
            TriangleCollection.Add(7);
            TriangleCollection.Add(4);

            TriangleCollection.Add(1);
            TriangleCollection.Add(2);
            TriangleCollection.Add(5);
            TriangleCollection.Add(5);
            TriangleCollection.Add(2);
            TriangleCollection.Add(6);

            TriangleCollection.Add(0);
            TriangleCollection.Add(1);
            TriangleCollection.Add(4);
            TriangleCollection.Add(1);
            TriangleCollection.Add(5);
            TriangleCollection.Add(4);

            TriangleCollection.Add(0);
            TriangleCollection.Add(4);
            TriangleCollection.Add(3);
            TriangleCollection.Add(4);
            TriangleCollection.Add(7);
            TriangleCollection.Add(3);

            TriangleCollection.Add(7);
            TriangleCollection.Add(6);
            TriangleCollection.Add(2);
            TriangleCollection.Add(7);
            TriangleCollection.Add(2);
            TriangleCollection.Add(3);

            mesh.TriangleIndices = TriangleCollection;

            var material = new DiffuseMaterial(entity.DefaultColor());

            var model = new GeometryModel3D(mesh, material);
            model.SetValue(FrameworkElement.TagProperty, entity);

            return model;
        }


        public static HashSet<GeometryModel3D> CreateLines(LineEntity entity)
        {
            HashSet<GeometryModel3D> Line = new HashSet<GeometryModel3D>();
            for (int i = 0; i < entity.Vertices.Count - 1; i++)
            {
                Line.Add(CreateLine(entity.Vertices[i], entity.Vertices[i + 1], CreateBrush(entity.ConductorMaterial), entity));
            }
            return Line;
        }
        private static Brush CreateBrush(string material)
        {

            switch (material)
            {
                case "Steel":
                    return Brushes.SpringGreen;
                case "Copper":
                    return Brushes.Goldenrod;
                case "Acsr":
                    return Brushes.LightSlateGray;
                default:
                    return Brushes.Black;

            }
        }
        private static GeometryModel3D CreateLine(Point3D start, Point3D end, Brush brush, LineEntity entity)
        {
            double size = HEIGHT / 2;
            double entityZ = 0.03;
            var mesh = new MeshGeometry3D();
            Point3DCollection PositionCollection = new Point3DCollection();
            PositionCollection.Add(new Point3D(start.X, start.Y, entityZ));//donji deo
            PositionCollection.Add(new Point3D(start.X + size, start.Y, entityZ));
            PositionCollection.Add(new Point3D(end.X, end.Y, entityZ));
            PositionCollection.Add(new Point3D(end.X + size, end.Y, entityZ));
            PositionCollection.Add(new Point3D(start.X + (size / 2), start.Y, entityZ + size));//gornji
            PositionCollection.Add(new Point3D(end.X + (size / 2), end.Y, entityZ + size));
            mesh.Positions = PositionCollection;

            Int32Collection TriangleCollection = new Int32Collection();
         
            TriangleCollection.Add(0);
            TriangleCollection.Add(2);
            TriangleCollection.Add(4);

            TriangleCollection.Add(4);
            TriangleCollection.Add(2);
            TriangleCollection.Add(5);

            TriangleCollection.Add(2);
            TriangleCollection.Add(3);
            TriangleCollection.Add(5);

            TriangleCollection.Add(1);
            TriangleCollection.Add(3);
            TriangleCollection.Add(5);

            TriangleCollection.Add(4);
            TriangleCollection.Add(5);
            TriangleCollection.Add(1);

            TriangleCollection.Add(1);
            TriangleCollection.Add(4);
            TriangleCollection.Add(0);

           
            TriangleCollection.Add(0);
            TriangleCollection.Add(4);
            TriangleCollection.Add(2);

            TriangleCollection.Add(4);
            TriangleCollection.Add(5);
            TriangleCollection.Add(2);

            TriangleCollection.Add(2);
            TriangleCollection.Add(5);
            TriangleCollection.Add(3);

            TriangleCollection.Add(1);
            TriangleCollection.Add(5);
            TriangleCollection.Add(3);

            TriangleCollection.Add(4);
            TriangleCollection.Add(1);
            TriangleCollection.Add(5);

            TriangleCollection.Add(1);
            TriangleCollection.Add(0);
            TriangleCollection.Add(4);

            mesh.TriangleIndices = TriangleCollection;

            var material = new DiffuseMaterial(brush);

            var model = new GeometryModel3D(mesh, material);
            model.SetValue(FrameworkElement.TagProperty, entity);
            return model;
        }
    }
}
