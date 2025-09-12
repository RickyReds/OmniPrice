using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.Etichette
{
    public class DtoEtichettaStandard
    {
        public int IdCliente { get; set; }
        public Enum.Psr.Cliente Cliente
        {
            get { return (Enum.Psr.Cliente)IdCliente; }
        }


        public string RagioneSociale { get; set; }

        public string DescrizioneArticolo { get; set; }                     //preformattata
        public string DescrizioneArticoloSoemaContract { get; set; }

        public string BarcodeCliente { get; set; }

        public DateTime? DataOrdine { get; set; }

        public string Riferimento { get; set; }
        public string RiferimentoCliente { get; set; }              //stringa con numero e data  (es: "219 | 05/02/24")

        [IgnoreDataMember]
        public string RiferimentoClienteNumero
        {
            get     //in PSR la parte splittata si chiama   RiferimentoOrdineClienteNumero
            {
                try
                {
                    return RiferimentoCliente.Split('|')[0].Trim();
                }
                catch
                {
                    return null;
                }
            }
        }
        [IgnoreDataMember]
        public DateTime? RiferimentoClienteData
        {
            get     //in PSR la parte splittata si chiama   RiferimentoOrdineClienteData
            {
                try
                {
                    DateTime ret;
                    if (DateTime.TryParseExact(RiferimentoCliente.Split('|')[1].Trim(), "dd/MM/yy", null, System.Globalization.DateTimeStyles.None, out ret))
                        return ret;
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
            }
        }


        public string RiferimentoEtichettaImab { get; set; }         //IMAB ha la sua logica;
        [IgnoreDataMember]
        public string RiferimentoEtichetta
        {
            get
            {
                string lbl = GetCustomLabelReference();

                if (lbl != null)
                {
                    string rif = (Riferimento != null) ? Riferimento.Trim() : null;
                    string rifOC = (GetRiferimentoOrdineCliente() != null) ? GetRiferimentoOrdineCliente().Trim() : null;

                    int a = Convert.ToInt32(!string.IsNullOrWhiteSpace(rif.Replace("_", string.Empty)));
                    int b = Convert.ToInt32(!string.IsNullOrWhiteSpace(rifOC.Replace("_", string.Empty)));

                    switch ($"{a}{b}")
                    {
                        case "00":
                        case "10":
                            return Riferimento;

                        case "01":
                            return GetRiferimentoOrdineCliente();

                        case "11":
                            return $"{Riferimento}  |  {GetRiferimentoOrdineCliente(string.Empty)}";
                    }

                    return null;
                }
                else
                    return lbl;
            }
        }

        public string Colore { get; set; }

        public string Collo { get; set; }                       //ARBLU ha il suo;  per gli altri arriva da ordini.Ordini

        public string Stampo { get; set; }                      //(da ordine.Stampo.Stampo in PSR)  In teoria non serve ma meglio avercelo....

        public bool Living { get; set; }

        public bool IsKanban { get; set; }
        public bool IsAutomatico { get; set; }                  //nella versione PSR, è true quando:   BarCode > 499999 And BarCode < 700000 e compreso tra [750000,800000)   <--- DA RIPENSARE!!!


        public string CodiceCliente { get; set; }               //in psr risulta "obsoleto".  E' ancora in uso?

        public double DimensioniL { get; set; }
        public double DimensioniP { get; set; }

        public string RAL { get; set; }                         //solo Disenia?
        public string RALDescrizione { get; set; }              //solo Disenia?
        public string RALColorePastaInTinta { get; set; }       //solo Disenia?

        public string CustomRALName { get; set; }               //da popolare sul client mediante GetCustomRALname(ByVal ordine As cOrdine, ByRef RALname As String)   (query 974)

        public string ClienteFinale { get; set; }


        public Enum.Psr.LabelLayout ProductLabelType { get; set; }


        public int IdMateriale { get; set; }
        public Omnitech.Enum.Psr.Materiale Materiale
        {
            get { return (Enum.Psr.Materiale)IdMateriale; }
        }

        public Enum.Psr.TipologiaArticolo TipologiaArticoloMaster { get; set; }
        public Enum.Psr.TipologiaImballo TipologiaImballo { get; set; }
        public Enum.Psr.TipologiaRinforzoImballo TipologiaRinforzoImballo { get; set; }

        public Enum.Psr.TipoFaP FaP { get; set; }                   //(da ordine.FaP in PSR)

        public DtoOrdineArblu OrdineArblu { get; set; }

        public bool ImportaNoteDaDisegno { get; set; }              //campo in   [clienti].[DescrizioneArticoli]   ---> governa la stampa dell'immagine etc. etc.

        #region Immagini / Loghi

        public string BaseLogoPath { get; set; }                    //tipicamente   \\10.0.0.2\produzione\LoghiClienti\
        public string BaseIconsPath { get; set; }                   //tipicamente   \\10.0.0.2\produzione\VersioniProgramma\Icos\
        public string BaseTemplateOrdiniSLPath { get; set; }        //tipicamente   \\10.0.0.2\produzione\TemplateOrdini_SL\

        public string OrderTemplateRelativeFilename { get; set; }

        public string OrderTemplate
        {
            get
            {
                return Path.Combine(BaseTemplateOrdiniSLPath, OrderTemplateRelativeFilename);
            }
        }

        public string LogoMaterialeFullFilename
        {
            get
            {
                return Path.Combine(BaseLogoPath, $"logo{Materiale.ToString()}.png");
            }
        }
        public string LogoClienteFullFilename
        {
            get
            {
                return Path.Combine(BaseLogoPath, $"logoCliente_{IdCliente.ToString().PadLeft(3, '0')}.png");
            }
        }
        public string LogoWareHouseFullFilename
        {
            get
            {
                switch (TipologiaArticoloMaster)
                {
                    case Enum.Psr.TipologiaArticolo.TopVascaIntegrata:
                    case Enum.Psr.TipologiaArticolo.TopVascaIntegrataSemincasso:
                    case Enum.Psr.TipologiaArticolo.Lavanderia:
                    case Enum.Psr.TipologiaArticolo.Monoblocco:
                    case Enum.Psr.TipologiaArticolo.Consolle:
                    case Enum.Psr.TipologiaArticolo.Lavamani:
                    case Enum.Psr.TipologiaArticolo.Mensola:
                    case Enum.Psr.TipologiaArticolo.LastraPiùVascaSaldata:
                    case Enum.Psr.TipologiaArticolo.Catino:
                    case Enum.Psr.TipologiaArticolo.Laminato:
                        {
                            switch (TipologiaImballo)
                            {
                                case Enum.Psr.TipologiaImballo.tviTipoA:
                                case Enum.Psr.TipologiaImballo.cartone:
                                    {
                                        switch (TipologiaRinforzoImballo)
                                        {
                                            case Enum.Psr.TipologiaRinforzoImballo.Gabbia:
                                                return (DimensioniL > 2000 - 90) ? Path.Combine(BaseLogoPath, $"ico_gabbia_oriz.png") : Path.Combine(BaseLogoPath, $"ico_gabbia_vert.png");

                                            default:
                                                return (DimensioniL > 1600) ? Path.Combine(BaseLogoPath, $"ico_top_xl.png") : Path.Combine(BaseLogoPath, $"ico_top_n.png");
                                        }
                                    }

                                case Enum.Psr.TipologiaImballo.undefined:
                                    return (DimensioniL > 1600) ? Path.Combine(BaseLogoPath, $"ico_top_xl.png") : Path.Combine(BaseLogoPath, $"ico_top_n.png");

                                default:
                                    return null;
                            }
                        }

                    case Enum.Psr.TipologiaArticolo.VascaBagno:
                        return Path.Combine(BaseLogoPath, $"ico_vdb.png");

                    case Enum.Psr.TipologiaArticolo.PiattoDoccia:
                        return Path.Combine(BaseLogoPath, $"ico_pdc.png");

                    case Enum.Psr.TipologiaArticolo.LavaboCostruito:
                    case Enum.Psr.TipologiaArticolo.Cassettiera:
                    case Enum.Psr.TipologiaArticolo.Colonna:
                    case Enum.Psr.TipologiaArticolo.Accessorio:
                    case Enum.Psr.TipologiaArticolo.Alzatina:
                    case Enum.Psr.TipologiaArticolo.Lastra:
                    case Enum.Psr.TipologiaArticolo.Boiserie:
                    case Enum.Psr.TipologiaArticolo.Tavoletta:
                    case Enum.Psr.TipologiaArticolo.Varie:
                    case Enum.Psr.TipologiaArticolo.Campioncini:
                    case Enum.Psr.TipologiaArticolo.Copripiletta:
                    case Enum.Psr.TipologiaArticolo.Tavolo:
                    default:
                        return null;
                }
            }
        }

        #endregion

        public List<DtoAccessorio> Accessori { get; set; }

        #region Funzioni varie

        private string GetRiferimentoOrdineCliente(string Separator = "|", bool Essenziale = false)
        {
            //in psr è usata "RiferimentoOrdineCliente" che è una proprità readonly coi parametri (in VB.net è possibile!)

            if ((string.IsNullOrWhiteSpace(RiferimentoClienteNumero) || RiferimentoClienteNumero == "_") && RiferimentoClienteData == null)
                return null;

            else if (string.IsNullOrWhiteSpace(RiferimentoClienteNumero) && RiferimentoClienteData != null)
                return $"| {RiferimentoClienteData.Value.ToString("dd/MM/yy")}";

            else if (RiferimentoClienteData == null)
                return RiferimentoClienteNumero;

            else
                return (Essenziale) ? $"{RiferimentoClienteNumero} {Separator} {RiferimentoClienteData.Value.ToString("dd/MM/yy")}" : $"Ord.{RiferimentoClienteNumero} {Separator} del {RiferimentoClienteData.Value.ToString("dd/MM/yy")}";
        }

        private string GetCustomLabelReference()
        {
            //ritorna la stringa oppure null!

            int a = Convert.ToInt32(!string.IsNullOrWhiteSpace(Riferimento));
            int b = Convert.ToInt32(!string.IsNullOrWhiteSpace(ClienteFinale));
            int c = Convert.ToInt32(!string.IsNullOrWhiteSpace(GetRiferimentoOrdineCliente()));
            int d = Convert.ToInt32(!string.IsNullOrWhiteSpace(Collo));

            switch (Cliente)
            {
                case Enum.Psr.Cliente.Arblu:
                    {
                        switch ($"{a}{d}")
                        {
                            case "00":
                                return null;

                            case "01":
                                return Collo;

                            case "10":
                                return Riferimento;

                            case "11":
                                return $"{Riferimento} - {Collo}";
                        }
                    }
                    break;

                case Enum.Psr.Cliente.Mobilcrab:
                    {
                        switch ($"{a}{b}{c}")
                        {
                            case "000":
                                return null;

                            case "001":
                                return $"Ord.{GetRiferimentoOrdineCliente(string.Empty)}";

                            case "010":
                                return ClienteFinale;

                            case "011":
                                return $"{ClienteFinale}  |  {GetRiferimentoOrdineCliente(string.Empty)}";

                            case "100":
                                return Riferimento;

                            case "101":
                                return $"{Riferimento}  |  {GetRiferimentoOrdineCliente(string.Empty)}";

                            case "110":
                                return $"{Riferimento}  |  {ClienteFinale}";

                            case "111":
                                return $"{Riferimento}  |  {ClienteFinale}{Environment.NewLine}{GetRiferimentoOrdineCliente()}";
                        }
                    }
                    break;

                default:
                    {
                        return null;
                    }

            }

            return null;

        }

        #endregion
    }
}
