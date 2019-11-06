using System;
using System.Threading.Tasks;
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
            var step = HttpStep.Create("request",
                context => Http.CreateRequest("GET", "http://localhost:5000/api/remote")
                    .WithHeader("Content-Type", "application/json"));

            var pause = Step.Create("pause", async context =>
            {
                var r = new Random();
                await Task.Delay(r.Next(100, 30000));
                return Response.Ok();
            });
            
            var scenario = ScenarioBuilder.CreateScenario("test load", step, pause)
                .WithConcurrentCopies(1000) 
                .WithWarmUpDuration(TimeSpan.FromSeconds(10))
                .WithDuration(TimeSpan.FromSeconds(300));

            NBomberRunner.RegisterScenarios(scenario).RunTest();
        }
    }
}