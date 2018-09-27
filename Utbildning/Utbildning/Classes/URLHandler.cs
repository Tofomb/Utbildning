using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utbildning.Classes
{
    //  Kursledare/Kurser/Kurs/Bokning/Redigera/32-Leffe-Johansson
    public static class URLHandler
    {
        public static bool GetIds(this string url, out List<int> Id)
        {
            Id = new List<int>();
            if (url.Length > 0)
            {
                List<string> Urls = url.Split('-').ToList();
                for(int i = 0; i < Urls.Count; i++)
                {
                    if (int.TryParse(Urls[i], out int data))
                    {
                        Id.Add(data);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return Id.Count > 0;
        }

        public static bool GetIds(this string url, out List<int> Id, int outputs)
        {
            Id = new List<int>();
            if (url.Length > 0)
            {
                List<string> Urls = url.Split('-').ToList();
                for (int i = 0; i < Urls.Count; i++)
                {
                    if (int.TryParse(Urls[i], out int data))
                    {
                        Id.Add(data);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return Id.Count >= outputs;
        }

        public static bool HasIds(this string url)
        {
            if (url != null)
                return int.TryParse(url.Split('-').First(), out int n);
            return false;
        }

        public static bool HasIds(this string url, int outputs)
        {
            List<string> data = url.Split('-').ToList();

            for (int i = 0; i < outputs; i++)
            {
                if (!int.TryParse(data[i], out int n))
                    return false;
            }
            return true;
        }
    }
}