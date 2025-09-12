using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.BusinessCentral.COMMON
{
    public static class BcTipoTroppoPieno
    {
        //NOTA: Verificare perchè originariamente il troppo pieno (overflow) era di tipo BcBool.....


        public const string Nessuno = "0";

        public const string Tondo = "30";
        public const string AsolatoSenzaGhiera = "33";
        public const string AsolatoConGhieraCromata = "31";
        public const string AsolatoConGhieraStinata = "34";
        public const string AsolatoConGhieraBianca = "35";

    }
}
