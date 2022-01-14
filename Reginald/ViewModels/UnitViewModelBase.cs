namespace Reginald.ViewModels
{
    using Caliburn.Micro;
    using Reginald.Core.AbstractProducts;

    public abstract class UnitViewModelBase<T> : ViewViewModelBase
    {
        private BindableCollection<Unit> _units = new();

        private T _selectedUnit;

        public UnitViewModelBase()
        {
        }

        public BindableCollection<Unit> Units
        {
            get => _units;
            set
            {
                _units = value;
                NotifyOfPropertyChange(() => Units);
            }
        }

        public T SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;
                NotifyOfPropertyChange(() => SelectedUnit);
            }
        }
    }
}
