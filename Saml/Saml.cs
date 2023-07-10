/*	Jitbit's simple SAML 2.0 component for ASP.NET
	https://github.com/jitbit/AspNetSaml/
	(c) Jitbit LP, 2016
	Use this freely under the Apache license (see https://choosealicense.com/licenses/apache-2.0/)
	version 1.2.3
*/
/* Modifications copyright (C) 2021 Displayr Australia Pty Ltd */

using System;
using System.Web;
using System.IO;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.IO.Compression;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;

namespace Saml
{
	/// <summary>
	/// this class adds support of SHA256 signing to .NET 4.0 and earlier
	/// (you can use it in .NET 4.5 too, if you don't want a "System.Deployment" dependency)
	/// </summary>
	public sealed class RSAPKCS1SHA256SignatureDescription : SignatureDescription
	{
		public RSAPKCS1SHA256SignatureDescription()
		{
			KeyAlgorithm = typeof(RSACryptoServiceProvider).FullName;
			DigestAlgorithm = typeof(SHA256Managed).FullName;   // Note - SHA256CryptoServiceProvider is not registered with CryptoConfig
			FormatterAlgorithm = typeof(RSAPKCS1SignatureFormatter).FullName;
			DeformatterAlgorithm = typeof(RSAPKCS1SignatureDeformatter).FullName;
		}

		public override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(key);
			deformatter.SetHashAlgorithm("SHA256");
			return deformatter;
		}

