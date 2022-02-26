namespace Reginald.Data.Units
{
    using System.Collections.Generic;
    using System.Linq;

    public class UnitClient
    {
        public UnitClient(UnitFactory factory, UnitDataModelBase model)
        {
            Unit = factory.CreateUnit(model);
        }

        public UnitClient(UnitFactory factory, IEnumerable<UnitDataModelBase> models)
        {
            List<Unit> units = new(models.Count());
            foreach (UnitDataModelBase model in models)
            {
                units.Add(factory.CreateUnit(model));
            }

            Units = units;
        }

        public Unit Unit { get; set; }

        public IEnumerable<Unit> Units { get; set; }
    }
}
