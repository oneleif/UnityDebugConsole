using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Oneleif.debugconsole
{
    public class DeveloperConsole : MonoBehaviour
    {
        [SerializeField] public ConsoleCommand[] commands;

        [Header("UI Components")]
        [SerializeField] private Canvas consoleCanvas;
        [SerializeField] private Text consoleText;
        [SerializeField] private InputField consoleInput;

        private bool consoleIsActive = false;

        private AutoComplete autoComplete;
        private FileLogger fileLogger;

        private List<string> cachedCommands;
        private int currentCacheIndex;

        enum CacheDirection
        {
            up, down
        }

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
            consoleCanvas.gameObject.SetActive(consoleIsActive);
            autoComplete = GetComponentInChildren<AutoComplete>();
            fileLogger = GetComponent<FileLogger>();
            cachedCommands = new List<string>();
        }

        private void OnEnable()
        {
            if (ConsoleConstants.shouldOutputDebugLogs)
            {
                Application.logMessageReceived += HandleLog;
            }
        }

        private void OnDisable()
        {
            if (ConsoleConstants.shouldOutputDebugLogs)
            {
                Application.logMessageReceived -= HandleLog;
            }
        }

        private void HandleLog(string logMessage, string stackTrace, LogType type)
        {
            string color = "";
            switch (type)
            {
                case LogType.Error:
                    color = "red";
                    break;

                case LogType.Warning:
                    color = "yellow";
                    break;

                case LogType.Log:
                    color = "white";
                    break;
            }

            LogMessage("<color=" + color + ">[" + type.ToString() + "]" + logMessage + "</color> ");
        }

        private void Update()
        {
            if (Input.GetKeyDown(ConsoleConstants.toggleKey))
            {
                ToggleConsole();
            }

            if (consoleIsActive)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    MoveCache(CacheDirection.up);
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    MoveCache(CacheDirection.down);
                }
            }
        }

        private void ToggleConsole()
        {
            consoleIsActive = !consoleIsActive;
            consoleCanvas.gameObject.SetActive(consoleIsActive);
            if (consoleIsActive)
            {
                currentCacheIndex = cachedCommands.Count;
                SetupInputField();
            }
            else
            {
                consoleInput.DeactivateInputField();
            }
        }

        private void LogMessage(string message)
        {
            consoleText.text += message + "\n";
            fileLogger.LogToFile(message);
        }

        private void SetupInputField()
        {
            ClearInputField(consoleInput);
            consoleInput.text = string.Empty;
            consoleInput.Select();
            consoleInput.ActivateInputField();

            consoleInput.onEndEdit.RemoveAllListeners();
            consoleInput.onEndEdit.AddListener(delegate
            { ProcessCommand(consoleInput); });

            consoleInput.onValueChanged.RemoveAllListeners();
            consoleInput.onValueChanged.AddListener(delegate
            { ShowCommandAutoComplete(consoleInput); });
        }

        public void ShowCommandAutoComplete(InputField consoleInput)
        {
            autoComplete.FillResults(consoleInput);
        }

        public void ProcessCommand(InputField consoleInput)
        {
            (ConsoleCommand command, string[] args) = GetCommandFromInput(consoleInput.text);
            LogMessage(ConsoleConstants.commandPrefix + consoleInput.text);
            ClearInputField(consoleInput);
            if (command != null)
            {
                command.Process(args);
            }
            else
            {
                Debug.LogWarning("Command not recognized");
            }
        }

        private (ConsoleCommand, string[]) GetCommandFromInput(string input)
        {
            // Split command from arguments
            string[] inputSplit = input.Split(' ');

            string commandInput = inputSplit[0];
            string[] commandArguments = inputSplit.Skip(1).ToArray();

            ConsoleCommand command = GetValidCommand(commandInput);
            return (command, commandArguments);
        }

        public ConsoleCommand GetValidCommand(string inputCommand)
        {
            foreach (var command in commands)
            {
                if(command.Command == inputCommand)
                {
                    return command;
                }
            }

            return null;
        }
        private void ClearInputField(InputField consoleInput)
        {
            consoleInput.text = string.Empty;
            consoleInput.Select();
            consoleInput.ActivateInputField();
        }


        private void MoveCache(CacheDirection direction)
        {
            if(cachedCommands.Count > 0)
            {
                if (direction == CacheDirection.up)
                {
                    if (currentCacheIndex > 0)
                    {
                        currentCacheIndex--;
                        consoleInput.text = cachedCommands[currentCacheIndex];

                    }
                }
                else if (direction == CacheDirection.down)
                {
                    if (currentCacheIndex < cachedCommands.Count - 1)
                    {
                        currentCacheIndex++;
                        consoleInput.text = cachedCommands[currentCacheIndex];
                    }
                    else
                    {
                        consoleInput.text = string.Empty;
                    }
                }
            }

            consoleInput.ActivateInputField();
            consoleInput.Select();
            consoleInput.MoveTextEnd(false);
        }
    }
}