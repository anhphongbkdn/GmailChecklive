using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailChecker
{
    class EmailController
    {
        public static object obj = new object();
        public FirefoxDriver driver { get; set; }
        public EmailController()
        {

        }

        public bool LoginEmail()
        {
            try
            {
                this.driver = driver;
                var services = FirefoxDriverService.CreateDefaultService();
                services.HideCommandPromptWindow = true;

                var options = new FirefoxOptions();
                options.AddArguments("-private");
                options.SetPreference("--enable-automation", false);
                options.SetPreference("dom.webdriver.enabled", false);
                options.SetPreference("webdriver_enable_native_events", false);
                options.SetPreference("webdrive_assume_untrusted_issuer", false);
                options.SetPreference("media.peerconnection.enabled", false);
                options.SetPreference("media.navigator.permission.disabled", true);
                //options.SetPreference("general.useragent.override", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Safari/605.1.15");

                driver = new FirefoxDriver(services, options);
                //driver.Manage().Window.Size = new System.Drawing.Size(750, 600);
                //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(90);
                driver.Manage().Window.Maximize();
                driver.Url = "https://mail.google.com/mail/u/0/#inbox";
                driver.Navigate();
                WaitHelper.ForDriverLoadCompleted(driver);
                WaitHelper.ForSeconds(1);

                var readAllFile = File.ReadAllLines("Assets\\email.txt").ToList();
                if (readAllFile.Count <= 0)
                {
                    return false;
                }

                var emailInfo = readAllFile[0].Split('|').ToList();
                var email = new { UserName = emailInfo[0], Password = emailInfo[1] };

                var userNameXpath = driver.FindElement(By.XPath("//*[@id=\"identifierId\"]"));
                userNameXpath.SendKeys(email.UserName);
                lock (obj)
                {
                    userNameXpath.SendKeys(Keys.Enter);
                }
                WaitHelper.ForSeconds(5);

                try
                {
                    var foundError = driver.FindElement(By.XPath("//*[@id=\"headingText\"]/span"));
                    if (foundError != null)
                    {
                        var text = foundError.Text;
                        //ReSend user name
                        if (!text.Equals("Chào mừng"))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("location.reload();");
                            WaitHelper.ForDriverLoadCompleted(driver);
                            userNameXpath = driver.FindElement(By.XPath("//*[@id=\"identifierId\"]"));
                            userNameXpath.SendKeys(email.UserName);
                            lock (obj)
                            {
                                userNameXpath.SendKeys(Keys.Enter);
                            }

                            WaitHelper.ForSeconds(3);
                        }
                    }
                }
                catch
                {

                }

                //Send password
                var passwordXpath = driver.FindElement(By.XPath("//*[@id=\"password\"]/div[1]/div/div[1]/input"));
                passwordXpath.SendKeys(email.Password);
                lock (obj)
                {
                    passwordXpath.SendKeys(Keys.Enter);
                }

                var stw = Stopwatch.StartNew();
                while (true)
                {
                    try
                    {
                        if (stw.ElapsedMilliseconds > 15000)
                        {
                            break;
                        }

                        bool exits = isElementExitsWithTimeout("//*[@id=\"password\"]/div[1]/div/div[1]/input", ByType.BY_XPATH, 1000);
                        if (!exits)
                        {
                            break;
                        }

                        var elementExits = FindElementIfExists(driver, By.CssSelector(".OyEIQ > div:nth-child(2) > span:nth-child(1)"));
                        if (elementExits != null && elementExits.Text.StartsWith("Mật khẩu của bạn đã thay đổi"))
                        {
                            break;
                        }

                        WaitHelper.ForMiliSeconds(100);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        break;
                    }
                }

                stw.Restart();
                while (true)
                {
                    try
                    {
                        if (stw.ElapsedMilliseconds > 15000)
                        {
                            break;
                        }

                        var findErrorPass = driver.FindElement(By.CssSelector("#password > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > input:nth-child(1)"));
                        if (findErrorPass != null)
                        {
                            try
                            {
                                var findText = driver.FindElement(By.XPath("/html/body/div[1]/div[1]/div[2]/div/div[2]/div/div/div[2]/div/div[1]/div/form/span/section/div/div/div[1]/div[2]/div[2]/span"));
                            }
                            catch
                            {

                            }
                            return true;
                        }

                        WaitHelper.ForSeconds(1);
                    }
                    catch
                    {
                        break;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IWebElement FindElementIfExists(IWebDriver driver, By by)
        {
            var elements = driver.FindElements(by);
            return (elements.Count >= 1) ? elements.First() : null;
        }

        private bool isElementExitsWithTimeout(string elementName, ByType type, int milisecondsTimeout)
        {
            var stw = Stopwatch.StartNew();
            while (true)
            {
                try
                {
                    if (stw.ElapsedMilliseconds > milisecondsTimeout)
                    {
                        return false;
                    }

                    IWebElement element = null;
                    switch (type)
                    {
                        case ByType.BY_XPATH:
                            element = FindElementIfExists(driver, By.XPath(elementName));
                            break;

                        case ByType.BY_CSSSELECTOR:
                            element = FindElementIfExists(driver, By.CssSelector(elementName));
                            break;

                        default:
                            break;
                    }

                    if (element != null)
                    {
                        return true;
                    }

                    WaitHelper.ForMiliSeconds(100);
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