		public override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(key);
			formatter.SetHashAlgorithm("SHA256");
			return formatter;
		}

		private static bool _initialized = false;
		public static void Init()
		{
			if (!_initialized)
				CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
			_initialized = true;
		}
	}

	public class LoadCertificateException : Exception
	{
		public LoadCertificateException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	public interface IResponse
	{
		void LoadXml(string xml);
		void LoadXmlFromBase64(string saml_response);
		string Audience { get; }
		bool IsValid();
		string GetID();
		DateTime GetValidUntilUtc();
		string GetNameID();
		string GetDisplayName();
		string GetEmail();
		string GetFirstName();
		string GetLastName();
		string GetDepartment();
		string GetPhone();
		string GetCompany();
		List<string> GetGroups();
		DateTime? GetSessionEndDate();
	}

	public partial class Response : IResponse
	{
		private static byte[] StringToByteArray(string st)
		{
			byte[] bytes = new byte[st.Length];
			for (int i = 0; i < st.Length; i++) {
				bytes[i] = (byte)st[i];
			}
			return bytes;
		}

		private const string NORMAL_ATTRIBUTE_NAME_PREFIX = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/";
		private const string GROUPS_MS_NAME_PREFIX = "http://schemas.microsoft.com/ws/2008/06/identity/claims/";

		protected XmlDocument _xmlDoc;
		protected readonly X509Certificate2 _certificate;
		protected XmlNamespaceManager _xmlNameSpaceManager; //we need this one to run our XPath queries on the SAML XML

		public string Xml { get { return _xmlDoc.OuterXml; } }

		public Response(string certificateStr, string responseString)
			: this(StringToByteArray(certificateStr), responseString) { }

		public Response(byte[] certificateBytes, string responseString) : this(certificateBytes)
		{
			LoadXmlFromBase64(responseString);
		}

		public Response(string certificateStr) : this(StringToByteArray(certificateStr)) { }

		public Response(byte[] certificateBytes)
		{
			RSAPKCS1SHA256SignatureDescription.Init(); //init the SHA256 crypto provider (for needed for .NET 4.0 and lower)

			try {
				_certificate = new X509Certificate2(certificateBytes);
			} catch (Exception ex) {
				throw new LoadCertificateException("Failed to load certificate", ex);
			}
		}

		public void LoadXml(string xml)
		{
			_xmlDoc = new XmlDocument();
			_xmlDoc.PreserveWhitespace = true;
			_xmlDoc.XmlResolver = null;
			_xmlDoc.LoadXml(xml);

			_xmlNameSpaceManager = GetNamespaceManager(); //lets construct a "manager" for XPath queries
		}

		public void LoadXmlFromBase64(string response)
		{
			UTF8Encoding enc = new UTF8Encoding();
			LoadXml(enc.GetString(Convert.FromBase64String(response)));
		}

		/// <summary>Gets the intended audience of this request. This is intended for a multi-tennant IdP initiated request, where we don't yet know
		/// which tennant in a multi tennanted system this request is for. Once we know we can then use that tennant's certificate to validate the
		/// rest of the request. If an attacker sends a fake Audience value, it doesn't matter as long as you only trust it as far as choosing an appropriate
		/// certificate from your data store and nothing else.</summary>
		public string Audience
		{
			get
			{
				var nodeList = _xmlDoc.SelectNodes("//samlp:Response/saml:Assertion[1]/saml:Conditions/saml:AudienceRestriction/saml:Audience", _xmlNameSpaceManager);
				if (nodeList.Count > 0) {
					return nodeList[0].InnerText;
				}
				return null;
			}
		}

		public DateTime? GetSessionEndDate()
		{
			var auth_statement_node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion[1]/saml:AuthnStatement", _xmlNameSpaceManager);
			if (auth_statement_node != null && auth_statement_node.Attributes["SessionNotOnOrAfter"] != null) {
				if (DateTime.TryParse(auth_statement_node.Attributes["SessionNotOnOrAfter"].Value, out var expirationDate)) {
					return expirationDate;
				}
			}
			return null;
		}

		public bool IsValid()
		{
			XmlNodeList nodeList;

			// We don't want an exception to be thrown from this class.
			try {
				nodeList = _xmlDoc.SelectNodes("//ds:Signature", _xmlNameSpaceManager);
			} catch (Exception) {
				return false;
			}

			SignedXml signedXml = new SignedXml(_xmlDoc);

			if (nodeList.Count == 0) return false;

			signedXml.LoadXml((XmlElement)nodeList[0]);
			return ValidateSignatureReference(signedXml) && signedXml.CheckSignature(_certificate, true) && !IsExpired();
		}

		//an XML signature can "cover" not the whole document, but only a part of it
		//.NET's built in "CheckSignature" does not cover this case, it will validate to true.
		//We should check the signature reference, so it "references" the id of the root document element! If not - it's a hack
		private bool ValidateSignatureReference(SignedXml signedXml)
		{
			if (signedXml.SignedInfo.References.Count != 1) //no ref at all
				return false;

			var reference = (Reference)signedXml.SignedInfo.References[0];
			var id = reference.Uri.Substring(1);

			var idElement = signedXml.GetIdElement(_xmlDoc, id);

			if (idElement == _xmlDoc.DocumentElement)
				return true;
			else //sometimes its not the "root" doc-element that is being signed, but the "assertion" element
			{
				var assertionNode = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion", _xmlNameSpaceManager) as XmlElement;
				if (assertionNode != idElement)
					return false;
			}

			return true;
		}

		private bool IsExpired()
		{
			return DateTime.UtcNow > GetValidUntilUtc();
		}

		/// <summary>The datetime from which the assertion is no longer valid.</summary>
		/// <remarks>Comes from NotOnOrAfter in the SubjectConfirmationData</remarks>
		public DateTime GetValidUntilUtc()
		{
			DateTime expirationDate = DateTime.MaxValue;
			XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion[1]/saml:Subject/saml:SubjectConfirmation/saml:SubjectConfirmationData", _xmlNameSpaceManager);
			if (node != null && node.Attributes["NotOnOrAfter"] != null) {
				DateTime.TryParse(node.Attributes["NotOnOrAfter"].Value, out expirationDate);
			}
			return expirationDate.ToUniversalTime();
		}

		/// <summary>The Assertion ID</summary>
		public string GetID()
		{
			XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion[1]", _xmlNameSpaceManager);
			return node.Attributes["ID"].InnerText;
		}

		public string GetNameID()
		{
			XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion[1]/saml:Subject/saml:NameID", _xmlNameSpaceManager);
			return node?.InnerText;
		}

		public string GetUpn()
		{
			return AssertionAttributeValueForWithCoalesce("upn");
		}

		public string GetDisplayName()
		{
			return AssertionAttributeValueForWithCoalesce("displayname", "dname");
		}

		public string GetEmail()
		{
			return AssertionAttributeValueForWithCoalesce("User.email", "emailaddress", "mail");
		}

		public string GetFirstName()
		{
			return AssertionAttributeValueForWithCoalesce("first_name", "givenname", "User.FirstName", "FirstName");
		}

		public string GetLastName()
		{
			return AssertionAttributeValueForWithCoalesce("last_name", "surname", "User.LastName", "sn", "LastName");
		}

		public string GetDepartment()
		{
			return AssertionAttributeValueForWithCoalesce("department", "Department");
		}

		public string GetPhone()
		{
			return AssertionAttributeValueForWithCoalesce("homephone", "telephonenumber", "Phone number");
		}

		public string GetCompany()
		{
			return AssertionAttributeValueForWithCoalesce("companyname", "Company", "User.CompanyName");
		}

		public List<string> GetGroups()
		{
			foreach (string name in CoalesceNamesAndPrefixedNames(new List<string> { "groups" }, GROUPS_MS_NAME_PREFIX)) {
				var attempt = GetAllCustomAttribute(name);
				if (attempt.Count() > 0)
					return attempt.ToList();
			}
			return new List<string>();
		}

		private IEnumerable<string> ApplyNamePrefixToAll(IEnumerable<string> names, string name_prefix) => names.Select(name => name_prefix + name);

		private IEnumerable<string> CoalesceNamesAndPrefixedNames(IEnumerable<string> names, string name_prefix) => names.Union(ApplyNamePrefixToAll(names, name_prefix));

		private string AssertionAttributeValueForWithCoalesce(params string[] names_to_try)
		{
			foreach (string name in CoalesceNamesAndPrefixedNames(names_to_try, NORMAL_ATTRIBUTE_NAME_PREFIX)) {
				var attempt = GetCustomAttribute(name);
				if (attempt != null)
					return attempt;
			}
			return null;
		}

		private string AttributeByNameXPath(string name) => $"/samlp:Response/saml:Assertion[1]/saml:AttributeStatement/saml:Attribute[@Name='{name}']/saml:AttributeValue";

		private string GetCustomAttribute(string attr)
		{
			XmlNode node = _xmlDoc.SelectSingleNode(AttributeByNameXPath(attr), _xmlNameSpaceManager);
			return node == null ? null : node.InnerText;
		}

		private IEnumerable<string> GetAllCustomAttribute(string attr)
		{
			var node_list = _xmlDoc.SelectNodes(AttributeByNameXPath(attr), _xmlNameSpaceManager);
			foreach (XmlNode node in node_list) {
				yield return node.InnerText;
			}
			yield break;
		}

		//returns namespace manager, we need one b/c MS says so... Otherwise XPath doesnt work in an XML doc with namespaces
		//see https://stackoverflow.com/questions/7178111/why-is-xmlnamespacemanager-necessary
		private XmlNamespaceManager GetNamespaceManager()
		{
			XmlNamespaceManager manager = new XmlNamespaceManager(_xmlDoc.NameTable);
			manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
			manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
			manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

			return manager;
		}
	}

	public class AuthRequest
	{
		public string _id;
		private string _issue_instant;

		private string _issuer;
		private string _assertionConsumerServiceUrl;

		public enum AuthRequestFormat
		{
			Base64 = 1
		}

		public AuthRequest(string issuer, string assertionConsumerServiceUrl)
		{
			RSAPKCS1SHA256SignatureDescription.Init(); //init the SHA256 crypto provider (for needed for .NET 4.0 and lower)

			if (issuer == null || assertionConsumerServiceUrl == null)
				throw new NullReferenceException();

			_id = "_" + Guid.NewGuid().ToString();
			_issue_instant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);

			_issuer = issuer;
			_assertionConsumerServiceUrl = assertionConsumerServiceUrl;
		}

		public string GetRequest(AuthRequestFormat format)
		{
			using (StringWriter sw = new StringWriter()) {
				XmlWriterSettings xws = new XmlWriterSettings();
				xws.OmitXmlDeclaration = true;

				using (XmlWriter xw = XmlWriter.Create(sw, xws)) {
					xw.WriteStartElement("samlp", "AuthnRequest", "urn:oasis:names:tc:SAML:2.0:protocol");
					xw.WriteAttributeString("ID", _id);
					xw.WriteAttributeString("Version", "2.0");
					xw.WriteAttributeString("IssueInstant", _issue_instant);
					xw.WriteAttributeString("ProtocolBinding", "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST");
					xw.WriteAttributeString("AssertionConsumerServiceURL", _assertionConsumerServiceUrl);

					xw.WriteStartElement("saml", "Issuer", "urn:oasis:names:tc:SAML:2.0:assertion");
					xw.WriteString(_issuer);
					xw.WriteEndElement();

					xw.WriteStartElement("samlp", "NameIDPolicy", "urn:oasis:names:tc:SAML:2.0:protocol");
					xw.WriteAttributeString("Format", "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified");
					xw.WriteAttributeString("AllowCreate", "true");
					xw.WriteEndElement();

					/*xw.WriteStartElement("samlp", "RequestedAuthnContext", "urn:oasis:names:tc:SAML:2.0:protocol");
					xw.WriteAttributeString("Comparison", "exact");
					xw.WriteStartElement("saml", "AuthnContextClassRef", "urn:oasis:names:tc:SAML:2.0:assertion");
					xw.WriteString("urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport");
					xw.WriteEndElement();
					xw.WriteEndElement();*/

					xw.WriteEndElement();
				}

				if (format == AuthRequestFormat.Base64) {
					//byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(sw.ToString());
					//return System.Convert.ToBase64String(toEncodeAsBytes);

					//https://stackoverflow.com/questions/25120025/acs75005-the-request-is-not-a-valid-saml2-protocol-message-is-showing-always%3C/a%3E
					var memoryStream = new MemoryStream();
					var writer = new StreamWriter(new DeflateStream(memoryStream, CompressionMode.Compress, true), new UTF8Encoding(false));
					writer.Write(sw.ToString());
					writer.Close();
					string result = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, Base64FormattingOptions.None);
					return result;
				}

				return null;
			}
		}

		//returns the URL you should redirect your users to (i.e. your SAML-provider login URL with the Base64-ed request in the querystring
		public string GetRedirectUrl(string samlEndpoint, string relayState = null)
		{
			var queryStringSeparator = samlEndpoint.Contains("?") ? "&" : "?";

			var url = samlEndpoint + queryStringSeparator + "SAMLRequest=" + HttpUtility.UrlEncode(GetRequest(AuthRequestFormat.Base64));

			if (!string.IsNullOrEmpty(relayState)) {
				url += "&RelayState=" + HttpUtility.UrlEncode(relayState);
			}

			return url;
		}
	}
}
