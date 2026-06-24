// Decompiled with JetBrains decompiler
// Type: Content.Shared.StoryGen.StoryGeneratorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Dataset;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.StoryGen;

public sealed class StoryGeneratorSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _protoMan;
  [Dependency]
  private IRobustRandom _random;

  public bool TryGenerateStoryFromTemplate(
    ProtoId<StoryTemplatePrototype> template,
    [NotNullWhen(true)] out string? story,
    int? seed = null)
  {
    StoryTemplatePrototype prototype1;
    if (!this._protoMan.TryIndex<StoryTemplatePrototype>(template, out prototype1))
    {
      story = (string) null;
      return false;
    }
    if (seed.HasValue)
      this._random.SetSeed(seed.Value);
    ValueList<(string, object)> valueList = new ValueList<(string, object)>(prototype1.Variables.Count);
    foreach ((string key, ProtoId<LocalizedDatasetPrototype> id) in prototype1.Variables)
    {
      LocalizedDatasetPrototype prototype2;
      if (this._protoMan.TryIndex<LocalizedDatasetPrototype>(id, out prototype2))
      {
        string str = this.Loc.GetString(RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) prototype2.Values));
        valueList.Add((key, (object) str));
      }
    }
    story = this.Loc.GetString((string) prototype1.LocId, valueList.ToArray());
    return true;
  }
}
