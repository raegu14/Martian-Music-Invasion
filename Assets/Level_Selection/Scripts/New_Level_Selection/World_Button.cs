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

    private int worldNum;

    public void Display(World_Info world)
    {
        worldNum = world.WorldNumber;

        if (world.Unlocked)
        {
            _worldSprite.sprite = world.WorldSprite;
            _button.interactable = true;
        }
        else
        {
            _button.interactable = false;
        }
    }
}
