using System;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuppeteerSharp;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

/// <summary>
/// Integration testing for the SAML library AspNet.
/// These tests ensure that the SAML library is able to work with the Azure active directory.
/// </summary>
namespace Saml.Integration
{
    [TestClass]
    public class TestSamlIntegration
    { 
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
                AuthRequest auth = new AuthRequest(
                    Constants.APP_ID,         // put your app's "unique ID" here
                    Constants.REPLY_URL      // assertion Consumer Url - the redirect URL where the provider will send authenticated users
                );

                // goto the micrsoft login page
                string sso_redirect = auth.GetRedirectUrl(Constants.SAML_ENDPOINT);
                await page.GoToAsync(sso_redirect);

                Console.WriteLine("Redirecting to sso url: " + sso_redirect);

                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 2560,
                    Height = 1080
                });
               

                await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "0.png");
                await page.WaitForNavigationAsync();

                // sign in with our username + password
                await page.TypeAsync(Constants.USERNAME_SELECTOR, Constants.USERNAME);
                await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "1.png");

                await page.ClickAsync(Constants.NEXT_BUTTON_SELECTOR);
                await page.WaitForNavigationAsync();
                await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "2.png");

                await page.TypeAsync(Constants.PASSWORD_SELECTOR, Constants.PASSWORD);
                await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "3.png");

                await page.ClickAsync(Constants.SIGN_IN_BUTTON_SELECTOR);
                await page.WaitForNavigationAsync();
                await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "4.png");
                
                await page.ClickAsync(Constants.NOT_STAY_SIGNED_IN_BUTTON_SELECTOR);

                // this is actually a request from the azure IDP
                var response = await page.WaitForRequestAsync(Constants.REPLY_URL);
                await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "5.png");

                await page.CloseAsync();
                await browser.CloseAsync();

                // we can't do much with a NULL response so just leave here.
                Assert.IsNotNull(response);

                // grab the SAML token provided by the IDP and ensure 
                // and ensure it contains relevant information
                string saml_response = response.PostData.ToString();
                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();

                int index = saml_response.IndexOf("SAMLResponse=");
                int len = "SAMLResponse=".Length;

                string saml_formatted = saml_response.Substring(index + len);
                string saml_url_decoded = HttpUtility.UrlDecode(saml_formatted);

                MicrosoftResponse ms_response = new MicrosoftResponse(Constants.VALID_CERTIFICATE);
                ms_response.LoadXmlFromBase64(saml_url_decoded);

                string display_name = ms_response.GetDisplayName();

                Assert.AreEqual("intern1", display_name);
            }
        }
    }
}
