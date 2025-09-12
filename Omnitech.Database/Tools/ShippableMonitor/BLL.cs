using HI.Libs.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using static Omnitech.Database.Tools.ShippableMonitor.DTO;
using HI.Libs.Utility;
using System.Net.NetworkInformation;


namespace Omnitech.Database.Tools.ShippableMonitor
{
    public class BLL
    {
        public static async Task<Result<List<Order>>> ListShippableItems(string ConnectionString)
        {
            Result<List<Order>> r = new Result<List<Order>>("ShippableMonitor.ListShippableItems()");

            List<Order> oo = new List<Order>();

            try
            {
                #region 

                using (IDbConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = $@"
                                        DECLARE @today as date = getdate()

                                        SELECT 
                                              oo.[BarCode]
	                                          ,ac.RagioneSociale
	                                          ,ast.Stampo
	                                          , aca.CategoriaArticolo

	                                          --,[DataInserimento]

                                              ,[DataOrdine]
                                              ,[DataConsegna] DataConsegnaRichiesta
      
	                                          ,[dataConsegnaDefinitiva]	DataConsegnaDefinitiva	-- <--- usare questa PER LA PREVISONE di ciò che NON HO A MAGAZZINO
      
	                                          --,[DataConsegnaCalcolata]				<------ probabilmente errata
	  
	                                          ,wpf.WarehouseLocation LocazioneDiMagazzino
	                                          ,cast (wpf.dateIn as date) DataVersamentoAMagazzino
	                                          ,acc.idGiornoCarico
	  
	                                          --,ef.I
	                                          --,ef.B
	  
	                                          ,[produzione].[GetPrimaDataConsegnaPossibile](oo.BarCode, default) PrimaDataSpedizionePossibile		--> SERVE UNA SECONDA CHIAMATA --> fare un case per ciò che HO o che NON HO A MAGAZZINO!!!!!

                                          FROM [AvanzamentoProduzione].[ordini].[Ordini] oo
                                          left join anagrafica.Clienti ac on oo.idCliente = ac.idCliente
                                          left join anagrafica.Stampi ast on oo.idStampo = ast.idStampo
                                          left join ( select distinct idCategoriaArticolo, CategoriaArticolo from anagrafica.CategorieArticoli ) aca on oo.idLeanType = aca.idCategoriaArticolo
                                          left join warehouse.PF wpf on oo.BarCode = wpf.BarCode                       --and wpf.dateIn is not null and wpf.dateOut is null
                                          left join anagrafica.ClientiConsegne acc on oo.idCliente = acc.idCliente
                                          left join warehouse.PF_PickedList pl on oo.BarCode = pl.BarCode
	                                        OUTER APPLY
	                                        (
		                                        SELECT * FROM dbo.getWSDtempTable(oo.BarCode)          --quella che usava salmedin
	                                        ) ef


                                          where (Magazzino = 0 OR (Magazzino = 1 and oo.idCliente <> 86))										--Escluso OMNITECH
                                            and oo.DataConsegna > '20250101'
	                                        and SUBSTRING(oo.BarCode,1,1) <> '9'																--Esclusi barcode che iniziano con '9'
	                                        and isnull(oo.[dataConsegnaDefinitiva], DATEADD(mm, 1, @today)) <= DATEADD(mm, 1, @today)			--con [dataConsegnaDefinitiva] <= 1 mese in avanti
                                            and wpf.dateIn is not null and wpf.dateOut is null
	                                        and oo.Chiuso = 0
	                                        and ef.I is not null and ef.B is null
	                                        and pl.[Nr. lista prelievo] is null																--non inseriti in una lista di prelievo

                                           order by PrimaDataSpedizionePossibile, RagioneSociale
";


                    oo = (await conn.QueryAsync<Order>(query)).ToList();       

                    r.ReturnObject = oo;
                    r.IsOk = true;
                }

                #endregion
            }
            catch (Exception ex)
            {
                oo = null;

                r.Exception = ex;
            }

            return r;

        }


