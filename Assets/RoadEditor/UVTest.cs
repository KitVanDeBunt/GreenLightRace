using UnityEngine;
using System.Collections;

public class UVTest : MonoBehaviour{
	[SerializeField]
	private Mesh mesh;
	
	void Start (){
		int i;
		for(i = 0; i < mesh.uv.Length;i++){
			print ("uv:"+i+">"+mesh.uv[i]);
		}
		for(i = 0; i < mesh.uv1.Length;i++){
			print ("uv1:"+i+">"+mesh.uv1[i]);
		}
		for(i = 0; i < mesh.uv2.Length;i++){
			print ("uv1:"+i+">"+mesh.uv1[i]);
		}
	}
}

