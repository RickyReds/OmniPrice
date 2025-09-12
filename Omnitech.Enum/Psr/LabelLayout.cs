using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.Psr
{
    public enum LabelLayout
    {
        Undefined = -1,
        
        NoLabel = 0,
        
        OldLabel = 1,                                                                                                           //clienti vecchi/inattivi
        
        [Description("Logo Omnitech + nostro logo Materiale")]                                                                  //*** CASO PIU' COMUNE ***
        OurLogos = 2,
        
        [Description("NO logo Omnitech e Testo/ SI Logo Materiale del Cliente")]
        CustomerLogos = 3,                                                                                                      // IDIStudio
    
        [Description("Arca, No RagioneSociale, NO logo Omnitech, NO BarCode Cliente")]
        Custom01 = 4,                                                                                                           // ARCA
    
        [Description("Punto 3, Nessun Riferimento Punto3, tutto il resto si")]
        Custom02 = 5,                                                                                                           // Punto 3
    
        [Description("Solo Riferimento")]
        Riferimento = 6,                                                                                                        // Agorà Mastella Synergie
    
        [Description("No Data Ordine, no Logo Omnitech")]
        NoDataOrdine = 7,                                                                                                       // ARCHEDA
    
        [Description("No RAL, No logo Materiale, No Logo Omnitech")]
        Rexa = 8,                                                                                                               // Rexa
    
        [Description("Se presente ragione sociale cliente finale, appare quella invece di Mobilcrab")]
        Mobilcrab = 9,                                                                                                          // Mobilcrab
    
        [Description("Riferimento semplice, senza ordine ... del ..., no logo cliente, barcode cliente spostato a dx")]
        Arcom = 10,                                                                                                             // Arcom
    
        [Description("Materiali suoi")]
        NomeMaterialeProprioTesto = 11,                                                                                         // Arbi' Milldue
    
        [Description("Materiali suoi")]
        Arblu = 12,                                                                                                             // Arblu
    }

}
