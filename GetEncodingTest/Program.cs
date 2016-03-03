using System;
using System.IO;
using System.Text;
using System.Net;
using Mozilla.NUniversalCharDet;

namespace GetEncodingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //文件路径
            var path = $@"{AppContext.BaseDirectory}\Encoding";

            //目录
            var direct = new DirectoryInfo(path);

            foreach (var file in direct.GetFiles("*.*"))
            {
                //获取编码
                using (var fs = file.OpenRead())
                {
                    //获取编码
                    var encoding = GetEncoding(fs);

                    //获取文本内容
                    //var txt = new StreamReader(fs, encoding).ReadToEnd();

                    Console.WriteLine($"{file.Name}\t\t{encoding.EncodingName}");
                }
            }

            Console.ReadKey();
        }

        //static void Main(string[] args)
        //{
        //    //URL地址
        //    var urls = new string[] { "http://lfei.org/", "http://www.w3school.com.cn/" };

        //    foreach (var url in urls)
        //    {
        //        var request = (HttpWebRequest)WebRequest.Create(url);
        //        request.Method = "GET";
        //        using (var stream = request.GetResponse().GetResponseStream())
        //        {
        //            //读取响应到内存流
        //            var ms = new MemoryStream();
        //            var length = 0;
        //            var buffer = new byte[1024];
        //            while ((length = stream.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                ms.Write(buffer, 0, length);
        //            }

        //            //获取编码
        //            var encoding = GetEncoding(ms);

        //            //获取 HTML
        //            //var html = encoding.GetString(ms.GetBuffer());

        //            Console.WriteLine($"{url}\t\t{encoding.EncodingName}");
        //        }
        //    }

        //    Console.ReadKey();
        //}

        /// <summary>
        /// 获取字节流编码
        /// </summary>
        /// <param name="stream">字节流</param>
        /// <returns></returns>
        public static Encoding GetEncoding(Stream stream)
        {
            if (stream != null && stream.Length > 0)
            {
                //每次分配1024字节，进行编码判断
                var buffer = new byte[1024];

                var seek = stream.Position;
                stream.Seek(0, SeekOrigin.Begin);

                var ud = new UniversalDetector(null);
                while (!ud.IsDone() && stream.Read(buffer, 0, buffer.Length) > 0)
                {
                    ud.HandleData(buffer, 0, buffer.Length);
                }
                ud.DataEnd();

                stream.Seek(seek, SeekOrigin.Begin);

                var encoding = ud.GetDetectedCharset();
                if (encoding != null)
                {
                    if (encoding == Constants.CHARSET_X_ISO_10646_UCS_4_2143 || encoding == Constants.CHARSET_X_ISO_10646_UCS_4_3412)
                    {
                        encoding = "UTF-32";
                    }

                    return Encoding.GetEncoding(encoding);
                }
            }

            return Encoding.Default;
        }
    }
}