using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PromotionEngine.Models;

namespace PromotionEngine
{
    class PromotionEngine
    {
        //Function to test a new scenario
        public string testnewScenarioCartOrder()
        {
            int productA, productB, productC, productD;
            var cartItems = new Dictionary<string, int>();

            //Function to show the product pricing and Promotions
            //One can change the same or add new promotions appsettings.js
            pricingDiscountInfo();

            Console.WriteLine("Enter no. of A products : ");
            productA = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter no. of B products : ");
            productB = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter no. of C products : ");
            productC = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter no. of D products : ");
            productD = Convert.ToInt32(Console.ReadLine());
            
            //Assigning product values to cartItems
            cartItems.Add("A", productA);
            cartItems.Add("B", productB);
            cartItems.Add("C", productC);
            cartItems.Add("D", productD);

            //Function to process the cart order
            processCartOrder(cartItems);
            return "Processing done Successfully";
        }

        //function to call execute the old scenarios
        public string testExistingScenarios()
        {
            //Function to show the product pricing and Promotions
            //One can change the same or add new promotions appsettings.js
            pricingDiscountInfo();

            var cart1 = new Dictionary<string, int>();
            var cart2 = new Dictionary<string, int>();
            var cart3 = new Dictionary<string, int>();
            var cart4 = new Dictionary<string, int>();

            //Assigning values and Function call to execute Scenario 1 
            Console.WriteLine("Scenario 1 : ");
            cart1.Add("A", 1);
            cart1.Add("B", 1);
            cart1.Add("C", 1);
            cart1.Add("D", 0);
            processCartOrder(cart1);

            //Assigning values and Function call to execute Scenario 2
            Console.WriteLine("Scenario 2 : ");
            cart2.Add("A", 5);
            cart2.Add("B", 5);
            cart2.Add("C", 1);
            cart2.Add("D", 0);
            processCartOrder(cart2);

            //Assigning values and Function call to execute Scenario 3
            Console.WriteLine("Scenario 3 : ");
            cart3.Add("A", 3);
            cart3.Add("B", 5);
            cart3.Add("C", 1);
            cart3.Add("D", 1);
            processCartOrder(cart3);

            //Assigning values and Function call to execute Scenario 4
            Console.WriteLine("Scenario 4 : ");
            cart4.Add("A", 2);
            cart4.Add("B", 3);
            cart4.Add("C", 3);
            cart4.Add("D", 2);
            processCartOrder(cart4);
            return "Processing done Successfully for existing Scenario";

        }

        //Function to show the product pricing and Promotions
        //One can change the same or add new promotions appsettings.js
        public void pricingDiscountInfo()
        {
            var productWithPrices = GetProductDetails();
            var promotions = GetPromotionDetails();

            Console.WriteLine("Product Name : Unit Price ");
            foreach (var product in productWithPrices)                
                Console.WriteLine(product.ProductName + " : " + product.Price);

            Console.WriteLine("Active Promotions ");
            foreach (var promotion in promotions)
                Console.WriteLine(promotion.NoOfProducts + " of " + promotion.ProductName + " for " + promotion.Price);
        }

        public void processCartOrder(Dictionary<string, int> cartItems)
        {
            //Function to show Price without applying Promotion
            var priceWithoutPromotion = calculateTotalWithoutPromotion(cartItems);
            Console.WriteLine("Price Without Promotions : " + priceWithoutPromotion);

            //Function to show Price after applying Promotion
            var priceAfterPromotion = calculateTotalWithPromotion(cartItems);
            Console.WriteLine("Price after applying Promotion : " + priceAfterPromotion);

            //Displaying amount saved by the User
            var amountSaved = priceWithoutPromotion - priceAfterPromotion;
            Console.WriteLine("");
            Console.WriteLine("Congratulations you have saved : " + amountSaved + " bucks...");
        }

