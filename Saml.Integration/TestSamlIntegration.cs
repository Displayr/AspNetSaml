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
        Browser browser;
        Page page;

        string username;
        string password;

        /// <summary> This test ensures that the in built c# web client is able to use the SAML library to
        /// allow the user to perform single sign on to the azure active directory.
        /// </summary>
        [TestMethod()]
        public async Task TestRedirectToIdentityProviderAsync()
        {
            await CreateBrowserAndPage();

            SetUsername(Constants.USERNAME);
            SetPassword(Constants.PASSWORD);

            string saml_response = await DoLoginAsync();

            await DestroyBrowserAndPage();

            // grab the SAML token provided by the IDP and ensure 
            // and ensure it contains relevant information
            //string saml_response = response.PostData.ToString();
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

        /// <summary> This test ensures that after signing in using the SAML library, we are also able 
        /// to signout using the SAML library.
        /// </summary>
        /// <returns></returns>
        [TestMethod()]
        public async Task TestRedirectToSignoutAsync()
        {
            await CreateBrowserAndPage();

            SetUsername(Constants.USERNAME);
            SetPassword(Constants.PASSWORD);

            string saml_response = await DoLoginAsync();
            string logout_response = await DoLogoutAsync();

            await DestroyBrowserAndPage();

            Assert.AreEqual("You signed out of your account", logout_response);
        }

        /// <summary> This test ensures that that the webpage maintains state and does not prompt the user 
        /// to provide login details after previously signing in.
        /// </summary>
        /// <returns></returns>
        [TestMethod()]
        public async Task TestAlreadySignedIn()
        {
            await CreateBrowserAndPage();

            SetUsername(Constants.USERNAME);
            SetPassword(Constants.PASSWORD);

            string saml_response = await DoLoginAsync();

            // try to navigate to the web service again
            // we should already be logged in 
            await page.GoToAsync("https://localhost:44376/");
            await page.WaitForNavigationAsync();
            await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "login_again.png");

            // extract the display name from the page
            var element = await page.QuerySelectorAsync("body > div.container.body-content > div.row > div > div:nth-child(1) > div > h4:nth-child(4)");
            string display_name = await element.EvaluateFunctionAsync<string>("(element) => { return element.innerHTML; }");

            await DestroyBrowserAndPage();

            Assert.AreEqual("Username: intern1@DISPLAYRSAMLTEST.onmicrosoft.com", display_name);
        }

        /// <summary> The test ensures that if the user enters the wrong password at the login screen,
        /// then the appropriate error status is returned.
        /// </summary>
        /// <returns></returns>
        [TestMethod()]
        public async Task TestWrongPassword()
        {
            await CreateBrowserAndPage();

            SetUsername(Constants.USERNAME);
            SetPassword("Wrong password");

            await DoLoginIncorrectAsync();
            



            await DestroyBrowserAndPage();
        }

        /// <summary> This function uses the PuppeteerSharp module to perform single sign out. 
        /// This is assuming the user is already logged in.
        /// </summary>
        /// <returns></returns>
        async Task<string> DoLogoutAsync()
        {
            await this.page.ScreenshotAsync(Constants.SCREENHOST_PATH + "logout_0.png");
            await this.page.ClickAsync(Constants.LOGOUT_SELECTOR);
            await this.page.WaitForNavigationAsync();
            await this.page.ScreenshotAsync(Constants.SCREENHOST_PATH + "logout_1.png");

            string html_doc = await this.page.GetContentAsync();

            var element = await this.page.QuerySelectorAsync("#login_workload_logo_text");
            string contents = await element.EvaluateFunctionAsync<string>("(element) => { return element.innerHTML; }");
            
            return contents;
        }

        /// <summary> Instantiates a headless chrome browser and a page object to hold the rendered 
        /// objects.
        /// </summary>
        /// <returns></returns>
        async Task CreateBrowserAndPage()
        {
            var options = new LaunchOptions { Headless = true };

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            this.browser = await Puppeteer.LaunchAsync(options);
            this.page = await browser.NewPageAsync();
        }

        /// <summary> Destroys the headless chrome browser and the page object.
        /// </summary>
        /// <returns></returns>
        async Task DestroyBrowserAndPage()
        {
            await this.page.CloseAsync();
            await this.browser.CloseAsync();

            this.page = null;
            this.browser = null;
        }

        /// <summary> This function uses PuppeteerSharp module to create a headless chrome which it 
        /// then uses to authenticate on our web service through our SAML library.
        /// </summary>
        /// <returns></returns>
        async Task<string> DoLoginAsync()
        {
            Console.WriteLine("Navigating to SSO url: ");

            string saml_response = null;

            AuthRequest auth = new AuthRequest(
                Constants.APP_ID,        // put your app's "unique ID" here
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

            await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "login_0.png");
            await page.WaitForNavigationAsync();

            // sign in with our username + password
            await DoEnterUsernameAsync();
            await DoSumitUsernameAsync();
            await DoEnterPasswordAsync();
            await DoSignInAsync();

            await page.ClickAsync(Constants.STAY_SIGNED_IN_BUTTON_SELECTOR);

            // this is actually a request from the azure IDP
            WaitForOptions w = new WaitForOptions();
            w.Timeout = 10000;

            var response = await page.WaitForRequestAsync(Constants.REPLY_URL, w);
            await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "login_5.png");

            saml_response = response.PostData.ToString();
            return saml_response;
        }

        async Task DoLoginIncorrectAsync()
        {
            Console.WriteLine("Navigating to SSO url: ");

            string saml_response = null;

            AuthRequest auth = new AuthRequest(
                Constants.APP_ID,        // put your app's "unique ID" here
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

            await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "login_0.png");
            await page.WaitForNavigationAsync();

            // sign in with our username + password
            await DoEnterUsernameAsync();
            await DoSumitUsernameAsync();
            await DoEnterPasswordAsync();
            await DoSignInAsync();
        }

        async Task DoEnterUsernameAsync()
        {
            await page.TypeAsync(Constants.USERNAME_SELECTOR, this.username);
            await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "login_1.png");
        }

        async Task DoSumitUsernameAsync()
        {
            await page.ClickAsync(Constants.NEXT_BUTTON_SELECTOR);
            await page.WaitForNavigationAsync();
            await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "login_2.png");
        }

        async Task DoEnterPasswordAsync()
        {
            await page.TypeAsync(Constants.PASSWORD_SELECTOR, this.password);
            await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "login_3.png");
        }

        async Task DoSignInAsync()
        {
            await page.ClickAsync(Constants.SIGN_IN_BUTTON_SELECTOR);
            await page.WaitForNavigationAsync();
            await page.ScreenshotAsync(Constants.SCREENHOST_PATH + "login_4.png");
        }

        void SetUsername(string username)
        {
            this.username = username;
        }

        void SetPassword(string password)
        {
            this.password = password;
        }
    }
}
