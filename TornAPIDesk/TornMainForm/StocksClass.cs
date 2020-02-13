using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TornMainForm
{
   public class StocksClass
    {
        public string NameOfStock { get; set; }
        public string AcronymOfStock { get; set; }
        public string SharesAvailable { get; set; }
        public long SharesAvailableNoFormat { get; set; }
        public double PriceOfStock { get; set; }
        public string Forecast { get; set; }
        public string Demand { get; set; }
        public bool WhenSharesAreZero { get; set; } // This value is for setting to 0 if ShareAvailable = 0. Then will be used to compare with
     //   future ShareAvailable to alert the user if new shares were added.
        

    }
}
