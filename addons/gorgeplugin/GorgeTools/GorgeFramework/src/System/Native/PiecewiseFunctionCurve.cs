using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class PiecewiseFunctionCurve
    {
        public PiecewiseFunctionCurve(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        private static partial ObjectList InjectorFieldDefaultValue_functionPieces()
        {
            return null;
        }

        private static partial ObjectArray InitializeField_functionPieces(ObjectList functionPieces)
        {
            if (functionPieces == null)
            {
                return null;
            }

            var objectList = new List<GorgeObject>();
            for (var i = 0; i < functionPieces.length; i++)
            {
                var functionPiece = (Injector) functionPieces.Get(i);
                if (functionPiece == null)
                {
                    objectList.Add(null);
                }
                else
                {
                    objectList.Add(functionPiece.Instantiate(0));
                }
            }

            return new ObjectArray(objectList.Count,
                new ObjectList(functionPieces.ItemClassType.SubTypes[0], objectList));
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_functionPieces() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "函数分段")
        };

        public override partial float Evaluate(float x)
        {
            if (functionPieces == null)
            {
                return 0;
            }

            for (var i = 0; i < functionPieces.length; i++)
            {
                var functionPiece = FunctionPiece.FromGorgeObject(functionPieces.Get(i));

                if (functionPiece == null)
                {
                    return 0;
                }

                if (((functionPiece.leftClosed && x >= functionPiece.startX) ||
                     (!functionPiece.leftClosed && x > functionPiece.startX)) &&
                    ((functionPiece.rightClosed && x <= functionPiece.endX) ||
                     (!functionPiece.rightClosed && x < functionPiece.endX))
                   )
                {
                    return functionPiece.functionCurve?.Evaluate(x) ?? 0;
                }
            }

            return 0;
        }

        private static partial Annotation[] ClassAnnotations()
        {
            return new[]
            {
                new Annotation("Editable", null, new Dictionary<string, Metadata>()
                {
                    ["displayName"] = new Metadata(GorgeType.String, "displayName", "分段函数"),
                })
            };
        }
    }
}