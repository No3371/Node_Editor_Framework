using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class ConnectionManager {

		public static Dictionary<string, ConnectionData> connectionData;

		public static string ConnectionHash (ConnectionPort from, ConnectionPort to)
		{
			if (from.GetInstanceID() > to.GetInstanceID()) return from.GetInstanceID().ToString() + to.GetInstanceID().ToString();
			else return to.GetInstanceID().ToString() + from.GetInstanceID().ToString();
		}

		public class ConnectionData
		{
			public bool isFocused = false;

			public ConnectionPort from { get; }

			public ConnectionPort to { get; }

			public ConnectionData (ConnectionPort from, ConnectionPort to)
			{
				this.from = from;
				this.to = to;
			}

			public void Deconnect ()
			{
				from.RemoveConnection(to);
			}
		}

		public static void Deconnect (ConnectionPort port1, ConnectionPort port2)
		{
			port1.RemoveConnection(port2);
		}

		public static void Deconnect (string hash)
		{
			connectionData[hash].Deconnect();
		}

		public static void LoadCanvasCallback (NodeCanvas canvas)
		{
			if (connectionData == null) connectionData = new Dictionary<string, ConnectionData>();
	        else connectionData.Clear();
			foreach (Node node in canvas.nodes)
			{
				foreach (ConnectionPort port in node.outputPorts)
				{
					foreach (ConnectionPort target in port.connections)
					{
						ConnectionManager.connectionData.Add(ConnectionManager.ConnectionHash(port, target), new ConnectionData(port, target));
						// Debug.Log("Hash: " + ConnectionHash(port, target));
						// Debug.Log("Connection: " + port + " to " + target);
					}
				}
			}
		}

		public static void AddConnectionCallback (ConnectionPort port1, ConnectionPort port2)
		{
			if (connectionData.ContainsKey(ConnectionHash(port1, port2))) {}
			else if (!connectionData.ContainsKey(ConnectionHash(port1, port2))) connectionData.Add(ConnectionHash(port1, port2), new ConnectionData(port1, port2));
						// Debug.Log("Connection Added: " + port1 + " to " + port2);
		}

		public static void RemoveConnectionCallback (ConnectionPort port1, ConnectionPort port2)
		{
			if (!connectionData.ContainsKey(ConnectionHash(port1, port2)))  {}
			else if (connectionData.ContainsKey(ConnectionHash(port1, port2))) connectionData.Remove(ConnectionHash(port1, port2));
						// Debug.Log("Connection Removed: " + port1 + " to " + port2);
		}

		public static void Hook ()
		{
			NodeEditorCallbacks.OnLoadCanvas -= LoadCanvasCallback;
			NodeEditorCallbacks.OnAddConnection -= AddConnectionCallback;
			NodeEditorCallbacks.OnRemoveConnection -= RemoveConnectionCallback;
			NodeEditorCallbacks.OnLoadCanvas += LoadCanvasCallback;
			NodeEditorCallbacks.OnAddConnection += AddConnectionCallback;
			NodeEditorCallbacks.OnRemoveConnection += RemoveConnectionCallback;
		}
	}
}