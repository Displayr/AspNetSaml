#r "Newtonsoft.Json"

using System.Net;
using System.Xml;
using System.Web;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Net.Http.Headers;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("SAMLResponse: ");

    string saml_response = await new StreamReader(req.Body).ReadToEndAsync();

    log.LogInformation("[Post]: " + saml_response);

    XmlDocument xml_doc = new XmlDocument();
    System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();

    // remove the SAMLResponse variable as well as any url encoding
    int index = saml_response.IndexOf("SAMLResponse=");
    int len = "SAMLResponse=".Length;
    saml_response = saml_response.Substring(index + len);
    saml_response = HttpUtility.UrlDecode(saml_response);

    string xml = enc.GetString(Convert.FromBase64String(saml_response));
    xml_doc.PreserveWhitespace = true;
    xml_doc.XmlResolver = null;
    xml_doc.LoadXml(xml);

    string beautiful_xml = BeautifyXml(xml_doc);

    return saml_response != null
        ? (ActionResult)new OkObjectResult($"{beautiful_xml}")
        : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
}

// Formats the XML document with line breaks and indentation.
// Acquired from: https://stackoverflow.com/questions/203528/what-is-the-simplest-way-to-get-indented-xml-with-line-breaks-from-xmldocument
public static string BeautifyXml(XmlDocument doc)
{
    StringBuilder sb = new StringBuilder();
    XmlWriterSettings settings = new XmlWriterSettings
    {
        Indent = true,
        IndentChars = "  ",
        NewLineChars = "\r\n",
        NewLineHandling = NewLineHandling.Replace
    };
    using (XmlWriter writer = XmlWriter.Create(sb, settings)) {
        doc.Save(writer);
    }
    return sb.ToString();
}


