using UnityEngine;
using System.Collections;

public class LoaderObject : MonoBehaviour {
    public string objectName = "loaderObject";

    void Awake(){
        DontDestroyOnLoad(gameObject);   
    }
}
