using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using jaytwo.AspNetCore.DataProtection.Tests;
using jaytwo.FluentHttp;
using Xunit;
using Xunit.Abstractions;

namespace jaytwo.Http.Authentication.Digest.Tests
{
    public class HttpClientTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly SampleAppServerContext _testServerContextFoo1;
        private readonly SampleAppServerContext _testServerContextFoo2;
        private readonly SampleAppServerContext _testServerContextBar1;
        private readonly SampleAppServerContext _testServerContextBar2;
        private readonly SampleAppServerContext _testServerContextNull1;
        private readonly SampleAppServerContext _testServerContextNull2;

        public HttpClientTests(ITestOutputHelper output)
        {
            _output = output;
            _testServerContextFoo1 = SampleAppServerContext.Create("foo");
            _testServerContextFoo2 = SampleAppServerContext.Create("foo");
            _testServerContextBar1 = SampleAppServerContext.Create("bar");
            _testServerContextBar2 = SampleAppServerContext.Create("bar");
            _testServerContextNull1 = SampleAppServerContext.Create(string.Empty);
            _testServerContextNull2 = SampleAppServerContext.Create(string.Empty);
        }

        [Fact]
        public async Task FooFooWorks()
        {
            // arrange
            var plain = "hello";

            // act
            using (var httpClient = new HttpClient())
            {
                var encryptWithFoo1Response = await httpClient.SendAsync(request =>
                {
                    request
                        .WithMethod(HttpMethod.Get)
                        .WithBaseUri(_testServerContextFoo1.Url)
                        .WithUriPath("encrypt")
                        .WithUriQueryParameter("input", plain)
                        ;
                });

                var encryptedWithFoo1 = await encryptWithFoo1Response.EnsureSuccessStatusCode().AsStringAsync();

                var decryptWithFoo2Response = await httpClient.SendAsync(request =>
                {
                    request
                        .WithMethod(HttpMethod.Get)
                        .WithBaseUri(_testServerContextFoo2.Url)
                        .WithUriPath("decrypt")
                        .WithUriQueryParameter("input", encryptedWithFoo1)
                        ;
                });

                var decryptedWithFoo2 = await decryptWithFoo2Response.EnsureSuccessStatusCode().AsStringAsync();

                // assert
                Assert.Equal(plain, decryptedWithFoo2);
            }
        }

        [Fact]
        public async Task BarBarWorks()
        {
            // arrange
            var plain = "hello";

            // act
            using (var httpClient = new HttpClient())
            {
                var encryptWithBar1Response = await httpClient.SendAsync(request =>
                {
                    request
                        .WithMethod(HttpMethod.Get)
                        .WithBaseUri(_testServerContextBar1.Url)
                        .WithUriPath("encrypt")
                        .WithUriQueryParameter("input", plain)
                        ;
                });

                var encryptedWithBar1 = await encryptWithBar1Response.EnsureSuccessStatusCode().AsStringAsync();

                var decryptWithBar2Response = await httpClient.SendAsync(request =>
                {
                    request
                        .WithMethod(HttpMethod.Get)
                        .WithBaseUri(_testServerContextBar2.Url)
                        .WithUriPath("decrypt")
                        .WithUriQueryParameter("input", encryptedWithBar1)
                        ;
                });

                var decryptedWithBar2 = await decryptWithBar2Response.EnsureSuccessStatusCode().AsStringAsync();

                // assert
                Assert.Equal(plain, decryptedWithBar2);
            }
        }

        [Fact]
        public async Task NullNullWorks()
        {
            // arrange
            var plain = "hello";

            // act
            using (var httpClient = new HttpClient())
            {
                var encryptWithNull1Response = await httpClient.SendAsync(request =>
                {
                    request
                        .WithMethod(HttpMethod.Get)
                        .WithBaseUri(_testServerContextNull1.Url)
                        .WithUriPath("encrypt")
                        .WithUriQueryParameter("input", plain)
                        ;
                });

                var encryptedWithNull1 = await encryptWithNull1Response.EnsureSuccessStatusCode().AsStringAsync();

                var decryptWithNull2Response = await httpClient.SendAsync(request =>
                {
                    request
                        .WithMethod(HttpMethod.Get)
                        .WithBaseUri(_testServerContextNull2.Url)
                        .WithUriPath("decrypt")
                        .WithUriQueryParameter("input", encryptedWithNull1)
                        ;
                });

                var decryptedWithNull2 = await decryptWithNull2Response.EnsureSuccessStatusCode().AsStringAsync();

                // assert
                Assert.Equal(plain, decryptedWithNull2);
            }
        }

        [Fact]
        public async Task FooBarDoesNotWork()
        {
            // arrange
            var plain = "hello";

            // act
            using (var httpClient = new HttpClient())
            {
                var encryptWithFoo1Response = await httpClient.SendAsync(request =>
                {
                    request
                        .WithMethod(HttpMethod.Get)
                        .WithBaseUri(_testServerContextFoo1.Url)
                        .WithUriPath("encrypt")
                        .WithUriQueryParameter("input", plain)
                        ;
                });

                var encryptedWithFoo1 = await encryptWithFoo1Response.EnsureSuccessStatusCode().AsStringAsync();

                var decryptWithBar1Response = await httpClient.SendAsync(request =>
                {
                    request
                        .WithMethod(HttpMethod.Get)
                        .WithBaseUri(_testServerContextBar1.Url)
                        .WithUriPath("decrypt")
                        .WithUriQueryParameter("input", encryptedWithFoo1)
                        ;
                });

                // assert
                Assert.Equal(HttpStatusCode.InternalServerError, decryptWithBar1Response.StatusCode);
            }
        }

        [Fact]
        public async Task FooNullDoesNotWork()
        {
            // arrange
            var plain = "hello";

            // act
            using (var httpClient = new HttpClient())
            {
                var encryptWithFoo1Response = await httpClient.SendAsync(request =>
                {
                    request
                        .WithMethod(HttpMethod.Get)
                        .WithBaseUri(_testServerContextFoo1.Url)
                        .WithUriPath("encrypt")
                        .WithUriQueryParameter("input", plain)
                        ;
                });

                var encryptedWithFoo1 = await encryptWithFoo1Response.EnsureSuccessStatusCode().AsStringAsync();

                var decryptWithNull1Response = await httpClient.SendAsync(request =>
                {
                    request
                        .WithMethod(HttpMethod.Get)
                        .WithBaseUri(_testServerContextNull1.Url)
                        .WithUriPath("decrypt")
                        .WithUriQueryParameter("input", encryptedWithFoo1)
                        ;
                });

                // assert
                Assert.Equal(HttpStatusCode.InternalServerError, decryptWithNull1Response.StatusCode);
            }
        }

        public void Dispose()
        {
            _testServerContextFoo1.Dispose();
            _testServerContextFoo2.Dispose();
            _testServerContextBar1.Dispose();
            _testServerContextBar2.Dispose();
            _testServerContextNull1.Dispose();
            _testServerContextNull2.Dispose();
        }
    }
}
