using Dapper;
using Omnitech.Prezzi.Core.Enums;
using Omnitech.Prezzi.Core.Models;
using Omnitech.Prezzi.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Omnitech.Prezzi.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public CustomerInfo GetCustomerInfo(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT
                        c.IdCliente as CustomerId,
                        c.RagioneSociale as CompanyName,
                        COALESCE(c.ListinoML, 0) as PriceListType,
                        COALESCE(c.MaggiorazioneColore, 0.0) as ColorSurcharge,
                        COALESCE(c.MaggiorazioneColoreCatini, 0.0) as ColorSurchargeCatini,
                        COALESCE(c.MaggiorazioneNero, 0.0) as BlackSurcharge,
                        COALESCE(c.MaggiorazioneNeroCatini, 0.0) as BlackSurchargeCatini,
                        COALESCE(c.[LimiteProfonditàCalcoloPrezzoTopVascaIntegrata], 0) as DepthLimitForTVI,
                        COALESCE(c.[MaggiorazioneProfonditàMassima], 0.0) as MaxDepthSurcharge,
                        0.0 as GeneralDiscount,       -- This column doesn't exist, use default
                        '1900-01-01' as PriceIncreaseDate,  -- This column doesn't exist, use default
                        0 as Increases2022Flag,       -- This column doesn't exist, use default
                        COALESCE(c.NuoviAumentiAccLav, 0) as UseNewIncreaseMethod,  -- AGGIUNTO: flag metodo aumenti
                        CASE WHEN lp.IdCliente IS NOT NULL THEN 1 ELSE 0 END as HasCustomPriceList
                    FROM anagrafica.Clienti c
                    LEFT JOIN anagrafica.ClientiListini__ lp
                        ON c.IdCliente = lp.IdCliente
                    WHERE c.IdCliente = @customerId";

                var customer = connection.QueryFirstOrDefault<CustomerInfo>(query, new { customerId });
                
                if (customer != null)
                {
                    // Converti il valore booleano per PriceListType
                    customer.PriceListType = customer.PriceListType == PriceListType.MetroLineare
                        ? PriceListType.MetroLineare
                        : PriceListType.Standard;

                    // AGGIUNTO: Carica aumenti se usa il metodo nuovo - replica logica PsR
                    if (customer.UseNewIncreaseMethod)
                    {
                        LoadCustomerIncreases(customer);
                    }
                }

                return customer;
            }
        }

        public CustomerInfo GetCustomerByCode(string customerCode)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT
                        c.IdCliente as CustomerId,
                        c.RagioneSociale as CompanyName,
                        COALESCE(c.ListinoML, 0) as PriceListType,
                        COALESCE(c.MaggiorazioneColore, 0.0) as ColorSurcharge,
                        COALESCE(c.MaggiorazioneColoreCatini, 0.0) as ColorSurchargeCatini,
                        COALESCE(c.MaggiorazioneNero, 0.0) as BlackSurcharge,
                        COALESCE(c.MaggiorazioneNeroCatini, 0.0) as BlackSurchargeCatini,
                        COALESCE(c.[LimiteProfonditàCalcoloPrezzoTopVascaIntegrata], 0) as DepthLimitForTVI,
                        COALESCE(c.[MaggiorazioneProfonditàMassima], 0.0) as MaxDepthSurcharge,
                        0.0 as GeneralDiscount,       -- This column doesn't exist, use default
                        '1900-01-01' as PriceIncreaseDate,  -- This column doesn't exist, use default
                        0 as Increases2022Flag,       -- This column doesn't exist, use default
                        CASE WHEN lp.IdCliente IS NOT NULL THEN 1 ELSE 0 END as HasCustomPriceList
                    FROM anagrafica.Clienti c
                    LEFT JOIN anagrafica.ClientiListini__ lp
                        ON c.IdCliente = lp.IdCliente
                    WHERE c.CodiceCliente = @customerCode";

                var customer = connection.QueryFirstOrDefault<CustomerInfo>(query, new { customerCode });
                
                if (customer != null)
                {
                    customer.PriceListType = customer.PriceListType == PriceListType.MetroLineare 
                        ? PriceListType.MetroLineare 
                        : PriceListType.Standard;
                }

                return customer;
            }
        }

        public bool HasCustomPriceList(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT COUNT(1)
                    FROM anagrafica.ClientiListini__
                    WHERE IdCliente = @customerId";

                return connection.QuerySingle<int>(query, new { customerId }) > 0;
            }
        }

        public List<CustomerInfo> GetActiveCustomers()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        c.IdCliente as CustomerId,
                        c.RagioneSociale as CompanyName,
                        c.ListinoML as PriceListType,
                        c.MaggiorazioneColore as ColorSurcharge,
                        c.MaggiorazioneColore_Catini as ColorSurchargeCatini,
                        c.MaggiorazioneNero as BlackSurcharge,
                        c.MaggiorazioneNero_Catini as BlackSurchargeCatini,
                        c.LimiteProfonditaCalcoloPrezzoTVI as DepthLimitForTVI,
                        c.MaggiorazioneProfonditaMassima as MaxDepthSurcharge,
                        c.ScontoGenerale as GeneralDiscount,
                        c.DataAumenti as PriceIncreaseDate,
                        c.Aumenti2022Flag as Increases2022Flag,
                        CASE WHEN lp.IdCliente IS NOT NULL THEN 1 ELSE 0 END as HasCustomPriceList
                    FROM anagrafica.Clienti c
                    LEFT JOIN anagrafica.ClientiListini__ lp
                        ON c.IdCliente = lp.IdCliente
                    WHERE c.Attivo = 1
                    ORDER BY c.RagioneSociale";

                var customers = connection.Query<CustomerInfo>(query).ToList();
                
                foreach (var customer in customers)
                {
                    customer.PriceListType = customer.PriceListType == PriceListType.MetroLineare 
                        ? PriceListType.MetroLineare 
                        : PriceListType.Standard;
                }

                return customers;
            }
        }

        public CustomerSurcharges GetCustomerSurcharges(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT
                        COALESCE(MaggiorazioneColore, 0.0) as ColorSurcharge,
                        COALESCE(MaggiorazioneColoreCatini, 0.0) as ColorSurchargeCatini,
                        COALESCE(MaggiorazioneNero, 0.0) as BlackSurcharge,
                        COALESCE(MaggiorazioneNeroCatini, 0.0) as BlackSurchargeCatini,
                        COALESCE([MaggiorazioneProfonditàMassima], 0.0) as MaxDepthSurcharge,
                        COALESCE([LimiteProfonditàCalcoloPrezzoTopVascaIntegrata], 0) as DepthLimitForTVI
                    FROM anagrafica.Clienti
                    WHERE IdCliente = @customerId";

                return connection.QueryFirstOrDefault<CustomerSurcharges>(query, new { customerId });
            }
        }

        public bool UpdateCustomerInfo(CustomerInfo customer)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE anagrafica.Clienti
                    SET
                        RagioneSociale = @CompanyName,
                        ListinoML = @PriceListType,
                        MaggiorazioneColore = @ColorSurcharge,
                        MaggiorazioneColoreCatini = @ColorSurchargeCatini,
                        MaggiorazioneNero = @BlackSurcharge,
                        MaggiorazioneNeroCatini = @BlackSurchargeCatini,
                        [LimiteProfonditàCalcoloPrezzoTopVascaIntegrata] = @DepthLimitForTVI,
                        [MaggiorazioneProfonditàMassima] = @MaxDepthSurcharge
                        -- Removed: ScontoGenerale, DataAumenti, Aumenti2022Flag - columns don't exist
                    WHERE IdCliente = @CustomerId";

                var rowsAffected = connection.Execute(query, customer);
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Equivalente di Query 88 di PsR - Recupera dati master del cliente
        /// Replica la logica di cAzienda.init(idCliente) in PsR
        /// </summary>
        public CustomerInfo GetCustomerInfoPsREquivalent(int idCliente)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT
                        c.IdCliente as CustomerId,
                        c.RagioneSociale as CompanyName,
                        COALESCE(c.ListinoML, 0) as IsLinearMeterPriceList,
                        COALESCE(c.MaggiorazioneColore, 0.0) as ColorSurcharge,
                        COALESCE(c.MaggiorazioneColoreCatini, 0.0) as ColorSurchargeCatini,
                        COALESCE(c.MaggiorazioneNero, 0.0) as BlackSurcharge,
                        COALESCE(c.MaggiorazioneNeroCatini, 0.0) as BlackSurchargeCatini,
                        COALESCE(c.[LimiteProfonditàCalcoloPrezzoTopVascaIntegrata], 0) as DepthLimitForTVI,
                        COALESCE(c.[MaggiorazioneProfonditàMassima], 0.0) as MaxDepthSurcharge,
                        COALESCE(c.idClientePadre, 0) as ParentCustomerId,
                        COALESCE(c.nuovoListino, 0) as HasNewPriceList,
                        COALESCE(c.nuovoListinoOnda, 0) as HasNewOndaPriceList,
                        COALESCE(c.IRC, 0) as IRC,
                        COALESCE(c.Contract, 0) as HasContract,
                        COALESCE(c.itsOwnBrand, 0) as HasOwnBrand,
                        COALESCE(c.NuoviAumentiAccLav, 0) as UseNewIncreaseMethod,
                        CASE WHEN lp.IdCliente IS NOT NULL THEN 1 ELSE 0 END as HasCustomPriceList
                    FROM anagrafica.Clienti c
                    LEFT JOIN anagrafica.ClientiListini lp
                        ON c.IdCliente = lp.IdCliente
                    WHERE c.IdCliente = @idCliente";

                return connection.QueryFirstOrDefault<CustomerInfo>(query, new { idCliente });
            }
        }

        /// <summary>
        /// Equivalente di Query 775 di PsR - Recupera aumenti e sconti del cliente
        /// Replica la logica di cAzienda.GetAumenti(idCliente) in PsR
        /// </summary>
        public void LoadCustomerIncreases(CustomerInfo customer)
        {
            if (customer == null) return;

            using (var connection = new SqlConnection(_connectionString))
            {
                // Prima query: recupera flag NuoviAumentiAccLav per determinare il metodo
                var flagQuery = @"
                    SELECT COALESCE(NuoviAumentiAccLav, 0) as UseNewMethod
                    FROM anagrafica.Clienti
                    WHERE IdCliente = @idCliente";

                var useNewMethod = connection.QueryFirstOrDefault<bool>(flagQuery, new { idCliente = customer.CustomerId });
                customer.UseNewIncreaseMethod = useNewMethod;

                if (!useNewMethod)
                {
                    // METODO OLD: Query aumenti classici da tabella AvanzamentoProduzioneListini.anagrafica.Aumenti_OLD
                    var oldIncreaseQuery = @"
                        SELECT
                            TipoAumento,
                            COALESCE(Aumento, 0.0) as IncreaseValue
                        FROM AvanzamentoProduzioneListini.anagrafica.Aumenti_OLD
                        WHERE idCliente = @idCliente";

                    var oldIncreases = connection.Query(oldIncreaseQuery, new { idCliente = customer.CustomerId });

                    foreach (var increase in oldIncreases)
                    {
                        switch (increase.TipoAumento?.ToString())
                        {
                            case "Accessori":
                                customer.AccessoryIncrease = increase.IncreaseValue ?? 0;
                                break;
                            case "Lavorazioni":
                                customer.WorkingIncrease = increase.IncreaseValue ?? 0;
                                break;
                        }
                    }
                }
                else
                {
                    // METODO NEW: Query aumenti dettagliati con logica di filtraggio PsR
                    // Questa query replica la logica che riduce 87 record ai 12 aumenti effettivi
                    var newIncreaseQuery = @"
                        WITH LatestIncreases AS (
                            -- Seleziona aumenti più recenti per ogni idTipoSconto e idFascicolo
                            SELECT
                                idFascicolo,
                                idTipoSconto,
                                PercentualeAumento,
                                DataEntrataVigore,
                                DaScontare,
                                idMateriale,
                                ROW_NUMBER() OVER (
                                    PARTITION BY idFascicolo, idTipoSconto
                                    ORDER BY DataEntrataVigore DESC,
                                             CASE WHEN DaScontare = 0 THEN 0 ELSE 1 END
                                ) as rn
                            FROM AvanzamentoProduzioneListini.anagrafica.Aumenti WITH (NOLOCK)
                            WHERE idCliente = @idCliente
                            AND Attivo = 1
                            AND DataEntrataVigore <= GETDATE()
                        ),
                        FilteredIncreases AS (
                            -- Prendi solo gli aumenti più recenti (rn = 1)
                            SELECT
                                idFascicolo,
                                idTipoSconto,
                                PercentualeAumento,
                                DataEntrataVigore,
                                DaScontare,
                                idMateriale
                            FROM LatestIncreases
                            WHERE rn = 1
                        )
                        SELECT
                            idFascicolo as Id,
                            @idCliente as CustomerId,
                            CASE
                                WHEN idTipoSconto <= 10 THEN 'Lavorazioni'
                                ELSE 'Accessori'
                            END as IncreaseType,
                            COALESCE(PercentualeAumento, 0.0) as IncreasePercentage,
                            CAST(COALESCE(idMateriale, 0) AS VARCHAR(50)) as ItemCode,
                            CONCAT('F', idFascicolo, '-T', idTipoSconto,
                                   CASE WHEN DaScontare = 1 THEN ' (Sconto)' ELSE ' (Aumento)' END) as Description,
                            DataEntrataVigore as ValidFrom,
                            NULL as ValidTo
                        FROM FilteredIncreases
                        ORDER BY idFascicolo, idTipoSconto";

                    customer.SpecificIncreases = connection.Query<CustomerIncrease>(newIncreaseQuery,
                        new { idCliente = customer.CustomerId }).ToList();

                    // Calcola aumenti medi per compatibilità
                    var accessoryIncreases = customer.SpecificIncreases.Where(i => i.IncreaseType == "Accessori");
                    var workingIncreases = customer.SpecificIncreases.Where(i => i.IncreaseType == "Lavorazioni");

                    customer.AccessoryIncrease = accessoryIncreases.Any() ? accessoryIncreases.Average(i => i.IncreasePercentage) : 0;
                    customer.WorkingIncrease = workingIncreases.Any() ? workingIncreases.Average(i => i.IncreasePercentage) : 0;
                }
            }
        }

        /// <summary>
        /// Metodo completo che replica il flusso PsR: Query 88 + Query 775
        /// Equivalente a: new cAzienda(idCliente) + GetAumenti(idCliente)
        /// </summary>
        public CustomerInfo GetCompleteCustomerInfo(int idCliente)
        {
            // Step 1: Query 88 equivalent - Customer master data
            var customer = GetCustomerInfoPsREquivalent(idCliente);

            if (customer != null)
            {
                // Step 2: Query 775 equivalent - Customer increases and discounts
                LoadCustomerIncreases(customer);
            }

            return customer;
        }
    }
}