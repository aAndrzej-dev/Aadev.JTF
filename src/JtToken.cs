using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Aadev.JTF
{
    public abstract class JtToken
    {
        private JtConditionCollection? _conditions;
        private IJtParentType? parent;
        private JTemplate template;

        public abstract JTokenType JsonType { get; }
        public abstract JtTokenType Type { get; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Required { get; set; }
        public string? DisplayName { get; set; }
        [Browsable(false)] public IJtParentType? Parent { get => parent; set { parent = value; if (!(parent is null)) template = parent.Template; } }
        public string Id { get; }
        public CustomType? CustomType { get; }

        [Browsable(false)] public JTemplate Template => template;
        [Browsable(false)] public bool IsArrayPrefab => Parent?.Type == JtTokenType.Array;
        [Browsable(false)] public bool IsInArrayPrefab => IsArrayPrefab || Parent?.IsInArrayPrefab is true;
        [Browsable(false)] public bool IsDynamicName => IsArrayPrefab && ((JtArray)Parent!).MakeAsObject;
        [Browsable(false)] public bool IsUsingCustomType => !(CustomType is null);

        public JtConditionCollection Conditions { get => _conditions ??= new JtConditionCollection(); set => _conditions = value; }
        [Browsable(false)] public bool IsExternal => IsUsingCustomType || Parent?.IsExternal is true;


        protected JtToken(JTemplate template)
        {
            this.template = template;
            Id = Guid.NewGuid().ToString();
        }





        protected JtToken(JObject obj, JTemplate template)
        {
            string? typeString = (string?)obj["type"];
            if (typeString?.StartsWith("@") is true)
            {
                CustomType = template.GetCustomType(typeString.AsSpan()[1..].ToString());
            }
            this.template = template;
            Name = (string?)obj["name"];
            Description = (string?)obj["description"];
            Required = (bool)(obj["required"] ?? false);
            DisplayName = (string?)(obj["displayName"] ?? Name);
            Id = (string?)obj["id"] ?? Guid.NewGuid().ToString();

            if (obj["if"] is JArray conditions)
            {
                foreach (JObject item in conditions)
                {
                    Conditions.Add(new JtCondition(item));
                }
            }
        }

        public JtToken[] GetTwinFamily() => Parent is null ? (new JtToken[] { this }) : Parent.Children.Where(x => x.Name == Name && x.Conditions == Conditions).ToArray();

        internal abstract void BulidJson(StringBuilder sb);

        public string GetJson()
        {
            StringBuilder sb = new StringBuilder();
            BulidJson(sb);
            return sb.ToString();
        }

        public static JtToken Create(JObject item, JTemplate template)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            string typeString = (string?)item["type"] ?? throw new Exception("item dont have type");

            if (typeString.Contains('|'))
                typeString = typeString.Split('|')[0];



            if (typeString.StartsWith("#"))
                throw new NotImplementedException("Types whih start with '#' are currently not suported!\nSTOP CODE: JTF_TYPE_DYNAMIC_TYPES_NOT_IMPLEMENTED_EXCEPTION");

            JtTokenType? type;
            if (typeString.StartsWith("@"))
            {
                CustomType? ctype = template.GetCustomType(typeString.AsSpan()[1..].ToString());

                if (ctype is null)
                    return new JtUnknown(item, template);

                type = ctype!.BaseType;
            }
            else
            {
                type = JtTokenType.GetByName(typeString) ?? throw new Exception($"Invalid type: '{typeString}'\nSTOP CODE: JTF_TYPE_INVALID_TYPE_EXCEPTION");
            }

            return type.InstanceFactory(item, template);
        }
    }
}
