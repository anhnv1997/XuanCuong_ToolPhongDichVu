using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Extensions
{
    public static class Extensions
    {
        public static void ToggleDoubleBuffered<TControl>(this TControl control, bool isOn)
            where TControl : Control
        {
            PropertyInfo pi = control.GetType().GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, isOn, null);
        }
    }
}
