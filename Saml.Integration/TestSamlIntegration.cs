using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuppeteerSharp;
using System.Threading.Tasks;
using System.Web;
using System.Net;

/// <summary>
/// Integration testing for the SAML library AspNet.
/// These tests ensure that the SAML library is able to work with the Azure active directory.
/// </summary>
namespace Saml.Integration
{
    [TestClass]
    public class TestSamlIntegration
    {
        const string SCREENHOST_PATH = "../../Screenshots/";
        const string USERNAME_SELECTOR = "#i0116";
        const string PASSWORD_SELECTOR = "#i0118";
        const string NEXT_BUTTON_SELECTOR = "#idSIButton9";
        const string SIGN_IN_BUTTON_SELECTOR = "#idSIButton9";
        const string NOT_STAY_SIGNED_IN_BUTTON_SELECTOR = "#idBtn_Back";

        const string USERNAME = "intern1@DISPLAYRSAMLTEST.onmicrosoft.com";
        const string PASSWORD = "Testmyapp5";

        /// <summary> This test ensures that the in built c# web client is able to use the SAML library to
        /// allow the user to perform single sign on to the azure active directory.
        /// </summary>
        [TestMethod()]
        public async Task TestRedirectToIdentityProvider()
        {
            var options = new LaunchOptions { Headless = true };
            Console.WriteLine("Downloading chromium for testing: ");

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Console.WriteLine("Navigating to SSO url: ");

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                string saml_endpoint = "https://login.microsoftonline.com/86c4efd3-7f59-4e51-8f64-6d7848dfcaef/saml2";

                AuthRequest auth = new AuthRequest(
                    "15eedc3e-ead5-47c8-8424-a98027d91da7",         // put your app's "unique ID" here
                    "https://localhost:44376/Home/WelcomeUser"      // assertion Consumer Url - the redirect URL where the provider will send authenticated users
                );

                // goto the micrsoft login page
                string sso_redirect = auth.GetRedirectUrl(saml_endpoint);
                await page.GoToAsync(sso_redirect);

                Console.WriteLine("Redirecting to sso url: " + sso_redirect);

                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 2560,
                    Height = 1080
                });

                await page.ScreenshotAsync(SCREENHOST_PATH + "0.png");
                await page.WaitForNavigationAsync();

                // sign in with our username + password
                await page.TypeAsync(USERNAME_SELECTOR, USERNAME);
                await page.ScreenshotAsync(SCREENHOST_PATH + "1.png");

                await page.ClickAsync(NEXT_BUTTON_SELECTOR);
                await page.WaitForNavigationAsync();
                await page.ScreenshotAsync(SCREENHOST_PATH + "2.png");

                await page.TypeAsync(PASSWORD_SELECTOR, PASSWORD);
                await page.ScreenshotAsync(SCREENHOST_PATH + "3.png");

                await page.ClickAsync(SIGN_IN_BUTTON_SELECTOR);
                await page.WaitForNavigationAsync();
                await page.ScreenshotAsync(SCREENHOST_PATH + "4.png");
                
                await page.ClickAsync(NOT_STAY_SIGNED_IN_BUTTON_SELECTOR);
                var response = await page.WaitForNavigationAsync();
                await page.ScreenshotAsync(SCREENHOST_PATH + "5.png");

                string saml_response = response.Request.PostData.ToString();

                // grab the SAML token provided by the IDP
               

                await page.CloseAsync();
                await browser.CloseAsync();
            }
        }
    }
}
