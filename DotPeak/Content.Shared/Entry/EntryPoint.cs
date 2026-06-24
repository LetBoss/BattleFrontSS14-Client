// Decompiled with JetBrains decompiler
// Type: Content.Shared.Entry.EntryPoint
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

#nullable enable
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
    this._prototypeManager.PrototypesReloaded -= new Action<PrototypesReloadedEventArgs>(this.PrototypeReload);
  }

  public override void Init() => this.IgnorePrototypes();

  public override void PostInit()
  {
    base.PostInit();
    this.InitTileDefinitions();
    IoCManager.Resolve<MarkingManager>().Initialize();
  }

  private void InitTileDefinitions()
  {
    this._prototypeManager.PrototypesReloaded += new Action<PrototypesReloadedEventArgs>(this.PrototypeReload);
    this._tileDefinitionManager.Register((ITileDefinition) this._prototypeManager.Index<ContentTileDefinition>("Space"));
    List<ContentTileDefinition> contentTileDefinitionList = new List<ContentTileDefinition>();
    foreach (ContentTileDefinition enumeratePrototype in this._prototypeManager.EnumeratePrototypes<ContentTileDefinition>())
    {
      if (!(enumeratePrototype.ID == "Space"))
        contentTileDefinitionList.Add(enumeratePrototype);
    }
    contentTileDefinitionList.Sort((Comparison<ContentTileDefinition>) ((a, b) => string.Compare(a.ID, b.ID, StringComparison.Ordinal)));
    foreach (ITileDefinition tileDef in contentTileDefinitionList)
      this._tileDefinitionManager.Register(tileDef);
    this._tileDefinitionManager.Initialize();
  }

  private void PrototypeReload(PrototypesReloadedEventArgs obj)
  {
    foreach (ContentTileDefinition enumeratePrototype in this._prototypeManager.EnumeratePrototypes<ContentTileDefinition>())
      enumeratePrototype.AssignTileId(this._tileDefinitionManager[enumeratePrototype.ID].TileId);
  }

  private void IgnorePrototypes()
  {
    List<SequenceDataNode> sequence;
    if (!this.TryReadFile(out sequence))
      return;
    foreach (SequenceDataNode sequenceDataNode in sequence)
    {
      foreach (ValueDataNode valueDataNode in (IEnumerable<DataNode>) sequenceDataNode.Sequence)
      {
        ResPath path = new ResPath(valueDataNode.Value);
        if (string.IsNullOrEmpty(path.Extension))
          this._prototypeManager.AbstractDirectory(path);
        else
          this._prototypeManager.AbstractFile(path);
      }
    }
  }

  private bool TryReadFile([NotNullWhen(true)] out List<SequenceDataNode>? sequence)
  {
    sequence = new List<SequenceDataNode>();
    foreach (ResPath file in this._resMan.ContentFindFiles(new ResPath?(this._ignoreFileDirectory)))
    {
      Stream fileStream;
      if (this._resMan.TryContentFileRead(new ResPath?(file), out fileStream))
      {
        using (StreamReader reader = new StreamReader(fileStream, EncodingHelpers.UTF8))
        {
          DataNodeDocument dataNodeDocument = DataNodeParser.ParseYamlStream((TextReader) reader).FirstOrDefault<DataNodeDocument>();
          if (!(dataNodeDocument == (DataNodeDocument) null))
            sequence.Add((SequenceDataNode) dataNodeDocument.Root);
        }
      }
    }
    return true;
  }
}
