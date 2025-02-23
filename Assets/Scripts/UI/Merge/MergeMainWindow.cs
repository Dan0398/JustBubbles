using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Merge
{
    public class MergeMainWindow : BaseAnimatedWindow
    {
        private const bool AdsAllowed = true;
        private const string HeaderLang_Slots = "SaveSlots";
        private const string HeaderLang_Themes = "Themes";
        private const string HeaderLang_Sizes = "Sizes";
        
        [field:Header("Merge окно")]
        [field:SerializeField] public SlotViewOnScene[] SaveSlots { get; private set; }
        [SerializeField] private ThemeConfigView _theme;
        [SerializeField] private SizeConfigView _size;
        private Gameplay.GameType.Merge _lastKnownGameType;
        private Coroutine _configuratorRoutine;
        private Content.Merge.Selector.Request _processedRequest;
        private Stage _stage;
        
        private enum Stage{ Hidden, InGame, Slots, Configurator }
        
        public bool Shown => _stage == Stage.Slots || _stage == Stage.Configurator;
        
        private void Start()
        {
            _size.Init();
            _theme.Init();
        }
        
        public void ShowSlotSelector(Gameplay.GameType.Merge merge, Data.Merge data)
        {
            gameObject.SetActive(true);
            _lastKnownGameType = merge;
            if (merge != null && data != null)
            {
                for (int i = 0; i < SaveSlots.Length; i++)
                {
                    SaveSlots[i].RefreshSource(data.SaveSlots[i], i, merge);
                }
            }
            StartCoroutine(FirstShowSlots());
        }
        
        private IEnumerator FirstShowSlots()
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
                var M = Header.GetComponent<MaskableGraphic>();
                var Color =  M.color;
                Color.a = 0;
                M.color = Color;
            }
        }

        private IEnumerator ShowSlotsWindow()
        {
            _theme.Hide();
            _size.Hide();
            _stage = Stage.Slots;
            yield return AnimateUnwrapWindow(.8f);
            _theme.Hide();
            _size.Hide();
            SetTurnOffStatus(true);
        }

        public void Hide(float Duration)
        {
            StartCoroutine(HideWindow(Duration));
            _stage = Stage.Hidden;
        }
        
        private IEnumerator HideWindow(float Duration = 1.5f)
        {
            SetTurnOffStatus(false);
            var Step = (Duration-0.1f) / 2f;
            yield return AnimateUnwrapWindow(Step, true);
            yield return new WaitForSeconds(.1f);
            StartCoroutine(AnimateHeaderColor(Step, true));
            yield return AnimateHeader(Step, true);
            gameObject.SetActive(false);
        }
        
        public void ShowConfigurator(Content.Merge.Selector.Request request, System.Action subloadTheme)
        {
            _stage = Stage.Configurator;
            _processedRequest = request;
            _configuratorRoutine = StartCoroutine(AnimateConfigurator(subloadTheme));
        }
        
        private IEnumerator AnimateConfigurator(System.Action subloadTheme)
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
            _theme.Show(_processedRequest.Theme, AdsAllowed, Unblocker);//Настроил темы
            
            yield return AnimateUnwrapWindow(WindowUnwrapTime, false);//развернул
            SetTurnOffStatus(true);
            while(blocked) yield return Wait;//Ждём юзера
            subloadTheme.Invoke();
            if (_processedRequest.ShowAds && AdsAllowed)
            {
                blocked = true;
                ShowAds(Unblocker);
                while(blocked) yield return Wait;//Ждём рекламу
            }
            
            SetTurnOffStatus(false);
            yield return AnimateUnwrapWindow(WindowUnwrapTime, true);//Свернул
            yield return ChangeHeaderTextAnimated(HeaderLang_Sizes, 0.3f);//Поменял заголовок
            yield return new WaitForSeconds(0.3f);
            _theme.Hide();
            blocked = true;
            _size.Show(_processedRequest.Size, Unblocker);//Настроил размеры
            
            yield return AnimateUnwrapWindow(WindowUnwrapTime, false);//развернул
            SetTurnOffStatus(true);
            while(blocked) yield return Wait;//Ждём юзера
            
            _processedRequest.OnDone.Invoke();

            static async void ShowAds(System.Action onEnd)
            {
                if (await Services.DI.Single<Services.Advertisements.Controller>().IsRewardAdSuccess())
                {
                    onEnd.Invoke();
                }
            }
        }
        
        private IEnumerator ChangeHeaderTextAnimated(string LangKey, float Duration)
        {
            yield return AnimateHeaderColor(Duration/3f, true);
            Header.SetNewKey(LangKey);
            yield return AnimateHeaderColor(2f*Duration/3f);
        }
        
        public void TryCloseByUser()
        {
            if (_stage == Stage.InGame || _stage == Stage.Hidden) return;
            if (_stage == Stage.Slots)
            {
                _lastKnownGameType.CloseByUser();
            }
            if (_stage == Stage.Configurator)
            {
                SetTurnOffStatus(false);
                StopCoroutine(_configuratorRoutine);
                StartCoroutine(AnimateToSlots());
            }
        }
        
        private IEnumerator AnimateToSlots()
        {
            StartCoroutine(ChangeHeaderTextAnimated(HeaderLang_Slots, 0.3f));
            yield return AnimateUnwrapWindow(0.8f, true);
            yield return ShowSlotsWindow();
        }
    }
}