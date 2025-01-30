using BrakelessGames.Localization;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI.Merge
{    
    public class BaseAnimatedWindow : MonoBehaviour
    {
        protected virtual bool AddHalfHeader => true;
        [SerializeField] float HeaderUnwrappedPos;
        [SerializeField] float WindowUnwrappedMinY, WindowUnwrappedMaxY;
        [SerializeField] Image Fade;
        [SerializeField] Image WindowMask;
        [SerializeField] RectTransform WindowTransform, HeaderTransform;
        [field:SerializeField] protected TextTMPLocalized Header { get; private set;}
        [SerializeField] GameObject CloseButton;
        Services.Audio.Sounds.Service Sounds;
        WaitForFixedUpdate Wait;
        
        protected IEnumerator AnimateHeader(float Duration = 1f, bool IsHide = false)
        {
            Wait ??= new ();
            
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            float HeaderHaflHeight = (HeaderTransform.anchorMax.y -  HeaderTransform.anchorMin.y) * 0.5f;
            HeaderTransform.anchorMin = new Vector2(HeaderTransform.anchorMin.x, 0.5f - HeaderHaflHeight);
            HeaderTransform.anchorMax = new Vector2(HeaderTransform.anchorMax.x, 0.5f + HeaderHaflHeight);
            if (!IsHide) HeaderTransform.gameObject.SetActive(true);
            
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp;
                if (IsHide)
                {
                    Lerp = 1 - EasingFunction.EaseOutCirc(0, 1, i/(float)Steps);
                }
                else
                {
                    //Lerp = EasingFunction.EaseInOutSine(0, 1, i/(float)Steps);
                    Lerp = EasingFunction.EaseOutBounce(0,1, i/(float)Steps);
                }
                HeaderTransform.localScale = new Vector3(Lerp, 1, 1);
                yield return Wait;
            }
            if (IsHide) HeaderTransform.gameObject.SetActive(false);
        }
        
        protected IEnumerator AnimateUnwrapWindow(float Duration = 1f, bool IsHide = false)
        {
            Wait ??= new ();
            Sounds ??= Services.DI.Single<Services.Audio.Sounds.Service>(); 
            Sounds.Play(Services.Audio.Sounds.SoundType.Unwrap);
            
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            
            float HeaderHaflHeight = (HeaderTransform.anchorMax.y -  HeaderTransform.anchorMin.y) * 0.5f;
            float WindowAppend = AddHalfHeader? HeaderHaflHeight : 0;
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp = EasingFunction.EaseInOutSine(0, 1, i/(float)Steps);
                if (IsHide) Lerp = 1 - Lerp;
                
                HeaderTransform.anchorMin = new Vector2(HeaderTransform.anchorMin.x, Mathf.Lerp(0.5f, HeaderUnwrappedPos, Lerp) - HeaderHaflHeight);
                HeaderTransform.anchorMax = new Vector2(HeaderTransform.anchorMax.x, Mathf.Lerp(0.5f, HeaderUnwrappedPos, Lerp) + HeaderHaflHeight);
                
                WindowTransform.anchorMax = new Vector2(WindowTransform.anchorMax.x, Mathf.Lerp(0.5f + WindowAppend, WindowUnwrappedMaxY, Lerp));
                WindowTransform.anchorMin = new Vector2(WindowTransform.anchorMin.x, Mathf.Lerp(0.5f + WindowAppend - (WindowUnwrappedMaxY - WindowUnwrappedMinY), WindowUnwrappedMinY, Lerp));
                
                WindowMask.fillAmount = Lerp;
                yield return Wait;
            } 
            Sounds.Stop(Services.Audio.Sounds.SoundType.Unwrap);
        }
        
        protected IEnumerator AnimateHeaderColor(float Duration = 1f, bool IsHide = false)
        {
            Wait ??= new ();
            
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            
            MaskableGraphic Target = Header.GetComponent<MaskableGraphic>();
            Color Usual = new Color(Target.color.r, Target.color.g, Target.color.b, 1);
            Color Clear = Usual - Color.black;
            
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp = i / (float) Steps;
                if (IsHide) Lerp = 1 - Lerp;
                Target.color = Color.Lerp(Clear, Usual, Lerp); 
                yield return Wait;
            }
        }
        
        protected void SetTurnOffStatus(bool enabled) => CloseButton.SetActive(enabled);
        
        protected IEnumerator AnimateFade(float Duration = 1f, bool IsHide = false)
        {
            Wait ??= new ();
            
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            
            Color Usual = Color.black * 0.7f;
            
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp = i / (float) Steps;
                if (IsHide) Lerp = 1 - Lerp;
                Fade.color = Color.Lerp(Color.clear, Usual, Lerp); 
                yield return Wait;
            }
        }
    }
}