using UnityEngine;
using System.Collections;

public class SpinningChair : MonoBehaviour
{
    public float spinSpeed = 90.0f;
    private Quaternion initRotation;
    private bool isSpinning = false;

    void Awake()
    {
        initRotation = transform.rotation;
    }

    void Update()
    {
        if (isSpinning)
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        }
    }

    public void StartSpin()
    {
        isSpinning = true;
    }
    
    public void StopSpin()
    {
        isSpinning = false;
        transform.rotation = initRotation;
    }
}
