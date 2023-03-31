# Multi-independent Render Streaming in Unity

This project demonstrates Unity Render Streaming with multiple independent streams, i.e. each with its own camera and input. This is mostly done by implementing a [custom Signaling Handler](Assets/MultiCameraSignalingHandler.cs).

### Limitations of the provided Broadcast Signaling Handler

Although you can add multiple camera streams to the provided [Broadcast Signaling Handler](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/api/Unity.RenderStreaming.Broadcast.html), the following limitations exist:

1. All clients share the same `InputReceiver`, so it is not possible to independently control a specific camera from different clients.
2. All active streams will be sent to all clients, e.g. if you have 3 streams and 3 clients, you will be sending 9 streams in total which is not efficient.

### How it works

Setup:
* Multiple Cameras are added to the Scene in an **inactive** state.
* Every Camera must have a `VideoStreamSender` and an optional `InputReceiver` Component. Refer to [Developing a streaming application](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/dev-streaming-app-intro.html) for more information, or the `StreamingCamera` prefab in the project.
* A custom Signaling Handler class ([MultiCameraSignalingHandler.cs](Assets/MultiCameraSignalingHandler.cs)) implements several methods for signal handling, such as `OnOffer(SignalingEventData)` to receive an SDP offer from a WebRTC client.

Important information:
* "Camera" in this document refers to the `GameObject` which contains the `Camera`, `VideoStreamSender` and the optional `InputReceiver` Components.
* The `active` state of the Camera is used to identify which cameras are available for an incoming stream to pick up. An active Camera means it is in use and will not be used for a new stream.
* The `name` of the Camera is used to identify which cameras are attached to a particular stream `connectionId`.

Signal Handling Process:
1. When a **connection offer** is received, `OnOffer(SignalingEventData)` is called:
    1. It finds an available Camera in the scene, i.e. one that is _inactive_.
    2. The `VideoStreamSender` Component is attached to the stream.
    3. The Camera is subsequently set to _active_.
    4. The `name` of the Camera is set to the `connectionId`.
2. When the client wishes to establish a data channel, such as for **keyboard or mouse input**, `OnAddChannel(SignalingEventData)` is called:
    1. The Camera with the same `connectionId` name is retrieved from the Scene
    2. Its `InputReceiver` Component is obtained and attached to the data channel.
3. When a connection **drops or disconnects**, `OnDeletedConnection(SignalingEventData)` or `OnDisconnect(SignalingEventData)` is called:
    1. The `active` state of the Camera is set to `false`, returning it back to the pool of available cameras for use.

_[MultiCameraSignalingHandler.cs](Assets/MultiCameraSignalingHandler.cs) class was adapated from the original source of [Unity.RenderStreaming.Broadcast](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/api/Unity.RenderStreaming.Broadcast.html) class._

### License

This project is licensed under the WTFPL, see http://www.wtfpl.net/.
