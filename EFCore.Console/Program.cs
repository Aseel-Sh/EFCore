using EFcore.Data;
using EFcore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;
using dotenv.net;
using Microsoft.Data.SqlClient;


//DotEnv.Load();


//we need an instance of context
//"using" limits the lifetime of the var which uses memory more efficiently
using var context = new FootballLeagueDBContext();
//be careful with this during production
//await context.Database.MigrateAsync();

#region Calling Funcs
//get all teams and only home matches where they have scored
//await InsertMoreMatches();

//filtering in includes
//await FilteringIncludes();

//lazy loading
//await LazyLoading();

//eager loading
//await EagerLoading();

//explicit loading
//await ExplicitLoading();

//insert rcord with FK
//await InsertMatch();

//insert parent/child
//await InsertTeam();

//insert parent with children
//await InsertLeague();

//Select all teams
//await GetAllTeams();

//await GetAllTeamsQuerySyntax();

//Select One team
//await GetOneTeam();

//select all records that meet a conditions
//await GetFilteredTeams();

//aggregate methods
//await AggregateMethods();

//grouping and aggregating
//await GroupByMethod();

//OrderingBy and aggregating
//await OrderByMethod();

//skip and take method
//await SkipAndTakeMethod();

//Projections and Select
//await ProjectionAndCustomDataTypes();

//Tracking Vs. No Tracking
//await TrackingVSNoTracking();

//IQueryables vs List Types
//await IQueryableVSListTypes();

//Inserting data into db
//await SimpleInsert();
//await InsertWithLoop();
//await BatchInsert();

//update operations
//await UpdateOperationTracked();
//await UpdateOperationUntracked();

//Delete stuff
//await DeleteData();

//execute delete
//await ExecuteDelete();

//execute update
//await ExecuteUpdate();

//projects and anonymous types
//await ProjectsAndAnonymous();

#endregion

#region Raw SQL
//await QueryingKeylessEntityOrView();

//await ExecutingRawSql();

//await RawSqlWithLinq();

//await OtherRawQueries();

//await otherQueryStuff();

