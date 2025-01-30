AppendScript();

function AppendScript()
{
	var Script = document.createElement("script");
	Script.src = "/sdk.js";
	Script.onload = () => 
	{
		YaGames.init().then(ysdk => 
		{
			Yandex = ysdk;
			SetupEnvironment();
			ForceLogin();
			InitLeaderboards();
		});
	}
	document.body.appendChild(Script);
}

function SetupEnvironment()
{
	MyEnv.EnvDescription = "Yandex Games";
	MyEnv.IsUsingTouch = !Yandex.deviceInfo.isDesktop();
	MyEnv.LoginAvailable = true;
	MyEnv.IsLogOn = false;
	UpdateLanguageByLangCode(Yandex.environment.i18n.lang);
	MyEnv.LanguageOverwriteByEnv = true;
	MyEnv.MoreGamesTabAvailable = true;
	UpdateMoreGamesLinkByDomain(Yandex.environment.i18n.tld);
	MyEnv.FeedbackAvailable = true;
	MyEnv.RequireSendGameplayStatus = false;
	MyEnv.EnvironmentReady = true;
}

async function ForceLogin()
{
	if(MyEnv.Player === null)
	{
		try
		{
			var pl = await Yandex.getPlayer();
			MyEnv.Player = pl;
			RefreshLogonStatus();
		}
		catch(error) 
		{
			console.error("Yandex Games. Error when try to get player data. Reason:" + error);
			setTimeout(ForceLogin(), 1000);
		}
	}
}

function RefreshLogonStatus()
{
    if (typeof MyEnv.Player === 'undefined' || MyEnv.Player === null) 
	{
        MyEnv.IsLogOn = false;
    }
	else
	{
		MyEnv.IsLogOn = MyEnv.Player.getMode() === "";
	}
}

function UpdateMoreGamesLinkByDomain(Domain)
{
	MyEnv.MoreGamesLink = "https://yandex."+ Domain +"/games/developer?name=BrakeLess%20Games";
    document.puzzlesLink = "https://yandex."+ Domain +"/games/app/281461";
}

function TryLogin()
{
	Yandex.auth.openAuthDialog().then
	if (!IsLogOn)
	{
		Yandex.auth.openAuthDialog().then(() => 
		{
			MyEnv.IsLogOn = true;
			MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'ChangeLogonStatus', IsLogOn.toString());
		}).catch((error) => 
		{
			MyEnv.IsLogOn = false;
			MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'ChangeLogonStatus', IsLogOn.toString());
			console.error("Yandex Games. Error when try to get player data. Reason:" + error);
		});
	}
}

function InitLeaderboards()
{
	if (typeof Yandex === 'undefined' || Yandex === null)
	{
		console.error("YandexSDK not inited");
		return;
	}
	if (typeof Leaderboadrs !== 'undefined' && Leaderboadrs !== null)
    {
        console.error("Leaderboards already inited");
		return;
    }
	var Yandex_Leaderboards = document.createElement("script");
	Yandex_Leaderboards.src = "TemplateData/YandexGames/Leaderbords.js";
	document.body.appendChild(Yandex_Leaderboards);
	TryLog("Yandex Leaderboards inited successfully");
}

function GiveInterstitialDelayFlag()
{
	if (typeof MyEnv.Flags === 'undefined' || MyEnv.Flags === null)
	{
		Yandex.getFlags().then(flags => 
		{
			MyEnv.Flags = flags;
			SendDataToEngine();
		}).catch((error) =>
        {
            console.log(error.message);
            SendDataToEngine();
        });
	}
	else
	{
		SendDataToEngine();
	}
    
    async function SendDataToEngine()
    {
		if (MyEnv.UnityInstance === null)
		{
			await waitFor(_ => (MyEnv.UnityInstance !== null));
		}
        if (typeof MyEnv.Flags === 'undefined' || MyEnv.Flags === null)
        {
            MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'TranslateInterstitialDelay', "-1");
        }
        else
        {
            var Data = MyEnv.Flags.EndlessMode_Interstitial_DelayBetweenShow_Sec;
            MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'TranslateInterstitialDelay', Data.toString());
        }
    }
}

