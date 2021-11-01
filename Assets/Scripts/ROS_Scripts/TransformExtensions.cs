using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.BuiltinInterfaces;
using UnityEngine;

public static class TransformExtensions
{

    public static TransformMsg ToROSTransform(this Transform tfUnity)
    {
        return new TransformMsg(
            // Using vector/quaternion To<>() because Transform.To<>() doesn't use localPosition/localRotation
            tfUnity.localPosition.To<FLU>(),
            tfUnity.localRotation.To<FLU>());
    }

    public static TransformStampedMsg ToROSTransformStamped(this Transform tfUnity, double time, string frame_id)
    {
        TimeStamp timeStamp = new TimeStamp(time);
        return new TransformStampedMsg(
            new HeaderMsg(Clock.GetCount(), new TimeMsg(timeStamp.Seconds, timeStamp.NanoSeconds), tfUnity.parent.gameObject.name),
            tfUnity.gameObject.name,
            tfUnity.ToROSTransform()
        );
    }
}
