using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS;
public class DebugMenuTest : EntityBase
{
    public int dropdownListCount = 10;
    public override void OnRunnerStart()
    {
        base.OnRunnerStart();

        var dropdownList = new List<string>();
        for (int i = 0; i < dropdownListCount; i++)
        {
            dropdownList.Add($"Item {i+1}");
        }
        DebugMenu.AddDropdown(
            page: null,
            label: "Dropdown",
            onChanged: (idx, obj) => { Debug.Log($"Dropdown: {idx}"); },
            stringList: dropdownList,
            0,
            onShow: (obj) => { Debug.Log("Dropdown <color=green>onShow</color>"); }
        );
    }
}