function ShowInterstitial()
{
	Yandex.adv.showFullscreenAdv(
	{
	    callbacks: 
	    { 
		onClose: function(wasShown)
			{
				MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'InterstitialSuccess');
				TryLog("Yandex Interstitial shows successfully");
			},
	    onError: function(error)
			{
				MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'InterstitialFailed');
				TryLog("Yandex Interstitial doesn't shows. Reason:" + error);
			}	
		}
	});
}

function ShowRewardedAd()
{
	var RewardSuccess = false;
	Yandex.adv.showRewardedVideo(
	{
	    callbacks: 
		{
			onRewarded: () => 
		    {
				RewardSuccess  = true;
			},
			onClose: () => 
			{
				if (RewardSuccess)
				{
					MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdSuccess');
					TryLog("Yandex RevardedAd shows successfully");
					return;
				}
				MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdFailed');
				TryLog("Yandex RevardedAd doesn't shows. It is was cloased too early");
			}, 
			onError: (error) => 
			{
				MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'RewardedAdFailed');
				TryLog("Yandex RevardedAd doesn't shows. Reason:" + error);
			}
		}
	});
}

function SaveData(Data)
{
	MyEnv.Player.setData({ "Key" : Data}, true)
	.then(() => 
 		 {
 			 TryLog('Data saved successfully');
 		 });
}

async function LoadData()
{
	if (MyEnv.Player === null || MyEnv.UnityInstance === null)
	{
		await waitFor(_ => (MyEnv.Player !== null && MyEnv.UnityInstance !== null));
	}
	try
	{
		MyEnv.Player.getData().then((_data) =>
		{
			var TargetObject;
			if (_data === null || _data.Key === undefined || _data.Key === null)
			{
				TargetObject = "{}";
			}
			else 
			{
				TargetObject = _data.Key;
			}
			TryLog(TargetObject);
			MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'TranslateData', TargetObject);
		}).catch((error) => 
		{
			var FullError = (`${error.name}: ${error.message}`).toString();
			console.error("Yandex Games. Failed to load data from server: " + FullError);
			MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'TranslateDataError', FullError);
		});
	}
	catch(error)
	{
		var FullError = (`${error.name}: ${error.message}`).toString();
		console.error("Yandex Games. Failed to load data from server. Reason:" + FullError);
		MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'TranslateDataError', FullError);
	}
}


function RequestReview()
{
	Yandex.feedback.canReview().then(({ value: isCanReview, reason: ReasonWhyNot }) => 
	{
		if (isCanReview) 
		{
			Yandex.feedback.requestReview().then(IsFeedbackSent => 
				{
					if (IsFeedbackSent)
					{
						UnityInstance.SendMessage('WebGLCatcher', 'ReviewSuccess');
					}
					else 
					{
						UnityInstance.SendMessage('WebGLCatcher', 'ReviewFail', "Rejected by user");
					}
				});
		} 
		else 
		{
			MyEnv.UnityInstance.SendMessage('WebGLCatcher', 'ReviewFail', ReasonWhyNot);
			console.error('Try to send feedback. Fail. Reason: ' + ReasonWhyNot)
		}
	})
}

function SubmitGameplayStart() { TryLog("Yandex Games. GameplayStart not supported by platform"); }
function SubmitGameplayEnd() { TryLog("Yandex Games. GameplayEnd not supported by platform"); }

var loadingSubmited = false;

function SubmitLoadingStart()
{
    TryLog("Yandex Games. LoadingStart not supported by platform"); 
}

function SubmitLoadingEnd()
{
	if (loadingSubmited === false)
	{
		loadingSubmited = true;
		SubmitLoadingEndDelayed();
	}
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
	Yandex.adv.getBannerAdvStatus().then(({ stickyAdvIsShowing , reason }) => {
    if (!stickyAdvIsShowing) 
		{
			Yandex.adv.showBannerAdv();
		} 
	});
}

function HideBanner()
{
	Yandex.adv.getBannerAdvStatus().then(({ stickyAdvIsShowing , reason }) => {
    if (stickyAdvIsShowing) 
		{
			Yandex.adv.hideBannerAdv();
		} 
	});
}

function GoToWorldOfPuzzles()
{
    window.open(document.puzzlesLink);
}