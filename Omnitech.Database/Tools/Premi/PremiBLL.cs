using HI.Libs.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using static Omnitech.DTO.Tools.Premi.DTO;


namespace Omnitech.Database.Tools.Premi
{
    public class BLL
    {   
        public static Result<List<AllItems>> GetData(string ConnectionString, List<string> CustomerNrs, DateTime From, DateTime? To = null)
        {
            Result<List<AllItems>> r = new Result<List<AllItems>>("Omnitech.Database.Tools.Premi.BLL.GenerateReport()");

            List<AllItems> oiList = new List<AllItems>();


            try
            {
                #region

                using (IDbConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    foreach(string CustomerNr in  CustomerNrs)
                    {
                        #region query

                        string query = @"

                                use [Omnitech]

                                DECLARE @Gruppo as int = 1
                                --DECLARE @CustomerNr as nvarchar(10) = 'CI000145'
                                --DECLARE @From as date = '2024-01-01'
                                --DECLARE @To as date = null	--'2023-12-31'									--(null --> today)

                                ------------------------------------------------------------------------------------------------------------------------------------------------

                                --DECLARE @CustomerNrs TABLE (CustomerNr nvarchar(10))											
                                --IF @Gruppo = 1
                                --	INSERT INTO @CustomerNrs VALUES ('CI000145'), ('CI000033'), ('CI000098'), ('CI000380');		--GRUPPO 1 : idea, aqua, disenia, blob
                                --IF @Gruppo = 2
                                --	INSERT INTO @CustomerNrs VALUES ('CI000029');												--GRUPPO 2 : Agora
                                --IF @Gruppo = 3
                                --	INSERT INTO @CustomerNrs VALUES ('CI000055'), ('CI000042');									--GRUPPO 3 : azzurra, area bagno
                                --IF @Gruppo = 4
                                --	INSERT INTO @CustomerNrs VALUES ('CI000039');												--GRUPPO 4 : archeda
                                --IF @Gruppo = 5
                                --	INSERT INTO @CustomerNrs VALUES ('CI000064');												--GRUPPO 5 : bmt
                                --IF @Gruppo = 6
                                --	INSERT INTO @CustomerNrs VALUES ('CI000360');												--GRUPPO 6 : gruppo geromin
                                --IF @Gruppo = 7
                                --	INSERT INTO @CustomerNrs VALUES ('CI000146');												--GRUPPO 7 : ideal bagni
                                --IF @Gruppo = 8
                                --	INSERT INTO @CustomerNrs VALUES ('CI000234');												--GRUPPO 8 : rexa


                                --DECLARE @Campioncini_Esclusi TABLE (CampNo nvarchar(15))
                                --INSERT INTO @Campioncini_Esclusi VALUES ('DBPCAMPIONCINO'), ('DMPCAMPIONCINO'), ('GMPCAMPIONCINO'), ('DSPCAMPIONCINO'), ('DSSCAMPIONCINO'), ('GRPCAMPIONCINO'), ('TRPCAMPIONCINO')

                                --DECLARE @ContiCG_Esclusi TABLE (ContoNo nvarchar(15))
                                --INSERT INTO @ContiCG_Esclusi VALUES ('')



                                -------------------------------------------------------------------------------------------------------------------------------------------------
                                SET @To = CAST(COALESCE(@To, GetDate()) as date)

                                DECLARE @Fap TABLE (FapValue int, FapDescription nvarchar(20))
                                INSERT INTO @Fap VALUES (0,''), (1,'Contratto'), (2,'Idea Serie My Time'), (3,'Prom. Qualità'), (4,'Form'), (5,'BENT'), (6,'Piatti'), (7,'Varie')		--Descrizioni tipologie FAP (0='',1='Contratto',2='Idea Serie My Time',3='Prom. Qualità',4='Form',5='BENT',6='Piatti',7='Varie') 



                                -------------------------------------------------------------------------------------------------------------------------------------------------
                                -- FATTURE (TUTTE)     CON AMOUNT <> 0     (RIGHE NEL PERIODO D'INDAGINE RAGGRUPPATE/SOMMATE)

                                select  
                                sih.[Sell-to Customer No_] CustomerNr, 
                                sih.[Bill-to Name] CustomerName, 
                                sih.No_ DocumentNr, 
                                sih.[Posting Date] PostingDate,								--FORMAT(sih.[Posting Date],'dd/MM/yyyy') [Posting Date], 
                                sih.[Posting Description] PostingDescription
                                --,sih.[Sell-to Contact No_], sih.[Bill-to Contact No_]
                                --,'|' '|'
                                --,sil.[Document No_]
                                ,sum(sil.Amount) Amount										--,FORMAT(sum(sil.Amount), 'N', 'it-IT') Amount
                                --,sil.[Sell-to Customer No_]
                                --,sil.No_
                                --,sil.Description, sil.[Vs_ Rif_Ordine], sil.Color, sil.[Salesperson Code],sil.[Source No_], sil.[Row Type]
                                ,sih.Fap FapType, /*sil.Fap,*/ 
                                faptable.FapDescription
                                ,sil.Type ItemType						-- --> 1=Conto C/G, 2=Articolo, 3=Resource, 4=Fixed, Asset 5=Addebito (articolo), 6=Allocation Account
                                ,CASE 
	                                WHEN sil.Type = 1 THEN sil.No_		--Con Type=1, No_ contine il Conto C/G
	                                WHEN sil.Type = 5 THEN sil.No_		--Con Type=5, No_ contine V_TRASP (=Addebito (articolo))
	                                ELSE null
                                 END ItemNr
                                --,* 
                                from [dbo].[Omnitech$Sales Invoice Header] sih
                                left join [dbo].[Omnitech$Sales Invoice Line] sil on sih.[No_] = sil.[Document No_]
                                left join @Fap faptable on sih.Fap = faptable.FapValue

                                where (sih.[Sell-to Customer No_] = @CustomerNr /*IN (select * from @CustomerNrs)*/ /*or sih.[Bill-to Customer No_] IN (select * from @CustomerNrs)*/ )			-- La seconda condizione non serve perche sono sempre valorizzati uguali (in coppia)
                                and sih.[Posting Date] > @From and sih.[Posting Date] <= @To
                                and sil.Amount <> 0

                                group by	sih.[Sell-to Customer No_], sih.[Bill-to Name], sih.No_, sih.[Posting Date], sih.[Posting Description], sih.Fap, /*sil.Fap,*/ faptable.FapDescription, 
			                                sil.Type, 
			                                CASE 
				                                WHEN sil.Type = 1 THEN sil.No_
				                                WHEN sil.Type = 5 THEN sil.No_
				                                ELSE null
			                                 END

                                order by sih.[Sell-to Customer No_], sih.No_, sih.[Posting Date]



                                -------------------------------------------------------------------------------------------------------------------------------------------------
                                -- NOTE DI CREDITO SENZA CONTO C/G (Per reso merce)     CON AMOUNT <> 0     (RIGHE NEL PERIODO D'INDAGINE RAGGRUPPATE/SOMMATE)

                                select  
                                scmh.[Sell-to Customer No_] CustomerNr, 
                                scmh.[Bill-to Name] CustomerName, 
                                scmh.No_ DocumentNr, 
                                scmh.[Posting Date] PostingDate,										--FORMAT(scmh.[Posting Date], 'dd/MM/yyyy') [Posting Date], 
                                scmh.[Posting Description] PostingDescription
                                --,scmh.[Sell-to Contact No_], scmh.[Bill-to Contact No_]
                                --,'|' '|'
                                --,scml.[Document No_]
                                ,sum(scml.Amount) Amount												--,FORMAT(sum(scml.Amount), 'N', 'it-IT') Amount
                                --,scml.[Sell-to Customer No_]
                                --,scml.No_
                                --,scml.Description, scml.[Vs_ Rif_Ordine], scml.Color, scml.[Salesperson Code], scml.[Source No_], scml.[Row Type]
                                ,scmh.Fap FapType, 
                                faptable.FapDescription
                                ,scml.Type ItemType							-- --> 1=Conto C/G
                                ,CASE 
	                                WHEN scml.Type = 1 THEN scml.No_		--Con Type=1, No_ contine il Conto C/G
	                                ELSE null
                                 END ItemNr
                                --,* 
                                from [dbo].[Omnitech$Sales Cr_Memo Header] scmh
                                left join [dbo].[Omnitech$Sales Cr_Memo Line] scml on scmh.[No_] = scml.[Document No_]
                                left join @Fap faptable on scmh.Fap = faptable.FapValue

                                where (scmh.[Sell-to Customer No_] = @CustomerNr /*IN (select * from @CustomerNrs)*/ /*or scmh.[Bill-to Customer No_] IN (select * from @CustomerNrs)*/ )			-- La seconda condizione non serve perche sono sempre valorizzati uguali (in coppia)
                                and scmh.[Posting Date] > @From and scmh.[Posting Date] <= @To
                                and scml.Amount <> 0
                                and Type <> 1			----------------> SENZA CONTO C/G (da togliere dal fatturato)   
						                                --				 (NOTA BENE: Se avesse il conto C/G sarebbe la Nota di Credito per il bonus di fine anno:  visto che non compare nella prima tabella, non serve nemmeno toglierla/riaggiungerla)

                                group by	scmh.[Sell-to Customer No_], scmh.[Bill-to Name], scmh.No_, scmh.[Posting Date], scmh.[Posting Description], scmh.Fap, /*sil.Fap,*/ faptable.FapDescription, 
			                                scml.Type, 
			                                CASE 
				                                WHEN scml.Type = 1 THEN scml.No_
				                                ELSE null
			                                 END

                                order by scmh.[Sell-to Customer No_], scmh.No_, scmh.[Posting Date]


                                ----------------------
                                -- NOTE DI CREDITO CON CONTO C/G (che sono i premi) e NON VANNO SOTTRATTE DAL FATTURATO.
                                -- Nei report pasati, le NdC per Bonus erano incluse nel fatturato e quindi dovevano essere riaggiunte!

                                -- (NOTA BENE: Se avesse il conto C/G sarebbe la Nota di Credito per il bonus di fine anno:  visto che non compare nella prima tabella (del fatturato), non serve nemmeno riaggiungerla)



                                -------------------------------------------------------------------------------------------------------------------------------------------------
                                -- CAMPIONCINI      CON AMOUNT <> 0     (RIGHE NEL PERIODO D'INDAGINE NON RAGGRUPPATE) 

                                select  
                                sih.[Sell-to Customer No_] CustomerNr, 
                                sih.[Bill-to Name] CustomerName, 
                                sih.No_ DocumentNr, 
                                sih.[Posting Date] PostingDate,								---FORMAT(sih.[Posting Date],'dd/MM/yyyy') [Posting Date], 
                                sih.[Posting Description] PostingDescription
                                ,sil.Amount													--,FORMAT((sil.Amount), 'N', 'it-IT') Amount
                                ,sih.Fap FapType, 
                                faptable.FapDescription
                                ,sil.Type ItemType														-- --> 1=Conto C/G
                                ,sil.No_ ItemNr
                                ,sil.Description
                                --,* 
                                from [dbo].[Omnitech$Sales Invoice Header] sih
                                left join [dbo].[Omnitech$Sales Invoice Line] sil on sih.[No_] = sil.[Document No_]
                                left join @Fap faptable on sih.Fap = faptable.FapValue

                                where (sih.[Sell-to Customer No_] = @CustomerNr /*IN ( select * from @CustomerNrs)*/ /*or sih.[Bill-to Customer No_] IN (select * from @CustomerNrs)*/ )			-- La seconda condizione non serve perche sono sempre valorizzati uguali (in coppia)
                                and sih.[Posting Date] > @From and sih.[Posting Date] <= @To
                                and sil.Amount <> 0
                                --and sil.No_ in (SELECT * FROM @Campioncini_Esclusi)	--> NON VA BENE PERCHE' CI SONO DEGLI STORNI/OMAGGI
                                and sil.Description like 'Campioncin%'					--> Questo è brutto ma tiene conto anche dei campioncini omaggio

                                order by sih.[Sell-to Customer No_], sih.No_, sih.[Posting Date]


                                -------------------------------------------------------------------------------------------------------------------------------------------------
                                -- TRASPORTI      CON AMOUNT <> 0     (RIGHE NEL PERIODO D'INDAGINE NON RAGGRUPPATE) ------>   Type = 5    No_ = ""V_TRASP""

                                select  
                                sih.[Sell-to Customer No_] CustomerNr, 
                                sih.[Bill-to Name] CustomerName, 
                                sih.No_ DocumentNr, 
                                sih.[Posting Date] PostingDate,								---FORMAT(sih.[Posting Date],'dd/MM/yyyy') [Posting Date], 
                                sih.[Posting Description] PostingDescription
                                ,sil.Amount													--,FORMAT((sil.Amount), 'N', 'it-IT') Amount
                                ,sih.Fap FapType, 
                                faptable.FapDescription
                                ,sil.Type ItemType														-- --> 5=Addebito (articolo)
                                ,sil.No_ ItemNr
                                ,sil.Description
                                --,* 
                                from [dbo].[Omnitech$Sales Invoice Header] sih
                                left join [dbo].[Omnitech$Sales Invoice Line] sil on sih.[No_] = sil.[Document No_]
                                left join @Fap faptable on sih.Fap = faptable.FapValue

                                where (sih.[Sell-to Customer No_] = @CustomerNr /*IN ( select * from @CustomerNrs)*/ /*or sih.[Bill-to Customer No_] IN (select * from @CustomerNrs)*/ )			-- La seconda condizione non serve perche sono sempre valorizzati uguali (in coppia)
                                and sih.[Posting Date] > @From and sih.[Posting Date] <= @To
                                and sil.Amount <> 0
                                and Type = 5				---------------->   Type = 5    No_ = ""V_TRASP""
                                and sil.No_ = 'V_TRASP'

                                order by sih.[Sell-to Customer No_], sih.No_, sih.[Posting Date]

                                -------------------------------------------------------------------------------------------------------------------------------------------------
                                -- PROGETTAZIONE VENDITA STAMPI      CON AMOUNT <> 0     (RIGHE NEL PERIODO D'INDAGINE NON RAGGRUPPATE) 

                                select  
                                sih.[Sell-to Customer No_] CustomerNr, 
                                sih.[Bill-to Name] CustomerName, 
                                sih.No_ DocumentNr, 
                                sih.[Posting Date] PostingDate,								---FORMAT(sih.[Posting Date],'dd/MM/yyyy') [Posting Date], 
                                sih.[Posting Description] PostingDescription
                                ,sil.Amount													--,FORMAT((sil.Amount), 'N', 'it-IT') Amount
                                ,sih.Fap FapType, 
                                faptable.FapDescription
                                ,sil.Type ItemType														-- --> 5=Addebito (articolo)
                                ,sil.No_ ItemNr
                                ,sil.Description
                                --,* 
                                from [dbo].[Omnitech$Sales Invoice Header] sih
                                left join [dbo].[Omnitech$Sales Invoice Line] sil on sih.[No_] = sil.[Document No_]
                                left join @Fap faptable on sih.Fap = faptable.FapValue

                                where (sih.[Sell-to Customer No_] = @CustomerNr /*IN ( select * from @CustomerNrs)*/ /*or sih.[Bill-to Customer No_] IN (select * from @CustomerNrs)*/ )			-- La seconda condizione non serve perche sono sempre valorizzati uguali (in coppia)
                                and sih.[Posting Date] > @From and sih.[Posting Date] <= @To
                                and sil.Amount <> 0
                                and Type = 1				---------------->   Type = 1    No_ = ""0134810""
                                and sil.No_ = '0134810'

                                order by sih.[Sell-to Customer No_], sih.No_, sih.[Posting Date]


                                -------------------------------------------------------------------------------------------------------------------------------------------------
                                -- PROGETTAZIONE VENDITA MODELLI      CON AMOUNT <> 0     (RIGHE NEL PERIODO D'INDAGINE NON RAGGRUPPATE) 

                                select  
                                sih.[Sell-to Customer No_] CustomerNr, 
                                sih.[Bill-to Name] CustomerName, 
                                sih.No_ DocumentNr, 
                                sih.[Posting Date] PostingDate,								---FORMAT(sih.[Posting Date],'dd/MM/yyyy') [Posting Date], 
                                sih.[Posting Description] PostingDescription
                                ,sil.Amount													--,FORMAT((sil.Amount), 'N', 'it-IT') Amount
                                ,sih.Fap FapType, 
                                faptable.FapDescription
                                ,sil.Type ItemType														-- --> 5=Addebito (articolo)
                                ,sil.No_ ItemNr
                                ,sil.Description
                                --,* 
                                from [dbo].[Omnitech$Sales Invoice Header] sih
                                left join [dbo].[Omnitech$Sales Invoice Line] sil on sih.[No_] = sil.[Document No_]
                                left join @Fap faptable on sih.Fap = faptable.FapValue

                                where (sih.[Sell-to Customer No_] = @CustomerNr /*IN ( select * from @CustomerNrs)*/ /*or sih.[Bill-to Customer No_] IN (select * from @CustomerNrs)*/ )			-- La seconda condizione non serve perche sono sempre valorizzati uguali (in coppia)
                                and sih.[Posting Date] > @From and sih.[Posting Date] <= @To
                                and sil.Amount <> 0
                                and Type = 1				---------------->   Type = 1    No_ = ""0134810""
                                and sil.No_ = '0017800'

                                order by sih.[Sell-to Customer No_], sih.No_, sih.[Posting Date]


                                -------------------------------------------------------------------------------------------------------------------------------------------------
                                -- CONTRIBUTO COSTRUZIONE STAMPI e  MODELLI      CON AMOUNT <> 0     (RIGHE NEL PERIODO D'INDAGINE NON RAGGRUPPATE) 

                                select  
                                sih.[Sell-to Customer No_] CustomerNr, 
                                sih.[Bill-to Name] CustomerName, 
                                sih.No_ DocumentNr, 
                                sih.[Posting Date] PostingDate,								---FORMAT(sih.[Posting Date],'dd/MM/yyyy') [Posting Date], 
                                sih.[Posting Description] PostingDescription
                                ,sil.Amount													--,FORMAT((sil.Amount), 'N', 'it-IT') Amount
                                ,sih.Fap FapType, 
                                faptable.FapDescription
                                ,sil.Type ItemType														-- --> 5=Addebito (articolo)
                                ,sil.No_ ItemNr
                                ,sil.Description
                                --,* 
                                from [dbo].[Omnitech$Sales Invoice Header] sih
                                left join [dbo].[Omnitech$Sales Invoice Line] sil on sih.[No_] = sil.[Document No_]
                                left join @Fap faptable on sih.Fap = faptable.FapValue

                                where (sih.[Sell-to Customer No_] = @CustomerNr /*IN ( select * from @CustomerNrs)*/ /*or sih.[Bill-to Customer No_] IN (select * from @CustomerNrs)*/ )			-- La seconda condizione non serve perche sono sempre valorizzati uguali (in coppia)
                                and sih.[Posting Date] > @From and sih.[Posting Date] <= @To
                                and sil.Amount <> 0
                                and Type = 1				---------------->   Type = 1    No_ = ""0134810""
                                --and (sil.No_ = '0136450' or sil.No_ = '0136451')                              --------------> NOTA BENE: Dal 2025 va tolto il '0136450'
                                and sil.No_ = '0136451'                                                         --------------> NOTA BENE: Dal 2025 va tolto il '0136450'

                                order by sih.[Sell-to Customer No_], sih.No_, sih.[Posting Date]


                                -------------------------------------------------------------------------------------------------------------------------------------------------
                                -- ORDINI ""CONTRACT""      CON AMOUNT <> 0     (RIGHE NEL PERIODO D'INDAGINE RAGGRUPPATE/SOMMATE) 

                                select  
                                sih.[Sell-to Customer No_] CustomerNr, 
                                sih.[Bill-to Name] CustomerName, 
                                sih.No_ DocumentNr, 
                                sih.[Posting Date] PostingDate,								---FORMAT(sih.[Posting Date],'dd/MM/yyyy') [Posting Date], 
                                sih.[Posting Description] PostingDescription
                                ,sil.Amount													--,FORMAT((sil.Amount), 'N', 'it-IT') Amount
                                ,sih.Fap FapType, 
                                faptable.FapDescription
                                ,sil.Type ItemType														-- --> 5=Addebito (articolo)
                                ,sil.No_ ItemNr
                                ,sil.Description
                                --,* 
                                from [dbo].[Omnitech$Sales Invoice Header] sih
                                left join [dbo].[Omnitech$Sales Invoice Line] sil on sih.[No_] = sil.[Document No_]
                                left join @Fap faptable on sih.Fap = faptable.FapValue

                                where (sih.[Sell-to Customer No_] = @CustomerNr /*IN ( select * from @CustomerNrs)*/ /*or sih.[Bill-to Customer No_] IN (select * from @CustomerNrs)*/ )			-- La seconda condizione non serve perche sono sempre valorizzati uguali (in coppia)
                                and sih.[Posting Date] > @From and sih.[Posting Date] <= @To
                                and sil.Amount <> 0
                                and Type = 2				---------------->   Type = 2    (Item / Articolo)
                                and sih.Fap = 1				---------------->	Fap = 1		(Contract)

                                order by sih.[Sell-to Customer No_], sih.No_, sih.[Posting Date]

                                -------------------------------------------------------------------------------------------------------------------------------------------------
                                -- PROMOZIONI      CON AMOUNT <> 0     (RIGHE NEL PERIODO D'INDAGINE RAGGRUPPATE/SOMMATE) 

                                -- NON CONTEMPLATE (Per il momento)

                                -------------------------------------------------------------------------------------------------------------------------------------------------
                                -- COLLEZIONE FORM      CON AMOUNT <> 0     (RIGHE NEL PERIODO D'INDAGINE RAGGRUPPATE/SOMMATE) 

                                select  
                                sih.[Sell-to Customer No_] CustomerNr, 
                                sih.[Bill-to Name] CustomerName, 
                                sih.No_ DocumentNr, 
                                sih.[Posting Date] PostingDate,								---FORMAT(sih.[Posting Date],'dd/MM/yyyy') [Posting Date], 
                                sih.[Posting Description] PostingDescription
                                ,sil.Amount													--,FORMAT((sil.Amount), 'N', 'it-IT') Amount
                                ,sih.Fap FapType, 
                                faptable.FapDescription
                                ,sil.Type ItemType														-- --> 5=Addebito (articolo)
                                ,sil.No_ ItemNr
                                ,sil.Description
                                --,* 
                                from [dbo].[Omnitech$Sales Invoice Header] sih
                                left join [dbo].[Omnitech$Sales Invoice Line] sil on sih.[No_] = sil.[Document No_]
                                left join @Fap faptable on sih.Fap = faptable.FapValue

                                where (sih.[Sell-to Customer No_] = @CustomerNr /*IN ( select * from @CustomerNrs)*/ /*or sih.[Bill-to Customer No_] IN (select * from @CustomerNrs)*/ )			-- La seconda condizione non serve perche sono sempre valorizzati uguali (in coppia)
                                and sih.[Posting Date] > @From and sih.[Posting Date] <= @To
                                and sil.Amount <> 0
                                and Type = 2				---------------->   Type = 2    (Item / Articolo)
                                and sih.Fap = 4				---------------->	Fap = 4		(Form)			---- IN TEORIA SI APPLICA SOLO al GRUPPO IDEA

                                order by sih.[Sell-to Customer No_], sih.No_, sih.[Posting Date]






    ";

                    
                        #endregion

                        var results = conn.QueryMultiple(query, new { CustomerNr, From, To });

                        AllItems oi = new AllItems();

                        oi.TotaleFatturato = results.Read<LineItem>().ToList();
                        oi.AccreditiPerResiMerce = results.Read<LineItem>().ToList();
                        oi.Campioncini = results.Read<LineItem>().ToList();
                        oi.Trasporti = results.Read<LineItem>().ToList();
                        oi.ProgettazioneVenditaStampi = results.Read<LineItem>().ToList();
                        oi.ProgettazioneVenditaModelli = results.Read<LineItem>().ToList();
                        oi.ContributoCostruzioneStampiModelli = results.Read<LineItem>().ToList();
                        oi.OrdiniContract = results.Read<LineItem>().ToList();
                        //oi.Promozioni = results.Read<LineItem>().ToList();        //al momento non esistono
                        oi.CollezioneForm = results.Read<LineItem>().ToList();

                        oiList.Add(oi);
                    }





                    r.ReturnObject = oiList;
                    r.IsOk = true;
                }

                #endregion

            }
            catch (Exception ex)
            {
                oiList = null;
                r.Exception = ex;
            }

            return r;
        }
    }
}
