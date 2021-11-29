using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.Input;

[RequireComponent(typeof(GrabbableView), typeof(NearInteractionGrabbable), typeof(PhotonView))]
/// <summary>
/// A basic example of a grabbable photon view for MRTK
/// </summary>
public class GrabbableView : MonoBehaviourPun, IMixedRealityPointerHandler, IPunOwnershipCallbacks, IPunObservable
{
    private PhotonStreamQueue _streamQueue = new PhotonStreamQueue(120);
    private GameObject _moveRef;
    [SerializeField]
    private int _owner = 0;

    void Update()
    {

        //Ensure in photon room else reset and return
        if (PhotonNetwork.InRoom == false)
        {
            _streamQueue.Reset();
            return;
        }

        //Serialize data if owner, else deserialize it
        if (_owner == PhotonNetwork.LocalPlayer.ActorNumber)
            this.SerializeData();
        else if (_streamQueue.HasQueuedObjects())
            this.DeserializeData();

    }

    /// <summary>
    /// Serializes the local position and rotation
    /// relative to the network origin
    /// </summary>
    private void SerializeData()
    {
        _streamQueue.SendNext(transform.localPosition);
        _streamQueue.SendNext(transform.localRotation);
    }

    /// <summary>
    /// Deserialise the local position and roation data
    /// </summary>
    private void DeserializeData()
    {
        transform.localPosition = (Vector3)_streamQueue.ReceiveNext();
        transform.localRotation = (Quaternion)_streamQueue.ReceiveNext();
    }

    /// <summary>
    /// Trys to get ownership of this object
    /// will always return false if _owner != 0
    /// </summary>
    /// <returns></returns>
    private bool TryGetOwnership()
    {
        if (_owner == 0)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            photonView.RPC(nameof(SetOwnerRPC), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Releases the ownership of this object
    /// _owner is set back to 0 (none)
    /// </summary>
    private void ReleaseOwnership()
    {
        photonView.RPC(nameof(SetOwnerRPC), RpcTarget.All, 0);
    }

    /// <summary>
    /// Serialize view callback
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting == true)
        {
            this._streamQueue.Serialize(stream);
        }
        else
        {
            this._streamQueue.Deserialize(stream);
        }
    }

    [PunRPC]
    /// <summary>
    /// Remote procedural call for setting the _owner
    /// </summary>
    /// <param name="owner"></param>
    private void SetOwnerRPC(int owner)
    {
        _owner = owner;
    }

    /// <summary>
    /// MRTK pointer down callback
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        if (eventData.Pointer is SpherePointer)
        {
            if (TryGetOwnership())
            {
                Transform pointer = ((SpherePointer)(eventData.Pointer)).transform;
                _moveRef = new GameObject("Move Reference");
                _moveRef.transform.SetParent(pointer);
                _moveRef.transform.position = transform.position;
                _moveRef.transform.rotation = transform.rotation;
            }
        }
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        if (eventData.Pointer is SpherePointer)
        {
            if (_owner == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                transform.position = _moveRef.transform.position;
                transform.rotation = _moveRef.transform.rotation;
            }
        }
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        if (eventData.Pointer is SpherePointer)
        {
            if (_owner == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                Destroy(_moveRef);
                ReleaseOwnership();
            }
        }
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    { }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    { }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    { }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    { }
}
