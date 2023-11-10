public class Team
{
    public string TeamName { get; set; }
    public string TeamDivision { get; set; }
    public int TeamMatches { get; set; }
    public int TeamGoalsAtHome { get; set; }
    public int TeamGoalsAtAway { get; set; }
    public int TeamGoalsAgaisntHome { get; set; }
    public int TeamGoalsAgaisntAway { get; set; }
    public int TeamPlacement { get; set; }

    public Team()
    {
    }

    // Deserialization constructor
    public Team(string teamName, string teamDivision, int teamMatches, int teamGoalsAtHome, int teamGoalsAtAway, int teamGoalsAgaisntHome, int teamGoalsAgaisntAway, int teamPlacement)
    {
        TeamName = teamName;
        TeamDivision = teamDivision;
        TeamMatches = teamMatches;
        TeamGoalsAtHome = teamGoalsAtHome;
        TeamGoalsAtAway = teamGoalsAtAway;
        TeamGoalsAgaisntHome = teamGoalsAgaisntHome;
        TeamGoalsAgaisntAway = teamGoalsAgaisntAway;
        TeamPlacement = teamPlacement;
    }

    public int GetTeamPlacement() 
    {
        return TeamPlacement;
    }

    public string GetTeamName()
    {
        return TeamName;
    }

    public int GetMatches()
    {
        return TeamMatches;
    }

    public int GetMaxGoals()
    {
        return TeamGoalsAtHome + TeamGoalsAtAway + TeamGoalsAgaisntHome + TeamGoalsAgaisntAway;
    }
}
