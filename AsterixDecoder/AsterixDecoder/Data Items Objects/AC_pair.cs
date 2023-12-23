using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsterixDecoder.Data_Items_Objects
{
    class AC_pair
    {
        public string AC1 { get; set; }
        public string AC2 { get; set; }
        public DateTime t1 { get; set; }
        public DateTime t2 { get; set; }
        public double lat1 { get; set; }
        public double lon1 { get; set; }
        public double lat2 { get; set; }
        public double lon2 { get; set; }
        public double real_dist { get; set; }
        public double min_radar { get; set; }
        public double min_estela { get; set; }
        public double min_LoA { get; set; }

        //Incumplimientos
        public bool hayIncumplimiento { get; set; }
        public bool incumpleEstela { get; set; }
        public bool incumpleLoA { get; set; }
        public bool incumpleRadar { get; set; }

        public AC_pair()
        {

        }
        public AC_pair(string AC1, DateTime t1, double lat1, double long1, string AC2, DateTime t2, double lat2, double long2, double real_dist, double min_radar, double min_estela, double min_LoA)
        {
            this.AC1 = AC1;
            this.AC2 = AC2;
            this.t1 = t1;
            this.t2 = t2;
            this.lat1 = lat1;
            this.lat2 = lat2;
            this.lon1 = long1;
            this.lon2 = long2;
            this.real_dist = real_dist;
            this.min_radar = min_radar;
            this.min_estela = min_estela;
            this.min_LoA = min_LoA;

            if (real_dist >= min_radar)
            {
                this.incumpleRadar = false;
                this.hayIncumplimiento = false;
            }
            else
            {
                this.incumpleRadar = true;
                this.hayIncumplimiento = true;
            }
            if (real_dist >= min_estela)
            {
                this.incumpleEstela = false;
                this.hayIncumplimiento = false;
            }
            else
            {
                this.incumpleEstela = true;
                this.hayIncumplimiento = true;
            }
            if (real_dist >= min_LoA)
            {
                this.incumpleLoA = false;
                this.hayIncumplimiento = false;
            }
            else
            {
                this.incumpleLoA = true;
                this.hayIncumplimiento = true;
            }

        }

        public string getIncumplimientosString()
        {
            string res = "";
            if (incumpleEstela)
            {
                res = res + "Estela";
            }
            if (incumpleLoA)
            {
                res = res + "+LoA";
            }
            if (incumpleRadar)
            {
                res = res + "+Radar";
            }
            if (res == "")
            {
                return "Non";
            }
            else
            {
                string first = Convert.ToString(res.ToCharArray()[0]);
                if (first == "+")
                {
                    return res.Substring(1);
                }
                else
                {
                    return res;
                }

            }
        }

    }
}
