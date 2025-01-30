using BrakelessGames.Localization;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace UI.Merge
{
    public class SlotViewOnScene : MonoBehaviour
    {
        [SerializeField] GameObject EmptyParent, DataParent;
        [SerializeField] Image ThemePreview;
        [SerializeField] TextTMPLocalized FieldSizeLabel, ThemeNameLabel, ScoreLabel, MoneyLabel;
        [SerializeField] GameObject PhoneBlocker;
        MergeMainWindow window;
        Content.Merge.ThemesList.Theme actualView;
        Gameplay.Merge.SaveModel data;
        int number;
        bool EnvTouchSubscribed, LangSubscribed, usingTouch;
        Gameplay.GameType.Merge parent;
        Coroutine PhoneBlockerRoutine;
        WaitForFixedUpdate Wait;
        
        bool PhoneBlocked => usingTouch && data.FieldSize != Gameplay.Merge.Barrier.SizeType.Slim;
        
        public void Init(MergeMainWindow parent)
        {
            window = parent;
        }
        
        public void RefreshSource(Gameplay.Merge.SaveModel Data, int ID, Gameplay.GameType.Merge Parent)
        {
            data = Data;
            number = ID;
            parent = Parent;
            TrySubscribeTouch();
            TrySubscribeLangs();
            RefreshViewItem(parent.Views);
            RefreshView();
        }

        void TrySubscribeTouch()
        {
            if (EnvTouchSubscribed) return;
            var Env = Services.DI.Single<Services.Environment>();
            System.Action Refresh = () => 
            {
                usingTouch = Env.IsUsingTouch.Value;
                RefreshView();
            };
            Refresh.Invoke();
            Env.IsUsingTouch.Changed += Refresh;
            EnvTouchSubscribed = true;
        }
        
        void TrySubscribeLangs()
        {
            if (LangSubscribed) return;
            BrakelessGames.Localization.Controller.OnLanguageChange += RefreshView;
            LangSubscribed = true;
        }

        void RefreshViewItem(Content.Merge.ThemesList Views)
        {
            actualView = null;
            if (data == null) return;
            foreach(var item in Views.Themes)
            {
                if (data.BundlePath == item.BundlePath)
                {
                    actualView = item;
                    return;
                }
            }
        }
        
        void RefreshView()
        {
            EmptyParent.SetActive(data == null);
            DataParent.SetActive(data != null);
            if (data != null && actualView != null)
            {
                PhoneBlocker.SetActive(PhoneBlocked);
                ThemePreview.sprite = actualView.Sprite;
                var SizeLocalized = BrakelessGames.Localization.Controller.GetValueByKey(System.Enum.GetName(typeof(Gameplay.Merge.Barrier.SizeType), data.FieldSize));
                FieldSizeLabel.SetNewKeyFormatted("Merge_SizeName_Formatted", new string[] { SizeLocalized });
                
                var ThemeLocalized = BrakelessGames.Localization.Controller.GetValueByKey(actualView.NameLangKey);
                ThemeNameLabel.SetNewKeyFormatted("Merge_ThemeName_Formatted", new string[] { ThemeLocalized });
                ScoreLabel.SetNewKeyFormatted("Score_Formatted", new string[] { data.Points.Value.ToString() });
                MoneyLabel.SetNewKeyFormatted("Money_Formatted", new string[] { data.Money.Value.ToString()});
            }
        }     

        public void DeleteSlot() => parent.DeleteSlot(number, this);
        
        public void StartNewGameFast() => parent.LoadSlot(number); 
        
        public void ConfigureAndStartGame() => parent.ConfigureSlot(number);
        
        public void SelectSlot()
        {
            if (PhoneBlocked)
            {
                if (PhoneBlockerRoutine != null) StopCoroutine(PhoneBlockerRoutine);
                PhoneBlockerRoutine = StartCoroutine(AnimatePhoneBlocker());
                Services.DI.Single<Services.Audio.Sounds.Service>().Play(Services.Audio.Sounds.SoundType.InstrumentFail);
                return;
            }
            parent.LoadSlot(number);
        } 
        
        IEnumerator AnimatePhoneBlocker()
        {
            Wait ??= new();
            var Target = PhoneBlocker.GetComponent<RectTransform>();
            var offsetMin = Target.offsetMin;
            var offsetMax = Target.offsetMax;
            for (int i = 0; i <= 25; i++)
            {
                var Lerp = Mathf.Sin(i/25f * 1440 * Mathf.Deg2Rad) * 10 * Vector2.right;
                Target.offsetMin = offsetMin + Lerp;
                Target.offsetMax = offsetMax + Lerp;
                yield return Wait;
            }
        }
    }
}