        //Function to calculate price without Promotion
        public long calculateTotalWithoutPromotion(Dictionary<string, int> cartItems)
        {
            long totalPrice = 0;
            
            //fetching products from appsettings.json
            var productWithPrices = GetProductDetails();
            Console.WriteLine("Product Name : Product Count  *   Unit Price   =   Total Price");
            
            //displaying price without promotion
            foreach (var product in productWithPrices)
            {
                if (cartItems.ContainsKey(product.ProductName) && cartItems[product.ProductName] > 0)
                {
                    var productPrice = (product.Price * cartItems[product.ProductName]);
                    totalPrice = totalPrice + productPrice;
                    Console.WriteLine(product.ProductName + " : " + cartItems[product.ProductName] + " * " + product.Price + " = " + productPrice);
                    
                }
            }
            return totalPrice;

        }

        //Function to calculate price after applying promotion
        public long calculateTotalWithPromotion(Dictionary<string, int> cartItems)
        {
            Console.WriteLine("");
            long totalPrice = 0;
            List<string> promotionApplied = new List<string>() ;

            //declaring variable to use as Out parameters
            long totalPriceAfterSinglePromotionApplied;
            long totalPriceForDoublePromotionProduct;
            List<string> productsIncludedInSinglePromotion;

            //function to calculate price of cart based on single (eg. 3A's) product promotion
            totalPriceAfterSinglePromotion(cartItems, out totalPriceAfterSinglePromotionApplied, out productsIncludedInSinglePromotion);
            
            //function to calculate price of cart based on combo (eg. 3A's) product promotion
            //single promotion list is passed as if discount is once applied it should not apply again
            totalPriceAfterComboPromotion(cartItems, productsIncludedInSinglePromotion, out totalPriceForDoublePromotionProduct);
            
            //calculating total price
            totalPrice = totalPriceAfterSinglePromotionApplied + totalPriceForDoublePromotionProduct;
            return totalPrice;           
        }

        //function to calculate price of cart based on single (eg. 3A's) product promotion
        private static void totalPriceAfterSinglePromotion(Dictionary<string, int> cartItems, out long totalPriceAfterSinglePromotionApplied, out List<string> productsIncludedInSinglePromotion)
        {
            //fetching product details from appsetting.cs
            var productWithPrices = GetProductDetails();

            //fetching promotion details from appsetting.cs
            var promotions = GetPromotionDetails();

            //initializing out parameters
            totalPriceAfterSinglePromotionApplied = 0;
            productsIncludedInSinglePromotion = new List<string>();
            
            //filtering the values to minimize the loops 
            promotions = promotions.Where(x => x.Type == "single").ToList();
 
            //logic to calcluate the single product promotions (eg. 3A's or 2 B's)
            foreach (var promotion in promotions)
            {
                //entering only if the value is passed by user for product is greater than 0
                if (cartItems.ContainsKey(promotion.ProductName) && cartItems[promotion.ProductName] > 0)
                {
                    //fetching product's unit price
                    var productUnitPrice = GetProductUnitPrice(promotion.ProductName);

                    //applying the promotion if product count satisfy the promotion condition
                    if (cartItems[promotion.ProductName] >= promotion.NoOfProducts)
                    {
                            int productCountWithNoPromotionApplied;

                            //fetching divident and remainder of the single product to apply the promotion
                            int productCountWithPromotionApplied = Math.DivRem(cartItems[promotion.ProductName], (int)promotion.NoOfProducts, out productCountWithNoPromotionApplied);
                            
                            // calculating the price of products without promotion
                            var priceWithoutPromotion = productCountWithNoPromotionApplied * productUnitPrice;

                            // calculating the price of products after promotion is applied
                            var priceWithPromotion = productCountWithPromotionApplied * promotion.Price;
                            
                            //adding the total price
                            var totalProductPrice = priceWithoutPromotion + priceWithPromotion;
                            totalPriceAfterSinglePromotionApplied = totalPriceAfterSinglePromotionApplied + totalProductPrice;
                            
                            //pushing the products for which the promotion is applied already
                            productsIncludedInSinglePromotion.Add(promotion.ProductName);
                    }
                    else
                    {
                            //calculating price for products for which the promotion is not applied
                            totalPriceAfterSinglePromotionApplied = cartItems[promotion.ProductName] * productUnitPrice;
                            productsIncludedInSinglePromotion.Add(promotion.ProductName);
                    }
                }
            }

        }

