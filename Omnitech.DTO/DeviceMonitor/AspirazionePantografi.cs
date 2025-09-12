using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.DTO.DeviceMonitor
{
    public class AspirazionePantografi
    {
        public DateTime RecordDate { get; set; }                //per insert/lettura da dBase

        public float? DepressioneSetpoint { get; set; }         //(Pa)
        public float? DepressioneEffettiva { get; set; }        //(Pa)
        public float? VelocitaManSetpoint { get; set; }         //(%)
        public short? VelocitaEffettiva { get; set; }           //(%) 
        public short? Corrente {  get; set; }                   //(A)
        public short? Potenza { get; set; }                     //(%)
        public short? Frequenza { get; set; }                   //(Hz)
        public float? Vibrazioni { get; set; }                  //(mm/s)


    }
}
