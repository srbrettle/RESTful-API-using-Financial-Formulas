using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;

namespace TestsSelenium
{
    public class SeleniumTests
    {
        [Fact]
        public void Test()
        {
            // Start main project process
            using (var webServerProcess = new System.Diagnostics.Process
            {
                EnableRaisingEvents = false,
                StartInfo = {
                    WorkingDirectory = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "RESTful Financial Formulas API"),
                    FileName = $"dotnet.exe",
                    Arguments = " run"
                }                
            }) {            
                // Selenium test
                using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
                {
                    // Test API result
                    driver.Navigate().GoToUrl(@"http://localhost:63995/api/FinancialFormulas/CalcAssets/10/20/");
                    var value = driver.FindElement(By.XPath("/html/body/pre"));
                    Assert.Equal("\"30\"", value.Text);
                }
            }        
        }
    }
}
