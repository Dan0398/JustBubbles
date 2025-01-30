AppendScript();

function AppendScript()
{
    var Script = document.createElement("script");
    Script.src = "https://integration.gamepix.com/sdk/v3/gamepix.sdk.js";
    Script.onload = () => 
    {
        SetupEnvironment();
    }
    document.head.appendChild(Script);
}

function SetupEnvironment()
{
    MyEnv.EnvDescription = "GamePix";
    MyEnv.GenericRecognizeTouch();
    MyEnv.LoginAvailable = false;
    MyEnv.IsLogOn = false;
	MyEnv.Player = null;
    UpdateLanguageByLangCode(GamePix.lang());
    MyEnv.LanguageOverwriteByEnv = true;
    MyEnv.MoreGamesTabAvailable = false;
    MyEnv.FeedbackAvailable = false;
    MyEnv.RequireSendGameplayStatus = false;
}

function ShowInterstitial()
{
    GamePix.interstitialAd().then(function (res) 
    {
        if (res.success) 
        {
            MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'InterstitialSuccess');
        } 
        else 
        {
            MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'InterstitialFailed');
        }
    }).catch(error => 
    {
        console.error(error);
        MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'InterstitialFailed');
    });
}

function ShowRewardedAd()
{
    GamePix.rewardAd().then(function (res) 
    {
        if (res.success) 
        {
            MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdSuccess');
        } 
        else 
        {
            MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdFailed');
        }
    }).catch(error => 
    {
        MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdFailed');
        TryLog("GamePix RewardedAd doesn't shows. Reason:" + error);
    });
}

function SaveData(Data)
{
    GamePix.localStorage.setItem("Key", Data);
    TryLog('GamePix. Data saved successfully');
}

async function LoadData()
{
    await waitFor(_ => (MyEnv !== null && MyEnv.UnityInstance !== null));
	try
	{
        var TargetObject = GamePix.localStorage.getItem("Key");
        if (TargetObject === null)
        {
            TargetObject = "{}";         
        }
        TryLog(TargetObject);
        MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'TranslateData', TargetObject);
	}
	catch(error)
	{
		console.error("GamePix. Failed to load data from server. Reason:" + error);
		MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'TranslateDataError', error.toString());
	}
}

function SubmitLoadingStart() { GamePix.loading(); }
function SubmitLoadingEnd() { GamePix.loaded(); }

function SubmitGameplayStart() { GamePix.gameAction(); }
function SubmitGameplayEnd() { GamePix.gameStop(); }
