using Caliburn.Micro;
using Reginald.Core.AbstractProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reginald.ViewModels
{
    public abstract class UnitViewModelBase<T> : ViewViewModelBase
    {
        private BindableCollection<Unit> _units = new();
        public BindableCollection<Unit> Units
        {
            get => _units;
            set
            {
                _units = value;
                NotifyOfPropertyChange(() => Units);
            }
        }

        private T _selectedUnit;
        public T SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;
                NotifyOfPropertyChange(() => SelectedUnit);
            }
        }

        public UnitViewModelBase()
        {

        }
    }
}
