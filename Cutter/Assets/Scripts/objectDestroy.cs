
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroy : MonoBehaviour
{
    public float explodeForce = 1000;
    public int maxCuts = 3;

    private List<Vector3> daggerPositions = new List<Vector3>();
    private bool isCutting = false;

    void OnCollisionEnter(Collision collision)
    {
     
        if (collision.gameObject.CompareTag("Dagger"))
        {
            daggerPositions.Clear();
            isCutting = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (isCutting && collision.gameObject.CompareTag("Dagger"))
        {
            daggerPositions.Add(collision.contacts[0].point);
            Debug.Log("hitting");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("done");
        if (isCutting && collision.gameObject.CompareTag("Dagger"))
        {
            isCutting = false;
            SliceAlongPath();
            for (int i = 0; i < daggerPositions.Count; i++)
            {
                Debug.Log(daggerPositions[i]);
            }
        }

      
    }

    void SliceAlongPath()
    {
        Mesh originalMesh = GetComponent<MeshFilter>().mesh;
        Bounds meshBounds = originalMesh.bounds;
        originalMesh.RecalculateBounds();

        List<PartMesh> parts = new List<PartMesh>();
        List<PartMesh> subParts = new List<PartMesh>();

        PartMesh mainPart = new PartMesh(originalMesh, meshBounds);
        parts.Add(mainPart);

        int cuts = Mathf.Min(maxCuts, daggerPositions.Count - 2);
        for (int i = 0; i < cuts; i++)
        {
            Vector3 a = daggerPositions[i];
            Vector3 b = daggerPositions[i + 1];
            Vector3 direction = b - a;
            Vector3 normal = Vector3.Cross(direction, Vector3.up).normalized;
            Plane cuttingPlane = new Plane(normal, (a + b) / 2f);

            foreach (PartMesh part in parts)
            {
                subParts.Add(part.GenerateMesh(cuttingPlane, true));
                subParts.Add(part.GenerateMesh(cuttingPlane, false));
            }
            parts = new List<PartMesh>(subParts);
            subParts.Clear();
        }

        foreach (PartMesh part in parts)
        {
            part.CreateGameObjectFromPart(this);
            part.GameObject.GetComponent<Rigidbody>().AddForceAtPosition(part.Bounds.center * explodeForce, transform.position);
           // var tempmesh =part.GetComponent<MeshFilter>() ;
           
           
        }

        Destroy(gameObject);
    }
}

public class PartMesh
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector3> normals = new List<Vector3>();
    public List<Vector2> uvs = new List<Vector2>();

    public Bounds Bounds { get; private set; }
    public GameObject GameObject { get; private set; }

    public PartMesh(Mesh mesh, Bounds bounds)
    {
        vertices.AddRange(mesh.vertices);
        triangles.AddRange(mesh.triangles);
        normals.AddRange(mesh.normals);
        uvs.AddRange(mesh.uv);
        Bounds = bounds;
    }

    public PartMesh GenerateMesh(Plane plane, bool keepAbove)
    {
        PartMesh result = new PartMesh(new Mesh(), Bounds);

        for (int i = 0; i < triangles.Count; i += 3)
        {
            int i1 = triangles[i];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            if (plane.GetSide(v1) == keepAbove && plane.GetSide(v2) == keepAbove && plane.GetSide(v3) == keepAbove)
            {
                result.AddTriangle(v1, v2, v3);
            }
        }

        return result;
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int index = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);

        triangles.Add(index);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);

        uvs.Add(Vector2.zero);
        uvs.Add(Vector2.right);
        uvs.Add(Vector2.up);
    }

    public void CreateGameObjectFromPart(MonoBehaviour context)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();

        GameObject = new GameObject("MeshPart");
        GameObject.transform.position = context.transform.position;
        GameObject.transform.rotation = context.transform.rotation;
        GameObject.transform.localScale = context.transform.localScale;

        MeshFilter mf = GameObject.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        MeshRenderer mr = GameObject.AddComponent<MeshRenderer>();
        mr.material = context.GetComponent<MeshRenderer>().material;

        Rigidbody rb = GameObject.AddComponent<Rigidbody>();
        BoxCollider bc = GameObject.AddComponent<BoxCollider>();
    }
}
