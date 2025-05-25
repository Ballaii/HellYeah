using Eliot.Environment;
using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// The Standard Library of inventory related actions.
    /// </summary>
    public class StandardInventoryActionInterface : InventoryActionInterface
    {
        public StandardInventoryActionInterface(EliotAgent agent) : base(agent) { }

        /// <summary>
        /// Pick all the items that are close to the Agent.
        /// </summary>
        [IncludeInBehaviour]
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
                    item.GetComponent<EliotItem>().AddToInventory(AgentInventory);
                    Agent.GetAgentComponent<Eliot.AgentComponents.AgentPerception>().AgentMemory.Forget(item);
                }
            }
        }

        /// <summary>
        /// Find the best weapon in Inventory and wield it.
        /// </summary>
        [IncludeInBehaviour] public void WieldBestWeapon() { AgentInventory.BestWeapon().Wield(Agent); }

        /// <summary>
        /// Find the worst item in Inventory and drop it.
        /// </summary>
        [IncludeInBehaviour] public void DropWorstItem() { AgentInventory.DropWorstItem(); }

        /// <summary>
        /// Try to find a healing potion in the Inventory and use it.
        /// </summary>
        [IncludeInBehaviour]
        public void UseBestHealingPotion()
        {
            EliotItem bestPotion = null;
            for (var i = 0; i < AgentInventory.Items.Count; i++)
            {
                var item = AgentInventory.Items[i];
                if (item.Type == ItemType.HealingPotion && item.Skill && item.Value > (bestPotion == null ? -10000 : bestPotion.Value))
                    bestPotion = item;
            }
            if (bestPotion != null)
                bestPotion.Use(Agent);
        }

        /// <summary>
        /// Try to find an energy potion in the Inventory and use it.
        /// </summary>
        [IncludeInBehaviour]
        public void UseBestEnergyPotion()
        {
            EliotItem bestPotion = null;
            for (var i = 0; i < AgentInventory.Items.Count; i++)
            {
                var item = AgentInventory.Items[i];
                if (item.Type == ItemType.EnergyPotion && item.Skill && item.Value > (bestPotion == null ? -10000 : bestPotion.Value))
                    bestPotion = item;
            }
            if (bestPotion != null)
                bestPotion.Use(Agent);
        }
    }
}
