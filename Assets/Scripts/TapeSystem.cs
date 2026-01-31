using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TapePoint
{
    public TapePoint(Transform pointTransform, Vector3 normal, GameObject hitObject)
    {
        this.pointTransform = pointTransform;
        this.normal = normal;
        this.hitObject = hitObject;
    }
    public Vector3 getPoint()
    {
        return pointTransform.position;
    }
    public Transform pointTransform;
    public Vector3 normal;
    public GameObject hitObject;
}

public class TapeSystem : MonoBehaviour
{
    public GameObject playerCamera;
    public float tapePartLength = .5f;
    public float tapeRange = 6.0f;
    public float tapeDistanceFromHit = .05f;
    public GameObject tapePartPrefab;

    private InputAction tapeAction;
    private TapePoint prevTapePoint = null;
    private int tapeable_layer = 0;
    private List<GameObject> tapeParts = new List<GameObject>();
    private float edgeScanRotateAngle = 5.0f;
    GameObject tapeStartPoint = null;

    void Awake()
    {
        tapeAction = InputSystem.actions.FindAction("Tape");
        tapeable_layer = 1 << LayerMask.NameToLayer("Tapeable");
    }

    GameObject createTapePart(TapePoint from, Vector3 toPoint, Vector3 toNormal)
    {
        float tapeLength = (toPoint - from.getPoint()).magnitude;
        GameObject tapePart = Instantiate(tapePartPrefab);
        Transform tapeChildTransform = tapePart.transform.GetChild(0);
        tapeChildTransform.localScale = new Vector3(tapeChildTransform.localScale.x,
                                                    tapeChildTransform.localScale.y,
                                                    tapeLength * .5f);
        tapeChildTransform.localPosition = new Vector3(.0f, .0f, tapeChildTransform.localScale.z * 1.0f);

        tapePart.transform.position = prevTapePoint.getPoint();
        tapePart.transform.LookAt(toPoint, toNormal);
        tapePart.transform.SetParent(from.hitObject.transform, true);
        
        tapeParts.Add(tapePart);
        return tapePart;
    }

    void Update()
    {
        if (tapeAction.IsPressed())
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 2.0f);
            
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, tapeRange))
            {
                Vector3 newTapePoint = hit.point + hit.normal * tapeDistanceFromHit;
                Vector3 newTapePointNormal = hit.normal;

                if (prevTapePoint != null)
                {
                    Vector3 tapeDir = newTapePoint - prevTapePoint.getPoint();
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
                        while (rotateAngle <= 90.0f && Physics.Raycast(prevTapePoint.getPoint(), scanDir1, distanceFromLast))
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
                            while (rotateAngle <= 90.0f && Physics.Raycast(newTapePoint, scanDir2, distanceFromLast))
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
                                Plane plane = new Plane(Vector3.Cross(rotateAxis1, scanDir1), prevTapePoint.getPoint());
                                Ray ray = new Ray(newTapePoint, scanDir2);
                                float intersectDist;
                                createEdgePoint = plane.Raycast(ray, out intersectDist);
                                if (createEdgePoint)
                                {
                                    // Create the edge tape part.
                                    GameObject edgePart = createTapePart(prevTapePoint,
                                                                     newTapePoint + scanDir2 * intersectDist,
                                                                     prevTapePoint.normal);
                                    // Get the transform of the child object named "to" of the tape part prefab.
                                    // (It's the second child)
                                    Transform edgeToTransform = edgePart.transform.GetChild(0).GetChild(1);
                                    prevTapePoint = new TapePoint(edgeToTransform,
                                                                  prevTapePoint.normal,
                                                                  hit.collider.gameObject);
                                }
                            }
                        }

                        GameObject part = createTapePart(prevTapePoint, newTapePoint, newTapePointNormal);
                        // Get the transform of the child object named "to" of the tape part prefab.
                        // (It's the second child)
                        Transform toTransform = part.transform.GetChild(0).GetChild(1);
                        prevTapePoint = new TapePoint(toTransform,
                                                      newTapePointNormal,
                                                      hit.collider.gameObject);
                    }
                }
                else
                {
                    tapeStartPoint = new GameObject();
                    tapeStartPoint.transform.position = newTapePoint;
                    tapeStartPoint.transform.SetParent(hit.collider.transform);
                    prevTapePoint = new TapePoint(tapeStartPoint.transform,
                                                  newTapePointNormal,
                                                  hit.collider.gameObject);
                }
            }
            else
            {
                Destroy(tapeStartPoint);
                prevTapePoint = null;
            } 
        }
        else
        {
            Destroy(tapeStartPoint);
            prevTapePoint = null;
        }
    }
}
