using UnityEngine.Networking;
using UnityEngine;

namespace Services.Advertisements 
{
    public class Timed
    {
        public bool RequestDone     { get; private set; }
        public bool Ready           { get; private set; }
        int countBetweenShows;
        int countFromStart;
        
        public int CountBetweenShows => countBetweenShows;
        
        public bool InitialTimingDone => Ready && countFromStart > Time.time;
        
        public Timed()
        {
            RequestTimings();
        }
        
        async void RequestTimings()
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
                        &&  int.TryParse(parts[0], out countFromStart)
                        &&  countFromStart >= 0
                        &&  int.TryParse(parts[1], out countBetweenShows)
                        &&  countBetweenShows > 0;
                    #if UNITY_EDITOR
                    /*
                    if (Ready)
                    {
                        countFromStart = 15;
                        countBetweenShows = 13;
                    }
                    */
                    #endif
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