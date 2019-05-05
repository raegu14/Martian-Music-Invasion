using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public class Level_Button : MonoBehaviour {

    [SerializeField]
    private Image _musicTile = null;
    public Image MusicTile => _musicTile;

    [SerializeField]
    private Image _comicTile = null;

    [SerializeField]
    private Text _levelNumber = null;

    [SerializeField]
    private Canvas _canvas = null;

    [SerializeField]
    private Button _button = null;

    private Level_Info _levelInfo;
    private bool _selected;

    public void Display(Level_Info level, bool locked = false)
    {
        _levelInfo = level;
        _levelNumber.text = level.LevelNumber.ToString();

        if (level.Unlocked && !locked)
        {
            _musicTile.sprite = level.MusicSprite;
            _comicTile.sprite = level.ComicSprite;

            _button.interactable = true;
        }
        else if (level.Available)
        {
            _button.interactable = true;
            _comicTile.sprite = level.ComicSprite;
        }
        else
        {
            _button.interactable = false;
        }
    }

    public void OnPointerEnter()
    {
        if (_levelInfo.Available || _levelInfo.Unlocked)
        {
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = SortingLayer.GetLayerValueFromName("LevelSelectionAnimation");
        }
    }

    public void OnPointerExit()
    {
        if (!_selected)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            _canvas.sortingOrder = SortingLayer.GetLayerValueFromName("LevelSelection");
            _canvas.overrideSorting = false;
        }
    }

    public void UnSelect()
    {
        _selected = false;
        OnPointerExit();
    }

    public void Select()
    {
        _selected = true;
        OnPointerEnter();
    }

    public void SelectLevelRaiser()
    {
        Level_Selection_Util.SelectLevel.Invoke(_levelInfo.LevelNumber);
    }

    public void HoverOverLevelRaiser()
    {
        Level_Selection_Util.HoverOverLevel.Invoke(_levelInfo.LevelNumber);
    }
}
