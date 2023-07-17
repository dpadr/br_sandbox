using System.Collections.Generic;
using Photon;

namespace BehaviorTree
{
    public class IsHost : Node
    {
        public IsHost() : base() { }

        public override NodeState Evaluate()
        {
            if (Photon.Pun.PhotonNetwork.IsMasterClient)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }

    }

}