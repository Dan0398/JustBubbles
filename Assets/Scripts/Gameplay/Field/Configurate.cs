using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gameplay.Field
{
    public partial class BubbleField: MonoBehaviour, IField
    {
        public void ReceiveConfig(Config config)
        {
            _reactOnBubbleSet = config.ReactOnBubbleSet;
            _isFieldSizeDynamic = config.IsFieldSizeDynamic;
            _maxAspectRatio = config.MaxAspectRatio;
            UpperRelativePlace = config.RelativeOutstand;
            CheckAspectChange();
        }

        public class Config
        {
            public bool IsFieldSizeDynamic;
            public float MaxAspectRatio;
            public Action<List<Place>, List<Place>, Type> ReactOnBubbleSet;
            public float RelativeOutstand;
        }
    }
}
