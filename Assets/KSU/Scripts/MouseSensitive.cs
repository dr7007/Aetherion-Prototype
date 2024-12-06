using Unity.Cinemachine;
using UnityEngine;

public class MouseSensitive : MonoBehaviour
{
    [SerializeField]
    private CinemachineOrbitalFollow mouseInput;
    [SerializeField]
    private float sensitive = 1f;

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitive;
        float mouseY = Input.GetAxis("Mouse Y") * sensitive;

        mouseInput.HorizontalAxis.Value += mouseX;
        mouseInput.VerticalAxis.Value -= mouseY;
    }
}
