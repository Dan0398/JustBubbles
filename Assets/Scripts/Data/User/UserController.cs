namespace Data
{
    public class UserController : BaseController<User>
    {
        protected override void SubscribeOnChanges()
        {
            Data.SurvivalBestScore.Changed += SaveData;
        }
    }
}