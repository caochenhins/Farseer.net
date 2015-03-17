using System;
using System.IO;
using System.Web;

namespace FS.Utils.WebForm
{
    /// <summary>
    ///     文件工具
    /// </summary>
    public abstract class Files : Common.Files
    {
        /// <summary>
        ///     获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        public static string GetMapPath(string strPath)
        {
            strPath = ConvertPath(strPath);
            try
            {
                return ConvertPath(HttpContext.Current != null ? HttpContext.Current.Server.MapPath(strPath) : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath));
            }
            catch
            {
                return ConvertPath(strPath);
            }
        }

        /// <summary>
        ///     以指定的ContentType输出指定文件文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">输出的文件名</param>
        /// <param name="fileType">将文件输出时设置的ContentType</param>
        public static void ResponseFile(string filePath, string fileName, string fileType)
        {
            Stream iStream = null;
            // 缓冲区为10k
            var buffer = new Byte[10000];
            // 文件长度
            // 需要读的数据长度

            try
            {
                // 打开文件
                iStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // 需要读的数据长度
                var dataToRead = iStream.Length;

                HttpContext.Current.Response.ContentType = fileType;
                HttpContext.Current.Response.AddHeader("Content-Disposition",
                                                       "attachment;filename=" +
                                                       Url.UrlEncode(fileName.Trim()).Replace("+", " "));

                while (dataToRead > 0)
                {
                    // 检查客户端是否还处于连接状态
                    if (HttpContext.Current.Response.IsClientConnected)
                    {
                        var length = iStream.Read(buffer, 0, 10000);
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        HttpContext.Current.Response.Flush();
                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        // 如果不再连接则跳出死循环
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    // 关闭文件
                    iStream.Close();
                }
            }
            HttpContext.Current.Response.End();
        }
    }
}