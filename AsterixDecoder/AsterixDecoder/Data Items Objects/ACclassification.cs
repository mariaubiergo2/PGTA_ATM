using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsterixDecoder.Data_Items_Objects
{
    class ACclassification
    {
        public Dictionary<string, string> classificationDictionary { get; set; }

        public ACclassification()
        {
            this.classificationDictionary = new Dictionary<string, string>();
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
        public int GetMinimumSeparation (string ACmodel, string sucesiva, bool misma)
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
    }
}
