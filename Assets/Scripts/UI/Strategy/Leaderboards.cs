using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace UI.Strategy
{
    [System.Serializable]
    public class Leaderboards
    {
        [SerializeField] GameObject LineSample;
        [SerializeField] GameObject Header, Content;
        [SerializeField] Transform Separator;
        List<LBLineOnScene> shownLines;
        Endgame parent;
        
        public void Init(Endgame Parent)
        {
            parent = Parent;
        }
        
        public void PrepareView(Result Result)
        {
            Header.gameObject.SetActive(false);
            Content.gameObject.SetActive(false);
            CleanOld();
            CreateNewLines(Result);
        }
        
        void CleanOld()
        {
            if (shownLines == null)
            {
                shownLines = new List<LBLineOnScene>();
                return;
            }
            for(int i = 0; i < shownLines.Count; i++)
            {
                shownLines[i].Destroy();
            }
            shownLines.Clear();
            Separator.gameObject.SetActive(false);
        }
        
        async void CreateNewLines(Result result)
        {
            #if UNITY_WEBGL
            if (result.IsNewHighScore) await Utilities.Wait(5000);
            
                #if UNITY_EDITOR && UNITY_WEBGL
                Tester();
                #endif
            
            var Lines = await Services.Web.Catcher.RequestLeaderboard(result.LeaderBoardName);
            for(int i = 0; i < Lines.Length; i++)
            {
                shownLines.Add(new LBLineOnScene(LineSample, Lines[i]));
                if (Separator != null && i > 0 && Lines[i].Rank - Lines[i-1].Rank > 1)
                {
                    Separator.gameObject.SetActive(true);
                    Separator.SetSiblingIndex(shownLines.Count);
                }
            }
            #endif
        }
        
        #if UNITY_EDITOR && UNITY_WEBGL
        async void Tester()
        {
            string TTT = "[{\"ID\":\"OFh0akUqEfQyJtT+MFe2kpYaKiSg8tBINQYw475M4tY=\",\"Name\":\"Валерий Смирнов\",\"Rank\":1,\"Score\":29,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"gL8yzUW8TupOk3c9nRLZ+ya2YDgYpAFdy8NVNROZ4Hs=\",\"Name\":\"Влад Иванов\",\"Rank\":2,\"Score\":30,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"iiXQB9Fc8yYAP0jEqfr2kD9xWqJi2qndjGZXEILDoE8=\",\"Name\":\"людмила л.\",\"Rank\":3,\"Score\":30,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"wR/ScRpyCRlVeTm3wVeRptqxykfr6gnuo4tDhIJwX0o=\",\"Name\":\"Frk Ergl\",\"Rank\":4,\"Score\":31,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"/0UVRU6Hw9eVXnLZWWt8KGU9zR1BYVWQpqemesoSi7o=\",\"Name\":\"Ярослав И.\",\"Rank\":5,\"Score\":32,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/O3FC7LQFP4DROIO6YLRBSVGAL2OGSXPJQTQYGRC2LLJMX7S5VCMTCR5RC53EDICX424GAI2D7DWKCWT4MOV4WFUUUI6VCTG667QBCKLCZPIXKFVUMCNZUYFKZZVMNDTMRBVKUDNKZ57QEFS3UHDH2SCE5I======/islands-retina-medium\"},{\"ID\":\"d3y3oBahNpZe85Lfv4TkjIsIKaNxaBYjMPtTEeObocg=\",\"Name\":\"Катерина Б.\",\"Rank\":71,\"Score\":108,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"r9PK4X5H2Z1n6owWe3qPlaJVBu8l9IC7IpR2RTlmlh4=\",\"Name\":\"Шеба Эс\",\"Rank\":72,\"Score\":109,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/WWMHRBJ4HVTKJXJWUCA5OEEN7IW3YF37V3IWMJPCUYZ2EZ2T4FLFKITS3VQI7MJSQWJM4PJUHWANK4BDLJ4VXGZZERXAC7GTTRYXKGEMV5W5LZL4V2TYR5WLLOAQ7MCYDDQJLXKGUTGX2YZXSDNUA7ZZ/islands-retina-medium\"},{\"ID\":\"ezwevceiP3i4JS0dGeFE+6EHVOVL+JScSamYreB7Lt8=\",\"Name\":\"Татьяна Сивер\",\"Rank\":73,\"Score\":110,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"6oPcAi1I/VHDrOjtodh1cervr6/p2cBwHdaGVHki9jc=\",\"Name\":\"Елена Евсигнеева\",\"Rank\":74,\"Score\":110,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"VQsZF/KXuXUrjvA8JOfya9WWgeugKlSMdslwbRd+zLs=\",\"Name\":\"федор ловков\",\"Rank\":75,\"Score\":111,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"QDMP8YHQCHpueF73CvOUuyGG8n+Qq0PwkQfyC0PwyTM=\",\"Name\":\"Dan398\",\"Rank\":76,\"Score\":112,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"j7yBem8R5ncYvEUdotC9m8d/fxST12A+U5McyM3Z7jo=\",\"Name\":\"татьяна никифорова\",\"Rank\":77,\"Score\":115,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"K7ovcu/c4d+trI9fY2APWWTb+397zKylV6d+yq9WXX4=\",\"Name\":\"Михаил Богуслов\",\"Rank\":78,\"Score\":116,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"GPZT0E/jd5sphYnQO4vxKaFQEjukOjIza8f7Jp99OkI=\",\"Name\":\"corp-instrumentstankiosna70613\",\"Rank\":79,\"Score\":121,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"i2+ekLMns3MXhoSOu5TIXzr7uEpZc5Mjam0rOKDcA34=\",\"Name\":\"Татьяна Коковкина\",\"Rank\":80,\"Score\":127,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"},{\"ID\":\"gpJqWWgiTmt2+yOiyOLEYjs1EHGIBp+k9hwdGYg8niI=\",\"Name\":\"михаил ф.\",\"Rank\":81,\"Score\":127,\"AvatarURL\":\"https://games-sdk.yandex.ru/games/api/sdk/v1/player/avatar/0/islands-retina-medium\"}]";
            await Utilities.Wait(3000);
            GameObject.Find("WebGLCatcher").GetComponent<Services.Web.Catcher>().ApplyLeaderboard(TTT);
        }
        #endif
        
        public void Show()
        {
            Header.gameObject.SetActive(true);
            Content.gameObject.SetActive(true);
        }
        
        class LBLineOnScene
        {
            GameObject onScene;
            
            public LBLineOnScene(GameObject sample, Services.Leaderboards.Line data)
            {
                onScene = GameObject.Instantiate(sample);
                onScene.SetActive(true);
                var Tr = onScene.transform;
                Tr.SetParent(sample.transform.parent);
                Tr.localScale = Vector3.one;
                
                var Number = Tr.GetChild(0).GetComponent<TMP_Text>();
                var Name   = Tr.GetChild(1).GetComponent<TMP_Text>();
                var Score  = Tr.GetChild(2).GetComponent<TMP_Text>();
                
                Number.text = data.Rank.ToString();
                Name.text = data.Name.ToString();
                Score.text = data.Score.ToString();
                
                if (data.IsUser)
                {
                    var Fade = onScene.GetComponent<Image>();
                    Fade.enabled = true;
                    
                    var FadeAnim = onScene.GetComponent<Animation>();
                    FadeAnim.Play();
                }
            }
            
            public void Destroy()
            {
                UnityEngine.Object.Destroy(onScene);
            }
        }
    }
}