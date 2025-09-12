using Omnitech.Prezzi.Core.Enums;
using System;

namespace Omnitech.Prezzi.Core.Models
{
    /// <summary>
    /// Informazioni sul cliente
    /// </summary>
    public class CustomerInfo
    {
        /// <summary>
        /// ID del cliente
        /// </summary>
        public int CustomerId { get; set; }  // idCliente

        /// <summary>
        /// Ragione sociale del cliente
        /// </summary>
        public string CompanyName { get; set; }  // RagioneSociale

        /// <summary>
        /// Tipo di listino utilizzato (normale o metro lineare)
        /// </summary>
        public PriceListType PriceListType { get; set; }  // ListinoML

        /// <summary>
        /// Maggiorazione colore in percentuale
        /// </summary>
        public decimal ColorSurcharge { get; set; }  // MaggiorazioneColore

        /// <summary>
        /// Maggiorazione colore per catini in percentuale
        /// </summary>
        public decimal ColorSurchargeCatini { get; set; }  // MaggiorazioneColore_Catini

        /// <summary>
        /// Maggiorazione nero in percentuale
        /// </summary>
        public decimal BlackSurcharge { get; set; }  // MaggiorazioneNero

        /// <summary>
        /// Maggiorazione nero per catini in percentuale
        /// </summary>
        public decimal BlackSurchargeCatini { get; set; }  // MaggiorazioneNero_Catini

        /// <summary>
        /// Limite profondità per calcolo prezzo top vasca integrata
        /// </summary>
        public double DepthLimitForTVI { get; set; }  // LimiteProfonditàCalcoloPrezzoTopVascaIntegrata

        /// <summary>
        /// Maggiorazione profondità massima in percentuale
        /// </summary>
        public decimal MaxDepthSurcharge { get; set; }  // MaggiorazioneProfonditàMassima

        /// <summary>
        /// Sconto generale applicato al cliente
        /// </summary>
        public decimal GeneralDiscount { get; set; }

        /// <summary>
        /// Data di riferimento per aumenti
        /// </summary>
        public DateTime? PriceIncreaseDate { get; set; }  // Aumenti.DTordine

        /// <summary>
        /// Flag per aumenti 2022
        /// </summary>
        public bool Increases2022Flag { get; set; }  // Aumenti2k22_flag

        /// <summary>
        /// Verifica se il cliente ha un listino personalizzato
        /// </summary>
        public bool HasCustomPriceList { get; set; }  // ListinoPersonalizzato
    }
}