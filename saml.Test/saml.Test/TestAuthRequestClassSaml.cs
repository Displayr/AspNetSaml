using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Saml.Test
{
    [TestClass]
    public class TestAuthRequestClassSaml
    {
        /// <summary> This test ensures that the appropriate exception is thrown when 
        /// the app ID provided is NULL.
        /// </summary>
        [TestMethod]
        public void TestGetRequestAppIdNull()
        {
            AuthRequest auth_request = new AuthRequest(null, null);

            string request = auth_request.GetRequest(AuthRequest.AuthRequestFormat.Base64);
            Console.WriteLine(request);
        }

        /// <summary> This test ensures that the appropriate exception is thrown when reply url 
        /// provided is invalid e.g www.google.casdvvdsfm.
        /// </summary>
        [TestMethod]
        public void TestGetRequestInvalidReplyUrl()
        {
            AuthRequest 
        }

    }
}
