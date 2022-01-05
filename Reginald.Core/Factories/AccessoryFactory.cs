using Reginald.Core.AbstractFactories;
using Reginald.Core.AbstractProducts;
using Reginald.Core.DataModels;
using Reginald.Core.Products;
using System;

namespace Reginald.Core.Factories
{
    public class AccessoryFactory : UnitFactory
    {
        public override Unit CreateUnit(UnitDataModelBase model)
        {
            Type type = model.GetType();
            return type switch
            {
                Type when type == typeof(ThemeDataModel) => new Theme(model as ThemeDataModel),
                _ => null,
            };
        }
    }
}
