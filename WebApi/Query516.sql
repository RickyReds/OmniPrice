--<516> Recupera info Ordine da BarCode
-- 2025.03	  | Aggiunta Gestione ExtraVerniciatura
-- 2024.11.18 | Aggiunto: IdDivisione
-- 2024.10.10 | Aggiunto: BarcodePilette
-- 2023.10.16 | Aggiunto: Flag Modula
-- 2023.09.25 | Aggiunto: Tassativo  (--> dbase: DataBloccataAle)
-- 2022.03.11 | Aggiunto: OrdineComposto
-- 2022.02.02 | Aggiunto: SenzaPastaInTinta
-- 2021.12.13 | Aggiunto: OrdineAccessorio
-- 2021.12.07 | Aggiunto: DataLottoStampaggio, ral.HEX
-- 2021.11.05 | Aggiunto: RientratoScartoDaTerzista
-- 2021.08.13 | Aggiunto: dbo.getDates(oo.BarCode)
-- 2020.11.13 | Aggiunto: GruppoSpedizione
-- 2020.11.12 | Aggiunto: BollinoRosso
-- 2020.09.17 | Aggiunto: NoteSuDisegno
-- 2020.06.26 | Aggiunti campi: isOrdineLotto
-- 2020.06.24 | Aggiunti campi: CreaOP
-- 2020.03.16 | Aggiunti campi: Scatolatura2Lunghezza
-- 2020.02.24 | Aggiunti campi: FresataVetro e RiduzioneProfondità
-- 2020.02.21 | Secondo collegamento alla tabella Clienti.DescrizioneArticoli per codiceclientevendita idea
-- 2020.01.20 | Inizio migrazione campi lavorazioni in tabella: 'orini.OrdiniLavorazioni'
-- 2019.12.23 | Aggiunto campo: Ikon
-- 2019.10.25 | Aggiunto campo: CodiceCliente
-- 2019.10.16 | Aggiunto campo: nomeFileTemplate
-- 2019.09.05 | Aggiunto campi: idTipoOrdineNAV, CodiceKanban
-- 2019.05.13 | Aggiunti campi: idTipologiaImballo, idTipologiaRinforzoImballo e idFinituraRinforzoImballo
-- 2019.04.26 | Aggiunto campo: ScatolaturaCambioProfondità
-- 2019.04.04 | Aggiunto campo: nomeFileTemplate
-- 2019.02.06 | Aggiunto campo: Modello
-- 2019.01.23 | Aggiunto campo: Ager
-- 2018.11.26 | Aggiunto campo: FormRAL
-- 2018.10.02 | Aggiunto campo: Living
-- 2018.09.11 | Aggiunto campo: SalvaGoccia
-- 2018.09.04 | Aggiunti campi: NumeroVasche, SequenzaVasche
-- 2018.08.02 | Aggiunto campo: AltezzaIncasso
-- 2018.08.02 | Aggiunto campo: ca.idCategoriaArticoloRaggruppamento=t1.[Data]
-- 2018.07.27 | Aggiunto campo: idSLorder
-- 2018.07.27 | Aggiunto campo: consumatoDaBarCode
-- 2018.05.18 | Aggiunto campo: AirPool
-- 2018.05.17 | Aggiunti campi: idTipologiaPiletta, idTipologiaCopriPiletta, idMaterialeCopripiletta
-- 2018.04.09 | prende la categoria e la categoriaMaster da tabelle: StampiCategorieArticoli e CategorieArticoli
-- 2018.03.06 | Aggiunto BookingStep, BookingDate
-- 2018.03.05 | Aggiunto PiacereAlCliente
-- 2018.03.01 | Aggiunto GiunzioneOrizzontaleLunghezza
-- 2018.02.28 | Aggiunto CambioProfondità
-- 2018.02.15 | Aggiunto Numero Carrello e dataCondegnaDefinitiva
-- 2017.11.28 | aggiunte info CSA
-- 2017.10.04 |
--
DECLARE @BarCode AS dbo.BarCode = '012027'
;
SELECT
oo.BarCode, da2.CodiceVenditaCliente,

oo.OrdineComposto, --2022.03.11
oo.OrdineAccessorio, --2021.12.13
oo.SenzaPastaInTinta, --2022.02.02

