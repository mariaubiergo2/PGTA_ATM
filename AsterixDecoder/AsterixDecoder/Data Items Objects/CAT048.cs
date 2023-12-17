using System;
using System.Collections.Generic;
using System.Reflection;
using MultiCAT6.Utils;

namespace AsterixDecoder
{
    internal class CAT048
    {
        public UsefulFunctions useful_functions;
        public List<Byte> datablock;
        public int msg_length;
        public int datalength;

        public string NUM { get; set; }
        public string SAC { get; set; }
        public string SIC { get; set; }

        //Variables for UTC Time (140)
        public string UTCTime { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        // Variables for Target Report Descriptor (020) 
        public string TYP { get; set; }
        public string SIM { get; set; }
        public string RDP { get; set; }
        public string SPI { get; set; }
        public string RAB { get; set; }
        public string TST { get; set; }
        public string ME { get; set; }
        public string MI { get; set; }
        public string FOE_FRI { get; set; }

        //Measured Position in Polar Co-ordinates (040)
        public string RHO { get; set; }
        public string THETA { get; set; }

        //Mode-3/A Code in Octal Representation (070)
        public string Mode_3A { get; set; }

        public string V070 { get; set; }
        public string G070 { get; set; }
        public string L070 { get; set; }

        public string V090 { get; set; }
        public string G090 { get; set; }

        public string Flight_Level { get; set; }
        public string ModeC_corrected { get; set; }

        //Height Measured by a 3D Radar (110)
        public string Height_3D { get; set; }

        //Radar Plot Characteristics (130)
        public string SRL130 { get; set; }
        public string SRR130 { get; set; }
        public string SAM130 { get; set; }
        public string PRL130 { get; set; }
        public string PAM130 { get; set; }
        public string RPD130 { get; set; }
        public string APD130 { get; set; }

        //Variables for Track Number (161)
        public string Track_NUM { get; set; }

        // Variables for Calculated Track Velocity in Polar Co-ordinates (170)

        public string ground_speed { get; set; }
        public string heading { get; set; }

        // Variables for Track Status (170)
        public string CNF { get; set; }
        public string RAD { get; set; }
        public string DOU { get; set; }
        public string MAH { get; set; }
        public string CDM { get; set; }
        public string TRE { get; set; }
        public string GHO { get; set; }
        public string SUP { get; set; }
        public string TCC { get; set; }


        // Variables for Aircraft Address (220)
        public string AC_Adress { get; set; }

        //Variables for Aircraft Identification (240)
        public string AC_ID { get; set; }

        // Variables for Mode S MB Data (230)
        public string MODE_S { get; set; }
        // BDS 4.0
        public string MCPSTATUS { get; set; }
        public string MCP_ALT { get; set; }
        public string FMSSTATUS { get; set; }
        public string FMS_ALT { get; set; }
        public string BPSTATUS { get; set; }
        public string BP { get; set; }
        public string MODESTATUS { get; set; }
        public string VNAV { get; set; }
        public string ALTHOLD { get; set; }
        public string APP { get; set; }
        public string TARGETALT_STATUS { get; set; }
        public string TARGETALT_SOURCE { get; set; }

        // BDS 5.0
        public string RASTATUS { get; set; }
        public string RA { get; set; }
        public string TTASTATUS { get; set; }
        public string TTA { get; set; }
        public string GSSTATUS { get; set; }
        public string GS { get; set; }
        public string TARSTATUS { get; set; }
        public string TAR { get; set; }
        public string TASSTATUS { get; set; }
        public string TAS { get; set; }

        //BDS 6.0
        public string HDGSTATUS { get; set; }
        public string HDG { get; set; }
        public string IASSTATUS { get; set; }
        public string IAS { get; set; }
        public string MACHSTATUS { get; set; }
        public string MACH { get; set; }
        public string BARSTATUS { get; set; }
        public string BAR { get; set; }
        public string IVVSTATUS { get; set; }
        public string IVV { get; set; }

        //Variables for Position in Cartesian Coordinates
        public string X_component { get; set; }
        public string Y_component { get; set; }

        //Variables for communications/ACAS Capability and Flight Status 
        public string COM { get; set; }
        public string STAT { get; set; }
        public string MSSC { get; set; }
        public string ARC { get; set; }
        public string AIC { get; set; }
        public string B1A_230 { get; set; }
        public string B1B_230 { get; set; }


        public CAT048()
        {

        }

        public CAT048(List<byte> FSFEC, List<byte> datablock, int datalength, int num)
        {
            useful_functions = new UsefulFunctions();
            this.NUM = Convert.ToString(num);
            //Place for constructor 
            this.datablock = datablock;
            //The binary values of this FSPEC its gonna determine from the 1 to the 7th
            int[] FSFEC17 = useful_functions.Dec2BinLSBend(FSFEC[0], 8);
            this.datalength = datalength;

            if (FSFEC17[0] == 1) //010
            {
                //Data Source Identifier
                DI_010(); //OK
            }
            if (FSFEC17[1] == 1) //140 
            {
                //Time of Day
                DI_140(); //OK
            }
            if (FSFEC17[2] == 1) //020
            {
                //Target Report Descriptor
                DI_020(); //OK 
            }
            if (FSFEC17[3] == 1) //040
            {
                //Measured Position in Polar Co-ordinates
                DI_040(); //OK
            }
            if (FSFEC17[4] == 1)  //070
            {
                //Mode-3/A Code in Octal Representation
                DI_070(); //OK
            }
            if (FSFEC17[5] == 1) //090
            {
                //Flight level in binary representation
                DI_090(); //OK
            }
            if (FSFEC17[6] == 1) //130
            {
                //Radar Plot Characteristics
                DI_130(); //OK
            }
            if (FSFEC17[7] == 1) //The 8th position determines if it follows another FSPEC octet
            {

                int[] FSFEC814 = useful_functions.Dec2BinLSBend(FSFEC[1], 8);
                if (FSFEC814[0] == 1) //220
                {
                    //Aircraft Adress
                    DI_220(); //OK
                }
                if (FSFEC814[1] == 1) //240 
                {
                    //Aircraft identification
                    DI_240(); //OK
                }
                if (FSFEC814[2] == 1) //250
                {
                    //Mode S MB Data
                    DI_250(); //OK
                }

                get_LatLong(); //Compute Latitude and Longitude

                if (FSFEC814[3] == 1) //161
                {
                    //Track number
                    DI_161(); //OK? crec que li entren malament
                }
                if (FSFEC814[4] == 1)  //042
                {
                    //Calculated position in cartesian coordinates
                    DI_042(); // en els 60' no hi ha x/y component 
                }
                if (FSFEC814[5] == 1) //200
                {
                    //Calculated track velocity in polar coordinates
                    DI_200(); //SHA DE TESTEJAR
                }
                if (FSFEC814[6] == 1) //170
                {
                    // Track Status
                    DI_170(); //SHA DE TESTEJAR
                }
                if (FSFEC814[7] == 1) //The 15th position determines if it follows another FSPEC octet
                {
                    int[] FSFEC1521 = useful_functions.Dec2BinLSBend(FSFEC[2], 8);
                    if (FSFEC1521[0] == 1) //210
                    {
                        //Track Quality 
                        //Neglegted
                        this.datalength += 4;
                    }
                    if (FSFEC1521[1] == 1) //030 
                    {
                        //Warning/error conditions
                        //Neglegted (VARIABLE LENGTH)
                        ComputeVarLen();
                    }
                    if (FSFEC1521[2] == 1) //080
                    {
                        //Neglegted
                        this.datalength += 2;
                    }
                    if (FSFEC1521[3] == 1) //100
                    {
                        //Neglegted
                        this.datalength += 4;
                    }
                    if (FSFEC1521[4] == 1)  //110
                    {
                        //Height measured by a 3D-Radar
                        DI_110(); //SHA DE TESTEJAR
                    }
                    if (FSFEC1521[5] == 1) //120
                    {
                        //Neglegted (VARIABLE LENGTH)
                        ComputeVarLen();
                    }
                    if (FSFEC1521[6] == 1) //230
                    {
                        //Communications/ACAS Capability and Flight Status
                        DI_230();
                    }
                    if (FSFEC1521[7] == 1)
                    {
                        int[] FSFEC2228 = useful_functions.Dec2BinLSBend(FSFEC[3], 8);
                        if (FSFEC2228[0] == 1) //260
                        {
                            //Neglegted
                            this.datalength += 7;
                        }
                        if (FSFEC2228[1] == 1) //055 
                        {
                            //Neglegted 
                            this.datalength += 1;

                        }
                        if (FSFEC2228[2] == 1) //050
                        {
                            //Neglegted
                            this.datalength += 2;
                        }
                        if (FSFEC2228[3] == 1) //065
                        {
                            //Neglegted
                            this.datalength += 1;
                        }
                        if (FSFEC2228[4] == 1)  //060
                        {
                            //Neglegted
                            this.datalength += 2;
                        }
                        if (FSFEC2228[5] == 1) //SP-Data Item
                        {

                        }
                        if (FSFEC2228[6] == 1) //SP-Data Item
                        {

                        }

                    }
                }

            }

            ReplaceNullPropertiesWithNA();
        }

        // Method to replace null properties with "N/A"
        public void ReplaceNullPropertiesWithNA()
        {
            Type cat048Type = this.GetType();
            PropertyInfo[] properties = cat048Type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    object propertyValue = property.GetValue(this);
                    if (propertyValue == null)
                    {
                        property.SetValue(this, "N/A");
                    }
                }
            }
        }


