using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils; 

public class FollowMouse : MonoBehaviour
{
    private void Update() => transform.position = UtilsClass.GetMouseWorldPosition();
}
