using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Dadadada
{
    public class Engine
    {
        private static Team? homeTeam { get; set; }
        private static Team? awayTeam { get; set; }

        public Engine() 
        {
        }

public void EngineRun()
{
    string json = File.ReadAllText("teams.json");

    // Deserialize JSON into a list of Team objects
    List<Team> teams = new List<Team>();

    // Use JsonDocument to parse the JSON array
    using (JsonDocument document = JsonDocument.Parse(json))
    {
        JsonElement root = document.RootElement;

        // Check if the root is an array
        if (root.ValueKind == JsonValueKind.Array)
        {
            // Iterate through the JSON array
            foreach (JsonElement element in root.EnumerateArray())
            {
                // Access JSON properties and create Team object
                Team team = new Team
                {
                    TeamName = element.GetProperty("teamName").GetString(),
                    TeamDivision = element.GetProperty("teamDivision").GetString(),
                    TeamMatches = element.GetProperty("teamMatches").GetInt32(),
                    TeamGoalsAtHome = element.GetProperty("teamGoalsAtHome").GetInt32(),
                    TeamGoalsAtAway = element.GetProperty("teamGoalsAtAway").GetInt32(),
                    TeamGoalsAgaisntHome = element.GetProperty("teamGoalsAgaisntHome").GetInt32(),
                    TeamGoalsAgaisntAway = element.GetProperty("teamGoalsAgaisntAway").GetInt32(),
                    TeamPlacement = element.GetProperty("teamPlacement").GetInt32(),
                };

                teams.Add(team);
            }
        }
        else
        {
            Console.WriteLine("The root of the JSON document is not an array.");
        }
    }

    // Now 'teams' contains the data you loaded from the JSON file

    string targetTeamName = "Boston Bruins";
    string targetTeamName2 = "New York Islanders";

    // Use FirstOrDefault with a null check to avoid "Object reference not set to an instance of an object" error
    homeTeam = teams.FirstOrDefault(team => team.GetTeamName() == targetTeamName);
    awayTeam = teams.FirstOrDefault(team => team.GetTeamName() == targetTeamName2);

    // Check if homeTeam is not null before accessing properties
    if (homeTeam != null)
    {
        Console.WriteLine("Home : " + homeTeam.GetTeamName());
    }

    // Check if awayTeam is not null before accessing properties
    if (awayTeam != null)
    {
        Console.WriteLine("Away : " + awayTeam.GetTeamName());
    }

int maxGames = homeTeam.GetMatches() + awayTeam.GetMatches();
Console.WriteLine("Max games : " + maxGames.ToString());
int maxGoals = homeTeam.GetMaxGoals() + awayTeam.GetMaxGoals();
Console.WriteLine("Max maxGoals : " + maxGoals.ToString());
double medians = (double)maxGoals / maxGames; // Use double for the division
Console.WriteLine("Max medians : " + medians.ToString());
double odds = medians / 100.0; // Use double for the division
Console.WriteLine("Max odds : " + odds.ToString());
double maxodds = odds * maxGames; // Use double for the multiplication



    Console.WriteLine("-----------------------------------");
    Console.WriteLine("MaxGames : " + maxGames.ToString() + " MaxGoals : " + maxGoals.ToString() + " Odds : " + maxodds.ToString());
    Console.WriteLine("......");
    Console.WriteLine(PredictOutcome(teams));
    Console.ReadKey();
}
public static string PredictOutcome(List<Team> teams)
{
    // Calculate average goals scored and conceded by each team
    double homeTeamAverageGoals = (double)(teams[0].TeamGoalsAtHome + teams[1].TeamGoalsAtAway) / 2;
    double awayTeamAverageGoals = (double)(teams[1].TeamGoalsAtHome + teams[0].TeamGoalsAtAway) / 2;

    // Consider other factors and adjust probabilities accordingly
    double homeTeamOffensiveStrength = CalculateOffensiveStrength(teams[0]);
    double awayTeamOffensiveStrength = CalculateOffensiveStrength(teams[1]);

    double homeTeamDefensiveStrength = CalculateDefensiveStrength(teams[0]);
    double awayTeamDefensiveStrength = CalculateDefensiveStrength(teams[1]);

    // Consider team placement as a factor
    double homeTeamPlacementFactor = CalculatePlacementFactor(teams[0]);
    double awayTeamPlacementFactor = CalculatePlacementFactor(teams[1]);

    // Adjust factors based on recent performance, head-to-head stats, etc.
    AdjustFactorsBasedOnRecentPerformance(teams);

    // Calculate the expected total goals in the match
    double expectedTotalGoals = (homeTeamAverageGoals * homeTeamOffensiveStrength * homeTeamPlacementFactor +
                                awayTeamAverageGoals * awayTeamOffensiveStrength * awayTeamPlacementFactor) /
                               (homeTeamDefensiveStrength * homeTeamPlacementFactor + awayTeamDefensiveStrength * awayTeamPlacementFactor);

    // Predict the outcome based on the expected total goals
    if (expectedTotalGoals > 5.5)
    {
        return "Over 5.5 goals";
    }
    else
    {
        return "Under 5.5 goals";
    }
}

// Helper method to calculate offensive strength
private static double CalculateOffensiveStrength(Team team)
{
    // You can fine-tune this based on the specific characteristics of your teams
    return team.TeamGoalsAtHome + team.TeamGoalsAtAway;
}

// Helper method to calculate defensive strength
private static double CalculateDefensiveStrength(Team team)
{
    // You can fine-tune this based on the specific characteristics of your teams
    return team.TeamGoalsAgaisntHome + team.TeamGoalsAgaisntAway;
}

// Helper method to calculate the placement factor
private static double CalculatePlacementFactor(Team team)
{
    // Consider team placement as a factor (higher placement, higher factor)
    return 1.0 + (team.GetTeamPlacement() / 10.0);
}

// Helper method to adjust factors based on recent performance
private static void AdjustFactorsBasedOnRecentPerformance(List<Team> teams)
{
    // Fine-tune this method based on your specific requirements
    // For example, you can analyze the results of the last few matches and adjust factors accordingly.
}


    }
}