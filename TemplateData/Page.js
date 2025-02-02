var container = document.querySelector("#LoadContainer");
var progressTitle = document.querySelector("#UnpackText");
var LoadProgress = +0;

function UpdateProgress(Progress)
{
	LoadProgress = Progress;
	UpdateLoadingBar();
}

function UpdateLoadingBar()
{
	var Lerp = (100/90) * LoadProgress;
	document.body.style.setProperty("--angle",  (Math.round(Lerp * 100) + "%"));
	
	if (progressTitle === null)
	{
		progressTitle = document.querySelector("#UnpackText");
	}
	if (LoadProgress >= +0.9)
	{
		document.querySelector("#UnpackText").style.display = "block";
		document.querySelector("#UnpackText").textContent = UnpackingLabel;
	}
	else 
	{
		progressTitle.style.display = "none";
	}
}

function HideLoadingScreen()
{
	if (container === null)
	{
		container = document.querySelector("#LoadContainer");
	}
	container.style.display = "none";
}
