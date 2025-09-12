
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Omnitech.Database.Tools.ContenutoCarrello.DTO;
using Dapper;

namespace Omnitech.Database.Tools.ContenutoCarrello
{
    public class BLL
    {
        public static List<Item> ListICartItems(string ConnectionString, string CartName)
        {
            List<Item> items = null;

            try
            {
                #region 

                using (IDbConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = $@"execute warehouse.ContenutoCarrello @cartName";

                    items = conn.Query<Item>(query, new { cartName = CartName }).ToList();
                }

                #endregion
            }
            catch (Exception ex)
            {
                items = null;
            }

            return items;
        }
    }
}
