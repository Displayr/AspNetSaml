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

            Assert.AreEqual(Constants.EXPECTED_DISPLAY_NAME, display_name);
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

            await DoLoginAsync();
            await DoLogoutAsync();

            string sso_redirect = GetRedirectUrl();
            await this.page.GoToAsync(sso_redirect);

            await this.page.SetViewportAsync(new ViewPortOptions
            {
                Width = 2560,
                Height = 1080
            });

            await this.page.WaitForNavigationAsync();
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "signed_out.png");

            // verify that we need to login again 
            string title = await this.page.GetTitleAsync();

            await DestroyBrowserAndPageAsync();

            Assert.AreEqual("Sign in to your account", title);
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

            await DoLoginAsync();

            // goto the micrsoft login page again and the SAMl response should be shown
            string sso_redirect = GetRedirectUrl();

            await this.page.GoToAsync(sso_redirect);
            await this.page.WaitForNavigationAsync();
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_again.png");

            string saml_response = await ExtractXmlAsync();
            string saml_url_decoded = HttpUtility.UrlDecode(saml_response);

            MicrosoftResponse ms_response = new MicrosoftResponse(Constants.VALID_CERTIFICATE);
            ms_response.LoadXml(saml_url_decoded);

            string display_name = ms_response.GetDisplayName();

            await DestroyBrowserAndPageAsync();

            Assert.AreEqual(Constants.EXPECTED_DISPLAY_NAME, display_name);
        }

        /// <summary> Instantiates a headless chrome browser and a page object to hold the rendered 
        /// objects.
        /// </summary>
        /// <returns></returns>
        async Task CreateBrowserAndPageAsync()
        {
            var options = new LaunchOptions { Headless = false };

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
            await this.page.SetViewportAsync(new ViewPortOptions
            {
                Width = 2560,
                Height = 1080
            });

            string redirect = GetRedirectUrl();
            await this.page.GoToAsync(redirect);
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_0.png");
            await this.page.WaitForNavigationAsync();

            // sign in with our username + password
            await DoEnterUsernameAsync();
            await DoSumitUsernameAsync();
            await DoEnterPasswordAsync();
            await DoSignInAsync();

            await this.page.EvaluateFunctionAsync(Constants.ClickButtonJavascript("No"));
            await this.page.WaitForNavigationAsync();

            // extract the XMl from the page 
            //string page_contents = await this.page.GetContentAsync();
            string saml_response = await ExtractXmlAsync();
            
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_5.png");

            return saml_response;
        }

        /// <summary> This function uses the PuppeteerSharp module to perform single sign out. 
        /// This is assuming the user is already logged in.
        /// </summary>
        /// <returns></returns>
        async Task DoLogoutAsync()
        {
            await this.page.GoToAsync(Constants.SIGNOUT_URL);

            await this.page.SetViewportAsync(new ViewPortOptions
            {
                Width = 2560,
                Height = 1080
            });

            await this.page.WaitForNavigationAsync();
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "logout.png");
        }

        /// <summary> Extracts the XML content that is returned from the Azure function "saml-test55"
        /// </summary>
        /// <returns></returns>
        async Task<string> ExtractXmlAsync()
        {
            var element = await this.page.WaitForSelectorAsync(Constants.XML_CONTENTS_SELECTOR);
            string xml = await element.EvaluateFunctionAsync<string>(Constants.RETURN_INNER_TEXT_FUNC);
            return xml;
        }

        /// <summary> Types our password into the input box on the Microsoft login page.
        /// </summary>
        /// <returns></returns>
        async Task DoEnterUsernameAsync()
        {
            string javascript_func = Constants.GetSelectorIdJavascript("loginfmt");
            string selector = await this.page.EvaluateFunctionAsync<string>(javascript_func);

            Assert.IsNotNull(selector);

            var element = await this.page.WaitForSelectorAsync("#" + selector);
            await element.TypeAsync(Constants.USERNAME);
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_1.png");
        }

        /// <summary> Submits the entered username at the login page and waits for the browser to 
        /// progress to the next page (password page).
        /// </summary>
        /// <returns></returns>
        async Task DoSumitUsernameAsync()
        {
            await this.page.EvaluateFunctionAsync(Constants.ClickButtonJavascript("Next"));
            await this.page.WaitForNavigationAsync();
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_2.png");
        }

        /// <summary> Types the password into the password field at the login page.
        /// </summary>
        /// <returns></returns>
        async Task DoEnterPasswordAsync()
        {
            string javascript_func = Constants.GetSelectorIdJavascript("passwd");
            var selector = await this.page.EvaluateFunctionAsync<string>(javascript_func);

            Assert.IsNotNull(selector);

            var element = await this.page.WaitForSelectorAsync("#" + selector);
            await element.TypeAsync(Constants.PASSWORD);
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_3.png");
        }

        /// <summary> Clicks the signin button on the Microsoft login page and waits for the 
        /// browser navigate to the "stay signed in?" dialog.
        /// </summary>
        /// <returns></returns>
        async Task DoSignInAsync()
        {
            await this.page.EvaluateFunctionAsync(Constants.ClickButtonJavascript("Sign in"));
            await this.page.WaitForNavigationAsync();
            await this.page.ScreenshotAsync(Constants.SCREENSHOT_PATH + "login_4.png");
        }

        /// <summary> Sets the username to use to log into Microsoft Azure Active Directory.
        /// </summary>
        /// <param name="username"></param>
        void SetUsername(string username)
        {
            this.username = username;
        }

        /// <summary> Sets the password corresponding to the username 
        /// </summary>
        /// <param name="username"></param>
        void SetPassword(string password)
        {
            this.password = password;
        }

        /// <summary> Returns the url used to perform SAML single sign in with Azure active directory.
        /// </summary>
        /// <returns></returns>
        string GetRedirectUrl()
        {
            AuthRequest auth = new AuthRequest(
                Constants.APP_ID,        // put your app's "unique ID" here
                Constants.REPLY_URL      // assertion Consumer Url - the redirect URL where the provider will send authenticated users
            );

            // goto the micrsoft login page
            string sso_redirect = auth.GetRedirectUrl(Constants.SAML_ENDPOINT);
            return sso_redirect;
        }
    }
}
