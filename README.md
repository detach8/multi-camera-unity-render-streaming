# Multi Independent Render Streaming in Unity

This project demonstrates how multiple independent Render Streaming streams (i.e. each with its own camera and input) can be achieved by implementing your own Signaling Handler.

The `CameraSignalingHandler` class was adapated from the original source of `Unity.RenderStreaming.Broadcast` class.

Although you can add multiple camera streams to the provided `Broadcast` Signaling Handler, the following limitations exist:

1. You can't use a separate `InputReceiver` per stream to independently control a specific camera
2. All active streams will all sent to all clients, i.e. if you have 3 streams and 3 clients, you will be sending 9 streams in total
