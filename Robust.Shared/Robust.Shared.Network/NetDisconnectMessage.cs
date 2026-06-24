using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Robust.Shared.Network;

public sealed class NetDisconnectMessage
{
	private const string LidgrenDisconnectedPrefix = "Disconnected: ";

	internal const string DefaultReason = "unknown reason";

	internal const bool DefaultRedialFlag = false;

	public const string ReasonKey = "reason";

	public const string RedialKey = "redial";

	internal readonly Dictionary<string, object> Values;

	public string Reason => StringOf("reason", "unknown reason");

	public bool RedialFlag => BoolOf("redial", defaultValue: false);

	internal NetDisconnectMessage(Dictionary<string, object> values)
	{
		Values = values;
	}

	internal NetDisconnectMessage(string reason = "unknown reason", bool redialFlag = false)
	{
		Values = new Dictionary<string, object>
		{
			{ "reason", reason },
			{ "redial", redialFlag }
		};
	}

	internal static NetDisconnectMessage Decode(string text)
	{
		ReadOnlyMemory<char> json = text.AsMemory().TrimStart();
		if (json.Span.StartsWith("Disconnected: ".AsSpan()))
		{
			int length = "Disconnected: ".Length;
			json = json.Slice(length, json.Length - length);
		}
		if (json.Span.StartsWith("{".AsSpan()))
		{
			try
			{
				using JsonDocument jsonDocument = JsonDocument.Parse(json);
				return JsonToReason(jsonDocument.RootElement);
			}
			catch (Exception)
			{
			}
		}
		return new NetDisconnectMessage(new Dictionary<string, object> { { "reason", text } });
	}

	internal string Encode()
	{
		return JsonSerializer.Serialize(Values);
	}

	private static NetDisconnectMessage JsonToReason(JsonElement obj)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (JsonProperty item in obj.EnumerateObject())
		{
			object value;
			switch (item.Value.ValueKind)
			{
			case JsonValueKind.String:
				value = item.Value.GetString();
				break;
			case JsonValueKind.Number:
			{
				value = ((!item.Value.TryGetInt32(out var value2)) ? ((object)item.Value.GetSingle()) : ((object)value2));
				break;
			}
			case JsonValueKind.True:
			case JsonValueKind.False:
				value = item.Value.GetBoolean();
				break;
			default:
				continue;
			}
			dictionary[item.Name] = value;
		}
		return new NetDisconnectMessage(dictionary);
	}

	public object? ValueOf(string key)
	{
		return Values.GetValueOrDefault(key);
	}

	[return: NotNullIfNotNull("defaultValue")]
	public string? StringOf(string key, string? defaultValue = null)
	{
		if (!(ValueOf(key) is string result))
		{
			return defaultValue;
		}
		return result;
	}

	public bool? BoolOf(string key)
	{
		return ValueOf(key) as bool?;
	}

	public bool BoolOf(string key, bool defaultValue)
	{
		return BoolOf(key) ?? defaultValue;
	}

	public int? Int32Of(string key)
	{
		return ValueOf(key) as int?;
	}

	public int Int32Of(string key, int defaultValue)
	{
		return Int32Of(key) ?? defaultValue;
	}

	public float? SingleOf(string key)
	{
		object obj = ValueOf(key);
		return (obj as float?) ?? ((float?)(obj as int?));
	}

	public float SingleOf(string key, float defaultValue)
	{
		return SingleOf(key) ?? defaultValue;
	}
}
