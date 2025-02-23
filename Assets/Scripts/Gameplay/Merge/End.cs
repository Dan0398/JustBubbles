using System.Collections.Generic;
using System.Collections;
using Utils.Observables;
using UnityEngine;

namespace Gameplay.Merge
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class End : MonoBehaviour
    {
        public ObsFloat RelativeFillChanged { get; private set; } = 0;
        [SerializeField] SpriteRenderer _endlineView;
        [SerializeField] float _fillTime;
        [SerializeField] float _timer;
        [SerializeField] Collider2D _bombCollider;
        private BoxCollider2D _mineCollider;
        private WaitForFixedUpdate _wait;
        private bool _someoneInside;
        private List<Collider2D> _ignoreList;
        
        public void RegisterAsIgnore(Collider2D ignored)
        {
            _ignoreList ??= new();
            _ignoreList.Add(ignored);
        }
        
        public void RemoveAsIgnore(Collider2D unIgnored)
        {
            _ignoreList ??= new();
            _ignoreList.Remove(unIgnored);
        }
        
        private void FixedUpdate()
        {
            if (!_someoneInside && _timer > 0)
            {
                _timer -= Time.fixedDeltaTime;
                if (_timer < 0) _timer = 0;
                RelativeFillChanged.Value = _timer / _fillTime;
            }
            else if (_someoneInside && _timer < _fillTime)
            {
                _timer += Time.fixedDeltaTime;
                if (_timer > _fillTime) _timer = _fillTime;
                RelativeFillChanged.Value = _timer / _fillTime;
            }
            _someoneInside = false;
        }
        
        private void OnTriggerStay2D(Collider2D col)
        {
            _ignoreList ??= new();
            if (_ignoreList.Contains(col)) return;
            if (col.Equals(_bombCollider)) return;
            _someoneInside = true;
        }

        public void Clean()
        {
            _ignoreList ??= new();
            _ignoreList.Clear();
            _mineCollider ??= GetComponent<BoxCollider2D>();
            _mineCollider.enabled = true;
        }
        
        public void ResetAndShow(float duration)
        {
            RelativeFillChanged.Value = 0;
            _timer = 0;
            StartCoroutine(AnimateShow(duration));
        }
        
        private IEnumerator AnimateShow(float Duration = 1f, bool Reversed = false)
        {
            _wait ??= new();
            _mineCollider ??= GetComponent<BoxCollider2D>();
            if (Reversed) _mineCollider.enabled = false;
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            if (!Reversed) _endlineView.gameObject.SetActive(true);
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp = EasingFunction.EaseInSine(0, 1, i/(float)Steps);
                if (Reversed) Lerp = 1 - Lerp;
                _endlineView.color = new Color(1,1,1,Lerp);
                yield return _wait;
            }
            if (!Reversed) _mineCollider.enabled = true;
        }
        
        public void Hide(float duration)
        {
            if (_endlineView.color.a != 0)
            {
                StartCoroutine(AnimateShow(duration, true));
            }
        }

        public void RefreshView(float x, float lerp)
        {
            _mineCollider ??= GetComponent<BoxCollider2D>();
            _mineCollider.size = new Vector2(x, _mineCollider.size.y);
            _endlineView.size = new Vector2(x, _endlineView.size.y);
            _endlineView.color = new Color(1, 1, 1, lerp);
        }
    }
}