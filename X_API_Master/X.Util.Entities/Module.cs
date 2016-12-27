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
        /// 注册作用域
        /// </summary>
        public Scope Scope { get; set; }

        /// <summary>
        /// Module FullName
        /// </summary>
        public string FullName
        {
            get { return string.Format("{0}.{1}", Scope.Path, Id); }
        }
    }

    /// <summary>
    /// 定义作用域(命名空间方式以‘/’间隔)
    /// </summary>
    public class Scope
    {
        public Scope(string path)
        {
            if (path.StartsWith("/"))
            {
                Path = path;
            }
            else
            {
                Path = "/" + path;
            }
            NameSpaces = Path.Substring(1).Split('/').ToList();
        }

        /// <summary>
        /// Scope Path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Scope NameSpaces
        /// </summary>
        public List<string> NameSpaces { get; set; }
    }
}
