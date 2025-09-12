using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using static Omnitech.Database.Tools.ProductionMonitor.DTO;
using HI.Libs.Utility;

namespace Omnitech.Database.Tools.ProductionMonitor
{
    public class BLL
    {
        public static async Task<Result<List<Postazione>>> ListOdP(string ConnectionString)
        {
			Result<List<Postazione>> r = new Result<List<Postazione>>("ListOdP()");

            List<Postazione> odps = new List<Postazione>();

            try
            {
                #region 

                using (IDbConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

					//2025.01.13:  Aggiunto il totale degli ordini chiusi/completi per l'operatore nella postazione.

                    string query = $@"
											DECLARE @NOW AS DATETIME = GETDATE();
											DECLARE @NOWDATE AS DATE = CAST(@NOW as date);

											WITH
											CTE AS
											(
												SELECT	abc.ov_numord,
														sum(DATEDIFF(SECOND, abc.ov_dataorastart, abc.dataorastop)) as Timpiegato			--- CONVERTITO IN SECONDI
												FROM
												(
													SELECT	atemp.ov_numord,
															atemp.ov_dataorastart,
															CASE 
																WHEN atemp.ov_dataorastop IS NULL THEN @now
																ELSE atemp.ov_dataorastop
															END dataorastop
													FROM [SRVMES].[OMNITECH].[dbo].[ov_shopfloorctl] atemp
		
													WHERE	atemp.ov_fase = 120          --LEVIGATURA
														and atemp.ov_idOP <> 1111    --BISOGNA TOGLIERE L'AUTOMATICO   
												)abc
												group by abc.ov_numord
											),
											cte2 as
											(
												SELECT	lav.ov_codcentro CodiceCentroLavoro,
														lav.ov_idOP as CodiceOperatore,
														anaOpe.tb_descope AS Operatore,
														RIGHT('000000'+CAST(lav.ov_numord AS NVARCHAR(6)),6) as OdP,		--Padda con Zeri         --lav.ov_numord as OdP,
														cte.Timpiegato,
														atemp.[TLevigaturaAssegnato]*60 as Tassegnato,											--- CONVERTITO IN SECONDI
														atemp.Affidabilita
													  --,atemp.[TLevigaturaAssegnato]*60 - cte.Timpiegato AS dT									--- CONVERTITO IN SECONDI
												FROM [SRVMES].[OMNITECH].[dbo].[ov_shopfloorctl] lav
												LEFT JOIN [SRVMES].[OMNITECH].[dbo].[tabcope] anaOpe ON anaOpe.tb_codcope = lav.ov_idOP		--se non serve 'Operatore' (il nome), non serve nemmeno questo join
												LEFT JOIN cte on cte.ov_numord = lav.ov_numord
												LEFT JOIN [AvanzamentoProduzione].[schedulatore].[TempiLevigatura] atemp on atemp.barcode = lav.ov_numord
												WHERE 	lav.ov_dataorastop IS NULL 
													and	lav.ov_fase = 120      --LEVIGATURA
													and lav.ov_idOP <> 1111    --BISOGNA TOGLIERE L'AUTOMATICO

													and lav.ov_codcentro >= 380 and lav.ov_codcentro <= 530
												  --and lav.ov_codcentro = 380
												  --and lav.ov_codcentro >= 380 and lav.ov_codcentro <= 400

												--order by lav.ov_codcentro			--ORDER BY dt
											)
											select cte2.CodiceCentroLavoro,
												   cte2.CodiceOperatore,
												   cte2.Operatore,
												   cte2.OdP,
												   cte2.Timpiegato,
												   cte2.Tassegnato,
												   cte2.Affidabilita,
											       isnull(chiusi.CompletatiOperatoreToday,0) CompletatiOperatoreToday
											from cte2
											left join
											(
												select ov_idOP, count(distinct ov_numord) CompletatiOperatoreToday        
												FROM [SRVMES].[OMNITECH].[dbo].[ov_shopfloorctl]
												where ov_evaso = 'S' 
												  and ov_dataorastart > @NOWDATE 
												  and ov_dataorastop is not null
												  and ov_fase = 120      --LEVIGATURA
												  and ov_idOP <> 1111    --BISOGNA TOGLIERE L'AUTOMATICO
												  and ov_codcentro >= 380 and ov_codcentro <= 530

												group by ov_idOP
											
											) chiusi on  cte2.CodiceOperatore = chiusi.ov_idOP
											
											order by CodiceCentroLavoro			--ORDER BY dt
";



                    odps = (await conn.QueryAsync<Postazione>(query)).ToList();       //NOTA BENE: Se la query ritorna Tassegnato = null, il valore del DTO Tassegnato sarà 0.  Questo caso si può verificare con articoli "nuovi" dove non c'è ancora un tempo stimato/assegnato.

#if DEBUG
					//Genera un conflitto

					//Postazione c1 = new Postazione() { CodiceCentroLavoro = 530, Operatore = "PIERO STOPPANI", CodiceOperatore = 100 };
					//Postazione c2 = new Postazione() { CodiceCentroLavoro = 530, Operatore = "TONY CAPUOZZO", CodiceOperatore = 88 };
					//Postazione c3 = new Postazione() { CodiceCentroLavoro = 390, Operatore = "ELONIO MUSKIO", CodiceOperatore = 903 };
					//Postazione c4 = new Postazione() { CodiceCentroLavoro = 390, Operatore = "DONALDO TRUMPO", CodiceOperatore = 555 };
					//odps.Add(c1);
					//odps.Add(c2);
					//odps.Add(c3);
					//odps.Add(c4);
#endif


					r.ReturnObject = odps;
					r.IsOk = true;
                }

                #endregion
            }
            catch (Exception ex)
            {
                odps = null;

				r.Exception = ex;
            }

            return r;

        }
    }
}
