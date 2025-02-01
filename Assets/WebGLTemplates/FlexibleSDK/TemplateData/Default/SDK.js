SetupEnvironment();

function SetupEnvironment()
{
	MyEnv.EnvDescription = "Default environment";
	MyEnv.GenericRecognizeTouch();
	MyEnv.LoginAvailable = false;
	MyEnv.IsLogOn = false;
	MyEnv.GenericRecognizeLanguage();
	MyEnv.LanguageOverwriteByEnv = false;
	MyEnv.MoreGamesTabAvailable = false;
	UpdateMoreGamesLinkByDomain();
	MyEnv.FeedbackAvailable = false;
	MyEnv.RequireSendGameplayStatus = false;
	MyEnv.EnvironmentReady = true;
}

function UpdateMoreGamesLinkByDomain()
{
	var Domain = "com";
	MyEnv.MoreGamesLink = "https://yandex."+ Domain +"/games/developer?name=BrakeLess%20Games";
    document.puzzlesLink = "https://yandex."+ Domain +"/games/app/281461";
}

function ShowInterstitial()
{
	MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'InterstitialSuccess');
	//MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'InterstitialFailed');
}

function ShowRewardedAd()
{
	MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdSuccess');
	//MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdFailed');
}

function SaveData(Data)
{
	MyEnv.GenericSave(Data);
}

function LoadData()
{
	MyEnv.GenericLoad();
}


function RequestReview()
{
	TryLog("Trying to request review. Environment not allowed.");
}

function SubmitGameplayStart() 	{ TryLog("Trying to submit gameplay end. Environment not allowed."); }
function SubmitGameplayEnd() 	{ TryLog("Trying to submit gameplay end. Environment not allowed."); }

function SubmitLoadingStart()
{
    TryLog("Trying to submit loading start. Environment not allowed."); 
}

function SubmitLoadingEnd()
{
    TryLog("Trying to submit loading end. Environment not allowed."); 
}

async function SubmitLoadingEndDelayed()
{
	if (typeof Yandex === 'undefined' || Yandex === null)
	{
		await waitFor(_ => (typeof Yandex !== 'undefined' && Yandex !== null));
	}
	Yandex.features.LoadingAPI.ready();
}

function ShowBanner()
{
	TryLog("Trying to show banner. Environment not allowed.");
}

function HideBanner()
{
	TryLog("Trying to hide banner. Environment not allowed.");
}

function GoToWorldOfPuzzles()
{
    window.open(document.puzzlesLink);
}