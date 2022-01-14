namespace Reginald.Core.AbstractFactories
{
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.DataModels;

    public abstract class UnitFactory
    {
        public abstract Unit CreateUnit(UnitDataModelBase model);
    }
}
