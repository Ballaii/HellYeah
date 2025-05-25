using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eliot.AgentComponents
{
    public class StandardPlayerInputConditionInterface : PlayerInputConditionInterface
    {
        public StandardPlayerInputConditionInterface(EliotAgent agent) : base(agent) 
        {
        }
        
        [IncludeInBehaviour] public bool InputDetected()
        {
            return agentPlayerInput.inputDetected;
        }
        
        [IncludeInBehaviour] public bool GetKey(string keyName)
        {
            return Input.GetKey(keyName);
        }
        
        [IncludeInBehaviour] public bool GetKeyDown(string keyName)
        {
            return Input.GetKeyDown(keyName);
        }
        
        [IncludeInBehaviour] public bool GetKeyUp(string keyName)
        {
            return Input.GetKeyUp(keyName);
        }
        
        [IncludeInBehaviour] public bool GetButton(string buttonName)
        {
            return Input.GetButton(buttonName);
        }
        
        [IncludeInBehaviour] public bool GetButtonDown(string buttonName)
        {
            return Input.GetButtonDown(buttonName);
        }
        
        [IncludeInBehaviour] public bool GetButtonUp(string buttonName)
        {
            return Input.GetButtonUp(buttonName);
        }
    }
}