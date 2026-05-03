using System.Collections.Generic;
using System.Linq;
using Gorge.GorgeLanguage.Objective;

namespace Gorge.GorgeFramework.Runtime
{
    public class NativeImplementationBase : IImplementationBase
    {
        public NativeImplementationBase(IEnumerable<GorgeClass> classes, IEnumerable<GorgeInterface> interfaces,
            IEnumerable<GorgeEnum> enums)
        {
            Interfaces = interfaces;
            Enums = enums;
            Classes = classes;
            ClassDeclarations = Classes.Select(c=>c.Declaration);
        }

        public IEnumerable<ClassDeclaration> ClassDeclarations { get; }
        public IEnumerable<GorgeInterface> Interfaces { get; }
        public IEnumerable<GorgeEnum> Enums { get; }
        public IEnumerable<GorgeClass> Classes { get; }
    }
}