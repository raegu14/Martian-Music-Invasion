using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Level_Button : MonoBehaviour {

    [SerializeField]
    private Image _musicTile = null;

    [SerializeField]
    private Image _comicTile = null;

    [SerializeField]
    private Text _levelNumber = null;

    [SerializeField]
    private Button _button = null;

    private string _levelString;

    public void Display(Level_Info level)
    {
        _levelString = "Level_Play" + level.LevelNumber;
        _levelNumber.text = level.LevelNumber.ToString();

        if (level.Unlocked)
        {
            _musicTile.sprite = level.MusicSprite;
            _comicTile.sprite = level.ComicSprite;

            _button.interactable = true;
        }
        else if (level.Available)
        {
            _button.interactable = true;
        }
        else
        {
            _button.interactable = false;
        }
    }

    public void LoadLevelScene()
    {
        SceneManager.LoadScene(_levelString);
    }
}
