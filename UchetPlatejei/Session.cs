using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UchetPlatejei
{
    public static class Session
    {
        public static int userId;
        public static DateTime dateAuth;

        public static DateTime dateExit;

        public static int countAdd, countUpdate, countDelete, count = 0;

        public static int CountAdd { get => countAdd; set => countAdd = value; }
        public static int CountUpdate { get => countUpdate; set => countUpdate = value; }
        public static int CountDelete { get => countDelete; set => countDelete = value; }
        public static int Count { get => count; set => count = value; }


        public static DateTime DateAuth { get => dateAuth; set => dateAuth = value; }

        public static DateTime DateExit { get => dateExit; set => dateExit = value; }
    }
}
