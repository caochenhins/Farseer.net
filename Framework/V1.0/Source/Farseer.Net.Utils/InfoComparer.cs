using System;
using System.Collections.Generic;

namespace FS.Utils.Common
{
    /// <summary>
    ///     对象比较的实现
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class InfoComparer<TInfo, T> : IEqualityComparer<TInfo> where TInfo : class
    {
        private readonly Func<TInfo, T> _keySelect;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="keySelect"></param>
        public InfoComparer(Func<TInfo, T> keySelect)
        {
            _keySelect = keySelect;
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(TInfo x, TInfo y)
        {
            return EqualityComparer<T>.Default.Equals(_keySelect(x), _keySelect(y));
        }

        /// <summary>
        /// HashCode
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(TInfo obj)
        {
            return EqualityComparer<T>.Default.GetHashCode(_keySelect(obj));
        }
    }
}