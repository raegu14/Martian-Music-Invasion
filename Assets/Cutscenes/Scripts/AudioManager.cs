using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField]
    private string _assetFolderURL = "http://martianmusicinvasion.com/game_assets/";

    [Header("World BG Audio")]
    [SerializeField]
    private string _jungleFile = "Jungle_Background_Audio";
    [SerializeField]
    private string _cityFile = "City_Background_Audio";
    [SerializeField]
    private string _cloudsFile = "Clouds_Background_Audio";
    [SerializeField]
    private string _spaceFile = "Space_Background_Audio";
    [SerializeField]
    private string _alienBattleFile = "AlienBattleAudio";
    [SerializeField]
    private string _covertAffairFile = "Covert Affair";
    [SerializeField]
    private string _straussOrchestraFile = "StraussOrchestra";

    private string _unityFileExt = ".ogg";
    private string _webFileExt = ".mp3";

    private void Awake()
    {
        StartCoroutine(InitializeAudio());
    }

    private IEnumerator InitializeAudio()
    {
        WWW reader;
#if UNITY_EDITOR
        reader = new WWW(_assetFolderURL + _covertAffairFile + _unityFileExt);
        yield return reader;
        AudioManagerUtility.CovertAffairClip = reader.GetAudioClip();

        reader = new WWW(_assetFolderURL + _alienBattleFile + _unityFileExt);
        yield return reader;
        AudioManagerUtility.AlienBattleClip = reader.GetAudioClip();

        reader = new WWW(_assetFolderURL + _jungleFile + _unityFileExt);
        yield return reader;
        AudioManagerUtility.JungleClip = reader.GetAudioClip();

        reader = new WWW(_assetFolderURL + _cityFile + _unityFileExt);
        yield return reader;
        AudioManagerUtility.CityClip = reader.GetAudioClip();

        reader = new WWW(_assetFolderURL + _cloudsFile + _unityFileExt);
        yield return reader;
        AudioManagerUtility.CloudsClip = reader.GetAudioClip();

        reader = new WWW(_assetFolderURL + _spaceFile + _unityFileExt);
        yield return reader;
        AudioManagerUtility.SpaceClip = reader.GetAudioClip();

        reader = new WWW(_assetFolderURL + _straussOrchestraFile + _unityFileExt);
        yield return reader;
        AudioManagerUtility.StraussOrchestraClip = reader.GetAudioClip();
#endif

#if UNITY_WEBGL
        reader = new WWW(_assetFolderURL + _covertAffairFile + _webFileExt);
        yield return reader;
        AudioManagerUtility.CovertAffairClip = reader.GetAudioClip();

        reader = new WWW(_assetFolderURL + _alienBattleFile + _webFileExt);
        yield return reader;
        AudioManagerUtility.AlienBattleClip = reader.GetAudioClip();

        reader = new WWW(_assetFolderURL + _jungleFile + _webFileExt);
        yield return reader;
        AudioManagerUtility.JungleClip = reader.GetAudioClip();

        reader = new WWW(_assetFolderURL + _cityFile + _webFileExt);
        yield return reader;
        AudioManagerUtility.CityClip = reader.GetAudioClip();

        reader = new WWW(_assetFolderURL + _cloudsFile + _webFileExt);
        yield return reader;
        AudioManagerUtility.CloudsClip = reader.GetAudioClip();

        reader = new WWW(_assetFolderURL + _spaceFile + _webFileExt);
        yield return reader;
        AudioManagerUtility.SpaceClip = reader.GetAudioClip();

        reader = new WWW(_assetFolderURL + _straussOrchestraFile + _webFileExt);
        yield return reader;
        AudioManagerUtility.StraussOrchestraClip = reader.GetAudioClip();
#endif
    }
}

public static class AudioManagerUtility
{
    /* Intro */
    public static AudioClip AlienBattleClip;
    public static AudioClip CovertAffairClip;
    public static AudioClip StraussOrchestraClip;

    /* Level BGs */
    public static AudioClip JungleClip;
    public static AudioClip CityClip;
    public static AudioClip CloudsClip;
    public static AudioClip SpaceClip;


}