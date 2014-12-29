//source: https://gist.github.com/craigmjohnston/e3c014cfb7b8deeea034
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class VisualiseMeshNormals : MonoBehaviour {
    public enum MeshType { Filter, Collider }
    public MeshType meshType = MeshType.Filter;

    protected MeshFilter meshFilter;
    protected MeshCollider meshCollider;

    protected Mesh mesh {
        get {
            if (meshType == MeshType.Filter && meshFilter == null) {
                meshFilter = GetComponent<MeshFilter>();
            } else if (meshType == MeshType.Collider && meshCollider == null) {
                meshCollider = GetComponent<MeshCollider>();
            }
            return meshType == MeshType.Filter ? 
                meshFilter.sharedMesh : meshCollider.sharedMesh;
        }
    }

    void OnDrawGizmos() {
        for (int i = 0; i < mesh.vertexCount; i++) {
            Gizmos.color = Color.yellow;
            Handles.Label(transform.position + mesh.vertices[i], i.ToString());
            Gizmos.DrawLine(transform.position + mesh.vertices[i], 
                transform.position + mesh.vertices[i] 
                + mesh.normals[i].normalized * 10);
        }
    }
}
#endif