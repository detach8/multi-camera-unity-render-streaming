# Multi-independent Render Streaming in Unity

This project demonstrates how to use Unity Render Streaming with multiple independent streams (i.e. each with its own camera and input) can be achieved by implementing a custom Signaling Handler.

### Limitations of the provided Broadcast Signaling Handler

Although you can add multiple camera streams to the provided `Broadcast` Signaling Handler, the following limitations exist:

1. You can't use a separate `InputReceiver` per stream to independently control a specific camera.
2. All active streams will be sent to all clients, i.e. if you have 3 streams and 3 clients, you will be sending 9 streams in total which is not very efficient.

### How it works

1. Multiple Cameras are added to the scene in an inactive state. The state is important as it is used to identify which cameras are available for an incoming stream to pick up later. An active Camera means it is in use.
2. Every `Camera` should have a `VideoStreamSender` and `InputReceiver` Component set up. Refer to [Developing a streaming application](https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/dev-streaming-app-intro.html) for more information.
3. A custom Signaling Handler class (here `CameraSignalingHandler`) implements the `OnOffer(SignalingEventData)` method to receive an SDP offer from a WebRTC client.
4. When `OnOffer(SignalingEventData)` is called, it finds an available Camera in the scene, i.e. one that is inactive.
5. The `VideoStreamSender` Component is obtained from the Camera and attached to the stream.
6. At the same time, the name of the Camera is set to the `connectionId` so it can be referenced later.
7. The `OnAddChannel(SignalingEventData)` is called when the client wishes to establish a data channel, such as for keyboard or mouse input.
8. The Camera with the same `connectionId` name is retrieved from the scene, and its `InputReceiver` Component is obtained and attached to the data channel.
9. When a connection drops or disconnects, the `active` state of the Camera is set to `false`.

The `CameraSignalingHandler` class was adapated from the original source of `Unity.RenderStreaming.Broadcast` class.

### License

This project is licensed under the WTFPL, see http://www.wtfpl.net/.
