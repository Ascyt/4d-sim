using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tesseract : Hyperobject
{
    public Tesseract() : base(GenerateTetrahedrons().ToArray())
    {

    }

    // Thanks to GPT-o3-mini for this code <3

    // This function returns a list of Tetrahedron instances that triangulate the 3D boundary of the unit 4D hypercube.
    public static List<Tetrahedron> GenerateTetrahedrons()
    {
        // First, generate our 16 hypercube vertices as Vector4's where each entry is 0 or 1.
        // We store these in an array where the index is computed by: index = a*8 + b*4 + c*2 + d.
        Vector4[] vertices = new Vector4[16];
        for (int a = 0; a <= 1; a++)
        {
            for (int b = 0; b <= 1; b++)
            {
                for (int c = 0; c <= 1; c++)
                {
                    for (int d = 0; d <= 1; d++)
                    {
                        int idx = a * 8 + b * 4 + c * 2 + d;
                        vertices[idx] = new Vector4(a, b, c, d);
                    }
                }
            }
        }

        // We now need to process the 8 cube facets.
        // Each facet is the intersection of one coordinate = fixed value.
        // The available facets are: x=0, x=1, y=0, y=1, z=0, z=1, w=0, w=1.
        // For each facet, we will extract the 8 vertices (by filtering the vertices in the hypercube).
        // Then we apply our fixed subdivision pattern.
        //
        // For the template, let’s fix the following ordering for the facet "x = constant".
        // We adopt the labeling for the facet as described:
        //
        //    Let the facet (cube) vertices be:
        //    A = (constant, 0, 0, 0)
        //    B = (constant, 0, 1, 0)
        //    C = (constant, 1, 1, 0)
        //    D = (constant, 1, 0, 0)
        //    E = (constant, 0, 0, 1)
        //    F = (constant, 0, 1, 1)
        //    G = (constant, 1, 1, 1)
        //    H = (constant, 1, 0, 1)
        //
        // And use the following tetrahedra (each defined by 4 vertices):
        // T1 = (A, B, D, E)
        // T2 = (B, D, C, G)
        // T3 = (B, E, F, G)
        // T4 = (D, E, H, G)
        // T5 = (B, D, E, G)
        //
        // For facets that are not aligned with the x–axis, we “rotate” the coordinates.
        // We define a helper function later that given the facet (which coordinate is fixed, and its fixed value)
        // takes our template (with coordinate order [x,y,z,w]) and returns the properly permuted 4–tuples.

        List<Tetrahedron> tetrahedra = new List<Tetrahedron>();

        // Define our 8 facets; each facet is a tuple: (coordIndex, fixedValue).
        // coordIndex: 0 for x, 1 for y, 2 for z, 3 for w.
        var facets = new List<(int coordIndex, int fixedValue)>
        {
            (0, 0), (0, 1),
            (1, 0), (1, 1),
            (2, 0), (2, 1),
            (3, 0), (3, 1)
        };

        // Template: the cube facet when the fixed coordinate is x.
        // We use indices into a 4-tuple (x,y,z,w) for convenience.
        // For our template, we have:
        // A = (0,0,0,0)
        // B = (0,0,1,0)
        // C = (0,1,1,0)
        // D = (0,1,0,0)
        // E = (0,0,0,1)
        // F = (0,0,1,1)
        // G = (0,1,1,1)
        // H = (0,1,0,1)
        //
        // The five tetrahedrons (each a list of 4 vertices from {A,B,C,D,E,F,G,H}):
        // T1: A, B, D, E
        // T2: B, D, C, G
        // T3: B, E, F, G
        // T4: D, E, H, G
        // T5: B, D, E, G
        //
        // We record the indices for the cube vertices in our template ordering:
        // Let TemplateVertices[0] = A, 1 = B, 2 = C, 3 = D, 4 = E, 5 = F, 6 = G, 7 = H.
        //
        // For the template facet, the coordinates in each vertex are:
        // Index: 0 => (fixed, 0, 0, 0)
        //        1 => (fixed, 0, 1, 0)
        //        2 => (fixed, 1, 1, 0)
        //        3 => (fixed, 1, 0, 0)
        //        4 => (fixed, 0, 0, 1)
        //        5 => (fixed, 0, 1, 1)
        //        6 => (fixed, 1, 1, 1)
        //        7 => (fixed, 1, 0, 1)
        //
        // And note that when the fixed coordinate is not x (coord index 0), we “rotate” the coordinates.

        // Each tetrahedron from the template is defined by 4 indices into the template vertex array.
        int[][] templateTets = new int[][]
        {
            new int[] {0, 1, 3, 4}, // T1: A, B, D, E
            new int[] {1, 3, 2, 6}, // T2: B, D, C, G
            new int[] {1, 4, 5, 6}, // T3: B, E, F, G
            new int[] {3, 4, 7, 6}, // T4: D, E, H, G
            new int[] {1, 3, 4, 6}  // T5: B, D, E, G
        };

        // The template cube facet’s 8 vertices in “template coordinates”: these are 4-tuples (c0, c1, c2, c3)
        // where we think of the first coordinate as the fixed one.
        // For the facet "x = constant", our template is:
        Vector4[] templateVertices = new Vector4[8];
        // We let position[0] be set later from the facet fixed value.
        // But in terms of relative positions for the non-fixed coordinates, we have:
        templateVertices[0] = new Vector4(0, 0, 0, 0);  // A : (fixed, 0, 0, 0)
        templateVertices[1] = new Vector4(0, 0, 1, 0);  // B : (fixed, 0, 1, 0)
        templateVertices[2] = new Vector4(0, 1, 1, 0);  // C : (fixed, 1, 1, 0)
        templateVertices[3] = new Vector4(0, 1, 0, 0);  // D : (fixed, 1, 0, 0)
        templateVertices[4] = new Vector4(0, 0, 0, 1);  // E : (fixed, 0, 0, 1)
        templateVertices[5] = new Vector4(0, 0, 1, 1);  // F : (fixed, 0, 1, 1)
        templateVertices[6] = new Vector4(0, 1, 1, 1);  // G : (fixed, 1, 1, 1)
        templateVertices[7] = new Vector4(0, 1, 0, 1);  // H : (fixed, 1, 0, 1)

        // For a given facet, the fixed coordinate is specified by facet.coordIndex and facet.fixedValue.
        // The mapping is: in the template above the first coordinate (index 0) is “the fixed coordinate”.
        // For a general facet with fixed coordinate at position p, we must permute the coordinates so that:
        //   new[p] = fixedValue,
        //   and the remaining coordinates come from the template vertex’s Y, Z, W parts (in order).
        //
        // For example, if p==0 then nothing changes (our template already has coordinate 0 fixed).
        // If p==1 (i.e. facet y = constant), then for each template vertex T = (dummy, a, b, c) we want 
        // the actual vertex = (a, fixed, b, c).
        // If p==2 (facet z = constant): from T = (dummy, a, b, c) output (a, b, fixed, c).
        // If p==3 (facet w = constant): from T = (dummy, a, b, c) output (a, b, c, fixed).
        //
        // We write a small helper “Remap” function that does this.

        foreach (var facet in facets)
        {
            // Get the remapped vertices for this facet.
            Vector4[] facetVerts = RemapFacetVertices(templateVertices, facet.coordIndex, facet.fixedValue);
            // For each tetrahedron in our template, create a Tetrahedron using the facet vertices.
            foreach (var tet in templateTets)
            {
                // tet is a list of 4 indices into facetVerts.
                Tetrahedron t = new Tetrahedron(
                    facetVerts[tet[0]],
                    facetVerts[tet[1]],
                    facetVerts[tet[2]],
                    facetVerts[tet[3]]);
                tetrahedra.Add(t);
            }
        }

        // At the end, tetrahedra.Count will be 8*5 = 40.
        return tetrahedra;
    }

    // RemapFacetVertices takes the template vertices (where the first coordinate is the fixed one)
    // and returns new vertices where the fixed coordinate is moved to the desired slot.
    // coordIndex indicates which coordinate (0=x, 1=y, 2=z, 3=w) will be set to fixedValue.
    // The remaining three coordinates come from the template’s (Y, Z, W) components (in order).
    // In our template, each vertex is stored as (dummy, a, b, c) and dummy will be replaced.
    private static Vector4[] RemapFacetVertices(Vector4[] templateVerts, int coordIndex, int fixedValue)
    {
        Vector4[] result = new Vector4[templateVerts.Length];
        foreach (var (v, i) in Enumerate(templateVerts))
        {
            float a = v.y;  // template second coordinate
            float b = v.z;  // template third coordinate
            float c = v.w;  // template fourth coordinate
            // Build a new Vector4 where at index coordIndex we put fixedValue,
            // and the other coordinates are taken in order from (a, b, c).
            float[] comps = new float[4];
            // We'll fill comps in order.
            int freeIndex = 0;
            for (int j = 0; j < 4; j++)
            {
                if (j == coordIndex)
                {
                    comps[j] = fixedValue;
                }
                else
                {
                    // Take the next available value from a, b, c.
                    if (freeIndex == 0) { comps[j] = a; }
                    else if (freeIndex == 1) { comps[j] = b; }
                    else if (freeIndex == 2) { comps[j] = c; }
                    freeIndex++;
                }
            }
            result[i] = new Vector4(comps[0], comps[1], comps[2], comps[3]);
        }
        return result;
    }

    // Helper to enumerate an array with indices.
    private static IEnumerable<(T item, int index)> Enumerate<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
            yield return (array[i], i);
    }
}
