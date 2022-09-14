using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Xml;
using WpfApp1.Model;

namespace PZ2.Common
{
    public class Convertions
    {
        //Minimalne i maximalne latitude i longitude koje posmatram
        public const double min_lat = 45.2325;
        public const double min_lon = 19.793909;
        public const double max_lat = 45.277031;
        public const double max_lon = 19.894459;


        //Promenljive koje se korsiti da se decimalni brojevi ucitani iz fajla pretvore u odgovarajuce xY koordinate
        private static double ScaleX;
        private static double ScaleY;



        public static Dictionary<long, SubstationEntity> substationEntities = new Dictionary<long, SubstationEntity>();
        public static Dictionary<long, NodeEntity> nodeEntities = new Dictionary<long, NodeEntity>();
        public static Dictionary<long, SwitchEntity> switchEntities = new Dictionary<long, SwitchEntity>();
        //public static Dictionary<long, PowerEntity> allEntites = new Dictionary<long, PowerEntity>();
        private static HashSet<LineEntity> lineEntities = new HashSet<LineEntity>();
        private static HashSet<LineEntity> removedLineEntities = new HashSet<LineEntity>();
        public static Dictionary<string, string> AllConductorMaterial = new Dictionary<string, string>();

        public static HashSet<LineEntity> LineEntities { get => lineEntities; set => lineEntities = value; }

