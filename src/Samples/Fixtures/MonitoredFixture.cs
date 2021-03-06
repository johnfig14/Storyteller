﻿using System;
using System.Threading;
using Baseline;
using StoryTeller;

namespace Samples.Fixtures
{
    public class MonitoredFixture : Fixture
    {
        public static TimeSpan WaitTime = TimeSpan.Zero;

        public MonitoredFixture()
        {
            

            this["DoSomething"] = Do("Inline grammar that should run within 100 ms", c =>
            {
                Thread.Sleep(WaitTime);
            }).PerfLimit(100);
        }

        public override void SetUp()
        {
            PerformancePolicies.ClearAll();
            PerformancePolicies.PerfLimit(50, r => r.Subject == "Fake");
        }

        [FormatAs("Pause for {waitTime} milliseconds")]
        public void Wait(int waitTime)
        {
            WaitTime = waitTime.Milliseconds();
        }

        [PerfLimit(100), FormatAs("Sentence w/ 100 ms threshold")]
        public void Sentence()
        {
            Thread.Sleep(WaitTime);
        }

        [PerfLimit(100), FormatAs("Fact w/ 100 ms threshold")]
        public bool Fact()
        {
            Thread.Sleep(WaitTime);
            return true;
        }

        private string[] names()
        {
            Thread.Sleep(WaitTime);
            return new string[] { "Bill", "Jill", "Jake" };
        }

        [PerfLimit(100)]
        public IGrammar SetVerification()
        {
            return VerifyStringList(names).Titled("Check the names within 100ms");
        }

        [FormatAs("Register a fake perf record that runs for {runtime} ms")]
        public void RegisterFakeRecord(int runtime)
        {
            var record = Context.Timings.Subject("Fake", "Fake", 0);
            Thread.Sleep(runtime.Milliseconds());

            Context.Timings.End(record);
        }
    }
}