# jaytwo.AspNetCore.DataProtection

<p align="center">
  <a href="https://jenkins.jaytwo.com/job/jaytwo.AspNetCore.DataProtection/job/master/" alt="Build Status (master)">
    <img src="https://jenkins.jaytwo.com/buildStatus/icon?job=jaytwo.AspNetCore.DataProtection%2Fmaster&subject=build%20(master)" /></a>
  <a href="https://jenkins.jaytwo.com/job/jaytwo.AspNetCore.DataProtection/job/develop/" alt="Build Status (develop)">
    <img src="https://jenkins.jaytwo.com/buildStatus/icon?job=jaytwo.AspNetCore.DataProtection%2Fdevelop&subject=build%20(develop)" /></a>
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/jaytwo.AspNetCore.DataProtection/" alt="NuGet Package jaytwo.AspNetCore.DataProtection">
    <img src="https://img.shields.io/nuget/v/jaytwo.AspNetCore.DataProtection.svg?logo=nuget&label=jaytwo.AspNetCore.DataProtection" /></a>
  <a href="https://www.nuget.org/packages/jaytwo.AspNetCore.DataProtection/" alt="NuGet Package jaytwo.AspNetCore.DataProtection (beta)">
    <img src="https://img.shields.io/nuget/vpre/jaytwo.AspNetCore.DataProtection.svg?logo=nuget&label=jaytwo.AspNetCore.DataProtection" /></a>
</p>

A simpler way to enable cookies and authentication encryption dependencies between hosts in ASP.NET Core.

## Installation

Add the NuGet package

```
PM> Install-Package jaytwo.AspNetCore.DataProtection
```

## The Problem

The problem is best outlined [in the Microsoft documentation](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/web-farm?view=aspnetcore-3.1):

> When Data Protection or caching isn't configured for a web farm environment, intermittent errors occur when requests are processed. This occurs because nodes don't share the same resources and user requests aren't always routed back to the same node.

> Consider a user who signs into the app using cookie authentication. The user signs into the app on one web farm node. If their next request arrives at the same node where they signed in, the app is able to decrypt the authentication cookie and allows access to the app's resource. If their next request arrives at a different node, the app can't decrypt the authentication cookie from the node where the user signed in, and authorization for the requested resource fails.

The [official solutions](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-3.1) are 
pretty complicated when all you want to do is deploy this in a simple load-balanced environment.  Sometimes you don't have Redis infrastructure,
nor are you running in Azure.

## Usage

```csharp
// ...
using jaytwo.AspNetCore.DataProtection;
// ...

public class Startup
{
    // ...
    public void ConfigureServices(IServiceCollection services)
    {
        // ...
        services.AddDataProtection().UseStaticKeyFromSeed("my-seed-from-config-or-secrets");
        // ...
    }
    // ...
}
```

---

Made with &hearts; by Jake
