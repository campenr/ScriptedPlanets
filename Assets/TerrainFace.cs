using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface for each triangle face on the planet.
public class TerrainFace
{
    ShapeGenerator shapeGenerator;
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1 ) * 6];
        int triIndex = 0;

        // Iterate the face with the set resolution to build the meshes.
        // Each face has `resolution * resolution` number of squares on it.
        // Each square will itself need two triangular meshes: one in the lower left,
        // and one in the top right of the square, i.e:
        //
        //  ''''''  // resolution=1
        //  ''   '
        //  ' '  '
        //  '  ' '
        //  '   ''
        //  ''''''
        //
        //  '''''''''''  // resolution=2
        //  ''   ''   '
        //  ' '  ' '  '
        //  '  ' '  ' '
        //  '   ''   ''
        //  '''''''''''
        //  ''   ''   '
        //  ' '  ' '  '
        //  '  ' '  ' '
        //  '   ''   ''
        //  '''''''''''
        //
        int i = 0;  // essentially the number square we are on within the face, going left to right, top to bottom.
        for (int y=0; y < resolution; y++)
        {
            for (int x=0; x < resolution; x++)
            {
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + ((percent.x - .5f) * 2 * axisA) + ((percent.y - .5f) * 2 * axisB);
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);
                
                // we build triagles left to right, top to bottom, but we need to not build
                // them beyond the far right or bottom edges of our face.
                if (x != resolution -1 && y != resolution - 1)
                {
                    // calculate the points of the lower left triangle 
                    triangles[triIndex] = i;
                    triangles[triIndex+1] = i + resolution + 1;
                    triangles[triIndex+2] = i + resolution;

                    // calculate the points of the upper right triangle
                    triangles[triIndex+3] = i;
                    triangles[triIndex+4] = i + 1;
                    triangles[triIndex+5] = i + resolution + 1;

                    triIndex += 6;

                }
                                    
                i++;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

    }
}
