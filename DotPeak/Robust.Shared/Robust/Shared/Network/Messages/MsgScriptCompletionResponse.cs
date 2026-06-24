// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgScriptCompletionResponse
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace Robust.Shared.Network.Messages;

public sealed class MsgScriptCompletionResponse : NetMessage
{
  public ImmutableArray<MsgScriptCompletionResponse.LiteResult> Results;

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public int ScriptSession { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.ScriptSession = ((NetBuffer) buffer).ReadInt32();
    int num = ((NetBuffer) buffer).ReadInt32();
    ImmutableArray<MsgScriptCompletionResponse.LiteResult>.Builder builder = ImmutableArray.CreateBuilder<MsgScriptCompletionResponse.LiteResult>();
    for (int index = 0; index < num; ++index)
    {
      MsgScriptCompletionResponse.LiteResult liteResult = new MsgScriptCompletionResponse.LiteResult(buffer);
      builder.Add(liteResult);
    }
    this.Results = builder.ToImmutable();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.ScriptSession);
    ((NetBuffer) buffer).Write(this.Results.Length);
    foreach (MsgScriptCompletionResponse.LiteResult result in this.Results)
      result.WriteToBuffer(buffer);
  }

  public sealed class LiteResult
  {
    public string DisplayText;
    public string DisplayTextPrefix;
    public string DisplayTextSuffix;
    public string InlineDescription;
    public ImmutableArray<string> Tags;
    public ImmutableDictionary<string, string> Properties;

    public LiteResult(
      string displayText,
      string displayTextPrefix,
      string displayTextSuffix,
      string inlineDescription,
      ImmutableArray<string> tags,
      ImmutableDictionary<string, string> properties)
    {
      this.DisplayText = displayText;
      this.DisplayTextPrefix = displayTextPrefix;
      this.DisplayTextSuffix = displayTextSuffix;
      this.InlineDescription = inlineDescription;
      this.Tags = tags;
      this.Properties = properties;
    }

    public LiteResult(NetIncomingMessage buffer)
    {
      this.DisplayText = ((NetBuffer) buffer).ReadString();
      this.DisplayTextPrefix = ((NetBuffer) buffer).ReadString();
      this.DisplayTextSuffix = ((NetBuffer) buffer).ReadString();
      this.InlineDescription = ((NetBuffer) buffer).ReadString();
      int num1 = ((NetBuffer) buffer).ReadInt32();
      ImmutableArray<string>.Builder builder1 = ImmutableArray.CreateBuilder<string>();
      for (int index = 0; index < num1; ++index)
        builder1.Add(((NetBuffer) buffer).ReadString());
      this.Tags = builder1.ToImmutable();
      int num2 = ((NetBuffer) buffer).ReadInt32();
      ImmutableDictionary<string, string>.Builder builder2 = ImmutableDictionary.CreateBuilder<string, string>();
      for (int index = 0; index < num2; ++index)
        builder2.Add(((NetBuffer) buffer).ReadString(), ((NetBuffer) buffer).ReadString());
      this.Properties = builder2.ToImmutable();
    }

    public void WriteToBuffer(NetOutgoingMessage buffer)
    {
      ((NetBuffer) buffer).Write(this.DisplayText);
      ((NetBuffer) buffer).Write(this.DisplayTextPrefix);
      ((NetBuffer) buffer).Write(this.DisplayTextSuffix);
      ((NetBuffer) buffer).Write(this.InlineDescription);
      ((NetBuffer) buffer).Write(this.Tags.Length);
      foreach (string tag in this.Tags)
        ((NetBuffer) buffer).Write(tag);
      ((NetBuffer) buffer).Write(this.Properties.Count);
      foreach (KeyValuePair<string, string> property in this.Properties)
      {
        ((NetBuffer) buffer).Write(property.Key);
        ((NetBuffer) buffer).Write(property.Value);
      }
    }
  }
}
