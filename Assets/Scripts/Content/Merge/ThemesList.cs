using UnityEngine;

namespace Content.Merge
{
    [CreateAssetMenu(fileName = "MergeThemes", menuName = "Config/Merge/Themes Cfg", order = 160)]
	public class ThemesList: ScriptableObject
	{
		[field:SerializeField] public Theme[] Themes  { get; private set; }
        
        [System.Serializable]
        public class Theme
        {
            [field:SerializeField] public string NameLangKey        { get; private set; }
            [field:SerializeField] public Sprite Sprite             { get; private set; }
            [field:SerializeField] public string BundlePath         { get; private set; }
            [field:SerializeField] public bool AdsRequired          { get; private set; }
            [System.NonSerialized] public ViewConfig Loaded;
            [System.NonSerialized] public Services.Bundles.ContentPart DownloadRequest;
        }
	}
}