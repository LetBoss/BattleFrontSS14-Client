using System.IO;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;

namespace Robust.Shared.Console.Commands;

internal sealed class DumpSerializerTypeMapCommand : LocalizedCommands
{
	[Dependency]
	private readonly IRobustSerializerInternal _robustSerializer;

	public override string Command => "dump_netserializer_type_map";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		MemoryStream memoryStream = new MemoryStream();
		((RobustSerializer)_robustSerializer).GetHashManifest(memoryStream, writeNewline: true);
		memoryStream.Position = 0L;
		using StreamReader streamReader = new StreamReader(memoryStream);
		shell.WriteLine("Hash: " + _robustSerializer.GetSerializableTypesHashString());
		shell.WriteLine("Manifest:");
		while (true)
		{
			string text = streamReader.ReadLine();
			if (text != null)
			{
				shell.WriteLine(text);
				continue;
			}
			break;
		}
	}
}
