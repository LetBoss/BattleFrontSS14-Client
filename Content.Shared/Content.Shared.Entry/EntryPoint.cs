using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Content.Shared.Humanoid.Markings;
using Content.Shared.IoC;
using Content.Shared.Maps;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Utility;

namespace Content.Shared.Entry;

public sealed class EntryPoint : GameShared
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private ITileDefinitionManager _tileDefinitionManager;

	[Dependency]
	private IResourceManager _resMan;

	private readonly ResPath _ignoreFileDirectory = new ResPath("/IgnoredPrototypes/");

	public override void PreInit()
	{
		IoCManager.InjectDependencies<EntryPoint>(this);
		SharedContentIoC.Register();
	}

	public override void Shutdown()
	{
		_prototypeManager.PrototypesReloaded -= PrototypeReload;
	}

	public override void Init()
	{
		IgnorePrototypes();
	}

	public override void PostInit()
	{
		((GameShared)this).PostInit();
		InitTileDefinitions();
		IoCManager.Resolve<MarkingManager>().Initialize();
	}

	private void InitTileDefinitions()
	{
		_prototypeManager.PrototypesReloaded += PrototypeReload;
		ContentTileDefinition spaceDef = _prototypeManager.Index<ContentTileDefinition>("Space");
		_tileDefinitionManager.Register((ITileDefinition)(object)spaceDef);
		List<ContentTileDefinition> prototypeList = new List<ContentTileDefinition>();
		foreach (ContentTileDefinition tileDef in _prototypeManager.EnumeratePrototypes<ContentTileDefinition>())
		{
			if (!(tileDef.ID == "Space"))
			{
				prototypeList.Add(tileDef);
			}
		}
		prototypeList.Sort((ContentTileDefinition a, ContentTileDefinition b) => string.Compare(a.ID, b.ID, StringComparison.Ordinal));
		foreach (ContentTileDefinition tileDef2 in prototypeList)
		{
			_tileDefinitionManager.Register((ITileDefinition)(object)tileDef2);
		}
		_tileDefinitionManager.Initialize();
	}

	private void PrototypeReload(PrototypesReloadedEventArgs obj)
	{
		foreach (ContentTileDefinition def in _prototypeManager.EnumeratePrototypes<ContentTileDefinition>())
		{
			def.AssignTileId(_tileDefinitionManager[def.ID].TileId);
		}
	}

	private void IgnorePrototypes()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!TryReadFile(out List<SequenceDataNode> sequences))
		{
			return;
		}
		ResPath path = default(ResPath);
		foreach (SequenceDataNode item in sequences)
		{
			foreach (DataNode node in item.Sequence)
			{
				((ResPath)(ref path))._002Ector(((ValueDataNode)node).Value);
				if (string.IsNullOrEmpty(((ResPath)(ref path)).Extension))
				{
					_prototypeManager.AbstractDirectory(path);
				}
				else
				{
					_prototypeManager.AbstractFile(path);
				}
			}
		}
	}

	private bool TryReadFile([NotNullWhen(true)] out List<SequenceDataNode>? sequence)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		sequence = new List<SequenceDataNode>();
		Stream stream = default(Stream);
		foreach (ResPath path in _resMan.ContentFindFiles((ResPath?)_ignoreFileDirectory))
		{
			if (!_resMan.TryContentFileRead((ResPath?)path, ref stream))
			{
				continue;
			}
			using StreamReader reader = new StreamReader(stream, EncodingHelpers.UTF8);
			DataNodeDocument documents = DataNodeParser.ParseYamlStream((TextReader)reader).FirstOrDefault();
			if (!(documents == (DataNodeDocument)null))
			{
				sequence.Add((SequenceDataNode)documents.Root);
			}
		}
		return true;
	}
}
