using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoCompleteItem : MonoBehaviour
{
    public Text commandText;
    public Text commandDescriptionText;

    public void SetCommandText(string textToSet)
    {
        commandText.text = textToSet;
    }

    public void SetCommandDescriptionText(string textToSet)
    {
        commandDescriptionText.text = textToSet;
    }
}
