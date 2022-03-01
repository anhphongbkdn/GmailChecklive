using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailChecker
{
    class WaitHelper
    {
        public static void ForSeconds(int seconds)
        {
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }
        public static void ForMiliSeconds(int miliseconds)
        {
            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(miliseconds));
        }

        public static void ForDriverLoadCompleted(IWebDriver driver)
        {
            //Log.print("Waitting for drive load complete...");
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60.00));
            try
            {
                wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch
            {

            }
        }
    }
}
