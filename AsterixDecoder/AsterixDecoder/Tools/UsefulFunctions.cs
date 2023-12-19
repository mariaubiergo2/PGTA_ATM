using System;
using System.Drawing;

namespace AsterixDecoder
{
    class UsefulFunctions
    {
        public UsefulFunctions()
        {

        }

        // If the input is 2 (decimal) 
        // The output is [0,1] (binary)
        public int[] Dec2Bin(int decimalNumber, int numBits)
        {
            int[] binary = new int[numBits];

            for (int i = 0; decimalNumber > 0; i++)
            {
                binary[i] = decimalNumber % 2;
                decimalNumber = decimalNumber / 2;
            }

            return binary;
        }

        // If the input is 2 (decimal) 
        // The output is [1,0] (binary)
        public int[] Dec2BinLSBend(int decimalNumber, int numBits)
        {
            int[] binary = Dec2Bin(decimalNumber, numBits);
            Array.Reverse(binary);

            return binary;
        }

        //Given an array and the number of bits that it is wanted to convert
        //If input is [0*,1*,1*,1] and 0 and 3
        //Output is 2+4=6
        public int BinArray2Dec(int[] binaryArray, int initial, int numBits)
        {
            int dec = 0;

            for (int i = initial; i < initial + numBits; i++)
            {
                if (binaryArray[i] == 1)
                {
                    dec = dec + Convert.ToInt32(Math.Pow(2, i - initial));
                }
            }

            return dec;
        }
        //If input is [0*,1*,1*,1] and 0 and 2
        //Output is 1+2=3
        public int BinArray2DecLSBend(int[] binaryArray, int initial, int final)
        {
            int[] sliced = new int[(final - initial) + 1];
            Array.Copy(binaryArray, initial, sliced, 0, (final - initial) + 1);
            Array.Reverse(sliced);

            return BinArray2Dec(sliced, 0, sliced.Length);
        }

        public int BinComplementArray2DecLSBend(int[] binaryArray, int initial, int final)
        {
            int[] sliced = new int[(final - initial) + 1];
            Array.Copy(binaryArray, initial, sliced, 0, (final - initial) + 1);

            if (sliced[0] == 0)
            {
                Array.Reverse(sliced);
                return BinArray2Dec(sliced, 0, sliced.Length);
            }
            else
            {
                int[] complement2 = Bin2Complementary(sliced);
                Array.Reverse(complement2);

                return -BinArray2Dec(complement2, 0, complement2.Length);
            }
        }


        public int[] Bin2Complementary(int[] binaryArray)
        {
            if (binaryArray[0] == 1)
            {
                //Invert all bits
                for (int i = 0; i < binaryArray.Length; i++)
                {
                    binaryArray[i] = 1 - binaryArray[i];
                }

                // Add 1 to obtain two's complement
                int carry = 1;
                for (int i = binaryArray.Length - 1; i >= 0; i--)
                {
                    int sum = binaryArray[i] + carry;
                    binaryArray[i] = sum % 2;
                    carry = sum / 2;
                }
            }
            return binaryArray;

        }


        public string Bin2Hex(int a, int b, int c, int d)
        {
            string num = Convert.ToString(a) + Convert.ToString(b) + Convert.ToString(c) + Convert.ToString(d);

            if (num == "0000") { num = "0"; }
            if (num == "0001") { num = "1"; }
            if (num == "0010") { num = "2"; }
            if (num == "0011") { num = "3"; }
            if (num == "0100") { num = "4"; }
            if (num == "0101") { num = "5"; }
            if (num == "0110") { num = "6"; }
            if (num == "0111") { num = "7"; }
            if (num == "1000") { num = "8"; }
            if (num == "1001") { num = "9"; }
            if (num == "1010") { num = "A"; }
            if (num == "1011") { num = "B"; }
            if (num == "1100") { num = "C"; }
            if (num == "1101") { num = "D"; }
            if (num == "1110") { num = "E"; }
            if (num == "1111") { num = "F"; }

            return num;
        }

        public string Dec2Octal(int decimalNumber)
        {
            if (decimalNumber == 0)
                return "0000"; // Special case: binary 0 is octal 0

            string octal = "";
            while (decimalNumber > 0)
            {
                long remainder = decimalNumber % 8;
                octal = remainder.ToString() + octal;
                decimalNumber /= 8;
            }
            while (octal.Length < 4)
            {
                octal = "0" + octal;
            }

            return octal;
        }

