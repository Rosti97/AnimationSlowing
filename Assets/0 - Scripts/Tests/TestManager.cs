using UnityEngine;

public class TestManager : MonoBehaviour
{
public GameObject sphereObject;    // Referenz auf die bereits im Scene vorhandene Sphere
public GameObject recObject;       // Referenz auf vorhandenes Rechteck
    public GameObject middleObject;    // Referenz auf vorhandenes Mittelobjekt
    public GameObject downObject;
    public GameObject upObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            sphereObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            recObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //middleObject.SetActive(true);
            upObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            downObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            middleObject.SetActive(true);
        }
}

}
