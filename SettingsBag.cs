using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace DevExpressTreeListDemo
{
    // Wraps an object exposing only its [EditableSetting]-decorated properties
    // to a PropertyGrid. Changes are buffered locally so the caller can either
    // commit them (Apply) or discard them (e.g., on Cancel).
    internal sealed class SettingsBag : ICustomTypeDescriptor
    {
        private readonly object target;
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();
        private readonly Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
        private readonly PropertyDescriptorCollection descriptors;

        public SettingsBag(object target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            this.target = target;

            var list = new List<PropertyDescriptor>();
            foreach (var prop in target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = (EditableSettingAttribute)Attribute.GetCustomAttribute(prop, typeof(EditableSettingAttribute));
                if (attr == null || !prop.CanRead || !prop.CanWrite)
                    continue;

                values[prop.Name] = prop.GetValue(target, null);
                properties[prop.Name] = prop;
                list.Add(new SettingDescriptor(prop.Name, prop.PropertyType, attr, this));
            }
            descriptors = new PropertyDescriptorCollection(list.ToArray(), true);
        }

        public void Apply()
        {
            foreach (var kvp in properties)
                kvp.Value.SetValue(target, values[kvp.Key], null);
        }

        internal object GetValue(string name) => values[name];
        internal void SetValue(string name, object value) => values[name] = value;

        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(target, true);
        public string GetClassName() => TypeDescriptor.GetClassName(target, true);
        public string GetComponentName() => TypeDescriptor.GetComponentName(target, true);
        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(target, true);
        public EventDescriptor GetDefaultEvent() => null;
        public PropertyDescriptor GetDefaultProperty() => null;
        public object GetEditor(Type editorBaseType) => null;
        public EventDescriptorCollection GetEvents() => new EventDescriptorCollection(null);
        public EventDescriptorCollection GetEvents(Attribute[] attributes) => new EventDescriptorCollection(null);
        public PropertyDescriptorCollection GetProperties() => descriptors;
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => descriptors;
        public object GetPropertyOwner(PropertyDescriptor pd) => this;

        private sealed class SettingDescriptor : PropertyDescriptor
        {
            private readonly Type type;
            private readonly SettingsBag owner;

            public SettingDescriptor(string name, Type type, EditableSettingAttribute attr, SettingsBag owner)
                : base(name, new Attribute[]
                {
                    new CategoryAttribute(attr.Category),
                    new DisplayNameAttribute(attr.DisplayName),
                    new DescriptionAttribute(attr.Description)
                })
            {
                this.type = type;
                this.owner = owner;
            }

            public override Type ComponentType => typeof(SettingsBag);
            public override bool IsReadOnly => false;
            public override Type PropertyType => type;
            public override bool CanResetValue(object component) => false;
            public override object GetValue(object component) => owner.GetValue(Name);
            public override void ResetValue(object component) { }
            public override void SetValue(object component, object value) => owner.SetValue(Name, value);
            public override bool ShouldSerializeValue(object component) => false;
        }
    }
}
