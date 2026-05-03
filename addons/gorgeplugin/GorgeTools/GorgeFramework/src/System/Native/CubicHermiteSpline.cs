using System.Collections.Generic;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.Native.GorgeFramework
{
    public partial class CubicHermiteSpline
    {
        public CubicHermiteSpline(Injector injector) : base(injector)
        {
            FieldInitialize(injector);
        }

        private static partial Vector2 InitializeField_startPoint(Injector startPoint)
        {
            if (startPoint == null)
            {
                return new Vector2(0, 0);
            }

            return Vector2.ConstructInstance(startPoint);
        }

        private static partial float InitializeField_startTangent(float startTangent)
        {
            return startTangent;
        }

        private static partial float InitializeField_startWeight(float startWeight)
        {
            return startWeight;
        }

        private static partial Vector2 InitializeField_endPoint(Injector endPoint)
        {
            if (endPoint == null)
            {
                return new Vector2(1, 1);
            }

            return Vector2.ConstructInstance(endPoint);
        }

        private static partial float InitializeField_endTangent(float endTangent)
        {
            return endTangent;
        }

        private static partial float InitializeField_endWeight(float endWeight)
        {
            return endWeight;
        }

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_startPoint() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "首点")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_startTangent() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "首正切")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_startWeight() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "首权重")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_endPoint() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "尾点")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_endTangent() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "尾正切")
        };

        private static partial Dictionary<string, Metadata> InjectorFieldMetadata_endWeight() => new()
        {
            ["displayName"] = new Metadata(GorgeType.String, "displayName", "尾权重")
        };

        private static partial Injector InjectorFieldDefaultValue_startPoint()
        {
            var injector = Vector2.EmptyInjector();
            injector.x = 0;
            injector.y = 0;
            return injector;
        }

        private static partial float InjectorFieldDefaultValue_startTangent()
        {
            return 0;
        }

        private static partial float InjectorFieldDefaultValue_startWeight()
        {
            return 0.33333f;
        }

        private static partial Injector InjectorFieldDefaultValue_endPoint()
        {
            var injector = Vector2.EmptyInjector();
            injector.x = 1;
            injector.y = 1;
            return injector;
        }

        private static partial float InjectorFieldDefaultValue_endTangent()
        {
            return 0;
        }

        private static partial float InjectorFieldDefaultValue_endWeight()
        {
            return 0.33333f;
        }

        public override partial float Evaluate(float x)
        {
            if (x < startPoint.x)
            {
                return startPoint.y;
            }

            if (x > endPoint.x)
            {
                return endPoint.y;
            }

            // var xRange = endPoint.x - startPoint.x;
            // var yRange = endPoint.y - startPoint.y;
            // var t = (x - startPoint.x) / xRange;
            //
            // var part1 = (1 + 2 * t) * (1 - t) * (1 - t) * startPoint.y;
            // var part2 = t * (1 - t) * (1 - t) * xRange * startTangent;
            // var part3 = t * t * (3 - 2 * t) * endPoint.y;
            // var part4 = t * t * (t - 1) * xRange * endTangent;
            //
            // var result = part1 + part2 * startWeight * 3 + part3 + part4 * endWeight * 3;

            return (float) AnimationCurveInterpolant(startPoint.x, startPoint.y, startTangent,
                Math.Clamp(startWeight, 0, 1), endPoint.x, endPoint.y, endTangent, Math.Clamp(endWeight, 0, 1), x);
        }

        /// <summary>
        /// 来源于https://math.stackexchange.com/questions/3210725/weighting-a-cubic-hermite-spline
        /// weight在[0,1]以内是正确的，超出范围和UnityAnimationCurve表现不一致
        /// 在原代码基础上增加了y1==y2情况下的处理
        /// TODO 暂未考虑x1==x2，根据AnimationCurve实际情况确定一下取哪个值
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="yp1"></param>
        /// <param name="wt1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="yp2"></param>
        /// <param name="wt2"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double AnimationCurveInterpolant(double x1, double y1, double yp1, double wt1, double x2,
            double y2, double yp2, double wt2, double x)
        {
            // 这里原文中是2.22e-16，但是发现一组特殊参数
            // 0 ,0 ,0 ,0.699999988079071 ,10 ,-1.5 ,0 ,0 ,6.162614822387695
            // 此时System.Math.Abs(fg)稍稍大于2 * eps，并且无限循环
            // 因此把阈值提高到2.23e-16
            // 并且补充对死循环的检测
            const int loopLimit = 50;
            const double eps = 2.23e-16;
            double dx = x2 - x1;
            var xdx = (x - x1) / dx;
            double dy = y2 - y1;
            var yp1dy = yp1 * dx / dy;
            var yp2dy = yp2 * dx / dy;
            double wt2s = 1 - wt2;

            double t = 0.5;
            double t2;

            if (wt1 == 1 / 3.0 && wt2 == 1 / 3.0)
            {
                t = xdx;
                t2 = 1 - t;
            }
            else
            {
                var count = 0;
                while (true)
                {
                    t2 = (1 - t);
                    double fg = 3 * t2 * t2 * t * wt1 + 3 * t2 * t * t * wt2s + t * t * t - xdx;
                    if (System.Math.Abs(fg) < 2 * eps)
                        break;

                    // third order householder method
                    double fpg = 3 * t2 * t2 * wt1 + 6 * t2 * t * (wt2s - wt1) + 3 * t * t * (1 - wt2s);
                    double fppg = 6 * t2 * (wt2s - 2 * wt1) + 6 * t * (1 - 2 * wt2s + wt1);
                    double fpppg = 18 * wt1 - 18 * wt2s + 6;

                    t -= (6 * fg * fpg * fpg - 3 * fg * fg * fppg) /
                         (6 * fpg * fpg * fpg - 6 * fg * fpg * fppg + fg * fg * fpppg);
                    count++;
                    if (count > loopLimit)
                    {
                        break;
                    }
                }
            }

            if (System.Math.Abs(dy) < 2 * eps)
            {
                double y = 3 * t2 * t2 * t * wt1 * yp1 * dx + 3 * t2 * t * t * wt2 * yp2 * dx;
                return y + y1;
            }
            else
            {
                double y = 3 * t2 * t2 * t * wt1 * yp1dy + 3 * t2 * t * t * (1 - wt2 * yp2dy) + t * t * t;
                return y * dy + y1;
            }
        }

        private static partial Annotation[] ClassAnnotations() => new[]
        {
            new Annotation("Editable", null, new Dictionary<string, Metadata>()
            {
                ["displayName"] = new Metadata(GorgeType.String, "displayName", "加权三次埃尔米特曲线"),
            })
        };
    }
}