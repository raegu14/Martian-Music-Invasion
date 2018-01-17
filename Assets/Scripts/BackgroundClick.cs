using UnityEngine;
using System.Collections;

public class BackgroundClick : MonoBehaviour
{

	private Hero hero;

	public static BackgroundClick singleton;

	private bool clicksEnabled;

	protected void Awake() {
		BackgroundClick.singleton = this;
	}

	protected void Start() {
		this.hero = Hero.singleton;
		this.clicksEnabled = true;
	}

    private void FlyHero()
    {
		if (!this.clicksEnabled)
			return;

        Ray ray;

#if (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
        ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#else
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif

        hero.MoveTo(new Vector3(ray.origin.x, ray.origin.y, 0));

    }

    void OnMouseDown()
    {
        this.FlyHero();
    }

	public void DisableClicks() {
		this.clicksEnabled = false;
	}

	public void EnableClicks() {
		this.clicksEnabled = true;
	}
}
