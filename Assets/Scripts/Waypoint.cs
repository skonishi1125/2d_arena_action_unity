using UnityEngine;
using UnityEngine.SceneManagement;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private string transferToScene;
    [SerializeField] private RespawnType waypointType;
    //[SerializeField] private RespawnType connectedWaypoint;
    //[SerializeField] private bool canBeTriggered = true;

    private void OnValidate()
    {
        gameObject.name = "Waypoint - " + waypointType.ToString() + " - " + transferToScene;

        //if (waypointType == RespawnType.Enter)
        //    connectedWaypoint = RespawnType.Exit;

        //if (waypointType == RespawnType.Exit)
        //    connectedWaypoint = RespawnType.Enter;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ゲームセーブ
        // Gamemanagerで、シーン遷移する
        SceneManager.LoadScene(transferToScene);
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    canBeTriggered = true;
    //}

}
