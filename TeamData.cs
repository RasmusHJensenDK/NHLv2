using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Dadadada
{
    // Define the input data class
    public class TeamData
    {
        [LoadColumn(0)] public float TeamMatches;
        [LoadColumn(1)] public float TeamGoalsAtHome;
        [LoadColumn(2)] public float TeamGoalsAtAway;
        [LoadColumn(3)] public float TeamGoalsAgaisntHome;
        [LoadColumn(4)] public float TeamGoalsAgaisntAway;
        [LoadColumn(5)] public float TeamPlacement;
        [LoadColumn(6)] public float TotalGoals;

        // Adjust the LoadColumn index based on the actual position of TeamName in your JSON
        [LoadColumn(7)] public string TeamName;
    }

    // Define the prediction class
    public class TeamPrediction
    {
        [ColumnName("Score")]
        public float TotalGoals;
    }

    public class LinearRegressionModel
    {
        public static void TrainModel(List<TeamData> trainingData)
        {
            var mlContext = new MLContext();

            // Convert data to IDataView
            var trainData = mlContext.Data.LoadFromEnumerable(trainingData);

            // Define data preparation pipeline without Min-Max scaling
            var pipeline = mlContext.Transforms.CopyColumns("Label", nameof(TeamData.TotalGoals))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(TeamData.TeamMatches), nameof(TeamData.TeamGoalsAtHome),
                                                         nameof(TeamData.TeamGoalsAtAway), nameof(TeamData.TeamGoalsAgaisntHome),
                                                         nameof(TeamData.TeamGoalsAgaisntAway), nameof(TeamData.TeamPlacement)))
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Label"))
                .Append(mlContext.Transforms.CopyColumns("Score", "Score"));

            // Train the model
            var model = pipeline.Fit(trainData);

            // Save the model to a file
            mlContext.Model.Save(model, trainData.Schema, "LinearRegressionModel.zip");
        }


        public static void TestModel(List<TeamData> testData, string team1Name, string team2Name, int numberOfSimulations)
        {
            Console.WriteLine($"Testing model for teams: {team1Name} and {team2Name}");

            var mlContext = new MLContext();

            // Debug print to check available team names
            Console.WriteLine("Available teams for testing:");
            foreach (var team in testData)
            {
                Console.WriteLine(team.TeamName);
            }

            // Get the specific teams for testing
            var teamsToTest = testData.Where(team => team1Name.Equals(team.TeamName) || team2Name.Equals(team.TeamName)).ToList();

            if (teamsToTest.Count == 0)
            {
                Console.WriteLine("Couldn't find the specified teams for testing.");
                return;
            }

            Console.WriteLine($"Teams for testing: {teamsToTest[0].TeamName} and {teamsToTest[1].TeamName}");

            for (int i = 0; i < numberOfSimulations; i++)
            {
                // Run your custom algorithm to calculate predictions
                var homeTeam = ConvertToTeam(teamsToTest[0]);
                var awayTeam = ConvertToTeam(teamsToTest[1]);

                int maxGames = homeTeam.TeamMatches + awayTeam.TeamMatches;

                // Ensure maxGames is greater than 0 before calculations
                if (maxGames > 0)
                {
                    int maxGoals = homeTeam.GetMaxGoals() + awayTeam.GetMaxGoals();
                    double medians = (double)maxGoals / maxGames;
                    double odds = medians / 100.0;
                    double maxOdds = odds * maxGames;
                    int maxg = homeTeam.GetMatches() + awayTeam.GetMatches();

                    Console.WriteLine($"Simulation {i + 1}: Predicted Outcome for {homeTeam.GetTeamName()} vs {awayTeam.GetTeamName()}");
                    Console.WriteLine($"Max games: {maxg}");
                    Console.WriteLine($"Max maxGoals: {maxGoals}");
                    Console.WriteLine($"Max medians: {medians}");
                    Console.WriteLine($"Max odds: {odds}");
                    Console.WriteLine($"Max maxOdds: {maxOdds}");
                }
                else
                {
                    Console.WriteLine($"Simulation {i + 1}: Max games is 0, skipping calculation.");
                }
            }
        }





        public static void Main1()
        {
            string json = File.ReadAllText("teams.json");

            List<Team> teams = new List<Team>();

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

                        Console.WriteLine($"Loaded {team.GetTeamName()} from {team.TeamDivision} division."); // Debug print
                    }
                }
                else
                {
                    Console.WriteLine("The root of the JSON document is not an array.");
                }
            }

            // Debug print to check team names
            Console.WriteLine("Loaded teams:");
            foreach (var team in teams)
            {
                Console.WriteLine(team.GetTeamName());
            }

            // Debug print to check team namesP
            Console.WriteLine("Loaded teams:");
            foreach (var team in teams)
            {
                Console.WriteLine(team.GetTeamName());
            }

            // Convert Team data to TeamData for training
            var trainingData = teams.Select(team =>
            {
                var teamData = new TeamData
                {
                    TeamMatches = team.GetMatches(),
                    TeamGoalsAtHome = team.TeamGoalsAtHome,
                    TeamGoalsAtAway = team.TeamGoalsAtAway,
                    TeamGoalsAgaisntHome = team.TeamGoalsAgaisntHome,
                    TeamGoalsAgaisntAway = team.TeamGoalsAgaisntAway,
                    TeamPlacement = team.GetTeamPlacement(),
                    TotalGoals = team.GetMaxGoals(),
                    TeamName = team.GetTeamName() // Add TeamName to TeamData
                };

                Console.WriteLine($"Converted {team.GetTeamName()} to TeamData"); // Debug print

                return teamData;
            }).ToList();

            // Print out the team names available in the training data
            Console.WriteLine("Available teams for testing:");
            foreach (var team in trainingData)
            {
                Console.WriteLine(team.TeamName);
            }

            // Train the model
            TrainModel(trainingData);

            // Choose two teams for testing
            string team1 = "Vegas Golden Knights";
            string team2 = "Los Angeles Kings";

            // Test the model for the selected teams
            TestModel(trainingData, team1, team2, 10000);
        }
        private static Team ConvertToTeam(TeamData teamData)
        {
            return new Team
            {
                TeamName = teamData.TeamName,
                TeamGoalsAtHome = (int)teamData.TeamGoalsAtHome,
                TeamGoalsAtAway = (int)teamData.TeamGoalsAtAway,
                TeamGoalsAgaisntHome = (int)teamData.TeamGoalsAgaisntHome,
                TeamGoalsAgaisntAway = (int)teamData.TeamGoalsAgaisntAway,
                TeamPlacement = (int)teamData.TeamPlacement
            };
        }

    }
}
