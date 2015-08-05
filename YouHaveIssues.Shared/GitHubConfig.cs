using System;
using Newtonsoft.Json;
using System.IO;

namespace YouHaveIssues
{
    [JsonObject]
    public class GitHubConfig
    {
        public static GitHubConfig Load()
        {
            using (var reader = new JsonTextReader(new StreamReader("config.json")))
            {
                return JsonSerializer.CreateDefault().Deserialize<GitHubConfig>(reader);
            }
        }

        [JsonProperty]
        public string ClientID { get; }

        [JsonProperty]
        public string ClientSecret { get; }

        public GitHubConfig(string clientId, string clientSecret)
        {
            this.ClientID = clientId;
            this.ClientSecret = clientSecret;
        }
    }
}