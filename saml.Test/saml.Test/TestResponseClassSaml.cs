﻿using Saml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;

namespace Saml.Test
{
    [TestClass]
    public class TestResponseClassSaml
    {
        /// <summary>
        /// This test method ensures that the getter functions return the correct information.
        /// </summary>
        [TestMethod()]
        public void TestGetters()
        {
            Response response = new Response(Constants.VALID_CERTIFICATE);

            string xml_contents = GetResourceContents(Constants.VALID_XML_RESPONSE_RESOURCE);

            response.LoadXml(xml_contents);

            string display_name     = response.GetDisplayName();
            string first_name       = response.GetFirstName();
            string surname          = response.GetLastName();
            string email_address    = response.GetEmail();
            string name_id          = response.GetNameID();
            string department       = response.GetDepartment();
            string homephone        = response.GetPhone();
            string company_name     = response.GetCompany();
            string audience = response.Audience;

            List<string> groups     = response.GetGroups();

            Assert.AreEqual("George Willy", display_name);
            Assert.AreEqual("George", first_name);
            Assert.AreEqual("Willy", surname);
            Assert.AreEqual("george@george.com", email_address);
            Assert.AreEqual("george@george.com", name_id);
            Assert.AreEqual("Car repairs", department);
            Assert.AreEqual("04567843543", homephone);
            Assert.AreEqual("Displayr", company_name);
            Assert.AreEqual("spn:15eedc3e-ead5-47c8-8424-a98027d91da7", audience);

            Assert.AreEqual(2, groups.Count);
            Assert.AreEqual("12345678-testing1-testing2", groups[0]);
            Assert.AreEqual("87654321-testing4-testing3", groups[1]);
        }

        // <summary>
        /// This test method ensures that the groups getter works with the non MS groups name to.
        /// </summary>
        [TestMethod()]
        public void TestGroupsGetter()
        {
            // Certificate here is unimportant to actual response as we wont validate, just needs to be a valid cert.
            Response response = new Response(Constants.VALID_CERTIFICATE);

            string xml_contents = GetResourceContents(Constants.VALID_XML_WITH_BASE_GROUPS_RESPONSE_RESOURCE);

            response.LoadXml(xml_contents);

            List<string> groups = response.GetGroups();

            Assert.AreEqual(2, groups.Count);
            Assert.AreEqual("12345678-testing1-testing2", groups[0]);
            Assert.AreEqual("87654321-testing4-testing3", groups[1]);
        }

        /// <summary>This test method loads an empty XML SAML response and a legitimate certificate
        /// into the 'Response' class of the SAML library. The test ensures that the appropriate 
        /// exception is thrown.</summary>
        [TestMethod()]
        public void TestEmptyXml()
        {
            Response response = new Response(Constants.VALID_CERTIFICATE);

            string xml_contents = GetResourceContents(Constants.EMPTY_XML_RESPONSE_RESOURCE);

            var load_xml_exception = Assert.ThrowsException<XmlException>(() => { response.LoadXml(xml_contents); });
            Assert.IsTrue(load_xml_exception is XmlException);
            Assert.IsTrue(load_xml_exception.Message.StartsWith("Unexpected end tag"));
        }

        /// <summary>This test method loads an invalid certificate
        /// into the 'Response' class of the SAML library. This method tests to see 
        /// if the correct exceptions are thrown.</summary>
        [TestMethod]
        public void TestInvalidCertificate()
        {
            Response response = null;

            var exception = Assert.ThrowsException<LoadCertificateException>(() => { response = new Response(Constants.INVALID_CERTIFICATE);  });

            Assert.IsTrue(exception is LoadCertificateException);
        }

        /// <summary>This test method tests whether an exception is raised when the XML returned by the IDP
        /// is invalid i.e contains bad chars. 
        /// For this method, a valid certificate is used.</summary>
        [TestMethod]
        public void TestInvalidXml()
        { 
            Response response = new Response(Constants.VALID_CERTIFICATE);

            string xml_contents = GetResourceContents(Constants.INVALID_XML_RESPONSE_RESOURCE);
            
            var load_xml_exception = Assert.ThrowsException<XmlException>(() => { response.LoadXml(xml_contents); });

            Assert.IsFalse(response.IsValid());
            Assert.IsTrue(load_xml_exception is XmlException);
        }

        /// <summary>
        /// This test ensures that the library generates the appropriate exception 
        /// when an empty certificate is loaded.
        /// </summary>
        [TestMethod]
        public void TestResponseEmptyCertificate()
        {
            Response response = null;

            var exception = Assert.ThrowsException<LoadCertificateException>(() => { response = new Response(Constants.EMPTY_CERTIFICATE);  });
            Assert.IsTrue(exception is LoadCertificateException);
        }

        /// <summary> This function returns the contents of a resource file.</summary>
        /// <param name="resource_path"></param>
        /// <returns></returns>
        private string GetResourceContents(string resource_path)
        {
            var stream = typeof(TestResponseClassSaml).Assembly.GetManifestResourceStream(resource_path);
            string resource_contents = new StreamReader(stream).ReadToEnd();
            return resource_contents;
        }
    }
}
