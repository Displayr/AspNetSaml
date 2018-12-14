using System;
using System.IO;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace Saml.Test
{
    [TestClass]
    public class TestAuthRequestClassSaml
    {
        const string SAML_ENDPOINT = "https://login.microsoftonline.com/86c4efd3-7f59-4e51-8f64-6d7848dfcaef/saml2";
        const int SAML_REQUEST_OFFSET = 13;

        const string TEST_SERVICE_PROVIDER_URL = "dummy.com";
        const string TEST_REPLY_URL = "dummy.com/reply";

        /// <summary> This test ensures that the class throws a NullReferenceException when 
        /// the issuer or the assertionConsumerServiceUrl is null.
        /// </summary>
        [TestMethod()]
        public void TestGetRequestAppIdNull()
        { 
            Assert.ThrowsException<NullReferenceException>(() => { AuthRequest auth_request = new AuthRequest(null, null); });
        }

        /// <summary> This test ensures that the correct redirect url is returned.
        /// </summary>
        [TestMethod()]
        public void TestGetReplyUrl()
        {
            AuthRequest auth_request = new AuthRequest(TEST_SERVICE_PROVIDER_URL, TEST_REPLY_URL);

            string redirect_url = auth_request.GetRedirectUrl(SAML_ENDPOINT);

            // reverse the process applied by the SAML library to get the request XML
            // Undo UrlEncode => Undo Base64
            string saml_request             = redirect_url.Substring(SAML_ENDPOINT.Length + SAML_REQUEST_OFFSET);
            string url_param_decoded        = HttpUtility.UrlDecode(saml_request);
            byte[] base64_decoded_bytes     = Convert.FromBase64String(url_param_decoded);

            // apply the deflate decompression algorithm to decompress the result
            var memory_stream = new MemoryStream(base64_decoded_bytes);
            var deflate_stream = new DeflateStream(memory_stream, CompressionMode.Decompress);
            byte[] output_array = new byte[1000];

            deflate_stream.Read(output_array, 0, 1000);
            string xml_string = Encoding.UTF8.GetString(output_array);

            // convert the decompressed bytes into an XMl document
            XmlDocument xml_doc = new XmlDocument();
            xml_doc.LoadXml(xml_string);

            // microsoft complains about not using a namespace
            XmlNamespaceManager manager = new XmlNamespaceManager(xml_doc.NameTable);
            manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");

            XmlNode node = xml_doc.SelectSingleNode("/samlp:AuthnRequest", manager);
            XmlAttributeCollection node_attributes = node.Attributes;
            string reply_url = node_attributes["AssertionConsumerServiceURL"].Value;
    
            node = xml_doc.SelectSingleNode("/samlp:AuthnRequest/saml:Issuer", manager);
            string service_provider_url = node.InnerText;

            Assert.AreEqual(TEST_REPLY_URL, reply_url);
            Assert.AreEqual(TEST_SERVICE_PROVIDER_URL, service_provider_url);
        }

        /// <summary> This test ensures that if the parameter is NULL then an exception is thrown.
        /// </summary>
        [TestMethod()]
        public void TestGetRequestUrlNull()
        {
            AuthRequest auth_request = new AuthRequest(TEST_SERVICE_PROVIDER_URL, TEST_REPLY_URL);

            Assert.ThrowsException<NullReferenceException>(() => { string redirect_url = auth_request.GetRedirectUrl(null); });
        }

        /// <summary> This test ensures that if 
        /// </summary>
        [TestMethod()]
        public void TestGetRequestInvalidEncoding()
        {
            AuthRequest auth_request = new AuthRequest(TEST_SERVICE_PROVIDER_URL, TEST_REPLY_URL);
            string request = auth_request.GetRequest((AuthRequest.AuthRequestFormat)5);
            Assert.IsNull(request);
        }

        // borrowed from the SAML library
        byte[] StringToByteArray(string st)
        {
            byte[] bytes = new byte[st.Length];
            for (int i = 0; i < st.Length; i++)
            {
                bytes[i] = (byte)st[i];
            }
            return bytes;
        }
    }
}
