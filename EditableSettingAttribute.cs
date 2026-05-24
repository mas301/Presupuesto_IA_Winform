using System;

namespace PresupuestoIA
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class EditableSettingAttribute : Attribute
    {
        public string Category { get; }
        public string DisplayName { get; }
        public string Description { get; }

        public EditableSettingAttribute(string category, string displayName, string description)
        {
            Category = category;
            DisplayName = displayName;
            Description = description;
        }
    }
}
