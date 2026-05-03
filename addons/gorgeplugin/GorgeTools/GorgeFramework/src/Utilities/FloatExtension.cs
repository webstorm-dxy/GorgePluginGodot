using System;

namespace Gorge.GorgeFramework.Utilities
{
    public static class FloatExtension
    {
        public static int BitInt(this float f)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(f), 0);
        }
    }
}