        //function to calculate price of cart based on combo (eg. C & D) product promotion
        private static void totalPriceAfterComboPromotion(Dictionary<string, int> cartItems,List<string> productsIncludedInSinglePromotion, out long totalPriceForDoublePromotionProduct)
        {
            //Fetching the product details from appsettings.json
            var productWithPrices = GetProductDetails();

            //Fetching the promotion details from appsettings.json
            var promotions = GetPromotionDetails();

            //initializing the out parameters
            totalPriceForDoublePromotionProduct = 0;
            List<string> comboProducts = new List<String>();

            //Filtering the list for the conbo promotion type alone
            promotions = promotions.Where(x => x.Type == "combo").ToList();
            
            //removing the products from cartitems for which the price is already calculated
            foreach(var product in productsIncludedInSinglePromotion)
            {
                cartItems.Remove(product);
            }
            var pendingcartItems = cartItems;

            //logic to calculate the price for combo promotion
            foreach (var promotion in promotions)
            {
                //splitting combo promotion products
                if (promotion.ProductName.Contains(";"))
                {
                    comboProducts = promotion.ProductName.Split(";").ToList();
                }
                
                // using while loop till the all the combo promotions are being calculated 
                Boolean checkRequired = true;
                while (checkRequired)
                {
                    int index = 0;
                    int length = comboProducts.Count;

                    //logic to check that both products available in the combo promotion are in the cart or not
                    foreach (var product1 in comboProducts)
                    {                    
                        if (pendingcartItems.ContainsKey(product1) && pendingcartItems[product1] != 0)
                        {
                            index++;
                        }
                        else
                        {
                            checkRequired = false;
                            pendingcartItems.Remove(product1);
                            break;
                        }
                    }

                    //logic to add price and reduce the item from cart is the condition for combo promotion met
                    if (index == length)
                    {
                        foreach (var product2 in comboProducts)
                        {
                            if (pendingcartItems[product2] == 0)
                            {
                                checkRequired = false;
                                pendingcartItems.Remove(product2);                                
                            }
                            else
                            {
                                var newValue = pendingcartItems[product2] - 1;
                                pendingcartItems[product2] = newValue;                               
                            }
                        }
                        totalPriceForDoublePromotionProduct = totalPriceForDoublePromotionProduct + promotion.Price;
                    }
                }

            }

            //calculating price for the products which are included in the combo but not part of the promotion
            foreach (var cartItem in pendingcartItems)
            {
                if(cartItem.Value > 0 )
                totalPriceForDoublePromotionProduct = totalPriceForDoublePromotionProduct + (cartItem.Value * GetProductUnitPrice(cartItem.Key));
            }
        }

        //function to fetch the unit price of product
        private static long GetProductUnitPrice(string productName)
        {
            var data = GetProductDetails();
            var productPrice = data.Where(x=> x.ProductName == productName).FirstOrDefault().Price;
            return productPrice;
        }

        //logic to fetch product details from appsettings.js
        private static List<ProductWithPrice> GetProductDetails()
        {
            var data = GetlistFromAppSettings();
            return data.ProductWithPrices.ToList();
        }

        //logic to fetch promotion details from appsettings.js
        private static List<Promotion> GetPromotionDetails()
        {
            var data = GetlistFromAppSettings();
            return data.Promotions.ToList();

        }

        //logic to fetch the product and promotion details from AppSettings.json
        //these values should come from the database
        //but to assign more values pushed the json in sppsettings.json
        private static AppSettings GetlistFromAppSettings()
        {
            var JSON = System.IO.File.ReadAllText("appsettings.json");
            var appSettings = JObject.Parse(JSON);
            var data = JsonConvert.DeserializeObject<AppSettings>(appSettings.ToString());
            return data;
        }


    }
}
