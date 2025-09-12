using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Database.Tools.ShippableMonitor
{
    public class DTO
    {
        public class Order
        {
            public string BarCode { get; set; }
            public string RagioneSociale { get; set; }
            public string Stampo { get; set; }
            public string CategoriaArticolo { get; set; }
            
            public DateTime? DataOrdine {  get; set; }
            public DateTime? DataConsegnaRichiesta { get; set; }
            public DateTime? DataConsegnaDefinitiva { get; set; }

            public string LocazioneDiMagazzino { get; set; }
            public DateTime? DataVersamentoAMagazzino { get; set; }

            public string GiorniDiConsegna
            {
                get
                {
                    StringBuilder gg = new StringBuilder("-------");

                    if (string.IsNullOrWhiteSpace(idGiornoCarico))
                        return gg.ToString();

                    if (idGiornoCarico[0] == '1')
                        gg[0] = 'L';

                    if (idGiornoCarico[1] == '1')
                        gg[1] = 'M';

                    if (idGiornoCarico[2] == '1')
                        gg[2] = 'M';

                    if (idGiornoCarico[3] == '1')
                        gg[3] = 'G';

                    if (idGiornoCarico[4] == '1')
                        gg[4] = 'V';

                    return gg.ToString();
                }
            }

            public DateTime? PrimaDataSpedizionePossibile { get; set; }

            
            
            public string idGiornoCarico { get; set; }      //uso interno (perchè il valore del dBase fa un po schifo (ha i sabati valorizzati e non le domeniche))

        }
    }
}
