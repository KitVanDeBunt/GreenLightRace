using UnityEngine;
using System.Collections;

public class LoaderObject : MonoBehaviour {
    [SerializeField]
    private string name = "loaderObject";

    void Awake(){
        DontDestroyOnLoad(gameObject);   
    }
}
