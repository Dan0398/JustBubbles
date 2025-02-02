var Leaderboards;
GetL();

async function GetL()
{
	await waitFor(_ => (typeof Yandex !== 'undefined' && Yandex !== null));
    Yandex.getLeaderboards().then(lb => 
    {
        Leaderboards = lb;
    });
}

function GetLeaderboardsByName(LeaderboardName)
{
    var Settings = null;
	var UserID = null;
    if (MyEnv.IsLogOn)
    {
        Settings = { quantityTop: 5, includeUser: true, quantityAround: 5 };
		UserID = MyEnv.Player.getUniqueID();
    }
    else 
    {
        Settings = { quantityTop: 5, includeUser: false, quantityAround: 0 };
    }
    Leaderboards.getLeaderboardEntries(LeaderboardName, Settings)
    .then(Result => 
    {
        var Arr = new Array(Result.entries.length);
        for (var i = 0; i < Result.entries.length; i++)
        {
            Arr[i] = new LeaderboardLine(Result.entries[i], UserID);
        }
        var ArrString = JSON.stringify(Arr);
        MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'ApplyLeaderboard', ArrString);
    });
}

function SetNewScoreInLeaderboards(ScoreWrite)
{
    var Write = JSON.parse(ScoreWrite);
    Leaderboards.setLeaderboardScore(Write.LeaderboardName, Write.Score);
}

class LeaderboardLine
{
    constructor(Data, userID) 
    {
        this.ID = Data.player.uniqueID;
        this.Name = Data.player.publicName;
        this.Rank = Data.rank;
        this.Score = Data.score;
        this.AvatarURL = Data.player.getAvatarSrc("meduim");
		this.IsUser = userID === Data.player.uniqueID;
    }
    ID;
    Name;
    Rank;
    Score;
    AvatarURL;
	IsUser;
}
