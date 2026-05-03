using System.Collections.Generic;

namespace Gorge.GorgeLanguage.Objective
{
    public class InjectorFieldDeclaration
    {
        public InjectorFieldDeclaration(string name, GorgeType type, bool hasDefaultValue, Dictionary<string,Metadata> metadata)
        {
            Name = name;
            Type = type;
            HasDefaultValue = hasDefaultValue;
            Metadata = metadata;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public GorgeType Type { get; }

        /// <summary>
        /// 是否拥有默认值
        /// </summary>
        public bool HasDefaultValue { get; }

        public Dictionary<string, Metadata> Metadata { get; }
    }
}