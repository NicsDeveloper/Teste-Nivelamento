using Newtonsoft.Json;

namespace QUESTAO2.Entites;

public class FootballMatch
{
    public string? Competition { get; set; }
    public int? Year { get; set; }
    public string? Round { get; set; }
    public string? Team1 { get; set; }
    public string? Team2 { get; set; }

    [JsonProperty("team1goals")]
    public int Team1goals { get; set; }

    [JsonProperty("team2goals")]
    public int Team2goals { get; set; }
}

