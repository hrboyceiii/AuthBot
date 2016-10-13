#Using Authbot to authenticate in Visual Studio Online
Here are more details about VSO authentication with Authbot

##Bot in action
<img src="https://github.com/tiagonmas/AuthBot/blob/master/VSOBotSampeInAction.PNG" width="400">

##Configuring web.config
```xml
  <appSettings>
    <!-- update these with your appid and one of your appsecret keys-->
    <add key="BotId" value="Your Bot Id"/>
    <add key="MicrosoftAppId" value="Your App ID"/>
    <add key="MicrosoftAppPassword" value="Your Bot Pass"/>

    <!-- VSO -->
    <add key="ActiveDirectory.Mode" value="vso"/>
    <add key="ActiveDirectory.EndpointUrl" value="https://app.vssps.visualstudio.com"/> <!-- Do Not Change for VS-->
    <add key="ActiveDirectory.Tenant" value="Your Tenant"/>
    <add key="ActiveDirectory.Scopes" value="The scopes your app needs access to"/>
    <add key="ActiveDirectory.ClientId" value="Your Client Id"/>
    <add key="ActiveDirectory.ClientSecret" value="Your Client Secret"/>
    <add key="ActiveDirectory.RedirectUrl" value="https://tiagonmasbot.azurewebsites.net/api/OAuthCallback"/>
  </appSettings>
```

## VSO oAuth flow 

##Relevant links
https://app.vsaex.visualstudio.com/
https://app.vsaex.visualstudio.com/app/register
Authorize access to REST APIs with OAuth 2.0
https://www.visualstudio.com/docs/integrate/get-started/auth/oauth
