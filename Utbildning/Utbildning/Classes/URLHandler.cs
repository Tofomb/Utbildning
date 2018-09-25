using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utbildning.Classes
{
    public static class URLHandler
    {
        public static bool GetId(this string url, out int Id)
        {
            return int.TryParse(url.Split('-').First(), out Id);
        }
    }
}