using System;
using System.IO;
using System.IO.Compression;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Replays;

[NotContentImplementable]
public interface IReplayFileWriter
{
	ResPath BaseReplayPath { get; }

	void WriteBytes(ResPath path, ReadOnlyMemory<byte> bytes, CompressionLevel compressionLevel = CompressionLevel.Optimal);

	void WriteYaml(ResPath path, YamlDocument yaml, CompressionLevel compressionLevel = CompressionLevel.Optimal)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		MemoryStream memoryStream = new MemoryStream();
		using StreamWriter streamWriter = new StreamWriter(memoryStream);
		YamlStream val = new YamlStream();
		val.Add(yaml);
		val.Save((IEmitter)(object)new YamlMappingFix((IEmitter)new Emitter((TextWriter)streamWriter)), false);
		streamWriter.Flush();
		WriteBytes(path, memoryStream.AsMemory(), compressionLevel);
	}
}
