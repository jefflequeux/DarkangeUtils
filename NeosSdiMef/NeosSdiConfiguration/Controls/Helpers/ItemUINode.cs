using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace NeosSdiConfiguration.Controls.Helpers
{
    public class ItemUINode
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public object Tag { get; set; }
        public ObservableCollection<ItemUINode> Child { get; set; }

        public ItemUINode(string _name, object _tag)
        {
            Name = _name;
            Tag = _tag;
            Child = new ObservableCollection<ItemUINode>();
        }
        public ItemUINode(string _name, string _icon, object _tag)
        {
            Name = _name;
            Tag = _tag;
            Child = new ObservableCollection<ItemUINode>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
