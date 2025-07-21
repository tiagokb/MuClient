using System;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkConnection : MonoBehaviour
{
    public static NetworkConnection Instance { get; private set; }

    [SerializeField] private string serverIp = "127.127.127.127";
    [SerializeField] private int serverPort = 44406;

    private TcpClient _client;
    private NetworkStream _stream;
    private bool _isConnected = false;

    private readonly PacketParser _parser = new();
    private readonly PacketRouter _router = new();

    private CancellationTokenSource _listenTokenSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private async void Start()
    {
        await LoadingScreen.Instance.ShowStep("Carregando Dependencias...");
        RegisterHandlers();
        PacketDefinitionsRegistry.Instance.Load();

        await LoadingScreen.Instance.ShowStep("Conectando ao Servidor...");

        await ConnectToServer(serverIp, serverPort);

        await LoadingScreen.Instance.ShowStep("Carregando lista de servidores...");
    }

    private void RegisterHandlers()
    {
        var handlerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IPacketHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in handlerTypes)
        {
            if (Activator.CreateInstance(type) is IPacketHandler handler)
            {
                _router.RegisterHandler(handler);
                Debug.Log($"[Router] Registrado handler: {type.Name}");
            }
        }
    }

    public async Task ConnectToServer(string ip, int port)
    {
        await DisconnectAsync(); // garante que está limpo antes de conectar de novo

        try
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);

            if (_client.Connected)
            {
                _stream = _client.GetStream();
                _isConnected = true;
                Debug.Log($"[NetworkConnection] Conectado ao servidor {ip}:{port}");
                StartListening();
            }
            else
            {
                throw new Exception("Could not connect to server");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[Network Connection] Falha ao conectar: {e.Message}]");
        }
    }

    public void StartListening()
    {
        _listenTokenSource?.Cancel(); // Para qualquer escuta anterior

        _listenTokenSource = new CancellationTokenSource();
        _ = ListenToServer(_listenTokenSource.Token);
    }

    public async Task ListenToServer(CancellationToken token)
    {
        try
        {
            while (_isConnected && !token.IsCancellationRequested)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
                if (bytesRead == 0) break;

                byte[] packet = new byte[bytesRead];
                Array.Copy(buffer, 0, packet, 0, bytesRead);

                LogPacket(packet);
                var parsed = _parser.Parse(packet);
                _router.Route(parsed);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("[NetworkConnection] Leitura cancelada.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[NetworkConnection] Erro na leitura: {e.Message}");
        }
    }


    public async Task SendAsync(byte[] data)
    {
        if (_isConnected && _stream != null)
        {
            try
            {
                await _stream.WriteAsync(data, 0, data.Length);
                Debug.Log($"[NetworkConnection] Pacote Enviada: {data.Length} bytes");
            }
            catch (Exception e)
            {
                Debug.LogError($"[NetworkConnection] Erro ao enviar dados: {e.Message}");
            }
        }
    }

    public async Task DisconnectAsync()
    {
        try
        {
            OnApplicationQuit();
        }
        catch (Exception e)
        {
            Debug.LogError($"[NetworkConnection] Erro ao desconectar: {e.Message}");
        }

        await Task.Yield(); // Garante que não bloqueia o main thread
    }

    //
    private void LogPacket(byte[] packet)
    {
        StringBuilder hex = new StringBuilder();
        foreach (byte b in packet)
        {
            hex.Append($"{b:X2} ");
        }

        Debug.Log($"[Server -> Client] {hex.ToString().Trim()}");
    }

    //
    private void OnApplicationQuit()
    {
        _listenTokenSource?.Cancel(); // <--- isso garante o cancelamento limpo
        _isConnected = false;
        _stream?.Close();
        _stream?.Dispose();

        _client?.Close();
        _client?.Dispose();

        Debug.Log("[NetworkConnection] Desconectado com sucesso.");
    }
}