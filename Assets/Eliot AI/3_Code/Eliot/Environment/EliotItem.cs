#pragma warning disable CS0414, CS0649
using System;
using System.Collections.Generic;
using Eliot.AgentComponents;
using Eliot.BehaviourEditor;
using Eliot.Utility;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Eliot.Environment
{
	/// <summary>
	/// Represents an item that an Agent can carry in his Inventory.
	/// Items can hold Skills and some Agent's characteristics so that
	/// they can be changed when Agent uses the item. Skills let Items
	/// to be used as potions, and other options let Items to change Agent's
	/// Behaviour model, his graphics components etc.
	/// </summary>
	[RequireComponent(typeof(Unit))]
	public class EliotItem : MonoBehaviour
	{
		/// <summary>
		/// User-defined type of the item. May vary depending on the context of the game.
		/// </summary>
		public ItemType Type
		{
			get { return _itemType; }
		}

		/// <summary>
		/// Measurement of how much the item is valuable.
		/// </summary>
		public int Value
		{
			get { return _value; }
		}

		/// <summary>
		/// How much weight or space does it take inside the Inventory.
		/// </summary>
		public float Weight
		{
			get { return _weight * _amount; }
		}

		/// <summary>
		/// Return the Skill assigned to the Item.
		/// </summary>
		public Skill Skill
		{
			get { return _skill; }
		}

		[Tooltip("Measurement of how much the item is valuable.")]
		[SerializeField] private int _value;
		[Tooltip("How much weight or space does it take inside the Inventory.")]
		[SerializeField] private float _weight;
		[Tooltip("The number of times this Item occurs in Inventory.")]
		[SerializeField] private int _amount = 1;
		/// User-defined type of the item. May vary depending on the context of the game.
		[SerializeField] private ItemType _itemType;
		[Tooltip("The Skill that will be applied to an Agent when he uses the Item.")]
		[SerializeField] private Skill _skill;
		
		[Header("On Wield")]
		[Tooltip("New Behaviour model (if there is one) will be set for Agent when he wields the Item.")]
		[SerializeField] private EliotBehaviour _newBehaviour;
		[Tooltip("New skills will be added to Agent's didposal when he wields the item.")]
		[SerializeField] private Skill[] _addSkills;
		[Tooltip("New graphics (if there is one) will be set for Agent when he wields the Item.")]
		[SerializeField] private GameObject _newGraphics;
		
		[Space]
		[Tooltip("Sound of wielding the item.")]
		[SerializeField] private List<AudioClip> _wieldSounds = new List<AudioClip>();
		[Tooltip("Sound of unwielding the item.")]
		[SerializeField] private List<AudioClip> _unwieldSounds = new List<AudioClip>();
		[Tooltip("Sound of using the item as a potion.")]
		[SerializeField] private List<AudioClip> _useSounds = new List<AudioClip>();
		
		public UnityEvent onWield;
		public UnityEvent onUnwield;
		public UnityEvent onPickUp;
		public UnityEvent onDrop;
		public UnityEvent onUse;

		/// Whether the Item is currently in an Inventory.
		private bool _isInInventory;
		/// MeshRenderer component of the Item's gameObject;
		private MeshRenderer _renderer;
		/// Collider component of the Item's gameObject;
		private Collider _collider;
		/// Inventory that the Item is currently in. None if the Item is outside the Inventory.
		private AgentInventory _currentAgentInventory;
		
		/// <summary>
		/// Use this for initialization.
		/// </summary>
		private void Start()
		{
			_renderer = GetComponent<MeshRenderer>();
			_collider = GetComponent<Collider>();
		}

		/// <summary>
		/// Update is called once per frame.
		/// </summary>
		private void Update()
		{
			
		}

		/// <summary>
		/// Put Item into cpecified Agent's Inventory.
		/// </summary>
		/// <param name="agentInventory"></param>
		public void AddToInventory(AgentInventory agentInventory)
		{
			_isInInventory = true;
			_currentAgentInventory = agentInventory;
			agentInventory.AddItem(GetComponent<EliotItem>());
			gameObject.SetActive(false);
			transform.parent = agentInventory.ItemsContainer;
			transform.localPosition = Vector3.zero;
			if(onPickUp != null) onPickUp.Invoke();
		}

		/// <summary>
		/// Drop Item from Inventory if it currently is in one.
		/// </summary>
		public void GetDropped(float dropRadius)
		{
			if (!_isInInventory) return;
			gameObject.SetActive(true);
			_isInInventory = false;
			_currentAgentInventory = null;
			transform.parent = null;
			var offset = Random.insideUnitCircle * dropRadius;
			transform.position += new Vector3(offset.x, 0, offset.y);
			if(onDrop != null) onDrop.Invoke();
		}

		/// <summary>
		/// Apply the skill held by this Item to an Agent.
		/// </summary>
		/// <param name="agent"></param>
		public void Use(EliotAgent agent)
		{
			if (!_skill) return;
			agent.AddEffect(_skill, agent);
			_amount--;
			if (_useSounds != null && _useSounds.Count > 0)
			{
				agent.GetAudioSource().clip = _useSounds[Random.Range(0, _useSounds.Count)];
				agent.GetAudioSource().Play();
			}
			if (_amount > 0) return;
			GetDropped(agent.GetAgentComponent<Eliot.AgentComponents.AgentInventory>().DropRadius);
			Destroy(gameObject);
			if(onUse != null) onUse.Invoke();
		}

		/// <summary>
		/// Make an Agent wield the Item. May lead to changes in Agent's cofiguration.
		/// </summary>
		/// <param name="agent"></param>
		public void Wield(EliotAgent agent)
		{
			if(_currentAgentInventory.WieldedItem)
				_currentAgentInventory.WieldedItem.Unwield(agent);
			
			_currentAgentInventory.WieldedItem = this;
			if(_addSkills.Length > 0)
				foreach (var skill in _addSkills)
					agent.AddSkill(skill);
			if(_newBehaviour) agent.SetBehaviour(_newBehaviour);
			if(_newGraphics) agent.ReplaceGraphics(_newGraphics);
			
			if (_wieldSounds != null && _wieldSounds.Count > 0)
			{
				agent.GetAudioSource().clip = _wieldSounds[Random.Range(0, _wieldSounds.Count)];
				agent.GetAudioSource().Play();
			}
			if(onWield != null) onWield.Invoke();
		}

		/// <summary>
		/// Make Agent unwield the Item. Returns Agent's configuration to default.
		/// </summary>
		/// <param name="agent"></param>
		public void Unwield(EliotAgent agent)
		{
			if(_addSkills.Length > 0)
				foreach (var skill in _addSkills)
				{
					try
					{
						agent.RemoveSkill(skill);
					}
					catch (Exception)
					{
					}
				}

			if (_unwieldSounds != null && _unwieldSounds.Count > 0)
			{
				agent.GetAudioSource().clip = _unwieldSounds[Random.Range(0, _unwieldSounds.Count)];
				agent.GetAudioSource().Play();
			}
			
			agent.ResetGraphics();
			if(onUnwield != null) onUnwield.Invoke();
		}
	}
}