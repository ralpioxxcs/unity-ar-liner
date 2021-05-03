using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallObjectST : MonoBehaviour
{
    public List<GameObject> ballPrefab = new List<GameObject>();
    private GameObject nowBallObject;
    // public Transform ballObjectPool;
    public GameObject lineRendererPrefabs;
    private LineRenderer lineRenderer;

    // Ball의 시작점 결정
    public void SetObject(Vector3 pos) {
        Debug.LogFormat("SetObject -> pos : {0}",pos);
        if(nowBallObject != null) {
            Destroy(nowBallObject);
            nowBallObject = null;
        }

        GameObject tObj = null;
        tObj = ballPrefab[0];

        nowBallObject = Instantiate(tObj);
        Debug.Log("Instantiate");
        nowBallObject.transform.localScale = new Vector3(1,1,1);
        nowBallObject.transform.position = pos;
        // nowBallObject.transform.SetParent(ballObjectPool);

        lineRenderer.SetPosition(0, pos);
    }

    public void StartDrawLine(string index) {

    }
}
