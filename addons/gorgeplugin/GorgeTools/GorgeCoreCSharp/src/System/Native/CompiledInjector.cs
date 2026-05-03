using System;
using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeLanguage.VirtualMachine;

namespace Gorge.Native.Gorge
{
    public class CompiledInjector : Injector
    {
        public override ClassDeclaration InjectedClassDeclaration { get; }

        private readonly Tuple<int, bool>[] _int;
        private readonly Tuple<float, bool>[] _float;
        private readonly Tuple<bool, bool>[] _bool;
        private readonly Tuple<string, bool>[] _string;
        private readonly Tuple<GorgeObject, bool>[] _object;

        public CompiledInjector(ClassDeclaration injectedClassDeclaration)
        {
            InjectedClassDeclaration = injectedClassDeclaration;
            _int = new Tuple<int, bool>[injectedClassDeclaration.InjectorFieldTypeCount.Int];
            _float = new Tuple<float, bool>[injectedClassDeclaration.InjectorFieldTypeCount.Float];
            _bool = new Tuple<bool, bool>[injectedClassDeclaration.InjectorFieldTypeCount.Bool];
            _string = new Tuple<string, bool>[injectedClassDeclaration.InjectorFieldTypeCount.String];
            _object = new Tuple<GorgeObject, bool>[injectedClassDeclaration.InjectorFieldTypeCount.Object];

            for (var i = 0; i < _int.Length; i++)
            {
                _int[i] = new Tuple<int, bool>(default, true);
            }

            for (var i = 0; i < _float.Length; i++)
            {
                _float[i] = new Tuple<float, bool>(default, true);
            }

            for (var i = 0; i < _bool.Length; i++)
            {
                _bool[i] = new Tuple<bool, bool>(default, true);
            }

            for (var i = 0; i < _string.Length; i++)
            {
                _string[i] = new Tuple<string, bool>(default, true);
            }

            for (var i = 0; i < _object.Length; i++)
            {
                _object[i] = new Tuple<GorgeObject, bool>(default, true);
            }
        }

        public override GorgeObject Instantiate(int constructorIndex, params object[] args)
        {
            // 获取类实现，TODO 可以缓存
            var gorgeClass = GorgeLanguageRuntime.Instance.GetClass(InjectedClassDeclaration.Name);

            if (!gorgeClass.Declaration.TryGetConstructorById(constructorIndex, out var constructor))
            {
                throw new Exception($"类{gorgeClass.Declaration.Name}没有编号为{constructorIndex}的构造方法");
            }

            for (var i = 0; i < args.Length; i++)
            {
                switch (constructor.Parameters[i].Type.BasicType)
                {
                    case BasicType.Int:
                        InvokeParameterPool.Int[constructor.Parameters[i].Index] = (int) args[i];
                        break;
                    case BasicType.Float:
                        InvokeParameterPool.Float[constructor.Parameters[i].Index] = (float) args[i];
                        break;
                    case BasicType.Bool:
                        InvokeParameterPool.Bool[constructor.Parameters[i].Index] = (bool) args[i];
                        break;
                    case BasicType.Enum:
                        InvokeParameterPool.Int[constructor.Parameters[i].Index] = (int) args[i];
                        break;
                    case BasicType.String:
                        InvokeParameterPool.String[constructor.Parameters[i].Index] = (string) args[i];
                        break;
                    case BasicType.Object:
                        InvokeParameterPool.Object[constructor.Parameters[i].Index] = (GorgeObject) args[i];
                        break;
                    default:
                        throw new Exception("不支持该类型");
                }
            }

            InvokeParameterPool.Injector = this;
            gorgeClass.InvokeConstructor(constructorIndex);

            return InvokeParameterPool.ObjectReturn;
        }

        #region Injector对编译时提供的字段操作

        public override void SetInjectorInt(int index, int value)
        {
            _int[index] = new Tuple<int, bool>(value, false);
        }

        public override void SetInjectorIntDefault(int index)
        {
            _int[index] = new Tuple<int, bool>(default, true);
        }

        public override int GetInjectorInt(int index)
        {
            return _int[index].Item1;
        }

        public override bool GetInjectorIntDefault(int index)
        {
            return _int[index].Item2;
        }

        public override void SetInjectorFloat(int index, float value)
        {
            _float[index] = new Tuple<float, bool>(value, false);
        }

        public override void SetInjectorFloatDefault(int index)
        {
            _float[index] = new Tuple<float, bool>(default, true);
        }

        public override float GetInjectorFloat(int index)
        {
            return _float[index].Item1;
        }

        public override bool GetInjectorFloatDefault(int index)
        {
            return _float[index].Item2;
        }

        public override void SetInjectorBool(int index, bool value)
        {
            _bool[index] = new Tuple<bool, bool>(value, false);
        }

        public override void SetInjectorBoolDefault(int index)
        {
            _bool[index] = new Tuple<bool, bool>(default, true);
        }

        public override bool GetInjectorBool(int index)
        {
            return _bool[index].Item1;
        }

        public override bool GetInjectorBoolDefault(int index)
        {
            return _bool[index].Item2;
        }

        public override void SetInjectorString(int index, string value)
        {
            _string[index] = new Tuple<string, bool>(value, false);
        }

        public override void SetInjectorStringDefault(int index)
        {
            _string[index] = new Tuple<string, bool>(default, true);
        }

        public override string GetInjectorString(int index)
        {
            return _string[index].Item1;
        }

        public override bool GetInjectorStringDefault(int index)
        {
            return _string[index].Item2;
        }

        public override void SetInjectorObject(int index, GorgeObject value)
        {
            _object[index] = new Tuple<GorgeObject, bool>(value, false);
        }

        public override void SetInjectorObjectDefault(int index)
        {
            _object[index] = new Tuple<GorgeObject, bool>(default, true);
        }

        public override GorgeObject GetInjectorObject(int index)
        {
            return _object[index].Item1;
        }

        public override bool GetInjectorObjectDefault(int index)
        {
            return _object[index].Item2;
        }

        #endregion

        public override bool EditableEquals(Injector target)
        {
            throw new NotImplementedException("暂未实现比较器");
        }

        public override GorgeObject Clone()
        {
            return Clone(InjectedClassDeclaration);
        }

        public GorgeObject Clone(ClassDeclaration classDeclaration)
        {
            var newInjector = new CompiledInjector(classDeclaration);

            Array.Copy(_int, newInjector._int, _int.Length);
            Array.Copy(_float, newInjector._float, _float.Length);
            Array.Copy(_bool, newInjector._bool, _bool.Length);
            Array.Copy(_string, newInjector._string, _string.Length);

            for (var i = 0; i < _object.Length; i++)
            {
                newInjector._object[i] = new Tuple<GorgeObject, bool>(_object[i].Item1?.Clone(), _object[i].Item2);
            }

            return newInjector;
        }
    }
}