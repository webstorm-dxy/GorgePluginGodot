using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using Gorge.GorgeLanguage.Objective;

namespace Gorge.Native.Gorge
{
    public abstract partial class Injector
    {
        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        public abstract ClassDeclaration InjectedClassDeclaration { get; }

        public abstract GorgeObject Instantiate(int constructorIndex, params object[] args);

        #region Injector对编译时提供的字段操作

        public virtual void SetInjectorInt(int index, int value)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的int类型字段");
        }

        public virtual void SetInjectorIntDefault(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的int类型字段");
        }

        public virtual int GetInjectorInt(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的int类型字段");
        }

        public virtual bool GetInjectorIntDefault(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的int类型字段");
        }

        public virtual void SetInjectorFloat(int index, float value)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的float类型字段");
        }

        public virtual void SetInjectorFloatDefault(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的float类型字段");
        }

        public virtual float GetInjectorFloat(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的float类型字段");
        }

        public virtual bool GetInjectorFloatDefault(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的float类型字段");
        }

        public virtual void SetInjectorBool(int index, bool value)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的bool类型字段");
        }

        public virtual void SetInjectorBoolDefault(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的bool类型字段");
        }

        public virtual bool GetInjectorBool(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的bool类型字段");
        }

        public virtual bool GetInjectorBoolDefault(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的bool类型字段");
        }

        public virtual void SetInjectorString(int index, string value)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的string类型字段");
        }

        public virtual void SetInjectorStringDefault(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的string类型字段");
        }

        public virtual string GetInjectorString(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的string类型字段");
        }

        public virtual bool GetInjectorStringDefault(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的string类型字段");
        }

        public virtual void SetInjectorObject(int index, GorgeObject value)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的object类型字段");
        }

        public virtual void SetInjectorObjectDefault(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的object类型字段");
        }

        public virtual GorgeObject GetInjectorObject(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的object类型字段");
        }

        public virtual bool GetInjectorObjectDefault(int index)
        {
            throw new Exception($"类{InjectedClassDeclaration.Name}的Injector无索引为{index}的object类型字段");
        }

        #endregion

        public abstract bool EditableEquals(Injector target);

        /// <summary>
        /// 将字段值拷贝到目标Injector中
        /// </summary>
        /// <param name="toInjector"></param>
        public void CloneTo(Injector toInjector)
        {
        }

        /// <summary>
        /// 输出内容字符串
        /// </summary>
        /// <returns></returns>
        public string ToDisplayString(int indent = 0)
        {
            var writer = new StringWriter();
            var stringBuilder = new IndentedTextWriter(writer, "  ");
            stringBuilder.Indent = indent;
            stringBuilder.WriteLine($"{InjectedClassDeclaration.Name}:");
            stringBuilder.WriteLine("{");

            stringBuilder.Indent = indent + 1;

            for (var i = 0; i < InjectedClassDeclaration.InjectorFieldCount; i++)
            {
                if (!InjectedClassDeclaration.TryGetInjectorFieldById(i, out var field))
                {
                    continue;
                }

                string value;
                switch (field.Type.BasicType)
                {
                    case BasicType.Int:
                    case BasicType.Enum:
                        if (GetInjectorIntDefault(field.Index))
                        {
                            continue;
                        }

                        value = GetInjectorInt(field.Index).ToString();
                        break;
                    case BasicType.Float:
                        if (GetInjectorFloatDefault(field.Index))
                        {
                            continue;
                        }

                        value = GetInjectorFloat(field.Index).ToString(CultureInfo.InvariantCulture);
                        break;
                    case BasicType.Bool:
                        if (GetInjectorBoolDefault(field.Index))
                        {
                            continue;
                        }

                        value = GetInjectorBool(field.Index).ToString();
                        break;
                    case BasicType.String:
                        if (GetInjectorStringDefault(field.Index))
                        {
                            continue;
                        }

                        value = GetInjectorString(field.Index).ToString();
                        break;
                    case BasicType.Object:
                        if (GetInjectorObjectDefault(field.Index))
                        {
                            continue;
                        }

                        if (field.Type.FullName == "Gorge.Injector")
                        {
                            value = "\n" + ((Injector) GetInjectorObject(field.Index)).ToDisplayString(indent + 2);
                        }
                        else
                        {
                            value = GetInjectorObject(field.Index).ToString();
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                stringBuilder.WriteLine($"{field.Name}: {value}");
            }

            stringBuilder.Indent = indent;
            stringBuilder.WriteLine("}");

            var result = writer.ToString();
            stringBuilder.Close();
            writer.Close();
            return result;
        }
    }
}