#endregion
async Task otherQueryStuff()
{

    //non-querying statement
    var someNewTeamName = "New Team Name Here";
    var success = context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Teams SET Name = {someNewTeamName}");

    var teamToDeleteId = 1;
    var teamDeletedSuccess = context.Database.ExecuteSqlInterpolated($"EXEC dbo.DeleteTeam {teamToDeleteId}");

    //query scalar or non entity type
    var leagueIds = context.Database.SqlQuery<int>($"SELECT Id FROM Leagues")
        .ToList();

    //execute user defined query
    var earliestMatch = context.GetEarliestTeamMatch(1);
}
async Task RawSqlWithLinq()
{
    //mixing with LINQ
    var teamsList = context.Teams.FromSql($"SELECT * FROM Teams")
        .Where(q => q.Id == 1)
        .OrderBy(q => q.Id)
        .Include("League")
        .ToList();

    foreach (var t in teamsList)
    {
        Console.WriteLine(t);
    }
}
async Task OtherRawQueries()
{

    //executing stored procedures
    var leagueId = 1;
    var league = context.Leagues
        .FromSqlInterpolated($"EXEC dbo.StoredProcedureToGetLeagueName {leagueId}";
}
async Task ExecutingRawSql()
{
    //FromSqlRaw()
    Console.WriteLine("Enter Team Name:");
    var teamName = Console.ReadLine();
    var teamNameParam = new SqlParameter("teamName", teamName);
    var team = context.Teams.FromSqlRaw($"SELECT * FROM Teams WHERE NAME = @teamName", teamNameParam);
    foreach (var t in team)
    {
        Console.WriteLine(t);
    }

    //fromSQl()
    team = context.Teams.FromSql($"SELECT * FROM Teams WHERE NAME = {teamName}");
    foreach (var t in team)
    {
        Console.WriteLine(t);
    }

    //fromSQLInterpolated
    team = context.Teams.FromSqlInterpolated($"SELECT * FROM Teams WHERE NAME = {teamName}");
    foreach (var t in team)
    {
        Console.WriteLine(t);
    }
}
async Task QueryingKeylessEntityOrView()
{
    var details = await context.TeamsAndLeaguesView.ToListAsync();
}
async Task ProjectsAndAnonymous()
{
    var teams = await context.Teams
        .Select(q => new TeamDetails
        {
            TeamId = q.Id,
            TeamName = q.Name,
            CoachName = q.Coach.Name,
            TotalHomeGoals = q.HomeMatches.Sum(x => x.HomeTeamScore),
            TotalAwayGoals = q.AwayMatches.Sum(x => x.AwayTeamScore),
        })
        .ToListAsync();

    foreach (var team in teams)
    {
        Console.WriteLine($"{team.TeamName} - {team.CoachName} | Home Goals: {team.TotalHomeGoals} | Away Goals: {team.TotalAwayGoals}");
    }
}
async Task FilteringIncludes()
{
    var teams = await context.Teams
        .Include("Coach")
        .Include(q => q.HomeMatches.Where(q => q.HomeTeamScore > 0))
        .ToListAsync();

    foreach (var team in teams)
    {
        Console.WriteLine($"{team.Name} - {team.Coach.Name}");
        foreach (var match in team.HomeMatches)
        {
            Console.WriteLine($"Score - {match.HomeTeamScore}");
        }
    }
}
async Task InsertMoreMatches()
{
    var match1 = new Match
    {
        AwayTeamId = 2,
        HomeTeamId = 3,
        HomeTeamScore = 1,
        AwayTeamScore = 0,
        Date = new DateTime(2024, 11, 7),
        TicketPrice = 20,
    }; 
    var match2 = new Match
    {
        AwayTeamId = 2,
        HomeTeamId = 1,
        HomeTeamScore = 1,
        AwayTeamScore = 0,
        Date = new DateTime(2024, 11, 7),
        TicketPrice = 20,
    };
        var match3 = new Match
    {
        AwayTeamId = 1,
        HomeTeamId = 3,
        HomeTeamScore = 1,
        AwayTeamScore = 0,
        Date = new DateTime(2024, 11, 7),
        TicketPrice = 20,
    };
        var match4 = new Match
    {
        AwayTeamId = 10,
        HomeTeamId = 3,
        HomeTeamScore = 0,
        AwayTeamScore = 1,
        Date = new DateTime(2024, 11, 7),
        TicketPrice = 20,
    };

    await context.AddRangeAsync(match1, match2, match3, match4);
    await context.SaveChangesAsync();
}
async Task ExplicitLoading()
{
    var league = await context.FindAsync<League>(1);
    if (!league.Teams.Any())
    {
        Console.WriteLine("Teams have not been loaded");
    }

    await context.Entry(league)
        .Collection(q => q.Teams)
        .LoadAsync();

    if (league.Teams.Any())
    {
        foreach (var team in league.Teams)
        {
            Console.WriteLine($"{team.Name}");
        }
    }
}
async Task EagerLoading()
{
    var leagues = await context.Leagues
        //.Include("Teams") OR
        .Include(q => q.Teams)
        //to include coaches as well do
        .ThenInclude(q => q.Coach)
        .ToListAsync();

    foreach (var league in leagues)
    {
        Console.WriteLine($"League - {league.Name}");
        foreach (var team in league.Teams)
        {
            Console.WriteLine($"Team - {team.Name}, Coach - {team.Coach.Name}");

        }
    }
}
async Task LazyLoading()
{
    /*var league = await context.FindAsync<League>(1);
    foreach (var team in league.Teams)
    {
        Console.WriteLine($"{team.Name}");
    }*/

    foreach (var league in context.Leagues)
    {
        foreach (var team in league.Teams)
        {
            Console.WriteLine($"{team.Name} - {team.Coach.Name}");
        }
    }
}
async Task InsertLeague()
{
    var league = new League
    {
        Name = "New League",
        Teams = new List<EFcore.Domain.Team>
    {
        new EFcore.Domain.Team
        {
            Name = "Juventus",
            Coach = new Coach
            {
                Name = "Juve Coach"
            },
        },
        new Team
        {
            Name = "AC Milan",
            Coach = new Coach
            {
                Name = "Milan coach"
            },
        },
        new Team
        {
            Name = "AS Roma",
            Coach = new Coach
            {
                Name = "Roma Coach"
            },
        }


    }
    };

    await context.AddAsync(league);
    await context.SaveChangesAsync();
}
async Task InsertTeam()
{
    var team = new Team
    {
        Name = "New Team",
        Coach = new Coach
        {
            Name = "Johnson"
        },
    };

    await context.AddAsync(team);
    await context.SaveChangesAsync();
}
async Task InsertMatch()
{
    var match = new Match
    {
        AwayTeamId = 1,
        HomeTeamId = 2,
        HomeTeamScore = 0,
        AwayTeamScore = 0,
        Date = new DateTime(2024, 11, 7),
        TicketPrice = 20,
    };
    await context.AddAsync(match);
    await context.SaveChangesAsync();
}
async Task ExecuteUpdate()
{
    //same applies as delete here
    await context.Coaches.Where(q => q.Name == "Jose Mourinho").ExecuteUpdateAsync(set => set
    .SetProperty(prop => prop.Name, "Pep Guardiola")
    );
}
async Task ExecuteDelete()
{
    /*    var coaches = await context.Coaches.Where(q => q.Name == "Theodore Whitmore").ToListAsync();
        context.RemoveRange(coaches);
        await context.SaveChangesAsync();*/

    //make all the above shorter by
    await context.Coaches.Where(q => q.Name == "Theodore Whitmore").ExecuteDeleteAsync();
}
async Task DeleteData()
{
    var coach = await context.Coaches.FindAsync(10);
    /*context.Remove(coach);*/
    context.Entry(coach).State = EntityState.Deleted;
    await context.SaveChangesAsync();
}
async Task UpdateOperationUntracked()
{
    var coach1 = await context.Coaches
        .AsNoTracking()
        .FirstOrDefaultAsync(q => q.Id == 10);

    coach1.Name = "Test No Track";
    //context.Update(coach1);
    //anotherway
    context.Entry(coach1).State = EntityState.Modified;
    await context.SaveChangesAsync();
}
async Task UpdateOperationTracked()
{
    var coach = await context.Coaches.FindAsync(9);
    coach.Name = "Basboosa haha";
    coach.CreatedDate = DateTime.Now;
    await context.SaveChangesAsync();
}
async Task BatchInsert()
{
    var newCoach = new Coach
    {
        Name = "Jose Mourinho",
        CreatedDate = DateTime.Now,
    };
    var newCoach1 = new Coach
    {
        Name = "Theodore Whitmore",
        CreatedDate = DateTime.Now,
    };
    var newCoach2 = new Coach
    {
        Name = "Basboosa meow",
        CreatedDate = DateTime.Now
    };

    List<Coach> coaches = new List<Coach>
    {
        newCoach1,
        newCoach,
        newCoach2
    };

    //batch insert
    await context.Coaches.AddRangeAsync(coaches);
    await context.SaveChangesAsync();
}
async Task InsertWithLoop()
{
    //loop insert
    var newCoach = new Coach
    {
        Name = "Jose Mourinho",
        CreatedDate = DateTime.Now,
    };
    var newCoach1 = new Coach
    {
        Name = "Theodore Whitmore",
        CreatedDate = DateTime.Now,
    };

    List<Coach> coaches = new List<Coach>
    {
        newCoach1,
        newCoach
    };

    foreach (var coach in coaches)
    {
        await context.Coaches.AddAsync(coach);
    }
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
    await context.SaveChangesAsync();
    Console.WriteLine(context.ChangeTracker.DebugView.LongView);
    foreach (var coach in coaches)
    {
        Console.WriteLine($"{coach.Id} - {coach.Name}");
    }

}
async Task SimpleInsert()
{
    //simple insert
    var newCoach = new Coach
    {
        Name = "Jose Mourinho",
        CreatedDate = DateTime.Now,
    };
    await context.Coaches.AddAsync(newCoach);
    await context.SaveChangesAsync();
}
async Task IQueryableVSListTypes()
{
    Console.WriteLine("Enter '1' for Team wth Id 1 or '2' for teams that contain 'F.C.'");
    var option = Convert.ToInt32(Console.ReadLine());
    List<Team> teamAsList = new List<Team>();

    //loading records to memory and then doing the operations in memory
    teamAsList = await context.Teams.ToListAsync();

    if (option == 1)
    {
        teamAsList = teamAsList.Where(q => q.Id == 1).ToList();
    }
    else if (option == 2) 
    { 
        teamAsList = teamAsList.Where(q => q.Name.Contains("F.C.")).ToList();
    }

    foreach (var team in teamAsList)
    {
        Console.WriteLine(team.Name);
    }

    //keeping records as Iqueryable until tolistasync is executed
    var teamsAsQueryable = context.Teams.AsQueryable();
    if (option == 1)
    {
        teamsAsQueryable = teamsAsQueryable.Where(q => q.Id == 1);
    }
    else if (option == 2)
    {
        teamsAsQueryable = teamsAsQueryable.Where(q => q.Name.Contains("F.C."));
    }

    //actual query execution
    teamAsList = await teamsAsQueryable.ToListAsync();

    foreach (var team in teamAsList)
    {
        Console.WriteLine(team.Name);
    }

}
async Task TrackingVSNoTracking()
{   
    //good for readonly operations cuz doesnt track so more efficient
    var teams = await context.Teams
        .AsNoTracking()
        .ToListAsync();

    foreach (var team in teams)
    {
        Console.WriteLine(team.Name);
    }
}
async Task ProjectionAndCustomDataTypes()
{
    var teamNames = await context.Teams
        .Select(q => new Teaminfo{ Name = q.Name, Id = q.Id })
        .ToListAsync();

    foreach (var team in teamNames)
    {
        Console.WriteLine($"{team.Name} - {team.Id}");
    }
}
async Task SkipAndTakeMethod()
{
    var recordCount = 3;
    var page = 0;
    var next = true;

    while (next)
    {
        var teams = await context.Teams.Skip(page * recordCount).Take(3).ToListAsync();
        foreach (var team in teams)
        {
            Console.WriteLine(team.Name);
        }

        Console.WriteLine("Enter 'true' for the next set of records, 'false' to exit");
        next = Convert.ToBoolean(Console.ReadLine());

        if (!next) break;
        page++;
    }
}
async Task OrderByMethod()
{
    var orderedTeams = await context.Teams
        .OrderBy(q => q.Name)
        .ToListAsync();

    foreach (var item in orderedTeams)
    {
        Console.WriteLine(item.Name);
    }

    var descOrderedTeams = await context.Teams
        .OrderByDescending(q => q.Name)
        .ToListAsync();
    foreach (var item in descOrderedTeams)
    {
        Console.WriteLine(item.Name);
    }

    //getting the record with a max value
    var maxByDescOrder = await context.Teams
    .OrderByDescending(q => q.Id)
    .FirstOrDefaultAsync();

    var maxBy = context.Teams.MaxBy(q => q.Id);

    //getting the record with a min value
    var minByAscOrder = await context.Teams
    .OrderBy(q => q.Id)
    .FirstOrDefaultAsync();

    var minby = context.Teams.MinBy(q => q.Id);
}
async Task GroupByMethod()
{
    //go back and read the tolist issue in source code

    var groupedTeams = context.Teams
        .GroupBy(q => new { q.CreatedDate.Date });
        //if you wanna do the "having" statement do it here after grouping by using where
        //.Where(bla)

    foreach (var group in groupedTeams)
    {
        Console.WriteLine(group.Key);
        Console.WriteLine(group.Sum(q => q.Id));

        foreach (var team in group)
        {
            Console.WriteLine(team.Name);
        }
    }

}
async Task AggregateMethods() {
    //Aggregate Count method
    var numberOfTeams = await context.Teams.CountAsync();
    Console.WriteLine($"Number of teams: {numberOfTeams}");

    var numberOfTeamsWithCondition = await context.Teams.CountAsync(q => q.Id == 1);
    Console.WriteLine($"Number of teams with cond: {numberOfTeamsWithCondition}");


    //aggregate Max method
    var maxTeams = await context.Teams.MaxAsync(q => q.Id);
    Console.WriteLine($"Max of Teams: {maxTeams}");

    //aggregate min method
    var minTeams = await context.Teams.MinAsync(q => q.Id);
    Console.WriteLine($"Min of teams {minTeams}");

    //aggregate Avg method
    var avgTeams = await context.Teams.AverageAsync(q => q.Id);
    Console.WriteLine($"Avg of teams {avgTeams}");

    //aggregate sum method
    var sumTeam = await context.Teams.SumAsync(q => q.Id);
    Console.WriteLine($"Sum of teams {sumTeam}");
}
async Task GetAllTeamsQuerySyntax()
{
    Console.WriteLine("Enter Search term");
    var searchTerm = Console.ReadLine();

    var teams = await (from team in context.Teams 
                 where EF.Functions.Like(team.Name, $"%{searchTerm}")
                 select team).ToListAsync();

    foreach (var t in teams)
    {
        Console.WriteLine(t.Name);
    }
}
async Task GetFilteredTeams()
{
    Console.WriteLine("Enter Search term");
    var searchTerm = Console.ReadLine();

    var teamsFiltered = await context.Teams.Where(q => q.Name == searchTerm)
            .ToListAsync();
    foreach (var item in teamsFiltered)
    {
        Console.WriteLine(item.Name);
    }

    /*    var partialMatches = await context.Teams.Where(q => q.Name.Contains(searchTerm)).ToListAsync();*/
    //Select * from teams where name like '%f.c.'
    var partialMatches = await context.Teams.Where(q => EF.Functions.Like(q.Name, $"%{searchTerm}%")).ToListAsync();

    foreach (var item in partialMatches)
    {
        Console.WriteLine(item.Name);
    }

}
async Task GetOneTeam()
{
    var teamFirst = await context.Coaches.FirstAsync();
    if (teamFirst != null)
    {
        Console.WriteLine(teamFirst.Name);
    }
}
async Task GetAllTeams()
{
    //Selecting a single record -First one in the list
    var teamFirst = await context.Coaches.FirstAsync();
    if (teamFirst != null)
    {
        Console.WriteLine(teamFirst.Name);
    }
    var teamFirstOrDefault = await context.Coaches.FirstOrDefaultAsync();
    if (teamFirstOrDefault != null)
    {
        Console.WriteLine(teamFirstOrDefault.Name);
    }

    //Selecting a single record -First one in the list that meets a condition
    var teamFirstWithCondition = await context.Teams.FirstAsync(team => team.Id == 1);
    if (teamFirstWithCondition != null)
    {
        Console.WriteLine(teamFirstWithCondition.Name);
    }
    var teamFirstOrDefaultWithCondition = await context.Teams.FirstOrDefaultAsync(team => team.Id == 1);
    if (teamFirstOrDefaultWithCondition != null)
    {
        Console.WriteLine(teamFirstOrDefaultWithCondition.Name);
    }

    //Selecting a single record -Only one record should be returned, or an exception will be thrown
    var teamSingle = await context.Teams.SingleAsync();
    if (teamSingle != null)
    {
        Console.WriteLine(teamSingle.Name);
    }
    var teamSingleWithCondition = await context.Teams.SingleAsync(team => team.Id == 2);
    if (teamSingleWithCondition != null)
    {
        Console.WriteLine(teamSingleWithCondition.Name);
    }
    var SingleOrDefault = await context.Teams.SingleOrDefaultAsync(team => team.Id == 2);
    if (SingleOrDefault != null)
    {
        Console.WriteLine(SingleOrDefault.Name);
    }

    //Selecting based on Primary Key Id value
    var teamBasedOnId = await context.Teams.FindAsync(3);
    if (teamBasedOnId != null)
    {
        Console.WriteLine(teamBasedOnId.Name);
    }

    //all teams
    var teams = await context.Teams.ToListAsync();

    foreach (var team in teams)
    {
        Console.WriteLine(team.Name);
    }
}

class Teaminfo
{
    public int Id { get; set; }
    public string Name { get; set; }    
}
class TeamDetails
{
    public int TeamId { get; set; }
    public string TeamName { get; set; }
    public string CoachName { get; set; }
    public int TotalHomeGoals { get; set; }
    public int TotalAwayGoals { get; set; }
}