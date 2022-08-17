using System;
using System.Collections.Generic;
using System.Text;

namespace AdminApp
{
    public class Constants
    {
        public static string BaseUrl
        {
            get { return @"https://tryincdecazurefunction20220420002219.azurewebsites.net"; }
        }
        private static Admin _empty = null;
        public static Admin EmptyAuth
        {
            get
            {
                if (_empty == null)
                {
                    _empty = new Admin();
                    _empty.password = "";
                    _empty.username = "";
                }
                return _empty;
            }
        }

        public static Admin CurAdmin { get; set; }
    }
}
