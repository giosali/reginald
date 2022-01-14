namespace Reginald.Core.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using Reginald.Core.AbstractFactories;
    using Reginald.Core.AbstractProducts;
    using Reginald.Core.DataModels;

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
