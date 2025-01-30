InitMainScript();

async function InitMainScript()
{
	PrepareEnvironment();
	await waitFor(_ => (typeof MyEnv !== 'undefined' && MyEnv !== null));
	TryLog("Environment loaded");
	LoadPageLocales();
	await waitFor(_ => (typeof UnpackingLabel !== 'undefined' && UnpackingLabel !== null));
	TryLog("Locales loaded");
	LoadGame();
	DetectTargetSDK();
}

function PrepareEnvironment()
{
	var EnvironmentScript = document.createElement("script");
	EnvironmentScript.src = "TemplateData/Environment.js";
	document.body.appendChild(EnvironmentScript);
}

function LoadPageLocales()
{
	var Localization = document.createElement("script");
	Localization.src = "TemplateData/HTML_Locales.js";
	document.body.appendChild(Localization);
}

function LoadGame()
{
	var Initiator = document.createElement("script");
	Initiator.src = "TemplateData/Unity.js";
	document.body.appendChild(Initiator);
}

function DetectTargetSDK()
{
	var ParentalLink = document.referrer.toLowerCase();
	if (ParentalLink === null || isEmptyOrSpaces(ParentalLink))
	{
		ParentalLink = document.URL.toLowerCase();
	}
	if (true)//ParentalLink.includes("yandex."))
	{		
		TryLog("Detecting Yandex Games environment");
		EnableYandexGames();
		return;
	}
	else if (ParentalLink.includes("crazygames.") || ParentalLink.includes("1001juegos.com"))
	{
		TryLog("Detecting Crazy Games environment");
		EnableCrazyGames();
		return;
	}
	else if (ParentalLink.includes("gamedistribution"))
	{
		TryLog("Detecting GameDistribution environment");
		EnableGameDistribution();
		return;
	}
	else if (ParentalLink.includes("gamepix."))
	{
		TryLog("Detecting GamePix environment");
		EnableGamePix();
		return;
	}
	else if (ParentalLink.includes("gamemonetize"))
	{
		TryLog("Detecting GameMonetize environment");
		EnableGameMonetize();
		return;
	}
	
	function isEmptyOrSpaces(str){
		return str === null || str.match(/^ *$/) !== null;
	}
}

function EnableSDK(Name)
{
	var YandexGamesSDKScript = document.createElement("script");
	YandexGamesSDKScript.src = "TemplateData/" + Name + "/SDK.js";
	document.body.appendChild(YandexGamesSDKScript);
}

function EnableYandexGames() 		{ EnableSDK("YandexGames"); }
function EnableCrazyGames() 		{ EnableSDK("CrazyGames"); }
function EnableGameDistribution() 	{ EnableSDK("GameDistribution"); }
function EnableGamePix() 			{ EnableSDK("GamePix"); }
function EnableGameMonetize() 		{ EnableSDK("GameMonetize"); }

var FullLogEnabled = false;
function TryLog(message)
{
	if (FullLogEnabled)
	{
		console.log(message);
	}
}

function waitFor(conditionFunction) 
{
    const poll = resolve => {
        if(conditionFunction()) resolve();
        else setTimeout(_ => poll(resolve), 500);
    }
    return new Promise(poll);
}