oo.tipologia, oo.Modello,
oo.isOrdineLotto, --2020.06.26
oo.CreaOP, --2020.06.24

oo.DataBloccataAle as Tassativo,

case 
	when oo.Modula is null THEN 0
	else oo.Modula
end as Modula,

case 
	when oo.DwgDxf is null THEN 0
	else oo.DwgDxf
end as DwgDxf,

case 
	when oo.ExtraVerniciatura is null THEN 0
	else oo.ExtraVerniciatura
end as ExtraVerniciatura,

oo.idRALExtraVerniciatura,

case 
	when oo.idTipoExtraVerniciatura is null THEN -1			-- "-1" è il valore della enum
	else oo.idTipoExtraVerniciatura
end as idTipoExtraVerniciatura,

oo.IdDivisione,

--ACCESSORI
'-=ACCESSORI=-' '-=ACCESSORI=-',
oo.Dima, oo.idTipoCassaInLegno,
oo.idTipologiaImballo, oo.idTipologiaRinforzoImballo, oo.idFinituraRinforzoImballo,
oo.idTipoPiletta,
oo.idTipologiaPiletta, oo.idFormaCover, oo.idMaterialeCover, oo.BarcodePilette,
oo.idTipologiaSifone, oo.idTipoTroppoPieno, oo.Accessori,
oo.AirPool, oo.Ager,
--ACCESSORI
--CLIENTE
'-=CLIENTE=-' '-=CLIENTE=-'
, oo.idCliente,  [dbo].[getArticoloCodiceCliente_new](oo.[idCliente], oo.[idStampo], oo.[idMateriale], oo.[idPosizioneVasca], oo.[L], [dbo].[getNumFori](oo.[Forature]),
	CASE idfinitura
		WHEN 2 THEN 1 
		ELSE 0 
	END, oo.[idRAL]
	) CodCli,
--CLIENTE
--DB INFO
COALESCE(gd.DataInserimento, oo.DataInserimento) DataInserimento, oo.DataModifica, oo.idOrdine,
oo.CodiceCliente, --2019.10.25
--DB INFO

--DIMENSIONI
oo.L, oo.P, oo.H,
--DIMENSIONI

--LAVORAZIONI
oo.AltezzaIncasso --2018.08.02
--,'Inizio LAVORAZIONI' Lav
, COALESCE(ol.Giunzioni, oo.Giunzioni) Giunzioni
--, COALESCE(ol.Giunzione2Lunghezza, oo.GiunzioneOrizzontaleLunghezza) Giunzione2Lunghezza
, ol.Giunzione2Lunghezza
, ol.Giunzione4Lunghezza
, ol.Scatolatura2Lunghezza --2020.03.16
, COALESCE(ol.Scatolatura, oo.Scatolatura) Scatolatura
, COALESCE(ol.ScatolaturaCambioProfondità, oo.ScatolaturaCambioProfondità) ScatolaturaCambioProfondità --2019.04.26
, COALESCE(ol.CambioProfondità, CAST(oo.CambioProfondità as nvarchar(4))) CambioProfondità
, ol.Fresata45, ol.FresataLED
, ol.BordoContenitivo
, ol.FresataVetro --2020.02.24
, ol.RiduzioneProfondità --2020.02.24
, ol.Legni						--2024.06.03

, oo.idFinitura, oo.Forature
, oo.SenzaForo, oo.StampareDritto,
--LAVORAZIONI

--MATERIALE
oo.[Basic], oo.idMateriale, oo.idRAL, oo.idTexture, oo.EcomaltaRAL,
--MATERIALE

--ORDINE
(SELECT consumatoDaBarCode
FROM warehouse.Orders
WHERE BarCode = @BarCode) consumatoDaBarCode,
oo.BancalePrenotato, oo.BarCodeCliente, oo.ClienteFinale, oo.Collo

, COALESCE(oo.dataConsegnaDefinitiva, oo.DataConsegnaModificata, oo.DataConsegna) dc
, COALESCE(gd.DataOrdine, oo.DataOrdine) DataOrdine --2021.08.13

, oo.DataLottoStampaggio DataStampaggio --2021.12.07

