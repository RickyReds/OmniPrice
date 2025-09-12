namespace Omnitech.Prezzi.Core.Models
{
    /// <summary>
    /// Informazioni sullo stampo utilizzato
    /// </summary>
    public class Stamp
    {
        /// <summary>
        /// ID dello stampo
        /// </summary>
        public int StampId { get; set; }  // idStampo

        /// <summary>
        /// Codice dello stampo
        /// </summary>
        public string StampCode { get; set; }  // Stampo

        /// <summary>
        /// ID dello stampo padre (se esiste)
        /// </summary>
        public int? ParentStampId { get; set; }  // idStampoPadre

        /// <summary>
        /// Dimensioni massime supportate dallo stampo
        /// </summary>
        public Dimensions MaxDimensions { get; set; }

        /// <summary>
        /// Categoria dell'articolo prodotto con questo stampo
        /// </summary>
        public string ArticleCategory { get; set; }

        /// <summary>
        /// Verifica se lo stampo è valido
        /// </summary>
        public bool IsValid()
        {
            return StampId > 0 && !string.IsNullOrEmpty(StampCode);
        }
    }
}