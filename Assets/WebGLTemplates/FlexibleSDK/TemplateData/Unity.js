var canvas = document.querySelector("#unity-canvas");
var warningBanner = document.querySelector("#unity-warning");

var buildUrl = "Build";
var loaderUrl = buildUrl + "/{{{ LOADER_FILENAME }}}";
var config = 
{
	dataUrl: buildUrl + "/{{{ DATA_FILENAME }}}",
	frameworkUrl: buildUrl + "/{{{ FRAMEWORK_FILENAME }}}",
	codeUrl: buildUrl + "/{{{ CODE_FILENAME }}}",
#if MEMORY_FILENAME
        memoryUrl: buildUrl + "/{{{ MEMORY_FILENAME }}}",
#endif
#if SYMBOLS_FILENAME
        symbolsUrl: buildUrl + "/{{{ SYMBOLS_FILENAME }}}",
#endif
        streamingAssetsUrl: "StreamingAssets",
        companyName: {{{ JSON.stringify(COMPANY_NAME) }}},
        productName: {{{ JSON.stringify(PRODUCT_NAME) }}},
        productVersion: {{{ JSON.stringify(PRODUCT_VERSION) }}},
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