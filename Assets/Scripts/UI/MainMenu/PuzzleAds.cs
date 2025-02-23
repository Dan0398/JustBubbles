using UnityEngine.Networking;
using UnityEngine;

namespace UI.Menu
{
    public partial class MainMenu : MonoBehaviour, Services.IService
    {
        [SerializeField] private GameObject _turnablePuzzleAds;
        
        private async void Start()
        {
            var request = UnityWebRequest.Get(@"https://brakelessgames.ru/webkey/request?key=JustBubbles_NeedShowPuzzleAds");
            request.SendWebRequest();
            while(!request.isDone) await Utilities.Wait();
            if (request.result == UnityWebRequest.Result.Success)
            {
                _turnablePuzzleAds.SetActive(request.downloadHandler.text == "Yes");
            }
            request.Dispose();
        }
    }
}