using UnityEngine;


[CreateAssetMenu(fileName = "AdvisorPicture", menuName = "Scriptable/AdvisorPicture", order = 0)]
public class AdvisorPicture : ScriptableObject
{
    public Sprite AdvisorImage;
    public AdvisorType AdvisorType;
    public AdvisorGender Gender;
}

