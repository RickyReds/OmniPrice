using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;

namespace WebApi.Misc.Controllers
{
    public partial class PriceController : ApiController
    {
        PriceController()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            LOG_DEBUG_MODE = (WebConfigurationManager.AppSettings["LogModeDebug"] == "true");
            LOG_DEBUG_MODE = true;
            LOG_FILE_NAME = WebConfigurationManager.AppSettings["LogFilename"];
#if DEBUG
            LOG_FILE_NAME = @"C:\WebApiLog\Price\LogWebApiPrice.txt";                       //Autocrea.
#endif

            CONNSTR_PREZZI = WebConfigurationManager.AppSettings["ConnectionStringPrezzi"];
            //CONNSTR_PREZZI = "Test_ConnectionString";    // WebConfigurationManager.AppSettings["ConnectionStringPrezzi"];

        }



        [Route("api/v1/price/evaluate/{CustomerNo}")]
        [HttpPost]
        public async Task<IHttpActionResult> PostPriceEvaluate([System.Web.Http.FromUri] string CustomerNo, Omnitech.DTO.BusinessCentral.Alexide.ConfigItem ItemDto)
        {
            string reqID = null;
            string clientIP = null;

            int step = 0;

            try
            {
                #region codice commentato
                //reqID = Guid.NewGuid().ToString();
                //clientIP = GetClientIp();

                //log.Information($"({reqID}) REQUEST START: {this.Request.RequestUri}      (Client IP: {clientIP})");
                //log.Information($"({reqID}) Json DTO: {await GetRawJson()}");
                //log.Debug("({@rID}) DTO: {@dto}", reqID, dto);

                //if (!ModelState.IsValid)
                //{
                //    log.Warning($"({reqID}) ModelState: Invalid data.");
                //    return BadRequest("Invalid data.");
                //}
                #endregion

                step = 10;

                await Task.Delay(10);

                //TBD

                step = 20;
                
                // TEST: Decommentare per testare FileLogger
                // HI.Libs.Utility.TestFileLogger.RunTest();

                string f = Omnitech.Database.BusinessCentral.Prezzi.SqlExecutor_Examples.SelectExamples.GetBC(CONNSTR_PREZZI);

                // Logging con FileLogger utility - gestisce automaticamente timestamp e rotazione
                HI.Libs.Utility.FileLogger.LogDebugWithTimestamp(LOG_FILE_NAME, 
                    $"CustomerNo: {CustomerNo}, ItemCategory: {ItemDto?.Categoria}, ItemHeight: {ItemDto?.Altezza}, BarcodeFetched: {f}", 
                    LOG_DEBUG_MODE);

                decimal prezzo = Omnitech.Prezzi.BusinessCentral.Articoli.CalcolaPrezzo(CONNSTR_PREZZI, CustomerNo, ItemDto);



                if (prezzo >= 0)
                    return Ok(prezzo);

                else
                    throw new Exception("Prezzo negativo!");

            }
            catch (Exception ex)
            {
                ////log.Error($"({reqID}) UNEXPECTED ERROR (Step: {step}): {ex.Message} {ex?.InnerException?.Message}");

                switch (step)
                {
                    case 0:
                        return Content(HttpStatusCode.InternalServerError, "Init error.");
                        break;

                    case 10:
                        return Content(HttpStatusCode.InternalServerError, "BLL error.");
                        break;

                    case 20:
                    default:
                        return Content(HttpStatusCode.InternalServerError, $"{ex.Message} {ex?.InnerException?.Message}");
                        break;
                }
            }
            finally
            {
                //log.Information($"({reqID}) REQUEST END.");
            }
        }




    }
}
