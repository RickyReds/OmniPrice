using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Misc.Controllers
{
    public partial class PriceController
    {
        #region privati

        private static string LOG_FILE_NAME;        // = @"C:\inetpub\WebApiLog\Etichette\LogWebApiEtichette.txt";      //Autocrea anche la cartella, se non esiste.
        private static bool LOG_DEBUG_MODE;

        private static string CONNSTR_SETTINGS;
        private static string CONNSTR_PREZZI;       //TBD


        #endregion
    }
}