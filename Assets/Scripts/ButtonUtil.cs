using UnityEngine;
using UnityEngine.UI;

public class ButtonUtil
{
	public static void Show(GameObject button)
    {
        button.SetActive(true);
        return;

        /* button.GetComponent<Image>().raycastTarget = true;
        button.GetComponent<Button>().enabled = true;
        button.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
        foreach (CanvasRenderer cr in button.GetComponentsInChildren<CanvasRenderer>())
        {
            cr.SetAlpha(1.0f);
        }
        */
    }

    public static void Hide(GameObject button)
    {
        button.SetActive(false);
        return;

        /* button.GetComponent<Image>().raycastTarget = false;
        button.GetComponent<Button>().enabled = false;
        button.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
        foreach (CanvasRenderer cr in button.GetComponentsInChildren<CanvasRenderer>())
        {
            cr.SetAlpha(0.0f);
        } */
    }
}
