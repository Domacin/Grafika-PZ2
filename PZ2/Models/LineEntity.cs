using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfApp1.Model
{
    public class LineEntity
    {
        private long id;
        private string name;
        private bool isUnderground;
        private float r;
        private string conductorMaterial;
        private string lineType;
        private long thermalConstantHeat;
        private long firstEnd;
        private long secondEnd;
        private List<Point3D> vertices;

        public LineEntity()
        {

        }


        public virtual Brush DefaultLineColor(string conductorMaterial)
        {
            if (conductorMaterial == "Steel")
                return Brushes.SpringGreen;
            else if (conductorMaterial == "Copper")
                return Brushes.Goldenrod;
            else if(conductorMaterial == "Acsr")
                return Brushes.LightSlateGray;
            else
                return Brushes.Black;
        }
        public long Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public bool IsUnderground
        {
            get
            {
                return isUnderground;
            }

            set
            {
                isUnderground = value;
            }
        }

        public float R
        {
            get
            {
                return r;
            }

            set
            {
                r = value;
            }
        }

        public string ConductorMaterial
        {
            get
            {
                return conductorMaterial;
            }

            set
            {
                conductorMaterial = value;
            }
        }

        public string LineType
        {
            get
            {
                return lineType;
            }

            set
            {
                lineType = value;
            }
        }

        public long ThermalConstantHeat
        {
            get
            {
                return thermalConstantHeat;
            }

            set
            {
                thermalConstantHeat = value;
            }
        }

        public long FirstEnd
        {
            get
            {
                return firstEnd;
            }

            set
            {
                firstEnd = value;
            }
        }

        public long SecondEnd
        {
            get
            {
                return secondEnd;
            }

            set
            {
                secondEnd = value;
            }
        }

        public List<Point3D> Vertices
        {
            get
            {
                return vertices;
            }

            set
            {
                vertices = value;
            }
        }

        public override string ToString()
        {
            string ret = "ID: " + this.id.ToString();
            ret += "\nName: " + this.name;
            //if (this.isUnderground)
            //    ret += "\nIsUnderground: True";
            //else
            //    ret += "\nIsUnderground: False";
            //ret += "\nR: " + this.r.ToString();
            //ret += "\nConductorMaterial: " + this.conductorMaterial;
            //ret += "\nLineType: " + this.lineType;
            //ret += "ThermalConstantHeat: " + this.thermalConstantHeat.ToString();


            return ret;
        }
    }
}
