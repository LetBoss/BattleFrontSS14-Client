// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Equipment.SharedArtifactCrusherSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.Storage.Components;
using Content.Shared.Xenoarchaeology.Equipment.Components;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Equipment;

public abstract class SharedArtifactCrusherSystem : EntitySystem
{
  [Dependency]
  protected SharedAppearanceSystem Appearance;
  [Dependency]
  protected SharedAudioSystem AudioSystem;
  [Dependency]
  protected SharedContainerSystem ContainerSystem;
  [Dependency]
  private EmagSystem _emag;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ArtifactCrusherComponent, ComponentInit>(new EntityEventRefHandler<ArtifactCrusherComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<ArtifactCrusherComponent, StorageAfterOpenEvent>(new EntityEventRefHandler<ArtifactCrusherComponent, StorageAfterOpenEvent>(this.OnStorageAfterOpen));
    this.SubscribeLocalEvent<ArtifactCrusherComponent, StorageOpenAttemptEvent>(new EntityEventRefHandler<ArtifactCrusherComponent, StorageOpenAttemptEvent>(this.OnStorageOpenAttempt));
    this.SubscribeLocalEvent<ArtifactCrusherComponent, ExaminedEvent>(new EntityEventRefHandler<ArtifactCrusherComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<ArtifactCrusherComponent, GotEmaggedEvent>(new EntityEventRefHandler<ArtifactCrusherComponent, GotEmaggedEvent>(this.OnEmagged));
  }

  private void OnInit(Entity<ArtifactCrusherComponent> ent, ref ComponentInit args)
  {
    ent.Comp.OutputContainer = this.ContainerSystem.EnsureContainer<Container>((EntityUid) ent, ent.Comp.OutputContainerName);
  }

  private void OnStorageAfterOpen(
    Entity<ArtifactCrusherComponent> ent,
    ref StorageAfterOpenEvent args)
  {
    this.StopCrushing(ent);
    this.ContainerSystem.EmptyContainer((BaseContainer) ent.Comp.OutputContainer);
  }

  private void OnEmagged(Entity<ArtifactCrusherComponent> ent, ref GotEmaggedEvent args)
  {
    if (!this._emag.CompareFlag(args.Type, EmagType.Interaction) || this._emag.CheckFlag((EntityUid) ent, EmagType.Interaction) || ent.Comp.AutoLock)
      return;
    ent.Comp.AutoLock = true;
    args.Handled = true;
  }

  private void OnStorageOpenAttempt(
    Entity<ArtifactCrusherComponent> ent,
    ref StorageOpenAttemptEvent args)
  {
    if (!ent.Comp.AutoLock || !ent.Comp.Crushing)
      return;
    args.Cancelled = true;
  }

  private void OnExamine(Entity<ArtifactCrusherComponent> ent, ref ExaminedEvent args)
  {
    args.PushMarkup(ent.Comp.AutoLock ? this.Loc.GetString("artifact-crusher-examine-autolocks") : this.Loc.GetString("artifact-crusher-examine-no-autolocks"));
  }

  public void StopCrushing(Entity<ArtifactCrusherComponent> ent, bool early = true)
  {
    (EntityUid _, ArtifactCrusherComponent comp) = ent;
    if (!comp.Crushing)
      return;
    comp.Crushing = false;
    this.Appearance.SetData((EntityUid) ent, (Enum) ArtifactCrusherVisuals.Crushing, (object) false);
    if (early)
    {
      SharedAudioSystem audioSystem = this.AudioSystem;
      ref (EntityUid, AudioComponent)? local1 = ref comp.CrushingSoundEntity;
      EntityUid? uid = local1.HasValue ? new EntityUid?(local1.GetValueOrDefault().Item1) : new EntityUid?();
      ref (EntityUid, AudioComponent)? local2 = ref comp.CrushingSoundEntity;
      AudioComponent component = local2.HasValue ? local2.GetValueOrDefault().Item2 : (AudioComponent) null;
      audioSystem.Stop(uid, component);
      comp.CrushingSoundEntity = new (EntityUid, AudioComponent)?();
    }
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }
}
