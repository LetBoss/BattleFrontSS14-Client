using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Shared.Administration;

public sealed class AdminCommandPermissions
{
	public readonly HashSet<string> AnyCommands = new HashSet<string>();

	public readonly Dictionary<string, AdminFlags[]> AdminCommands = new Dictionary<string, AdminFlags[]>();

	public void LoadPermissionsFromStream(Stream fs)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_007f: Expected O, but got Unknown
		using StreamReader reader = new StreamReader(fs, EncodingHelpers.UTF8);
		YamlStream val = new YamlStream();
		val.Load((TextReader)reader);
		YamlNode flagsNode = default(YamlNode);
		foreach (YamlMappingNode item in (YamlSequenceNode)val.Documents[0].RootNode)
		{
			IEnumerable<string> commands = ((IEnumerable<YamlNode>)YamlHelpers.GetNode<YamlSequenceNode>(item, "Commands")).Select((YamlNode p) => YamlHelpers.AsString(p));
			if (YamlHelpers.TryGetNode(item, "Flags", ref flagsNode))
			{
				AdminFlags flags = AdminFlagsHelper.NamesToFlags(YamlHelpers.AsString(flagsNode).Split(",", StringSplitOptions.RemoveEmptyEntries));
				foreach (string cmd in commands)
				{
					if (!AdminCommands.TryGetValue(cmd, out AdminFlags[] exFlags))
					{
						AdminCommands.Add(cmd, new AdminFlags[1] { flags });
						continue;
					}
					AdminFlags[] newArr = new AdminFlags[exFlags.Length + 1];
					exFlags.CopyTo(newArr, 0);
					newArr[^1] = flags;
					AdminCommands[cmd] = newArr;
				}
			}
			else
			{
				AnyCommands.UnionWith(commands);
			}
		}
	}

	public bool CanCommand(string cmdName, AdminData? admin)
	{
		if (AnyCommands.Contains(cmdName))
		{
			return true;
		}
		if (!AdminCommands.TryGetValue(cmdName, out AdminFlags[] flagsReq))
		{
			return false;
		}
		if (admin == null)
		{
			return false;
		}
		AdminFlags[] array = flagsReq;
		foreach (AdminFlags flagReq in array)
		{
			if (admin.HasFlag(flagReq))
			{
				return true;
			}
		}
		return false;
	}
}
