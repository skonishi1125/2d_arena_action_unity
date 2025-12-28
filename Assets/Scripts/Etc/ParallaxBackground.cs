using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Camera mainCamera;
    private float lastCameraPositionX;
    private float cameraHalfWidth;

    [SerializeField] private ParallaxLayer[] backgroundLayers;

    private void Awake()
    {
        mainCamera = Camera.main;

        lastCameraPositionX = mainCamera.transform.position.x;
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        CalculateImageLength();
    }

    // Teleport等を使うと画面ががたつく
    // カメラ側もLateUpdateとして、こちらもLateで合わせる
    // そのあとUnity本体の実行順序を調整して、必ずカメラ移動の後、背景を移動させるようにする
    private void LateUpdate()
    {
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;

        float currentCameraPositionX = mainCamera.transform.position.x;
        float distanceToMove = currentCameraPositionX - lastCameraPositionX;
        lastCameraPositionX = currentCameraPositionX;

        float cameraLeftEdge = currentCameraPositionX - cameraHalfWidth;
        float cameraRightEdge = currentCameraPositionX + cameraHalfWidth;

        foreach(ParallaxLayer layer in backgroundLayers)
        {
            layer.Move(distanceToMove);
            layer.LoopBackground(cameraLeftEdge, cameraRightEdge);
        }

    }

    private void CalculateImageLength()
    {
        foreach (ParallaxLayer layer in backgroundLayers)
        {
            layer.CalculateImageWidth();
        }
    }


}
