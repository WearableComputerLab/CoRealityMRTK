using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// This is an example script that spawns a photonView for the network
/// </summary>
public class SpawnNetworkedObject : MonoBehaviour, IOnEventCallback
{
    /// <summary>
    /// The byte code for this spawn object event
    /// </summary>
    public const byte SPAWN_EVENT = 0x55;

    [SerializeField]
    private GrabbableView _objectToSpawn;

    void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    /// <summary>
    /// Inspector event binding
    /// </summary>
    public void Binding_SpawnObject()
    {
        SpawnObject(false, new Vector3(0, 0.5f, 0f));
    }

    /// <summary>
    /// Spawns the object either for remote users or the local user
    /// </summary>
    /// <param name="remote"></param>
    /// <param name="pos"></param>
    /// <param name="viewID"></param>
    private void SpawnObject(bool remote, Vector3 pos, int viewID = -1)
    {
        print("Spawning object");

        if (NetworkModule.Instance && PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            print("Got the the nextest part");

            GrabbableView go = Instantiate(_objectToSpawn);
            go.transform.SetParent(NetworkModule.NetworkOrigin);
            go.transform.localPosition = pos;
            if (remote)
            {
                go.photonView.ViewID = viewID;
            }
            else
            {
                if (PhotonNetwork.AllocateViewID(go.photonView))
                {
                    PhotonNetwork.RaiseEvent(
                        SPAWN_EVENT,
                        new object[]
                        {
                            go.transform.localPosition,
                            go.photonView.ViewID
                        },
                        new RaiseEventOptions
                        {
                            Receivers = ReceiverGroup.Others,
                            CachingOption = EventCaching.AddToRoomCache
                        },
                        new SendOptions
                        {
                            Reliability = true
                        }
                    );
                }
            }
        }
    }

    /// <summary>
    /// Photon callback that listens to raised events on the network
    /// </summary>
    /// <param name="photonEvent"></param>
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == SPAWN_EVENT)
        {
            var data = (object[])photonEvent.CustomData;
            SpawnObject(
                true,
                (Vector3)data[0],
                (int)data[1]
            );
        }
    }
}
