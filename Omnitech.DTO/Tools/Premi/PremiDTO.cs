using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.Tools.Premi
{
    public class DTO
    {
        public class AllItems
        {
            public List<LineItem> TotaleFatturato { get; set; }
            public List<LineItem> AccreditiPerResiMerce { get; set; }
            public List<LineItem> Campioncini { get; set; }
            public List<LineItem> Trasporti { get; set; }
            public List<LineItem> ProgettazioneVenditaStampi { get; set; }
            public List<LineItem> ProgettazioneVenditaModelli { get; set; }
            public List<LineItem> ContributoCostruzioneStampiModelli { get; set; }
            public List<LineItem> OrdiniContract { get; set; }
            public List<LineItem> Promozioni { get; set; }
            public List<LineItem> CollezioneForm { get; set; }
        }

        public class LineItem
        {
            public string CustomerNr { get; set; }
            public string CustomerName { get; set; }
            public string DocumentNr { get; set; }
            public DateTime PostingDate { get; set; }
            public string PostingDescription { get; set; }
            public decimal Amount { get; set; }

            public short FapType { get; set; }
            public string FapDescription { get; set; }

            public short ItemType { get; set; }     // 1=Conto C/G, 2=Articolo
            public string ItemNr { get; set; }      // Valore (in funzione di ItemType)

            public string ItemDescription { get; set; }
        }

        
        public class CoordinateSomma
        {
            public string NomeCliente;
            public string CodiceCliente;

            public string SommaTotaleFatture;                                   //ci deve essere
            
            public string SommaNoteDiCreditoResoMerce = "0";                    //queste (con "0") sono componenti opzionali
            public string SommaCampioncini = "0";
            public string SommaTrasporti = "0"; 
            public string SommaProgettazioneVenditaStampi = "0";
            public string SommaProgettazioneVenditaModelli = "0"; 
            public string SommaContributiCostruzioneStampiModelli = "0"; 
            public string SommaContract = "0"; 
            public string SommaPromozioni = "0";                                //Non usate (per il momento)
            public string SommaForm = "0";

            public string Subtotale = "0";                      //Solo per Gruppo Idea e Rexa, valore utilizzato per il calcolo del coefficiente del premio. (Negli altri clienti è uguale al Totale)
            public string Totale = "0";                         //Valore usato ai fini del calcolo del premio
        }

        public class RegolaPremio
        {
            public RegolaPremio(decimal FatturatoMinimo, decimal? FatturatoMassimo, decimal Premio) 
            { 
                this.FatturatoMinimo = FatturatoMinimo;
                this.FatturatoMassimo = FatturatoMassimo;
                this.Premio = Premio;
            }

            public decimal FatturatoMinimo;
            public decimal? FatturatoMassimo;
            public decimal Premio;
        }
    }
}
