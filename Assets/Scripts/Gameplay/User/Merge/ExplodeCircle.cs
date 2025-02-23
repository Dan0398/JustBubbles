using System.Collections;
using UnityEngine;

namespace Gameplay.User
{
    [System.Serializable]
    public class ExplodeCircle
    {
        [SerializeField] private GameObject _circle;
        [SerializeField] private GameObject _outline;
        private Transform _circleTransform;
        private bool _shown, _replaceAvailable;
        private float _mainLerp;
        private Pair _circlePair, _outlinePair;
        private MonoBehaviour _coroutineRunner;
        private Coroutine _outlineRoutine;
        private WaitForFixedUpdate _wait;
        
        public void Init(MergeTrajectory parent)
        {
            _circlePair = new Pair(_circle);
            _outlinePair = new Pair(_outline);
            _coroutineRunner = parent;
            _wait = new();
            _circleTransform = _circle.transform;
        }
        
        public void Show()
        {
            _shown = true;
            _outlineRoutine = _coroutineRunner.StartCoroutine(AnimateOutline());
            _circle.SetActive(true); 
            _replaceAvailable = true;
            Recolor(0);
        }
        
        public void Hide()
        {
            _shown = false;
            _coroutineRunner.StopCoroutine(_outlineRoutine);
            _circle.SetActive(false); 
            _replaceAvailable = false;
        }

        public void Recolor(float lerp)
        {
            if (!_shown || !_replaceAvailable) return;
            _mainLerp = lerp;
            _circlePair.Recolor(lerp);
        }
        
        public void TryReplace(Vector3 worldPos)
        {
            if (!_shown || !_replaceAvailable) return;
            _circleTransform.position = worldPos;
        }
        
        private IEnumerator AnimateOutline()
        {
            const int MaxStep = 50;
            int Step = 0;
            while(true)
            {
                Step ++;
                if (Step >= MaxStep) Step = 0;
                float Lerp = 2f * Step/(float)MaxStep;
                if (Lerp > 1) Lerp = -Lerp + 2;
                _outlinePair.Recolor(Lerp * _mainLerp);
                yield return _wait;
            }
        }

        public void DontReplaceBombCircle() => _replaceAvailable = false;

        private class Pair
        {
            private SpriteRenderer _renderer;
            private Color _usual, _pure;
            
            public Pair(GameObject onScene)
            {
                _renderer = onScene.GetComponent<SpriteRenderer>();
                _usual = _renderer.color;
                _pure = _usual - Color.black * _usual.a;
            }
            
            public void Recolor(float Scale)
            {
                _renderer.color = Color.Lerp(_pure, _usual, Scale);
            }
        }
    }
}