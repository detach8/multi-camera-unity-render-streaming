using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine;

public class MultiCameraSignalingHandler : SignalingHandlerBase, IOfferHandler, IAddChannelHandler, IDisconnectHandler, IDeletedConnectionHandler,
        IAddReceiverHandler
{
    private List<string> connectionIds = new List<string>();

    /// <summary>
    /// GameObject that has Cameras under its heirarchy
    /// </summary>
    public GameObject CameraGroup;

    /// <summary>
    /// Get an available GameObject that contains a VideoStreamSender comonent
    /// that is not in use, i.e. not active and enabled
    /// </summary>
    /// <returns>GameObject that has a VideoStreamSender component</returns>
    private GameObject GetNextAvailableVideoStreamSender()
    {
        var items = CameraGroup.GetComponentsInChildren<VideoStreamSender>(true);

        foreach (var item in items)
        {
            if (!item.isActiveAndEnabled) return item.gameObject;
        }

        return null;
    }

    /// <summary>
    /// Find an *active* GameObject that contains a VideoStreamSender component
    /// that has the same name as the connectionId
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns>GameObject that has a VideoStreamSender component</returns>
    private GameObject FindActiveVideoStreamSender(string connectionId)
    {
        var items = CameraGroup.GetComponentsInChildren<VideoStreamSender>();

        foreach (var item in items)
        {
            if (item.gameObject.name == connectionId) return item.gameObject;
        }

        return null;
    }

    /// <summary>
    /// Find an *active* GameObject that contains a InputReciver component
    /// that has the same name as the connectionId
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns>GameObject that has an InputReceiver component</returns>
    private GameObject FindActiveInputReceiver(string connectionId)
    {
        var items = CameraGroup.GetComponentsInChildren<InputReceiver>();

        foreach (var item in items)
        {
            if (item.gameObject.name == connectionId) return item.gameObject;
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

        // Find and disable the game object
        FindActiveVideoStreamSender(connectionId)?.SetActive(false);

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
        
        var gameObject = GetNextAvailableVideoStreamSender();
        if (gameObject == null)
        {
            Debug.Log((object)($"Sorry, no more streams available to assign to connectionId : {data.connectionId}"));
            return;
        }

        Debug.Log((object)($"Found an available stream, adding to connectionId : {data.connectionId}"));
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
        var camera = FindActiveInputReceiver(data.connectionId);
        if (camera == null)
        {
            Debug.Log((object)($"Could not find an InputReceiver for connectionId : {data.connectionId}"));
            return;
        }

        Debug.Log((object)($"Attaching input receiver to connectionId : {data.connectionId}"));
        var receiver = camera.GetComponent<InputReceiver>();
        receiver?.SetChannel(data.connectionId, data.channel);
    }
}
