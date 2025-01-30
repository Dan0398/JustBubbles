var crazysdk = null;
AppendScriptLoader();

function AppendScriptLoader()
{
	var Script = document.createElement("script");
    Script.src = "https://gameframe.crazygames.com/crazygames-gameframe-v1.bundle.js";
    Script.type = "text/javascript";
    Script.onload = () => 
    {
        CustomLoad();
    }
    document.head.appendChild(Script);
}

async function CustomLoad()
{
    var slugify = (text) => 
	{
      return (text || '')
        .replace(/&/g, ' and ')
        .replace(/[^a-zA-Z0-9]/g, ' ')
        .trim()
        .replace(/\s+/g, '-')
        .toLowerCase();
    }
	var buildUrl = "Build";
    var options = 
	{
      author: {{{ JSON.stringify(COMPANY_NAME) }}},
      gameName: {{{ JSON.stringify(PRODUCT_NAME) }}},
      gameSlug: slugify({{{ JSON.stringify(PRODUCT_NAME) }}}),
      gameLink: "https://www.crazygames.com/game/your-game-here",
      allowFullscreen: true,
      locale: "en_US",
      loader: "unity2020",
      loaderOptions: {
        unityLoaderUrl: buildUrl + "/{{{ LOADER_FILENAME }}}",
        unityConfigOptions: {
          "dataUrl": buildUrl + "/{{{ DATA_FILENAME }}}",
          "frameworkUrl": buildUrl + "/{{{ FRAMEWORK_FILENAME }}}",
          "codeUrl":  buildUrl + "/{{{ CODE_FILENAME }}}",
#if MEMORY_FILENAME
          "memoryUrl": buildUrl + "/{{{ MEMORY_FILENAME }}}",
#endif
#if SYMBOLS_FILENAME
		  "symbolsUrl": buildUrl + "/{{{ SYMBOLS_FILENAME }}}",
#endif
          "streamingAssetsUrl": "StreamingAssets",
        }
      },
      category: "UnitySDK",
      categoryLink: "https://www.crazygames.com",
      thumbnail: "https://images.crazygames.com/upcoming.png",
      gameStatus: "published",
      dollarRate: 1,
      sdkDebug: true,
      gameLink: 'https://www.crazygames.com/testgame',
      forceTestAds: true,
    };
	await window.Crazygames.load(options);
	
	AppendScriptSDK();
}

function AppendScriptSDK()
{	
	var Script = document.createElement("script");
	Script.src = "https://sdk.crazygames.com/crazygames-sdk-v1.js";
	Script.type = "text/javascript";
	Script.onload = () => 
	{
		crazysdk = window.CrazyGames.CrazySDK.getInstance();
		crazysdk.addEventListener("initialized", AfterInitSDK);
		crazysdk.init();
	};
	document.head.appendChild(Script);
}

function AfterInitSDK(event)
{
    crazysdk.removeEventListener("initialized", AfterInitSDK);
    MyEnv.EnvDescription = "CrazyGames";
	MyEnv.Player = event.userInfo;
    MyEnv.IsUsingTouch = MyEnv.Player.device.type !== "desktop";
	MyEnv.LoginAvailable = false;
	MyEnv.IsLogOn = false;
    UpdateLanguageByLangCode(MyEnv.Player.countryCode.toLowerCase());
	MyEnv.LanguageOverwriteByEnv = true;
	MyEnv.MoreGamesTabAvailable = false;
	MyEnv.FeedbackAvailable = false;
	MyEnv.RequireSendGameplayStatus = true;
	GetInstanceCycled();
}

async function GetInstanceCycled()
{
	if (window.Crazygames.getUnityInstance() === null)
	{
		await waitFor(_ => (window.Crazygames.getUnityInstance() !== null));
	}
	MyEnv.UnityInstance = window.Crazygames.getUnityInstance();
	MyEnv.EnvironmentReady = true;
    TryLog("Success inited");
}

function ShowInterstitial()
{
    crazysdk.addEventListener("adFinished", ApplyInterstitialSuccess);
    crazysdk.addEventListener("adFinished", CleanupInterstitialEvents);
    crazysdk.addEventListener("adError", ApplyInterstitialError);
    crazysdk.addEventListener("adError", CleanupInterstitialEvents);
    
	crazysdk.requestAd();
}

function ApplyInterstitialSuccess()
{
    MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'InterstitialSuccess');
    TryLog("CrazyGames Interstitial shows successfully");
}

function ApplyInterstitialError()
{
    MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'InterstitialFailed');
    TryLog("CrazyGames Interstitial doesn't shows. Reason:" + error);
}

function CleanupInterstitialEvents()
{
    crazysdk.removeEventListener("adFinished", ApplyInterstitialSuccess);
    crazysdk.removeEventListener("adFinished", CleanupInterstitialEvents);
    crazysdk.removeEventListener("adError", ApplyInterstitialError);
    crazysdk.removeEventListener("adError", CleanupInterstitialEvents);
}

function ShowRewardedAd()
{
    crazysdk.addEventListener("adFinished", ApplyRewardedSuccess);
    crazysdk.addEventListener("adFinished", CleanupRewardedEvents);
    crazysdk.addEventListener("adError", ApplyRewardedError);
    crazysdk.addEventListener("adError", CleanupRewardedEvents);
    
	crazysdk.requestAd("rewarded");
}

function ApplyRewardedSuccess()
{
    MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdSuccess');
    TryLog("CrazyGames Rewarded shows successfully");
}

function ApplyRewardedError()
{
    MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdFailed');
    TryLog("CrazyGames Rewarded doesn't shows. Reason:" + error);
}

function CleanupRewardedEvents()
{
    crazysdk.removeEventListener("adFinished", ApplyRewardedSuccess);
    crazysdk.removeEventListener("adFinished", CleanupRewardedEvents);
    crazysdk.removeEventListener("adError", ApplyRewardedError);
    crazysdk.removeEventListener("adError", CleanupRewardedEvents);
}

function SaveData(Data)
{
    MyEnv.GenericSave(Data);
}

function LoadData()
{
   MyEnv.GenericLoad();
}

function SubmitLoadingStart()
{
    crazysdk.sdkGameLoadingStart();
}

function SubmitLoadingEnd()
{
    crazysdk.sdkGameLoadingStop();
}

function SubmitGameplayStart()
{
    crazysdk.gameplayStart();
}

function SubmitGameplayEnd()
{
    crazysdk.gameplayStop();
}
