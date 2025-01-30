using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UI.Menu
{
    public partial class MainMenu : MonoBehaviour, Services.IService
    {
        [SerializeField] GameObject TurnablePuzzleAds;
        
        async void Start()
        {
            var request = UnityWebRequest.Get(@"https://brakelessgames.ru/webkey/request?key=JustBubbles_NeedShowPuzzleAds");
            request.SendWebRequest();
            while(!request.isDone) await Utilities.Wait();
            if (request.result == UnityWebRequest.Result.Success)
            {
                TurnablePuzzleAds.SetActive(request.downloadHandler.text == "Yes");
            }
            request.Dispose();
        }
    }
}