        public void ComputeVarLen()
        {
            int octet = this.datablock[this.datalength];
            this.datalength += 1;

            if (octet % 2 != 0)
            {
                this.datalength += 1;
            }
        }

        //Code 010 
        public void DI_010()
        {
            this.SAC = Convert.ToString(datablock[datalength]); //System Area Code
            this.SIC = Convert.ToString(datablock[datalength + 1]); //System Identification Code
            this.datalength = this.datalength + 2;
        }

        //Code 020
        public void DI_020()
        {
            int[] octet1 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);

            //DECODIFICATION OF ITEM TYPE
            if (octet1[0] == 0)
            {
                if (octet1[1] == 0)
                {
                    if (octet1[2] == 0)
                    {
                        //000
                        this.TYP = "No detection";
                    }
                    else
                    {   //001
                        this.TYP = "Single PSR detection";
                    }
                }
                else
                {
                    if (octet1[2] == 0)
                    {
                        //010
                        this.TYP = "Single SSR detection";
                    }
                    else
                    {
                        //011
                        this.TYP = "SSR+PSR detection";
                    }
                }
            }
            else
            {
                if (octet1[1] == 0)
                {
                    if (octet1[2] == 0)
                    {
                        //100
                        this.TYP = "Single ModeS All-Call";
                    }
                    else
                    {
                        //101
                        this.TYP = "Single ModeS Roll-Call";
                    }
                }
                else
                {
                    if (octet1[2] == 0)
                    {
                        //110
                        this.TYP = "ModeS All-Call + PSR";
                    }
                    else
                    {
                        //111
                        this.TYP = "ModeS Roll-Call + PSR";
                    }
                }

            }

