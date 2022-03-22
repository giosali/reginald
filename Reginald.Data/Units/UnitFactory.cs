namespace Reginald.Data.Units
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class UnitFactory
    {
        public static Unit CreateUnit(IUnitDataModel model)
        {
            Type type = model.GetType();
            return type switch
            {
                Type when type == typeof(ThemeDataModel) => new Theme(model as ThemeDataModel),
                _ => null,
            };
        }

        public static Unit[] CreateUnits(IEnumerable<IUnitDataModel> models)
        {
            Type type = models.GetType().GetElementType();
            return type switch
            {
                Type when type == typeof(ThemeDataModel) => models.Select(m => new Theme(m as ThemeDataModel)).ToArray(),
                _ => null,
            };
        }
    }
}
