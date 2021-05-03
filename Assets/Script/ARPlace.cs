using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlace : MonoBehaviour
{
    public ARRaycastManager aRRaycaster;
    public GameObject placeObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdaetCenterObject();
    }

    void UpdaetCenterObject() {
        Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        aRRaycaster.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        if(hits.Count > 0) {
            Pose placementPose = hits[0].pose;
            placeObject.SetActive(true);
            placeObject.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        // } else {
        //     placeObject.SetActive(false);
        // }

    }
}
