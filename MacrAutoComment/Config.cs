using Newtonsoft.Json;

namespace MacrAutoComment
{
    internal class Config
    {
        [JsonProperty]
        public string LANGUAGE { get; internal set; }
        [JsonProperty]
        public string AUTHOR { get; internal set; }
        [JsonProperty]
        public string VERSION { get; internal set; }
    }
}