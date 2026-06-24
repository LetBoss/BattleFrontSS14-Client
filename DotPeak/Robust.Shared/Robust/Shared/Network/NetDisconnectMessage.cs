// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetDisconnectMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

#nullable enable
namespace Robust.Shared.Network;

public sealed class NetDisconnectMessage
{
  private const string LidgrenDisconnectedPrefix = "Disconnected: ";
  internal const string DefaultReason = "unknown reason";
  internal const bool DefaultRedialFlag = false;
  public const string ReasonKey = "reason";
  public const string RedialKey = "redial";
  internal readonly Dictionary<string, object> Values;

  internal NetDisconnectMessage(Dictionary<string, object> values) => this.Values = values;

  internal NetDisconnectMessage(string reason = "unknown reason", bool redialFlag = false)
  {
    this.Values = new Dictionary<string, object>()
    {
      {
        nameof (reason),
        (object) reason
      },
      {
        "redial",
        (object) redialFlag
      }
    };
  }

  public string Reason => this.StringOf("reason", "unknown reason");

  public bool RedialFlag => this.BoolOf("redial", false);

  internal static NetDisconnectMessage Decode(string text)
  {
    ReadOnlyMemory<char> json = text.AsMemory().TrimStart();
    if (json.Span.StartsWith<char>("Disconnected: ".AsSpan()))
    {
      ref ReadOnlyMemory<char> local = ref json;
      int length = "Disconnected: ".Length;
      json = local.Slice(length, local.Length - length);
    }
    if (json.Span.StartsWith<char>("{".AsSpan()))
    {
      try
      {
        using (JsonDocument jsonDocument = JsonDocument.Parse(json))
          return NetDisconnectMessage.JsonToReason(jsonDocument.RootElement);
      }
      catch (Exception ex)
      {
      }
    }
    return new NetDisconnectMessage(new Dictionary<string, object>()
    {
      {
        "reason",
        (object) text
      }
    });
  }

  internal string Encode() => JsonSerializer.Serialize<Dictionary<string, object>>(this.Values);

  private static NetDisconnectMessage JsonToReason(JsonElement obj)
  {
    Dictionary<string, object> values = new Dictionary<string, object>();
    foreach (JsonProperty jsonProperty in obj.EnumerateObject())
    {
      object obj1;
      switch (jsonProperty.Value.ValueKind)
      {
        case JsonValueKind.String:
          obj1 = (object) jsonProperty.Value.GetString();
          break;
        case JsonValueKind.Number:
          int num;
          obj1 = !jsonProperty.Value.TryGetInt32(out num) ? (object) jsonProperty.Value.GetSingle() : (object) num;
          break;
        case JsonValueKind.True:
        case JsonValueKind.False:
          obj1 = (object) jsonProperty.Value.GetBoolean();
          break;
        default:
          continue;
      }
      values[jsonProperty.Name] = obj1;
    }
    return new NetDisconnectMessage(values);
  }

  public object? ValueOf(string key) => this.Values.GetValueOrDefault<string, object>(key);

  [return: NotNullIfNotNull("defaultValue")]
  public string? StringOf(string key, string? defaultValue = null)
  {
    return !(this.ValueOf(key) is string str) ? defaultValue : str;
  }

  public bool? BoolOf(string key) => this.ValueOf(key) as bool?;

  public bool BoolOf(string key, bool defaultValue) => this.BoolOf(key) ?? defaultValue;

  public int? Int32Of(string key) => this.ValueOf(key) as int?;

  public int Int32Of(string key, int defaultValue) => this.Int32Of(key) ?? defaultValue;

  public float? SingleOf(string key)
  {
    object obj = this.ValueOf(key);
    float? nullable1 = obj as float?;
    if (nullable1.HasValue)
      return nullable1;
    int? nullable2 = obj as int?;
    return !nullable2.HasValue ? new float?() : new float?((float) nullable2.GetValueOrDefault());
  }

  public float SingleOf(string key, float defaultValue) => this.SingleOf(key) ?? defaultValue;
}
