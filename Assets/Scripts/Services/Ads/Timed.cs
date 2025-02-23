using UnityEngine.Networking;
using UnityEngine;

namespace Services.Advertisements 
{
    public class Timed
    {
        public bool RequestDone     { get; private set; }
        public bool Ready           { get; private set; }
        private int _countBetweenShows;
        private int _countFromStart;
        
        public int CountBetweenShows => _countBetweenShows;
        
        public bool InitialTimingDone => Ready && _countFromStart > Time.time;
        
        public Timed()
        {
            RequestTimings();
        }
        
        private async void RequestTimings()
        {
            for(int i = 0; i < 5; i++)
            {
                var request = UnityWebRequest.Get(@"https://brakelessgames.ru/webkey/request?key=JustBubbles_AdsData");
                request.SendWebRequest();
                while(!request.isDone) await Utilities.Wait();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var parts = request.downloadHandler.text.Split("|");
                    Ready = parts.Length == 2
                        &&  int.TryParse(parts[0], out _countFromStart)
                        &&  _countFromStart >= 0
                        &&  int.TryParse(parts[1], out _countBetweenShows)
                        &&  _countBetweenShows > 0;
                    request.Dispose();
                    break;
                }
                Debug.Log("Error when requesting ads timings: " + request.error);
                request.Dispose();
            }
            RequestDone = true;
        }
    }
}