using MyEngine2D.Core.Physic;

namespace MyEngine2D.Core.Structure;

public class AABBNode
{
    public RigidBody Body;
    public AxisAlignedBoundingBox BoundingBox;

    public int ParentIndex;
    public int LeftChildIndex;
    public int RightChildIndex;
}