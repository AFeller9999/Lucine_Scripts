using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using static Input_Player;

public class Player_Controller : MonoBehaviour
{

	/// <summary>
	/// The current state of the player.
	/// </summary>
	public enum State
	{
		Running,
		Jumping,
		Falling,
		Rolling,
		Attacking,
		Hitstun,
		Idle,
		Crouching,
		RollRecovery
	};

	// The current desired move direction of the player.
	internal Vector2 moveDirection = new Vector2();

	// The CharacterController associated with this game object.
	internal CharacterController characterController;

	// The SpriteRenderer associated with this game object.
	internal SpriteRenderer spriteRenderer;

	// The Input_Player object that will handle and process inputs.
	internal Input_Player inputHandeler;

	// The current state of the player
	[HideInInspector]
	public State PlayerState;

	// The Animator that animates the player
	private Animator animator;

	// Gravity represented as units-per-frame-squared
	public float gravity;

	// Player speed, represented as units-per-frame
	public float speed;

	// Jump power
	public float jumpPower;

	// Super-jump power
	public float highJumpPower;

	// The speed at which the player can air-dash
	public float airDashSpeed;

	// The speed at which the player rolls
	public float rollSpeed;

	// The length of a roll
	public float rollTime;

	// Whether or not the player has input control
	[HideInInspector]
	internal bool canInput;

	// Whether or not the player has hit their head when jumping.
	private bool bonkedHead = false;

	// Whether or not the player has air-dashed during this jump.
	private bool HasAirdashed = false;

	// The player's current velocity
	private float velocity = 0;

	// The smoothing time of the player's run
	public float smoothTime;

	// The look direction of the player, 1 being right and -1 being left.
	[HideInInspector]
	public int lookDirection = 1;


	// The current health of the player
	public float health = 5000f;

	// The maximum health of the player
	public float maxHealth = 5000f;

	// Whether or not the player is currently invincible
	[HideInInspector]
	public bool invincible = false;

	// The terminal velocity of the player, being Star's maximum downward air speed
	const float TERMINAL_VELOCITY = -.28f;

	void Awake()
	{
		inputHandeler = GetComponent<Input_Player>();
		characterController = GetComponent<CharacterController>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		//animator = GetComponent<Animator>();
		gravity *= 0.0026f;
		speed *= .12f;
		airDashSpeed *= .12f;
		jumpPower *= .22f;
		highJumpPower *= .22f;
		rollSpeed *= .12f;
		PlayerState = State.Idle; // The player defaults to an idle state.
		canInput = true; // By default we can control the character
	}

	void Update()
	{
		// Basic player input and movement

		if (this.canInput)
		{
			MoveCharacter(this.inputHandeler.MoveInput.x);
		}

		// Player state shenanigans, as well as situations for landing.


		if (this.characterController.isGrounded == true)
		{
			HasAirdashed = false;
			if (Mathf.Abs(moveDirection.x) > 0.01) this.PlayerState = State.Running; else this.PlayerState = State.Idle;
		}


		if (moveDirection.y < -0.01 && this.characterController.isGrounded == false) this.PlayerState = State.Falling;
		else if (moveDirection.y > 0.01 && this.characterController.isGrounded == false) this.PlayerState = State.Jumping;

		// Gravity

		if (this.characterController.isGrounded == false)
		{
			moveDirection.y -= gravity;
			if (moveDirection.y < TERMINAL_VELOCITY) moveDirection.y = TERMINAL_VELOCITY;
		}

		// Actual movement
		characterController.Move(moveDirection);
	}

	#region Movement options

	/// <summary>
	/// Moves the character by a specific value, with smooth times and movement speed indicated.
	/// </summary>
	/// <param name="x"> The X speed of the movement.</param>
	void MoveCharacter(float x)
	{
		float newX = Mathf.SmoothDamp(moveDirection.x, x * (speed), ref velocity, smoothTime);
		moveDirection = new Vector2(newX, moveDirection.y);
		if (x > 0)
		{
			lookDirection = 1;
			spriteRenderer.flipX = false;
		}
		else if (x < 0)
		{
			lookDirection = -1;
			spriteRenderer.flipX = true;
		}

		animator?.SetInteger("RunState", (Mathf.Abs(x - 0.001f) < 0.002f) ? 0 : 1); // Sets the run animation based on speed

	}

	// Makes the player jump.
	internal void Jump(bool button)
	{
		if (characterController.isGrounded && button)
		{
			PlayerState = State.Jumping;
			animator?.SetInteger("ArialState", 1);
			moveDirection.y = jumpPower;
		}
		else if (!characterController.isGrounded && !button && moveDirection.y > 0)
		{
			PlayerState = State.Falling;
			animator?.SetInteger("ArialState", 1);
			moveDirection.y *= 0.2f;
		}
	}

	// Makes the player high-jump.
	internal void HighJump()
	{
		if (characterController.isGrounded)
		{
			Debug.Log("high jump");
			PlayerState = State.Jumping;
			animator?.SetInteger("ArialState", 1); // 3
			moveDirection.y = highJumpPower;
		}
	}

	// Makes the player air-dash
	internal void AirDash(){
		if (HasAirdashed == false && characterController.isGrounded == false){
			HasAirdashed = true;
			moveDirection.x = airDashSpeed * lookDirection;
			moveDirection.y = jumpPower*0.2f;
			// Animation stuff
		}
	}

	// Makes the player roll
	internal void Roll(){
		if (PlayerState == State.Idle || PlayerState == State.Running){
			PlayerState = State.Rolling;
			inputHandeler.CONTROL_STATE = ControlState.None;
			Timer RollTimer = new Timer(rollTime*1000);
			RollTimer.AutoReset = false;
			RollTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
			{
				RollEnd();
			};
			RollTimer.Start();
			MoveCharacter(rollSpeed*lookDirection);
			moveDirection.y = jumpPower*0.1f;
			invincible = true;
			// Animation stuff and make sure to modify the speed to match the roll time
		}
	}

	// Called when a roll ends. Should not be called at any other point.
	private void RollEnd(){ 
		inputHandeler.CONTROL_STATE = ControlState.General;
		invincible = false;
	}

	#endregion

	// Makes the player sustain the given knockback with the given angle (from the +X axis) and magnitude
	internal void sustainKnockback(float magnitude, float angle)
	{
		// Shenanigans
	}

	// Makes the player sustain the given hitstun for the given number of frames, assuming the game is running at 60 frames per second.
	internal void sustainHitstun(int frames)
	{
		// Also shenanigans
	}

	// Called when the hitstun duration is over. Should not be called at any other point.
	private void hitsunEnd(){

	}
}
