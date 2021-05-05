using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class BallManager : MonoBehaviour
{
    public Text carry;
    public Text bspeed;
    public Text cspeed;
    public Text type;

    public InputField inputField;
    public Camera mainCam;

    public ARRaycastManager m_Raycaster;
    static List<ARRaycastHit> s_hits = new List<ARRaycastHit>();

    public Transform camPivot;
    public Transform pivot;

    public Vector2 m_centerVec;

    private Vector3 m_curPivtoPos;
    private Quaternion m_curCamRot;

    private Vector3 m_ballPos;
    private Quaternion m_ballRot;
    private bool m_ballEnable = false;
    private bool m_ballSpawned = false;
    private GameObject m_ballObject;
    public Transform ballObjectPool;

    public GameObject lineRendererPrefab;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        m_centerVec = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        inputField.text = "1"; // default
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Raycaster.Raycast(m_centerVec, s_hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_hits[0].pose;
            m_ballEnable = true;
            pivot.position = Vector3.Lerp(pivot.position, hitPose.position, 0.2f);
            pivot.rotation = Quaternion.Lerp(pivot.rotation, hitPose.rotation, 0.2f);
            m_curPivtoPos = hitPose.position;
            m_curCamRot = mainCam.transform.rotation;
        }
        else
        {
            m_ballEnable = false;
        }
    }

    public void MakeBallObject()
    {
        Debug.Log("MakeBallObject");
        if (m_ballEnable)
        {
            if (m_ballObject != null)
            {
                Destroy(m_ballObject);
                m_ballObject = null;
            }
            Debug.Log("Instantiate");

            GameObject prefab = Resources.Load("Prefab/GolfBall") as GameObject;
            m_ballObject = Instantiate(prefab) as GameObject;
            m_ballObject.transform.SetParent(ballObjectPool);
            m_ballObject.transform.position = m_curPivtoPos;
            m_ballObject.transform.rotation = m_curCamRot;
            m_ballObject.transform.localScale /= 3;

            m_ballPos = m_curPivtoPos;
            m_ballRot = m_curCamRot;
            m_ballSpawned = true;
        }
    }

    [SerializeField] private float renderAnimateDuration = 1f;
    public void DrawBallTrajectory()
    {
        Debug.Log("DrawBall First");
        if (m_ballSpawned)
        {
            string index = inputField.text;
            if (int.Parse(index) < 0 || int.Parse(index) > 5)
            {
                index = "1";
            }
            Debug.LogFormat("DrawBall (index : {0})", index);

            string filenamePrefix = "data_";
            string filename = filenamePrefix + index;
            TextAsset ResourceRequest = Resources.Load(filename) as TextAsset;
            RadarData rd = RadarData.CreateFromJSON(ResourceRequest.ToString());

            GameObject tLine = Instantiate(lineRendererPrefab);
            lineRenderer = tLine.GetComponent<LineRenderer>();

            Quaternion test = mainCam.transform.rotation;
            test.x = 0;
            test.z = 0;

            lineRenderer.transform.SetPositionAndRotation(m_ballPos, test);
            lineRenderer.alignment = LineAlignment.TransformZ;
            lineRenderer.useWorldSpace = false;

            int pointCounts = rd.Index.Count;
            Vector3[] linePoints = new Vector3[pointCounts];
            lineRenderer.positionCount = pointCounts;

            for (int i = 0; i < pointCounts; i++)
            {
                linePoints[i] = new Vector3(
                    (float)rd.X[i] - (float)rd.X[0],
                    (float)rd.Y[i] - (float)rd.Y[0],
                    (float)rd.Z[i] - (float)rd.Z[0]);
                //Debug.LogFormat("vector[{0}]: {1}", i, linePoints[i]);
            }
            //lineRenderer.SetPositions(linePoints);
            carry.text = rd.CarryResult.carry.ToString();
            bspeed.text = rd.BallResult.speed.ToString() + "m/s";
            cspeed.text = rd.ClubResult.club_speed.ToString() + "m/s";
            type.text = rd.Type;

            Debug.LogFormat("type : {0}", rd.Type);

            StartCoroutine(AnimateLines(linePoints));

            m_ballSpawned = false;
        }
    }

    private IEnumerator AnimateLines(Vector3[] linePnts)
    {
        int cnt = linePnts.Length;
        float segmentDurtaion = renderAnimateDuration / cnt;

        lineRenderer.SetPosition(0, linePnts[0]);
        for (int i = 0; i < cnt - 1; i++)
        {
            float startTime = Time.time;

            Vector3 startPos = linePnts[i];
            Vector3 endPos = linePnts[i + 1];

            Vector3 pos = startPos;
            while (pos != endPos)
            {
                float t = (Time.time - startTime) / segmentDurtaion;
                pos = Vector3.Lerp(startPos, endPos, t);
                for (int j = i + 1; j < cnt; j++)
                {
                    lineRenderer.SetPosition(j, pos);
                }
                yield return null;
            }
        }
    }
}
