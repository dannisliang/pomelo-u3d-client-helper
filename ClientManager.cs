using System;
using Pomelo.DotNetClient;
using SimpleJson;

public class ClientManager {

	private static PomeloClient _client = null;
	private static ClientManager _instance = null;

	private string _gateHost = null;
	private int _gatePort;

	private string _connectorHost = null;
	private int _connectorPort;

	private int status;

	private UnityThreading.Dispatcher dispatcher;

	public static ClientManager Instance() {
		if (_instance == null) {
			_instance = new ClientManager();
		}
		return _instance;
	}

	private ClientManager() {

	}

	// gate host port
	public void Init(string gateHost, int gatePort) {
		_gateHost = gateHost;
		_gatePort = gatePort;
		_client = new PomeloClient (_gateHost, _gatePort);
		dispatcher = UnityThreadHelper.Dispatcher;
	}

	public void Connect(Action<JsonObject> action) {
		_client.connect((data)=>{
			if (!data.ContainsKey("code")) {
				_client.request("gate.gateHandler.queryEntry", (res)=>{
					_connectorHost = (string)res ["host"];
					_connectorPort = Convert.ToInt32(res["port"]);
					_client = new PomeloClient(this._connectorHost, this._connectorPort);
					_client.connect((_res)=>{
						if (_res.ContainsKey("code")) {
							dispatcher.Dispatch(()=>{
								action(_res);
							});
						}
						else {
							JsonObject msg = new JsonObject();
							msg.Add("code", Code.OK);
							dispatcher.Dispatch(()=>{
								action(msg);
							});
						}
					});
				});
			}
			else {
				action(data);
			}
		});
	}


	public void Request(string route, Action<JsonObject> action) {
		_client.request(route, new JsonObject(), action);
	}
	
	public void Request(string route, JsonObject msg, Action<JsonObject> action) {
		_client.request(route, msg, (jsonObject)=>{
			dispatcher.Dispatch(()=>{
				action(jsonObject);
			});
		});
	}

	public void Notify(string route, JsonObject msg) {
		_client.notify(route, msg);
	}

	public void On(string eventName, Action<JsonObject> action) {
		_client.on(eventName, (jsonObject)=>{
			dispatcher.Dispatch(()=>{
				action(jsonObject);
			});
		});
	}
}
