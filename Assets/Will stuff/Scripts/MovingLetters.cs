using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float amplitude = 10f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float waveOffset = 0.5f;
    
    private TMP_Text textMesh;
    
    void Start()
    {
        textMesh = GetComponent<TMP_Text>();
    }
    
    void Update()
    {
        textMesh.ForceMeshUpdate();
        var textInfo = textMesh.textInfo;
        
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;
            
            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            
            for (int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * frequency + i * waveOffset) * amplitude, 0);
            }
        }
        
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMesh.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}