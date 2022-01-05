using Reginald.Core.AbstractFactories;
using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace Reginald.Core.Clients
{
    public class UnitClient
    {
        public Unit Unit { get; set; }

        public IEnumerable<Unit> Units { get; set; }

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
    }
}
