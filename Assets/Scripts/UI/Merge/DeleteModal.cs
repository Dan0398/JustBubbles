using BrakelessGames.Localization;
using System.Collections;
using UnityEngine;

public class DeleteModal : MonoBehaviour
{
    [SerializeField] private TextTMPLocalized _description;
    [SerializeField] private RectTransform _windowRect;
    [SerializeField] private float _windowYMin;
    [SerializeField] private float _windowYMax;
    [SerializeField] private RectTransform _headerRect;
    [SerializeField] private float _headerYMin;
    [SerializeField] private float _headerYMax;
    [SerializeField] private CanvasGroup _fader;
    private System.Action _onDelete;
    private WaitForFixedUpdate _wait;
    
    public void Delete()
    {
        _onDelete.Invoke();
        _onDelete = null;
        StartCoroutine(ReturnDelayed());
    }
    
    private IEnumerator ReturnDelayed()
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
        _description.SetNewKeyFormatted("DeleteRequest_Formatted", new string[]{ Number.ToString() });
        _onDelete = onDelete;
        gameObject.SetActive(true);
        StartCoroutine(AnimateWindow());
    }
    
    private IEnumerator AnimateWindow(bool inverted = false, System.Action OnEnd = null)
    {
        _wait ??= new();
        for (int i = 1; i <= 25; i++)
        {
            float Lerp = EasingFunction.EaseOutSine(0,1, i/25f);
            if (inverted) Lerp = 1 - Lerp;
            _fader.alpha = Lerp;
            float Outstand = (1 - Lerp) * .1f;
            _windowRect.anchorMin = new Vector2(.5f, _windowYMin - Outstand);
            _windowRect.anchorMax = new Vector2(.5f, _windowYMax - Outstand);
            _headerRect.anchorMin = new Vector2(.5f, _headerYMin - Outstand);
            _headerRect.anchorMax = new Vector2(.5f, _headerYMax - Outstand);
            yield return _wait;
        }
        OnEnd?.Invoke();
    }
}