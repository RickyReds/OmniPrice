using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Omnitech.Database.NAV
{
    public static class NavDAL
    {
        public static List<Omnitech.DTO.NAV.Sessione> ListaSessioni()
        {
            string connStr = $"Data Source=10.0.0.6;Initial Catalog=AvanzamentoProduzione;Integrated Security=SSPI;";



            List<Omnitech.DTO.NAV.Sessione > ss = new List<Omnitech.DTO.NAV.Sessione>();

            using (IDbConnection db = new SqlConnection(connStr))
            {
                string sql = $@" 
                                    SELECT TOP (1000) --[timestamp]
                                          --,[Server Instance ID],
                                          [Session ID] as SessionID
                                          --,[User SID]
                                          ,[Server Instance Name] as ServerInstanceName
                                          --,[Server Computer Name]
                                          ,[User ID] as UserID
	                                      ,CASE
			                                    WHEN [User ID] = 'OMNITECHGROUP\UTENTE01' THEN 'Elena'
			                                    WHEN [User ID] = 'OMNITECHGROUP\UTENTE02' THEN 'Nicola'
			                                    WHEN [User ID] = 'OMNITECHGROUP\UTENTE11' THEN 'Valentina'
			                                    WHEN [User ID] = 'OMNITECHGROUP\UTENTE17' THEN 'Mauro'
			                                    WHEN [User ID] = 'OMNITECHGROUP\UTENTE27' THEN 'Stefania'
			                                    WHEN [User ID] = 'OMNITECHGROUP\UTENTE30' THEN 'Luca'
                                                WHEN [User ID] = 'OMNITECHGROUP\LINDA.A'  THEN 'Linda'
			                                    WHEN [User ID] = 'OMNITECHGROUP\UESLI.G'  THEN 'Uesli'
			                                    --WHEN [User ID] = 'OMNITECHGROUP\SALMEDIN.E' THEN 'Salmedin'
			                                    WHEN [Server Instance Name] = 'dynamicsnav110_web' THEN 'Web Service (Pistole)'
			                                    WHEN [Server Instance Name] = 'dynamicsnav110' AND [User ID] = 'OMNITECHGROUP\ADMINISTRATOR' THEN '** Admin **' 
			                                    ELSE ''
	                                       END as Alias
                                          --,[Client Type]
                                          ,[Client Computer Name] as ClientComputerName
                                          ,[Login Datetime] as LoginDateTime
                                          --,[Database Name]
                                          --,[Session Unique ID]
                                      FROM [192.168.100.4\SQLEXPRESS].[Omnitech].[dbo].[Active Session]
                            
                ";

                ss = db.Query<Omnitech.DTO.NAV.Sessione>(sql).ToList();

            }

            return ss;
        }

        public static bool KillSessione(int SessionID)
        {
            string connStr = $"Data Source=10.0.0.6;Initial Catalog=AvanzamentoProduzione;Integrated Security=SSPI;";


            try
            {
                #region 

                using (IDbConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    //using (IDbTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {

                            string query = $@"delete from [192.168.100.4\SQLEXPRESS].[Omnitech].[dbo].[Active Session] where [Session ID] = {SessionID}";

                            int rows = conn.Execute(query);     //, null, trans);

                            if (rows != 1)
                                throw new Exception("Sessioni multiple!");

                            //trans.Commit();

                            return true;

                        }
                        catch (Exception ex)
                        {
                            //trans.Rollback();
                            return false;
                        }
                    }

                }

                #endregion
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
