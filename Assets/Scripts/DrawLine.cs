using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private float animateDuration = 1f;
    // [SerializeField] private string fileIndex;

    private LineRenderer m_lineRenderer;
    private Vector3[] m_linePoints;
    private int m_pointCounts;

    public void StartDraw(string fileIndex) {
        m_lineRenderer = GetComponent<LineRenderer>();

        string filenamePrefix = "data_";
        string filename = filenamePrefix + fileIndex;

        Debug.LogFormat("filename : {0}",filename);

        TextAsset ResourceRequest = Resources.Load(filename) as TextAsset;
        RadarData rd = RadarData.CreateFromJSON(ResourceRequest.ToString());

        m_pointCounts = rd.Index.Count;
        m_linePoints = new Vector3[m_pointCounts];
        m_lineRenderer.positionCount = m_pointCounts;
        for (int i = 0; i < m_pointCounts; i++)
        {
            m_linePoints[i] = new Vector3(
                (float)rd.X[i],
                (float)rd.Y[i],
                (float)rd.Z[i]);
            // Debug.LogFormat("vector[{0}]: {1}", i , m_linePoints[i]);
        }
        StartCoroutine(AnimateLines());
    }

    void Start() {
    }

    private IEnumerator AnimateLines()
    {
        float segmentDurtaion = animateDuration / m_pointCounts;

        for (int i = 0; i < m_pointCounts - 1; i++)
        {
            float startTime = Time.time;
            Vector3 startPos = m_linePoints[i];
            Vector3 endPos = m_linePoints[i + 1];
            Vector3 pos = startPos;
            while (pos != endPos)
            {
                float t = (Time.time - startTime) / animateDuration;
                pos = Vector3.Lerp(startPos, endPos, 1);
                for (int j = i + 1; j < m_pointCounts; j++)
                {
                    m_lineRenderer.SetPosition(j, pos);
                }
                yield return null;
            }
        }
    }
}


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
[Serializable]
public class ClubResult
{
    public double club_path_angle;
    public long recv_time;
    public double attack_angle;
    public double club_speed;
    public double swing_plane_angle;
    public double swing_direction;
}

[Serializable]
public class CarryResult
{
    public double ball_flight2;
    public int ball_flight1;
    public long recv_time;
    public double carry_total;
    public double carry;
}

[Serializable]
public class BallResult
{
    public double launch_angle;
    public long recv_time;
    public int side_spin;
    public double speed;
    public double back_spin;
    public double direction;
}

[Serializable]
public class RadarData
{
    public double PathHeight;
    public ClubResult ClubResult;
    public List<int> Index;
    public CarryResult CarryResult;
    public List<double> X;
    public List<double> Y;
    public List<double> Z;
    public BallResult BallResult;
    public double APEX;

    public static RadarData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<RadarData>(jsonString);
    }
}
