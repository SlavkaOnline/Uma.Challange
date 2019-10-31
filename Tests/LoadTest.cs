using System;
using Microsoft.FSharp.Core;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Xunit;
using Assertion = NBomber.FSharp.Assertion;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests
{
    public class LoadTest
    {
        [Fact]
        public void Test()
        {
            var client = new WebApplicationFactory<WEBAPI.Startup>().CreateClient();
//            var step = HttpStep.Create("request",
//                async context => Http.CreateRequest("GET", "https://localhost:5001/api/remote")
//                    .WithHeader("Content-Type", "application/json"));

            var step = Step.Create("request", async context =>
            {
                await client.GetAsync("/api/remote");
                return Response.Ok();
            });
                

            var scenario = ScenarioBuilder.CreateScenario("test load", step)
                .WithConcurrentCopies(1000) 
                .WithAssertions(Assertion.forStep("request", FSharpFunc<Statistics, bool>.FromConverter(stats => Math.Abs(2000 - stats.Max) > 10), "Max > 2000"))
                .WithOutWarmUp()
                .WithDuration(TimeSpan.FromSeconds(300));

            NBomberRunner.RegisterScenarios(scenario).RunTest();
        }
    }
}