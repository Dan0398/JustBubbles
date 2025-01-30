using UnityEngine;
using System;

namespace Services.Audio.Sounds
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Config/Audio/Sounds")]
    public class Config: ScriptableObject
    {
        [field:SerializeField] public string BundlePath { get; private set; }
        [field:SerializeField] public Pair[] SoundPairs   { get; private set; }
        
        [System.Serializable]
        public class Pair
        {
            [SerializeField] string Name; 
            [field:SerializeField] public SoundType Type        { get; private set; }
            [field:SerializeField] public string NameInBundle   { get; private set; }
            [field:SerializeField] public bool Looped           { get; private set; }
            [field:Range(.0f,1.0f)]
            [field:SerializeField] public float DefaultVolume   { get; private set; } = 1f;
            [NonSerialized, HideInInspector] public AudioClip   LoadedData;
            [NonSerialized, HideInInspector] public AudioSource OnScene;
            [field:NonSerialized][field:HideInInspector] public float InGameMastering = 1f;
        }
    }
}