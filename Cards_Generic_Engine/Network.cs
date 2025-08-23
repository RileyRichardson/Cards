using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cards_Generic_Engine {
	internal class Network {
		IPEndPoint EndPoint;
		int port;
		public event EventHandler UpdateReceived;
		Thread serverThread;
		Thread clientThread;
		private bool StayActive = true;
		public Network() {
			EndPoint = new IPEndPoint(IPAddress.Any, 5500);
			port = 5500;
		}
		public void EndNetworkThreads() {
			StayActive = false;
		}
		public String GetNetworkCode() {
			var host = Dns.GetHostEntry(Dns.GetHostName());
			string Ip = "";
			foreach (var ip in host.AddressList) {
				if (ip.AddressFamily == AddressFamily.InterNetwork) {
					Ip = ip.ToString();
				}
			}
			Ip += ":" + port.ToString();
			return Ip;
		}
		private TcpListener listener;
		public void StartServer() {
			listener = new TcpListener(IPAddress.Any,port);
			listener.Start();
			serverThread = new Thread(ServerLoop);
			serverThread.IsBackground = true;
			serverThread.Start();
		}
		//private void AcceptClients() {
		//	int next_client_num = 1;
		//	while (StayActive) {
		//		NetworkStream stream = listener.AcceptTcpClient().GetStream();
		//		ServerLoop(stream, next_client_num);
		//		next_client_num++;
		//	}
		//}
		//private async void ServerLoop(NetworkStream stream, int client_num) {
		//	await stream.WriteAsync(Encoding.UTF8.GetBytes(client_num.ToString()));
		//	while (StayActive) {
		//		if (stream.CanRead) {
		//			Memory<byte> msg = new();
		//			await stream.ReadAsync(msg);
		//			await stream.WriteAsync(msg);
		//		}
		//		Thread.Sleep(10);
		//	}
		//}

		private void ServerLoop() {
			List<TcpClient> handlers = [];
			int next_client_num = 1;
			while (StayActive) {
				for (int i = 0; i<handlers.Count; i++) {
					while(handlers[i].Available > 0) {
						byte[] msg_lngth_bffr = new byte[4];
						handlers[i].GetStream().Read(msg_lngth_bffr, 0, 4);
						byte[] msg = new byte[BitConverter.ToInt32(msg_lngth_bffr)];
						handlers[i].GetStream().Read(msg,0,msg.Length);
						Debug.WriteLine("msg from client: "+msg.ToString());
						for (int j = 0; j < handlers.Count; j++) {
							if (j==i) continue;
							handlers[i].GetStream().Write(msg_lngth_bffr,0,4);
							handlers[i].GetStream().Write(msg,0,msg.Length);
						}
					}
				}
				if (listener.Pending()) {
					TcpClient client = listener.AcceptTcpClient();
					handlers.Add(client);
					string msg = "I" + next_client_num.ToString();
					client.GetStream().Write(BitConverter.GetBytes(msg.Length),0,4);
					client.GetStream().Write(Encoding.UTF8.GetBytes(msg));
					next_client_num++;
					Debug.WriteLine("client accepted");
				}
				Thread.Sleep(10);
			}
		}
		private TcpClient client;
		public bool ConnectWithCode(String code) {
			if (!code.Contains(':')) {
				return false;
			}
			client = new (code.Split(":")[0], int.Parse(code.Split(":")[1]));
			if(!client.Connected)return false;
			clientThread = new Thread(ClientLoop);
			clientThread.Start();
			return true;
		}
		private void ClientLoop() {
			while (StayActive) {
				while (client.Available>0) {
					byte[] msg_lngth_bffr = new byte[4];
					client.GetStream().Read(msg_lngth_bffr,0,4);
					int msg_lngth = BitConverter.ToInt32(msg_lngth_bffr);
					byte[] msg = new byte[msg_lngth];
					int read = client.GetStream().Read(msg, 0, msg_lngth);
					UpdateReceived?.Invoke(Encoding.UTF8.GetString(msg), EventArgs.Empty);
				}
				Thread.Sleep(10);
			}
		}
		public void PostUpdate(object? sender, EventArgs e) {
			if (sender == null) return;
			int msg_length = ((string)sender).Length;
			client.GetStream().Write(BitConverter.GetBytes(msg_length),0,4);
			client.GetStream().Write(Encoding.UTF8.GetBytes((string)sender,0,msg_length));
		}
	}
}
