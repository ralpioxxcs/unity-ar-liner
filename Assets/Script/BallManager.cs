using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class BallManager : MonoBehaviour
{
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
        } else {
            m_ballEnable = false;
        }
    }

    public void MakeBallObject()
    {
        Debug.Log("MakeBallObject");
        if (m_ballEnable == true && m_ballSpawned == false)
        {
            Debug.Log("Instantiate");

            GameObject prefab = Resources.Load("Prefab/GolfBall") as GameObject;
            m_ballObject = Instantiate(prefab) as GameObject;
            m_ballObject.transform.SetParent(ballObjectPool);
            m_ballObject.transform.position = m_ballPos;
            m_ballObject.transform.localScale /= 3;

            m_ballPos = m_curPivtoPos;

            m_ballObject = null;
            
            m_ballSpawned = true;
        } else {
            m_ballSpawned = false;
        }
    }

    [SerializeField] private float renderAnimateDuration = 1f;
    public void DrawBallTrajectory()
    {
        Debug.Log("DrawBall First");
        if (m_ballSpawned)
        {
            string index = inputField.text;
            Debug.LogFormat("INPUT FIELD : {0}",index);
            if(index == null) {
                index = "1";
            }
            if(int.Parse(index) < 0 || int.Parse(index) > 5 ) {
                index = "1";
            }
            Debug.LogFormat("DrawBall (index : {0})", index);

            string filenamePrefix = "data_";
            string filename = filenamePrefix + index;
            TextAsset ResourceRequest = Resources.Load(filename) as TextAsset;
            RadarData rd = RadarData.CreateFromJSON(ResourceRequest.ToString());

            GameObject tLine = Instantiate(lineRendererPrefab);
            lineRenderer = tLine.GetComponent<LineRenderer>();
            lineRenderer.transform.position = m_ballPos;

            Vector3 lookatVec = (m_ballPos - mainCam.transform.position).normalized;
            lineRenderer.transform.rotation.SetLookRotation(lookatVec);
            // lineRenderer.transform.rotation.SetLookRotation(new Vector3(0, mainCam.transform.rotation.y, 0));
            lineRenderer.useWorldSpace = false;

            Debug.LogFormat("position: {0}", lineRenderer.transform.position);
            Debug.LogFormat("rotation : {0}", lineRenderer.transform.rotation);

            int pointCounts = rd.Index.Count;
            Vector3[] linePoints = new Vector3[pointCounts];
            lineRenderer.positionCount = pointCounts;

            for (int i = 0; i < pointCounts; i++)
            {
                // linePoints[i] = new Vector3(
                //     (float)rd.X[i]/10,
                //     (float)rd.Y[i]/10,
                //     (float)rd.Z[i]/10);
                linePoints[i] = new Vector3(
                    (float)rd.X[i]/10,
                    (float)rd.Y[i]/10,
                    (float)rd.Z[i]/10);
                // Debug.LogFormat("vector[{0}]: {1}", i, linePoints[i]);
            }
            StartCoroutine(AnimateLines(linePoints));
            
            m_ballSpawned = false;
        }
    }

    private IEnumerator AnimateLines(Vector3[] linePnts)
    {
        int cnt = linePnts.Length;
        float segmentDurtaion = renderAnimateDuration / cnt;

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
                for (int j = i; j < cnt; j++)
                {
                    lineRenderer.SetPosition(j, pos);
                }
                yield return null;
            }
        }
    }
}
