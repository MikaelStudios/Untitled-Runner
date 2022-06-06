using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterScript : ScriptableObject
{

    public int index;
    public GameObject CharacterObject;
    public int Price;
    public string Name;
    public float speed;
    
    public float jumpHeight;
    public float climbingSpeed;
    public bool isUnlocked;
}
