using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            const string inputFileName = "Assets\\input.txt";
            if (!File.Exists(inputFileName))
            {
                return;
            }
            var readAllLines = File.ReadAllLines(inputFileName).Where(x => !string.IsNullOrEmpty(x)).ToList();
            if (readAllLines.Count <= 0)
            {
                return;
            }

            EmailController emailControl = new EmailController();
            if (emailControl.LoginEmail())
            {
                emailControl.driver.Navigate().GoToUrl("https://hangouts.google.com/");
                WaitHelper.ForDriverLoadCompleted(emailControl.driver);
                WaitHelper.ForSeconds(5);
                var hangout = new HangoutChecker { driver = emailControl.driver };
                if (hangout.OpenGroup())
                {
                    if (!hangout.AddPeopel())
                    {
                        return;
                    }

                    while (true)
                    {
                        try
                        {
                            var email = getEmail(inputFileName);
                            if (string.IsNullOrEmpty(email))
                            {
                                Console.WriteLine("Email not found!");
                                WaitHelper.ForSeconds(1);
                                continue;
                            }

                            writeFile("Assets\\temp.txt", email);
                            bool isEmailLive = hangout.CheckEmail(email.Split('|')[0].Trim());
                            if (isEmailLive)
                            {
                                writeFile("Assets\\live.txt", email);
                                try
                                {
                                    File.Delete("Assets\\temp.txt");
                                }
                                catch
                                {

                                }
                            }
                            else
                            {
                                writeFile("Assets\\dead.txt", email);
                            }

                            WaitHelper.ForSeconds(1);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        static string getEmail(string inputFileName)
        {
            try
            {
                var readAllLines = File.ReadAllLines(inputFileName).Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (readAllLines.Count <= 0)
                {
                    return null;
                }

                var result = readAllLines[0];
                readAllLines.RemoveAt(0);

                File.WriteAllLines(inputFileName, readAllLines.ToArray());

                return result;
            }
            catch
            {
                return null;
            }
        }

        static void writeFile(string fileName, string content)
        {
            try
            {
                using (var stw = File.AppendText(fileName))
                {
                    stw.WriteLine(content);
                    stw.Flush();
                    stw.Close();
                }
            }
            catch
            {

            }
        }
    }
}
