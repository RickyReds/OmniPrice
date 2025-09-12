using Omnitech.Prezzi.Core.Enums;
using Omnitech.Prezzi.Core.Models;
using Omnitech.Prezzi.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Omnitech.Prezzi.Core.Services
{
    /// <summary>
    /// Servizio per il calcolo dei prezzi degli articoli
    /// Tradotto da cListini.CalcolaPrezzo() di PsR
    /// </summary>
    public class PriceCalculator
    {
        private readonly IPriceRepository _priceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IDiscountRepository _discountRepository;
        
        // Prezzi calcolati durante l'elaborazione
        private decimal _prezzoFisso;
        private decimal _prezzoIkon;
        private decimal _prezzoModelloPlus;
        private decimal _prezzoPianoIntegrato;
        private decimal _prezzoPiattoDoccia;
        private decimal _prezzoVascaDaBagno;
        private decimal _prezzoLastra;
        private decimal _prezzoLastraMQ;
        private decimal _prezzoScatolatura;
        private decimal _prezzoFresata;
        private decimal _prezzoFresataLed;
        private decimal _prezzoCatino;
        private decimal _prezzoConsolle;
        private decimal _prezzoColonna;
        private decimal _prezzoCassaInLegno;
        private decimal _prezzoDima;
        private decimal _prezzoSalvaGoccia;
        private decimal _prezzoBordoContenitivo;
        private decimal _prezzoAltro;
        
        // Sconti applicati
        private decimal _scontoPianoIntegrato;
        private decimal _scontoPiattoDoccia;
        private decimal _scontoVascaDaBagno;
        private decimal _scontoLastra;
        private decimal _scontoScatolatura;
        private decimal _scontoFresata;
        private decimal _scontoCassaInLegno;
        private decimal _scontoDima;
        
        // Maggiorazioni
        private decimal _prezzoMaggiorazioni;
        private decimal _maggiorazioneTVI;
        
        // Costi aggiuntivi
        private decimal _costoEcomalta;

        public PriceCalculator(
            IPriceRepository priceRepository,
            ICustomerRepository customerRepository,
            IDiscountRepository discountRepository)
        {
            _priceRepository = priceRepository ?? throw new ArgumentNullException(nameof(priceRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _discountRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
        }

        /// <summary>
        /// Calcola il prezzo per un ordine
        /// </summary>
        public PriceResult CalculatePrice(Order order)
        {
            var result = new PriceResult
            {
                Barcode = order.Barcode,
                CalculationDate = DateTime.Now
            };

            try
            {
                // Validazione ordine
                if (!order.IsValid())
                {
                    result.IsSuccess = false;
                    result.Errors.Add("Ordine non valido: verificare barcode, cliente, materiale e dimensioni");
                    return result;
                }

                // Applica trasformazioni cliente-specifiche (come in CalcolaPrezzo)
                ApplyCustomerSpecificTransformations(order);

                // Carica informazioni cliente se non presenti
                if (order.Customer == null)
                {
                    order.Customer = _customerRepository.GetCustomerInfo(order.CustomerId);
                    if (order.Customer == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add($"Cliente {order.CustomerId} non trovato");
                        return result;
                    }
                }

                // Determina il tipo di listino e la tabella da usare
                var priceListInfo = DeterminePriceList(order);
                result.PriceListType = priceListInfo.Type;

                // Reset prezzi
                ResetPrices();

                // Calcola i vari componenti di prezzo basandosi sulla tipologia articolo
                if (order.MainArticleType == ArticleType.TopVascaIntegrata)
                {
                    CalculateIntegratedTopPrice(order, priceListInfo);
                }
                else if (order.MainArticleType == ArticleType.PiattoDoccia)
                {
                    CalculateShowerTrayPrice(order, priceListInfo);
                }
                else if (order.MainArticleType == ArticleType.VascaDaBagno)
                {
                    CalculateBathtubPrice(order, priceListInfo);
                }
                else if (order.MainArticleType == ArticleType.Lastra)
                {
                    CalculateSheetPrice(order, priceListInfo);
                }

                // Calcola componenti aggiuntivi se presenti
                if (order.ArticleTypes.Contains(ArticleType.Scatolatura))
                {
                    CalculateBoxingPrice(order, priceListInfo);
                }

                if (order.ArticleTypes.Contains(ArticleType.Fresata))
                {
                    CalculateMillingPrice(order, priceListInfo);
                }

                if (order.ArticleTypes.Contains(ArticleType.Catino))
                {
                    CalculateBasinPrice(order, priceListInfo);
                }

                // Applica maggiorazioni per colore/texture
                ApplyColorSurcharges(order, result);

                // Applica maggiorazioni per profondità
                ApplyDepthSurcharges(order, result);

                // Applica sconti cliente
                ApplyCustomerDiscounts(order, result);

                // Calcola prezzo finale
                result.BasePrice = CalculateBasePrice();
                result.FinalPrice = CalculateFinalPrice(result);

                // Popola componenti nel risultato
                PopulateComponents(result);

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Errors.Add($"Errore nel calcolo del prezzo: {ex.Message}");
            }

            return result;
        }

        private void ApplyCustomerSpecificTransformations(Order order)
        {
            // Trasformazioni specifiche per cliente (da CalcolaPrezzo)
            
            // Agorà: DeimosMineral -> DeimosStone
            if (order.CustomerId == 1234) // TODO: sostituire con ID reale di Agorà
            {
                if (order.Material == Material.DeimosMineral)
                {
                    order.Material = Material.DeimosStone;
                }
            }

            // Archeda: Deimos -> DeimosStone e texture liscia
            if (order.CustomerId == 5678) // TODO: sostituire con ID reale di Archeda
            {
                if (order.Material == Material.Deimos)
                {
                    order.Material = Material.DeimosStone;
                }
                
                if (order.Texture != Texture.Liscia)
                {
                    if (order.IsSheetPlusBath)
                    {
                        order.IsSheetPlusBath = false;
                        order.MainArticleType = ArticleType.TopVascaIntegrata;
                        order.ArticleTypes[0] = ArticleType.TopVascaIntegrata;
                    }
                    order.Texture = Texture.Liscia;
                }
            }
        }

        private PriceListInfo DeterminePriceList(Order order)
        {
            var info = new PriceListInfo();
            
            // Determina se usare listino ML o normale
            if (order.Customer.PriceListType == PriceListType.MetroLineare)
            {
                info.Type = "ML";
                info.TableName = "listiniML.listinoZero";
            }
            else
            {
                info.Type = "Standard";
                info.TableName = "listini.listinoZero";
            }

            // Verifica se il cliente ha un listino personalizzato
            if (order.Customer.HasCustomPriceList)
            {
                var customTable = _priceRepository.GetCustomPriceListTable(order.CustomerId);
                if (!string.IsNullOrEmpty(customTable))
                {
                    info.TableName = customTable;
                    info.IsCustom = true;
                }
            }

            return info;
        }

        private void CalculateIntegratedTopPrice(Order order, PriceListInfo priceListInfo)
        {
            // Calcola prezzo piano integrato
            var priceData = _priceRepository.GetIntegratedTopPrice(
                priceListInfo.TableName,
                order.Stamp.StampCode,
                order.Material,
                order.Dimensions
            );

            if (priceData != null)
            {
                _prezzoPianoIntegrato = priceData.Price;
                _scontoPianoIntegrato = priceData.Discount;
            }
        }

        private void CalculateShowerTrayPrice(Order order, PriceListInfo priceListInfo)
        {
            // Calcola prezzo piatto doccia
            var priceData = _priceRepository.GetShowerTrayPrice(
                priceListInfo.TableName,
                order.Stamp.StampCode,
                order.Material,
                order.Dimensions
            );

            if (priceData != null)
            {
                _prezzoPiattoDoccia = priceData.Price;
                _scontoPiattoDoccia = priceData.Discount;
            }
        }

        private void CalculateBathtubPrice(Order order, PriceListInfo priceListInfo)
        {
            // Calcola prezzo vasca da bagno
            var priceData = _priceRepository.GetBathtubPrice(
                priceListInfo.TableName,
                order.Stamp.StampCode,
                order.Material,
                order.Dimensions
            );

            if (priceData != null)
            {
                _prezzoVascaDaBagno = priceData.Price;
                _scontoVascaDaBagno = priceData.Discount;
            }
        }

        private void CalculateSheetPrice(Order order, PriceListInfo priceListInfo)
        {
            // Calcola prezzo lastra
            if (order.Customer.PriceListType == PriceListType.MetroLineare)
            {
                // Calcolo a metro quadro
                var sqm = (order.Dimensions.Length * order.Dimensions.Depth) / 1000000.0;
                var pricePerSqm = _priceRepository.GetSheetPricePerSqm(
                    priceListInfo.TableName,
                    order.Material
                );
                
                _prezzoLastraMQ = (decimal)pricePerSqm;
                _prezzoLastra = (decimal)(pricePerSqm * sqm);
            }
            else
            {
                // Calcolo standard
                var priceData = _priceRepository.GetSheetPrice(
                    priceListInfo.TableName,
                    order.Material,
                    order.Dimensions
                );

                if (priceData != null)
                {
                    _prezzoLastra = priceData.Price;
                    _scontoLastra = priceData.Discount;
                }
            }
        }

        private void CalculateBoxingPrice(Order order, PriceListInfo priceListInfo)
        {
            // Calcola prezzo scatolatura
            var priceData = _priceRepository.GetBoxingPrice(
                priceListInfo.TableName,
                order.Material,
                order.Dimensions
            );

            if (priceData != null)
            {
                _prezzoScatolatura = priceData.Price;
                _scontoScatolatura = priceData.Discount;
            }
        }

        private void CalculateMillingPrice(Order order, PriceListInfo priceListInfo)
        {
            // Calcola prezzo fresata
            var priceData = _priceRepository.GetMillingPrice(
                priceListInfo.TableName,
                order.Material,
                order.Dimensions
            );

            if (priceData != null)
            {
                _prezzoFresata = priceData.Price;
                _scontoFresata = priceData.Discount;
            }

            // Calcola anche fresata LED se presente
            if (order.ArticleTypes.Contains(ArticleType.FresataLED))
            {
                var ledPriceData = _priceRepository.GetLEDMillingPrice(
                    priceListInfo.TableName,
                    order.Material,
                    order.Dimensions
                );

                if (ledPriceData != null)
                {
                    _prezzoFresataLed = ledPriceData.Price;
                }
            }
        }

        private void CalculateBasinPrice(Order order, PriceListInfo priceListInfo)
        {
            // Calcola prezzo catino
            var priceData = _priceRepository.GetBasinPrice(
                priceListInfo.TableName,
                order.Stamp.StampCode,
                order.Material
            );

            if (priceData != null)
            {
                _prezzoCatino = priceData.Price;
            }
        }

        private void ApplyColorSurcharges(Order order, PriceResult result)
        {
            var surcharge = new PriceAdjustment { Category = "Colore" };

            // Determina maggiorazione colore
            if (!string.IsNullOrEmpty(order.RALCode))
            {
                if (order.RALCode.ToUpper().Contains("NERO") || order.RALCode == "9005")
                {
                    // Maggiorazione nero
                    if (order.MainArticleType == ArticleType.Catino)
                    {
                        surcharge.Percentage = order.Customer.BlackSurchargeCatini;
                        surcharge.Description = "Maggiorazione nero catini";
                    }
                    else
                    {
                        surcharge.Percentage = order.Customer.BlackSurcharge;
                        surcharge.Description = "Maggiorazione nero";
                    }
                }
                else
                {
                    // Maggiorazione colore standard
                    if (order.MainArticleType == ArticleType.Catino)
                    {
                        surcharge.Percentage = order.Customer.ColorSurchargeCatini;
                        surcharge.Description = "Maggiorazione colore catini";
                    }
                    else
                    {
                        surcharge.Percentage = order.Customer.ColorSurcharge;
                        surcharge.Description = "Maggiorazione colore";
                    }
                }

                if (surcharge.Percentage > 0)
                {
                    surcharge.Amount = CalculateBasePrice() * (surcharge.Percentage / 100);
                    result.Surcharges.Add(surcharge);
                }
            }
        }

        private void ApplyDepthSurcharges(Order order, PriceResult result)
        {
            // Maggiorazione per profondità extra
            if (order.Dimensions.Depth > order.Customer.DepthLimitForTVI && 
                order.Customer.MaxDepthSurcharge > 0)
            {
                var surcharge = new PriceAdjustment
                {
                    Category = "Profondità",
                    Description = $"Maggiorazione profondità > {order.Customer.DepthLimitForTVI}mm",
                    Percentage = order.Customer.MaxDepthSurcharge,
                    Amount = CalculateBasePrice() * (order.Customer.MaxDepthSurcharge / 100)
                };
                
                result.Surcharges.Add(surcharge);
            }
        }

        private void ApplyCustomerDiscounts(Order order, PriceResult result)
        {
            // Recupera sconti cliente
            var discounts = _discountRepository.GetCustomerDiscounts(order.CustomerId);
            
            foreach (var discount in discounts)
            {
                if (IsDiscountApplicable(discount, order))
                {
                    var adjustment = new PriceAdjustment
                    {
                        Category = "Sconto Cliente",
                        Description = discount.Description,
                        Percentage = -discount.Percentage, // Negativo perché è uno sconto
                        Amount = -(CalculateBasePrice() * (discount.Percentage / 100))
                    };
                    
                    result.Discounts.Add(adjustment);
                }
            }

            // Applica sconto generale se presente
            if (order.Customer.GeneralDiscount > 0)
            {
                var generalDiscount = new PriceAdjustment
                {
                    Category = "Sconto Generale",
                    Description = "Sconto generale cliente",
                    Percentage = -order.Customer.GeneralDiscount,
                    Amount = -(CalculateBasePrice() * (order.Customer.GeneralDiscount / 100))
                };
                
                result.Discounts.Add(generalDiscount);
            }
        }

        private bool IsDiscountApplicable(CustomerDiscount discount, Order order)
        {
            // Logica per determinare se uno sconto è applicabile
            // Basata su materiale, tipologia articolo, dimensioni, ecc.
            
            if (discount.Material.HasValue && discount.Material != order.Material)
                return false;
                
            if (discount.ArticleType.HasValue && discount.ArticleType != order.MainArticleType)
                return false;
                
            if (discount.MinDimension.HasValue)
            {
                var maxDim = Math.Max(order.Dimensions.Length, 
                                     Math.Max(order.Dimensions.Depth, order.Dimensions.Height));
                if (maxDim < discount.MinDimension)
                    return false;
            }
            
            return true;
        }

        private decimal CalculateBasePrice()
        {
            // Somma tutti i componenti di prezzo base
            return _prezzoFisso +
                   _prezzoIkon +
                   _prezzoModelloPlus +
                   _prezzoPianoIntegrato +
                   _prezzoPiattoDoccia +
                   _prezzoVascaDaBagno +
                   _prezzoLastra +
                   _prezzoScatolatura +
                   _prezzoFresata +
                   _prezzoFresataLed +
                   _prezzoCatino +
                   _prezzoConsolle +
                   _prezzoColonna +
                   _prezzoCassaInLegno +
                   _prezzoDima +
                   _prezzoSalvaGoccia +
                   _prezzoBordoContenitivo +
                   _prezzoAltro +
                   _costoEcomalta;
        }

        private decimal CalculateFinalPrice(PriceResult result)
        {
            var finalPrice = result.BasePrice;

            // Applica maggiorazioni
            foreach (var surcharge in result.Surcharges)
            {
                finalPrice += surcharge.Amount;
            }

            // Applica sconti
            foreach (var discount in result.Discounts)
            {
                finalPrice += discount.Amount; // Amount è già negativo
            }

            // Arrotonda se necessario
            return Math.Round(finalPrice, 2);
        }

        private void PopulateComponents(PriceResult result)
        {
            result.Components.IntegratedTop = _prezzoPianoIntegrato;
            result.Components.ShowerTray = _prezzoPiattoDoccia;
            result.Components.Bathtub = _prezzoVascaDaBagno;
            result.Components.Sheet = _prezzoLastra;
            result.Components.SheetPerSqm = _prezzoLastraMQ;
            result.Components.Boxing = _prezzoScatolatura;
            result.Components.Milling = _prezzoFresata;
            result.Components.LEDMilling = _prezzoFresataLed;
            result.Components.Basin = _prezzoCatino;
            result.Components.Console = _prezzoConsolle;
            result.Components.Column = _prezzoColonna;
            result.Components.WoodenBox = _prezzoCassaInLegno;
            result.Components.Template = _prezzoDima;
            result.Components.DripSaver = _prezzoSalvaGoccia;
            result.Components.ContainmentEdge = _prezzoBordoContenitivo;
            result.Components.Other = _prezzoAltro;
            result.Components.FixedPrice = _prezzoFisso;
            result.Components.IkonPrice = _prezzoIkon;
            result.Components.ModelPlusPrice = _prezzoModelloPlus;
        }

        private void ResetPrices()
        {
            _prezzoFisso = 0;
            _prezzoIkon = 0;
            _prezzoModelloPlus = 0;
            _prezzoPianoIntegrato = 0;
            _prezzoPiattoDoccia = 0;
            _prezzoVascaDaBagno = 0;
            _prezzoLastra = 0;
            _prezzoLastraMQ = 0;
            _prezzoScatolatura = 0;
            _prezzoFresata = 0;
            _prezzoFresataLed = 0;
            _prezzoCatino = 0;
            _prezzoConsolle = 0;
            _prezzoColonna = 0;
            _prezzoCassaInLegno = 0;
            _prezzoDima = 0;
            _prezzoSalvaGoccia = 0;
            _prezzoBordoContenitivo = 0;
            _prezzoAltro = 0;
            _scontoPianoIntegrato = 0;
            _scontoPiattoDoccia = 0;
            _scontoVascaDaBagno = 0;
            _scontoLastra = 0;
            _scontoScatolatura = 0;
            _scontoFresata = 0;
            _scontoCassaInLegno = 0;
            _scontoDima = 0;
            _prezzoMaggiorazioni = 0;
            _maggiorazioneTVI = 0;
            _costoEcomalta = 0;
        }

        /// <summary>
        /// Classe interna per informazioni sul listino prezzi
        /// </summary>
        private class PriceListInfo
        {
            public string Type { get; set; }
            public string TableName { get; set; }
            public bool IsCustom { get; set; }
        }
    }

    /// <summary>
    /// Modello per dati prezzo da database
    /// </summary>
    public class PriceData
    {
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
    }

    /// <summary>
    /// Modello per sconti cliente
    /// </summary>
    public class CustomerDiscount
    {
        public string Description { get; set; }
        public decimal Percentage { get; set; }
        public Material? Material { get; set; }
        public ArticleType? ArticleType { get; set; }
        public double? MinDimension { get; set; }
    }
}