using BrakelessGames.Localization;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Merge
{    
    public class BaseAnimatedWindow : MonoBehaviour
    {
        [field:SerializeField] protected TextTMPLocalized Header { get; private set;}
        [SerializeField] private float _headerUnwrappedPos;
        [SerializeField] private float _windowUnwrappedMinY;
        [SerializeField] private float _windowUnwrappedMaxY;
        [SerializeField] private Image _fade;
        [SerializeField] private Image _windowMask;
        [SerializeField] private RectTransform _windowTransform;
        [SerializeField] private RectTransform _headerTransform;
        [SerializeField] private GameObject _closeButton;
        private Services.Audio.Sounds.Service _sounds;
        private WaitForFixedUpdate _wait;
        
        protected virtual bool AddHalfHeader => true;
        
        protected IEnumerator AnimateHeader(float Duration = 1f, bool IsHide = false)
        {
            _wait ??= new ();
            
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            float HeaderHaflHeight = (_headerTransform.anchorMax.y -  _headerTransform.anchorMin.y) * 0.5f;
            _headerTransform.anchorMin = new Vector2(_headerTransform.anchorMin.x, 0.5f - HeaderHaflHeight);
            _headerTransform.anchorMax = new Vector2(_headerTransform.anchorMax.x, 0.5f + HeaderHaflHeight);
            if (!IsHide) _headerTransform.gameObject.SetActive(true);
            
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp;
                if (IsHide)
                {
                    Lerp = 1 - EasingFunction.EaseOutCirc(0, 1, i/(float)Steps);
                }
                else
                {
                    Lerp = EasingFunction.EaseOutBounce(0,1, i/(float)Steps);
                }
                _headerTransform.localScale = new Vector3(Lerp, 1, 1);
                yield return _wait;
            }
            if (IsHide) _headerTransform.gameObject.SetActive(false);
        }
        
        protected IEnumerator AnimateUnwrapWindow(float Duration = 1f, bool IsHide = false)
        {
            _wait ??= new ();
            if (_sounds == null) _sounds = Services.DI.Single<Services.Audio.Sounds.Service>(); 
            _sounds.Play(Services.Audio.Sounds.SoundType.Unwrap);
            
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            
            float HeaderHaflHeight = (_headerTransform.anchorMax.y -  _headerTransform.anchorMin.y) * 0.5f;
            float WindowAppend = AddHalfHeader? HeaderHaflHeight : 0;
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp = EasingFunction.EaseInOutSine(0, 1, i/(float)Steps);
                if (IsHide) Lerp = 1 - Lerp;
                
                _headerTransform.anchorMin = new Vector2(_headerTransform.anchorMin.x, Mathf.Lerp(0.5f, _headerUnwrappedPos, Lerp) - HeaderHaflHeight);
                _headerTransform.anchorMax = new Vector2(_headerTransform.anchorMax.x, Mathf.Lerp(0.5f, _headerUnwrappedPos, Lerp) + HeaderHaflHeight);
                
                _windowTransform.anchorMax = new Vector2(_windowTransform.anchorMax.x, Mathf.Lerp(0.5f + WindowAppend, _windowUnwrappedMaxY, Lerp));
                _windowTransform.anchorMin = new Vector2(_windowTransform.anchorMin.x, Mathf.Lerp(0.5f + WindowAppend - (_windowUnwrappedMaxY - _windowUnwrappedMinY), _windowUnwrappedMinY, Lerp));
                
                _windowMask.fillAmount = Lerp;
                yield return _wait;
            } 
            _sounds.Stop(Services.Audio.Sounds.SoundType.Unwrap);
        }
        
        protected IEnumerator AnimateHeaderColor(float Duration = 1f, bool IsHide = false)
        {
            _wait ??= new ();
            
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            
            MaskableGraphic Target = Header.GetComponent<MaskableGraphic>();
            Color Usual = new Color(Target.color.r, Target.color.g, Target.color.b, 1);
            Color Clear = Usual - Color.black;
            
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp = i / (float) Steps;
                if (IsHide) Lerp = 1 - Lerp;
                Target.color = Color.Lerp(Clear, Usual, Lerp); 
                yield return _wait;
            }
        }
        
        protected void SetTurnOffStatus(bool enabled)
        {
            _closeButton.SetActive(enabled);
        }
        
        protected IEnumerator AnimateFade(float Duration = 1f, bool IsHide = false)
        {
            _wait ??= new ();
            
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            
            Color Usual = Color.black * 0.7f;
            
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp = i / (float) Steps;
                if (IsHide) Lerp = 1 - Lerp;
                _fade.color = Color.Lerp(Color.clear, Usual, Lerp); 
                yield return _wait;
            }
        }
    }
}