using System.Collections.Generic;

namespace Funnel
{
    public class Configuration
    {
        /// <summary>
        /// 输入文件列表
        /// </summary>
        /// <value></value>
        public List<string> Inputs{get;set;}

        /// <summary>
        /// 文件编码格式
        /// </summary>
        /// <value></value>
        public string Encoding{get;set;}

        /// <summary>
        /// 每秒读取文本行数的上限
        /// </summary>
        /// <value></value>
        public int Limitation{get;set;}

        /// <summary>
        /// 是否匹配多行
        /// </summary>
        /// <value></value>
        public bool Multiline{get;set;}

        /// <summary>
        /// 每一行的开头，仅开启多行匹配时生效
        /// </summary>
        /// <value></value>
        public string LineHead{get;set;}

        /// <summary>
        /// 不包含列表中的文本串的记录将被丢弃，支持正则表达式
        /// </summary>
        /// <value></value>
        public List<string> Keywords{get;set;}

        /// <summary>
        /// 输出方式，现支持 console 和 file
        /// </summary>
        /// <value></value>
        public List<string> Outputs{get;set;}
    }
}