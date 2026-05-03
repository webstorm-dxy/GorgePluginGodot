using System;
using System.Collections.Generic;
using System.Linq;
using Gorge.Native;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.GorgeFramework.Runtime
{
    /// <summary>
    /// 存储运行时使用的模态解析信息
    /// </summary>
    public class RuntimeFormContainer
    {
        /// <summary>
        /// 模态信息
        /// </summary>
        public Dictionary<string, FormInformation> Forms { get; } = new();

        /// <summary>
        /// 运行时Element修改器
        /// key是修改类型，value是修改器表
        /// </summary>
        public Dictionary<string, RuntimeElementContainer> ElementModifiers { get; } = new();

        /// <summary>
        /// 存储在运行时使用的响应效果方法
        /// key是响应效果名称，value是方法
        /// </summary>
        public Dictionary<string, Tuple<GorgeClass, MethodInformation>> InstantAudioMethods { get; } = new();

        public RuntimeFormContainer(GorgeLanguageRuntime languageRuntime)
        {
            foreach (var gorgeClass in languageRuntime.Classes)
            {
                #region 模态

                foreach (var method in gorgeClass.Declaration.StaticMethods)
                {
                    var annotation = method.Annotations.FirstOrDefault(a => a.Name == "Form");
                    if (annotation != null)
                    {
                        if (!annotation.TryGetParameter("name", out var formName))
                        {
                            throw new Exception("Form注解没有name字段");
                        }

                        var name = (string) formName;

                        if (!annotation.TryGetParameter("version", out var formVersion))
                        {
                            throw new Exception("Form注解没有version字段");
                        }

                        var version = (string) formVersion;

                        var elementTypeArray =
                            StringArray.FromGorgeObject(
                                (GorgeObject) gorgeClass.InvokeStaticMethod(method, Array.Empty<object>()));

                        var elementTypeList = new List<string>();
                        for (var i = 0; i < elementTypeArray.length; i++)
                        {
                            elementTypeList.Add(elementTypeArray.Get(i));
                        }

                        Forms.Add(name, new FormInformation(name, version, elementTypeList.ToArray()));
                    }
                }

                #endregion

                #region Element

                if (gorgeClass.Declaration.Is("GorgeFramework.Element"))
                {
                    ElementModifiers.Add(gorgeClass.Declaration.Name, new RuntimeElementContainer(gorgeClass));
                }

                #endregion

                #region 即时音效

                foreach (var method in gorgeClass.Declaration.StaticMethods)
                {
                    var annotation = method.Annotations.FirstOrDefault(a => a.Name == "InstantAudio");
                    if (annotation != null)
                    {
                        if (!annotation.TryGetParameter("name", out var respondEffectName))
                        {
                            throw new Exception("InstantAudio注解没有name字段");
                        }

                        var name = (string) respondEffectName;
                        InstantAudioMethods.Add(name, new Tuple<GorgeClass, MethodInformation>(gorgeClass, method));
                    }
                }

                #endregion
            }
        }
    }

    /// <summary>
    /// 模态信息
    /// </summary>
    public class FormInformation
    {
        /// <summary>
        /// 模态名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 模态版本
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// 元素类型
        /// </summary>
        public string[] ElementTypes { get; }

        public FormInformation(string name, string version, string[] elementTypes)
        {
            Name = name;
            Version = version;
            ElementTypes = elementTypes;
        }
    }

    /// <summary>
    /// 存储运行时所需的Element类信息
    /// </summary>
    public class RuntimeElementContainer
    {
        /// <summary>
        /// 运行时修改器
        /// </summary>
        public List<MethodInformation> Modifiers { get; }

        /// <summary>
        /// 初始创生构造器
        /// 如果为null则表示不进行初始创生
        /// </summary>
        public ConstructorInformation InitializeGenerateConstructor { get; }

        /// <summary>
        /// 正转定时创生构造器
        /// 如果为null则表示不进行正转定时创生
        /// </summary>
        public ConstructorInformation ForwardTimedGenerateConstructor { get; }

        /// <summary>
        /// 正转定时创生时间计算代理
        /// 如果为null则表示不进行正转定时创生
        /// </summary>
        public GorgeDelegate ForwardGenerateTime { get; }

        /// <summary>
        /// 反转定时创生构造器
        /// 如果为null则表示不进行反转定时创生
        /// </summary>
        public ConstructorInformation BackwardTimedGenerateConstructor { get; }

        /// <summary>
        /// 反转定时创生时间计算代理
        /// 如果为null则表示不进行反转定时创生
        /// </summary>
        public GorgeDelegate BackwardGenerateTime { get; }

        public RuntimeElementContainer(GorgeClass gorgeClass)
        {
            #region 修改器

            Modifiers = new List<MethodInformation>();

            foreach (var staticMethod in gorgeClass.Declaration.StaticMethods)
            {
                if (staticMethod.Annotations.Any(a => a.Name == "PeriodModifier"))
                {
                    Modifiers.Add(staticMethod);
                }
            }

            #endregion

            #region 生成方式

            foreach (var constructor in gorgeClass.Declaration.Constructors)
            {
                foreach (var annotation in constructor.Annotations)
                {
                    if (annotation.Name == "InitializeGenerate")
                    {
                        InitializeGenerateConstructor = constructor;
                    }
                    else if (annotation.Name == "ForwardTimedGenerate")
                    {
                        if (!annotation.TryGetMetadata("time", out var timeMetadata))
                        {
                            throw new Exception($"ForwardTimedGenerate注解没有名为time的元数据");
                        }

                        ForwardGenerateTime = (GorgeDelegate) timeMetadata.Value;
                        ForwardTimedGenerateConstructor = constructor;
                    }
                    else if (annotation.Name == "BackwardTimedGenerate")
                    {
                        if (!annotation.TryGetMetadata("time", out var timeMetadata))
                        {
                            throw new Exception($"BackwardTimedGenerate注解没有名为time的元数据");
                        }

                        BackwardGenerateTime = (GorgeDelegate) timeMetadata.Value;
                        BackwardTimedGenerateConstructor = constructor;
                    }
                }
            }

            #endregion
        }
    }
}