using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "UsableObjects", menuName = "Scriptable Objects/Usables")]
public class Usables : ItemData
{

    [Header("Usable Item Properties")]
    public int healAmount;
    public bool isUsable;

    


}