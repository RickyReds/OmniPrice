using Omnitech.Prezzi.Core.Enums;
using System;
using System.Collections.Generic;

namespace Omnitech.Prezzi.Core.Models
{
    /// <summary>
    /// Ordine/Articolo da prezzare (equivalente a cOrdine in PsR)
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Barcode univoco dell'articolo
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// ID del cliente
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Informazioni complete del cliente
        /// </summary>
        public CustomerInfo Customer { get; set; }

        /// <summary>
        /// Materiale dell'articolo
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// Texture applicata
        /// </summary>
        public Texture? Texture { get; set; }

        /// <summary>
        /// Codice RAL del colore
        /// </summary>
        public string RALCode { get; set; }

        /// <summary>
        /// Dimensioni dell'articolo
        /// </summary>
        public Dimensions Dimensions { get; set; }

        /// <summary>
        /// Informazioni sullo stampo
        /// </summary>
        public Stamp Stamp { get; set; }

        /// <summary>
        /// Tipologia principale dell'articolo
        /// </summary>
        public ArticleType MainArticleType { get; set; }  // TipologiaArticoloMaster

        /// <summary>
        /// Lista di tipologie articolo applicate
        /// </summary>
        public List<ArticleType> ArticleTypes { get; set; }  // TipologiaArticolo

        /// <summary>
        /// Numero di vasche (per articoli multipli)
        /// </summary>
        public int NumberOfBaths { get; set; } = 1;  // NumeroVasche

        /// <summary>
        /// Flag per imballo robusto
        /// </summary>
        public bool RobustPackaging { get; set; }  // ImballoRobusto

        /// <summary>
        /// Flag per articoli contract
        /// </summary>
        public bool IsContract { get; set; }  // FaP = Contract

        /// <summary>
        /// Modello specifico
        /// </summary>
        public string Model { get; set; }  // Modello

        /// <summary>
        /// Data dell'ordine
        /// </summary>
        public DateTime OrderDate { get; set; }  // DataOrdine

        /// <summary>
        /// Prezzo già calcolato (se esiste)
        /// </summary>
        public decimal? ExistingPrice { get; set; }  // Prezzo

        /// <summary>
        /// Prezzo automatico calcolato
        /// </summary>
        public decimal AutomaticPrice { get; set; }  // PrezzoAutomatico

        /// <summary>
        /// Flag per articoli in Ecomalta
        /// </summary>
        public bool IsEcomalta { get; set; }  // Ecomalta

        /// <summary>
        /// Costo Ecomalta
        /// </summary>
        public decimal EcomaltaCost { get; set; }  // CostoEcomaltaPi4

        /// <summary>
        /// Flag per lastra più vasca
        /// </summary>
        public bool IsSheetPlusBath { get; set; }  // LastraPiuVasca

        /// <summary>
        /// Flag per cambio profondità
        /// </summary>
        public bool DepthChange { get; set; }  // CambioProfondità

        /// <summary>
        /// Codice cliente dell'articolo
        /// </summary>
        public string CustomerArticleCode { get; set; }  // CodiceArticoloCliente / CodiceCliente

        /// <summary>
        /// Numero ordine NAV
        /// </summary>
        public string NAVOrderNumber { get; set; }  // apOrderNumber

        /// <summary>
        /// Numero riga NAV
        /// </summary>
        public string NAVRowNumber { get; set; }  // apRowNumber

        /// <summary>
        /// Flag per invio a NAV
        /// </summary>
        public bool ToNAV { get; set; }  // toNAV

        /// <summary>
        /// Flag per articoli da magazzino
        /// </summary>
        public bool IsWarehouse { get; set; }  // Magazzino

        /// <summary>
        /// Verifica se l'ordine è valido per il calcolo prezzo
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Barcode)) return false;
            if (CustomerId <= 0) return false;
            if (Material == Material.Undefined) return false;
            if (Dimensions == null || !Dimensions.IsValid()) return false;
            if (Stamp == null || !Stamp.IsValid()) return false;
            
            return true;
        }
    }
}