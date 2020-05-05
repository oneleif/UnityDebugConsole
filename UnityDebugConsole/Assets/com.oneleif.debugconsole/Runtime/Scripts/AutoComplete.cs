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

        public List<AutoCompleteItem> autoCompleteItems;
        public int selectionIndex;

        void Start()
        {
            autocompleteScrollView.gameObject.SetActive(false);
            autoCompleteItems = new List<AutoCompleteItem>();
            selectionIndex = 0;
        }

        public string GetAutoCompleteCommand()
        {
            return autoCompleteItems[selectionIndex].ConsoleCommand.Command;
        }

        public void ClearResults()
        {
            foreach (Transform child in autocompleteParent)
            {
                Destroy(child.gameObject);
            }

            autocompleteScrollView.gameObject.SetActive(false);
            autoCompleteItems = new List<AutoCompleteItem>();
            selectionIndex = 0;
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
                    item.ConsoleCommand = command;
                    autoCompleteItems.Add(item);
                }
            }

            if (foundValidCommand)
            {
                autocompleteScrollView.gameObject.SetActive(true);
                HighlightSelection();
            }
        }

        public bool AutoCompleteHasItems()
        {
            return autoCompleteItems.Count > 0;
        }

        public void SelectResult(SelectionDirection direction)
        {
            UnhighlightSelection();

            if (direction == SelectionDirection.up)
            {
                if(selectionIndex > 0)
                {
                    selectionIndex--;
                }
            }else if(direction == SelectionDirection.down)
            {
                if (selectionIndex < autoCompleteItems.Count - 1)
                {
                    selectionIndex++;
                }
            }

            HighlightSelection();
        }

        private void UnhighlightSelection()
        {
            autoCompleteItems[selectionIndex].Unhighlight();
        }

        private void HighlightSelection()
        {
            autoCompleteItems[selectionIndex].Highlight();
        }
    }
}
