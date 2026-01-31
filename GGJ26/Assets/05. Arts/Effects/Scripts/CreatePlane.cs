using UnityEngine;
using UnityEditor;

public class CreatePlane : MonoBehaviour
{
    [MenuItem("GameObject/3D Object/High Poly Plane")]
    static void Create()
    {
        GameObject obj = new GameObject("HighPolyPlane");
        MeshFilter filter = obj.AddComponent<MeshFilter>();
        MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        Mesh mesh = new Mesh();
        
        // 원하는 해상도 (예: 50 x 50) - 이 숫자가 높으면 주름이 부드러워짐
        int xSize = 50; 
        int ySize = 50;
        
        Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x / (float)xSize - 0.5f, y / (float)ySize - 0.5f, 0); // 중심점 맞춤
                uv[i] = new Vector2(x / (float)xSize, y / (float)ySize);
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        filter.mesh = mesh;
        Selection.activeGameObject = obj;
    }
}