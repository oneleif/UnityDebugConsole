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
        [Header("Options")]
        public KeyCode toggleKey = KeyCode.BackQuote;
        public bool shouldLogToFile = false;
        public bool shouldOutputDebugLogs = false;
        [SerializeField] private string commandPrefix = string.Empty;
        [SerializeField] private string userInputPrefix = "> ";
        [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

        [Header("UI Components")]
        [SerializeField] private Canvas consoleCanvas;
        [SerializeField] private Text consoleText;
        [SerializeField] private InputField consoleInput;

        // Console props
        private bool consoleIsActive = false;

        // Constants
        private string logFileName = "log.txt";
        private bool addTimestamp = true;

        private StreamWriter OutputStream;

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
            cachedCommands = new List<string>();

            if (shouldLogToFile)
            {
                // Outputs to: C:\Users\<your-user>\AppData\LocalLow\DefaultCompany\UnityDebugConsole\log.txt
                string logFilePath = Path.Combine(Application.persistentDataPath, logFileName);
                OutputStream = new StreamWriter(logFilePath, false);
                // TODO: clear old log files if they're too big
            }
        }

        private void OnEnable()
        {
            if (shouldOutputDebugLogs)
            {
                Application.logMessageReceived += HandleLog;

            }
        }

        private void OnDisable()
        {
            if (shouldOutputDebugLogs)
            {
                Application.logMessageReceived -= HandleLog;
            }
        }

        private void OnDestroy()
        {
            OutputStream.Close();
            OutputStream = null;
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
            if (Input.GetKeyDown(KeyCode.BackQuote))
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

            if (shouldLogToFile)
            {
                LogToFile(message);
            }
        }

        private void LogToFile(string message)
        {
            if (addTimestamp)
            {
                DateTime now = DateTime.Now;
                message = string.Format("[{0:H:mm:ss}] {1}", now, message);
            }

            if (OutputStream != null)
            {
                OutputStream.WriteLine(message);
                OutputStream.Flush();
            }
        }
        
        private void SetupInputField()
        {
            consoleInput.text = string.Empty;
            consoleInput.Select();
            consoleInput.ActivateInputField();

            consoleInput.onEndEdit.RemoveAllListeners();
            consoleInput.onEndEdit.AddListener(delegate { ProcessCommand(consoleInput); });

            consoleInput.onValueChanged.RemoveAllListeners();
            consoleInput.onValueChanged.AddListener(delegate { ShowCommandAutoComplete(consoleInput); });
        }

        public void ShowCommandAutoComplete(InputField consoleInput)
        {

        }

        public void ProcessCommand(InputField consoleInput)
        {
            string inputValue = consoleInput.text;

            if(inputValue.Contains("`"))
            {
                return;
            }

            // Print the user's input
            LogMessage(userInputPrefix + inputValue);

            if (!inputValue.StartsWith(commandPrefix))
            {
                Debug.LogWarning("Command not recognized");
                return;
            }

            // Remove prefix from the command string
            inputValue = inputValue.Remove(0, commandPrefix.Length);

            // Split command from arguments
            string[] inputSplit = inputValue.Split(' ');

            string commandInput = inputSplit[0];
            string[] commandArguments = inputSplit.Skip(1).ToArray();

            if (!Array.Exists(commands, command => command.Command.Equals(commandInput, StringComparison.OrdinalIgnoreCase)))
            {
                Debug.LogWarning("Command not recognized");
            }
            else
            {
                ProcessCommand(commandInput, commandArguments);
            }


            // Add command to cache and reset the field
            cachedCommands.Add(inputValue);
            currentCacheIndex = cachedCommands.Count;
            consoleInput.text = string.Empty;
            consoleInput.ActivateInputField();
            consoleInput.Select();
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