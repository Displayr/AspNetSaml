using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Saml.Test
{
    [TestClass]
    public class TestMicrosoftResponse
    {
        /// <summary> This test ensures that microsoft response class extracts the correct data.
        /// </summary>
        [TestMethod()]
        public void TestGetters()
        {
            Response response = new Response(Constants.VALID_CERTIFICATE);

            string xml_contents = GetResourceContents(Constants.MICROSOFT_XML_RESPONSE_RESOURCE);
            response.LoadXml(xml_contents);

            string name_id          = response.GetNameID();
            string display_name     = response.GetDisplayName();
            string email_address    = response.GetEmail();
            string first_name       = response.GetFirstName();
            string last_name        = response.GetLastName();
            string department       = response.GetDepartment();
            string phone_number     = response.GetPhone();
            string company          = response.GetCompany();
            List<string> groups     = response.GetGroups();

            Assert.AreEqual("interns@displayr.com", name_id);
            Assert.AreEqual("Intern one (correct)", display_name);
            Assert.AreEqual("intern.one@displayr.com", email_address);
            Assert.AreEqual("Intern", first_name);
            Assert.AreEqual("One", last_name);
            Assert.AreEqual("Programming", department);
            Assert.AreEqual("0451675123", phone_number);
            Assert.AreEqual("Numbers International Pty Ltd", company);

            Assert.AreEqual("af95480a-5829-4aba-be82-12d633fcfa5a", groups[0]);
            Assert.AreEqual("461ee5e9-6499-4f7d-b0ba-735dcc44fdc1", groups[1]);
            Assert.AreEqual("11b4175a-a8b9-4ea5-b9b2-697223cc51f5", groups[2]);
        }

        /// <summary> This function returns the contents of a resource file.</summary>
        /// <param name="resource_path"></param>
        /// <returns></returns>
        private string GetResourceContents(string resource_path)
        {
            var stream = typeof(TestMicrosoftResponse).Assembly.GetManifestResourceStream(resource_path);
            string resource_contents = new StreamReader(stream).ReadToEnd();
            return resource_contents;
        }
    }
}