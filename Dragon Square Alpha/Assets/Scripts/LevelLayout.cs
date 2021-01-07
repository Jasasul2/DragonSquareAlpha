using System.Collections;
using UnityEngine;

[System.Serializable]
public class LevelLayout
{
    // 0 = normal block 
    // 1 = wall / rock 
    // 2 = lava 


    [System.Serializable]
    public struct rowData
    {
        public int[] columns;
    }

    public rowData[] rows = new rowData[9]; // grid 17 * 9 
}
