using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This is a file used to represent an attack.
* 
* An attack primarily has data associated with it (damage, hitstun, etc.) and is used when an actor is attacking.
* When an acter attacks, data is pulled from said attack and used to determine if an attack hits an opponent or not.
* If so, the rest of the data is pulled and damage, knockback, and hitstun is inflicted upon the opponent.
 */


/// <summary>
/// A struct representing an attack, dealing damage, hitstun, and knockback whilst being a certain length with certain hitboxes.
/// </summary>
public struct Attack
{
	/// <summary>
	/// Creates an attack from the given data.
	/// </summary>
	/// <param name="dmg">The damage of the attack</param>
	/// <param name="hts">The hitstun of the attack</param>
	/// <param name="knbk">The knockback of the attack</param>
	/// <param name="lngth">The length of the attack, in frames</param>
	/// <param name="hblist">The list of hitboxes for this attack</param>
	Attack(double dmg, int hts, Vector2 knbk, int lngth, List<Hitbox> hblist){
		Damage = dmg;
		Hitstun = hts;
		Knockback = knbk;
		Length = lngth;
		Hitbox_List = hblist;
	}

	/// <summary>
	/// The damage that this attack deals to the opponent.
	/// For reference, Star, the player character, has a base health value of 10000.
	/// </summary>
	double Damage;

	/// <summary>
	/// The hitstun inflicted by this attack in frames.
	/// Hitstun is the effect in which the opponent cannot move after being hit for a certain amount of time.
	/// </summary>
	int Hitstun;

	/// <summary>
	/// The knockback inflicted by this attack as an offset from the opponent's base position.
	/// </summary>
	Vector2 Knockback;

	/// <summary>
	/// The length of the attack, in frames.
	/// </summary>
	int Length;

	/// <summary>
	/// A list of every hitbox inside of the attack.
	/// </summary>
	List<Hitbox> Hitbox_List;
}

/// <summary>
/// A struct representing a hitbox used during an attack.
/// A hitbox is an entity used to calculate the range of an attack, and obviously is shaped like a box.
/// </summary>
internal struct Hitbox
{
	/// <summary>
	/// The center of the hitbox as an offset from the attacker's transform.
	/// </summary>
	Vector2 Center;

	/// <summary>
	/// The size of the hitbox.
	/// </summary>
	Vector2 Size;
	
	/// <summary>
	/// The frame at which this hitbox will activate.
	/// Said hitbox will remain active until it hits another object or if a function is called explicitly flushing the hitboxes of the attacking actor.
	/// </summary>
	int Activate_Frame;
}
