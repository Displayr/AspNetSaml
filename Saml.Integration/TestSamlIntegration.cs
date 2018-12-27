using Microsoft.VisualStudio.TestTools.UnitTesting;
using PuppeteerSharp;
using System.Threading.Tasks;
using System.Web;

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
            await CreateBrowserAndPageAsync();

            SetUsername(Constants.USERNAME);
            SetPassword(Constants.PASSWORD);

            string saml_response = await DoLoginAsync();

            await DestroyBrowserAndPageAsync();

            // grab the SAML token provided by the IDP and ensure 
            // and ensure it contains relevant information
            // string saml_response = response.PostData.ToString();
            string saml_url_decoded = HttpUtility.UrlDecode(saml_response);

            MicrosoftResponse ms_response = new MicrosoftResponse(Constants.VALID_CERTIFICATE);
            ms_response.LoadXml(saml_url_decoded);

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
            await CreateBrowserAndPageAsync();

            SetUsername(Constants.USERNAME);
            SetPassword(Constants.PASSWORD);

            string saml_response = await DoLoginAsync();
            string logout_response = await DoLogoutAsync();

            // navigating back to the SSO login url should prompt us to login again 
            AuthRequest auth = new AuthRequest(
               Constants.APP_ID,        // put your app's "unique ID" here
               Constants.REPLY_URL      // assertion Consumer Url - the redirect URL where the provider will send authenticated users
           );

            // goto the micrsoft login page
            string sso_redirect = auth.GetRedirectUrl(Constants.SAML_ENDPOINT);
            await this.page.GoToAsync(sso_redirect);

            await this.page.SetViewportAsync(new ViewPortOptions
            {
                Width = 2560,
                Height = 1080
            });

            await this.page.WaitForNavigationAsync();
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "re_login.png");

            var element = await this.page.WaitForSelectorAsync(Constants.USERNAME_SELECTOR);

            await DestroyBrowserAndPageAsync();
        }

        /// <summary> This test ensures that that the webpage maintains state and does not prompt the user 
        /// to provide login details after previously signing in.
        /// </summary>
        /// <returns></returns>
        [TestMethod()]
        public async Task TestAlreadySignedIn()
        {
            await CreateBrowserAndPageAsync();

            SetUsername(Constants.USERNAME);
            SetPassword(Constants.PASSWORD);

            string saml_response = await DoLoginAsync();

            // try to navigate to the web service again
            // we should already be logged in 
            await this.page.GoToAsync(Constants.HOME_PAGE_URL);
            await this.page.WaitForNavigationAsync();
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_again.png");

            // extract the display name from the page
            var element = await this.page.QuerySelectorAsync(Constants.DISPLAY_NAME_SELECTOR);
            string display_name = await element.EvaluateFunctionAsync<string>(Constants.RETURN_INNER_TEXT);

            await DestroyBrowserAndPageAsync();

            Assert.AreEqual("Username: intern1@DISPLAYRSAMLTEST.onmicrosoft.com", display_name);
        }

        /// <summary> Instantiates a headless chrome browser and a page object to hold the rendered 
        /// objects.
        /// </summary>
        /// <returns></returns>
        async Task CreateBrowserAndPageAsync()
        {
            var options = new LaunchOptions { Headless = true };

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            this.browser = await Puppeteer.LaunchAsync(options);
            this.page = await browser.NewPageAsync();
        }

        /// <summary> Destroys the headless chrome browser and the page object.
        /// </summary>
        /// <returns></returns>
        async Task DestroyBrowserAndPageAsync()
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
            AuthRequest auth = new AuthRequest(
                Constants.APP_ID,        // put your app's "unique ID" here
                Constants.REPLY_URL      // assertion Consumer Url - the redirect URL where the provider will send authenticated users
            );

            // goto the micrsoft login page
            string sso_redirect = auth.GetRedirectUrl(Constants.SAML_ENDPOINT);
            await this.page.GoToAsync(sso_redirect);

            await this.page.SetViewportAsync(new ViewPortOptions
            {
                Width = 2560,
                Height = 1080
            });

            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_0.png");
            await this.page.WaitForNavigationAsync();

            // sign in with our username + password
            await DoEnterUsernameAsync();
            await DoSumitUsernameAsync();
            await DoEnterPasswordAsync();
            await DoSignInAsync();

            await this.page.ClickAsync(Constants.STAY_SIGNED_IN_BUTTON_SELECTOR);
            await this.page.WaitForNavigationAsync();

            // extract the XMl from the page 
            //string page_contents = await this.page.GetContentAsync();

            var element = await this.page.WaitForSelectorAsync("body > pre");
            string saml_response = await element.EvaluateFunctionAsync<string>(Constants.RETURN_INNER_TEXT);
            
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_5.png");

            return saml_response;
        }

        /// <summary> This function uses the PuppeteerSharp module to perform single sign out. 
        /// This is assuming the user is already logged in.
        /// </summary>
        /// <returns></returns>
        async Task<string> DoLogoutAsync()
        {
            await this.page.GoToAsync(Constants.SIGNOUT_URL);

            await this.page.SetViewportAsync(new ViewPortOptions
            {
                Width = 2560,
                Height = 1080
            });

            await this.page.WaitForNavigationAsync();
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "logout.png");

            return null;
        }

        async Task DoEnterUsernameAsync()
        {
            await this.page.TypeAsync(Constants.USERNAME_SELECTOR, this.username);
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_1.png");
        }

        async Task DoSumitUsernameAsync()
        {
            await this.page.ClickAsync(Constants.NEXT_BUTTON_SELECTOR);
            await this.page.WaitForNavigationAsync();
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_2.png");
        }

        async Task DoEnterPasswordAsync()
        {
            await this.page.TypeAsync(Constants.PASSWORD_SELECTOR, this.password);
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_3.png");
        }

        async Task DoSignInAsync()
        {
            await this.page.ClickAsync(Constants.SIGN_IN_BUTTON_SELECTOR);
            await this.page.WaitForNavigationAsync();
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_4.png");
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
