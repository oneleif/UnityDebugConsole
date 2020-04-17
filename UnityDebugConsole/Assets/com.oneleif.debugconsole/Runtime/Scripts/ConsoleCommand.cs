using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oneleif.debugconsole
{
    public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
    {
        [SerializeField] private string command = string.Empty;
        [SerializeField] private string description = string.Empty;
        [SerializeField] private string help = string.Empty;

        public string Command => command;
        public string Description => description;
        public string Help => help;

        public abstract bool Process(string[] args);
    }
}