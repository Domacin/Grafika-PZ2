using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WpfApp1.Model;

namespace PZ2.Common
{
    public class HitAndTollTip
    {
        private ToolTip toolTip = new ToolTip();
        private Window window;
        private Model3DGroup group3d;
        private Viewport3D viewport;
        HashSet<GeometryModel3D> previusModels;
        HashSet<GeometryModel3D> previousLine;
        HashSet<GeometryModel3D> previousSwtich;
        string materijal = "";

        public HitAndTollTip(Viewport3D viewpor, Model3DGroup model3DGroup, Window window)
        {
            previusModels = new HashSet<GeometryModel3D>();
            previousLine = new HashSet<GeometryModel3D>();
            previousSwtich = new HashSet<GeometryModel3D>();
            this.window = window;
            this.group3d = model3DGroup;
            this.viewport = viewpor;
            viewpor.MouseLeftButtonDown += MouseLeftButtonDown;
            CreateToolTip();
        }

        private HitTestResultBehavior HitResult(HitTestResult result)
        {
            ResetSwitchColor();
            var hitResult = result as RayHitTestResult;
            var value = hitResult?.ModelHit.GetValue(FrameworkElement.TagProperty);
            if (value is PowerEntity)
            {
                
                 
                toolTip.Content = value.ToString();
                toolTip.IsOpen = true;
                if (value is NodeEntity)
                    toolTip.Content += "\nType: Node";
                else if (value is SubstationEntity)
                    toolTip.Content += "\nType: Substation";
                else if (value is SwitchEntity)
                {
                    toolTip.Content += "\nType: Switch";
                    var swich = value as SwitchEntity;
                    var switchhh = group3d.Children.FirstOrDefault(item => (item.GetValue(FrameworkElement.TagProperty) as SwitchEntity)?.Id == swich.Id);
                    var switch3D = (switchhh as GeometryModel3D);
                    if(switch3D is object)
                    {
                        if(swich.Status == "Open")
                        {
                            switch3D.Material = new DiffuseMaterial(Brushes.Green);
                            previousSwtich.Add(switch3D);
                        }else if(swich.Status == "Closed")
                        {
                            switch3D.Material = new DiffuseMaterial(Brushes.Red);
                            previousSwtich.Add(switch3D);
                        }
                    }
                }
                   

                
                     
            }
            if (value is LineEntity)
            {
              
                
                RestoreColor();
                ResetLineColor();
                toolTip.Content = value.ToString();
                toolTip.Content += "\nType: Line";
                toolTip.IsOpen = true;
                var line = value as LineEntity;
                var a = hitResult.ModelHit as GeometryModel3D;
                var first = group3d.Children.FirstOrDefault(item => (item.GetValue(FrameworkElement.TagProperty) as PowerEntity)?.Id == line.FirstEnd);
                var second = group3d.Children.FirstOrDefault(item => (item.GetValue(FrameworkElement.TagProperty) as PowerEntity)?.Id == line.SecondEnd);
                var linija = group3d.Children.FirstOrDefault(item => (item.GetValue(FrameworkElement.TagProperty) as LineEntity)?.Id == line.Id);

                materijal = line.ConductorMaterial;
                var first3D = (first as GeometryModel3D);
                var second3D = (second as GeometryModel3D);
                var line3d = (linija as GeometryModel3D);    
                
                if (first3D is object)
                {
                    first3D.Material = new DiffuseMaterial(Brushes.Magenta);
                    previusModels.Add(first3D);
                }
                if (second3D is object)
                {
                    second3D.Material = new DiffuseMaterial(Brushes.Magenta);
                    previusModels.Add(second3D);
                }
                if(line is object)
                {
                    if(line.R < 1)
                    {
                        line3d.Material = new DiffuseMaterial(Brushes.Red);
                        previousLine.Add(line3d);
                    }else if(line.R >1 && line.R < 2)
                    {
                        line3d.Material = new DiffuseMaterial(Brushes.Orange);
                        previousLine.Add(line3d);
                    }else if(line.R > 2)
                    {
                        line3d.Material = new DiffuseMaterial(Brushes.Yellow);
                        previousLine.Add(line3d);
                    }

                }


            }
            return HitTestResultBehavior.Stop;
        }

        private void ResetSwitchColor()
        {
            foreach (var item in previousSwtich)
            {
                SwitchEntity value = (SwitchEntity)item.GetValue(FrameworkElement.TagProperty);
                item.Material = new DiffuseMaterial(value.DefaultColor());
            }
        }
        private void ResetLineColor()
        {
            foreach (var item in previousLine)
            {
                LineEntity value = (LineEntity)item.GetValue(FrameworkElement.TagProperty);
                item.Material = new DiffuseMaterial(value.DefaultLineColor(materijal));
            }
        }
        private void RestoreColor()
        {
            foreach (var item in previusModels)
            {
                PowerEntity value = (PowerEntity)item.GetValue(FrameworkElement.TagProperty);
                item.Material = new DiffuseMaterial(value.DefaultColor());
            }
            previusModels.Clear();
        }

        private void CreateToolTip()
        {
            toolTip.StaysOpen = false;
            toolTip.IsOpen = false;
            toolTip.Background = Brushes.Beige;
            toolTip.BorderBrush = Brushes.Black;
            toolTip.Foreground = Brushes.DarkSlateGray;
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            toolTip.IsOpen = false;
            var mouseposition = e.GetPosition(viewport);
            var testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            var testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);
            var pointparams = new PointHitTestParameters(mouseposition);
            var rayparams = new RayHitTestParameters(testpoint3D, testdirection);
            VisualTreeHelper.HitTest(viewport, null, HitResult, pointparams);
        }

       

    }
}
