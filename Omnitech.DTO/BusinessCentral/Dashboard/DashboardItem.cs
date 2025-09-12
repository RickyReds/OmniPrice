using Omnitech.Enum.BusinessCentral.COMMON;
using Omnitech.Enum.BusinessCentral.Dashboard;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.BusinessCentral.Dashboard
{

    public class DashboardItem
    {
        public bool activesubcontractingord {  get; set; }              //Active Subcontracting Ord. (Ordine di conto lavoro attivo)
        public BcBool airpool { get; set; }                             //Air Pool
        public string assignedemployeefirstname { get; set; }           //Assigned Employee First Name (Assegnato a - Nome)
        public string assignedemployeelastname { get; set; }            //Assigned Employee Last Name (Assegnato a - Cognome)
        public string assignedemployeid { get; set; }                   //Assigned Employe ID (Assegnato a)
        public string barcode { get; set; }                             //BarCode CLIENTE
        public BcBool boxed { get; set; }                               //Boxed (Scatolato)
        public string capacitytypejournal { get; set; }                 //Capacity Type Journal (Tipo) --- [DUMMY-OPTION: Senza deserializzazione specifica]
        public BcBool chassis { get; set; }                             //Chassis (Telaio)
        public string code { get; set; }                                //Code (Nostro barcode interno, (9 cifre esadecimali))
        public string color { get; set; }                               //Color Code (Codice Colore)
        public string colorhex { get; set; }                            //Color HEX (Codice HEX del colore)
        public DateTime? confirmreceiptdate { get; set; }               //Confirm. receipt date (Data conferma ricezione)
        public BcBool containmentedge { get; set; }                     //Containment Edge (Bordo di contenimento)
        public string customerkanbancode { get; set; }                  //Customer Kanban Code (Codice Kanban cliente)
        public string customermodel { get; set; }                       //Customer Model (Modello Cliente) ---> Nome del modello che sarà stampato per il Cliente (se diverso dal nome modello di Omnitech)
        public bool customerwhsesupply { get; set; }                    //Customer Whse supply (Rifornimento magazzino cliente)
        public string custsurfacefinish { get; set; }                   //Cust. Surface Finish (Finitura superficiale cliente)
        public decimal depth { get; set; }                              //Depth (Spessore)
        public BcBool distributing { get; set; }                        //Distributing (Erogatore)
        public DateTime? documentdate { get; set; }                     //Document Date (Data inserimento ordine in BC)
        public DateTime? endingdatetime { get; set; }                   //Ending Date-Time (Data-Ora Fine)
        public string externaldocumentno { get; set; }                  //External Document No. (Nr. ordine cliente)
        public BcBool fap {  get; set; }                                //FaP
        public string finalcustomer { get; set; }                       //Final Customer (Cliente Finale)
        public decimal height { get; set; }                             //Height (Altezza = H)
        public string itemdescription { get; set; }                     //Item Description (Descrizione Articolo)
        public string itemno { get; set; }                              //Item No. (Codice articolo interno/omnitech??)
        public string itemreferenceno { get; set; }                     //Item Reference No. (Codice articolo Cliente)   -> i vecchi CodiceArticoloCliente e CodiceCliente, ora sono un tutt'uno???
        public BcBool jointed { get; set; }                             //Jointed (Giuntato)
        public decimal length { get; set; }                             //Length (Lunghezza = L)
        public BcLineStatus linestatus { get; set; }                    //Line Status (Stato riga)
        public BcBool littlewall { get; set; }                          //Little Wall (Muretto)
        public string materialcode1 { get; set; }                       //Material Code 1 (Codice Materiale 1)
        public string materialcode2 { get; set; }                       //Material Code 2 (Codice Materiale 2)
        public string materialcode3 { get; set; }                       //Material Code 3 (Codice Materiale 3)
        public string materialdesc1 { get; set; }                       //Material Desc 1 (Descrizione Materiale 1)
        public string materialdesc2 { get; set; }                       //Material Desc 2 (Descrizione Materiale 2)
        public string materialdesc3 { get; set; }                       //Material Desc 3 (Descrizione Materiale 3)
        public string mouldno1 { get; set; }                            //Mould No. 1 (Codice Stampo 1) (Es: "B441")
        public string mouldno2 { get; set; }                            //Mould No. 2 (Codice Stampo 2)
        public string mouldno3 { get; set; }                            //Mould No. 3 (Codice Stampo 3)
        public string omnitechmodelcode { get; set; }                   //Omnitech Model Code (Codice Modello Omnitech)                 
        public string omnitechmodeldescription { get; set; }            //Omnitech Model Description (Descrizione Modello Omnitech)     ---> Nome del Modello di Omnitech. Se lo stampo è specifico (=usato solo) per un Cliente, questo nome dovrebbe essere uguale a quello del campo "customermodel"
        public string operationdescription { get; set; }                //Operation Description (Descrizione Operazione)
        public string omtcustomercolordescription { get; set; }                 //nome colore cliente (transcodifica)                   public string TEMP_DescrizioneColoreCliente { get; set; }
        public string omtcustomermaterialdesc { get; set; }                     //nome materiale cliente (transcodifica)                public string TEMP_DescrizioneMaterialeCliente { get; set; } 
        public string operationno { get; set; }                         //Operation No. (Nr. Operazione)
        public DateTime? orderdate { get; set; }                        //Order Date (Data Ordine)
        public int orderlineno { get; set; }                            //Order Line No. (Nr. riga ordine)
        public string orderno { get; set; }                             //Order No. (Nr. Ordine, No OdV?) (Es: "ODV24-000001") 
        public string ordertype { get; set; }                           //Order Type (Tipo Ordine) --- [DUMMY-OPTION: Senza deserializzazione specifica]
        public BcBool overflow { get; set; }                            //Overflow (Troppo Pieno)

                        //NOTA BENE:            Manca (e/o verificare nome) del "Tipo di Troppo Pieno"  (dovrebbero esserci le proprità valore e descrizione come per la piletta/sifone)

        public string packagingtype { get; set; }                       //Packaging Type (Codice Tipologia Imballo)         --> Omnitech.Enum.BusinessCentral.COMMON.BcTipologiaImballo
        public BcBool painted { get; set; }                             //Painted (Colorato)
        
        //public DateTime? plannedshipmentdate { get; set; }            //Planned Shipment Date (Data consegna pianificata internamente?)
        public DateTime? planneddate { get; set; }                      //??????  al posto di plannedshipmentdate ????

        public DateTime? postingdate { get; set; }                      //Posting Date (Data Registrazione)
        public BcPriority priority { get; set; }                        //Priority (Priorità)
        public string prodorderno { get; set; }                         //Prod. Order No. (No ordine di produzione) (Es: "ODPR000013")
        public BcProdOrderStatus prodorderstatus { get; set; }          //Prod. Order Status (Stato ordine produzione)
        public string product {  get; set; }                            //Product (Prodotto)
        public string productionorderprogress { get; set; }             //Production Order Progress (Avanzamento Ordine di produzione, codice)
        public DateTime? promiseddate { get; set; }
        public int quantity { get; set; }                               //Quantity (Quantità)
        public string queuetype { get; set; }                           //Queue Type (Tipologia Piletta/Sifone codice)      --> Omnitech.Enum.BusinessCentral.COMMON.BcTipoPilettaSifone
        public string queuetypedesc { get; set; }                       //Queue Type Description (Tipologia Piletta/Sifone descrizione)
        public string reasoncode { get; set; }                          //Reason Code (Causale)
        public string reference { get; set; }                           //Reference (Riferimento)
        public DateTime? requesteddeliverydate { get; set; }            //Requested Delivery Date (Data consegna richiesta (dal Cliente?))
        public string salesmaterialcode { get; set; }                   //Sales Material Code (Codice materiale di vendita)
        public string selltocustomername { get; set; }                  //Sell-to Customer Name (Vendere a - Nome, Ragione Sociale (Nome del Cliente))
        public string selltocustomerno { get; set; }                    //Sell-to Customer No. (Vendere a - Nr. cliente, Customer No  (Es: "CI000007"))     --> Omnitech.Enum.BusinessCentral.COMMON.BcCustomer
        public BcSetupType setuptype { get; set; }                      //Setup Type (Tipo di installazione)  --> (sottopiano, soprapiano, semiincasso, etc.)
        public BcBool sheettub { get; set; }                            //Sheet+Tub (Lastra+Vasca) **** VERIFICARE
        public DateTime? shipmentdate { get; set; }                     //Shipment Date (Data Promessa Consegna, Data consegna effettiva?)
        public DateTime? startingdatetime { get; set; }                 //Starting Date-Time (Data-Ora Inizio)
        public BcBool template { get; set; }                            //Template (Dima)
        public BcBool treatment { get; set; }                           //Treatment (Trattamento)
        public int tubquantity { get; set; }                            //Tub Quantity (N° Vasche)               --> NOTA: NON c'è il riferimento al fatto che la vasca sia a SX, DX o CENTRO!!
        public string unitofmeasurecode { get; set; }                   //Unit of Measure Code ((codice) Unità di misura) (Es: "PZ")
        public decimal unitprice { get; set; }                          //Unit Price (Prezzo unitario)
        public decimal weight { get; set; }                             //Weight (Peso)     ----------> che unità di misura?
        public decimal width { get; set; }                              //Width (Larghezza = P)
        public BcBool woodencage { get; set; }                          //Wooden Cage (Gabbia di legno)








        /////////////// CAMPI che verranno inseriti nella DashBoard e che dovranno essere controllati

        public string productfamilycode { get; set; }                   //Product Family Code               ----> la vecchia CategoriaArticoloMaster (code)         --> Omnitech.Enum.BusinessCentral.COMMON.BcProducFamily
        public string productfamilydescription{ get; set; }              //Product Family Description        ----> la vecchia CategoriaArticoloMaster (descrizione)
        public string neutralcategorycode { get; set; }                 //Neutral Category Code             ----> la vecchia CategoriaArticolo (code)
        public string neutralcategorydescription { get; set; }           //Neutral Category Description      ----> la vecchia CategoriaArticolo (descrizione)



        ////////////// CAMPI che non hanno ancora un nome definitivo

        public string TEMP_overflowtype { get; set; }                           //troppo pieno (valore)
        public string TEMP_overflowdesc { get; set; }                           //troppo pieno (descrizione)

        public string TEMP_descrizioneArticoloCliente { get; set; }








        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Campi derivati (utilizzati per semplificare le condizioni valutate in imballo ed etichette)

        public string _Dimensioni
        {
            get
            {
                if (length > 0 && width > 0 && height > 0)
                    return $"{(int)length}x{(int)width}x{(int)height}";

                return string.Empty;
            }
        }

        public string _EtichettaCEType
        {
            get
            {
                if (selltocustomerno == BcCustomer.Oasis)        //
                    return EtichettaCEType.SoloBarcode;

                
                ///////////////////////////////////////////////////////////////

                
                switch (productfamilycode)
                {
                    case BcProductFamily.Colonna:
                    case BcProductFamily.Consolle:                      //in origine c'era una condizione anche "consolle scatolata"; ora dovrebbe essere già compresa.
                    case BcProductFamily.Lavamani:
                    case BcProductFamily.Monoblocco:
                    case BcProductFamily.TopConVascaIntegrata:          //in origine c'era una condizione anche "TopVascaIntegrataSemincasso"; ora dovrebbe essere già compresa.
                    case BcProductFamily.Catino:
                        return EtichettaCEType.Lavabo;

                    case BcProductFamily.VascaDaBagno:
                        return EtichettaCEType.VascaDaBagno;

                    default:
                        if (_IsPiattoDoccia)
                            return EtichettaCEType.PiattoDoccia;

                        else if (_IsLavanderia)                        //ora tutti gli stampi "ex-lavanderia" dovrebbero essere dei TopConVascaIntegrata. Questa proprietà è stata definita solo per comodità e sicurezza della traduzione del codice.
                            return EtichettaCEType.Lavabo;

                        //else LastraPiùVascaSaldata, LastraPiùVascaSaldataDoppia, LastraPiùVascaSaldataDoppiaScatolata, LastraPiùVascaSaldataScatolata

                        else if (selltocustomerno == BcCustomer.Azzurra)        //se la categoria non rientra nelle precedenti e si tratta di Azzurra --> Etichetta solo barcode
                            return EtichettaCEType.SoloBarcode;

                        else 
                            return EtichettaCEType.Nessuna;
                }
            }
        }

        public bool _HasSifone
        {
            get
            {
                var sifoni = new HashSet<string>()
                {
                    //NOTA BENE: Se ci sono altri sifoni, integrare la lista!!!

                    BcTipoPilettaSifone.SifoneConScaricoLibero,
                    BcTipoPilettaSifone.SifoneConScaricoLiberoInTinta,
                    BcTipoPilettaSifone.SifoneConScaricoLiberoInTintaConLogo,
                    BcTipoPilettaSifone.SifoneConClickerInTinta,
                    BcTipoPilettaSifone.SifoneConClickerInTintaConLogo,
                    BcTipoPilettaSifone.SifoneConScaricoLiberoConPilettoneInTinta,
                    BcTipoPilettaSifone.SifoneConClickerConPilettoneInTinta,
                    BcTipoPilettaSifone.SifoneOmpConDentelliConCover,
                    BcTipoPilettaSifone.SifoneOmpSenzaDentelliConCover,
                    BcTipoPilettaSifone.SifoneOmpSenzaDentelliConCoverInox,
                    BcTipoPilettaSifone.SifoneOmpSenzaDentelliConCoverInTinta,
                    BcTipoPilettaSifone.SifoneOmpAlbireoConCover,
                    BcTipoPilettaSifone.SifoneSacithConCover,
                    BcTipoPilettaSifone.SifoneRibassatoConCover,
                    BcTipoPilettaSifone.SifoneOmpRibassatoConCover,
                    BcTipoPilettaSifone.SifoneOmpRibassatoConCoverConLogo,
                    BcTipoPilettaSifone.SifoneOmpLgaConCover,
                    BcTipoPilettaSifone.SifoneOmpLgaConCoverConLogo,
                    BcTipoPilettaSifone.SifoneSilfraConCover,
                    BcTipoPilettaSifone.SifoneSilfraConCoverConLogo
                };

                return sifoni.Contains(queuetype);
            }
        }

        public bool _IsLavanderia
        {
            get
            {
                //NOTA: Quelle che in PSR erano lavanderie ora sono tutte lastra+vasca (top+vascaintegrata).
                //      Per sicurezza (e velocità di traduzione) impostiamo l'elenco di stampi che definivano una lavandria.

                //throw new NotImplementedException("Come si identifica una Lavanderia?");
                //return false;

                string[] arrLavanderie = { "B252", "B372", "B373", "B501", "B502", "B503", "B504", "B505", "B505-T", "B506", "B506-T", "B607", "B608", "B635" };

                return arrLavanderie.Contains(mouldno1);

            }
        }

        public bool _IsPiattoDoccia
        {
            get
            {
                return (productfamilycode == BcProductFamily.PiattoDoccia);         //basta questo?
            }
        }

        public bool _IsCSAOrder
        {
            get
            {
                throw new NotImplementedException("Ordine CSA?");
            }
        }

        public string _Modello                                            
        {
            get 
            { 
                if (!string.IsNullOrWhiteSpace(customermodel))
                    return customermodel.Trim();
                
                else if (omnitechmodeldescription != null)
                    return omnitechmodeldescription.Trim();

                else 
                    return string.Empty;
            }
        }

        public bool _StampaNomeMaterialeCliente
        {
            get
            {
                //proprietà simulata a partire da anagrafica.Clienti (aventi ProductLabelType=3 e cliente Attivo=1)

                switch (selltocustomerno)
                {
                    case BcCustomer.AdrianoRizzetto:
                    case BcCustomer.Agape:
                    //case BcCustomer.AlMoAlberghi:                       //inattivo?
                    case BcCustomer.Albatros:
                    case BcCustomer.Aqua:
                    //case BcCustomer.ArredamentiIvanTuani:               //inattivo?
                    case BcCustomer.Blob:
                    case BcCustomer.Disenia:
                    case BcCustomer.GranTour:
                    case BcCustomer.Idea:
                    case BcCustomer.IdiStudioSrl:
                    case BcCustomer.Kios:
                    case BcCustomer.LaDocciaSrl:
                    case BcCustomer.LicorDesign:
                    case BcCustomer.MastroFiore:
                    case BcCustomer.NorahDesignSrl:
                    case BcCustomer.ProgettoBagno:
                    case BcCustomer.RABArredobagno:
                    case BcCustomer.Rainbox:
                    case BcCustomer.RCR:
                    case BcCustomer.SachGroupSrl:
                    case BcCustomer.SocietaAgricolaBellavista:
                        return true;

                    default:
                        return false;
                }
            }
        }





    }
}
