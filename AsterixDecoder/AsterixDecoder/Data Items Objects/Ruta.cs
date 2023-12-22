using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsterixDecoder.Data_Items_Objects
{
    class Ruta
    {
        public string id { get; set; }
        public string indicativo { get; set; }
        public DateTime HoraDespegue { get; set; }
        public string RutaSACTA { get; set; }
        public string TipoAeronave { get;  set; }
        public string Estela { get; set; }
        public string ProcDesp { get; set; }
        public string PistaDesp { get; set; }

        public Ruta()
        {

        }
        public Ruta(string id, string indicativo, string HoraDespegue, string RutaSACTA, string TipoAeronave, string Estela, string ProcDesp, string PistaDesp)
        {
            if (PistaDesp == "LEBL-06R" || PistaDesp == "LEBL-24L")
            {
                this.id = id;
                this.indicativo = indicativo;
                this.HoraDespegue = ParseDateTime(HoraDespegue);
                this.RutaSACTA = RutaSACTA;
                this.TipoAeronave = TipoAeronave;
                this.Estela = Estela;
                this.ProcDesp = ProcDesp;
                this.PistaDesp = PistaDesp;
            }         

        }


        public DateTime ParseDateTime(string input)
        {
            string[] temps = input.Split(' ')[1].Split(':');
            string hora = temps[0];

            if (Convert.ToInt32(hora) < 10)
            {
                hora = "0" + hora;
            }

            string newInput = input.Split(' ')[0] + " " + hora + ":" + temps[1] + ":" + temps[2];
            
            // Define the possible formats
            string formats = "dd/MM/yyyy HH:mm:ss";

            // Parse the string using the specified formats
            DateTime dt = DateTime.ParseExact(newInput, formats, null);

            return dt;
        }


    }
}
