using UnityEngine;


// Scriptable object to hold advisor images
[CreateAssetMenu(fileName = "AdvisorPicture", menuName = "Scriptable/AdvisorPicture", order = 0)]
public class AdvisorPicture : ScriptableObject
{
    // Sprite to use for the image
    public Sprite AdvisorImage;

    // Type of advisor the image is for
    public AdvisorType AdvisorType;

    // Gender of the picture (OTHER implies it could be MALE or FEMALE)
    public AdvisorGender Gender;
}

