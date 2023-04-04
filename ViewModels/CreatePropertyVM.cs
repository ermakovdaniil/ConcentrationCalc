using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlenkaAPI.Data;
using PlenkaAPI.Models;

using ViewModels.Utils;

namespace ViewModels
{
    public class CreatePropertyVM: ViewModelBase
    {
        public Property Property { get; set; }

        public List<Unit> Units
        {
            get
            {
                return DbContextSingleton.GetInstance().Units.ToList();
            } 
        }


        public CreatePropertyVM(Property property)
        {
            Property = property;
        }

        private RelayCommand _saveProperty;



        public RelayCommand SaveProperty
        {
            get
            {
                return _saveProperty ?? (_saveProperty = new RelayCommand(o =>
                {
                    
                }));
            }
        }

    }
}
