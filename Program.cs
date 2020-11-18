using System;
using System.Globalization;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace currencyConverter
{
    class Program
    {

        static protected bool displayAnnotation = true;
        static protected int queryNumber = 1;

        static void Main(string[] args)
        {
            if (displayAnnotation == true)
            {
                Console.WriteLine("WELCOME TO CURRENCY CONVERTER");
                Console.WriteLine("EXCHANGE RATES COURTESY OF Exchange rates API (https://exchangeratesapi.io)");
                Console.WriteLine("(C)2020 Dominik Zarsky, PROJECT IS MIT LICENSED");
                Console.WriteLine("------------------");
            }
            int currency = Options();
            bool reset = ConvertAmount(currency);
            if (reset)
            {
                displayAnnotation = false;
                queryNumber += 1;
                Console.WriteLine("------------------");
                Console.WriteLine("-----QUERY #{0}-----", queryNumber);
                Console.WriteLine("------------------");
                Main(null);
            } else
            {
                Console.WriteLine("------------------");
                Console.WriteLine("PRESS ANY KEY TO EXIT");
                Console.ReadKey();
            }
        }

        static bool ConvertAmount(int currency)
        {
            Console.Write("Amount to convert: ");
            string amountText = Console.ReadLine();
            amountText = amountText.Replace('.', ',');
            if (double.TryParse(amountText, out _))
            {
                double amount = Convert.ToDouble(amountText);
                Console.WriteLine("------RESULT------");
                Console.WriteLine(ConvertAmount(currency, amount));
                Console.WriteLine("-----RESTART?-----");
                Console.WriteLine("Y (Yes), N (No)");
                string cmd = Console.ReadLine().ToLower();
                if (cmd == "y" || cmd == "yes") return true; else return false;
            }
            else
            {
                Console.WriteLine("Entered amount is invalid");
                return true;
            }
        }

        static int Options()
        {
            Console.WriteLine("Select currency to convert");
            Console.WriteLine("1  | USD to CZK");
            Console.WriteLine("2  | CZK to USD");
            Console.WriteLine("3  | EUR to CZK");
            Console.WriteLine("4  | CZK to EUR");
            Console.WriteLine("5  | USD to EUR");
            Console.WriteLine("6  | EUR to USD");
            Console.WriteLine("7  | GBP to EUR");
            Console.WriteLine("8  | GBP to USD");
            Console.WriteLine("9  | GBP to CZK");
            Console.WriteLine("10 | EUR to GBP");
            Console.WriteLine("11 | USD to GBP");
            Console.WriteLine("12 | CZK to GBP");
            Console.WriteLine("------------------");
            Console.Write("Enter option number: ");
            string userInput = Console.ReadLine();
            try
            {
                int output = Convert.ToInt32(userInput);
                if (!checkOption(output))
                {
                    Console.WriteLine("------------------");
                    Console.WriteLine(output.ToString() + " is not a valid option");
                    Console.WriteLine("Try again");
                    Console.WriteLine("------------------");
                    return Options();
                }
                return output;
            } 
            catch
            {
                Console.WriteLine("------------------");
                Console.WriteLine("No value was entered");
                Console.WriteLine("Try again");
                Console.WriteLine("------------------");
                return Options();
            }
        }

        static bool checkOption(int option)
        {
            if (option > 0 && option < 13)
            {
                return true;
            }
            else return false;
        }

        static string ConvertAmount(int currency, double amount)
        {

            string[,] conversionTable = { 
                { "USD", "CZK" }, { "CZK", "USD" }, { "EUR", "CZK" }, 
                { "CZK", "EUR" }, { "USD", "EUR" }, { "EUR", "USD" }, 
                { "GBP", "EUR" }, { "GBP", "USD" }, { "GBP", "CZK" },
                { "EUR", "GBP" }, { "USD", "GBP" }, { "CZK", "GBP" }
            };

            string baseCurrency = conversionTable[currency-1, 0];
            string convertionCurrency = conversionTable[currency - 1, 1];

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            double rate = Convert.ToDouble(FetchRates(baseCurrency, convertionCurrency), provider);

            double convertedAmount = amount * rate;

            string returnValue = "At this moment, " + amount + " " + baseCurrency + " = " + convertedAmount + " " + convertionCurrency;

            //return Convert.ToString(convertedAmount);
            //return rate;
            return returnValue;

        }

        static string FetchRates(string baseCurrency, string convertionCurrency)
        {
            string url = "https://api.exchangeratesapi.io/latest?base=" + baseCurrency;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    string content = reader.ReadToEnd();
                    var obj = JObject.Parse(content);
                    var rate = (string)obj["rates"][convertionCurrency];
                    return rate;
                }
            } catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                return null;
            }
            
        }
    }
}
