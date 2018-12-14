using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml

namespace Saml
{
    class MicrosoftResponse : Response
    {
        public MicrosoftResponse(string certificate_str) : base(certificate_str)
        {

        }

        public string GetDisplayName()
        {
            XmlNode node = base._xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.microsoft.com/identity/claims/displayname']/saml:AttributeValue", _xmlNameSpaceManager);
            return node == null ? null : node.InnerText;
        }
    }
}
