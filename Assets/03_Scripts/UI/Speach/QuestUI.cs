using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : ButtonUI
{

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

}
