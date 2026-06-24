// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.SerializeStatsCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class SerializeStatsCommand : LocalizedCommands
{
  [Dependency]
  private readonly IRobustSerializerInternal _robustSerializer;

  public override string Command => "szr_stats";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    shell.WriteLine($"serialized: {this._robustSerializer.BytesSerialized} bytes, {this._robustSerializer.ObjectsSerialized} objects");
    shell.WriteLine($"largest serialized: {this._robustSerializer.LargestObjectSerializedBytes} bytes, {this._robustSerializer.LargestObjectSerializedType} objects");
    shell.WriteLine($"deserialized: {this._robustSerializer.BytesDeserialized} bytes, {this._robustSerializer.ObjectsDeserialized} objects");
    shell.WriteLine($"largest serialized: {this._robustSerializer.LargestObjectDeserializedBytes} bytes, {this._robustSerializer.LargestObjectDeserializedType} objects");
  }
}
