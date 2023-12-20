using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsterixDecoder.Data_Items_Objects
{
    class ACclassification
    {
        //LoA
        public Dictionary<string, string> classificationDictionary { get; set; }
        //Misma SID 06R
        public Dictionary<string, string> SIDinfoDictionary06R { get; set; }
        //Misma SID 
        public Dictionary<string, string> SIDinfoDictionary24L { get; set; }

        public ACclassification()
        {
            this.classificationDictionary = new Dictionary<string, string>();
            this.SIDinfoDictionary06R = new Dictionary<string, string>();
            this.SIDinfoDictionary24L = new Dictionary<string, string>();
        }


        public void AddModelClassification(string ACmodel, string classification)
        {
            this.classificationDictionary.Add(ACmodel, classification);
        }

        public string GetACclassification(string ACmodel)
        {
            return classificationDictionary[ACmodel];
        }

        //if bool misma == true -> misma SID
        //if bool misma == false -> distinta SID
        public int GetMinimumSeparation(string ACmodel, string sucesiva, bool misma)
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

        public int GetEstelaSeparation(string AC_Cat, string sucesiva)
        {
            int res = 0;
            switch (AC_Cat)
            {
                case "SH":
                    if (sucesiva == "H") { res = 6; }
                    else if (sucesiva == "M") { res = 7; }
                    else { res = 8; }
                    break;

                case "H":
                    if (sucesiva == "H") { res = 4; }
                    else if (sucesiva == "M") { res = 5; }
                    else { res = 6; }
                    break;

                case "M":
                    if (sucesiva == "L") { res = 5; }
                    break;

            }

            return res;
        }
    }
}
