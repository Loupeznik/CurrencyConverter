using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml.Schema;
using Newtonsoft.Json.Linq;

namespace currencyConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WELCOME TO CURRENCY CONVERTER");
            Console.WriteLine("EXCHANGE RATES COURTESY OF Exchange rates API (https://exchangeratesapi.io)");
            Console.WriteLine("(C)2020 Dominik Zarsky, PROJECT IS MIT LICENSED");
            Console.WriteLine("------------------");
            int currency = Options();
            Console.Write("Amount to convert: ");
            string amountText = Console.ReadLine();
            if (double.TryParse(amountText, out _))
            {
                amountText = amountText.Replace('.', ',');
                double amount = Convert.ToDouble(amountText);
                Console.WriteLine("------RESULT------");
                Console.WriteLine(ConvertAmount(currency, amount));
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("INVALID AMOUNT"); //add better logic for handling this
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
                    Options();
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
