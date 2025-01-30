using System.Collections.Generic;
using System.Collections;
using Utils.Observables;
using UnityEngine;
using System;

namespace Gameplay.Merge
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class End : MonoBehaviour
    {
        [SerializeField] SpriteRenderer EndlineView;
        [SerializeField] float FillTime;
        [SerializeField] float timer;
        [SerializeField] Collider2D bombCollider;
        BoxCollider2D mineCollider;
        public ObsFloat RelativeFillChanged { get; private set; } = 0;
        WaitForFixedUpdate Wait;
        bool SomeoneInside;
        List<Collider2D> ignoreList;
        
        public void RegisterAsIgnore(Collider2D ignored)
        {
            ignoreList ??= new();
            ignoreList.Add(ignored);
        }
        
        public void RemoveAsIgnore(Collider2D unIgnored)
        {
            ignoreList ??= new();
            ignoreList.Remove(unIgnored);
        }
        
        void FixedUpdate()
        {
            if (!SomeoneInside && timer > 0)
            {
                timer -= Time.fixedDeltaTime;
                if (timer < 0) timer = 0;
                RelativeFillChanged.Value = timer / FillTime;
            }
            else if (SomeoneInside && timer < FillTime)
            {
                timer += Time.fixedDeltaTime;
                if (timer > FillTime) timer = FillTime;
                RelativeFillChanged.Value = timer / FillTime;
            }
            SomeoneInside = false;
        }
        
        void OnTriggerStay2D(Collider2D col)
        {
            ignoreList ??= new();
            if (ignoreList.Contains(col)) return;
            if (col.Equals(bombCollider)) return;
            SomeoneInside = true;
        }

        public void Clean()
        {
            ignoreList ??= new();
            ignoreList.Clear();
            mineCollider ??= GetComponent<BoxCollider2D>();
            mineCollider.enabled = true;
        }
        
        public void ResetAndShow(float duration)
        {
            RelativeFillChanged.Value = 0;
            timer = 0;
            StartCoroutine(AnimateShow(duration));
        }
        
        IEnumerator AnimateShow(float Duration = 1f, bool Reversed = false)
        {
            Wait ??= new();
            mineCollider ??= GetComponent<BoxCollider2D>();
            if (Reversed) mineCollider.enabled = false;
            int Steps = Mathf.RoundToInt(Duration / Time.fixedDeltaTime);
            if (!Reversed) EndlineView.gameObject.SetActive(true);
            for (int i = 1; i <= Steps; i++)
            {
                float Lerp = EasingFunction.EaseInSine(0, 1, i/(float)Steps);
                if (Reversed) Lerp = 1 - Lerp;
                EndlineView.color = new Color(1,1,1,Lerp);
                yield return Wait;
            }
            if (!Reversed) mineCollider.enabled = true;
            //if (Reversed) EndlineView.gameObject.SetActive(false);
        }
        
        public void Hide(float duration)
        {
            if (EndlineView.color.a != 0)
            {
                StartCoroutine(AnimateShow(duration, true));
            }
        }

        public void RefreshView(float x, float lerp)
        {
            mineCollider ??= GetComponent<BoxCollider2D>();
            mineCollider.size = new Vector2(x, mineCollider.size.y);
            EndlineView.size = new Vector2(x, EndlineView.size.y);
            EndlineView.color = new Color(1, 1, 1, lerp);
        }
    }
}