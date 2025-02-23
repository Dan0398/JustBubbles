using UnityEngine;

namespace Gameplay.Pools
{
    public class BubblePool : BasePool<Bubble>
    {
        [SerializeField] private Content.Bubble _skins;
        
        protected override Bubble ConstructNew() => new(Sample, _skins);
    }
}