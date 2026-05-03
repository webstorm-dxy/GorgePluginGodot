using System.Collections.Generic;

namespace Gorge.GorgeFramework.Utilities
{
    public static class StackExtension
    {
        /// <summary>
        /// 获取栈顶
        /// </summary>
        /// <param name="stack"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>栈顶元素，如果无栈顶则为default</returns>
        public static T Top<T>(this Stack<T> stack) => stack.Count > 0 ? stack.Peek() : default;
    }
}