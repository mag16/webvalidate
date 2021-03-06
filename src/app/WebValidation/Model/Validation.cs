﻿using System.Collections.Generic;

namespace WebValidation.Model
{
    public class Validation
    {
        public int StatusCode { get; set; } = 200;
        public string ContentType { get; set; } = "application/json";
        public int? Length { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public int? MaxMilliseconds { get; set; }
        public List<ValueCheck> Contains { get; set; } = new List<ValueCheck>();
        public string ExactMatch { get; set; }
        public JsonArray JsonArray { get; set; }
        public List<JsonProperty> JsonObject { get; set; }
        public PerfTarget PerfTarget { get; set; }
    }

    public class ValueCheck
    {
        public string Value { get; set; }
        public bool IsCaseSensitive { get; set; } = true;
    }
}
