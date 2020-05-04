using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommandArguments
{
    bool ValidateArguments(string[] args);
}
