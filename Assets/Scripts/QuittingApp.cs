using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuittingApp : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        BackendGameData.Instance.SaveGameData();
    }
}
