﻿using System;
using System.Collections.Generic;

namespace WebValidation
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "can't be read-only - json serialization")]
    public class Request
    {
        public int? SortOrder { get; set; }
        public int? Index { get; set; }
        public string Verb { get; set; } = "GET";
        public string Url { get; set; }
        public string Body { get; set; } = null;
        public PerfTarget PerfTarget { get; set; }
        public List<Header> Headers { get; set; }
        public Validation Validation { get; set; }
    }

    public class PerfLog
    {
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Category { get; set; }
        public int PerfLevel { get; set; }
        public bool Validated { get; set; } = true;
        public string Body { get; set; } = string.Empty;
        public string ValidationResults { get; set; } = string.Empty;
        public double Duration { get; set; }
        public int StatusCode { get; set; }
        public long ContentLength { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "can't be read-only - json serialization")]
    public class PerfTarget
    {
        public string Category { get; set; }
        public List<double> Targets { get; set; }
    }

    public class Header
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "can't be read-only")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1724:Naming conflict", Justification = "safe to ignore")]
    public class Validation
    {
        public int Code { get; set; } = 200;
        public string ContentType { get; set; } = "application/json";
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public int? MaxMilliseconds { get; set; }
        public List<ValueCheck> Contains { get; set; } = new List<ValueCheck>();
        public ValueCheck ExactMatch { get; set; }
        public JsonArray JsonArray { get; set; }
        public List<JsonProperty> JsonObject { get; set; }
    }

    public class ValueCheck
    {
        public string Value { get; set; }
        public bool IsCaseSensitive { get; set; } = true;
    }

    public class JsonProperty
    {
        public string Field { get; set; }
        public object Value { get; set; }
    }

    public class JsonArray
    {
        public bool CountIsZero { get; set; }
        public int Count { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }
    }
}
