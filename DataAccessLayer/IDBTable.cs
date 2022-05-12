using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace DataAccessLayer
{
    public interface IDBTable
    {
        string GetCreate();
        string GetDelete();
        string GetInsert();
        string GetUpdate();
        string GetRead();
        void SetData(DbDataReader reader);
    }
}
