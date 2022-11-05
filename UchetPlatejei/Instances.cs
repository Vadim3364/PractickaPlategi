using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UchetPlatejei
{
    internal class Instances
    {
        private static db_payEntities _db = null;

        public static db_payEntities db
        {
            get
            {
                if (_db == null)
                    _db = new db_payEntities();
                return _db;
            }
        }
    }
}
