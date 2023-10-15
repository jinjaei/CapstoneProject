using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using InfiniteValue;


public class ResourceText : MonoBehaviour
{
    public ResourceManager.ResourceType resourceType;
    public TextMeshProUGUI resourceText;
    // Start is called before the first frame update
    private void OnEnable()
    {
        ResourceManager.instance.OnResourceChanged += UpdateResourceText;
        UpdateResourceText(resourceType, ResourceManager.instance.GetResourceValue(resourceType));
        resourceText.text = ResourceManager.instance.GetResourceValue(resourceType).ToString();

    }    
    private void UpdateResourceText(ResourceManager.ResourceType type, InfVal newValue)
    {
        if(resourceType== type)
        {
            resourceText.text = newValue.ToString();
        }
    }
}
