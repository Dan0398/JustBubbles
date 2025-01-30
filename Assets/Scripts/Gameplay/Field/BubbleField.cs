using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Gameplay.Field
{
    [AddComponentMenu("Gameplay/Game Field On Scene")]
    public partial class BubbleField: MonoBehaviour, IField
    {
        [SerializeField, Range(0f, 1.0f)] public float Difficulty;
        [SerializeField] public float UpperRelativePlace;
        [field:SerializeField]  public float BubbleSize          { get; private set; }
        [field:SerializeField]  public int BubblesCountPerLine   { get; private set; }
        public System.Action OnFieldRefreshed;
        [SerializeField] Vector2 FieldSize;
        [SerializeField] float FieldUsableSpace;
        [SerializeField] Barriers barriers;
        [SerializeField] Transform EndLine;
        [SerializeField] RectTransform Background;
        [SerializeField] Transform EffectsTransform;
        [SerializeField] SpriteRenderer[] FieldRenderers;
        
        [SerializeField, Header("Bindable")] Pools.BubblePool Pool;
        [SerializeField] Effects.Controller Effects;
        [SerializeField] UI.InGame.InGameCanvas inGameCanvas;
        public ColorStatistic ColorStats { get; private set; }
        System.Action<List<Place>, List<Place>, System.Type> ReactOnBubbleSet;
        Vector2 StartPoint;
        List<LineOfBubbles> Lines;
        bool LastLineShifted = false;
        float LineHeight, ShiftWidth;
        Coroutine LineDownAnimation, ViewsAnimation;
        WaitForFixedUpdate Wait;
        bool EffectsTransformMoveFrozen;
        bool Started;
        
        void Start()
        {
            if (Started) return;
            Started = true;
            Lines = new List<LineOfBubbles>(20);
            Wait = new WaitForFixedUpdate();
            
            ColorStats = new();
            
            RefreshFieldStats();
            SetupBarriers();
        }
    }
}