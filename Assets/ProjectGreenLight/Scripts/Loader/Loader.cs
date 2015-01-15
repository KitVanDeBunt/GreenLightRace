using UnityEngine;


public class Loader : MonoBehaviour {

    public LoaderObject[] loadObjects;

    private static bool loaded = false;

    void Start()
    {
        for (int i = 0; i < loadObjects.Length; i++)
        {
            GameObject newObject = (GameObject)GameObject.Instantiate(loadObjects[i].gameObject, Vector3.zero, Quaternion.identity);
            newObject.name = loadObjects[i].name;
        }
        loaded = true;
    }
	
}
