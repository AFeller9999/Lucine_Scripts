using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static Player_Controller;

///     Handles input from the player
public class Input_Player : MonoBehaviour
{

	/// <summary>
	/// An enum used to determine what inputs are allowed by the player.
	/// <para>
	/// General: All inputs are allowed by the player
	/// </para>
	/// <para>
	/// MovementOnly: Only movement inputs are allowed by the player
	/// </para>
	/// <para>
	/// None: No inputs are allowed by the player
	/// </para>
	/// </summary>
	public enum ControlState{
		General,
		MovementOnly,
		None
	}

	private Player_Controller playerController;

	internal ControlState CONTROL_STATE;

	internal Vector2 MoveInput { get; private set; }

	internal float JumpInput { get; private set; }

	internal float AttackInput { get; private set; }

	/// <summary>
	/// The ammount of time, in miliseconds, the player has after an input to perform 
	/// another input for it to be considered part of the same input chain.
	/// </summary>
	const int INPUT_BUFFER_TIME = 200;

	/// <summary>
	/// The number of extra inputs that the player can make for a proper input to be read
	/// from the input chain.
	/// </summary>
	const int INPUT_BUFFER_LENGTH = 2;

	private Timer inputTimer = new Timer(INPUT_BUFFER_TIME);

	private string InputChain = "";

	private int CHAIN_NUMBER = 0;



	// Start is called before the first frame update
	void Awake()
	{
		inputTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
		{
			//Debug.Log($"Input Chain #{++CHAIN_NUMBER}: {InputChain}");
			InputChain = "";
		};
		inputTimer.AutoReset = false;
		playerController = GetComponent<Player_Controller>();
	}

	public void OnMove(InputValue input)
	{
		if (this.CONTROL_STATE == ControlState.None){
			return;
		}

		MoveInput = input.Get<Vector2>();

		char finalInput = '5';

		if (MoveInput.x * playerController.lookDirection > 0)
		{
			if (MoveInput.y == 0)
			{
				finalInput = '6';
				if (InputChain.Length < 1 + INPUT_BUFFER_LENGTH)
				{
					if (InputChain.Contains("6")) playerController.AirDash();
				}
				else
				{
					if (InputChain.Substring(InputChain.Length - (1 + INPUT_BUFFER_LENGTH)).Contains("6")) playerController.AirDash();
				}
			}
			else if (MoveInput.y > 0)
			{
				finalInput = '9';
			}
			else
			{
				finalInput = '3';
			}
		}
		else if (MoveInput.x * playerController.lookDirection < 0)
		{
			if (MoveInput.y == 0)
			{
				finalInput = '4';
			}
			else if (MoveInput.y > 0)
			{
				finalInput = '7';
			}
			else
			{
				finalInput = '1';

			}
		}
		else
		{
			if (MoveInput.y == 0)
			{
				finalInput = '5';
			}
			else if (MoveInput.y > 0)
			{
				finalInput = '8';
			}
			else
			{
				finalInput = '2';
			}
		}

		if (MoveInput.y < 0 && playerController.characterController.isGrounded)
		{
			this.playerController.PlayerState = State.Crouching;
		}

		AddInputChain(finalInput);

	}

	public void OnJump(InputValue input)
	{
		if (this.CONTROL_STATE != ControlState.General)
		{
			return;
		}

		if (MoveInput.y > 0)
		{
			this.playerController.HighJump();
		} else if (MoveInput.y < 0){
			this.playerController.Roll();
		}
		else
		{
			this.playerController.Jump(input.isPressed);
		}
		if (input.isPressed)
		{
			AddInputChain('J');
		}
	}

	public void OnAttack(InputValue input)
	{
		if (this.CONTROL_STATE != ControlState.General)
		{
			return;
		}

		AddInputChain('A');
	}

	private void AddInputChain(char input)
	{
		InputChain += input;
		inputTimer.Stop();
		inputTimer.Start();
	}



}
