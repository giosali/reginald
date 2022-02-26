namespace Reginald.Data.Units
{
    using System;

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
