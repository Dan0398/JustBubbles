AppendScript();

function AppendScript()
{	
	window.SDK_OPTIONS = 
	{
		gameId: "uu4bd03vakw9lcl0y0y2r2sm6cgj3du3", // Fill the game_id
		onEvent: function (a) {
		   switch (a.name) {
			  case "SDK_GAME_PAUSE":
				 // pause game logic / mute audio
				 break;
			  case "SDK_GAME_START":
				 // advertisement done, resume game logic and unmute audio
				 break;
			  case "SDK_READY":
				 // when sdk is ready
				 break;
			  case "SDK_ERROR":
				 // when sdk get error
				 break;
		   }
		}
	};
	var Script = document.createElement("script");
	Script.src = "https://api.gamemonetize.com/sdk.js";
	Script.onload = () => 
	{
		SetupEnvironment();
	}
	document.head.appendChild(Script);
}

function SetupEnvironment()
{
	MyEnv.EnvDescription = "GameMonetize";
    MyEnv.GenericRecognizeTouch();
    MyEnv.GenericRecognizeLanguage();
}

function ShowInterstitial()
{
    sdk.showBanner();
}

function ShowRewardedAd()
{
    sdk.showBanner();
}

function SaveData(Data)
{
    MyEnv.GenericSave(Data);
}

function LoadData()
{
    MyEnv.GenericLoad();
}

function SubmitLoadingStart() { TryLog("GameMonetize. LoadingStart not supported by platform"); }
function SubmitLoadingEnd() { TryLog("GameMonetize. LoadingEnd not supported by platform"); }

function SubmitGameplayStart() { TryLog("GameMonetize. GameplayStart not supported by platform"); }
function SubmitGameplayEnd() { TryLog("GameMonetize. GameplayEnd not supported by platform"); }
