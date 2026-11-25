using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Rigidbody))]
public class DiceBehaviour : MonoBehaviour
{
    [Serializable]
    public struct DieFaces
    {
        public int value;
        public Vector3 localNormal;
    }

    [SerializeField]
    private DieFaces[] faces = new DieFaces[]
    {
        new DieFaces {value = 1, localNormal = Vector3.up},
        new DieFaces {value = 6, localNormal = Vector3.down},
        new DieFaces {value = 2, localNormal = Vector3.right},
        new DieFaces {value = 5, localNormal = Vector3.left},
        new DieFaces {value = 3, localNormal = Vector3.forward},
        new DieFaces {value = 4, localNormal = Vector3.back}
    };

    private Rigidbody _rb;
    public int topFaceValue = -1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public int GetTopFaceValue()
    {
        float maxDot = float.MinValue;
        int currentTopFaceValue = -1;
        foreach (var face in faces)
        {
            Vector3 worldNormal = transform.TransformDirection(face.localNormal);
            float dot = Vector3.Dot(worldNormal, Vector3.up);
            if (dot > maxDot)
            {
                maxDot = dot;
                currentTopFaceValue = face.value;
            }
        }
        return currentTopFaceValue;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TossDice(UnityEngine.Random.Range(2f, 4f), UnityEngine.Random.Range(5f, 10f));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(GetTopFaceValue());
        }
    }
    public void TossDice(float forceMagnitude, float torqueMagnitude)
    {
        _rb.AddForce(Vector3.up * forceMagnitude, ForceMode.Impulse);
        _rb.AddTorque(UnityEngine.Random.onUnitSphere * torqueMagnitude, ForceMode.Impulse);
    }


}
