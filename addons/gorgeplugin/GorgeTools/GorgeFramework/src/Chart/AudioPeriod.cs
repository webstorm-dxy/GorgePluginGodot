using System.Linq;
using System.Text;
using Gorge.Native.Gorge;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Chart
{
    public class AudioPeriod : Period
    {
        /// <summary>
        /// 音频资源注入器
        /// </summary>
        public Injector AudioInjector { get; private set; }

        public AudioAsset AudioAsset { get; private set; }

        /// <summary>
        /// 当前乐段的音频
        /// 如果加载失败则为null
        /// </summary>
        public Audio Audio { get; private set; }

        public AudioPeriod(string methodName, Injector configInjector, Injector audioInjector) : base(methodName,
            configInjector)
        {
            UpdateAudio(audioInjector);
        }

        public void UpdateAudio(Injector audioInjector)
        {
            AudioInjector = audioInjector;
            if (AudioInjector == null)
            {
                AudioAsset = null;
                Audio = null;
            }
            else
            {
                var constructorId = audioInjector.InjectedClassDeclaration.Constructors
                    .First(c => c.Parameters.Length == 0)
                    .Id;
                AudioAsset = AudioAsset.FromGorgeObject(AudioInjector.Instantiate(constructorId));
                Audio = AudioAsset.LoadAsset() ? AudioAsset.GetAsset() : null;
            }
        }

        /// <summary>
        /// 转换为代码
        /// </summary>
        /// <returns></returns>
        public override string ToGorgeCode(int indentation)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[", indentation);
            sb.AppendLine(
                $"GorgeFramework.PeriodConfig^ config = {InjectorHardcodeGenerator.Generate(ConfigInjector, indentation + 1)}",
                indentation + 1);
            sb.AppendLine("]", indentation);
            sb.AppendLine($"@Song", indentation);
            sb.AppendLine($"static GorgeFramework.AudioAsset^ {MethodName}()", indentation);
            sb.AppendLine("{", indentation);
            sb.AppendLine(
                $"return {InjectorHardcodeGenerator.Generate(AudioInjector, indentation + 1)};",
                indentation + 1);
            sb.AppendLine("}", indentation);
            return sb.ToString();
        }

        public override Period Clone()
        {
            return new AudioPeriod(MethodName, (Injector) ConfigInjector.Clone(), (Injector) AudioInjector.Clone());
        }
    }
}