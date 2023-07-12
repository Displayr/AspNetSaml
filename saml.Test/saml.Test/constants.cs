﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saml.Test
{
    public class Constants
    {
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

        public const string INVALID_CERTIFICATE = @"-----BEGIN CERTIFICATE-----
AIIC8DCCAdigAwIBAgIQFRsCueNmH6dLF0CkDs5zIzANBgkqhkiG9w0BAQsFADA0MTIwMAYDVQQD
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

        public const string EMPTY_CERTIFICATE = @"-----BEGIN CERTIFICATE-----
-----END CERTIFICATE-----";

        public const string VALID_XML_RESPONSE_RESOURCE     = "Saml.Test.valid_response.xml";
        public const string VALID_XML_WITH_BASE_GROUPS_RESPONSE_RESOURCE = "Saml.Test.valid_response_base_groups.xml";
        public const string INVALID_XML_RESPONSE_RESOURCE   = "Saml.Test.invalid_response.xml";
        public const string EMPTY_XML_RESPONSE_RESOURCE     = "Saml.Test.empty_response.xml";
        public const string MICROSOFT_XML_RESPONSE_RESOURCE = "Saml.Test.microsoft_response_valid.xml";
    }
}
