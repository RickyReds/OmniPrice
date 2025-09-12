using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnitech.DTO.BusinessCentral.Alexide;

namespace Omnitech.Prezzi.BusinessCentral
{
    public class Articoli
    {
        #region Metodi di calcolo prezzo (Parziali/Privati)

        private static decimal CalcolaPrezzoContributoDimensioni()
        {
            return 1;
        }

        private static decimal CalcolaPrezzoContributoCliente()
        {
            return 1;
        }

        private static decimal CalcolaPrezzoContributoXY()
        {
            return 1;
        }

        #endregion



        public static decimal CalcolaPrezzo(string ConnectionString, string CustomerNo, ConfigItem Item)
        {
            try
            {
                #region verifica input

                if (string.IsNullOrWhiteSpace(ConnectionString))
                    throw new Exception("'ConnectionString' non valida!");

                if (string.IsNullOrWhiteSpace(CustomerNo))
                    throw new Exception("'CustomerNo' non valido!");

                if (Item == null)
                    throw new Exception("'Item' non valido (nullo)!");

                #endregion


                decimal prezzoTotale = 0;

                #region Calcolo con Database

                // Ottieni il prezzo base dal database
                decimal prezzoBase = Omnitech.Database.BusinessCentral.Prezzi.DAL_Implementation.GetBasePrice(
                    ConnectionString, 
                    CustomerNo, 
                    Item.FamigliaProdotto
                );

                // Ottieni i fattori di prezzo dal database
                var fattori = Omnitech.Database.BusinessCentral.Prezzi.DAL_Implementation.GetPriceFactors(ConnectionString);

                // Applica i fattori di calcolo
                decimal moltiplicatore = 1.0m;
                decimal addendo = 0.0m;

                foreach (var fattore in fattori)
                {
                    switch (fattore.FactorType)
                    {
                        case "Dimensioni":
                            // Calcola contributo dimensioni basato su Item.Altezza, Item.Lunghezza, etc.
                            if (Item.Altezza > 0)
                                moltiplicatore *= (1 + (decimal)(Item.Altezza / 1000.0));
                            break;
                        case "Cliente":
                            // Il contributo cliente è già nel prezzo base
                            break;
                        case "Materiale":
                            // Calcola contributo materiale basato su Item.CodiceMateriale
                            if (!string.IsNullOrEmpty(Item.CodiceMateriale))
                                addendo += fattore.AdditiveValue;
                            break;
                        default:
                            moltiplicatore *= fattore.MultiplierValue;
                            addendo += fattore.AdditiveValue;
                            break;
                    }
                }

                // Calcola il prezzo finale
                prezzoTotale = (prezzoBase * moltiplicatore) + addendo;

                // Salva lo storico nel database
                Omnitech.Database.BusinessCentral.Prezzi.DAL_Implementation.SavePriceHistory(
                    ConnectionString,
                    CustomerNo,
                    prezzoTotale,
                    Item
                );

                #endregion

                #region Demo (commentato)
                /*
                decimal  p0, p1, p2;

                p0 = CalcolaPrezzoContributoCliente();
                p1 = CalcolaPrezzoContributoDimensioni();
                p2 = CalcolaPrezzoContributoXY();

                prezzoTotale = (p0 * p1) + p2;          //demo
                */
                #endregion

                return prezzoTotale;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }





    }
}
