using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperdogController : MonoBehaviour {

    public GameObject lineHelp;
    public GameObject spaceHelp;

    public GameObject LevelController;

    private AddressingController addressingController;

    // Use this for initialization
    void Start () {
        addressingController = LevelController.GetComponent<AddressingController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    protected void OnMouseDown()
    {
        Debug.Log("clicked on superdog");
        addressingController.ShowHelp();
    }

    public IEnumerator FlySuperdog(float duration)
    {
        yield return Transition.Resize(gameObject.transform, Vector3.one, duration / 4);
        yield return Transition.StandingWave(gameObject.transform, Vector3.up, 0.7f, 1, duration / 2);
        yield return Transition.Resize(gameObject.transform, new Vector3(-1f, 1f, 1f), duration / 4);
    }

    public IEnumerator FlySuperdogAway(float duration)
    {
        CircleCollider2D leftCirc = null, rightCirc = null;
        #region Get colliders
        foreach (CircleCollider2D col in gameObject.GetComponentsInChildren<CircleCollider2D>())
        {
            if (leftCirc == null)
            {
                leftCirc = col;
            }
            else
            {
                if (col.bounds.center.x < leftCirc.bounds.center.x)
                {
                    rightCirc = leftCirc;
                    leftCirc = col;
                }
                else
                {
                    rightCirc = col;
                }
            }
        }
        #endregion

        Vector3 start = gameObject.transform.position;
        Vector3 left = leftCirc.bounds.center;
        Vector3 right = rightCirc.bounds.center;

        Vector3 startSize = gameObject.transform.localScale;
        Vector3 midSize = new Vector3(0f, 1.3f * startSize.y, 1f);
        Vector3 finalSize = new Vector3(-1.3f * 1.3f * startSize.x, 1.3f * 1.3f * startSize.y, 1f);

        float elapsed = 0f;
        float p;

        Transform t = gameObject.transform;
        Vector3 startScale = t.localScale;

        do
        {
            p = elapsed / duration;
            t.position = Vector3.Lerp(start, left, Transition.SmoothLerp(p));
            t.localScale = Vector3.Lerp(startSize, midSize, p * p);
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);

        elapsed = 0f;

        do
        {
            p = elapsed / duration;
            t.position = Vector3.Lerp(left, right, Transition.SmoothLerp(p));
            t.localScale = Vector3.Lerp(midSize, finalSize, 1 - (1 - p) * (1 - p));
            yield return new WaitForEndOfFrame();
            elapsed += Time.deltaTime;
        } while (elapsed <= duration);

        Destroy(gameObject);
    }
}