, gd.DataConsegnaRichiesta --2021.08.13
, gd.DataConsegnaConfermata --2021.08.13
, oo.DataVersamentoMagazzino --2021.08.13

, oo.Fiera, oo.ImballoRobusto, oo.IRC, oo.Laminato,
oo.Magazzino, oo.Notazioni, oo.Prezzo, oo.PrezzoAutomatico, oo.Quantità, oo.Riferimento, oo.RiferimentoCliente, oo.RiferimentoClienteFinale, oo.BarCodeCliente, oo.Sospeso, oo.FaP, oo.CSA, oo.CSAnum,
oo.PiacereAlCliente, oo.BookingStep, oo.BookingDate,
--ORDINE

--PRODUZIONE
'-=PRODUZIONE=-' '-=PRODUZIONE=-'
,oo.toCRMdata, oo.toNAVdata, oo.daStemi,
(SELECT TOP (1) Locazione FROM Ordini.Locazioni WHERE BarCode = @BarCode) Carrello,
--PRODUZIONE

--LAVORAZIONI
oo.Living,
--LAVORAZIONI

--STAMPO
'-=STAMPO=-' '-=STAMPO=-'
, report.isSLorder(oo.idMateriale, oo.idStampo, Forature, oo.AltezzaIncasso, oo.Tipologia) isSLorder
, oo.DoppiaVasca, NumeroVasche, SequenzaVasche
, oo.idStampo, oo.LastraPiuVascaSaldata, oo.idLeanType, oo.idPosizioneVasca--, oo.Tipologia
, lt.leanTypeLabel, COALESCE(lt.idLeanTypeGrouping, 0) idLeanTypeGrouping
--STAMPO
, sca.CategoriaArticolo, sca.idCategoria, COALESCE(ca.idCategoriaArticoloMaster, ca.idCategoriaArticolo) idCategoriaArticoloMaster
, oo.SalvaGoccia --2018.09.11
--, oo.FormRAL --2018.11.26
, oo.idCodiceFormRAL --2018.11.30
, cf.CodiceForm CodiceFormRAL --2018.12.04
, oo.nomeFileTemplate --2019.04.04
, idTipoOrdineNAV --2019.09.05
, CodiceKanban --2019.09.05
, da.ImportaNoteDaDisegno --2019.11.12
, Ikon --2019.12.23
, oo.NoteSuDisegno --2020.09.17
, oo.BollinoRosso --2020.11.12
, oo.GruppoSpedizione --2020.11.13
, oo.RientratoScartoDaTerzista Scarto --2021.11.05
, ral.HEX --2021.12.07
FROM ordini.Ordini oo
	LEFT JOIN anagrafica.LeanTypes lt ON lt.idLeanType = oo.idLeanType
	LEFT JOIN anagrafica.ScalaRAL ral ON ral.idRAL = oo.idRAL --2021.12.07
	LEFT JOIN anagrafica.CodiciForm cf ON cf.idCodiceForm = oo.idCodiceFormRAL
	LEFT JOIN anagrafica.StampiDimensioni sd ON sd.idStampo = oo.idStampo --2018.08.02
	LEFT JOIN anagrafica.StampiCategorieArticoli sca
		ON sca.idStampo = oo.idStampo AND sca.idCategoria = oo.Tipologia
	CROSS APPLY dbo.Split(sca.idCategoria, ',') T1
	LEFT JOIN anagrafica.CategorieArticoli ca
		ON ca.idCategoriaArticolo = oo.idLeanType
	--CROSS APPLY dbo.Split(ca.idCategoriaArticoloRaggruppamento, ',') T2
	LEFT JOIN clienti.DescrizioneArticoli da ON da.NomeFile = oo.nomeFileTemplate
	LEFT JOIN clienti.DescrizioneArticoli da2
		ON da2.idStampo = oo.idStampo AND da2.idCliente = oo.idCliente AND da2.NumFori = SUBSTRING(oo.Forature, 1, 1)
	LEFT JOIN ordini.OrdiniLavorazioni ol ON ol.Barcode = oo.Barcode
	CROSS APPLY dbo.getDates(oo.BarCode) gd --2021.08.13
--CROSS APPLY dbo.Split(t.idTipologieArticolo, ',') T1

WHERE oo.BarCode = @BarCode --AND  t1.[Data] = t2.[Data]