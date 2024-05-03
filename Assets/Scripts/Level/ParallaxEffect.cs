using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform subject;
    public float yOffset;
    private Vector3 _startPostion;
    private float _startZ;



    // Start is called before the first frame update
    void Start()
    {
        _startPostion = transform.position;
        _startZ = transform.position.z;
    }

    // LateUpdate is called once per frame after Update
    void FixedUpdate()
    {
        if (subject == null) return; 

        Vector3 _travel = cam.transform.position - _startPostion;
        float DistanceFromSubject = transform.position.z - subject.position.z;
        float ClippingPlane =
           cam.transform.position.z + (DistanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane);
        float ParallaxFactor = Mathf.Abs(DistanceFromSubject) / ClippingPlane;


        Vector3 newPos = _startPostion + _travel * ParallaxFactor;
        transform.position = new Vector3(newPos.x, newPos.y - yOffset, _startZ);
    }
}
