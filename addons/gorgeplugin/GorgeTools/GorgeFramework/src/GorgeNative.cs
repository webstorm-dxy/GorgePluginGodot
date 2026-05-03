using System;
using System.Collections.Generic;
using System.Reflection;
using Gorge.GorgeFramework.Chart;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeLanguage.Objective.Attributes;

namespace Gorge.GorgeFramework
{
    public static class GorgeNative
    {
        private static readonly Lazy<List<GorgeClass>> LazyNativeClasses = new(() =>
        {
            var nativeClasses = new List<GorgeClass>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

                    foreach (var field in fields)
                    {
                        if (field.GetCustomAttribute<GorgeNativeClassAttribute>() != null)
                        {
                            if (field.GetValue(null) is GorgeClass gorgeClass)
                            {
                                nativeClasses.Add(gorgeClass);
                            }
                        }
                    }
                }
            }

            return nativeClasses;
        });

        public static readonly List<GorgeClass> NativeClasses = LazyNativeClasses.Value;

        private static readonly Lazy<List<GorgeEnum>> LazyNativeEnums = new(() =>
        {
            var nativeEnums = new List<GorgeEnum>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

                    foreach (var field in fields)
                    {
                        if (field.GetCustomAttribute<GorgeNativeEnumAttribute>() != null)
                        {
                            if (field.GetValue(null) is GorgeEnum gorgeEnum)
                            {
                                nativeEnums.Add(gorgeEnum);
                            }
                        }
                    }
                }
            }

            return nativeEnums;
        });

        public static readonly List<GorgeEnum> NativeEnums = LazyNativeEnums.Value;

        private static readonly Lazy<List<GorgeInterface>> LazyNativeInterfaces = new(() =>
        {
            var nativeInterfaces = new List<GorgeInterface>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

                    foreach (var field in fields)
                    {
                        if (field.GetCustomAttribute<GorgeNativeInterfaceAttribute>() != null)
                        {
                            if (field.GetValue(null) is GorgeInterface gorgeInterface)
                            {
                                nativeInterfaces.Add(gorgeInterface);
                            }
                        }
                    }
                }
            }

            return nativeInterfaces;
        });

        public static readonly List<GorgeInterface> NativeInterface = LazyNativeInterfaces.Value;

        public static IImplementationBase GorgeNativeImplementationBase()
        {
            return new NativeImplementationBase(NativeClasses, NativeInterface, NativeEnums);
        }

        public static Package GorgeCommonLibPackage()
        {
            return Package.LoadFolderPackage("./Packages/cn.starbow.gorge.unity/Runtime/Resources/gorge/code/Native",
                false);
        }
    }
}