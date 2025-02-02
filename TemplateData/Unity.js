var canvas = document.querySelector("#unity-canvas");
var warningBanner = document.querySelector("#unity-warning");

var buildUrl = "Build";
var loaderUrl = buildUrl + "/WebGL.loader.js";
var config = 
{
	dataUrl: buildUrl + "/3636fa823ee0b96b9205a901fd91c0e5.data.unityweb",
	frameworkUrl: buildUrl + "/f885f4d0a4e431d7ad8a09dcad79a7a7.js.unityweb",
	codeUrl: buildUrl + "/c110cb1b257bfdb720767ca23512604d.wasm.unityweb",
        streamingAssetsUrl: "StreamingAssets",
        companyName: "BrakelessGames",
        productName: "Just Bubbles",
        productVersion: "1.2",
        showBanner: unityShowBanner,
};
config.devicePixelRatio = 1.5;
LoadInstance();

function unityShowBanner(msg, type)  //Внутрянка, лучше не лезть
{
	function updateBannerVisibility() {
	  warningBanner.style.display = warningBanner.children.length ? 'block' : 'none';
	}
	var div = document.createElement('div');
	div.innerHTML = msg;
	warningBanner.appendChild(div);
	if (type == 'error') div.style = 'background: red; padding: 10px;';
	else {
	  if (type == 'warning') div.style = 'background: yellow; padding: 10px;';
	  setTimeout(function() {
		warningBanner.removeChild(div);
		updateBannerVisibility();
	  }, 5000);
	}
	updateBannerVisibility();
}

function LoadInstance()
{
	var script = document.createElement("script");
	script.src = loaderUrl;
	script.onload = () => 
	{
		createUnityInstance(canvas, config, UpdateProgress)
		.then((unityInstance) => 
		{
			MyEnv.UnityInstance = unityInstance;
			HideLoadingScreen();
			setTimeout(() => TrySubmitLoadingEnd(), 400);
		})
		.catch((message) => {
		  alert(message);
		});
	};
	document.body.appendChild(script);
}

function TrySubmitLoadingEnd()
{
	if (typeof SubmitLoadingEnd !== 'undefined' && SubmitLoadingEnd !== null)
	{
		SubmitLoadingEnd();
	}
}

function SetFullscreen()
{
	if (MyEnv.UnityInstance === null || MyEnv.UnityInstance === undefined)
	{
		return;
	}
	MyEnv.UnityInstance.SetFullscreen(1);
}
