class Environment 
{
    UnityInstance;
    EnvDescription;
    IsUsingTouch;
    LoginAvailable;
    IsLogOn;
    Player;
    LanguageCode;
    LanguageOverwriteByEnv;
    MoreGamesTabAvailable;
    MoreGamesLink;
    FeedbackAvailable;
    RequireSendGameplayStatus;
    EnvironmentSendFirstTime;
    EnvironmentReady;
    
    constructor()
    {
        this.EnvDescription = '';
        this.UnityInstance = null;
        this.IsUsingTouch = false;
        this.LoginAvailable = false;
        this.IsLogOn = false;
        this.Player = null;
        this.LanguageCode = "en";
        this.MoreGamesTabAvailable = false;
        this.MoreGamesLink = '';
        this.FeedbackAvailable = false;
        this.EnvironmentSendFirstTime = false;
        this.RequireSendGameplayStatus = false;
        this.EnvironmentReady = false;
        this.LanguageOverwriteByEnv = false;
    }
    
    GenericSave = function (Data)
    {
        if (typeof localStorage !== 'undefined' && localStorage !== null)
        {
            localStorage.setItem("Key", Data);
            TryLog('Data saved successfully');
        }
    }
    
    GenericLoad = async function ()
    {
        var TargetObject = "{}"; 
        if (typeof localStorage !== 'undefined' && localStorage !== null)
        {
            TargetObject = localStorage.getItem("Key");
            if (TargetObject === null)
            {
                TargetObject = "{}";
            }
        }
		await waitFor(_ => (this.UnityInstance !== null));
        this.UnityInstance.SendMessage('WebGLCatcher', 'TranslateData', TargetObject);
    }

    GenericRecognizeLanguage = function ()
    {
        var Lang = navigator.language.substring(0,2);
        UpdateLanguageByLangCode(Lang);
    }
    
    GenericRecognizeTouch = function ()
    {
        this.IsUsingTouch = navigator.userAgent.indexOf("Mobi") > -1
    }
}

window.MyEnv = new Environment();
TryLog("Environment Created");

function ShowMoreGames()
{
	if (!MyEnv.MoreGamesTabAvailable) return;
	window.open(MyEnv.MoreGamesLink);
}

function TrySendEnvironmentToEngine()
{
    if (MyEnv.EnvironmentSendFirstTime === false) return;
    SendEnvironmentToEngine();
}

async function SendEnvironmentToEngine()
{
	TryLog("Received Env request");
    if (MyEnv.UnityInstance === null || !MyEnv.EnvironmentReady)
    {
        await waitFor(_ => (MyEnv.UnityInstance !== null && MyEnv.EnvironmentReady === true))
    }
	TryLog("Env Request. Target ready");
    var TranslatedObj = new TranslatedEnvironment(window.MyEnv);
    var TranslatedJSON = JSON.stringify(TranslatedObj);
	TryLog("Env Request. PrepareToSend");
    MyEnv.UnityInstance.SendMessage('WebGLCatcher', "ReceiveEnvironment", TranslatedJSON);
	TryLog("Env Request. Sended");
    if (MyEnv.EnvironmentSendFirstTime === false)
    {
        MyEnv.EnvironmentSendFirstTime = true;
    }
}

class TranslatedEnvironment
{
    IsUsingTouch;
    LoginAvailable;
    IsLogOn;
    LanguageOverwriteByEnv;
    LanguageCode;
    MoreGamesTabAvailable;
    FeedbackAvailable;
    RequireSendGameplayStatus;
    
    constructor (FullEnvironment)
    {
        this.IsUsingTouch = FullEnvironment.IsUsingTouch;
        this.LoginAvailable = FullEnvironment.LoginAvailable;
        this.IsLogOn = FullEnvironment.IsLogOn;
        this.LanguageCode = FullEnvironment.LanguageCode;
        this.LanguageOverwriteByEnv = FullEnvironment.LanguageOverwriteByEnv;
        this.MoreGamesTabAvailable = FullEnvironment.MoreGamesTabAvailable;
        this.FeedbackAvailable = FullEnvironment.FeedbackAvailable;
        this.RequireSendGameplayStatus = FullEnvironment.RequireSendGameplayStatus;
    }
}
