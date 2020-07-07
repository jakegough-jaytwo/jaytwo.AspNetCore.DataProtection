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

## Installation

Add the NuGet package

```
PM> Install-Package jaytwo.AspNetCore.DataProtection
```

## Why

Everything from encrypted cookies to CSRF tokens uses Data Protection to encrypt and decrypt data 
server-side (similar to the old `MachineKey` section in the `Web.Config`).  If your application is running
in multiple instances, and if they don't share `DataProtection` keys, then data encrypted with one node 
will not be able to be decrypted on another node.

The problem is best outlined [in the Microsoft documentation](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/web-farm?view=aspnetcore-3.1):

> When Data Protection or caching isn't configured for a web farm environment, intermittent errors occur when requests are processed. This occurs because nodes don't share the same resources and user requests aren't always routed back to the same node.

> Consider a user who signs into the app using cookie authentication. The user signs into the app on one web farm node. If their next request arrives at the same node where they signed in, the app is able to decrypt the authentication cookie and allows access to the app's resource. If their next request arrives at a different node, the app can't decrypt the authentication cookie from the node where the user signed in, and authorization for the requested resource fails.

## How

The [official solutions](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-3.1) are 
pretty complicated when all you want to do is deploy this in a simple load-balanced environment.  Sometimes you don't have Redis infrastructure,
nor are you running in Azure.

Normally you would use a `.AddDataProtection().PersistKeysTo***()` extension method in startup.cs to 
configure how to rotate keys.  The default options are filesystem, registry, Azure, or Redis -- but we didn't 
have any of those readily available.  So we just generated a key template that's good for 1000 years and 
we populate a tempalte with a key on initialization.  That key is the 64-byte hash of a seed which can come
from configuration (including from secrets/config).

## Lessons Learned

The original implementation of this idea baked the actual encryption key into the application.  That prevented
being able to define a unique encryption per environment and store that key at rest in Vault.

### Making this better... or best

* Improving the feature:
  * allowing expiration to be loaded from configuration so keys can be rotated. (specify  multiple seeds each 
    with defined effective/expiry dates)
  * implementing key rotation by algorithm -- using a known TTL (14 days, for example), use the seed to populate 
    keys on demand, with effective dates and expiry dates staggered according to the TTL
* Implementing a DB-backed mechanism that can create new keys as old keys expire (and shortening the lifetime).

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
