#pragma warning disable CS0414, CS0649
using System;
using System.Collections.Generic;
using Eliot.Environment;
using Eliot.Utility;
using UnityEngine;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Encapsulates behaviour related to interaction with Items (Keep, use, wield, throw away, etc.).
	/// </summary>
	[Serializable]
	public class AgentInventory : AgentComponent
	{
		/// <summary>
		/// Child of Agent's Transform that is a parent to all Items in the Agent's Inventory.
		/// </summary>
		public Transform ItemsContainer
		{
			get { return _itemsContainer; }
			set { _itemsContainer = value; }
		}

		/// <summary>
		/// Item that is currently being wielded.
		/// </summary>
		public EliotItem WieldedItem
		{
			get { return _wieldedItem; }
			set { _wieldedItem = value; }
		}

		public float DropRadius
		{
			get { return _dropRadius; }
			set { _dropRadius = value; }
		}

		/// <summary>
		/// List of all items in the Inventory.
		/// </summary>
		public List<EliotItem> Items
		{
			get { return _items; }
			set { _items = value; }
		}

		/// <summary>
		/// Maximum weight that the inventory can handle.
		/// </summary>
		public float MaxWeight
		{
			get { return _maxWeight; }
			set { _maxWeight = value; }
		}

		/// <summary>
		/// If true, all objects from list of Items will be put into Inventory even if they are outside at the moment.
		/// </summary>
		public bool InitFromList
		{
			get { return _initFromList; }
			set { _initFromList = value; }
		}

		/// <summary>
		/// If true, all children Items of the ItemsContainer will be set up appropriately.
		/// </summary>
		public bool InitFromChildren
		{
			get { return _initFromChildren; }
			set { _initFromChildren = value; }
		}

		/// Maximum weight that the inventory can handle.
		[SerializeField] private float _maxWeight = 1f;
		/// List of all items in the Inventory.
		[SerializeField] private List<EliotItem> _items = new List<EliotItem>();
		/// Item that is currently being wielded.
		[SerializeField] private EliotItem _wieldedItem;
		[Space]
		[Tooltip("If true, all objects from list of Items will be put" +
		         " into Inventory even if they are outside at the moment.")]
		[SerializeField] private bool _initFromList;
		[Tooltip("If true, all children Items of the ItemsContainer " +
		         "will be set up appropriately.")]
		[SerializeField] private bool _initFromChildren;
		/// Maximum distance from an Agent in which an Item will be dropped.
		[Space][SerializeField] private float _dropRadius = 1f;
		
		/// Child of Agent's Transform that is a parent to all Items in the Agent's Inventory.
		private Transform _itemsContainer;

		/// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="agent"></param>
        public AgentInventory(EliotAgent agent) : base(agent)
		{
		}

		/// <summary>
		/// Initialize Inventory components.
		/// </summary>
		/// <param name="agent"></param>
		public override void Init(EliotAgent agent)
		{
			_agent = agent;
			_itemsContainer = agent.FindTransformByName("__inventory__");

			if(_initFromList) InitItemsFromList();
			if (_initFromChildren) InitItemsFromChildren();
		}

		/// <summary>
		/// Put Items from the Items list in inventory.
		/// </summary>
		private void InitItemsFromList()
		{
			if (_items.Count <= 0) return;
			foreach (var item in _items)
				item.AddToInventory(this);
		}
		
		/// <summary>
		/// Make sure children Items of container are sut up properly. 
		/// </summary>
		private void InitItemsFromChildren()
		{
			if (_itemsContainer.childCount <= 0) return;
			foreach (Transform item in _itemsContainer)
				if(item.GetComponent<EliotItem>())
					item.GetComponent<EliotItem>().AddToInventory(this);
		}

        /// <summary>
        /// Calculate weight of all items in the Inventory.
        /// </summary>
        /// <returns>Returns weight of all items in the Inventory.</returns>
        public float CurrentWeight()
		{
			var res = 0f;
			foreach (var item in _items)
				res += item.Weight;
			return res;
		}

		/// <summary>
		/// Add new Item to the Inventory.
		/// </summary>
		/// <param name="eliotItem"></param>
		public void AddItem(EliotItem eliotItem)
		{
			if (!_items.Contains(eliotItem))
			{
				if (CurrentWeight() + eliotItem.Weight > _maxWeight)
					DropWorstItem();
				_items.Add(eliotItem);
			}
		}

		/// <summary>
		/// Find Item with the least value.
		/// </summary>
		/// <returns>Returns Item with the smallest value.</returns>
		private EliotItem WorstItem()
		{
			var minValue = int.MaxValue;
			var index = 0;
			for (var i = 0; i < _items.Count; i++)
				if (_items[i].Value < minValue)
				{
					minValue = _items[i].Value;
					index = i;
				}

			return _items[index];
		}

		/// <summary>
		/// Find the best Item of type Weapon.
		/// </summary>
		/// <returns>Returns Item of type Weapon with the biggest value.</returns>
		public EliotItem BestWeapon()
		{
			var maxValue = 0;
			var index = 0;
			for(var i = 0; i < _items.Count; i++)
			{
				if (_items[i].Type == ItemType.Weapon && _items[i].Value > maxValue)
				{
					maxValue = _items[i].Value;
					index = i;
				}
			}

			return _items[index];
		}
		
		/// <summary>
		/// Collect the Items in the 'CloseRange' radius (or default radius to 3).
		/// </summary>
		public void PickItems()
		{
			var allUnits = Agent.GetAgentComponent<Eliot.AgentComponents.AgentPerception>().SeenUnits;
			foreach (var unit in Agent.GetAgentComponent<Eliot.AgentComponents.AgentPerception>().AgentMemory.Units)
				allUnits.Add(unit);
			foreach (var item in allUnits)
			{
				if (item.GetComponent<EliotItem>()
				    && Vector3.Distance(item.transform.position, Agent.transform.position)
				    <= Agent["CloseRange", 3f])
				{
					item.GetComponent<EliotItem>().AddToInventory(GetComponent<AgentInventory>());
					var perception = Agent.GetAgentComponent<Eliot.AgentComponents.AgentPerception>();
					if (!perception) continue;
					var memory = perception.AgentMemory;
					if(memory != null)
						Agent.GetAgentComponent<Eliot.AgentComponents.AgentPerception>().AgentMemory.Forget(item);
				}
			}
		}

		/// <summary>
		/// Drop Item out of Inventory and unwield it if necessary.
		/// </summary>
		/// <param name="eliotItem"></param>
		public void DropItem(EliotItem eliotItem)
		{
			_items.Remove(eliotItem);
			eliotItem.GetDropped(_dropRadius);
			if(WieldedItem == eliotItem)
				eliotItem.Unwield(_agent);
		}
		
		/// <summary>
		/// Find the worst Item and drop it.
		/// </summary>
		public void DropWorstItem()
		{
			if (_items.Count == 0) return;
			DropItem(WorstItem());
		}

		/// <summary>
		/// Check if an item is better than the currently wielded one.
		/// </summary>
		/// <param name="eliotItem"></param>
		/// <returns></returns>
		public bool ItemIsBetterThanCurrent(EliotItem eliotItem)
		{
			if (!_wieldedItem) return true;
			return eliotItem.Value > _wieldedItem.Value;
		}

		/// <summary>
		/// Drop all Items from Inventory.
		/// </summary>
		public void DropAllItems()
		{
			if (_items.Count <= 0) return;
			for (var i = _items.Count - 1; i >= 0; i--)
			{
				_items[i].GetDropped(_dropRadius);
				if(WieldedItem == _items[i])
					_items[i].Unwield(_agent);
				_items.RemoveAt(i);
			}
		}

		/// <summary>
		/// Return true if Agent has a better item of type ItemType in the inventory.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool HaveBetterItem(ItemType type)
		{
			foreach (var item in Items)
				if (item.Type == type && ItemIsBetterThanCurrent(item)) 
					return true;

			return false;
		}
	}
}