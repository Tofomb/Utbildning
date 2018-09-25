using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Utbildning.Models;

namespace Utbildning.Classes
{
    public static class UserHandler
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static string GetUserName(IPrincipal User)
        {
            string UserId = User.Identity.GetUserId();
            return db.Users.ToList().Where(x => x.Id == UserId).First().FullName;
        }

        public static string GetFullName(this IPrincipal User)
        {
            string UserId = User.Identity.GetUserId();
            return db.Users.ToList().Where(x => x.Id == UserId).First().FullName;
        }

        public static void SetFullName(this IPrincipal User, string FullName)
        {
            string UserId = User.Identity.GetUserId();
            db.Users.ToList().Where(x => x.Id == UserId).First().FullName = FullName;
            db.SaveChanges();
        }

        public static string GeneratePasswordString()
        {            
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[8];
            Random rnd = new Random();
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[rnd.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}