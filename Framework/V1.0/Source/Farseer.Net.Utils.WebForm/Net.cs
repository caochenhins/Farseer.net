using System;
using System.IO;
using System.Text;
using System.Web;

namespace FS.Utils.WebForm
{
    /// <summary>
    ///     下载文件
    /// </summary>
    public abstract class Net : FS.Utils.Net
    {

        /// <summary>
        /// 下载文件到客户端
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public static void DownFile(string filePath, string fileName)
        {
            var Response = HttpContext.Current.Response;

            //指定块大小   
            long chunkSize = 102400;
            //建立一个100K的缓冲区   
            var buffer = new byte[chunkSize];
            //已读的字节数   
            long dataToRead = 0;
            FileStream stream = null;
            try
            {
                //打开文件   
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                dataToRead = stream.Length;
                //添加Http头   
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachement;filename=" + fileName);
                Response.AddHeader("Content-Length", dataToRead.ToString());
                while (dataToRead > 0)
                {
                    if (Response.IsClientConnected)
                    {
                        var length = stream.Read(buffer, 0, Convert.ToInt32(chunkSize));
                        Response.OutputStream.Write(buffer, 0, length);
                        Response.Flush();
                        buffer = new Byte[10000];
                        dataToRead -= length;
                    }
                    else
                    {
                        //防止client失去连接  
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex) { Response.Write("Error:" + ex.Message); }
            finally
            { if (stream != null) { stream.Close(); } Response.Close(); }
        }

        /// <summary>
        /// 下载文件到客户端
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public static void DownStream(string s, string fileName, string contentType = "application/octet-stream")
        {
            var response = HttpContext.Current.Response;

            response.ContentEncoding = Encoding.UTF8;
            response.AppendHeader("content-disposition", "attachment;filename=\"" + HttpUtility.UrlEncode(fileName, Encoding.UTF8) + "\"");
            response.ContentType = contentType;

            response.Write(s);
            response.End();
        }

    }
}