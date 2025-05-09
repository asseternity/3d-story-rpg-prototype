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
    [CreateAssetMenu(fileName="ArticyGlobalVariables", menuName="Create GlobalVariables", order=620)]
    public class ArticyGlobalVariables : BaseGlobalVariables
    {
        
        [SerializeField()]
        [HideInInspector()]
        private GameState mGameState = new GameState();
        
        [SerializeField()]
        [HideInInspector()]
        private PlayerInfo mPlayerInfo = new PlayerInfo();
        
        #region Initialize static VariableName set
        static ArticyGlobalVariables()
        {
            variableNames.Add("GameState.talkedToRed");
            variableNames.Add("GameState.talkedToBlue");
            variableNames.Add("GameState.talkedToBoth");
            variableNames.Add("PlayerInfo.PlayerName");
            variableNames.Add("PlayerInfo.Him");
            variableNames.Add("PlayerInfo.His");
        }
        #endregion
        
        public GameState GameState
        {
            get
            {
                return mGameState;
            }
        }
        
        public PlayerInfo PlayerInfo
        {
            get
            {
                return mPlayerInfo;
            }
        }
        
        public static ArticyGlobalVariables Default
        {
            get
            {
                return ((ArticyGlobalVariables)(Articy.Unity.ArticyDatabase.DefaultGlobalVariables));
            }
        }
        
        public override void Init()
        {
            GameState.RegisterVariables(this);
            PlayerInfo.RegisterVariables(this);
        }
        
        public static ArticyGlobalVariables CreateGlobalVariablesClone()
        {
            return Articy.Unity.BaseGlobalVariables.CreateGlobalVariablesClone<ArticyGlobalVariables>();
        }
    }
}
