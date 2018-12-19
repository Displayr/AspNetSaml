using System.Collections.Generic;
using System.Xml;

namespace Saml
{
    public class MicrosoftResponse : Response
    {
        public MicrosoftResponse(string certificate_str) : base(certificate_str)
        {
        }

        public override string GetDisplayName()
        {
            XmlNode node = base._xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dname']/saml:AttributeValue", _xmlNameSpaceManager);
            return node == null ? null : node.InnerText;
        }

        public override string GetEmail()
        {
            XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']/saml:AttributeValue", _xmlNameSpaceManager);
            return node == null ? null : node.InnerText;
        }

        public override string GetFirstName()
        {
            XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname']/saml:AttributeValue", _xmlNameSpaceManager);
            return node == null ? null : node.InnerText;
        }

        public override string GetLastName()
        {
            XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname']/saml:AttributeValue", _xmlNameSpaceManager);
            return node == null ? null : node.InnerText;
        }

        public override string GetDepartment()
        {
            XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Department']/saml:AttributeValue", _xmlNameSpaceManager);
            return node == null ? null : node.InnerText;
        }

        public override string GetPhone()
        {
            XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Phone number']/saml:AttributeValue", _xmlNameSpaceManager);
            return node == null ? null : node.InnerText;
        }

        public override string GetCompany()
        {
            XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Company']/saml:AttributeValue", _xmlNameSpaceManager);
            return node == null ? null : node.InnerText;
        }

        public override List<string> GetGroups()
        {
            // this is only valid for azure claims.
            XmlNodeList node_list = _xmlDoc.SelectNodes("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.microsoft.com/ws/2008/06/identity/claims/groups']/saml:AttributeValue", _xmlNameSpaceManager);

            List<string> group_ids = new List<string>();

            foreach (XmlNode node in node_list)
            {
                group_ids.Add(node.InnerText);
            }

            return group_ids;
        }
    }
}
