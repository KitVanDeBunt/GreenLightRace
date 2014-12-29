
// http://wiki.unity3d.com/index.php?title=ObjExporter

using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class ObjExporter {
	
	public static string MeshToString(MeshFilter mf, bool invertXAxis = false, bool useWorldMatrix = false) {
		Mesh m = mf.sharedMesh;
		Material[] mats = mf.renderer.sharedMaterials;
		
		StringBuilder sb = new StringBuilder();
		
		sb.Append("g ").Append(mf.name).Append("\n");
		
		foreach(Vector3 v in m.vertices) {
			Vector3 vertexPos = new Vector3(v.x,v.y,v.z);
			if(useWorldMatrix){
				vertexPos = mf.transform.localToWorldMatrix.MultiplyPoint(vertexPos);
			}
			if(invertXAxis){
				sb.Append(string.Format("v {0} {1} {2}\n",-vertexPos.x,vertexPos.y,vertexPos.z));
			}else{
				sb.Append(string.Format("v {0} {1} {2}\n",vertexPos.x,vertexPos.y,vertexPos.z));
			}
		}
		sb.Append("\n");
		foreach(Vector3 v in m.normals) {
			sb.Append(string.Format("vn {0} {1} {2}\n",v.x,v.y,v.z));
		}
		sb.Append("\n");
		foreach(Vector3 v in m.uv) {
			sb.Append(string.Format("vt {0} {1}\n",v.x,v.y));
		}
		for (int material=0; material < m.subMeshCount; material ++) {
			sb.Append("\n");
			sb.Append("usemtl ").Append(mats[material].name).Append("\n");
			sb.Append("usemap ").Append(mats[material].name).Append("\n");
			
			int[] triangles = m.GetTriangles(material);
			for (int i=0;i<triangles.Length;i+=3) {
				sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", 
				                        triangles[i+2]+1, triangles[i+1]+1, triangles[i]+1));
			}
		}
		return sb.ToString();
	}
	
	public static void MeshToFile(MeshFilter mf, string path, string filename, bool invertXAxis = false, bool useWorldMatrix = false) {
		string file = (path+filename);  
		using (StreamWriter sw = new StreamWriter(file)) {
			sw.Write(MeshToString(mf,invertXAxis,useWorldMatrix));
		}
	}
}