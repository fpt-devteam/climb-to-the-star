using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ShurikenMovementPath", menuName = "Scriptable Objects/ShurikenMovementPath")]
public class ShurikenMovementPath : ScriptableObject
{
    public List<Vector2> movementPoints;
}
