using Utils.Observables;

namespace Gameplay.Instruments
{
    [System.Serializable]
    public class Counts
    {
        public System.Action<Content.Instrument.WorkType> OnFailUseInstrument;
        private Pair[] _pairs;
        
        public Counts(Content.Instrument.Config config)
        {
            _pairs = new Pair[config.Instruments.Length];
            for (int i = 0; i < _pairs.Length; i++)
            {
                _pairs[i] = new Pair(config.Instruments[i]);
            }
        }
        
        public Pair GetPair(Content.Instrument.WorkType type)
        {
            foreach(var pair in _pairs)
            {
                if (pair.WorkType == type) return pair;
            }
            return null;
        }
        
        [System.Serializable]
        public class Pair
        {
            public ObsInt Count { get; private set; }
            private Content.Instrument.Config.InstrumentView _source;

            public int IncreaseCount => _source.IncreaseCount;
            
            public Content.Instrument.WorkType WorkType => _source.Type;
            
            public Pair(Content.Instrument.Config.InstrumentView source)
            {
                _source = source;
                Count = 0;
            }
        }
    }
}