        public string BinArray2ASCII(int b6, int b5, int b4, int b3, int b2, int b1)
        {
            string row = Bin2Hex(b4, b3, b2, b1);
            int col;
            if (b6 == 0)
            {
                if (b5 == 0) { col = 0; }
                else { col = 1; }

            }
            else
            {
                if (b5 == 0) { col = 2; }
                else { col = 3; }
            }


            string res = "";

            if (row == "0")
            {
                if (col == 0) { res = ""; }
                if (col == 1) { res = "P"; }
                if (col == 2) { res = " "; }
                if (col == 3) { res = "0"; }
            }
            if (row == "1")
            {
                if (col == 0) { res = "A"; }
                if (col == 1) { res = "Q"; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = "1"; }
            }
            if (row == "2")
            {
                if (col == 0) { res = "B"; }
                if (col == 1) { res = "R"; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = "2"; }
            }
            if (row == "3")
            {
                if (col == 0) { res = "C"; }
                if (col == 1) { res = "S"; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = "3"; }
            }
            if (row == "4")
            {
                if (col == 0) { res = "D"; }
                if (col == 1) { res = "T"; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = "4"; }
            }
            if (row == "5")
            {
                if (col == 0) { res = "E"; }
                if (col == 1) { res = "U"; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = "5"; }
            }
            if (row == "6")
            {
                if (col == 0) { res = "F"; }
                if (col == 1) { res = "V"; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = "6"; }
            }
            if (row == "7")
            {
                if (col == 0) { res = "G"; }
                if (col == 1) { res = "W"; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = "7"; }
            }
            if (row == "8")
            {
                if (col == 0) { res = "H"; }
                if (col == 1) { res = "X"; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = "8"; }
            }
            if (row == "9")
            {
                if (col == 0) { res = "I"; }
                if (col == 1) { res = "Y"; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = "9"; }
            }
            if (row == "A")
            {
                if (col == 0) { res = "J"; }
                if (col == 1) { res = "Z"; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = ""; }
            }
            if (row == "B")
            {
                if (col == 0) { res = "K"; }
                if (col == 1) { res = ""; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = ""; }
            }
            if (row == "C")
            {
                if (col == 0) { res = "L"; }
                if (col == 1) { res = ""; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = ""; }
            }
            if (row == "D")
            {
                if (col == 0) { res = "M"; }
                if (col == 1) { res = ""; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = ""; }
            }
            if (row == "E")
            {
                if (col == 0) { res = "N"; }
                if (col == 1) { res = ""; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = ""; }
            }
            if (row == "F")
            {
                if (col == 0) { res = "O"; }
                if (col == 1) { res = ""; }
                if (col == 2) { res = ""; }
                if (col == 3) { res = ""; }
            }

            return res;
        }


        public string returnYesOrNo(int value)
        {
            if (value == 0)
            {
                return "No";
            }
            else
            {
                return "Yes";
            }
        }

        public string validatedOrNot(int value)
        {
            if (value == 0)
            {
                return "V";
            }
            else
            {
                return "NV";
            }
        }

        public string garbledOrDefault(int value)
        {
            if (value == 0)
            {
                return "Default";
            }
            else
            {
                return "Garbled code";
            }
        }

        public string derivedOrNot(int value)
        {
            if (value == 0)
            {
                return "Derived";
            }
            else
            {
                return "Not derived";
            }
        }

        //If N or E -> NSEW = 1.0
        //If W or S -> NSEW = -1.0
        public double minutes2decimalDegrees(double dd, double mm, double ss, double NSEW)
        {
            double res = (dd + (mm / 60) + (ss / 3600.0)) * NSEW;

            return res;
        }

        public bool isDateBetween(DateTime fromDate, DateTime toDate, DateTime curent)
        {
            int result = DateTime.Compare(fromDate, curent);

            if (result < 0)
            {
                int result2 = DateTime.Compare(curent, toDate);
                if (result2 < 0)
                {
                    return true;
                }
                else if (result == 0)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else if (result == 0)
            {

                return false;
            }
            else
            {
                return false;
            }
        }

        public string FormatTimeString(string input)
        {
            // Split the input string by colon
            string[] parts = input.Split(':');

            if (parts.Length >= 3)
            {
                // Create a new string with the first three parts joined by colon
                string formattedTime = $"{parts[0]}:{parts[1]}:{parts[2]}";

                return formattedTime;
            }
            else
            {
                // Handle invalid input
                Console.WriteLine("Invalid time format");
                return null;
            }
        }

        public DateTime fromToD2Hour(double seconds)
        {
            //CONVERTIR DE SEGONS A DATETIME FORMAT :)
            throw new NotImplementedException();
        }

    }

}
