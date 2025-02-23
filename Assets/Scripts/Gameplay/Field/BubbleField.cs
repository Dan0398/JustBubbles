using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Field
{
    [AddComponentMenu("Gameplay/Game Field On Scene")]
    public partial class BubbleField: MonoBehaviour, IField
    {
        [field:SerializeField]  public float BubbleSize          { get; private set; }
        [field:SerializeField]  public int BubblesCountPerLine   { get; private set; }
        [Range(0f, 1.0f)] public float Difficulty;
        public ColorStatistic ColorStats { get; private set; }
        public float UpperRelativePlace;
        public System.Action OnFieldRefreshed;
        [SerializeField] Vector2 _fieldSize;
        [SerializeField] float _fieldUsableSpace;
        [SerializeField] Barriers _barriers;
        [SerializeField] Transform _endLine;
        [SerializeField] RectTransform _background;
        [SerializeField] Transform _effectsTransform;
        [SerializeField] SpriteRenderer[] _fieldRenderers;
        
        [SerializeField, Header("Bindable")] Pools.BubblePool _pool;
        [SerializeField] Effects.Controller _effects;
        [SerializeField] UI.InGame.InGameCanvas _inGameCanvas;
        private System.Action<List<Place>, List<Place>, System.Type> _reactOnBubbleSet;
        private Vector2 _startPoint;
        private List<LineOfBubbles> _lines;
        private bool _lastLineShifted = false;
        private float _lineHeight, _shiftWidth;
        private Coroutine _lineDownAnimation, _viewsAnimation;
        private WaitForFixedUpdate _wait;
        private bool _effectsTransformMoveFrozen;
        private bool _started;
        
        private void Start()
        {
            if (_started) return;
            _started = true;
            _lines = new List<LineOfBubbles>(20);
            _wait = new WaitForFixedUpdate();
            
            ColorStats = new();
            
            RefreshFieldStats();
            SetupBarriers();
        }
    }
}