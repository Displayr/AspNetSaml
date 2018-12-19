using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saml.Integration
{
    class Constants
    {
        public const string SCREENHOST_PATH = "../../Screenshots/";
        public const string USERNAME_SELECTOR = "#i0116";
        public const string PASSWORD_SELECTOR = "#i0118";
        public const string NEXT_BUTTON_SELECTOR = "#idSIButton9";
        public const string SIGN_IN_BUTTON_SELECTOR = "#idSIButton9";
        public const string NOT_STAY_SIGNED_IN_BUTTON_SELECTOR = "#idBtn_Back";

        public const string USERNAME = "intern1@DISPLAYRSAMLTEST.onmicrosoft.com";
        public const string PASSWORD = "Testmyapp5";

        public const string REPLY_URL = "https://localhost:44376/Home/WelcomeUser";
        public const string APP_ID = "15eedc3e-ead5-47c8-8424-a98027d91da7";
        public const string SAML_ENDPOINT = "https://login.microsoftonline.com/86c4efd3-7f59-4e51-8f64-6d7848dfcaef/saml2";

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
    }
}
