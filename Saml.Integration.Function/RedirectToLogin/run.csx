#r "Newtonsoft.Json"

using System.Net;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    string url = "https://login.microsoftonline.com/86c4efd3-7f59-4e51-8f64-6d7848dfcaef/saml2?SAMLRequest=fZLRbpswFIZfBfneAQMOYIVMWbMtSF2WJbSTejMZfAiewCa2ado%2b%2fWiySd3FevvrP%2bfTd%2byF5X03sNXoWrWH0wjWecU6Rz%2fDJqKpyDLcEEhwDDDHaTOfYxKkIqmCKqNRjLx7MFZqlaNwFiCvsHaEQlnHlZuigKSYhDhMyoCwIGJR%2fIC8ndFO17r7KJWQ6pij0SimuZWWKd6DZa5mh9XXWzZtZNW1ZNmmLHd49%2b1QIm9lLRg3QW%2b0smMP5gDmUdZwt7%2fNUevcYJnvv1phN8lQOuMvo4EzVFZOwUyB8%2fkg%2fc3ULI08HsGQD7UWkH9SL19EONI4MrZYyz155PvN921ftf1B3D20W%2fr0I7zXK8rdydqCNr%2faU3bOc%2bQ99Z2y7HLJ932GP%2fJouXhts8vBzJv598f5X3W0JBRA1BFg4ILiOKlTnMZhjHmWBmEiMiJ4svDfQK7EgW2nrcV6pztZP3uftem5%2bz%2bUzMglkQI3lyoblR2glo0EMT1F1%2bnzjQHuIEfOjIA8f3ml%2fvullr8B";
    Redirect(url);
    return (ActionResult)new OkObjectResult($"Hello friend!");
}
