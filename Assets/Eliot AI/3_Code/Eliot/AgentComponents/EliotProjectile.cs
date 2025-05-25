#pragma warning disable CS0414, CS0649, CS1692
using System;
using Eliot.Environment;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Physical keeper of the Spell casted by an Agent. Moves forward in
	/// specified direction after being cast untill it collides with another
	/// physical object. Perfectly fits for fireballs, bullets etc.
	/// </summary>
	[RequireComponent(typeof(Unit))]
	public class EliotProjectile : MonoBehaviour
	{
		[SerializeField] private bool _canAffectEnemies = true;
		[SerializeField] private bool _canAffectFriends = true;
		[SerializeField] private int _minDamage;
		[SerializeField] private int _maxDamage;
		[SerializeField] private float _speed = 1f;
		[SerializeField] private float _lifeTime = 5f;
		[SerializeField] private bool _chaseTarget;
		[SerializeField] private float _rotationSpeed = 5f;
		[SerializeField] private bool _detachChildren;
		[SerializeField] private bool _destroyOnAnyCollision;
		[Space]
		[Tooltip("If true, a method with specified name will be invoked on an object" +
		         " that projectile colides with. Random number between MinDamage" +
		         " and MaxDamage will be passed as a parameter.")]
		[SerializeField] private bool _sendDamageMessage = true;
		[SerializeField] private string _damageMethodName = "Damage";
		[Space]
		[Tooltip("Other methods to be invoked without parameters on a collision object.")]
		[SerializeField] private string[] _messages;
		[Space]
		[Tooltip("Skill that will be applied to an Agent the Projectile collides with.")]
		[SerializeField] private Skill _skill;
		/// The caster of the Projectile.
		private EliotAgent _owner;
		/// The target of the Projectile.
		private Transform _target;
		/// Time at which the Projectile was cast.
		private float _initTime;

		/// <summary>
		/// Initialise Projectile's components.
		/// </summary>
		/// <param name="agent"></param>
		/// <param name="target"></param>
		/// <param name="skill"></param>
		/// <param name="minDamage"></param>
		/// <param name="maxDamage"></param>
		/// <param name="canAffectEnemies"></param>
		/// <param name="canAffectFriends"></param>
		public void Init(EliotAgent agent, Transform target, Skill skill, int minDamage = 0, int maxDamage = 5, 
			bool canAffectEnemies = true, bool canAffectFriends = true)
		{
			_canAffectEnemies = canAffectEnemies;
			_canAffectFriends = canAffectFriends;
			_owner = agent;
			_target = target;
			_skill = skill.Clone();
			_skill.Init(_owner, _owner.gameObject);
			_initTime = Time.time;
			_minDamage = minDamage;
			_maxDamage = maxDamage;
		}
		
		/// <summary>
		/// Update object every frame.
		/// </summary>
		private void Update()
		{
			if (_chaseTarget && _target)
			{
				var targetPos = _target.position;
				if (_target.GetComponent<Unit>() && _target.GetComponent<Unit>().Type == UnitType.Agent)
					targetPos.y += _target.localScale.y / 2;
				var targetDir = targetPos - transform.position;
				var step = _rotationSpeed * Time.deltaTime;
				var newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
				transform.rotation = Quaternion.LookRotation(newDir);
			}
			transform.position += transform.forward * _speed;
			if(Time.time >= _initTime + _lifeTime) Destroy(gameObject);
		}

		/// <summary>
		/// Pass all needed information to the object on collision.
		/// </summary>
		/// <param name="other"></param>
		private void OnTriggerEnter(Collider other)
		{
			var agent = other.gameObject.GetComponent<EliotAgent>();
			
			var damage = Random.Range(_minDamage, _maxDamage + 1);
			if (!agent && _sendDamageMessage) other.gameObject.SendMessage(
				_damageMethodName, damage, SendMessageOptions.DontRequireReceiver);
			foreach (var message in _messages)
				other.gameObject.SendMessage(message, SendMessageOptions.DontRequireReceiver);

			if (agent && !agent.Equals(_owner))
			{
				try
				{
					var targetIsFriend = _owner.Unit.IsFriend(agent.Unit);
					if (targetIsFriend && !_canAffectFriends) return;
					if (!targetIsFriend && !_canAffectEnemies) return;

					if (_skill != null) agent.AddEffect(_skill, _owner);
					else agent.Damage(damage);

					if (_detachChildren)
					{
						foreach (Transform child in transform)
							child.parent = null;
					}
				}
				catch (Exception)
				{
				}

				Destroy(gameObject);
			}

			if(!agent && (other.gameObject.GetComponent<Unit>() && (other.gameObject.GetComponent<Unit>().Type != UnitType.Projectile)))
				Destroy(gameObject);
			
			if(_destroyOnAnyCollision)
				Destroy(gameObject);
		}
	}
}