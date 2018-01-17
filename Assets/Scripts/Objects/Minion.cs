using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Minion : MonoBehaviour {

    public char letter;
    public bool beingCarried;

    public bool beingSupported;
    public List<Minion> supporting;
    public Minion supporter;

    public void StartSupporting(Minion other)
    {
        if (!this.supporting.Contains(other))
        {
            this.supporting.Add(other);
        }
        other.DisableGravity();
        other.beingSupported = true;
        other.supporter = this;
    }

    public void StopSupporting(Minion other)
    {
        this.supporting.Remove(other);
        other.supporter = null;
        other.beingSupported = false;
    }

    public void StopBeingSupported()
    {
        if (this.supporter != null)
        {
            this.supporter.StopSupporting(this);
        }
        this.beingSupported = false;
        this.EnableGravity();
        this.StopSupportingAll();
    }

    public void StopSupportingAll()
    {
        foreach (Minion m in this.supporting)
        {
            m.supporter = null;
            m.beingSupported = false;
            m.EnableGravity();
            m.StopSupportingAll();
        }
        this.supporting = new List<Minion>();
    }

	/* Detach from the hero onto the scene as the parent
	 */
	public void DetachToScene(Transform scene) 
	{
		this.transform.parent = scene;
		this.SetScale (/*reversed: */ false);
        this.beingCarried = false;
        this.StopBeingSupported();
		this.transform.position = new Vector3 (
			this.transform.position.x, this.transform.position.y,
			this.initialPosition.z
		);
	}
	
	/* Attach to the hero
	 */
	public void AttachToHero(Hero hero) 
	{
        if (this.supporter != null)
        {
            this.supporter.supporting.Remove(this);
        }
		this.beingCarried = true;
		this.transform.parent = hero.transform;
		this.DisableGravity();
	}

	/* Set the Minion's local scale based on whether supergirl is reversed.
	 * Minions should never appear reversed
	 */
	public void SetScale(bool reversed) 
	{
		this.transform.localScale = reversed ? Constants.BackwardScale : Constants.ForwardScale;
	}
	private static class Constants
	{
		public static readonly Vector3 ForwardScale = new Vector3 (1, 1, 1);
		public static readonly Vector3 BackwardScale = new Vector3 (-1, 1, 1);
        public static readonly float StackHeightFactor = 0.75f;
        public static readonly float StackWidthThreshold = 0.35f;
	}
	
	// // Private Members // //
	private Collider2D backgroundCollider; // The 2D Collider of the Background sprite
	private Vector3 initialPosition;
	private Vector3 lastRestingPosition;

    private Hero hero;
    private BackgroundClick background;
    private bool clicksEnabled;

    private float height;
    private float width;
	private Rigidbody2D rigidBody;

    /*private abstract class MinionSupport
    //
    // Represents a thing that the minion is being supported by
    // 
    {
        private Minion supported;

    }*/
    
	// // Event Handlers // //

	protected void Awake () 
	{
		this.rigidBody = GetComponent<Rigidbody2D> ();
        this.beingCarried = false;
        this.StopBeingSupported();
	}

	protected void Start () 
	{
        this.supporting = new List<Minion>();

        this.initialPosition = this.transform.position;
		this.lastRestingPosition = this.initialPosition;

		this.hero = Hero.singleton;
		this.clicksEnabled = true;

		this.background = BackgroundClick.singleton;
		this.backgroundCollider = background.GetComponent<Collider2D> ();

		LevelManager.singleton.RegisterMinion (this);

        this.height = this.GetComponent<SpriteRenderer>().bounds.size.y;
        this.width = this.GetComponent<SpriteRenderer>().bounds.size.x;
    }

	protected void OnMouseDown()
	{
		if (!this.clicksEnabled)
			return;

		if (LevelManager.singleton.showingTutorials)
			LevelManager.singleton.MinionClickedInTutorial (this);

		this.hero.PickUpMinion (this);
	}

	/* Called when the minion touches something with a 2D Collider
	 */
	protected void OnTriggerStay2D(Collider2D other) 
	{
        Minion otherM;

        if (this.beingCarried || this.beingSupported) {
			// Ignore collisions while being carried or supported
			return;
		}
		else if (other == this.backgroundCollider) {
			// We're inside the background
			return;
		}
		else if (other.GetComponentInParent<Note> () != null) {
			// Collided with a note
			return;
		}
        else if (other.GetComponentInParent<Superdog>() != null)
        {
            // Collided with superdog
            return;
        }

        else if ((otherM = other.GetComponentInParent<Minion>()) != null) {
            if (otherM.beingCarried)
            {
                // Don't collide with minions that are being carried or unsupported
                return;
            }

            else if (otherM.transform.position.y > this.transform.position.y)
            {
                // Can't really land on something above you
                return;
            }

            else if (!otherM.beingSupported)
            {
                // It can't stop you since it itself is still moving
                Vector2 averageVelocity;
                averageVelocity = (this.rigidBody.velocity + otherM.rigidBody.velocity) / 2;
                this.rigidBody.velocity = averageVelocity;
                otherM.rigidBody.velocity = averageVelocity;
                return;
            }

            else
            {
                // Collided with a minion that is being supported
                if ((Constants.StackWidthThreshold * this.width > Mathf.Abs(this.transform.position.x - otherM.transform.position.x)) &&
                    (Constants.StackHeightFactor * this.height >= Mathf.Abs(this.transform.position.y - otherM.transform.position.y)))
                {
                    this.transform.position = new Vector3(this.transform.position.x, other.transform.position.y +
                        this.height * Constants.StackHeightFactor, this.transform.position.z);
                    otherM.StartSupporting(this);
                    return;
                }
                else
                {
                    return;
                }
            }

        }

        else
            // Collided with a platform
		    this.DisableGravity();
	}

	public void EnableClicks() {
		this.clicksEnabled = true;
	}

	public void DisableClicks() {
		this.clicksEnabled = false;
	}
	
	/* Called when the minion stops touching something with a 2D Collider
	 * */
	protected void OnTriggerExit2D(Collider2D other) {
		if (!this.beingCarried && (other == this.backgroundCollider)) {
			// Left the screen
			this.ResetPositionToInitial();
		}
	}
	
	public void ResetPosition() 
	{
		this.transform.position = this.lastRestingPosition;
        this.StopBeingSupported();
        this.StopSupportingAll();
		this.EnableGravity ();
		this.rigidBody.velocity = Vector2.zero;
	}

	private void ResetPositionToInitial() {
		this.transform.position = this.initialPosition;
		this.EnableGravity ();
		this.rigidBody.velocity = Vector2.zero;
	}

	private void DisableGravity() 
	{
		this.rigidBody.isKinematic = true;
        this.rigidBody.velocity = new Vector3();
		this.lastRestingPosition = this.transform.position;
        this.beingSupported = true;
    }
	
	public void EnableGravity() 
	{
        if (!this.beingCarried)
        {
            this.rigidBody.isKinematic = false;
        }
	}
}
