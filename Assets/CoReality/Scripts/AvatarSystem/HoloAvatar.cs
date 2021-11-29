using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class HoloAvatar : MonoBehaviourPun
{
    private bool _local = true;

    /// <summary>
    /// Is this HoloAvatar local or remote?
    /// </summary>
    /// <value></value>
    public bool IsLocal
    {
        get => _local;
    }

    private Color _color;
    public Color Color
    {
        get => _color;
        set
        {
            _color = value;

            //RPC to update color to others if locally changed
            if (IsLocal)
            {
                photonView.RPC(
                    nameof(SetColorRPC),
                    RpcTarget.OthersBuffered,
                    new Vector3(_color.r, _color.g, _color.b)
                );
            }
        }
    }

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    #region RPCs

    [PunRPC]
    void SetColorRPC(Vector3 color)
    {
        Color = new Color(color.x, color.y, color.z);
    }

    #endregion

}
