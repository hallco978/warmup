using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace warmup.infrastructure
{
    public static class Verifier
    {


        public static string TestPath(string path)
        {
            return new Uri(path).IsFile
                ? Directory.Exists(path)
                    ? path
                    : AdjustedPath(path)
                : isValid(path)
                    ? path
                    : AdjustedPath(path);
        }

        public static string AdjustedPath(string path)
        {
            return path.EndsWith(".git")
                ? path
                : path + ".git";
        }

        public static bool isValid(string url)
        {
            try
            {
                var urlReq = (HttpWebRequest)WebRequest.Create(url);
                var urlRes = (HttpWebResponse)urlReq.GetResponse();
                var sStream = urlRes.GetResponseStream();

                string read = new StreamReader(sStream).ReadToEnd();
                return true;

            }
            catch (Exception)
            {
                //Url not valid
                return false;
            }

        }
    }
}
