using QUESTAO2.Services;

namespace QUESTAO2;

public class Program
{
    public static async Task Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        var totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);
    }

    public static async Task<int> GetTotalScoredGoals(string team, int year)
    {
        int totalGoals = 0;
        int currentPage = 1;
        int totalPages = 1;

        while (currentPage <= totalPages)
        {
            var response = await FootballMatchServices.GetMatches(year, team, currentPage);

            totalPages = response.Total_pages;

            foreach (var match in response.Data!)
            {
                if (match.Team1!.Equals(team, StringComparison.OrdinalIgnoreCase))
                    totalGoals += match.Team1goals;

                if (match.Team2!.Equals(team, StringComparison.OrdinalIgnoreCase))
                    totalGoals += match.Team2goals;
            }
            currentPage++;
        }

        return totalGoals;
    }

}