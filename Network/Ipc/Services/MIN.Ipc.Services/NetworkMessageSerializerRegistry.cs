using System.Text.Json;
using System.Text.Json.Nodes;
using MIN.Ipc.Services.Contracts.Interfaces;

namespace MIN.Ipc.Services;

/// <summary>
/// Регистр сообщений с типами для возможности их сериализации
/// </summary>
public sealed class NetworkMessageSerializerRegistry
{
    private readonly Dictionary<string, Type> typeMap = [];

    /// <summary>
    /// Зарегистрировать сообщение по типу сообщение в сети
    /// </summary>
    public void Register<T>(string? typeTag = null) where T : INetworkMessage
    {
        var instance = Activator.CreateInstance<T>();
        typeMap[typeTag ?? instance.Type] = typeof(T);
    }

    /// <summary>
    /// Зарегистрировать все сообщение с сборки
    /// </summary>
    /// <typeparam name="TAnchor">Якорь для определения сборки</typeparam>
    public void RegisterAllFromAnchor<TAnchor>()
    {
        var types = typeof(TAnchor).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(INetworkMessage))
                     && t is { IsAbstract: false, IsGenericType: false });

        foreach (var type in types)
        {
            var instance = (INetworkMessage)Activator.CreateInstance(type)!;
            typeMap[instance.Type] = type;
        }
    }

    internal byte[] Serialize<T>(string? typeTag, T message)
    {
        var type = message is INetworkMessage nm ? nm.Type : typeTag ?? string.Empty;

        if (!typeMap.TryGetValue(type, out var messageType))
        {
            throw new KeyNotFoundException($"Unknown message type: {type}");
        }

        var envelope = new JsonObject
        {
            ["type"] = type,
            ["data"] = JsonNode.Parse(JsonSerializer.Serialize(message, messageType)),
        };

        return JsonSerializer.SerializeToUtf8Bytes(envelope);
    }

    internal object? Deserialize(byte[] json)
    {
        using var doc = JsonDocument.Parse(json);
        var tag = doc.RootElement.GetProperty("type").GetString()!;
        var data = doc.RootElement.GetProperty("data");
        return typeMap.TryGetValue(tag, out var t)
            ? JsonSerializer.Deserialize(data, t)
            : null;
    }
}
