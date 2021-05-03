using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneDetect : MonoBehaviour
{

    public ARPlane m_ARPlane;
    public MeshRenderer m_PlaneMeshRenderer;
    public TextMesh m_TextMesh;
    public GameObject m_TextObj;
    GameObject m_mainCam;
    // Start is called before the first frame update
    void Start()
    {
        m_mainCam = FindObjectOfType<Camera>().gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateLabel();
        UpdatePlaneColor();
    }

    void UpdateLabel()
    {
        m_TextMesh.text = m_ARPlane.classification.ToString();
        m_TextObj.transform.position = m_ARPlane.center;
        m_TextObj.transform.LookAt(m_mainCam.transform);
        m_TextObj.transform.Rotate(new Vector3(0, 180, 0));
    }

    void UpdatePlaneColor() {
        Color planeMatColor = Color.cyan;
        switch(m_ARPlane.classification) {
            case PlaneClassification.None:
            planeMatColor = Color.cyan;
            break;
            case PlaneClassification.Wall:
            planeMatColor = Color.white;
            break;
            case PlaneClassification.Floor:
            planeMatColor = Color.green;
            break;
            case PlaneClassification.Ceiling:
            planeMatColor = Color.blue;
            break;
            case PlaneClassification.Table:
            planeMatColor = Color.yellow;
            break;
        }
        planeMatColor.a = 0.33f;
        m_PlaneMeshRenderer.material.color = planeMatColor;
    }
}
