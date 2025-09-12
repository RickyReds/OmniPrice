using System;
using System.Collections.Generic;

namespace Omnitech.Prezzi.Core.Models
{
    /// <summary>
    /// Risultato del calcolo prezzo
    /// </summary>
    public class PriceResult
    {
        /// <summary>
        /// Barcode dell'articolo
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Prezzo finale calcolato
        /// </summary>
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// Prezzo base prima di sconti e maggiorazioni
        /// </summary>
        public decimal BasePrice { get; set; }

        /// <summary>
        /// Dettaglio dei componenti di prezzo
        /// </summary>
        public PriceComponents Components { get; set; }

        /// <summary>
        /// Lista degli sconti applicati
        /// </summary>
        public List<PriceAdjustment> Discounts { get; set; }

        /// <summary>
        /// Lista delle maggiorazioni applicate
        /// </summary>
        public List<PriceAdjustment> Surcharges { get; set; }

        /// <summary>
        /// Flag per indicare se il calcolo è andato a buon fine
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Eventuali errori durante il calcolo
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Note aggiuntive sul calcolo
        /// </summary>
        public List<string> Notes { get; set; }

        /// <summary>
        /// Tipo di listino utilizzato
        /// </summary>
        public string PriceListType { get; set; }

        /// <summary>
        /// Data e ora del calcolo
        /// </summary>
        public DateTime CalculationDate { get; set; }

        public PriceResult()
        {
            Components = new PriceComponents();
            Discounts = new List<PriceAdjustment>();
            Surcharges = new List<PriceAdjustment>();
            Errors = new List<string>();
            Notes = new List<string>();
            CalculationDate = DateTime.Now;
        }
    }

    /// <summary>
    /// Componenti del prezzo
    /// </summary>
    public class PriceComponents
    {
        public decimal IntegratedTop { get; set; }  // prezzoPianoIntegrato
        public decimal ShowerTray { get; set; }  // prezzoPiattoDoccia
        public decimal Bathtub { get; set; }  // prezzoVascaDaBagno
        public decimal Sheet { get; set; }  // prezzoLastra
        public decimal SheetPerSqm { get; set; }  // prezzoLastraMQ
        public decimal Boxing { get; set; }  // prezzoScatolatura
        public decimal Milling { get; set; }  // prezzoFresata
        public decimal LEDMilling { get; set; }  // prezzoFresataLed
        public decimal Basin { get; set; }  // prezzoCatino
        public decimal Console { get; set; }  // prezzoConsolle
        public decimal Column { get; set; }  // prezzoColonna
        public decimal WoodenBox { get; set; }  // prezzoCassaInLegno
        public decimal Template { get; set; }  // prezzoDima
        public decimal DripSaver { get; set; }  // prezzoSalvaGoccia
        public decimal ContainmentEdge { get; set; }  // prezzoBordoContenitivo
        public decimal Other { get; set; }  // prezzoAltro
        public decimal FixedPrice { get; set; }  // prezzoFisso
        public decimal IkonPrice { get; set; }  // prezzoIkon
        public decimal ModelPlusPrice { get; set; }  // prezzoModelloPlus
    }

    /// <summary>
    /// Rappresenta uno sconto o maggiorazione applicata
    /// </summary>
    public class PriceAdjustment
    {
        /// <summary>
        /// Descrizione dell'aggiustamento
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Percentuale applicata (positiva per maggiorazioni, negativa per sconti)
        /// </summary>
        public decimal Percentage { get; set; }

        /// <summary>
        /// Valore in euro dell'aggiustamento
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Categoria dell'aggiustamento
        /// </summary>
        public string Category { get; set; }
    }
}