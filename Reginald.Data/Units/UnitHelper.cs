namespace Reginald.Data.Units
{
    using System.Collections.Generic;

    public static class UnitHelper
    {
        public static Unit ToUnit(UnitDataModelBase model)
        {
            AccessoryFactory factory = new();
            UnitClient client = new(factory, model);
            return client.Unit;
        }

        public static IEnumerable<Unit> ToUnits(IEnumerable<UnitDataModelBase> models)
        {
            AccessoryFactory factory = new();
            UnitClient client = new(factory, models);
            return client.Units;
        }
    }
}
