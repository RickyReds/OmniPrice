using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.BusinessCentral.COMMON
{
    public static class BcMateriale
    {
        public const string Tecnoril = "1";
        
        public const string Geacril = "2";
        public const string GeacrilMatt = "3";
        public const string GeacrilBasic = "6";

        public const string Deimos = "4";
        public const string DeimosStone = "7";
        public const string DeimosBianco = "9";
        public const string DeimosMineral = "10";

        public const string SheerCrystal = "11";            //lucido
        public const string SheerCrystalMatt = "12";

        public const string Altro = "99";



        public static string GetMaterialName_Raw(string MaterialCode)
        {
            switch (MaterialCode)
            {
                case Tecnoril:
                    return "Tecnoril";

                case Geacril:
                    return "Geacril";

                case GeacrilMatt:
                    return "GeacrilMatt";

                case GeacrilBasic:
                    return "GeacrilBasic";

                case Deimos:
                    return "Deimos";

                case DeimosStone:
                    return "DeimosStone";

                case DeimosBianco:
                    return "DeimosBianco";

                case DeimosMineral:
                    return "DeimosMineral";

                case SheerCrystal:
                    return "SheerCrystal";

                case SheerCrystalMatt:
                    return "SheerCrystalMatt";

                case Altro:
                    return "Altro";

                default:
                    return "NA";
            }

            /*
            Type type = typeof(BcMateriale); // MyClass is static class with static properties
            foreach (var p in type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
            {
                var v = p.GetValue(null); // static classes cannot be instanced, so use null...
 
                                          //do something with v
                Console.WriteLine(v.ToString());
            }
            */

        }

        public static string GetFinitura(string MaterialCode)
        {
            switch (MaterialCode)
            {
                case Tecnoril:
                case Deimos:
                case DeimosStone:
                case GeacrilMatt:
                case DeimosBianco:
                case SheerCrystalMatt:
                    return "OPACO";

                case Geacril:
                case SheerCrystal:
                    return "LUCIDO";


                case GeacrilBasic:
                case DeimosMineral:
                default:
                    return "BASIC";
            }
        }
    }
}
