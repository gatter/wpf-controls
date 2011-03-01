using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace DemoApplication.Demos
{
    public class DemoItem : UserControl
    {
        public string Label           { get; set; }
        public string Group           { get; set; }
        public bool   IsContentStatic { get; set; }

        public DemoItem()
        {
            IsContentStatic = false;
        }
    }
}
