# SharePoint CSOM ADFS Example

This is a cut down version of the [Using the SharePoint CSCOM with a Claimed Based Auth Site][ClaimsArticle] example originally written by Steve Peschka.

I've modified it a little, changes include: 
- Now builds in Visual Studio 2015 on .Net 4.5.1.
- Removed all the UI elements except those related to ADFS as I didn't need all the other stuff.

To get it to build on .Net 4.5.1 I've updated a few references and code which pointed to Microsoft.Identity.* which where part of the Windows Identity Framework back in .Net 3.5 but then changed to System.Identity.* etc. in .Net 4.???? something.

I was a bit short of time when I did the conversion and there where a few Enums which must have been public in the past but are now marked internal in the System.Identity namespace. To get around these I had to replace a few enum look-ups with string values. Crap I know but it works ok.fdsfsdf



fsdfsdf






















<!-- LINKS -->
[ClaimsArticle]: https://samlman.wordpress.com/2015/02/28/using-the-client-object-model-with-a-claims-based-auth-site-in-sharepoint-2010/



