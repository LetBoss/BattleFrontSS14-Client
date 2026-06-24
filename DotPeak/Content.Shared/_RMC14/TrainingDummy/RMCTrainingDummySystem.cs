// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TrainingDummy.RMCTrainingDummySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.TrainingDummy;

public sealed class RMCTrainingDummySystem : EntitySystem
{
  [Dependency]
  private GrammarSystem _grammarSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCTrainingDummyComponent, ComponentStartup>(new EntityEventRefHandler<RMCTrainingDummyComponent, ComponentStartup>(this.OnStartup));
  }

  private void OnStartup(Entity<RMCTrainingDummyComponent> ent, ref ComponentStartup args)
  {
    if (ent.Comp.RemoveComponents != null)
      this.EntityManager.RemoveComponents(ent.Owner, ent.Comp.RemoveComponents);
    GrammarComponent comp;
    if (!this.TryComp<GrammarComponent>(ent.Owner, out comp))
      return;
    this._grammarSystem.SetGender((Entity<GrammarComponent>) (ent.Owner, comp), new Gender?(Gender.Neuter));
    this._grammarSystem.SetProperNoun((Entity<GrammarComponent>) (ent.Owner, comp), new bool?(false));
  }
}
