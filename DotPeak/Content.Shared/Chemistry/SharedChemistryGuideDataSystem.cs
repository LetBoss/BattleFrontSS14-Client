// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.SharedChemistryGuideDataSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry;

public abstract class SharedChemistryGuideDataSystem : EntitySystem
{
  [Dependency]
  protected IPrototypeManager PrototypeManager;
  protected readonly Dictionary<string, ReagentGuideEntry> Registry = new Dictionary<string, ReagentGuideEntry>();

  public IReadOnlyDictionary<string, ReagentGuideEntry> ReagentGuideRegistry
  {
    get => (IReadOnlyDictionary<string, ReagentGuideEntry>) this.Registry;
  }

  public abstract void ReloadAllReagentPrototypes();
}
