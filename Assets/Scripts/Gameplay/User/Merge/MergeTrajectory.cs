using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Gameplay.User
{
    public class MergeTrajectory : MonoBehaviour
    {
        [SerializeField] private LineRenderer _dropLine;
        [SerializeField] private Transform _lineTransform;
        [SerializeField] private List<Collider2D> _ignore;
        [SerializeField] private ExplodeCircle _circle;
        private Gradient _oldGradient;
        private RaycastHit2D[] _hits;
        private GradientAlphaKey[] _trailOldAlphas;
        private Vector2 _physicsOrigin;
        private float _mineYPos, _actualHalfWidth;
        private int _trailStep, _contactsCount;
        private bool _shown;
        private WaitForFixedUpdate _wait;
        private Coroutine _trailShowRoutine, _changeWidthRoutine;
        
        private void Start()
        {
            _hits = new RaycastHit2D[5];
            _oldGradient = _dropLine.colorGradient;
            _trailOldAlphas = _dropLine.colorGradient.alphaKeys;
            _mineYPos = transform.position.y;
            ChangeAlpha(0);
            _circle.Init(this);
            _trailStep = 0;
            _wait = new();
        }
        
        private void ChangeAlpha(float Value)
        {
            _trailOldAlphas[1] = new GradientAlphaKey(Value,_trailOldAlphas[1].time);
            _oldGradient.SetKeys(_oldGradient.colorKeys, _trailOldAlphas);
            _dropLine.colorGradient = _oldGradient;
        }
        
        public void ChangeWidth(float newWidth)
        {
            _actualHalfWidth = newWidth * 0.5f;
            if (_changeWidthRoutine != null) StopCoroutine(_changeWidthRoutine);
            _changeWidthRoutine = StartCoroutine(ChangeWidthAnimated(newWidth));
        }
        
        private IEnumerator ChangeWidthAnimated(float newWidth)
        {
            var oldWidth = _dropLine.widthMultiplier;
            for (int i = 1; i <= 15; i++)
            {
                float Lerp = EasingFunction.EaseInSine(0,1, i/15f);
                _dropLine.widthMultiplier = Mathf.Lerp(oldWidth, newWidth, Lerp);
                yield return _wait;
            }
        }
        
        public void Show()
        {
            _shown = true;
            if (_trailShowRoutine != null) StopCoroutine(_trailShowRoutine);
            _trailShowRoutine = StartCoroutine(AnimateView());
        }
        
        public void Hide()
        {
            _shown = false;
            if (_trailShowRoutine != null) StopCoroutine(_trailShowRoutine);
            _trailShowRoutine = StartCoroutine(AnimateView(true));
        }
        
        private IEnumerator AnimateView(bool IsHide = false)
        {
            const int Max = 5;
            int EndStep = IsHide? 0 : Max;
            int Dir = IsHide? -1 : 1;
            if (!IsHide) 
            {
                _lineTransform.gameObject.SetActive(true);
            }
            while(_trailStep != EndStep)
            {
                _trailStep += Dir;
                float Lerp = _trailStep/(float)Max;
                _circle.Recolor(Lerp);
                ChangeAlpha(Lerp);
                yield return _wait;
            }
            if (IsHide)
            {
                _lineTransform.gameObject.SetActive(false);
            }
        }
        
        public void Replace(float XPos)
        {
            if (XPos == _physicsOrigin.x) return;
            _physicsOrigin = new Vector2(XPos, _mineYPos);
            _lineTransform.localPosition = Vector3.right * XPos;
        }
        
        private void FixedUpdate()
        {
            RecalculateTrajectory();
        }
        
        private void RecalculateTrajectory()
        {
            if (!_shown) return;
            _contactsCount = Physics2D.CircleCastNonAlloc(_physicsOrigin, _actualHalfWidth, Vector2.down, _hits, 15);
            for (int i = 0; i < _contactsCount; i++)
            {
                if (_ignore.Contains(_hits[i].collider)) continue;
                _dropLine.SetPosition(2, new Vector3(0, _hits[i].centroid.y - _mineYPos, 1));
                _circle.TryReplace(new Vector3(_hits[i].centroid.x, _hits[i].centroid.y, -1));
                return;
            }
        }
        
        public void ActivateBombView()
        {
            _circle.Show();
        }
        
        public void StopBombReplacing()
        {
            _circle.DontReplaceBombCircle();
        }
        
        public void HideBombView()
        {
            _circle.Hide();
        }
    }
}