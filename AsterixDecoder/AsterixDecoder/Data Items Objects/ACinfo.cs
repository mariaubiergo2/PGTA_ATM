using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsterixDecoder.Data_Items_Objects
{
    class ACinfo
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public double Height { get; private set; }
        public double I140_ToD { get; private set; }
        public double I040_RHO { get; private set; }
        public double I040_THETA { get; private set; }
        public double I090_FL { get; private set; }
        public string I240_TId { get; private set; }
        public double I250_BDS40_BP { get; private set; }
        public double I250_BDS50_RA { get; private set; }
        public double I250_BDS50_TTA { get; private set; }
        public int I250_BDS50_GS { get; private set; }
        public double I250_BDS50_TAR { get; private set; }
        public int I250_BDS50_TAS { get; private set; }
        public double I250_BDS60_HDG { get; private set; }
        public int I250_BDS60_IAS { get; private set; }
        public int I250_BDS60_BAR { get; private set; }
        public int I250_BDS60_IVV { get; private set; }
        public int I161_Tn { get; private set; }
        public int I230_STAT { get; private set; }

        public ACinfo()
        {

        }       

        public ACinfo(string[] row)
        {
            try
            {
                double lat = GetDoubleValue(row[0]);
                double longi = GetDoubleValue(row[1]);
                if (40.9 < lat && lat < 41.7 && 1.5 < longi && longi < 2.6)
                {
                    this.Latitude = lat;
                    this.Longitude = longi;
                    this.Height = GetDoubleValue(row[2]);
                    this.I140_ToD = GetDoubleValue(row[3]);
                    this.I040_RHO = GetDoubleValue(row[4]);
                    this.I040_THETA = GetDoubleValue(row[5]);
                    this.I090_FL = GetDoubleValue(row[6]);
                    this.I240_TId = row[7];
                    this.I250_BDS40_BP = GetDoubleValue(row[8]);
                    this.I250_BDS50_RA = GetDoubleValue(row[9]);
                    this.I250_BDS50_TTA = GetDoubleValue(row[10]);
                    this.I250_BDS50_GS = GetIntValue(row[11]);
                    this.I250_BDS50_TAR = GetDoubleValue(row[12]);
                    this.I250_BDS50_TAS = GetIntValue(row[13]);
                    this.I250_BDS60_HDG = GetDoubleValue(row[14]);
                    this.I250_BDS60_IAS = GetIntValue(row[15]);
                    this.I250_BDS60_BAR = GetIntValue(row[16]);
                    this.I250_BDS60_IVV = GetIntValue(row[17]);
                    this.I161_Tn = GetIntValue(row[18]);
                    this.I230_STAT = GetIntValue(row[19]);
                }
            }
            catch
            {
                // Handle the exception
            }
        }

        private double GetDoubleValue(string value)
        {
            return double.TryParse(value, out double result) ? result : 0.0;
        }

        private int GetIntValue(string value)
        {
            return int.TryParse(value, out int result) ? result : 0;
        }


    }
}
