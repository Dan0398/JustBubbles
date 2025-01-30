using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Merge
{
    public class MergeMainWindow : BaseAnimatedWindow
    {
        const bool AdsAllowed = true;
        const string HeaderLang_Slots = "SaveSlots";
        const string HeaderLang_Themes = "Themes";
        const string HeaderLang_Sizes = "Sizes";
        
        [field:Header("Merge окно")]
        [field:SerializeField] public SlotViewOnScene[] SaveSlots { get; private set; }
        [SerializeField] ThemeConfigView Theme;
        [SerializeField] SizeConfigView Size;
        Gameplay.GameType.Merge lastKnownGameType;
        
        enum Stage{ Hidden, InGame, Slots, Configurator }
        Stage stage;
        
        public bool Shown => stage == Stage.Slots || stage == Stage.Configurator;
        
        void Start()
        {
            Size.Init();
            Theme.Init();
            for (int i = 0; i < SaveSlots.Length; i++)
            {
                SaveSlots[i].Init(this);
            }
        }
        
        public void ShowSlotSelector(Gameplay.GameType.Merge merge, Data.Merge data)
        {
            gameObject.SetActive(true);
            lastKnownGameType = merge;
            if (merge != null && data != null)
            {
                for (int i = 0; i < SaveSlots.Length; i++)
                {
                    SaveSlots[i].RefreshSource(data.SaveSlots[i], i, merge);
                }
            }
            StartCoroutine(FirstShowSlots());
        }
        
        IEnumerator FirstShowSlots()
        {
            SetTurnOffStatus(false);
            FastTurnOffHeaderColor();
            Header.SetNewKey(HeaderLang_Slots);
            StartCoroutine(AnimateHeaderColor(.2f));
            yield return AnimateHeader(.7f);
            yield return new WaitForSeconds(.1f);
            yield return ShowSlotsWindow();
            
            void FastTurnOffHeaderColor()
            {
                var M = Header.GetComponent<MaskableGraphic>();//.color; = Color.white - Color.black;
                var Color =  M.color;
                Color.a = 0;
                M.color = Color;
            }
        }

        IEnumerator ShowSlotsWindow()
        {
            Theme.Hide();
            Size.Hide();
            stage = Stage.Slots;
            yield return AnimateUnwrapWindow(.8f);
            Theme.Hide();
            Size.Hide();
            SetTurnOffStatus(true);
        }

        internal void Hide(float Duration)
        {
            StartCoroutine(HideWindow(Duration));
            stage = Stage.Hidden;
        }
        IEnumerator HideWindow(float Duration = 1.5f)
        {
            SetTurnOffStatus(false);
            var Step = (Duration-0.1f) / 2f;
            yield return AnimateUnwrapWindow(Step, true);
            yield return new WaitForSeconds(.1f);
            StartCoroutine(AnimateHeaderColor(Step, true));
            yield return AnimateHeader(Step, true);
            gameObject.SetActive(false);
        }
        
        Coroutine ConfiguratorRoutine;
        Content.Merge.Selector.Request processedRequest;
        
        public void ShowConfigurator(Content.Merge.Selector.Request request, System.Action subloadTheme)
        {
            stage = Stage.Configurator;
            processedRequest = request;
            ConfiguratorRoutine = StartCoroutine(AnimateConfigurator(subloadTheme));
        }
        
        IEnumerator AnimateConfigurator(System.Action subloadTheme)
        {
            const float WindowUnwrapTime = 0.8f;
            
            var Wait = new WaitForSeconds(.25f);
            bool blocked = false;
            System.Action Unblocker = () => blocked = false;
            
            SetTurnOffStatus(false);
            yield return AnimateUnwrapWindow(WindowUnwrapTime, true);//Свернул
            yield return ChangeHeaderTextAnimated(HeaderLang_Themes, 0.3f); //Поменял заголовок
            yield return new WaitForSeconds(0.3f);
            blocked = true;
            Theme.Show(processedRequest.Theme, AdsAllowed, Unblocker);//Настроил темы
            
            yield return AnimateUnwrapWindow(WindowUnwrapTime, false);//развернул
            SetTurnOffStatus(true);
            while(blocked) yield return Wait;//Ждём юзера
            subloadTheme.Invoke();
            if (processedRequest.ShowAds && AdsAllowed)
            {
                blocked = true;
                ShowAds(Unblocker);
                while(blocked) yield return Wait;//Ждём рекламу
            }
            
            SetTurnOffStatus(false);
            yield return AnimateUnwrapWindow(WindowUnwrapTime, true);//Свернул
            yield return ChangeHeaderTextAnimated(HeaderLang_Sizes, 0.3f);//Поменял заголовок
            yield return new WaitForSeconds(0.3f);
            Theme.Hide();
            blocked = true;
            Size.Show(processedRequest.Size, Unblocker);//Настроил размеры
            
            yield return AnimateUnwrapWindow(WindowUnwrapTime, false);//развернул
            SetTurnOffStatus(true);
            while(blocked) yield return Wait;//Ждём юзера
            
            processedRequest.onDone.Invoke();
            
            async void ShowAds(System.Action onEnd)
            {
                if (await Services.DI.Single<Services.Advertisements.Controller>().IsRewardAdSuccess())
                {
                    onEnd.Invoke();
                }
            }
        }
        
        IEnumerator ChangeHeaderTextAnimated(string LangKey, float Duration)
        {
            yield return AnimateHeaderColor(Duration/3f, true);
            Header.SetNewKey(LangKey);
            yield return AnimateHeaderColor(2f*Duration/3f);
        }
        
        public void TryCloseByUser()
        {
            if (stage == Stage.InGame || stage == Stage.Hidden) return;
            if (stage == Stage.Slots)
            {
                lastKnownGameType.CloseByUser();
            }
            if (stage == Stage.Configurator)
            {
                SetTurnOffStatus(false);
                StopCoroutine(ConfiguratorRoutine);
                processedRequest.Dispose();
                StartCoroutine(AnimateToSlots());
            }
        }
        
        IEnumerator AnimateToSlots()
        {
            StartCoroutine(ChangeHeaderTextAnimated(HeaderLang_Slots, 0.3f));
            yield return AnimateUnwrapWindow(0.8f, true);
            yield return ShowSlotsWindow();
        }
        
    }
}