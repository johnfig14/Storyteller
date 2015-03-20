﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StoryTeller.Results;

namespace StoryTeller.Testing
{
    public static class SpecContextExtensions
    {
        public static void AssertTheOnlyResultIs(this SpecContext context, IResultMessage expectation)
        {
            if (context.Results.Count == 0)
            {
                Assert.Fail("No results were captured");
            }

            if (context.Results.Count > 1)
            {
                Assert.Fail("Multiple results were captured: " + context.Results.Select(x => x.ToString()).Join(", "));
            }

            context.Results.Single().ShouldEqual(expectation);
        }
    }
}