        public static async Task<Result<int>> GetIdCustomer(string ConnectionString, string RagioneSociale)
        {
            Result<int> r = new Result<int>("GetIdCustomer()");

            int id = 0;

            // id > 0  --->  IdCustomer
            // id = 0  --->  Non trovato
            // id = -1 --->  Multipli
            // id = -2 --->  Error


            try
            {
                #region 

                using (IDbConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = $@"  select idCliente from anagrafica.Clienti where RagioneSociale = @RagioneSociale
";


                    id = await conn.QuerySingleAsync<int>(query, new { RagioneSociale });

                    r.ReturnObject = id;
                    r.IsOk = true;
                }

                #endregion
            }
            catch (Exception ex)
            {
                r.Exception = ex;
            }

            return r;
        }


        public static async Task<Result<List<Order>>> ListShippableItems(string ConnectionString, int IdCliente, DateTime DataSpedizioneFrom, DateTime DataSpedizioneTo)
        {
            Result<List<Order>> r = new Result<List<Order>>("ShippableMonitor.ListShippableItems(Id,Date,Date)");

            List<Order> oo = new List<Order>();

            try
            {
                #region 

                using (IDbConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = $@"
                                            DECLARE @today as date = getdate()

                                            ;
                                            with cte as
                                            (

                                            -- 1) Pezzi che ho a magazzino 'oggi'
                                            
                                            SELECT oo.[BarCode], oo.idCliente, oo.idStampo, oo.idLeanType

                                                  ,[DataOrdine]
                                                  ,[DataConsegna] DataConsegnaRichiesta
	                                              ,[dataConsegnaDefinitiva]		-- <--- usare questa PER LA PREVISONE di ciò che NON HO A MAGAZZINO
	  
	                                              ,wpf.WarehouseLocation LocazioneDiMagazzino
	                                              ,cast (wpf.dateIn as date) DataVersamentoAMagazzino
	                                              --,wpf.dateOut
	                                              --,pl.[Nr. lista prelievo]
	                                              ,acc.idGiornoCarico

                                              FROM [AvanzamentoProduzione].[ordini].[Ordini] oo
                                              left join warehouse.PF wpf on oo.BarCode = wpf.BarCode
                                              left join anagrafica.ClientiConsegne acc on oo.idCliente = acc.idCliente
                                              left join warehouse.PF_PickedList pl on oo.BarCode = pl.BarCode
	                                            OUTER APPLY
	                                            (
		                                            SELECT * FROM dbo.getWSDtempTable(oo.BarCode)								--quella che usava salmedin
	                                            ) ef

                                              where 
	                                              (Magazzino = 0 OR (Magazzino = 1 and oo.idCliente <> 86))										--Escluso OMNITECH
                                              and oo.idCliente = @IdCliente
                                              and oo.DataConsegna > '20250101'
                                              and wpf.dateIn is not null and wpf.dateOut is null												--che sono a magazzino (ORA)
                                              and oo.Chiuso = 0
                                              and ef.I is not null and ef.B is null
                                              and pl.[Nr. lista prelievo] is null																--non inseriti in una lista di prelievo
                                              and isnull(oo.[dataConsegnaDefinitiva], DATEADD(mm, 2, @today)) < DATEADD(mm, 3, @today)			--con [dataConsegnaDefinitiva] < 3 mesi   (se nulla è come se fosse a 2 mesi)


                                            union
                                            
                                            -- 2) Pezzi (che posso non avere 'oggi' a magazzino) per i quali ho previsto la consegna in  [dataConsegnaDefinitiva] (o [DataConsegna] qualora la definitiva non sia ancora disponibile)

                                            SELECT oo.[BarCode], oo.idCliente, oo.idStampo, oo.idLeanType

                                                  ,[DataOrdine]
                                                  ,[DataConsegna] DataConsegnaRichiesta
	                                              ,[dataConsegnaDefinitiva]		-- <--- usare questa PER LA PREVISONE di ciò che NON HO A MAGAZZINO
	  
	                                              ,wpf.WarehouseLocation LocazioneDiMagazzino
	                                              ,cast (wpf.dateIn as date) DataVersamentoAMagazzino
	                                              --,wpf.dateOut
	                                              --,pl.[Nr. lista prelievo]
	                                              ,acc.idGiornoCarico

                                              FROM [AvanzamentoProduzione].[ordini].[Ordini] oo
                                              left join warehouse.PF wpf on oo.BarCode = wpf.BarCode
                                              left join anagrafica.ClientiConsegne acc on oo.idCliente = acc.idCliente
                                              left join warehouse.PF_PickedList pl on oo.BarCode = pl.BarCode
	                                            OUTER APPLY
	                                            (
		                                            SELECT * FROM dbo.getWSDtempTable(oo.BarCode)								--quella che usava salmedin
	                                            ) ef
                                              where --oo.BarCode = '272223'															
	                                              (Magazzino = 0 OR (Magazzino = 1 and oo.idCliente <> 86))													--Escluso OMNITECH
                                              and oo.idCliente = @IdCliente
                                              and oo.DataConsegna > '20250101'
                                              ----and wpf.dateIn is null																						--non sono a magazzino
                                              and wpf.dateOut is null																
                                              and oo.Chiuso = 0
                                              and ef.B is null
                                              and pl.[Nr. lista prelievo] is null																			--non inseriti in una lista di prelievo
                                              and coalesce([dataConsegnaDefinitiva], [DataConsegna]) between @DataSpedizioneFrom and @DataSpedizioneTo

                                            )
                                            ,
                                            cte2 as
                                            (

                                            select	cte.BarCode, ac.RagioneSociale, ast.Stampo, aca.CategoriaArticolo,
		                                            cte.DataOrdine, cte.DataConsegnaRichiesta, cte.dataConsegnaDefinitiva,

		                                            cte.LocazioneDiMagazzino, cte.DataVersamentoAMagazzino, 
		                                            --cte.dateOut, cte.[Nr. lista prelievo],
		                                            cte.idGiornoCarico

		                                            ,case
		                                                when cte.LocazioneDiMagazzino is not null then [produzione].[GetPrimaDataConsegnaPossibile](cte.BarCode, default)
			
			                                            when coalesce([dataConsegnaDefinitiva], [DataConsegnaRichiesta]) >= @DataSpedizioneFrom and coalesce([dataConsegnaDefinitiva], [DataConsegnaRichiesta]) > @today       then [produzione].[GetDataProssimoCamion](    (select cast(MAX(DataSped) as date) from ( values (@today),(@DataSpedizioneFrom) , (coalesce([dataConsegnaDefinitiva], [DataConsegnaRichiesta]))   ) as AllDate(DataSped))      , cte.idCliente, 1) 

			                                            when coalesce([dataConsegnaDefinitiva], [DataConsegnaRichiesta]) >= @DataSpedizioneFrom																				   then [produzione].[GetDataProssimoCamion](    (select cast(MAX(DataSped) as date) from ( values (@today),(@DataSpedizioneFrom) , (coalesce([dataConsegnaDefinitiva], [DataConsegnaRichiesta]))   ) as AllDate(DataSped))      , cte.idCliente, default) 

			
			                                            else null
	                                                end PrimaDataSpedizionePossibile

                                            from cte
                                            left join anagrafica.Clienti ac on cte.idCliente = ac.idCliente
                                            left join anagrafica.Stampi ast on cte.idStampo = ast.idStampo
                                            left join ( select distinct idCategoriaArticolo, CategoriaArticolo from anagrafica.CategorieArticoli ) aca on cte.idLeanType = aca.idCategoriaArticolo
                                            )

                                            select * from cte2
                                            where PrimaDataSpedizionePossibile <= @DataSpedizioneTo
                                            order by BarCode                                    
";


                    oo = (await conn.QueryAsync<Order>(query, new { IdCliente, DataSpedizioneFrom, DataSpedizioneTo })).ToList();

                    r.ReturnObject = oo;
                    r.IsOk = true;
                }

                #endregion
            }
            catch (Exception ex)
            {
                oo = null;

                r.Exception = ex;
            }

            return r;

        }
    }
}
