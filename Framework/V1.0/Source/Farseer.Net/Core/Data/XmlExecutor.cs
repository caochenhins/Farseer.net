﻿using System.IO;
using System.Xml.Linq;

namespace FS.Core.Data
{
    internal class XmlExecutor
    {
        protected internal string FilePath { get; set; }
        protected internal string XmlName { get; set; }

        /// <summary>
        ///     读取XML文档(直接读取xml文档)
        ///     路径不存，或者返回Null时，会自动创建xml
        /// </summary>
        protected internal XElement Load()
        {
            var element = !File.Exists(FilePath) ? Create() : XElement.Load(FilePath);
            return element;
        }

        /// <summary>
        ///     创建XML文件
        /// </summary>
        protected internal XElement Create()
        {
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(XmlName));

            Directory.CreateDirectory(FilePath);
            doc.Save(FilePath);
            return doc.Element(XmlName);
        }
    }
}