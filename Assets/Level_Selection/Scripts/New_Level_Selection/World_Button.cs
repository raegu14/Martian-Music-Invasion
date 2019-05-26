using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class World_Button : MonoBehaviour
{
    [SerializeField]
    private Image _worldSprite = null;

    [SerializeField]
    private Button _button = null;
    public Button Button => _button;

    private World_Info _worldInfo = null;
    private int _worldNum;
    private bool _selected = false;


    public void Display(World_Info world)
    {
        _worldNum = world.WorldNumber;
        _worldInfo = world;

        if (world.Unlocked)
        {
            _worldSprite.sprite = world.WorldSprite;
            _button.interactable = true;
        }
        else
        {
            _button.interactable = false;
        }

        _selected = false;
    }

    public void OnPointerEnter()
    {
        if (_worldInfo.Unlocked)
        {
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
    }

    public void OnPointerExit()
    {
        if (!_selected)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void SelectWorldRaiser()
    {
        Level_Selection_Util.SelectWorld.Invoke(_worldInfo.WorldNumber);
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
}
