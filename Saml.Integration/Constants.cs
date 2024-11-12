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

        public const string EXPECTED_FIRST_NAME = "intern1";
        public const string USERNAME = "intern1@DISPLAYRSAMLTEST.onmicrosoft.com";
        public const string PASSWORD = "Testmyapp6";

        // Azure Active Directory > Saml Single Signon Test Organisation
        // GUID here is our Active Directory (see above) tenant ID.
        public const string SAML_ENDPOINT = @"https://login.microsoftonline.com/86c4efd3-7f59-4e51-8f64-6d7848dfcaef/saml2";
        public const string SIGNOUT_URL = "https://login.microsoftonline.com/common/wsfederation?wa=wsignout1.0";

        // ... > Enterprise Applications > Displayr (saml-test-master)
        public const string APP_ID = "2cb753ec-8174-4f4d-8bb2-a3a50de9cd29";
        // ... > Single signon on > Basic SAML Configuration.
        public const string REPLY_URL = @"https://functions-csharp.displayr.com/api/SamlTestIdentityProviderResponse?code=lLl0kDBzXIz84eiJJKRtq7N4fD35b0nd24R95HJqLq5X5bgfCkhcVg==";
        // ... > SAML Signing Certificate
        public const string VALID_CERTIFICATE = @"-----BEGIN CERTIFICATE-----
MIIC8DCCAdigAwIBAgIQaBn3qbDVJ7ZDYJ1vNoM2mzANBgkqhkiG9w0BAQsFADA0MTIwMAYDVQQD
EylNaWNyb3NvZnQgQXp1cmUgRmVkZXJhdGVkIFNTTyBDZXJ0aWZpY2F0ZTAeFw0yMDEyMTcyMzU4
NThaFw0yMzEyMTcyMzU4NThaMDQxMjAwBgNVBAMTKU1pY3Jvc29mdCBBenVyZSBGZWRlcmF0ZWQg
U1NPIENlcnRpZmljYXRlMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA5WucU1KB6d6L
HFz3TkkQdSE09PVd7VoM0gVtWoaGhywJKRyRykA+nHr1kJ2NwJ51h0jn6dpdXElI7Jr91mXbzmQA
1wJz9Sf+ham1FFQfCY46Y+kAPKj5U4W8Rr3mRNfQc3YVIeLLZwoRz8by8Bp/0SGvaaD2JSmx1F9E
mtl8YQQfDF1WBzkkDvDEIWqRKDNiA9tgdOUqjpSHGkIMKBXM2POeyi753nnIgAVtL93c0IIn7txH
AojOFbIA+mFAL8j/3mxk1RVDgUe3ts4GAyWEhleuA0vtIMqjLsodLbbFaJt3Bwtn6BDWOKuIW8bD
VFPmDYOAhklvNtf1qHJGfx/qdQIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQCuf5yi99qZbMnA40aZ
CSyZ8FL0QkQ9Ts+MWMdw3WnqiKxthfpW0xK/x+P2ym3860Kvji6KoIcCBDKQHz7ApLS/PROSh2xo
8kkMHjHGxhHq6nctxXpozUJsxj9xpnJ5+VqeD9O8MArWTXfIW1G652UfSTQcqelwglQ4LMd2FK3S
izr81KNsVYSbXmk2GpA3/YWrfFYQBSGzyf1dwYrr2Kq1WWP2gyKLfc84LsHcz0/RivMdedWn91Va
0KSXA3g/tcxt/mmw0VRzAQsRnbulE3k4F3jKsm5saZCGZfi60OwGbJokZZ6Wx1gzuvAKyIjdYVr0
y3eWF9z8XhuBKGWyZ7n0
-----END CERTIFICATE-----";

        public const string XML_CONTENTS_SELECTOR = "body > pre";

        public const string RETURN_INNER_TEXT_FUNC = "(element) => { return element.innerText; }";

        /// <summary> This function takes in the name of a HTML element and returns a javascript function 
        /// that can find the corresponding ID of the element. The javascript function is returned as a string.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetSelectorIdJavascript(string name)
        {
            return  "() => {" +
                        "var node_list = document.getElementsByName('" + name + "');" +
                        "var id = node_list[0].id;" +
                        "console.log('" + name + "'); console.log(node_list); console.log(id); " +
                        "return id;" +
                    "}";
        }

        /// <summary> This function takes in the value of an element and returns a javascript function 
        /// that performs a click() on the corresponding element.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ClickButtonJavascript(string value)
        {
            return "() => { " +
                        "var n = document.querySelectorAll(\"input[value = \'" + value + "\']\"); " +
                        "n[0].click();" +
                    "}";
        }
    }
}
