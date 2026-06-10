using System.IO.Pipes;
using System.Text.Json;
using MIN.Ipc.Transport.Contracts.Enums;
using MIN.Ipc.Transport.Contracts.Interfaces;
using MIN.Ipc.Transport.Contracts.Models;

namespace MIN.Ipc.Transport.NamedPipes;

/// <summary>
/// <inheritdoc cref="IProcessClientTransport"/>
/// </summary>
public sealed class NamedPipeProcessClientTransport : IProcessClientTransport
{
    private readonly CancellationTokenSource cts = new();
    private NamedPipeClientStream? pipe;

    /// <inheritdoc />
    public event EventHandler<ProcessClientTransportMessageEventArgs>? MessageReceived;

    async Task IProcessClientTransport.ConnectAsync(string connectionString, CancellationToken cancellationToken)
    {
        var info = JsonSerializer.Deserialize<ConnectionInfo>(connectionString);
        if (info?.Type != "pipe")
        {
            throw new ArgumentException("Expected pipe transport");
        }

        pipe = new NamedPipeClientStream(".", info.Value, PipeDirection.InOut);
        await pipe.ConnectAsync(5000, cancellationToken);
        _ = ReadLoopAsync(cts.Token);
    }

    private async Task ReadLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            var lengthBuf = new byte[4];
            while (!cancellationToken.IsCancellationRequested)
            {
                await pipe!.ReadExactlyAsync(lengthBuf, cancellationToken);
                var length = BitConverter.ToInt32(lengthBuf);
                var body = new byte[length];
                await pipe.ReadExactlyAsync(body, cancellationToken);

                MessageReceived?.Invoke(this, new ProcessClientTransportMessageEventArgs
                {
                    Data = body,
                });
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    async Task IProcessClientTransport.SendAsync(byte[] data, CancellationToken cancellationToken)
    {
        var len = BitConverter.GetBytes(data.Length);

        await pipe!.WriteAsync(len, cancellationToken);
        await pipe!.WriteAsync(data, cancellationToken);
        await pipe!.FlushAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DisconnectAsync()
    {
        cts.Cancel();
        cts.Dispose();
        pipe?.Dispose();
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    async ValueTask IAsyncDisposable.DisposeAsync() => await DisconnectAsync();
}

