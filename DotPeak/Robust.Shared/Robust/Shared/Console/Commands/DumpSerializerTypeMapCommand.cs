// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.DumpSerializerTypeMapCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using System.IO;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class DumpSerializerTypeMapCommand : LocalizedCommands
{
  [Dependency]
  private readonly IRobustSerializerInternal _robustSerializer;

  public override string Command => "dump_netserializer_type_map";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    MemoryStream memoryStream = new MemoryStream();
    ((RobustSerializer) this._robustSerializer).GetHashManifest((Stream) memoryStream, true);
    memoryStream.Position = 0L;
    using (StreamReader streamReader = new StreamReader((Stream) memoryStream))
    {
      shell.WriteLine("Hash: " + this._robustSerializer.GetSerializableTypesHashString());
      shell.WriteLine("Manifest:");
      while (true)
      {
        string text = streamReader.ReadLine();
        if (text != null)
          shell.WriteLine(text);
        else
          break;
      }
    }
  }
}
