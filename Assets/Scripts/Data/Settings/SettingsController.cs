using Services;

namespace Data
{
    public class SettingsController : BaseController<Settings>
    {
        protected override void SubscribeOnChanges()
        {
            //Data.SoundLevel             .Changed += SaveData;
            //Data.MusicLevel             .Changed += SaveData;
        }
        
        public void ChangeLangWithoutSave(string LangCode)
        {
            
        }
    }
}