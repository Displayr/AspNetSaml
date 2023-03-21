# AspNetSaml

[![Build status](https://ci.appveyor.com/api/projects/status/j47v6dygoqpwknay?svg=true)](https://ci.appveyor.com/project/NumbersInternational/aspnetsaml)

Very simple SAML 2.0 "consumer" implementation in C#. It's a *SAML client* library, not a *SAML server*, allows adding SAML single-sign-on to your ASP.NET app, but *not* to provide auth services to other apps.

It's a *SAML client* library, not a *SAML server*, allows adding SAML single-sign-on to your ASP.NET app, but *not* to provide auth services to other apps.

## Installation

We have published this to Nuget. To install run this from the Package Manager Console in Visual Studio or Visual Studio Code:

`Install-Package Displayr.AspNetSaml`

This will add a reference to a compiled version of this assembly.

## Usage

### How SAML works?

SAML workflow has 2 steps:

1. User is redirected to the SAML provider (where he authenticates)
1. User is redirected back to your app, where you validate the payload

Here's how you do it (this example is for ASP.NET MVC:

### 1. Redirecting the user to the saml provider:

```c#
//this example is an ASP.NET MVC action method
public ActionResult Login()
{
	//TODO: specify the SAML provider url here, aka "Endpoint"
	var samlEndpoint = "http://saml-provider-that-we-use.com/login/";

	var request = new AuthRequest(
		"http://www.myapp.com", //TODO: put your app's "entity ID" here
		"http://www.myapp.com/SamlConsume" //TODO: put Assertion Consumer URL (where the provider should redirect users after authenticating)
		);

	//redirect the user to the SAML provider
	return Redirect(request.GetRedirectUrl(samlEndpoint));
}
```

### 2. User has been redirected back

User is sent back to your app - you need to validate the SAML response ("assertion") that you recieved via POST.

Here's an example of how you do it in ASP.NET MVC

```c#
//ASP.NET MVC action method... But you can easily modify the code for Web-forms etc.
public ActionResult SamlConsume()
{
	// 1. TODO: specify the certificate that your SAML provider gave you
	string samlCertificate = @"-----BEGIN CERTIFICATE-----
BLAHBLAHBLAHBLAHBLAHBLAHBLAHBLAHBLAHBLAHBLAHBLAH123543==
-----END CERTIFICATE-----";

	// 2. Let's read the data - SAML providers usually POST it into the "SAMLResponse" var
	Saml.Response samlResponse = new Response(samlCertificate, Request.Form["SAMLResponse"]);

	// 3. We're done!
	if (samlResponse.IsValid())
		username = samlResponse.GetNameID();
}
```

# Reading more attributes from the provider

SAML providers usually send more data with their response: username, first/last names etc. Here's how to get it:

```c#
if (samlResponse.IsValid())
{
	//WOOHOO!!! user is logged in

	//Some more optional stuff for you
	//let's extract username/firstname etc
	string username, email, firstname, lastname;
	try
	{
		username = samlResponse.GetNameID();
		email = samlResponse.GetEmail();
		firstname = samlResponse.GetFirstName();
		lastname = samlResponse.GetLastName();
	}
	catch(Exception ex)
	{
		//insert error handling code
		//no, really, please do
		return null;
	}

	//user has been authenticated, put your code here, like set a cookie or something...
	//or call FormsAuthentication.SetAuthCookie() or something
}
```

# Dependencies

Depending on your .NET version, your Project should reference `System.Security` 
for .NET Framework and `System.Security.Cryptography.Xml` for .NET Core.

# Testing
Tests are Visual Studio tests.  Build the solution and you should find them in the Test Explorer.
These rely on:

* The test Azure Active Directory called "Saml Single Signon Test Organisation"
* In which the "Displayr (saml-test-master)" Enterprise Application is registered, including a reference to the redirection URL (below).
* An Azure function that serves the redirection URL.  (Constants.cs:REPLY_URL)

# Nuget Publishing Steps

1. Open the solution in Visual Studio
2. Do a release build
3. Copy the build output Saml.dll to the lib folder.
4. Update Saml.Nuspec version number, and please use the SEMVER (https://semver.org/) scheme to decide which digit to increment. Commit this and push.
5. Update Saml.Nuspec releasenotes fields and put some description of what you changed there. But don't commit this.
6. From the Package Manage Console, run this command:

nuget pack Saml\Saml.nuspec

7. Ignore warnings about using bin folders.
8. This will generate a file like Displayr.AspNetSaml.1.1.0.nupkg in your solution root folder.
9. Visit https://www.nuget.org/packages/Displayr.AspNetSaml/ and login, and click upload.
10. Upload the nupkg file generated.
11. In the preview, enter https://raw.githubusercontent.com/Displayr/AspNetSaml/master/README.md for the doco url.
12. Click Submit

# About Displayr

[![Displayr](https://github.com/Displayr/AspNetSaml/blob/master/displayr_d.jpg?raw=true)](https://www.displayr.com)

Powerful business intelligence and online reporting for survey data. Now hiring...

Company: https://www.displayr.com  
Careers: https://www.displayr.com/careers/  
