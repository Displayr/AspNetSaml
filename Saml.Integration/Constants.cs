using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saml.Integration
{
    public class Constants
    {
        public const string SCREENSHOT_PATH = "../../Screenshots/";

        public const string EXPECTED_DISPLAY_NAME = "intern1";
        public const string USERNAME = "intern1@DISPLAYRSAMLTEST.onmicrosoft.com";
        public const string PASSWORD = "Testmyapp5";

        public const string REPLY_URL = @"https://saml-test55.azurewebsites.net/api/HttpTrigger1?code=EnzGd2u543rsIDiR1vaRHQNmbhmSdUZhN5xW2VoA5atqssI5fjhq9w==";

        public const string APP_ID = "15eedc3e-ead5-47c8-8424-a98027d91da7";
        public const string SAML_ENDPOINT = @"https://login.microsoftonline.com/86c4efd3-7f59-4e51-8f64-6d7848dfcaef/saml2";
        public const string SIGNOUT_URL = "https://login.microsoftonline.com/common/wsfederation?wa=wsignout1.0";

        public const string XML_CONTENTS_SELECTOR = "body > pre";

        public const string RETURN_INNER_TEXT_FUNC = "(element) => { return element.innerText; }";

        // Azure's certificate 
        public const string VALID_CERTIFICATE = @"-----BEGIN CERTIFICATE-----
MIIC8DCCAdigAwIBAgIQFRsCueNmH6dLF0CkDs5zIzANBgkqhkiG9w0BAQsFADA0MTIwMAYDVQQD
EylNaWNyb3NvZnQgQXp1cmUgRmVkZXJhdGVkIFNTTyBDZXJ0aWZpY2F0ZTAeFw0xODEyMDMwMTQ4
MjVaFw0yMTEyMDMwMTQ4MjVaMDQxMjAwBgNVBAMTKU1pY3Jvc29mdCBBenVyZSBGZWRlcmF0ZWQg
U1NPIENlcnRpZmljYXRlMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAx8WKyZTYdJcC
B8Yh2Y/cs1U1/DXgNc//Wt9JROTv95j1YEtkLMY1/F2hyBKql9Ypn1i2Zv7d2cQ85GGnVYi3tKiE
LpgXzc9eebyYholNbAgFO7kL2Zum2gUvHAWXB8qSIisJWTU46fCZ/0VUy8I+/j0S9q2Ats1slK1+
rZ7KixOXq/taRMI9f0xo8l5EIpaQiMPw5NdcLLZXwPaYK973+ec3CtuauHHgGHN8jmgvA0LSWBlv
zAsjcFLb+TCthQxsCPmws/1P8KRln7RxjJqbeIdt5TslbgmxSJ7pAlsb+8qUPoYRlpwIsERX94kZ
q/NqwAtgH58ruVDz8zrn8aH8fQIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQAtIGv9jyjF4wxS8LTx
w/tfJi3jOr5Z54DRY3z1oKRtEvz907fIL9oRNhL4L+VsEyi1dGMqFAbysY8z7Pe5f/Za1RwSEVF5
RjxHydnckLaguquZwfLGBHtJnCeF2LeAVS1WbWowCNjiOJ9l/q+BCzlXZRTV4S9TrXp33hGYob6o
wgiz1CflTYIfj6h/yyM80QBaQwa4zNjS4UbgHb+SK+x/YH/BY6SFB9WqqSUbwPTkxGKNxC/A8CX8
tpSXlVyXjN2Y7UsDaCkj/TGRSatTqPA6GIKx3AQMAvs/wkla2iHWoQYEn/fN6Mwc10i7M4AevA81
unr4TOQQAYtnBQT4DCGs
-----END CERTIFICATE-----";

        /// <summary> This function takes the name of a HTML element and returns its id.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetSelectorIdJavascript(string name)
        {
            return "() => {" +
                "var node_list = document.getElementsByName('" + name + "');" +
                "var id = node_list[0].id;" +
                "console.log('" + name + "'); console.log(node_list); console.log(id); " +
                "return id;" +
            "}";
        }

        public static string ClickButtonJavascript(string value)
        {
            return "() => { var n = document.querySelectorAll(\"input[value = \'" + value + "\']\"); " +
                "n[0].click();" +
                "}";
        }
    }
}
