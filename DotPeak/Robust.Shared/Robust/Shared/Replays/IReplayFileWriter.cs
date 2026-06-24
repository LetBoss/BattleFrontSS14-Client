// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Replays.IReplayFileWriter
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;
using System.IO;
using System.IO.Compression;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Replays;

[NotContentImplementable]
public interface IReplayFileWriter
{
  ResPath BaseReplayPath { get; }

  void WriteBytes(ResPath path, ReadOnlyMemory<byte> bytes, CompressionLevel compressionLevel = CompressionLevel.Optimal);

  void WriteYaml(ResPath path, YamlDocument yaml, CompressionLevel compressionLevel = CompressionLevel.Optimal)
  {
    MemoryStream ms = new MemoryStream();
    using (StreamWriter streamWriter = new StreamWriter((Stream) ms))
    {
      YamlStream yamlStream = new YamlStream();
      yamlStream.Add(yaml);
      yamlStream.Save((IEmitter) new YamlMappingFix((IEmitter) new Emitter((TextWriter) streamWriter)), false);
      streamWriter.Flush();
      this.WriteBytes(path, (ReadOnlyMemory<byte>) ms.AsMemory(), compressionLevel);
    }
  }
}
