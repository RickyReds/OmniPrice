using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.BusinessCentral.COMMON
{
    public class Intavolato
    {   
        //public int IdLayout { get; set; }           //ID di riga (attualmente inutile)

        public int IdReport { get; set; }           //ID Report/Etichetta
        public string AreaName { get; set; }
        public string CustomerNo { get; set; }      //CustomerNo di BusinessCentral (..sta pure in PSR). NUll = Tutti

        public bool Skip {  get; set; }

        
        ////////////////////////////////////////

        public string Title { get; set; }
        public string FixedText { get; set; }
        public string FixedImage { get; set; }      //path RELATIVO dell'immagine!!!   -> Nella cartella "Misc"


        ////////////////////////////////////////



        public float? X { get; set; }
        public float? Y { get; set; }
        public float? Width { get; set; }
        public float? Height { get; set; }

        public char? HorizontalAlignment { get; set; }          //'L', 'C', 'R'  (Left, Center, Right)
        public char? VerticalAlignment { get; set; }            //'T', 'M', 'B'  (Top, Middle, Bottom)
        public char? TextCase { get; set; }                     //'U', 'L', null (Uppercase, Lowercase, normal)

        public float? FontMinSize { get; set; }
        public float? FontMaxSize { get; set; }
        public float? FontLeading { get; set; }
        public bool? FontBold { get; set; }
        public bool? FontItalic { get; set; }

        public string Border { get; set; }                  //"0111" --> Top,Right,Bottom,Left
        public float? BorderWidth { get; set; }


        public float? PaddingTop { get; set; }
        public float? PaddingBottom { get; set; }
        public float? PaddingLeft { get; set; }
        public float? PaddingRight { get; set; }

        public int? RotationDeegree { get; set; }

    }
}
