using System;
using Gorge.GorgeFramework.Utilities;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;
using Newtonsoft.Json;

namespace Gorge.Native.GorgeFramework
{
    public partial class TouchSignal
    {
        private TouchSignal(Injector injector, bool isTouching, Vector2 position)
        {
            FieldInitialize(injector);

            this.isTouching = isTouching;
            this.position = position;
        }

        [JsonConstructor]
        public TouchSignal(bool isTouching, Vector2 position)
        {
            this.isTouching = isTouching;
            this.position = position;
        }

        public override bool Equals(object obj)
        {
            return obj is TouchSignal touch && Equals(touch);
        }

        private bool Equals(TouchSignal other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return isTouching == other.isTouching && position.Equals(other.position);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(isTouching, position);
        }

        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        private static partial bool InitializeField_isTouching() => default;

        private static partial Vector2 InitializeField_position() => default;

        public override string ToString()
        {
            return $"{nameof(isTouching)}: {isTouching}, {nameof(position)}: {position}";
        }
    }
}