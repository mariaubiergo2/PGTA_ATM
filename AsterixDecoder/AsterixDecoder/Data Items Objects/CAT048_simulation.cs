using System;
using System.Text.RegularExpressions;

namespace AsterixDecoder
{
    class CAT048_simulation
    {

        //Variables that define the flight trajectory
        public DateTime UTCTime { get; set; }
        public string ground_speed { get; set; }
        public string Flight_Level { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string AC_ID { get; set; }
        public string TAS { get; set; }
        public string heading { get; set; }
        public string AC_Adress { get; set; }
        public string Mode_3A { get; set; }
        public string TYP { get; set; }
       

        public CAT048_simulation()
        {

        }

        

        
        public CAT048_simulation(CAT048 raw)
        {
            this.UTCTime = DateTime.ParseExact(raw.UTCTime, "HH:mm:ss:fff", null); ;

            this.Latitude = raw.Latitude;

            this.Longitude = raw.Longitude;

            if (raw.ModeC_corrected != " ")
            {
                this.Flight_Level = raw.ModeC_corrected;
            }
            else

            //if (this.Flight_Level == "")
            {
                if (raw.Flight_Level != "N/A")
                {
                    string fl = raw.Flight_Level;
                    float fl2 = float.Parse(fl) * 100;
                    this.Flight_Level = Convert.ToString(fl2);
                }
                else
                {
                    this.Flight_Level = "0.";
                }
            }

            this.ground_speed = raw.ground_speed;

            this.AC_ID = raw.AC_ID;

            this.TAS = raw.TAS;

            this.heading = raw.heading;

            this.AC_Adress = raw.AC_Adress;

            this.TYP = raw.TYP;

            this.Mode_3A = raw.Mode_3A;
        }

    }
}
