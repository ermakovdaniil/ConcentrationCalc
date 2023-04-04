using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using PlenkaAPI.Models;

namespace ViewModels
{
    public class MaterialEdit3VM : ViewModelBase
    {
        public ObservableCollection<Value> values { get; set; }
        public Material Material { get; set; }

        public MaterialEdit3VM(Material material)
        {
            Material = material;
            values = new ObservableCollection<Value>(material.Values);
        }

    }
}