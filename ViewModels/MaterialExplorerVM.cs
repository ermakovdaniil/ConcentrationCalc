using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using PlenkaAPI.Data;
using PlenkaAPI.Models;

using ViewModels.Utils;

namespace ViewModels
{
    public class MaterialExplorerVM : ViewModelBase
    {
        #region Properties

        public List<Material> Materials { get; set; }
        public Material SelectedMaterial { get; set; }


        #endregion


        #region Functions

        #region Constructors

        public MaterialExplorerVM()
        {
            var con = DbContextSingleton.GetInstance();
            Materials = con.Materials.ToList();
        }

        #endregion

        #endregion


        #region Commands

        private RelayCommand _addNewMaterial;

        public RelayCommand AddNewMaterial
        {
            get
            {
                return _addNewMaterial ??= new RelayCommand(o =>
                {
                    ShowChildWindow(new );
                });
            }
        }

        private RelayCommand _editMaterial;

        public RelayCommand EditMaterial
        {
            get
            {
                return _editMaterial ??= new RelayCommand(o =>
                    {
                        
                    },
                    (c) => SelectedMaterial!=null
                );
            }
        }
        #endregion

    }
}