using System;

namespace FS.Mapping.Table
{
    /// <summary>
    /// 设置存储过程参数
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ProcAttribute : Attribute
    {
        /// <summary>
        /// 设置存储过程参数
        /// </summary>
        public ProcAttribute()
        {
            IsInParam = true;
        }

        /// <summary>
        /// 指示字段是否为存储过程中输出的参数
        /// （默认为false)
        /// </summary>
        public bool IsOutParam { get; set; }

        /// <summary>
        /// 指示字段是否为存储过程中输入的参数
        /// （默认为false)
        /// </summary>
        public bool IsInParam { get; set; }
    }
}
