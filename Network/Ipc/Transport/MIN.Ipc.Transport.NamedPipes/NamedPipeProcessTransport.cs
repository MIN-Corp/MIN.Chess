using System.IO.Pipes;
using System.Security.Principal;
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
    private readonly SemaphoreSlim writeLock = new(1, 1);
    private readonly SemaphoreSlim readLock = new(1, 1);
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

        if (!OperatingSystem.IsWindows())
        {
            throw new InvalidOperationException("Windows only");
        }

        pipe = new NamedPipeClientStream(".", info.Value, PipeDirection.InOut,
            PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
        await pipe.ConnectAsync(5000, cancellationToken);
        pipe.ReadMode = PipeTransmissionMode.Message;
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
                    await pipe!.ReadExactlyAsync(lengthBuf, cancellationToken);
                    var length = BitConverter.ToInt32(lengthBuf);
                    body = new byte[length];
                    await pipe.ReadExactlyAsync(body, cancellationToken);
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
            await pipe!.WriteAsync(BitConverter.GetBytes(data.Length), cancellationToken);
            await pipe!.WriteAsync(data, cancellationToken);
            await pipe!.FlushAsync(cancellationToken);
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
        pipe?.Dispose();
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    async ValueTask IAsyncDisposable.DisposeAsync() => await DisconnectAsync();
}

