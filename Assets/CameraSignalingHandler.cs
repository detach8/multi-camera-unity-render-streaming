using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine;

public class CameraSignalingHandler : SignalingHandlerBase, IOfferHandler, IAddChannelHandler, IDisconnectHandler, IDeletedConnectionHandler,
        IAddReceiverHandler
{
    private List<string> connectionIds = new List<string>();

    /// <summary>
    /// GameObject that has Cameras under its heirarchy
    /// </summary>
    public GameObject CameraGroup;

    /// <summary>
    /// Get a camera that is not in use (i.e. not active and enabled)
    /// </summary>
    /// <returns>GameObject that has the Camera component</returns>
    private GameObject GetAvailableCamera()
    {
        var cameras = CameraGroup.GetComponentsInChildren<Camera>(true);

        Debug.Log($"Looking through {cameras.Length} cameras");

        foreach (var c in cameras)
        {
            if (!c.isActiveAndEnabled) return c.gameObject;
        }

        return null;
    }

    /// <summary>
    /// Find an active camera that is assigned to this connectionId
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns>GameObject that has the Camera component</returns>
    private GameObject FindCamera(string connectionId)
    {
        var cameras = CameraGroup.GetComponentsInChildren<Camera>();

        foreach (var c in cameras)
        {
            if (c.gameObject.name == connectionId) return c.gameObject;
        }

        return null;
    }

    public void OnDeletedConnection(SignalingEventData eventData)
    {
        Disconnect(eventData.connectionId);
    }

    public void OnDisconnect(SignalingEventData eventData)
    {
        Disconnect(eventData.connectionId);
    }

    private void Disconnect(string connectionId)
    {
        if (!connectionIds.Contains(connectionId))
        {
            return;
        }

        // Find and disable the camera
        var camera = FindCamera(connectionId);
        if (camera != null) camera.SetActive(false);

        connectionIds.Remove(connectionId);
    }

    public void OnAddReceiver(SignalingEventData data)
    {
        //MediaStreamTrack track = data.transceiver.Receiver.Track;
    }

    public void OnOffer(SignalingEventData data)
    {
        if (connectionIds.Contains(data.connectionId))
        {
            Debug.Log((object)("Already answered this connectionId : " + data.connectionId));
            return;
        }
        connectionIds.Add(data.connectionId);
        
        var gameObject = GetAvailableCamera();
        if (gameObject == null)
        {
            Debug.Log("No more cameras available!");
            return;
        }

        Debug.Log($"Found an available camera, setting up for connectionId : {data.connectionId}");
        gameObject.name = data.connectionId;
        gameObject.SetActive(true);

        // Randomly place the camera to create different views
        var camera = gameObject.GetComponent<Camera>();
        camera.transform.position = Random.insideUnitSphere * 5.0f;
        camera.transform.LookAt(Vector3.zero);

        AddSender(data.connectionId, gameObject.GetComponent<VideoStreamSender>());
        SendAnswer(data.connectionId);
    }

    public void OnAddChannel(SignalingEventData data)
    {
        var camera = FindCamera(data.connectionId);
        if (camera != null)
        {
            Debug.Log($"Adding input receiver for connectionId : {data.connectionId}");
            var receiver = camera.GetComponent<InputReceiver>();
            receiver.SetChannel(data.connectionId, data.channel);
        }
    }
}
