using System.Text.Json;
using MIN.Ipc.Transport.Contracts.Interfaces;
using MIN.Ipc.Transport.Contracts.Models;
using MIN.Ipc.Transport.NamedPipes;
using MIN.Ipc.Transport.Tcp;

namespace MIN.Ipc.Services;

/// <summary>
/// Фабрика по получению междупроцессорного транспорта
/// </summary>
public static class SessionProcessTransportFactory
{
    /// <summary>
    /// Получить транспорт по типу строки подключения
    /// </summary>
    public static IProcessClientTransport Create(string connectionString)
    {
        var info = JsonSerializer.Deserialize<ConnectionInfo>(connectionString);
        return info?.Type switch
        {
            "pipe" => new NamedPipeProcessClientTransport(),
            "tcp" => new TcpProcessClientTransport(),
            _ => throw new ArgumentException($"Unknown transport: {info?.Type}")
        };
    }
}
