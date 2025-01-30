SDK_Init("3d2f573a01b247aaa3c43ac7cbcceedf");

function SDK_Init(gameKey) 
{
  window["GD_OPTIONS"] = 
  {
    debug: false, // Enable debugging console. This will set a value in local storage as well, remove this value if you don't want debugging at all. You can also call it by running gdsdk.openConsole() within your browser console.
    gameId: gameKey, // Your gameId which is unique for each one of your games; can be found at your Gamedistribution.com account.
  };
  if (document.getElementById("gamedistribution-jssdk")) return;
  var Script = document.createElement("script");
  Script.id = "gamedistribution-jssdk";
  Script.src = "https://html5.api.gamedistribution.com/main.min.js";
  document.body.appendChild(Script);
  Script.onload = () => 
  {
    gdsdk = window["gdsdk"];
    TryLog("Game Distribution. SDK Inited");
	SetupEnv();
  };
}

function SetupEnv()
{
  MyEnv.EnvDescription = "Game Distribution";
  MyEnv.GenericRecognizeTouch();
  MyEnv.LoginAvailable = false;
  MyEnv.IsLogOn = false;
  MyEnv.Player = null;
  MyEnv.GenericRecognizeLanguage();
  MyEnv.LanguageOverwriteByEnv = false;
  MyEnv.MoreGamesTabAvailable = true;
  MyEnv.MoreGamesLink = "https://gamedistribution.com/games?company=Brakeless%20Games";
  MyEnv.FeedbackAvailable = false;
  MyEnv.RequireSendGameplayStatus = true;
}

function ShowInterstitial()
{
  gdsdk.showAd()
  .then(response => 
  {
    TryLog("Game Distribution. Interstitial Ad show success");
    MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'InterstitialSuccess');
  })
  .catch(error =>
  {
    console.error("Game Distribution. Interstitial Ad show failed. Reason:" + error);
    MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'InterstitialFailed');
  })
}


function ShowRewardedAd()
{
  gdsdk.preloadAd('rewarded')
  .then(response => 
  {
    TryLog("Game Distribution. Rewarded Ad loaded");
    gdsdk.showAd('rewarded')
    .then(response => 
    {
      TryLog("Game Distribution. Rewarded Ad shown");
      MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdSuccess');
      // Ad process done. You can track "SDK_REWARDED_WATCH_COMPLETE" event if that event triggered, that means the user watched the advertisement completely, you can give reward there.
    })
    .catch(error => 
    {
      console.error("Game Distribution. Rewarded Ad show failed. Reason:" + error);
      MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdFailed');
    });
  }).catch(error => 
  {
    console.error("Game Distribution. Rewarded Ad load failed. Reason:" + error);
    MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdFailed');
  });
}

function SaveData(Data)
{
    MyEnv.GenericSave(Data);
}

function LoadData()
{
  MyEnv.GenericLoad();
}

function SDK_SendEvent(options) 
{
  options = Pointer_stringify(options);
  if (typeof gdsdk !== "undefined" && typeof gdsdk.sendEvent !== "undefined" && typeof options !== "undefined") 
  {
    gdsdk.sendEvent(options).then(function(response)
    {
      console.log("Game event post message sent Succesfully...")
    }
    ).catch(function(error)
    {
      console.log(error.message)
    });
  }
}

function SubmitLoadingStart() {   TryLog("Game Distribution. LoadingStart not supported by platform"); }
function SubmitLoadingEnd() {     TryLog("Game Distribution. LoadingEnd not supported by platform"); }

function SubmitGameplayStart() {  gdsdk.sendEvent("SDK_GAME_START"); }
function SubmitGameplayEnd() {    gdsdk.sendEvent("SDK_GAME_PAUSE");}
