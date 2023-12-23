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
        public string airline { get; set; }
        public string clasificacion { get; set; }
        public string grupoSID { get; set; }
        public double minima_distancia { get; set; }


        //Incumplimientos
        public bool hayIncumplimientoDespegue { get; set; }
        public string incumplimientosList { get; set; }
        public bool incumpleEstelaDespegue { get; set; }
        public bool incumpleLoADespegue { get; set; }
        public bool incumpleRadarDespegue { get; set; }

        

        public Ruta()
        {

        }
        public Ruta(string id, string indicativo, string HoraDespegue, string RutaSACTA, string TipoAeronave, string Estela, string ProcDesp, string PistaDesp, string airline)
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
                this.airline = airline;
                this.minima_distancia = -1;
                this.incumplimientosList = "Non";
                this.hayIncumplimientoDespegue = false;
                this.incumpleEstelaDespegue = false;
                this.incumpleLoADespegue = false;
                this.incumpleRadarDespegue = false;
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

        public void setIncumplimentos (AC_pair pareja)
        {
            if (!this.hayIncumplimientoDespegue)
            {
                this.hayIncumplimientoDespegue = pareja.hayIncumplimiento;
            }
            if (this.hayIncumplimientoDespegue)
            {
                if (pareja.incumpleEstela)
                {
                    this.incumpleEstelaDespegue = true;
                }
                if (pareja.incumpleLoA)
                {
                    this.incumpleLoADespegue = true;
                }
                if (pareja.incumpleRadar)
                {
                    this.incumpleRadarDespegue = true;
                }

                

            }

        }

        public void setMinimaDistancia (double min)
        {
            if (this.minima_distancia < 0)
            {
                this.minima_distancia = min;
            }
            else
            {
                if (min < this.minima_distancia)
                {
                    this.minima_distancia = min;
                }
                
            }
            
        }

        public void setClassification(string clasi)
        {
            this.clasificacion = clasi;
        }

        public void setGroupSID(string grupi)
        {
            this.grupoSID = grupi;
        }
    }
}
