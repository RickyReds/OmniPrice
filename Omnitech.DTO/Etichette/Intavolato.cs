using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.Etichette
{
    public class Intavolato
    {
        public string titleFontName { get; set; }
        public float titleFontDim { get; set; }
        public short titleFontStyle { get; set; }
        public string titleFontColor { get; set; }

        public float L { get; set; }
        public float H { get; set; }

        //////////////////////////////////////////////

        public string NomeArea { get; set; }
        public string Title { get; set; }

        public float? X { get; set; }
        public float? Y { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }
        public float xOffset { get; set; }
        public float yOffset { get; set; }
        public int borderWidth { get; set; }
        public string fontName { get; set; }
        public float? fontDim { get; set; }
        public short? fontStyle { get; set; }
        public string fontColor { get; set; }

        public string fontCase { get; set; }

        public short? titleAlignment { get; set; }
        public short? textAlignment { get; set; }
        public short? imageAlignment { get; set; }

        public bool ignore { get; set; }
        public string fillColor { get; set; }

        public int rotate { get; set; }



        /////////////////// PROPRIETA' AGGIUNTE ////////////////////////

        public string BorderSides { get; set; }             //Per evitare il doppio bordo in celle adiacenti



    }
}
