using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves_compute : MonoBehaviour
{
    private Mesh m;
    private Vector3[] verts;

    public float Amplitude = 1f;
    public float cosAmplitude = 1f;
    public float numWaves = 1f;
    public float cosnumWaves = 1f;
    public int _resolution = 8;
    public ComputeShader shader;
    ComputeBuffer _vertexBuff;
    ComputeBuffer _argsBuffer;
    Vertex[] _data;
    int _kernelIndex;
    static readonly int
            verticesId = Shader.PropertyToID("_Vertices"),
            amplitudeId = Shader.PropertyToID("Amplitude"),
            numWavesId = Shader.PropertyToID("numWaves"),
            cosAmplitudeId = Shader.PropertyToID("cosAmplitude"),
            cosnumWavesId = Shader.PropertyToID("cosnumWaves"),
            timeId = Shader.PropertyToID("_Time");
    private void Start()
    {
        m = GetComponent<MeshFilter>().sharedMesh;
        verts = m.vertices;
        _vertexBuff = new ComputeBuffer(verts.Length, 3 * sizeof(float));
        _argsBuffer = new ComputeBuffer(verts.Length, 4 * sizeof(int), ComputeBufferType.IndirectArguments);
        _argsBuffer.SetData(new int[] { verts.Length, 1, 0, 0 });
        _kernelIndex = shader.FindKernel("main");
        shader.SetBuffer(_kernelIndex,verticesId,_vertexBuff);
        _data = new Vertex[verts.Length];
    }
    private void Update()
    {
        for (int i = 0; i < verts.Length; i++)
        {
            _data[i] = new Vertex
            {
                position = verts[i]
            };
        }
        _vertexBuff.SetData(_data);
        shader.SetFloat(amplitudeId, Amplitude);
        shader.SetFloat(cosAmplitudeId, cosAmplitude);
        shader.SetFloat(numWavesId, numWaves);
        shader.SetFloat(cosnumWavesId, cosnumWaves);
        shader.SetFloat(timeId, Time.time);
        int groups = Mathf.CeilToInt(_resolution / 8f);
        shader.Dispatch(_kernelIndex, groups, groups, 1);
        _vertexBuff.GetData(_data);
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = _data[i].position;
        }
        m.vertices = verts;
        m.RecalculateBounds();
    }
    public float getSinatX(float x)
    {
        return Amplitude * Mathf.Sin((x * numWaves) + Time.time);
    }
    public float getCoseatZ(float z)
    {
        return cosAmplitude * Mathf.Cos((z * cosnumWaves) + Time.time);
    }
}
public struct Vertex
{
    public Vector3 position;
}