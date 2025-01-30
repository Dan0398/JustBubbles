using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Gameplay.User
{
    public class MergeTrajectory : MonoBehaviour
    {
        [SerializeField] LineRenderer dropLine;
        [SerializeField] Transform lineTransform;
        [SerializeField] List<Collider2D> Ignore;
        [SerializeField] ExplodeCircle Circle;
        Gradient oldGradient;
        RaycastHit2D[] hits;
        GradientAlphaKey[] trailOldAlphas;
        Vector2 PhysicsOrigin;
        float mineYPos, actualHalfWidth;
        int trailStep, contactsCount;
        bool shown;
        WaitForFixedUpdate Wait;
        Coroutine trailShowRoutine, changeWidthRoutine;
        
        void Start()
        {
            hits = new RaycastHit2D[5];
            oldGradient = dropLine.colorGradient;
            trailOldAlphas = dropLine.colorGradient.alphaKeys;
            mineYPos = transform.position.y;
            ChangeAlpha(0);
            Circle.Init(this);
            trailStep = 0;
            Wait = new();
        }
        
        void ChangeAlpha(float Value)
        {
            trailOldAlphas[1] = new GradientAlphaKey(Value,trailOldAlphas[1].time);
            oldGradient.SetKeys(oldGradient.colorKeys, trailOldAlphas);
            dropLine.colorGradient = oldGradient;
        }
        
        public void ChangeWidth(float newWidth)
        {
            actualHalfWidth = newWidth * 0.5f;
            if (changeWidthRoutine != null) StopCoroutine(changeWidthRoutine);
            changeWidthRoutine = StartCoroutine(ChangeWidthAnimated(newWidth));
        }
        
        IEnumerator ChangeWidthAnimated(float newWidth)
        {
            var oldWidth = dropLine.widthMultiplier;
            for (int i = 1; i <= 15; i++)
            {
                float Lerp = EasingFunction.EaseInSine(0,1, i/15f);
                dropLine.widthMultiplier = Mathf.Lerp(oldWidth, newWidth, Lerp);
                yield return Wait;
            }
        }
        
        public void Show()
        {
            shown = true;
            if (trailShowRoutine != null) StopCoroutine(trailShowRoutine);
            trailShowRoutine = StartCoroutine(AnimateView());
        }
        
        public void Hide()
        {
            shown = false;
            if (trailShowRoutine != null) StopCoroutine(trailShowRoutine);
            trailShowRoutine = StartCoroutine(AnimateView(true));
        }
        
        IEnumerator AnimateView(bool IsHide = false)
        {
            const int Max = 5;
            int EndStep = IsHide? 0 : Max;
            int Dir = IsHide? -1 : 1;
            if (!IsHide) 
            {
                lineTransform.gameObject.SetActive(true);
            }
            while(trailStep != EndStep)
            {
                trailStep += Dir;
                float Lerp = trailStep/(float)Max;
                Circle.Recolor(Lerp);
                ChangeAlpha(Lerp);
                yield return Wait;
            }
            if (IsHide)
            {
                lineTransform.gameObject.SetActive(false);
            }
        }
        
        public void Replace(float XPos)
        {
            if (XPos == PhysicsOrigin.x) return;
            PhysicsOrigin = new Vector2(XPos, mineYPos);
            lineTransform.localPosition = Vector3.right * XPos;
        }
        
        void FixedUpdate()
        {
            RecalculateTrajectory();
        }
        
        void RecalculateTrajectory()
        {
            if (!shown) return;
            contactsCount = Physics2D.CircleCastNonAlloc(PhysicsOrigin, actualHalfWidth, Vector2.down, hits, 15);
            for (int i = 0; i < contactsCount; i++)
            {
                if (Ignore.Contains(hits[i].collider)) continue;
                dropLine.SetPosition(2, new Vector3(0, hits[i].centroid.y - mineYPos, 1));
                Circle.TryReplace(new Vector3(hits[i].centroid.x, hits[i].centroid.y, -1));
                return;
            }
        }
        
        public void ActivateBombView() => Circle.Show();
        
        public void StopBombReplacing() => Circle.DontReplaceBombCircle();
        
        public void HideBombView() => Circle.Hide();
    }
}