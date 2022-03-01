using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailChecker
{
    class HangoutChecker
    {
        public FirefoxDriver driver { get; set; }
        public HangoutChecker()
        {

        }

        public bool OpenGroup()
        {
            var stw = Stopwatch.StartNew();
            while (true)
            {
                try
                {
                    if (stw.ElapsedMilliseconds > 10000)
                    {
                        Console.WriteLine("timeout!");
                        return false;
                    }

                    var webElements = driver.FindElements(By.TagName("iframe"));
                    int index = 0;
                    for (int i = 0; i < webElements.Count; i++)
                    {
                        var iwebElement = webElements[i];
                        var id = iwebElement.GetAttribute("id");
                        if (!string.IsNullOrEmpty(id))
                        {
                            index = i;
                            break;
                        }
                    }

                    driver.SwitchTo().Frame(webElements[index]);
                    var selector = driver.FindElement(By.XPath("/html/body/div/div/div/div/div[2]/div/div/div[2]/div/div/div/div/div[1]/div[1]"));
                    selector.Click();
                    WaitHelper.ForSeconds(3);
                    return true;
                }
                catch
                {

                }

                WaitHelper.ForSeconds(1);
            }
        }

        public bool AddPeopel()
        {
            driver.SwitchTo().ParentFrame();
            var stw = Stopwatch.StartNew();
            while (true)
            {
                try
                {
                    if (stw.ElapsedMilliseconds > 10000)
                    {
                        return false;
                    }

                    var webElements = driver.FindElements(By.TagName("iframe"));
                    driver.SwitchTo().Frame(webElements[webElements.Count - 1]);
                    driver.FindElement(By.XPath("/html/body/div/div[3]/div/div/div/div/div[1]/div/div[2]/div[1]/div/div/div/div[1]/div/div/div[2]/button/div[1]")).Click();
                    driver.FindElement(By.XPath("/html/body/div/div[3]/div/div/div/div[2]/div[3]/div/div/div/div/div/div[2]/div/button")).Click();
                    WaitHelper.ForSeconds(3);
                    return true;
                }
                catch
                {

                }

                WaitHelper.ForSeconds(1);
            }
        }

        public bool CheckEmail(string email)
        {
            var stw = Stopwatch.StartNew();
            var findInput = driver.FindElement(By.XPath("/html/body/div/div[3]/div/div/div/div[3]/div/div[1]/div/div/div/div/div/input"));
            findInput.Clear();
            findInput.SendKeys(email);

            while (true)
            {
                try
                {
                    if (stw.ElapsedMilliseconds > 10000)
                    {
                        return false;
                    }

                    var resultElement = driver.FindElement(By.XPath("/html/body/div/div[3]/div/div/div/div[3]/div/div[2]/div/div/div/div/div[12]/div[2]/ul/li[1]"));
                    var text = resultElement.Text;
                    Debug.WriteLine(resultElement.Text);
                    if (text.Contains(email) && text.Contains("Invite"))
                    {
                        findInput.Clear();
                        return true;
                    }

                    findInput.Clear();
                    return false;
                }
                catch
                {

                }

                WaitHelper.ForSeconds(1);
            }
        }
    }
}
