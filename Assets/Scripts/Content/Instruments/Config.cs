using UnityEngine;

namespace Content.Instrument
{
    [CreateAssetMenu(fileName = "Instruments", menuName = "Config/Instruments", order = 155)]
	public class Config : ScriptableObject
	{
		[field:SerializeField] public InstrumentView[] Instruments  { get; private set; }
        
        [System.Serializable]
        public class InstrumentView
        {
            [field:SerializeField] public WorkType Type             { get; private set; }
            [field:SerializeField] public Sprite Sprite             { get; private set; }
            [field:SerializeField] public string NameLangKey        { get; private set; }
            [field:SerializeField] public string DescriptionLangKey { get; private set; }
            [field:SerializeField] public int IncreaseCount         { get; private set; }
        }
	}
}