using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CatProperties", menuName = "Cats/CatProperties", order = 1)]
public class CatProperties : ScriptableObject {
    public float speed = 10;
    public LayerMask mask;
    public float hoistTime = 1;
    public float hoistDrag = 10;
    public float hoistVelocity = 10;
}
