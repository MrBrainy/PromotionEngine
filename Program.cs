using System;
using System.IO;
using System.Transactions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PromotionEngine;

namespace PromotionEngine
{
    public class Program
    {     

        static void Main(string[] args)
        {
            PromotionEngine obj = new PromotionEngine();

            Console.WriteLine("*** Welcome to Promotion Enging ***");
            //Select to test the scenarios as mentioned in PDF.
            Console.WriteLine("1. To execute existing test scenarios.");
            //Select to test a dynamic scenario in real time.
            Console.WriteLine("2. To test a new Scenario.");
            try
            {
                int selectedValue = Convert.ToInt32(Console.ReadLine());

                //Switch Case using C# 8

                var res = selectedValue switch
                {
                    1 => obj.testExistingScenarios(),
                    2 => obj.testnewScenarioCartOrder(),
                    _ => "Invalid Selection",

                };
                Console.WriteLine(res);
            }
            catch (FormatException)
            {
                Console.WriteLine("Select/Enter a positive integer value");
            }
            catch (Exception)
            {
                throw;
            }

            Console.ReadLine();
        }       

    }
}
