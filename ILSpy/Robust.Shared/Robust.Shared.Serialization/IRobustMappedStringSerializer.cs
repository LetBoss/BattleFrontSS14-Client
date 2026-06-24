using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using NetSerializer;
using Robust.Shared.Network;
using Robust.Shared.Serialization.Markdown;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Serialization;

internal interface IRobustMappedStringSerializer
{
	bool Locked { get; }

	ITypeSerializer TypeSerializer { get; }

	ReadOnlySpan<byte> MappedStringsHash { get; }

	bool EnableCaching { get; set; }

	event Action? ClientHandshakeComplete;

	Task Handshake(INetChannel channel);

	void AddString(string str);

	void AddStrings(Assembly asm);

	void AddStrings(YamlStream yaml);

	void AddStrings(DataNode dataNode);

	void AddStrings(IEnumerable<string> strings);

	void LockStrings();

	void Initialize();

	(byte[] mapHash, byte[] package) GeneratePackage();

	void SetPackage(byte[] hash, byte[] package);
}
