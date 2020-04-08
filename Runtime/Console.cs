using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oneleif.debugconsole
{
    public class Console : MonoBehaviour
    {
        #region Singleton

        public static Console Instance { get; private set; }

        private void Awake()
        {
            if (FindObjectsOfType(GetType()).Length > 1)
            {
                Destroy(gameObject);
            }

            if (Console.Instance == null)
            {
                Console.Instance = this;
            }
            else if (Console.Instance == this)
            {
                Destroy(Console.Instance.gameObject);
                Console.Instance = this;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        #endregion Singleton
    }
}