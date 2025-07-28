using TMPro;
using UnityEngine;

public class PlayerBillboard : MonoBehaviour
{
    

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_camera)
        {
            transform.forward = _camera.transform.forward;
        }
    }
}