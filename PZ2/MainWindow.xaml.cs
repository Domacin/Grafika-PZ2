using PZ2.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PZ2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   //Promenljive za zoom
        private int zoomMax = 13;
        private int zoomCurent = 1;

        //Promenljive za pomeranje
        private Point start = new Point();
        private Point diffOffset = new Point();

        //Promenljive za rotaciju
        private bool rotating;
        private Quaternion rotationDelta;
        private Quaternion rotation;

        //Promenljive koje se koriste za prikaz i sakrivanje vodova
        private static  bool show, hide;

        public static HashSet<WpfApp1.Model.LineEntity> lineEntities = new HashSet<WpfApp1.Model.LineEntity>();
        //public static HashSet<WpfApp1.Model.LineEntity> deletedLines = new HashSet<WpfApp1.Model.LineEntity>();

        
        HitAndTollTip haTP;

        //Promenljiva koja mi govori da li je u switchu status Open ili Closed
        string active = "";

        public static bool Show1 { get => show; set => show = value; }
        public static bool Hide1 { get => hide; set => hide = value; }

        public MainWindow()
        {
            rotationDelta = Quaternion.Identity;
            InitializeComponent();
            Convertions.LoadAllModels();
            Convertions.Convert();
            Convertions.Create3DElement(Group);
            lineEntities = Convertions.LineEntities;
            haTP = new HitAndTollTip(viewport, Group, this);


        }
        //Zoom in and zoom out funkcija
        private void viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point p = e.MouseDevice.GetPosition(this);
            double scaleX = 1;
            double scaleY = 1;
            double scaleZ = 1;
            if (e.Delta > 0 && zoomCurent < zoomMax)
            {
                scaleX = skaliranje.ScaleX + 0.1;
                scaleY = skaliranje.ScaleY + 0.1;
                scaleZ = skaliranje.ScaleZ + 0.1;

                skaliranje.CenterX = 5;
                skaliranje.CenterY = 5;
                skaliranje.CenterZ = 0;

                skaliranje.ScaleX = scaleX;
                skaliranje.ScaleY = scaleY;
                skaliranje.ScaleZ = scaleZ;
                zoomCurent++;
            }
            else if (e.Delta <= 0 && zoomCurent > -zoomMax)
            {
                scaleX = skaliranje.ScaleX - 0.1;
                scaleY = skaliranje.ScaleY - 0.1;
                scaleZ = skaliranje.ScaleZ - 0.1;

                skaliranje.CenterX = 5;
                skaliranje.CenterY = 5;
                skaliranje.CenterZ = 0;

                skaliranje.ScaleX = scaleX;
                skaliranje.ScaleY = scaleY;
                skaliranje.ScaleZ = scaleZ;

                zoomCurent--;
            }
        }

        //akcija koja se izvesava kada se pritisne lijevi taster misa
        private void viewport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewport.CaptureMouse();
            start = e.GetPosition(this);
            diffOffset.X = translacija.OffsetX;
            diffOffset.Y = translacija.OffsetY;
        }

        //akcija koja se izvrsava kada se pusti lijevi taster misa
        private void viewport_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewport.ReleaseMouseCapture();
        }

        //akcija koja ze izvrsava kada se mis pomjera
        private void viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewport.IsMouseCaptured)
            {
                Point end = e.GetPosition(this);
                double offsetX = end.X - start.X;
                double offsetY = end.Y - start.Y;
                double w = this.Width;
                double h = this.Height;
                double translateX = (offsetX * 200) / w;
                double translateY = -(offsetY * 200) / h;

                if (rotating)
                {
                   var xAngle = (RotaionX.Angle + -translateX) % 360;
                   var yAngle = (RotaionY.Angle + translateY) % 360;

                    if (-75 < yAngle && yAngle < 75)
                    {
                        RotaionY.Angle = yAngle;
                    }
                    if (-20 < xAngle && xAngle < 75)
                    {
                        RotaionX.Angle = xAngle;
                    }
                    start = end;
                }
                else
                {
                    translacija.OffsetX = diffOffset.X + (translateX / (100 * skaliranje.ScaleX));
                    translacija.OffsetY = diffOffset.Y + (translateY / (100 * skaliranje.ScaleX));
                    translacija.OffsetZ = translacija.OffsetZ;
                }

          
            }
        }

        //akcija na pritiska srednejg(scroll) dugmeta na misu
        private void viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Pressed)
            {
                rotating = true;
                viewport.CaptureMouse();
                start = e.GetPosition(this);

                diffOffset.X = translacija.OffsetX;
                diffOffset.Y = translacija.OffsetY;
            }
        }


        private void RadioShow_Checked(object sender, RoutedEventArgs e)
        {
            active = "";
            Show1 = true;
            Hide1 = false; 
            Convertions.AddLine(Group,active);
        }

        private void RadioHide_Checked(object sender, RoutedEventArgs e)
        {
            active = "";
            Hide1 = true;
            Show1 = false;
            Convertions.RemoveLine(Group);
        }

        private void OpenStatus_Checked(object sender, RoutedEventArgs e)
        {
            active = "Open";
            Hide1 = false;
            Show1 = false;
            Convertions.AddLine(Group, active);
        }

        private void CloseStatus_Checked(object sender, RoutedEventArgs e)
        {
            active = "Closed";
            Hide1 = false;
            Show1 = false;
            Convertions.AddLine(Group, active);
        }

        private void viewport_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (rotating)
            {
                rotation = rotationDelta * rotation;
            }

            if(e.MiddleButton == MouseButtonState.Released)
            {
                rotating = false;
                viewport.ReleaseMouseCapture();
            }
        }

       
    }
}
