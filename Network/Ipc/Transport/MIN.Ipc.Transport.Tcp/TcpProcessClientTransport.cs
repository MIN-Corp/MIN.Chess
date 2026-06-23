using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using MIN.Ipc.Transport.Contracts.Enums;
using MIN.Ipc.Transport.Contracts.Interfaces;
using MIN.Ipc.Transport.Contracts.Models;

namespace MIN.Ipc.Transport.Tcp;

/// <inheritdoc cref="IProcessClientTransport"/> на основе Tcp loopback
public class TcpProcessClientTransport : IProcessClientTransport
{
    private readonly CancellationTokenSource cts = new();
    private readonly SemaphoreSlim writeLock = new(1, 1);
    private readonly SemaphoreSlim readLock = new(1, 1);
    private TcpClient? client;
    private NetworkStream? stream;

    /// <inheritdoc />
    public event EventHandler<ProcessClientTransportMessageEventArgs>? MessageReceived;

    async Task IProcessClientTransport.ConnectAsync(string connectionString, CancellationToken cancellationToken)
    {
        var info = JsonSerializer.Deserialize<ConnectionInfo>(connectionString);
        if (info?.Type != "tcp")
        {
            throw new ArgumentException("Expected tcp transport");
        }

        var parts = info.Value.Split(':');
        client = new TcpClient();
        await client.ConnectAsync(IPAddress.Loopback, int.Parse(parts[1]), cancellationToken);
        stream = client.GetStream();
        _ = ReadLoopAsync(cts.Token);
    }

    private async Task ReadLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            var lengthBuf = new byte[4];
            while (!cancellationToken.IsCancellationRequested)
            {
                byte[] body;

                await readLock.WaitAsync(cancellationToken);

                try
                {
                    await stream!.ReadExactlyAsync(lengthBuf, cancellationToken);
                    var length = BitConverter.ToInt32(lengthBuf);
                    body = new byte[length];
                    await stream.ReadExactlyAsync(body, cancellationToken);
                }
                finally
                {
                    readLock.Release();
                }

                MessageReceived?.Invoke(this, new ProcessClientTransportMessageEventArgs
                {
                    Data = body,
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ReadLoop error: {ex.Message}");
            return;
        }
    }

    async Task IProcessClientTransport.SendAsync(byte[] data, CancellationToken cancellationToken)
    {
        await writeLock.WaitAsync(cancellationToken);
        try
        {
            await stream!.WriteAsync(BitConverter.GetBytes(data.Length), cancellationToken);
            await stream!.WriteAsync(data, cancellationToken);
            await stream!.FlushAsync(cancellationToken);
        }
        finally
        {
            writeLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task DisconnectAsync()
    {
        cts.Cancel();
        cts.Dispose();
        client?.Dispose();
        stream?.Dispose();
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    async ValueTask IAsyncDisposable.DisposeAsync() => await DisconnectAsync();
}
