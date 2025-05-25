#pragma warning disable CS0414, CS0649, CS1692
using System;
using System.Collections.Generic;
using System.Linq;
using Eliot.Environment;
using UnityEngine;

namespace Eliot.AgentComponents
{
	/// <summary>
	///  Сache for Agents' representation of the world.
	/// </summary>
	[Serializable]
	public class AgentMemory
	{
		/// <summary>
		///   <para>Amount of units that can be remembered at the same time.</para>
		/// </summary>
		public int Capacity
		{
			get { return _capacity; }
			set { _capacity = value; }
		}
		
		/// <summary>
		///   <para>Time for which an unapdated unit will stay in memory.</para>
		/// </summary>
		public int Duration
		{
			get { return _duration; }
			set { _duration = value; }
		}
		
		/// <summary>
		///   <para>Memory container.</para>
		/// </summary>
		public Queue<Unit> Units
		{
			get { return _units; }
			set { _units = value; }
		}
		
		[Tooltip("How many entities can it remember silmutaniously")]
		[SerializeField] private int _capacity = 1;
		[Tooltip("How long does agent remember each unit")]
		[SerializeField] private int _duration = 30;
		/// Memory container.
		[SerializeField] private Queue<Unit> _units = new Queue<Unit>();
		[Tooltip("If true, Agent will remember the position in which he " +
		         "had last leen the target. Otherwise, he will remember " +
		         "the actual reference to the target.")]
		[SerializeField] private bool _rememberPositionOfTarget;
		
		/// Memory container for positions.
		private Dictionary<Unit, Vector3> _positions = new Dictionary<Unit, Vector3>();
		/// The last time Forget was updated.
		private float _lastTimeForgot;
		/// Actual link to the controller.
		private EliotAgent _agent;

		/// <summary>
		/// Initialize memory.
		/// </summary>
		/// <param name="agent"></param>
		public void Init(EliotAgent agent)
		{
			_agent = agent;
		}

		/// <summary>
		/// <para>Create new Memory.</para>
		/// </summary>
		public static AgentMemory CreateMemory(int duration, int capacity = 1)
		{
			return new AgentMemory
			{
				_duration = duration,
				_capacity = capacity
			};
		}

		/// <summary>
		/// Make sure Agent's Memory target is never lost.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public Unit GetDefaultTarget(Vector3 pos)
		{
			return _agent.GetDefaultTarget(pos).GetComponent<Unit>();
		}

		/// <summary>
		/// Remember a seen unit. Must be called intentionally by user.
		/// </summary>
		/// <param name="unit"></param>
		public void Memorise(Unit unit)
		{
			if (_units.Count == _capacity) _units.Dequeue();
			if (!_units.Contains(unit)) _units.Enqueue(unit);
			if (_rememberPositionOfTarget)
			{
				if (!_positions.ContainsKey(unit))
					_positions.Add(unit, unit.transform.position);
				else _positions[unit] = unit.transform.position;
			}
		}

		/// <summary>
		///   <para>Forget a unit when it has not been updated for certain time.</para>
		/// </summary>
		private void Forget()
		{
			var poper = _units.Count > 0 ?_units.Dequeue() : null;
			if(poper != null)_positions.Remove(poper);
		}

		/// <summary>
		/// Forget specified Unit.
		/// </summary>
		/// <param name="unit"></param>
		public void Forget(Unit unit)
		{
			var list = _units.ToArray().ToList();
			list.Remove(unit);
			_units = new Queue<Unit>();
			foreach (var entry in list) _units.Enqueue(entry);
			_positions.Remove(unit);
		}

		/// <summary>
		/// Check if agent remembers an enemy.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public Unit RememberUnit(UnitQuery query)
		{
			foreach (var unit in _units)
				if (query(unit))
					return _rememberPositionOfTarget ? GetDefaultTarget(_positions[unit]) : unit;
			return null;
		}

		/// <summary>
		/// Return all remembered Entities that satisfy the query.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public List<Unit> RememberUnits(UnitQuery query)
		{
			var result = new List<Unit>();
			foreach (var unit in _units)
				if (query(unit))
					result.Add(_rememberPositionOfTarget ? GetDefaultTarget(_positions[unit]) : unit);
			return result;
		}

		/// <summary>
		///   <para>Forget a unit when it has not been updated for certain time.</para>
		/// </summary>
		public void Update()
		{
			if (Time.time >= _lastTimeForgot + _duration)
			{
				Forget();
				_lastTimeForgot = Time.time;
			}
		}
	}
}