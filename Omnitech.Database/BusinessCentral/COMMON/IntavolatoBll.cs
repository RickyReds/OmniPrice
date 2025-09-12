using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dto = Omnitech.DTO.BusinessCentral.COMMON;
using Dapper;

namespace Omnitech.Database.BusinessCentral.COMMON
{
    public class Intavolato
    {
        public static List<dto.Intavolato> Load(string ConnectionString, int IdReport, string CustomerNo, string LanguageIsoCode = null)
        {
            List<dto.Intavolato> l = null;



            #region 

            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                //                string query = $@"
                //                                    WITH cte AS 
                //                                    (
                //	                                    SELECT  DENSE_RANK() OVER (PARTITION BY AreaName ORDER BY CustomerNo DESC) AS Gruppo,
                //												*

                //	                                    FROM dbo.ReportLayout
                //	                                    WHERE IdReport = @IdReport 
                //	                                      AND (CustomerNo is null OR CustomerNo = @CustomerNo)
                //                                    )
                //                                    SELECT *
                //                                    FROM cte
                //									WHERE Gruppo = 1
                //                                    AND [Skip] = 0
                //";

                string query = $@"
                                    WITH cte AS 
                                    (
	                                    SELECT  DENSE_RANK() OVER (PARTITION BY AreaName ORDER BY CustomerNo DESC) AS Gruppo,
												*

	                                    FROM dbo.ReportLayout
	                                    WHERE IdReport = @IdReport 
	                                      AND (CustomerNo is null OR CustomerNo = @CustomerNo)
                                    )
									, cte2 AS
									(
	                                    SELECT  DENSE_RANK() OVER (PARTITION BY AreaName ORDER BY LanguageIsoCode DESC) AS Gruppo2,
												*
	                                    FROM cte
										WHERE Gruppo = 1
										AND [Skip] = 0
	                                    AND (LanguageIsoCode is null or LanguageIsoCode = @LanguageIsoCode)
									)
                                    SELECT *
                                    FROM cte2
									WHERE Gruppo2 = 1
                                    AND [Skip] = 0
";

                l = conn.Query<dto.Intavolato>(query, new { IdReport, CustomerNo, LanguageIsoCode }, null, true, null, null).ToList();

            }

            #endregion

            return l;

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
    }
}
