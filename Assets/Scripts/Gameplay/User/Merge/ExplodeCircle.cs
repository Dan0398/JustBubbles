using System.Collections;
using UnityEngine;

namespace Gameplay.User
{
    [System.Serializable]
    public class ExplodeCircle
    {
        [SerializeField] GameObject Circle, Outline;
        Transform circleTransform;
        bool shown, replaceAvailable;
        float mainLerp;
        Pair circlePair, outlinePair;
        MonoBehaviour CoroutineRunner;
        Coroutine outlineRoutine;
        WaitForFixedUpdate Wait;
        
        public void Init(MergeTrajectory parent)
        {
            circlePair = new Pair(Circle);
            outlinePair = new Pair(Outline);
            CoroutineRunner = parent;
            Wait = new();
            circleTransform = Circle.transform;
        }
        
        public void Show()
        {
            shown = true;
            outlineRoutine = CoroutineRunner.StartCoroutine(AnimateOutline());
            Circle.SetActive(true); 
            replaceAvailable = true;
            Recolor(0);
        }
        
        public void Hide()
        {
            shown = false;
            CoroutineRunner.StopCoroutine(outlineRoutine);
            Circle.SetActive(false); 
            replaceAvailable = false;
        }

        public void Recolor(float lerp)
        {
            if (!shown || !replaceAvailable) return;
            mainLerp = lerp;
            circlePair.Recolor(lerp);
        }
        
        public void TryReplace(Vector3 worldPos)
        {
            if (!shown || !replaceAvailable) return;
            circleTransform.position = worldPos;
        }
        
        IEnumerator AnimateOutline()
        {
            const int MaxStep = 50;
            int Step = 0;
            while(true)
            {
                Step ++;
                if (Step >= MaxStep) Step = 0;
                float Lerp = 2f * Step/(float)MaxStep;
                if (Lerp > 1) Lerp = -Lerp + 2;
                outlinePair.Recolor(Lerp * mainLerp);
                yield return Wait;
            }
        }

        public void DontReplaceBombCircle() => replaceAvailable = false;

        class Pair
        {
            GameObject onScene;
            SpriteRenderer renderer;
            Color usual, pure;
            
            public Pair(GameObject onScene)
            {
                this.onScene = onScene;
                renderer = onScene.GetComponent<SpriteRenderer>();
                usual = renderer.color;
                pure = usual - Color.black * usual.a;
            }
            
            public void Recolor(float Scale)
            {
                renderer.color = Color.Lerp(pure, usual, Scale);
            }
        }
    }
}