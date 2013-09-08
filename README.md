DropnetExt
==========

Some additions to the great Dropnet library that makes working with Dropbox from .Net code even easier.

How to use it
=============
* Create an instance of DropnetWrapper using your app's API key and Secret.
* Obtain the Auth URL using the `GetAuthorizeUrl` method.
* Open it in e.g. a popup window. Make sure the user clicks the blue button.
* Now you can get an instance of the `DropNetClient` class and use its members to access Dropbox.
* Check out a few useful extension methods in the DropnetExtensions class.
* See http://ivonna.biz/blog/2013/9/8/using-dropbox-in-your-net-application.aspx for more details.
