using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oneleif.debugconsole
{
    public class AutoComplete : MonoBehaviour
    {
        [SerializeField] private RectTransform autocompleteParent;
        [SerializeField] private RectTransform autoCompleteItemPrefab;
        [SerializeField] private RectTransform autocompleteScrollView;

        void Start()
        {
            autocompleteScrollView.gameObject.SetActive(false);
        }

        public void ClearResults()
        {
            foreach (Transform child in autocompleteParent)
            {
                Destroy(child.gameObject);
            }

            autocompleteScrollView.gameObject.SetActive(false);
        }

        public void FillResults(InputField consoleInput)
        {
            ClearResults();

            if (string.IsNullOrEmpty(consoleInput.text))
            {
                return;
            }

            ConsoleCommand[] commands = DeveloperConsole.Instance.commands;
            bool foundValidCommand = false;

            foreach(ConsoleCommand command in commands)
            {
                if (command.Command.StartsWith(consoleInput.text))
                {
                    foundValidCommand = true;
                    RectTransform autoCompleteItem = Instantiate(autoCompleteItemPrefab, autocompleteParent) as RectTransform;
                    AutoCompleteItem item = autoCompleteItem.GetComponent<AutoCompleteItem>();
                    item.SetCommandText(command.Command);
                    item.SetCommandDescriptionText(command.Description);
                }
            }

            if (foundValidCommand)
            {
                autocompleteScrollView.gameObject.SetActive(true);
            }
        }
    }
}
