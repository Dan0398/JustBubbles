using BrakelessGames.Localization;
using System.Collections;
using UnityEngine;

public class DeleteModal : MonoBehaviour
{
    [SerializeField] TextTMPLocalized Description;
    [SerializeField] RectTransform WindowRect;
    [SerializeField] float WindowYMin, WindowYMax;
    [SerializeField] RectTransform HeaderRect;
    [SerializeField] float HeaderYMin, HeaderYMax;
    [SerializeField] CanvasGroup Fader;
    System.Action OnDelete;
    WaitForFixedUpdate Wait;
    
    public void Delete()
    {
        OnDelete.Invoke();
        OnDelete = null;
        StartCoroutine(ReturnDelayed());
    }
    
    IEnumerator ReturnDelayed()
    {
        yield return new WaitForSeconds(.3f);
        Return();
    }
    
    public void Return()
    {
        StartCoroutine(AnimateWindow(true, () => gameObject.SetActive(false)));
    }
    
    public void ShowWindow(int Number, System.Action onDelete)
    {
        Description.SetNewKeyFormatted("DeleteRequest_Formatted", new string[]{ Number.ToString() });
        OnDelete = onDelete;
        gameObject.SetActive(true);
        StartCoroutine(AnimateWindow());
    }
    
    IEnumerator AnimateWindow(bool inverted = false, System.Action OnEnd = null)
    {
        Wait ??= new();
        for (int i = 1; i <= 25; i++)
        {
            float Lerp = EasingFunction.EaseOutSine(0,1, i/25f);
            if (inverted) Lerp = 1 - Lerp;
            Fader.alpha = Lerp;
            float Outstand = (1 - Lerp) * .1f;
            WindowRect.anchorMin = new Vector2(.5f, WindowYMin - Outstand);
            WindowRect.anchorMax = new Vector2(.5f, WindowYMax - Outstand);
            HeaderRect.anchorMin = new Vector2(.5f, HeaderYMin - Outstand);
            HeaderRect.anchorMax = new Vector2(.5f, HeaderYMax - Outstand);
            yield return Wait;
        }
        OnEnd?.Invoke();
    }
}