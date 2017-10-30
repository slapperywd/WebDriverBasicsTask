using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Configuration;

namespace SeleniumWebDriverBasicsTask
{
    [TestFixture]
    public class MailServiceTest
    {
        IWebDriver driver;
        WebDriverWait wait;
        string mailLogin = ConfigurationManager.AppSettings["logname"];
        string mailPassword = ConfigurationManager.AppSettings["password"];

        //message will be sent to an e-mail account specified in the following line
        string messageRecipient = "maglish@mail.ru";
        string messageSubject = "Test subject";
        string messageBody = "Body body body test body test body bla bla ";

        [SetUp]
        public void SetUp()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Manage().Window.Maximize();
            
        }

        [Test]
        public void MailTest()
        {
            //Login to the mail box
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
            driver.Navigate().GoToUrl("https://www.google.com/gmail/");
            //Find email input and enter login
            driver.FindElement(By.CssSelector("input[type='email']")).SendKeys(mailLogin);
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("identifierNext"))).Click();
            //Find password input and enter password
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[name='password']"))).SendKeys(mailPassword);
            //Submit entered credentials
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("passwordNext"))).Click();

            //Assert, that the login is successful
            Assert.IsTrue(wait.Until(ExpectedConditions.TitleContains("Inbox")));

            //Open typing mail dialog
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.T-I-KE"))).Click();
            //Fill recipient input
            wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("vO"))).SendKeys(messageRecipient);
            //Fill subject input
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[name='subjectbox']"))).SendKeys(messageSubject);
            //Fill message body 
            wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("LW-avf"))).SendKeys(messageBody);
            //Close dialog (your mail will be automatically saved in "Drafts" folder)
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("img.Ha"))).Click();

            //Go to "Drafts" folder
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.TK div.aim:nth-child(4) a"))).Click();

            //Assert, that the mail presents in ‘Drafts’ folder. 
            //and draft content (addressee, subject and body the same )
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.AO div.nH div.ae4.UI[gh='tl'] table.zt tr"))).Click();
           
            //addresse
            Assert.IsTrue(wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("table.aoP.aoC input[name='subjectbox']")))
                .GetAttribute("value").Contains(messageSubject));
            // body 
            Assert.IsTrue(wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("table.aoP.aoC div.LW-avf")))
               .Text.Contains(messageBody));
            //to
             Assert.IsTrue(wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.oL.aDm.az9 span")))
               .Text.Contains(messageRecipient));

            //Send mail
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.T-I.J-J5-Ji.aoO.T-I-atl.L3"))).Click();

            //Verify, that the mail disappeared from ‘Drafts’ folder.
            //Verify, that the mail is in ‘Sent’ folder.
            //Go to "Sent mails" folder
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.TK div.aim:nth-child(3) a"))).Click();
            //this line is neccessary
            wait.Until(ExpectedConditions.TitleContains("Sent Mail"));
            ReadOnlyCollection<IWebElement> sentMails = driver.FindElements(By.CssSelector("div.AO div.nH div.ae4.UI[gh='tl'] table.zt tr"));

            bool isMailSend = false;
            foreach (var i in sentMails)
            {
                if (CheckIfMailExists(i))
                {
                    isMailSend = true;
                    break;
                }
            }

            //Verify, that the mail is in ‘Sent’ folder.
            Assert.IsTrue(isMailSend);

            //Log off.
             wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a.gb_b.gb_gb.gb_R"))).Click(); 
             wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a#gb_71"))).Click();
        }

        [TearDown]
        public void TearDown()
        {
           driver.Quit();
        }

        private bool CheckIfMailExists(IWebElement mailElement)
        {
            string mailRecepient = mailElement.FindElement(By.CssSelector("td.yX.xY div.yW span.yP")).Text;
            string mailSubject = mailElement.FindElement(By.CssSelector("td.xY.a4W div.y6 span.bog")).Text;
            string mailBody = mailElement.FindElement(By.CssSelector("td.xY.a4W div.y6 span.y2")).Text
                .Remove(0,3);
            bool c = messageRecipient.Contains(mailRecepient);
            bool a = messageSubject.Contains(mailSubject);
            bool b = messageBody.Contains(mailBody);
           
            return messageSubject.Contains(mailSubject) && messageBody.Contains(mailBody);
        }
    }
}
