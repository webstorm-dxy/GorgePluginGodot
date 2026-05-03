using System;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeLanguage.Objective;

namespace Gorge.Native.GorgeFramework
{
    public partial class Environment
    {
        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        public static partial Asset GetAssetByName(string assetName)
        {
            return RuntimeStatic.Runtime.Score.GetAssetByName(assetName);
        }

        public static partial Element FindAliveLane(string typeName, string laneName)
        {
            return Element.FromGorgeObject(RuntimeStatic.Runtime.SimulationRuntime.Chart.AliveElements.Find(
                lane =>
                    lane.RealObject.GorgeClass.Declaration.Name == typeName &&
                    lane.RealObject.GetStringField("name") == laneName));
        }

        public static partial Vector3 ScreenToWorldPoint(Vector3 position)
        {
            return Base.Instance.ScreenToWorldPoint(position);
        }

        public static partial void Scoring(int result)
        {
            // Debug.Log(RespondResult.Enum.Values[result]);
            var scoring = RuntimeStatic.Runtime.SimulationRuntime.Scene.Scoring;
            scoring?.Respond(result);
        }

        public static partial void PlayRespondEffect(string name)
        {
            RuntimeStatic.Runtime.SimulationRuntime.Audio.PlayRespondEffect(name);
        }
    }
}