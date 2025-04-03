using Newtonsoft.Json;
using QUESTAO2.Entites;

namespace QUESTAO2.Services;

public class FootballMatchServices
{
    public static async Task<FootballMatchesResponse> GetMatches(int year, string team, int page)
    {
        HttpClient httpClient = new();
        string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={team}&page={page}";

        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();

        FootballMatchesResponse matchesResponse = JsonConvert.DeserializeObject<FootballMatchesResponse>(jsonString)!;

        return matchesResponse;
    }

}