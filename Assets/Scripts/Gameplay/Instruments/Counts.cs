using Utils.Observables;

namespace Gameplay.Instruments
{
    [System.Serializable]
    public class Counts
    {
        public System.Action<Content.Instrument.WorkType> OnFailUseInstrument;
        Pair[] pairs;
        
        public Counts(Content.Instrument.Config config)
        {
            pairs = new Pair[config.Instruments.Length];
            for (int i = 0; i < pairs.Length; i++)
            {
                pairs[i] = new Pair(config.Instruments[i]);
            }
        }
        
        public Pair GetPair(Content.Instrument.WorkType type)
        {
            foreach(var pair in pairs)
            {
                if (pair.WorkType == type) return pair;
            }
            return null;
        }
        
        [System.Serializable]
        public class Pair
        {
            public ObsInt Count { get; private set; }
            Content.Instrument.Config.InstrumentView source;

            public int IncreaseCount => source.IncreaseCount;
            public Content.Instrument.WorkType WorkType => source.Type;
            
            public Pair(Content.Instrument.Config.InstrumentView source)
            {
                Count = 0;
                this.source = source;
            }
        }
    }
}