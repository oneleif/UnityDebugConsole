using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Oneleif.debugconsole
{
    public class DeveloperConsole : MonoBehaviour
    {
        [SerializeField] private string prefix = string.Empty;
        [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

        [Header("UI Components")]
        [SerializeField] private Canvas consoleCanvas;

        [SerializeField] private Text consoleText;
        [SerializeField] private Text inputText;
        [SerializeField] private InputField consoleInput;

        #region Singleton

        public static DeveloperConsole Instance { get; private set; }
        private void Awake()
        {
            if (FindObjectsOfType(GetType()).Length > 1)
            {
                Destroy(gameObject);
            }

            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        #endregion Singleton

        private void Start()
        {
            consoleCanvas.gameObject.SetActive(false);
            AddMessageToConsole("Starting Developer Console...");
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logMessage, string stackTrace, LogType type)
        {
            string color = type.Equals(LogType.Warning) ? "yellow" : "white";
            AddMessageToConsole("<color=" + color + ">[" + type.ToString() + "]" + logMessage + "</color> ");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                consoleCanvas.gameObject.SetActive(!consoleCanvas.gameObject.activeInHierarchy);
                SetupInputField();
            }

            if (consoleCanvas.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.Return) && inputText.text != "")
            {
                AddMessageToConsole(inputText.text);
                ProcessCommand(inputText.text);

                SetupInputField();
            }
        }

        private void AddMessageToConsole(string msg)
        {
            consoleText.text += msg + "\n";
        }
        private void SetupInputField()
        {
            consoleInput.text = string.Empty;
            consoleInput.Select();
            consoleInput.ActivateInputField();
        }

        public void ProcessCommand(string inputValue)
        {
            if (!inputValue.StartsWith(prefix))
            {
                Debug.LogWarning("Command not recognized");
                return;
            }

            inputValue = inputValue.Remove(0, prefix.Length);

            string[] inputSplit = inputValue.Split(' ');

            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();

            if (!Array.Exists(commands, command => command.Command.Equals(commandInput, StringComparison.OrdinalIgnoreCase)))
            {
                Debug.LogWarning("Command not recognized");
                return;
            }

            ProcessCommand(commandInput, args);
        }

        public void ProcessCommand(string commandInput, string[] args)
        {
            foreach (var command in commands)
            {
                if (!commandInput.Equals(command.Command, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (command.Process(args))
                {
                    return;
                }
            }
        }
    }
}