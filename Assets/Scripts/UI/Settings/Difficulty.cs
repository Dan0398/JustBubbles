using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Settings
{
    [System.Serializable]
    public class Difficulty
    {
        [SerializeField] string FormattedString;
        [SerializeField] GameObject LineBlocker;
        
        
        public void TurnOn()
        {
            LineBlocker.SetActive(false);
        }
        
        public void TurnOff()
        {
            LineBlocker.SetActive(true);
        }
        
        public void Subscribe()
        {
            
        }
    }
}