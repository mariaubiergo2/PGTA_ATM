using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiCAT6.Utils;


namespace AsterixDecoder.Data_Items_Objects
{
    class ACclassification
    {
        //LoA
        public Dictionary<string, string> classificationDictionary { get; set; }
        //Misma SID 06R
        public Dictionary<string, string> SIDDictionary { get; set; }
        public Dictionary<string, string> estelaDictionary { get; set; }

        //Per a utilitzar les funcions
        UsefulFunctions useful = new UsefulFunctions();
        GeoUtils geo = new GeoUtils();


        public ACclassification()
        {
            this.classificationDictionary = new Dictionary<string, string>();
            this.SIDDictionary = new Dictionary<string, string>();
            this.estelaDictionary = new Dictionary<string, string>();
        }

        public void AddModelClassification(string ACmodel, string classification)
        {
            this.classificationDictionary.Add(ACmodel, classification);
        }

        public string GetACclassification(string ACmodel)
        {
            return classificationDictionary[ACmodel];
        }

        public string GetACEstela(string AC_ID)
        {
            return classificationDictionary[AC_ID];
        }

        public string GetSIDGroup(string SID)
        {
            SID = useful.RemoveEnd(SID, 2);
            
            return SIDDictionary[SID];
        }


        public bool SameSID(string SID1, string SID2)
        {
            bool res; 
            if(SID1== SID2) { res = true; }
            else { res = false; }

            return res;
        }


        //if bool misma == true -> misma SID
        //if bool misma == false -> distinta SID
        public double GetMinimumSeparation(string ACmodel, string sucesiva, bool misma)
        {
            switch (ACmodel)
            {
                case "HP":
                    if (sucesiva == "HP" || sucesiva == "R" || sucesiva == "LP")
                    {
                        if (misma)
                        {
                            return 5;
                        }
                        else
                        {
                            return 3;
                        }
                    }
                    else
                    {
                        return 3;
                    }
                case "R":
                    if (sucesiva == "HP")
                    {
                        if (misma)
                        {
                            return 7;
                        }
                        else
                        {
                            return 5;
                        }
                    }
                    else if (sucesiva == "R" || sucesiva == "LP")
                    {
                        if (misma)
                        {
                            return 5;
                        }
                        else
                        {
                            return 3;
                        }
                    }
                    else
                    {
                        return 3;
                    }
                case "LP":
                    if (sucesiva == "HP")
                    {
                        if (misma)
                        {
                            return 8;
                        }
                        else
                        {
                            return 6;
                        }
                    }
                    else if (sucesiva == "R")
                    {
                        if (misma)
                        {
                            return 6;
                        }
                        else
                        {
                            return 4;
                        }
                    }
                    else if (sucesiva == "LP")
                    {
                        if (misma)
                        {
                            return 5;
                        }
                        else
                        {
                            return 3;
                        }
                    }
                    else
                    {
                        return 3;
                    }
                case "NR+":
                    if (sucesiva == "HP")
                    {
                        if (misma)
                        {
                            return 11;
                        }
                        else
                        {
                            return 8;
                        }
                    }
                    else if (sucesiva == "R" || sucesiva == "LP")
                    {
                        if (misma)
                        {
                            return 9;
                        }
                        else
                        {
                            return 6;
                        }
                    }
                    else if (sucesiva == "NR+")
                    {
                        if (misma)
                        {
                            return 5;
                        }
                        else
                        {
                            return 3;
                        }
                    }
                    else
                    {
                        return 3;
                    }
                case "NR-":
                    if (sucesiva == "NR")
                    {
                        return 3;
                    }
                    else if (sucesiva == "NR-")
                    {
                        if (misma)
                        {
                            return 5;
                        }
                        else
                        {
                            return 3;
                        }
                    }
                    else if (sucesiva == "NR+")
                    {
                        if (misma)
                        {
                            return 9;
                        }
                        else
                        {
                            return 6;
                        }
                    }
                    else
                    {
                        return 9;
                    }
                default: //NO REACTOR TAS < 160 KTS
                    if (sucesiva == "NR")
                    {
                        if (misma)
                        {
                            return 5;
                        }
                        else
                        {
                            return 3;
                        }
                    }
                    else
                    {
                        return 9;
                    }
            }
        }

        public void SetACpairs()
        {
            //Recórrer la llista de despegues

            //Trobar la hora a la que toca començar --> bucle recorrent els AC_INFO fins que un no aparegui més 
            //Creo de manera provisional les dos departures i els dos ACinfo que hem de buscar dins del bucle. 
            Ruta dep1 = new Ruta();
            Ruta dep2 = new Ruta();
            ACinfo aC1 = new ACinfo();
            ACinfo aC2 = new ACinfo();

            CoordinatesWGS84 geodesic_AC1 = new CoordinatesWGS84(aC1.Latitude * Math.PI / 180, aC1.Longitude * Math.PI / 180, aC1.Height);
            CoordinatesUVH stereo_AC1 = useful.GetStereographic(geodesic_AC1);

            CoordinatesWGS84 geodesic_AC2 = new CoordinatesWGS84(aC2.Latitude * Math.PI / 180, aC2.Longitude * Math.PI / 180, aC2.Height);
            CoordinatesUVH stereo_AC2 = useful.GetStereographic(geodesic_AC2);

            //Get the real distance between consecutive aircrafts
            double d_real= useful.ComputeRealDistances(stereo_AC1.U, stereo_AC1.V, stereo_AC2.U, stereo_AC2.V);

            //Get the required radar separation
            double min_radar = 3;

            //Get the required separation due to estela
            double min_estela = computeEstelaMinima(dep1.Estela, dep2.Estela);

            //Get the required separation due to LoA
            //First get the classification
            string class_AC1 = GetACclassification(dep1.TipoAeronave);
            string class_AC2 = GetACclassification(dep2.TipoAeronave);
            //Second get the SID and check if belong to same group
            string SID_AC1 = GetSIDGroup(dep1.ProcDesp);
            string SID_AC2 = GetSIDGroup(dep2.ProcDesp);
            bool same = SameSID(SID_AC1, SID_AC2);

            double min_LoA = GetMinimumSeparation(class_AC1, class_AC2, same);

            AC_pair aC_Pair = new AC_pair(aC1.I161_Tn, aC1.I140_ToD, aC1.Latitude, aC1.Longitude, aC2.I161_Tn, aC2.I140_ToD, aC2.Latitude, aC2.Longitude,d_real, min_radar, min_estela, min_LoA);

            //Repetir cada 4 segons fins que el primer AC desaparegui (passar a la següent parella de ACinfo)

            //Repetir per a la següent parella (departure +1)

        }

        public double computeEstelaMinima(string estela1, string sucesiva)
        {
            double minSeparation = 0.0; //Separacion minima
                                       
            switch (estela1)
            {
                case "Super Pesada":
                    if (sucesiva == "Pesada") { minSeparation = 6.0; }
                    else if (sucesiva == "Media") { minSeparation = 7.0; }
                    else { minSeparation = 8.0; }
                    break;

                case "Pesada":
                    if (sucesiva == "Pesada") { minSeparation = 4.0; }
                    else if (sucesiva == "Media") { minSeparation = 5.0; }
                    else { minSeparation = 6.0; }
                    break;

                case "Mediana":
                    if (sucesiva == "Ligera") { minSeparation = 5.0; }
                    break;
            }
           
            return minSeparation;
        }


    }
}
