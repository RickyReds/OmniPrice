using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnitech.Enum.BusinessCentral.Dashboard
{
    public enum BcBool
    {
        [Description("No/False")]
        No = 0,

        [Description("Si/True")]
        Si = 1,

        [Description("Not Applicable")]
        NA = 2
    }
}