        public static void LoadAllModels()
        {
            ScaleX = (max_lon - min_lon) / 10;
            ScaleY = (max_lat - min_lat) / 10;

            var doc = new XmlDocument();
            doc.Load("Geographic.xml");
            AddEntities(substationEntities, doc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity"));
            AddEntities(nodeEntities, doc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity"));
            AddEntities(switchEntities, doc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity"));
            AddLineEntities(LineEntities, doc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity"));
        }


        public static void AddEntities<T>(Dictionary<long, T> entities, XmlNodeList nodeList) where T : PowerEntity, new()
        {
            foreach (XmlNode item in nodeList)
            {
                ToLatLon(double.Parse(item.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture), double.Parse(item.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture), 34, out var y, out var x);

               
                if (!(min_lat <= y && y <= max_lat) || !(min_lon <= x && x <= max_lon))
                {
                    continue;
                }

                var entity = new T()
                {
                    Id = long.Parse(item.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture),
                    Name = item.SelectSingleNode("Name").InnerText,
                    X = x,
                    Y = y
                };
                if (typeof(T) == typeof(SwitchEntity))
                {
                    (entity as SwitchEntity).Status = item.SelectSingleNode("Status").InnerText;
                }
                entities.Add(entity.Id, entity);
               // allEntites.Add(entity.Id, entity);
            }
        }
        public static void AddLineEntities(HashSet<LineEntity> entites, XmlNodeList nodeList)
        {
           
           
            foreach (XmlNode item in nodeList)
            {
                var line = new LineEntity()
                {
                    Id = long.Parse(item.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture),
                    Name = item.SelectSingleNode("Name").InnerText,
                    ConductorMaterial = item.SelectSingleNode("ConductorMaterial").InnerText,
                    LineType = item.SelectSingleNode("LineType").InnerText,
                    IsUnderground = bool.Parse(item.SelectSingleNode("IsUnderground").InnerText),
                    R = float.Parse(item.SelectSingleNode("R").InnerText, CultureInfo.InvariantCulture),
                    ThermalConstantHeat = long.Parse(item.SelectSingleNode("ThermalConstantHeat").InnerText, CultureInfo.InvariantCulture),
                    FirstEnd = long.Parse(item.SelectSingleNode("FirstEnd").InnerText, CultureInfo.InvariantCulture),
                    SecondEnd = long.Parse(item.SelectSingleNode("SecondEnd").InnerText, CultureInfo.InvariantCulture),
                    Vertices = new List<Point3D>()
                };
              

                if (!AllConductorMaterial.ContainsKey(line.ConductorMaterial))
                    AllConductorMaterial.Add(line.ConductorMaterial, line.ConductorMaterial);
                foreach (XmlNode point in item.SelectSingleNode("Vertices"))
                {
                    ToLatLon(double.Parse(point.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture), double.Parse(point.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture), 34, out var y, out var x);

                    if (!(min_lat <= y && y <= max_lat) || !(min_lon <= x && x <= max_lon))
                    {
                        
                        continue;
                    }
                    line.Vertices.Add(new Point3D()
                    {
                        X = x,
                        Y = y,
                        Z = 1
                    });
                }             
                    entites.Add(line);
            }
        }

      
        public static void RemoveLine(Model3DGroup model3dgroup)
        {
            if(MainWindow.Hide1 == true)
            {
                foreach (var line in LineEntities)
                {
                    ICollection<GeometryModel3D> lines = Objects3D.CreateLines(line);
                    foreach (var item in lines)
                    {
                        var toRemove = model3dgroup.Children.FirstOrDefault(temp => (temp.GetValue(FrameworkElement.TagProperty) as LineEntity)?.Id == line.Id);
                        model3dgroup.Children.Remove(toRemove);
                    }
                }        
            }
        }

        public static void AddLine(Model3DGroup model3dgroup,string active)
        {
            if(MainWindow.Show1 == true)
            {
              
                    foreach (var line in lineEntities)
                    {

                        ICollection<GeometryModel3D> lines = Objects3D.CreateLines(line);
                        foreach (var item in lines)
                        {
                            model3dgroup.Children.Add(item);
                        }
                    }
      
            }
            else if (active == "Open")
            {
                foreach (var line in lineEntities)
                {
                    ICollection<GeometryModel3D> lines = Objects3D.CreateLines(line);
                    foreach (var Switch in switchEntities.Values.Where(s => s.Status == "Open"))
                    {
                        
                        foreach (var item in lines)
                        {
                            if (line.FirstEnd == Switch.Id)
                            {
                                model3dgroup.Children.Add(item);
                            }
                        }
                       
                    }
                }
            }
            else if (active == "Closed")
            {
                foreach (var line in lineEntities)
                {
                    ICollection<GeometryModel3D> lines = Objects3D.CreateLines(line);
                    foreach (var Switch in switchEntities.Values.Where(s => s.Status == "Closed"))
                    {

                        foreach (var item in lines)
                        {
                            if (line.FirstEnd == Switch.Id)
                            {
                                model3dgroup.Children.Add(item);
                            }
                        }

                    }
                }
            }
        }
        public static void Create3DElement(Model3DGroup model3dgroup)
        {
            foreach (var item in substationEntities.Values)
            {
                model3dgroup.Children.Add(Objects3D.Create3Delement(item, model3dgroup));
            }
            foreach (var item in nodeEntities.Values)
            {
                model3dgroup.Children.Add(Objects3D.Create3Delement(item, model3dgroup));
            }
            foreach (var item in switchEntities.Values)
            {
                model3dgroup.Children.Add(Objects3D.Create3Delement(item, model3dgroup));
            }
          

        }

        //Funkcija koja skalira koordinate na xy za sve entitete
        public static void Convert()
        {
            foreach (var item in substationEntities.Values)
            {
                item.X = Convertion(item.X, min_lon, ScaleX);
                item.Y = Convertion(item.Y, min_lat, ScaleY);
            }

            foreach (var item in nodeEntities.Values)
            {
                item.X = Convertion(item.X, min_lon, ScaleX);
                item.Y = Convertion(item.Y, min_lat, ScaleY);
            }

            foreach (var item in switchEntities.Values)
            {
                item.X = Convertion(item.X, min_lon, ScaleX);
                item.Y = Convertion(item.Y, min_lat, ScaleY);
            }

            foreach (var line in LineEntities)
            {
                for (int i = 0; i < line.Vertices.Count; i++)
                {
                    line.Vertices[i] = new Point3D(Convertion(line.Vertices[i].X, min_lon, ScaleX), Convertion(line.Vertices[i].Y, min_lat, ScaleY), line.Vertices[i].Z);
                }

            }

        }
        public static double Convertion(double coordinate,double minLonLat, double scale)
        {
            return Math.Abs((coordinate - minLonLat) / scale);
        }
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            var isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow(Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = (zone * 6.0) - 183.0;
            var lat = y / (c_sa * 0.9996);
            var v = c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow(Math.Cos(lat), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * Math.Cos(lat), 2)) / 3.0;
            var alfa = 3.0 / 4.0 * e2cuadrada;
            var beta = 5.0 / 3.0 * Math.Pow(alfa, 2);
            var gama = 35.0 / 27.0 * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - (alfa * j2) + (beta * j4) - (gama * j6));
            var b = (y - bm) / v;
            var epsi = e2cuadrada * Math.Pow(a, 2) / 2.0 * Math.Pow(Math.Cos(lat), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / Math.Cos(nab));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = (delt * (180.0 / Math.PI)) + s + diflon;
            latitude = ((lat + ((1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)) - (3.0 / 2.0 * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat))) * (tao - lat))) * (180.0 / Math.PI)) + diflat;
        }
    }
}
