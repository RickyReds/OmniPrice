namespace Omnitech.Prezzi.Core.Models
{
    /// <summary>
    /// Dimensioni dell'articolo
    /// </summary>
    public class Dimensions
    {
        /// <summary>
        /// Lunghezza in mm
        /// </summary>
        public double Length { get; set; }  // DimensioniL

        /// <summary>
        /// Profondità in mm
        /// </summary>
        public double Depth { get; set; }   // DimensioniP

        /// <summary>
        /// Altezza in mm
        /// </summary>
        public double Height { get; set; }  // DimensioniH

        /// <summary>
        /// Stringa formattata delle dimensioni
        /// </summary>
        public string FormattedDimensions => $"{Length}x{Depth}x{Height}";

        /// <summary>
        /// Verifica se le dimensioni sono valide
        /// </summary>
        public bool IsValid()
        {
            return Length > 0 && Depth > 0 && Height > 0;
        }
    }
}