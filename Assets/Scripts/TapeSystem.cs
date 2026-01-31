using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TapePoint
{
    public TapePoint(Vector3 point, Vector3 normal)
    {
        this.point = point;
        this.normal = normal;
    }
    public Vector3 point;
    public Vector3 normal;
}

public class TapeSystem : MonoBehaviour
{
    public GameObject playerCamera;
    public float tapePartLength = .5f;
    public float tapeRange = 6.0f;
    public float tapeDistanceFromHit = .05f;
    public GameObject tapePartPrefab;

    private InputAction tapeAction;
    private bool isTaping = false;
    private TapePoint prevTapePoint = null;
    private int tapeable_layer = 0;
    private List<GameObject> tapeParts = new List<GameObject>();
    private float edgeScanRotateAngle = 5.0f;

    void Awake()
    {
        tapeAction = InputSystem.actions.FindAction("Tape");
        tapeable_layer = 1 << LayerMask.NameToLayer("Tapeable");
    }

    void createTapePart(Vector3 from, TapePoint to)
    {
        float tapeLength = (to.point - from).magnitude;
        GameObject tapePart = Instantiate(tapePartPrefab);
        Transform tapeChildTransform = tapePart.transform.GetChild(0);
        tapeChildTransform.localScale = new Vector3(tapeChildTransform.localScale.x,
                                                    tapeChildTransform.localScale.y,
                                                    tapeLength * .5f);
        tapeChildTransform.localPosition = new Vector3(.0f, .0f, tapeChildTransform.localScale.z * 1.0f);

        tapePart.transform.position = prevTapePoint.point;
        tapePart.transform.LookAt(to.point, to.normal);
        
        tapeParts.Add(tapePart);
    }

    void Update()
    {
        if (tapeAction.IsPressed())
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 2.0f);
            
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, tapeRange))
            {
                TapePoint newTapePoint = new TapePoint(hit.point + hit.normal * tapeDistanceFromHit, hit.normal);
                if (prevTapePoint != null)
                {
                    Vector3 tapeDir = newTapePoint.point - prevTapePoint.point;
                    float distanceFromLast = tapeDir.magnitude;
                    tapeDir.Normalize();

                    if (distanceFromLast > tapePartLength)
                    {
                        bool createEdgePoint = false;
                        // Find scan dir 1.
                        // This is the direction that points in the general direction from the previous
                        // tape hit to the new one, and the ray in this direction doesn't intersect any edge.
                        Vector3 scanDir1 = tapeDir;
                        Vector3 rotateAxis1 = Vector3.Cross(scanDir1, prevTapePoint.normal);
                        float rotateAngle = edgeScanRotateAngle;
                        // This quaternion will rotate the scanDir1 away/out from the edge by edgeScanRotateAngle degrees.
                        Quaternion edgeScanRot = Quaternion.AngleAxis(edgeScanRotateAngle, rotateAxis1);
                        // Keep rotating the scanDir1 out from the edge until the ray in this direction doesn't hit anything anymore.
                        // Stop if we exceed 90 degrees angle.
                        while (rotateAngle <= 90.0f && Physics.Raycast(prevTapePoint.point, scanDir1, distanceFromLast))
                        {
                            scanDir1 = (edgeScanRot * scanDir1).normalized;
                            rotateAngle += edgeScanRotateAngle;
                            createEdgePoint = true;
                        }

                        if (createEdgePoint)
                        {
                            createEdgePoint = false;
                            // Find scan dir 2.
                            // This is the same this as scanDir1 above, but in the opposite direction,
                            // i.e. going from the new tape hit to the previous one.
                            Vector3 scanDir2 = -tapeDir;
                            Vector3 rotateAxis2 = Vector3.Cross(scanDir2, prevTapePoint.normal);
                            rotateAngle = edgeScanRotateAngle;
                            // This quaternion will rotate the scanDir2 away/out from the edge by edgeScanRotateAngle degrees.
                            // Stop if we exceed 90 degrees angle.
                            edgeScanRot = Quaternion.AngleAxis(edgeScanRotateAngle, rotateAxis2);
                            while (rotateAngle <= 90.0f && Physics.Raycast(newTapePoint.point, scanDir2, distanceFromLast))
                            {
                                scanDir2 = (edgeScanRot * scanDir2).normalized;
                                rotateAngle += edgeScanRotateAngle;
                                createEdgePoint = true;
                            }

                            if (createEdgePoint)
                            {
                                // Find the intersection between the 2 opposing edge scan direction (scanDir1/scanDir2).
                                // However, because intersecting two Vector3's is not a thing we convert one of the dirs to a plane,
                                // and we get the intersection between this plane and the other dir.
                                Plane plane = new Plane(Vector3.Cross(rotateAxis1, scanDir1), prevTapePoint.point);
                                Ray ray = new Ray(newTapePoint.point, scanDir2);
                                float intersectDist;
                                createEdgePoint = plane.Raycast(ray, out intersectDist);
                                if (createEdgePoint)
                                {    
                                    TapePoint edgeTapePoint = new TapePoint(newTapePoint.point + scanDir2 * intersectDist, prevTapePoint.normal);
                                    // Create the edge tape part.
                                    createTapePart(prevTapePoint.point, edgeTapePoint);
                                    prevTapePoint = edgeTapePoint;
                                }
                            }
                        }

                        createTapePart(prevTapePoint.point, newTapePoint);
                        prevTapePoint = newTapePoint;
                    }
                }
                else
                {
                    prevTapePoint = newTapePoint;
                }
            }
            else
            {
                prevTapePoint = null;
            }
        }
        else
        {
            prevTapePoint = null;
        }
    }
}
