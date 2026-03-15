using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinhManager
{
    public class Manager : MonoBehaviour {
        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    } 
}
