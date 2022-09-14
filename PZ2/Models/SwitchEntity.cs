using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1.Model
{
    public class SwitchEntity : PowerEntity
    {
        private string status;

        public override Brush DefaultColor()
        {
            return Brushes.Purple;
        }

        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }
    }
}
