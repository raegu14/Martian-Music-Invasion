using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Hero : MonoBehaviour {
    public float speed = 4f;
	public float floatingFreq = 1.65f;
	public float floatingAmp = 0.28f;
	public float turningSpeed = 0.18f;

	// There is only ever one hero, so make a reference to it
	public static Hero singleton;

    public void MoveTo(Vector3 dest)
    {
        this.commandQ.Enqueue(new MoveCommand(dest));
    }

    public void PickUpMinion(Minion minion, bool includeSupported=true)
    {
		if (this.minionsCarrying.Contains (minion)) {
			this.SetDownMinions (minion);
		} else {
			this.commandQ.Enqueue (new PickupMinionCommand (minion, this, includeSupported));
		}
    }

    public void TurnInNote(Note note)
    {
        this.commandQ.Enqueue(new TurninNoteCommand(note));
    }

    /****************************** PRIVATE ******************************/

    private abstract class HeroCommand
    {
        // <x, y> vectors representing the start and finish positions
        // for this command
		public Vector3 start; 
		public abstract Vector3 finish { get; }

		public abstract void PreComplete (float timeToCompletion, Hero hero);
		public abstract void complete (Hero hero);
		public abstract bool stillValid ();
    }

	private class MoveCommand : HeroCommand
	{
		public override Vector3 finish {
			get {
				return this.destination;
			}
		}

		private Vector3 destination;

		public MoveCommand(Vector3 destination) {
			Logger.Instance.LogAction("Hero", "Move Command", destination.ToString());
			this.destination = destination;
		}

		public override void complete (Hero hero) { 
			Logger.Instance.LogAction("Hero", "Move Complete", destination.ToString());
			hero.CompleteMove ();
		}

		public override bool stillValid () {
			return true;
		}

		public override void PreComplete (float timeToCompletion, Hero hero) { }
	}

	private class PickupMinionCommand : HeroCommand
	{
		public override Vector3 finish {
			get {
				return this.minion.transform.position - this.hero.minionStackHeight;;
			}
		}

		private Minion minion;
		private Hero hero;
        private bool includeSupported;

        public PickupMinionCommand(Minion minion, Hero hero, bool includeSupported) {
			this.minion = minion;
			this.hero = hero;
            this.includeSupported = includeSupported;
            Logger.Instance.LogAction ("Minion", "Pickup Command Issued", this.minion.name);
		}

		public override void complete (Hero hero) {
			Logger.Instance.LogAction ("Minion", "Pickup Complete", this.minion.name);
			hero.CompletePickupMinion (this.minion, includeSupported);
		}

		public override bool stillValid () {
			if (this.minion.gameObject == null) {
				Logger.Instance.LogAction ("Minion", "Pickup Fail, Destroyed", this.minion.name);
				return false;
			} else if (hero.minionsCarrying.Contains(this.minion)) {
                Logger.Instance.LogAction("Minion", "Pickup Fail, Already Holding", this.minion.name);
                return false;
			} else {
                return true;
            }
		}

		public override void PreComplete (float timeToCompletion, Hero hero) { }
	}

	private class TurninNoteCommand : HeroCommand
	{
		public override Vector3 finish {
			get {
				return this.note.position;
			}
		}

		private Note note;

		public TurninNoteCommand(Note note) {
			this.note = note;
			Logger.Instance.LogAction ("Note", "Turnin Command Issued", this.note.number);
		}

		public override void complete (Hero hero) {
			hero.CompleteTurninNote (this.note);
		}

		public override bool stillValid () {
			return this.note.gameObject != null;
		}

		public override void PreComplete (float timeToCompletion, Hero hero) { 
			if (hero.minionsMatchNote (this.note)) {
				Logger.Instance.LogAction ("Note", "Turnin Successful", this.note.number);
				LevelManager.singleton.PrePlayNote (this.note, timeToCompletion);
			} else {
				if (hero.minionsCarrying.Count != 0) {
					Logger.Instance.LogAction ("Note", "Turnin Failed", this.note.number);
					LevelManager.singleton.PreFailNote (this.note, timeToCompletion);
				} else {
					Logger.Instance.LogAction ("Note", "Clicked with No Minions", this.note.number);
				}
			}
		}
	}

    // Queue of commands to execute next
    private Queue<HeroCommand> commandQ;

    // Command currently being executed
    private HeroCommand currentCommand;

    // Whether the hero is waiting for a new command (floating) or
    // currently executing a command (flying)
    private enum HeroState
    {
        FLOATING,
        FLYING
    };
    private HeroState state;

    // During FLYING state, keep track of current and total time
    private float currentMoveTime, totalMoveTime;

    // During FLOATING state, keep track of time elapsed
    private float floatingTime;

	public float currentScale, destScale;
	private bool turning;

	// Equilibrium position. center of floating wave
	private Vector2 eqPos;

	// List of minions currently being carried
	public List<Minion> minionsCarrying;

	private LevelManager levelManager;

	private Vector2 floatingDelta {
		get {
			return new Vector2(
				0f,
				this.floatingAmp * Mathf.Sin(this.floatingTime * this.floatingFreq)
			);
		}
	}

	private Vector2 position {
		get {
			return this.eqPos + this.floatingDelta;
		}
		set {
			this.eqPos = value - this.floatingDelta;
		}
	}

	public Vector3 minionStackHeight {
		get {
			if (this.levelManager.ChordsAllowed())
				return Vector3.up * (Constants.MinionSpacing * this.minionsCarrying.Count);
			else
				return Vector3.zero;
		}
	}

	protected void Awake() {
		Hero.singleton = this;
	}

	protected void Start()
	{
		this.commandQ = new Queue<HeroCommand>();
		this.state = HeroState.FLOATING;
		this.floatingTime = 0f;
		this.eqPos = this.transform.position;
		this.currentScale = 1f;
		this.destScale = 1f;
		this.turning = false;

		this.levelManager = LevelManager.singleton;

		this.minionsCarrying = new List<Minion> ();
	}
	
	/** Calculate total flight to get from start to finish
     */
    private float FlightTime(Vector2 start, Vector2 finish)
    {
        float dist = Vector2.Distance(start, finish);
        float time = Mathf.Sqrt(dist) / this.speed;
		return Mathf.Max (time, LevelManager.audioDelay);
    }

    private void BeginCommand(HeroCommand cmd)
    {
        cmd.start = transform.position;

		this.currentScale = this.transform.localScale.x;

		if (cmd.start.x < cmd.finish.x) {
			// Moving right
			this.destScale = 1f;
		} else if (cmd.start.x > cmd.finish.x) {
			// Moving left
			this.destScale = -1f;
		}
		this.turning = (this.destScale != this.currentScale);

        this.totalMoveTime = this.FlightTime(cmd.start, cmd.finish);
        this.currentMoveTime = 0f;
        this.state = HeroState.FLYING;
        this.currentCommand = cmd;

		cmd.PreComplete (this.totalMoveTime, this);
    }

	private static class Constants
	{
		public static readonly Vector3 NoScale = new Vector3 (1, 1, 1);
		public static readonly float MinionSpacing = 0.6f;
	}
	
	private void SetDownMinions(Minion start=null) {
        bool seen = start == null;
        List<Minion> toRemove = new List<Minion>();
		foreach (Minion m in this.minionsCarrying) {
            seen |= (m == start);
            if (seen)
            {
                toRemove.Add(m);
                m.DetachToScene(this.transform.parent);
            }
		}
        foreach (Minion r in toRemove)
        {
            this.minionsCarrying.Remove(r);
        }
	}

	private void DestroyMinions() {
		foreach (Minion m in this.minionsCarrying) {
			if (this.levelManager.StillNeedsMinion(m)) {
				m.DetachToScene(this.transform.parent);
				m.ResetPosition();
				this.levelManager.DoneWithMinion(m);
			} else {
				this.levelManager.DeregisterMinion(m);
				Destroy (m.gameObject);
			}
		}
		this.minionsCarrying.Clear ();
	}

	private void PickupMinion(Minion minion) {
		minion.AttachToHero(this);

		//minion.transform.position += this.minionStackHeight;
		minion.transform.position += Vector3.back * this.minionsCarrying.Count;

        this.minionsCarrying.Add (minion);
	}

	public void CompletePickupMinion(Minion minion, bool includeSupported=true) {
		if (this.minionsCarrying.Count != 0 && !this.levelManager.ChordsAllowed ())
			this.SetDownMinions ();

        Logger.Instance.LogAction("Minion", "Pickup completed", minion.name);
        this.PickupMinion(minion);

        if (this.levelManager.ChordsAllowed() && includeSupported)
        {
            Minion s = minion;
            while (1 == s.supporting.Count)
            {
                s = s.supporting[0];
                if (s != null)
                {
                    Logger.Instance.LogAction("Minion", string.Format("Picking up {0} (on stack above) {1}", s.name, minion.name), s.name);
                    this.PickupMinion(s);
                }else
                {
                    break;
                }
            }
        }

        minion.StopSupportingAll();
    }

	private string getMinionLetters() {
		string letters = "";
		foreach (Minion m in this.minionsCarrying) {
			letters += m.letter;
		}
		return letters;
	}

	private bool minionsMatchNote(Note note) {
		if (this.minionsCarrying.Count != note.toneCount)
			return false;
		int i = 0;
		foreach (char c in note.letters) {
			if (this.minionsCarrying[i++].letter != c)
				return false;
		}
		return true;
	}

	public void CompleteTurninNote(Note note) {
		if (this.minionsMatchNote(note)) {
			// Great success! Hooray!! You're learning!
			note.Match ();
			this.DestroyMinions ();
		} else {
			if (this.minionsCarrying.Count != 0) {
				note.Fail ();
				this.SetDownMinions();
			}
		}
	}

	public void CompleteMove() {

	}

	public void Caffeinate(float speedDelta=1.2f) {
		this.speed *= speedDelta;
		this.turningSpeed *= speedDelta;
	}

    private void FinishCommand()
    {
        HeroCommand cmd = this.currentCommand;
		if (cmd.stillValid ()) {
			this.floatingTime = (cmd.finish.y > cmd.start.y) ? 0 : (Mathf.PI / this.floatingFreq);

			this.eqPos = cmd.finish;
			this.transform.position = this.eqPos;

			if (cmd.stillValid ())
				cmd.complete (this);
		}

		bool startingNewCommand = false;

        while (this.commandQ.Count > 0) {
			HeroCommand nextCommand = this.commandQ.Dequeue();
			if (nextCommand.stillValid()) {
				this.BeginCommand(nextCommand);
				startingNewCommand = true;
				break;
			}
        }
		if (!startingNewCommand) {
            this.state = HeroState.FLOATING;
            this.currentCommand = null;
        }
    }

	private void UpdateScale() 
	{
		if (!this.turning)
			return;

		if (this.currentScale > this.destScale) {
			this.currentScale -= this.turningSpeed;
			if (this.currentScale <= this.destScale) {
				this.currentScale = this.destScale;
				this.turning = false;
			}
		} else if (this.currentScale < this.destScale) {
			this.currentScale += this.turningSpeed;
			if (this.currentScale >= this.destScale) {
				this.currentScale = this.destScale;
				this.turning = false;
			}
		} else {
			this.turning = false;
		}

		Vector3 scale = new Vector3 (this.currentScale, 1, 1);
		this.transform.localScale = scale;

		foreach (Minion m in this.minionsCarrying) {
			bool reversed = this.currentScale < 0;
			m.SetScale(reversed);
		}
	}

    private void SetFloatingTransform()
    {
		this.transform.position = this.position;
		this.UpdateScale ();
    }

    private void SetFlyingTransform()
    {
        HeroCommand cmd = this.currentCommand;

        // What portion of the way are we through our flight command?
        float t = (this.totalMoveTime == 0f) ? 1f : this.currentMoveTime / this.totalMoveTime;

        // "Smootherstep"
        // https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
        t = t * t * t * (t * (6f * t - 15f) + 10f);

        this.transform.position = Vector2.Lerp(cmd.start, cmd.finish, t);
		this.UpdateScale ();
    }

    protected void FixedUpdate () {
        if (this.state == HeroState.FLOATING)
        {
            this.floatingTime = (this.floatingTime + Time.fixedDeltaTime) % (2f * Mathf.PI / this.floatingFreq);
			if (this.commandQ.Count > 0)
            	this.BeginCommand(this.commandQ.Dequeue());
        } else if (this.state == HeroState.FLYING)
        {
            this.currentMoveTime += Time.fixedDeltaTime;
            if (this.currentMoveTime >= this.totalMoveTime)
            {
                this.FinishCommand();
            }
        }

        if (this.state == HeroState.FLOATING)
        {
            this.SetFloatingTransform();
        } else if (this.state == HeroState.FLYING)
        {
            this.SetFlyingTransform();
        }
	}
}