            //DECODIFICATION OF ITEM SIM
            if (octet1[3] == 0)
            {
                this.SIM = "Actual target report";
            }
            else
            {
                this.SIM = "Simulated target report";
            }

            //DECODIFICATION OF ITEM RDP
            if (octet1[4] == 0)
            {
                this.RDP = "Report from RDP Chain 1";
            }
            else
            {
                this.RDP = "Report from RDP Chain 2";
            }

            //DECODIFICATION OF ITEM SPI
            if (octet1[5] == 0)
            {
                this.SPI = "Abscense of SPI";
            }
            else
            {
                this.SPI = "Special Position Identification";
            }

            //DECODIFICATION OF ITEM RAB
            if (octet1[6] == 0)
            {
                this.RAB = "Report from aircraft transponder";
            }
            else
            {
                this.RAB = "Report from field monitor";
            }

            //EXTENDED DATA ITEM
            if (octet1[7] == 0)
            {
                this.datalength += 1;
            }
            else
            {
                int[] octet2 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 1], 8);

                //DECODIFICATION OF ITEM TST
                if (octet2[0] == 0)
                {
                    this.TST = "Real target report";
                }
                else
                {
                    this.TST = "Test target report";
                }

                //DECODIFICATION OF ITEM ME
                if (octet2[3] == 0)
                {
                    this.ME = "No military emergency";
                }
                else
                {
                    this.ME = "Military emergency";
                }

                //DECODIFICATION OF ITEM MI 
                if (octet2[4] == 0)
                {
                    this.MI = "No military ID";
                }
                else
                {
                    this.MI = "Military ID";
                }

                //DECODIFICATION OF ITEM FOE/FRI
                if (octet2[5] == 0)
                {
                    if (octet2[6] == 0)
                    {
                        //00
                        this.FOE_FRI = "No Mode 4 Interrogation";
                    }
                    else
                    {
                        //01
                        this.FOE_FRI = "Friendly target";
                    }
                }
                else
                {
                    if (octet2[6] == 0)
                    {
                        //10
                        this.FOE_FRI = "Unknown target";
                    }
                    else
                    {
                        //11
                        this.FOE_FRI = "No reply";
                    }
                }

                //EXTENDED MODE 
                if (octet2[7] == 0)
                {
                    //END OF DATA ITEM
                }
                else
                {
                    //EXTENSION INTO NEXT EXTENT
                }

                this.datalength += 2;
            }
        }

        //Code 040
        private void DI_040()
        {
            //RHO is the 2 first octets
            int RHO = this.datablock[this.datalength] << 8 | this.datablock[this.datalength + 1];
            //Theta is the 3rd and 4th octet
            int Theta = this.datablock[this.datalength + 2] << 8 | this.datablock[this.datalength + 3];
            //RHO max range = 256 NM
            double rho_value = Math.Round((Convert.ToDouble(RHO) / 256), 4);
            //Theta range = 360º
            double theta_value = Math.Round((Theta * 360 / Math.Pow(2, 16)), 4);

            this.RHO = Convert.ToString(rho_value);
            this.THETA = Convert.ToString(theta_value);

            this.datalength += 4;
        }

        //Code 042
        public void DI_042()
        {
            int xcom = this.datablock[this.datalength] << 8 | this.datablock[this.datalength + 1];
            int ycom = this.datablock[this.datalength + 2] << 8 | this.datablock[this.datalength + 3];

            int[] x = useful_functions.Dec2BinLSBend(xcom, 16);
            int[] y = useful_functions.Dec2BinLSBend(ycom, 16);

            int x2 = useful_functions.BinComplementArray2DecLSBend(x, 0, x.Length - 1);
            int y2 = useful_functions.BinComplementArray2DecLSBend(y, 0, y.Length - 1);


            this.X_component = Convert.ToString(x2 * 0.0078125);
            this.Y_component = Convert.ToString(y2 * 0.0078125);

            this.datalength += 4;
        }

        private void DI_070()
        {
            int octets = this.datablock[this.datalength] << 8 | this.datablock[this.datalength + 1];
            int[] binary = useful_functions.Dec2BinLSBend(octets, 16);

            // Bit 16 usually 0
            this.V070 = useful_functions.validatedOrNot(binary[0]);

            // Bit 15 is 1 when error correction
            this.G070 = useful_functions.garbledOrDefault(binary[1]);

            // Bit 14
            this.L070 = "Not extacted";
            if (binary[2] == 0)
            {
                this.L070 = "Derived";
            }

            // Bit 13 is set to 0

            int num = useful_functions.BinArray2DecLSBend(binary, 3, binary.Length - 1);
            this.Mode_3A = useful_functions.Dec2Octal(num);


            this.datalength += 2;
        }

        private void DI_090()
        {
            int octets = (this.datablock[this.datalength] << 8 | this.datablock[this.datalength + 1]);
            int[] binary = useful_functions.Dec2BinLSBend(octets, 16);

            // Bit 16 usually 0
            this.V090 = useful_functions.validatedOrNot(binary[0]);

            // Bit 15 is 1 when error correction
            this.G090 = useful_functions.garbledOrDefault(binary[1]);


            int num = useful_functions.BinComplementArray2DecLSBend(binary, 2, binary.Length - 1);

            this.Flight_Level = Convert.ToString(num * 0.25);

            this.datalength += 2;
        }

        public void DI_110()
        {
            int octets = (this.datablock[this.datalength] << 8 | this.datablock[this.datalength + 1]);
            int[] binary = useful_functions.Dec2BinLSBend(octets, 16);
            int num = useful_functions.BinArray2DecLSBend(binary, 2, binary.Length - 1);
            int sign = 1;

            if (binary[2] == 1)
            {
                int[] complement = useful_functions.Bin2Complementary(binary);
                num = useful_functions.BinArray2DecLSBend(complement, 2, binary.Length - 1);
                sign = -1;
            }

            this.Height_3D = Convert.ToString(sign * num * 25) + " ft";

            this.datalength += 2;

        }

        private void DI_130()
        {
            //1st octet defines if there are or not the corresponding subfields
            int[] octet1st = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);
            this.datalength += 1;

            List<Action> functions = new List<Action>();
            functions.Add(DI_130_SRL);
            functions.Add(DI_130_SRR);
            functions.Add(DI_130_SAM);
            functions.Add(DI_130_PRL);
            functions.Add(DI_130_PAM);
            functions.Add(DI_130_RPD);
            functions.Add(DI_130_APD);

            int i = 0;
            foreach (Action func in functions)
            {
                if (octet1st[i] == 1)
                {
                    func();
                }
                i++;
            }

        }

        private void DI_130_SRL()
        {
            int[] octet = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);
            decimal num = useful_functions.BinArray2DecLSBend(octet, 0, octet.Length - 1);
            decimal resol = Convert.ToDecimal(Math.Pow(2, 13));
            decimal SRL = num * 360.0m / resol;
            //decimal SRL = num * 0.044m;
            this.SRL130 = Convert.ToString(SRL);

            this.datalength += 1;
        }

        private void DI_130_SRR()
        {
            int[] octet = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);
            int num = useful_functions.BinArray2DecLSBend(octet, 0, octet.Length - 1);

            this.SRR130 = Convert.ToString(num);

            this.datalength += 1;
        }

        private void DI_130_SAM()
        {
            int[] octet = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);
            int num = useful_functions.BinComplementArray2DecLSBend(octet, 0, octet.Length - 1);

            this.SAM130 = Convert.ToString(num) + " dBm";

            this.datalength += 1;
        }

        private void DI_130_PRL()
        {
            int[] octet = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);
            int num = useful_functions.BinArray2DecLSBend(octet, 0, octet.Length - 1);
            double val = Math.Round(num * 360 / Math.Pow(2, 13), 3);
            //double val = num * 0.044;
            this.PRL130 = Convert.ToString(val);

            this.datalength += 1;
        }

        private void DI_130_PAM()
        {
            int[] octet = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);
            int num = useful_functions.BinArray2DecLSBend(octet, 0, octet.Length - 1);

            this.PAM130 = Convert.ToString(num) + " dBm";

            this.datalength += 1;
        }
        private void DI_130_RPD()
        {
            int[] octet = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);
            int num = useful_functions.BinArray2DecLSBend(octet, 0, octet.Length - 1);
            double val = Math.Round(Convert.ToDouble(num * 0.00390625), 3);

            this.RPD130 = Convert.ToString(val) + " NM";

            this.datalength += 1;
        }
        private void DI_130_APD()
        {
            int[] octet = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);
            int num = useful_functions.BinComplementArray2DecLSBend(octet, 0, octet.Length - 1);
            double val = Math.Round(num * 360 / Math.Pow(2, 14), 3);

            this.APD130 = Convert.ToString(val) + " dg";

            this.datalength += 1;
        }
        private void DI_130_FX()
        {
            this.datalength += 1;
        }

        //Code 140
        public void DI_140()
        {
            int octets = (this.datablock[this.datalength] << 8 | this.datablock[this.datalength + 1]) << 8 | this.datablock[this.datalength + 2];
            double t = octets * Math.Pow(2, -7);

            //Using predetermined functions. 
            TimeSpan time = TimeSpan.FromSeconds(t);
            this.UTCTime = time.ToString(@"hh\:mm\:ss\:fff");

            this.datalength += 3;
        }

        //Code 161
        public void DI_161()
        {

            int octets = (this.datablock[this.datalength] << 8 | this.datablock[this.datalength + 1]);
            int[] binary = useful_functions.Dec2BinLSBend(octets, 16);

            int num = useful_functions.BinArray2DecLSBend(binary, 4, binary.Length - 1);

            this.Track_NUM = Convert.ToString(num);

            this.datalength += 2;

        }

        //Code 170
        public void DI_170()
        {
            int[] octet = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);

            // Confirmed vs tentative track
            if (octet[0] == 0)
            {
                this.CNF = "Confirmed track";
            }
            else
            {
                this.CNF = "Tentative track";
            }
            //Type of Sensors maintaining track
            if (octet[1] == 0)
            {
                if (octet[2] == 0)
                {
                    this.RAD = "Combined track";
                }
                else
                {
                    this.RAD = "PSR Track";
                }
            }
            else
            {
                if (octet[2] == 0)
                {
                    this.RAD = "SSR/Mode S track";
                }
                else
                {
                    this.RAD = "Invalid";
                }
            }

            //Signals level of confidence in plot to track association process
            if (octet[3] == 0)
            {
                this.DOU = "Normal confidence";

            }
            else
            {
                this.DOU = "Low confidence";
            }

            //Manoeuvre detection in Horizontal Sense
            if (octet[4] == 0)
            {
                this.MAH = "No horizontal man.sensed";

            }
            else
            {
                this.MAH = "Horizontal man.sensed";

            }

            //Climbing/descending mode
            if (octet[5] == 0)
            {
                if (octet[6] == 0)
                {
                    this.CDM = "Mantaining";
                }
                else
                {
                    this.CDM = "Climbing";
                }
            }
            else
            {
                if (octet[6] == 0)
                {
                    this.CDM = "Descending";
                }
                else
                {
                    this.CDM = "Invalid";
                }
            }

            if (octet[7] == 0)
            {
                this.datalength += 1;
            }
            else
            {
                int[] octet2 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 1], 8);

                // Signal for End_of_track
                if (octet2[0] == 0)
                {
                    this.TRE = "Track still alive";
                }
                else
                {
                    this.TRE = "End of track lifetime";
                }

                //Ghost vs true target
                if (octet2[1] == 0)
                {
                    this.GHO = "True target track";
                }
                else
                {
                    this.GHO = "Ghost target track";
                }

                //Track mantained with track information from neighbouring Node B on the cluster, or network 
                if (octet2[2] == 0)
                {
                    this.SUP = "No";
                }
                else
                {
                    this.SUP = "Yes";
                }

                //Type of plot coordinate transformation mechanism 
                if (octet2[3] == 0)
                {
                    this.TCC = "Radar plane";
                }
                else
                {
                    this.TCC = "2D reference plane";
                }

                this.datalength += 2;
            }


        }


        //Code 200
        public void DI_200()
        {
            int octets1 = this.datablock[this.datalength] << 8 | this.datablock[this.datalength + 1];
            int octets2 = this.datablock[this.datalength + 2] << 8 | this.datablock[this.datalength + 3];

            int[] binary = useful_functions.Dec2BinLSBend(octets2, 16);

            int num = useful_functions.BinArray2DecLSBend(binary, 0, binary.Length - 1);

            this.ground_speed = Convert.ToString(octets1 * 0.22);
            this.heading = Convert.ToString(Math.Round((num * 360 / Math.Pow(2, 16)), 4));

            this.datalength += 4;

        }


        //Code 220
        public void DI_220()
        {
            int[] octet1 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);
            int[] octet2 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 1], 8);
            int[] octet3 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 2], 8);

            string id1 = useful_functions.Bin2Hex(octet1[0], octet1[1], octet1[2], octet1[3]);
            string id2 = useful_functions.Bin2Hex(octet1[4], octet1[5], octet1[6], octet1[7]);
            string id3 = useful_functions.Bin2Hex(octet2[0], octet2[1], octet2[2], octet2[3]);
            string id4 = useful_functions.Bin2Hex(octet2[4], octet2[5], octet2[6], octet2[7]);
            string id5 = useful_functions.Bin2Hex(octet3[0], octet3[1], octet3[2], octet3[3]);
            string id6 = useful_functions.Bin2Hex(octet3[4], octet3[5], octet3[6], octet3[7]);

            this.AC_Adress = id1 + id2 + id3 + id4 + id5 + id6;

            this.datalength += 3;

        }

        //Code 240
        public void DI_240()
        {
            int[] octet1 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);
            int[] octet2 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 1], 8);
            int[] octet3 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 2], 8);
            int[] octet4 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 3], 8);
            int[] octet5 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 4], 8);
            int[] octet6 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 5], 8);

            string ch1 = useful_functions.BinArray2ASCII(octet1[0], octet1[1], octet1[2], octet1[3], octet1[4], octet1[5]);
            string ch2 = useful_functions.BinArray2ASCII(octet1[6], octet1[7], octet2[0], octet2[1], octet2[2], octet2[3]);
            string ch3 = useful_functions.BinArray2ASCII(octet2[4], octet2[5], octet2[6], octet2[7], octet3[0], octet3[1]);
            string ch4 = useful_functions.BinArray2ASCII(octet3[2], octet3[3], octet3[4], octet3[5], octet3[6], octet3[7]);
            string ch5 = useful_functions.BinArray2ASCII(octet4[0], octet4[1], octet4[2], octet4[3], octet4[4], octet4[5]);
            string ch6 = useful_functions.BinArray2ASCII(octet4[6], octet4[7], octet5[0], octet5[1], octet5[2], octet5[3]);
            string ch7 = useful_functions.BinArray2ASCII(octet5[4], octet5[5], octet5[6], octet5[7], octet6[0], octet6[1]);
            string ch8 = useful_functions.BinArray2ASCII(octet6[2], octet6[3], octet6[4], octet6[5], octet6[6], octet6[7]);

            this.AC_ID = ch1 + ch2 + ch3 + ch4 + ch5 + ch6 + ch7 + ch8;

            this.datalength += 6;

        }

        //Code 250
        public void DI_250()
        {
            int octetREP = this.datablock[this.datalength];
            this.datalength += 1;
            string[] modeS = new string[octetREP];

            int i = 0;
            while (i < octetREP)
            {
                int[] last_octet = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 7], 8);

                string BDS1 = useful_functions.Bin2Hex(last_octet[0], last_octet[1], last_octet[2], last_octet[3]);
                string BDS2 = useful_functions.Bin2Hex(last_octet[4], last_octet[5], last_octet[6], last_octet[7]);

                modeS[i] = "BDS:" + BDS1 + "," + BDS2;


                if ((BDS1 == "4" || BDS1 == "5" || BDS1 == "6") && BDS2 == "0")
                {
                    int[] oct1 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength], 8);
                    int[] oct2 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 1], 8);
                    int[] oct3 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 2], 8);
                    int[] oct4 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 3], 8);
                    int[] oct5 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 4], 8);
                    int[] oct6 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 5], 8);
                    int[] oct7 = useful_functions.Dec2BinLSBend(this.datablock[this.datalength + 6], 8);

                    if (BDS1 == "4") { BDS_40(oct1, oct2, oct3, oct4, oct5, oct6, oct7); }
                    else if (BDS1 == "5") { BDS_50(oct1, oct2, oct3, oct4, oct5, oct6, oct7); }
                    else if (BDS1 == "6") { BDS_60(oct1, oct2, oct3, oct4, oct5, oct6, oct7); }

                }

                this.datalength += 8;
                i++;

            }

            this.MODE_S = string.Join(" - ", modeS);

        }

        //Code 230
        public void DI_230()
        {
            int octets = this.datablock[this.datalength] << 8 | this.datablock[this.datalength + 1];
            int[] binary = useful_functions.Dec2BinLSBend(octets, 16);

            int comm = useful_functions.BinArray2DecLSBend(binary, 0, 3);


            string[] options = {"No communications capability, survillance only",
                "Communications A and B capability",
                "Communications A and B Uplink ELM",
                "Communications A and B Uplink ELM and Downlink ELM",
                "Level 5 Transponder capability 5 to 7 not assigned",
                "Not assigned",
                "Not assigned",
                "Not assigned"
            };
            this.COM = options[comm];

            int status = useful_functions.BinArray2DecLSBend(binary, 3, 3);

            string[] options2 = {"No alert, no SPI, aircraft airbone",
                "No alert, no SPI, aircraft on ground",
                "Alert, no SPI, aircraft airbone",
                "Alert, no SPI, aircraft on ground",
                "Alert, SPI, aircraft airbone or on ground",
                "No alert, SPI, aircraft airbone or on ground",
                "Not assigned",
                "Not assigned"
            };
            this.STAT = options2[status];

            this.MSSC = useful_functions.returnYesOrNo(binary[8]);

            if (binary[9] == 0)
            {
                this.ARC = "100ft resolution";
            }
            else
            {
                this.ARC = "25 ft resolution";
            }

            this.AIC = useful_functions.returnYesOrNo(binary[10]);

            this.B1A_230 = "BDS 1,0 bit 16: " + Convert.ToString(binary[11]);
            this.B1B_230 = "BDS 1,0 bits 37/40: " + Convert.ToString(binary[12]) + Convert.ToString(binary[13]) + Convert.ToString(binary[14]) + Convert.ToString(binary[15]);
            this.datalength += 2;
        }

        public void BDS_40(int[] oct1, int[] oct2, int[] oct3, int[] oct4, int[] oct5, int[] oct6, int[] oct7)
        {
            this.MCPSTATUS = Convert.ToString(oct1[0]);

            //Computation of MCP/FCU altitude
            int[] alt = { oct1[1], oct1[2], oct1[3], oct1[4], oct1[5], oct1[6], oct1[7], oct2[0], oct2[1], oct2[2], oct2[3], oct2[4] };
            int altMcp = useful_functions.BinArray2DecLSBend(alt, 0, 11) * 16;
            this.MCP_ALT = Convert.ToString(altMcp);

            this.FMSSTATUS = Convert.ToString(oct2[5]);

            //Computation of FMS altitude
            int[] alt2 = { oct2[6], oct2[7], oct3[0], oct3[1], oct3[2], oct3[3], oct3[4], oct3[5], oct3[6], oct3[7], oct4[0], oct4[1] };
            int altFms = useful_functions.BinArray2DecLSBend(alt2, 0, 11) * 16;
            this.FMS_ALT = Convert.ToString(altFms);

            this.BPSTATUS = Convert.ToString(oct4[2]);

            //Computation of Barometric pressure
            int[] pres = { oct4[3], oct4[4], oct4[5], oct4[6], oct4[7], oct5[0], oct5[1], oct5[2], oct5[3], oct5[4], oct5[5], oct5[6] };
            double presBar = useful_functions.BinArray2DecLSBend(pres, 0, 11) * 0.1 + 800;
            this.BP = Convert.ToString(presBar);

            //Mode of MCP/FCU status
            this.MODESTATUS = Convert.ToString(oct6[7]);
            this.VNAV = Convert.ToString(oct7[0]);
            this.ALTHOLD = Convert.ToString(oct7[1]);
            this.APP = Convert.ToString(oct7[2]);

            //Target altitude
            this.TARGETALT_STATUS = Convert.ToString(oct7[5]);
            if (oct7[6] == 0)
            {
                if (oct7[7] == 0) { this.TARGETALT_SOURCE = "Unknown"; }
                else { this.TARGETALT_SOURCE = "Aircraft altitude"; }
            }
            else
            {
                if (oct7[7] == 0) { this.TARGETALT_SOURCE = "MCP selected altitude"; }
                else { this.TARGETALT_SOURCE = "FMS selected altitude"; }
            }

        }

        public void BDS_50(int[] oct1, int[] oct2, int[] oct3, int[] oct4, int[] oct5, int[] oct6, int[] oct7)
        {
            //Roll angle calculation 			
            this.RASTATUS = Convert.ToString(oct1[0]);
            int sign = 1;
            if (oct1[1] == 1) { sign = -1; }
            int[] RAs = { oct1[1], oct1[2], oct1[3], oct1[4], oct1[5], oct1[6], oct1[7], oct2[0], oct2[1], oct2[2] };
            int[] compl1 = useful_functions.Bin2Complementary(RAs);
            decimal resol = 45.0m / 256.0m;
            decimal ra = Math.Round(Convert.ToDecimal(useful_functions.BinArray2DecLSBend(compl1, 0, 9)) * resol, 3);
            this.RA = Convert.ToString(sign * ra);

            //True track angle calculation 
            this.TTASTATUS = Convert.ToString(oct2[3]);
            sign = 1;
            if (oct2[4] == 1) { sign = -1; }
            int[] TTAs = { oct2[4], oct2[5], oct2[6], oct2[7], oct3[0], oct3[1], oct3[2], oct3[3], oct3[4], oct3[5], oct3[6] };
            int[] compl2 = useful_functions.Bin2Complementary(TTAs);
            decimal resol2 = 90.0m / 512.0m;
            decimal tta = Math.Round(Convert.ToDecimal(useful_functions.BinArray2DecLSBend(compl2, 0, 10)) * resol2, 3);
            this.TTA = Convert.ToString(sign * tta);

            //Groundspeed calculation --> REVISAR, FA ALGO RARO 
            this.GSSTATUS = Convert.ToString(oct3[7]);
            int[] GSs = { oct4[0], oct4[1], oct4[2], oct4[3], oct4[4], oct4[5], oct4[6], oct4[7], oct5[0], oct5[1] };
            int gs = useful_functions.BinArray2DecLSBend(GSs, 0, 9) * 2;
            this.GS = Convert.ToString(gs);

            //Track angle rate 
            this.TARSTATUS = Convert.ToString(oct5[2]);
            sign = 1;
            if (oct5[3] == 1) { sign = -1; }
            int[] TARs = { oct5[3], oct5[4], oct5[5], oct5[6], oct5[7], oct6[0], oct6[1], oct6[2], oct6[3], oct6[4] };
            int[] compl3 = useful_functions.Bin2Complementary(TARs);
            decimal resol3 = 8.0m / 256.0m;
            decimal tar = Math.Round(Convert.ToDecimal(useful_functions.BinArray2DecLSBend(compl3, 0, 9)) * resol3, 3);
            this.TAR = Convert.ToString(sign * tar);

            //True Airspeed calculation
            this.TASSTATUS = Convert.ToString(oct6[5]);
            int[] TASs = { oct6[6], oct6[7], oct7[0], oct7[1], oct7[2], oct7[3], oct7[4], oct7[5], oct7[6], oct7[7] };
            int tas = useful_functions.BinArray2DecLSBend(TASs, 0, 9) * 2;
            this.TAS = Convert.ToString(tas);

        }

        public void BDS_60(int[] oct1, int[] oct2, int[] oct3, int[] oct4, int[] oct5, int[] oct6, int[] oct7)
        {
            //Magnetic heading calculation 
            this.HDGSTATUS = Convert.ToString(oct1[0]);
            int sign = 1;
            if (oct1[1] == 1) { sign = -1; }
            int[] HDGs = { oct1[1], oct1[2], oct1[3], oct1[4], oct1[5], oct1[6], oct1[7], oct2[0], oct2[1], oct2[2], oct2[3] };
            int[] compl1 = useful_functions.Bin2Complementary(HDGs);
            decimal resol1 = 90.0m / 512.0m;
            decimal hdg = Math.Round(Convert.ToDecimal(useful_functions.BinArray2DecLSBend(compl1, 0, 10) * resol1), 3);
            this.HDG = Convert.ToString(sign * hdg);

            //Indicated Airspeed calculation
            this.IASSTATUS = Convert.ToString(oct2[4]);
            int[] IASs = { oct2[5], oct2[6], oct2[7], oct3[0], oct3[1], oct3[2], oct3[3], oct3[4], oct3[5], oct3[6] };
            int ias = useful_functions.BinArray2DecLSBend(IASs, 0, 9) * 1;
            this.IAS = Convert.ToString(ias);

            //Mach calculation
            this.MACH = Convert.ToString(oct3[7]);
            int[] MACHs = { oct4[0], oct4[1], oct4[2], oct4[3], oct4[4], oct4[5], oct4[6], oct4[7], oct5[0], oct5[1] };
            decimal resol2 = 2.048m / 512.0m;
            decimal mach = useful_functions.BinArray2DecLSBend(MACHs, 0, 9) * resol2;
            this.MACH = Convert.ToString(mach);

            //Barometric altitude rate calculation
            this.BARSTATUS = Convert.ToString(oct5[2]);
            sign = 1;
            if (oct5[3] == 1) { sign = -1; }
            int[] BARs = { oct5[3], oct5[4], oct5[5], oct5[6], oct5[7], oct6[0], oct6[1], oct6[2], oct6[3], oct6[4] };
            int[] compl2 = useful_functions.Bin2Complementary(BARs);
            int bar = useful_functions.BinArray2DecLSBend(compl2, 0, 9) * 32;
            this.BAR = Convert.ToString(sign * bar);

            //Inertial vertical velocity calculation
            this.IVVSTATUS = Convert.ToString(oct6[5]);
            sign = 1;
            if (oct6[6] == 1) { sign = -1; }
            int[] IVVs = { oct6[6], oct6[7], oct7[0], oct7[1], oct7[2], oct7[3], oct7[4], oct7[5], oct7[6], oct7[7] };
            int[] compl3 = useful_functions.Bin2Complementary(IVVs);
            int ivv = useful_functions.BinArray2DecLSBend(compl3, 0, 9) * 32;
            this.IVV = Convert.ToString(sign * ivv);

        }


        public void get_LatLong()
        {
            //Constants
            GeoUtils geoTool = new GeoUtils();

            //Radar information
            string LatRadar = Convert.ToString(useful_functions.minutes2decimalDegrees(41.0, 18.0, 2.5283, 1.0));
            string lonRadar = Convert.ToString(useful_functions.minutes2decimalDegrees(2.0, 6.0, 7.4095, 1.0));
            double elTerrain = 2.007;
            double elAntena = 25.25;

            //Get radar coordinates 
            CoordinatesWGS84 radarPos = new CoordinatesWGS84(LatRadar, lonRadar, elTerrain);

            //Obtain radar slant coordinates
            double XR = Convert.ToDouble(this.RHO) * 1852.0 * Math.Sin(Convert.ToDouble(this.THETA) * Math.PI / 180.0);
            double YR = Convert.ToDouble(this.RHO) * 1852.0 * Math.Cos(Convert.ToDouble(this.THETA) * Math.PI / 180.0);

            double H = 0;
            double h_corrected = 0;

            if (this.Flight_Level != null)
            {
                H = Convert.ToDouble(this.Flight_Level) * 100; //In feet

                //If FL < 0 -> H = 0
                //else H = FL
                //If N/A -> H = 0 assumeixes
                //If N/A -> no sa pot calcular

                //QNH correction
                if (H < 6000)
                {
                    if (this.BP != null)
                    {
                        H = (H + (Convert.ToDouble(this.BP) - 1013.25) * 30);
                        h_corrected = H;
                    }
                    else
                    {
                        H = 0;
                    }

                }
            }
            if (h_corrected != 0)
            {
                this.ModeC_corrected = Convert.ToString(Math.Round(h_corrected, 2));

            }
            else
            {
                this.ModeC_corrected = " ";
            }

            H = H * 0.3048; //to meters
            //Get the spherical coordinates of the radar (SR5 transformation) = rho, azimut and elevation
            double rhoMeters = Math.Sqrt(XR * XR + YR * YR);
            double azimutRad = GeoUtils.CalculateAzimuth(XR, YR);

            //double R = GeoUtils.CalculateEarthRadius(radarPos);
            double R = 6370000;
            double num = 2 * R * (H - elAntena) + H * H - elAntena * elAntena - rhoMeters * rhoMeters;
            double den = 2 * rhoMeters * (R + elAntena);
            double elevation = Math.Asin(num / den);

            CoordinatesPolar sphericalCoord = new CoordinatesPolar(rhoMeters, azimutRad, elevation);

            //To cartesian coordinates (SR7 transformation)
            CoordinatesXYZ cartesianCoord = GeoUtils.change_radar_spherical2radar_cartesian(sphericalCoord);

            //To geocentric coordinates (SR10 transformation)
            CoordinatesXYZ geocentricCoord = geoTool.change_radar_cartesian2geocentric(radarPos, cartesianCoord);

            //To geodesic coordinates (SR17 transformation)
            CoordinatesWGS84 geodesicCoord = geoTool.change_geocentric2geodesic(geocentricCoord);

            this.Latitude = Convert.ToString(Math.Round(geodesicCoord.Lat * 180 / Math.PI, 6));
            this.Longitude = Convert.ToString(Math.Round(geodesicCoord.Lon * 180 / Math.PI, 6));

        }

    }
}