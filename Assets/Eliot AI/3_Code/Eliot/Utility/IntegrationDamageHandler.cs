using UnityEngine;

namespace Eliot.Utility
{
	/// <summary>
	/// Handles passing the damage info from Eliot Skills to an arbitrary third-party asset.
	/// </summary>
	public static class IntegrationDamageHandler
	{
		/// <summary>
		/// Check if a target has necessary component and pass the damage info.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="power"></param>
		/// <param name="attacker"></param>
		/// <param name="pushPower"></param>
		public static void PassDamage(GameObject target, float power, GameObject attacker, float pushPower)
		{

		}
	}
}