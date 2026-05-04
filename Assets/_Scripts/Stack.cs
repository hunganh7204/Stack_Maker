using System.Collections.Generic;
using UnityEngine;

public class Stack : MonoBehaviour
{
    [SerializeField] private GameObject stackPrefab;
    [SerializeField] private Transform stackParent;
    [SerializeField] private Transform playerVisual; 

    [SerializeField] private float stackHeight = 0.3f;

    private List<GameObject> stackList = new List<GameObject>();
    private List<GameObject> droppedBlocks = new List<GameObject>();

    public void AddStack()
    {
        GameObject stack = Instantiate(stackPrefab, stackParent);

        float yOffset = stackList.Count * stackHeight;
        stack.transform.localPosition = new Vector3(0, yOffset, 0);

        stackList.Add(stack);

        UpdatePlayerHeight();
    }

    private void UpdatePlayerHeight()
    {
        float height = stackList.Count * stackHeight;
        playerVisual.localPosition = new Vector3(0, height, 0);
    }

    public int GetStackCount()
    {
        return stackList.Count;
    }

    public void RemoveStack(Vector3 dropPosition)
    {
        if(stackList.Count > 0)
        {
            GameObject topStack = stackList[stackList.Count - 1];
            stackList.RemoveAt(stackList.Count - 1);

            topStack.transform.SetParent(null);
            topStack.transform.position = new Vector3(dropPosition.x, dropPosition.y, dropPosition.z);

            topStack.layer = LayerMask.NameToLayer("Default");

            Collider col = topStack.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            droppedBlocks.Add(topStack);
            UpdatePlayerHeight();
        }
    }

    public void ClearStack()
    {
        foreach (GameObject stack in stackList)
        {
            if(stack != null) Destroy(stack);
        }
        stackList.Clear();
        foreach (GameObject dropped in droppedBlocks)
        {
            if (dropped != null) Destroy(dropped);
        }
        droppedBlocks.Clear();
        UpdatePlayerHeight();
    }
}

