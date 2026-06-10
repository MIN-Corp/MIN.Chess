using System.Text.Json;
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

    internal string Serialize<T>(string? typeTag, T message)
    {
        var type = message is INetworkMessage networkMessage
            ? networkMessage.Type
            : typeTag ?? string.Empty;

        var json = JsonSerializer.Serialize(message, typeMap[type]);
        return $$"""{"type":"{{type}}","data":{{json}}}""";
    }

    internal object? Deserialize(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var tag = doc.RootElement.GetProperty("type").GetString()!;
        var data = doc.RootElement.GetProperty("data");
        return typeMap.TryGetValue(tag, out var t)
            ? (tag, JsonSerializer.Deserialize(data, t))
            : (tag, null);
    }
}
