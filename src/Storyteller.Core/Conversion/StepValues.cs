﻿using System;
using System.Collections.Generic;
using System.Linq;
using Storyteller.Core.Model;
using Storyteller.Core.Results;

namespace Storyteller.Core.Conversion
{
    public class StepValues : Node
    {
        private readonly IDictionary<string, object> _values = new Dictionary<string, object>();
        private readonly IList<Delayed> _delayeds = new List<Delayed>();

        public StepValues(string id)
        {
            Id = id;
        }

        public void DoDelayedConversions(ISpecContext context)
        {
            _delayeds.Each(x =>
            {
                try
                {
                    var converted = x.Converter.Convert(x.Raw, context);
                    if (converted == null)
                    {
                        LogError(x.Key, "The converter was not able to create a value. Check the formatting.");
                    }
                    else
                    {
                        Store(x.Key, converted);
                    }
                }
                catch (Exception ex)
                {
                    LogError(x.Key, ex);
                }
            });
        }

        public IDictionary<string, object> RawData
        {
            get { return _values; }
        }

        /// <summary>
        /// For set verification purposes
        /// </summary>
        public bool IsMatched { get; set; }

        /// <summary>
        /// For set verification purposes
        /// </summary>
        public int Order { get; set; }

        public void RegisterDelayedConversion(string key, string raw, IRuntimeConvertor convertor)
        {
            _delayeds.Add(new Delayed
            {
                Key = key, Raw = raw, Converter = convertor
            });
        }

        public void Store(string key, object value)
        {
            _values.Add(key, value);
        }

        public object Get(string key)
        {
            return _values[key];
        }

        public IList<CellResult> Errors = new List<CellResult>();

        public StepResult ToConversionErrorResult()
        {
            return new StepResult(Id, ResultStatus.ok)
            {
                cells = Errors.ToArray()
            };
        }

        public void LogError(string key, Exception ex)
        {
            Errors.Add(CellResult.Error(key, ex.ToDisplayMessage()));
        }

        public void LogError(string key, string message)
        {
            Errors.Add(CellResult.Error(key, message));
        }

        public class Delayed
        {
            public string Key;
            public string Raw;
            public IRuntimeConvertor Converter;
        }

        public bool Has(string key)
        {
            return _values.ContainsKey(key);
        }

        public bool HasErrors()
        {
            return Errors.Any();
        }

        
    }

    public static class StepValuesExtensions
    {
        public static void ApplyOrdering(this IEnumerable<StepValues> list)
        {
            var i = 0;
            list.Each(x => x.Order = ++i);
        }
    }
}