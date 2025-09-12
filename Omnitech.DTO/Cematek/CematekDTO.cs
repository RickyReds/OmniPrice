using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.Cematek
{
    public class CematekDTO
    {
        public CematekDTO() 
        {
            Lunghezza = Larghezza = Altezza = 0;

            Lav_1 = Lav_2 = Lav_3 = 0;

            Stato = 0;

            dtor_stato = DateTime.Now;
        }


        public string Posizione { get; set; }                   //posizione di ingresso. Serve a determinare se il collo è inserito dal traslatore "02" o dal "05" e verrà usata per cercare nella tabella le informazioni del collo (potrebbe essere "TR02" e "TR05").
        public string codice_univoco { get; set; }              //identificativo  (barcode?)


        public string Cod_articolo { get; set; }                //OPZIONALE (cod. Stampo)
        public string Descrizione { get; set; }                 //OPZIONALE (Cat. articolo)


        public int? Lunghezza { get; set; }                     //L (in mm)
        public int? Larghezza { get; set; }                     //P (in mm)
        public int? Altezza { get; set; }                       //H (in mm)   (Hmax / Ingombro?)


        public short? Lav_1 { get; set; }                       //(Lav1 / "Fondo")      0=lav. da non eseguire, 1=lav. da eseguire
        public string Cod_lav_1 { get; set; }                   //codice colore Lav1
        public int? Tmin_lav_1 { get; set; }                    //tempo minimo (in minuti) di permanenza in forno
        public int? Tmax_lav_1 { get; set; }                    //tempo massimo (in minuti) di permanenza in forno

        public short? Lav_2 { get; set; }                       //(Lav2 / "Colore")      0=lav. da non eseguire, 1=lav. da eseguire
        public string Cod_lav_2 { get; set; }                   //codice colore Lav2
        public int? Tmin_lav_2 { get; set; }                    //tempo minimo (in minuti) di permanenza in forno
        public int? Tmax_lav_2 { get; set; }                    //tempo massimo (in minuti) di permanenza in forno

        public short? Lav_3 { get; set; }                       //(Lav3 / "Finitura")      0=lav. da non eseguire, 1=lav. da eseguire
        public string Cod_lav_3 { get; set; }                   //codice colore Lav3
        public int? Tmin_lav_3 { get; set; }                    //tempo minimo (in minuti) di permanenza in forno
        public int? Tmax_lav_3 { get; set; }                    //tempo massimo (in minuti) di permanenza in forno

        
        public short? Stato {  get; set; }                      //      0=i dati non sono stati associati (=nuovo record), 1=i dati sono stati associati correttamente
        public DateTime? dtor_stato { get; set; }               //      orario di cambio dello stato


        public int? Rif_IDCollo { get; set; }                   // ????

    }
}
