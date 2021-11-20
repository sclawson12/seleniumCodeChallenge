using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;

namespace Test_Automation
{
    class SeleniumChallenge
    {
        static void Main(string[] args)
        {
            var csvFile = "..\\..\\..\\input.csv";
            var outputFile = "..\\..\\..\\output.csv";

            var readData = File.ReadAllLines(csvFile);
            var productId = new List<string>();
            var price = new List<string>();

            foreach (var line in readData)
            {
                var columns = line.Split(",");

                productId.Add(columns[0]);
                price.Add(columns[1]);
            }

            var searches = productId.Zip(price, (p, d) => new { Product = p, Price = d });

            if (File.Exists(outputFile))
            {
                File.WriteAllText(outputFile, String.Empty);
            }


            IWebDriver driver = new ChromeDriver();
            driver.Url = "https://www.amazon.com";

            foreach (var element in searches)
            {
                try
                {
                    driver.FindElement(By.Id("twotabsearchtextbox")).Clear();
                    driver.FindElement(By.Id("twotabsearchtextbox")).SendKeys(element.Product);
                    driver.FindElement(By.Id("nav-search-submit-button")).Click();
                    driver.FindElement(By.CssSelector("div[data-asin='" + element.Product + "']")).Click();
                    var result = "ProductId " + element.Product + " - expected price = " + element.Price + ", Displayed price = " + driver.FindElement(By.CssSelector("span[class='a-price a-text-price a-size-medium apexPriceToPay']")).Text + ", Price matches = " + AssertionException.Equals(driver.FindElement(By.CssSelector("span[class='a-price a-text-price a-size-medium apexPriceToPay']")).Text, element.Price) + "\n";
                    File.AppendAllText(outputFile, result);
                }
                catch (Exception)
                {
                    File.AppendAllText(outputFile, "Error finding element.\n");
                }
                //Console.WriteLine("Price matches expected - " + AssertionException.Equals(driver.FindElement(By.CssSelector("span[class='a-price a-text-price a-size-medium apexPriceToPay']")).Text, element.Price));

            }



            driver.Close();
            driver.Quit();
            
        }
    }
}
