//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Articy.Unity;
using Articy.Unity.Interfaces;
using System;
using System.Collections;
using UnityEngine;


namespace Articy.Articy_Tutorial.GlobalVariables
{
    
    
    [Serializable()]
    public class GameState : IArticyNamespace
    {
        
        [SerializeField()]
        private BaseGlobalVariables _VariableStorage;
        
        // 
        public bool talkedToRed
        {
            get
            {
                return _VariableStorage.Internal_GetVariableValueBoolean(0);
            }
            set
            {
                _VariableStorage.Internal_SetVariableValueBoolean(0, value);
            }
        }
        
        // 
        public bool talkedToBlue
        {
            get
            {
                return _VariableStorage.Internal_GetVariableValueBoolean(1);
            }
            set
            {
                _VariableStorage.Internal_SetVariableValueBoolean(1, value);
            }
        }
        
        // 
        public bool talkedToBoth
        {
            get
            {
                return _VariableStorage.Internal_GetVariableValueBoolean(2);
            }
            set
            {
                _VariableStorage.Internal_SetVariableValueBoolean(2, value);
            }
        }
        
        public void RegisterVariables(BaseGlobalVariables aStorage)
        {
            _VariableStorage = aStorage;
            aStorage.RegisterVariable("GameState.talkedToRed", false);
            aStorage.RegisterVariable("GameState.talkedToBlue", false);
            aStorage.RegisterVariable("GameState.talkedToBoth", false);
        }
    }
}
namespace Articy.Articy_Tutorial.GlobalVariables
{
    
    
    [Serializable()]
    public class PlayerInfo : IArticyNamespace
    {
        
        [SerializeField()]
        private BaseGlobalVariables _VariableStorage;
        
        public string PlayerName
        {
            get
            {
                return _VariableStorage.Internal_GetVariableValueString(0, true);
            }
            set
            {
                _VariableStorage.Internal_SetVariableValueString(0, value);
            }
        }
        
        // 
        public string Unresolved_PlayerName
        {
            get
            {
                return _VariableStorage.Internal_GetVariableValueString(0, false);
            }
        }
        
        public string Him
        {
            get
            {
                return _VariableStorage.Internal_GetVariableValueString(1, true);
            }
            set
            {
                _VariableStorage.Internal_SetVariableValueString(1, value);
            }
        }
        
        // 
        public string Unresolved_Him
        {
            get
            {
                return _VariableStorage.Internal_GetVariableValueString(1, false);
            }
        }
        
        public string His
        {
            get
            {
                return _VariableStorage.Internal_GetVariableValueString(2, true);
            }
            set
            {
                _VariableStorage.Internal_SetVariableValueString(2, value);
            }
        }
        
        // 
        public string Unresolved_His
        {
            get
            {
                return _VariableStorage.Internal_GetVariableValueString(2, false);
            }
        }
        
        public void RegisterVariables(BaseGlobalVariables aStorage)
        {
            _VariableStorage = aStorage;
            aStorage.RegisterVariable("PlayerInfo.PlayerName", "Circle");
            aStorage.RegisterVariable("PlayerInfo.Him", "him");
            aStorage.RegisterVariable("PlayerInfo.His", "his");
        }
    }
}
