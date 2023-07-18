using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

namespace BehaviorTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node : IPunObservable
    {
        /** here's how we might handle the network update of the state **/
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting && PhotonNetwork.IsMasterClient) 
            {
                // We own this player: send the others our data
                stream.SendNext(state); // does this receive a reference or the actual object??
            }
            else
            {
                // Network player, receive data
                this.state = (NodeState)stream.ReceiveNext();
            }
            
        }
        
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

        private Dictionary<string, object> dataContext =
        new Dictionary<string, object>();
        public Node()
        {
            parent = null;
        }
        
        /** what's the deal with the default node evaluation set to failure? **/
        public virtual NodeState Evaluate() => NodeState.FAILURE;
        public Node(List<Node> children)
        {
            foreach (Node child in children)
                _Attach(child);
        }

        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }
        public void SetData(string key, object value)
        {
            dataContext[key] = value;
        }

        public object GetData(string key)
        {
            object val = null;
            if (dataContext.TryGetValue(key, out val))
                return val;

            Node node = parent;
            if (node != null)
                val = node.GetData(key);
            return val;
        }

        public bool ClearData(string key)
        {
            bool cleared = false;
            if (dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            if (node != null)
                cleared = node.ClearData(key);
            return cleared;
        }

        
    }

}