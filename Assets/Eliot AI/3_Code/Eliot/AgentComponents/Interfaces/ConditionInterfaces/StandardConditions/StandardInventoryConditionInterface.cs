using Eliot.Environment;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// The Standard Library of inventory related conditions.
    /// </summary>
    public class StandardInventoryConditionInterface : InventoryConditionInterface
    {
        public StandardInventoryConditionInterface(EliotAgent agent) : base(agent)
        {
        }

        /// <summary>
        /// Check whether the Agent has a better weapon in his inventory than the currently wielded one.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool HaveBetterWeapon()
        {
            return AgentInventory.HaveBetterItem(ItemType.Weapon);
        }

        /// <summary>
        /// Check whether the Agent has any healing potion in its Inventory.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool HaveHealingPotion()
        {
            foreach (var item in AgentInventory.Items)
                if (item.Type == ItemType.HealingPotion && item.Skill)
                {
                    return true;
                }
            return false;
        }

        /// <summary>
        /// Check whether the Agent has any energy replenishing potion in its Inventory.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool HaveEnergyPotion()
        {
            foreach (var item in AgentInventory.Items)
                if (item.Type == ItemType.EnergyPotion && item.Skill)
                    return true;
            return false;
        }
    }
}
