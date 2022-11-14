using UnityEngine;

public class ColorTest : MonoBehaviour
{
    public Color testColor;
    public Color testColor2;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (testColor.GetHashCode() == testColor2.GetHashCode())
            {
                Debug.Log("==");
            }
            else
            {
                Debug.Log("!=");
            }
        }
    }
}
