using System.Collections.Generic;
using System.Linq;

namespace X.Util.Entities
{
    /// <summary>
    /// 定义模块功能，以NameSpace来定义唯一
    /// </summary>
    public class Module
    {
        /// <summary>
        /// Module Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Module NickName
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Module NameSpace
        /// </summary>
        public string NameSpace { get; set; }

        /// <summary>
        /// Module NameSpaces
        /// </summary>
        public List<string> NameSpaces
        {
            get { return string.IsNullOrEmpty(NameSpace) ? new List<string>() : NameSpace.Split('/').ToList(); }
        }

        /// <summary>
        /// Module FullName
        /// </summary>
        public string FullName
        {
            get { return string.Format("{0}.{1}", NameSpace, Id); }
        }
    }
}
