// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.EntitySystems.SharedSolutionContainerMixerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry.EntitySystems;

public abstract class SharedSolutionContainerMixerSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedSolutionContainerSystem _solution;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionContainerMixerComponent, ActivateInWorldEvent>(new EntityEventRefHandler<SolutionContainerMixerComponent, ActivateInWorldEvent>((object) this, __methodptr(OnActivateInWorld)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SolutionContainerMixerComponent, ContainerIsRemovingAttemptEvent>(new EntityEventRefHandler<SolutionContainerMixerComponent, ContainerIsRemovingAttemptEvent>((object) this, __methodptr(OnRemoveAttempt)), (Type[]) null, (Type[]) null);
  }

  private void OnActivateInWorld(
    Entity<SolutionContainerMixerComponent> entity,
    ref ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    this.TryStartMix(entity, new EntityUid?(args.User));
    args.Handled = true;
  }

  private void OnRemoveAttempt(
    Entity<SolutionContainerMixerComponent> ent,
    ref ContainerIsRemovingAttemptEvent args)
  {
    if (!(((ContainerAttemptEventBase) args).Container.ID == ent.Comp.ContainerId) || !ent.Comp.Mixing)
      return;
    ((CancellableEntityEventArgs) args).Cancel();
  }

  protected virtual bool HasPower(Entity<SolutionContainerMixerComponent> entity) => true;

  public void TryStartMix(Entity<SolutionContainerMixerComponent> entity, EntityUid? user)
  {
    EntityUid entityUid1;
    SolutionContainerMixerComponent containerMixerComponent1;
    entity.Deconstruct(ref entityUid1, ref containerMixerComponent1);
    EntityUid entityUid2 = entityUid1;
    SolutionContainerMixerComponent containerMixerComponent2 = containerMixerComponent1;
    if (containerMixerComponent2.Mixing)
      return;
    if (!this.HasPower(entity))
    {
      if (!user.HasValue)
        return;
      this._popup.PopupClient(this.Loc.GetString("solution-container-mixer-no-power"), Entity<SolutionContainerMixerComponent>.op_Implicit(entity), new EntityUid?(user.Value));
    }
    else
    {
      BaseContainer baseContainer;
      if (!this._container.TryGetContainer(entityUid2, containerMixerComponent2.ContainerId, ref baseContainer, (ContainerManagerComponent) null) || baseContainer.Count == 0)
      {
        if (!user.HasValue)
          return;
        this._popup.PopupClient(this.Loc.GetString("solution-container-mixer-popup-nothing-to-mix"), Entity<SolutionContainerMixerComponent>.op_Implicit(entity), new EntityUid?(user.Value));
      }
      else
      {
        containerMixerComponent2.Mixing = true;
        if (this._net.IsServer)
        {
          SolutionContainerMixerComponent containerMixerComponent3 = containerMixerComponent2;
          SharedAudioSystem audio = this._audio;
          SoundSpecifier mixingSound1 = containerMixerComponent2.MixingSound;
          EntityUid entityUid3 = Entity<SolutionContainerMixerComponent>.op_Implicit(entity);
          SoundSpecifier mixingSound2 = containerMixerComponent2.MixingSound;
          AudioParams? nullable1;
          if (mixingSound2 == null)
          {
            nullable1 = new AudioParams?();
          }
          else
          {
            AudioParams audioParams = mixingSound2.Params;
            nullable1 = new AudioParams?(((AudioParams) ref audioParams).WithLoop(true));
          }
          (EntityUid, AudioComponent)? nullable2 = audio.PlayPvs(mixingSound1, entityUid3, nullable1);
          Entity<AudioComponent>? nullable3 = nullable2.HasValue ? new Entity<AudioComponent>?(Entity<AudioComponent>.op_Implicit(nullable2.GetValueOrDefault())) : new Entity<AudioComponent>?();
          containerMixerComponent3.MixingSoundEntity = nullable3;
        }
        containerMixerComponent2.MixTimeEnd = this._timing.CurTime + containerMixerComponent2.MixDuration;
        this._appearance.SetData(Entity<SolutionContainerMixerComponent>.op_Implicit(entity), (Enum) SolutionContainerMixerVisuals.Mixing, (object) true, (AppearanceComponent) null);
        this.Dirty(entityUid2, (IComponent) containerMixerComponent2, (MetaDataComponent) null);
      }
    }
  }

  public void StopMix(Entity<SolutionContainerMixerComponent> entity)
  {
    EntityUid entityUid1;
    SolutionContainerMixerComponent containerMixerComponent1;
    entity.Deconstruct(ref entityUid1, ref containerMixerComponent1);
    EntityUid entityUid2 = entityUid1;
    SolutionContainerMixerComponent containerMixerComponent2 = containerMixerComponent1;
    if (!containerMixerComponent2.Mixing)
      return;
    SharedAudioSystem audio = this._audio;
    Entity<AudioComponent>? mixingSoundEntity = containerMixerComponent2.MixingSoundEntity;
    EntityUid? nullable = mixingSoundEntity.HasValue ? new EntityUid?(Entity<AudioComponent>.op_Implicit(mixingSoundEntity.GetValueOrDefault())) : new EntityUid?();
    audio.Stop(nullable, (AudioComponent) null);
    this._appearance.SetData(Entity<SolutionContainerMixerComponent>.op_Implicit(entity), (Enum) SolutionContainerMixerVisuals.Mixing, (object) false, (AppearanceComponent) null);
    containerMixerComponent2.Mixing = false;
    containerMixerComponent2.MixingSoundEntity = new Entity<AudioComponent>?();
    this.Dirty(entityUid2, (IComponent) containerMixerComponent2, (MetaDataComponent) null);
  }

  public void FinishMix(Entity<SolutionContainerMixerComponent> entity)
  {
    EntityUid entityUid1;
    SolutionContainerMixerComponent containerMixerComponent1;
    entity.Deconstruct(ref entityUid1, ref containerMixerComponent1);
    EntityUid entityUid2 = entityUid1;
    SolutionContainerMixerComponent containerMixerComponent2 = containerMixerComponent1;
    if (!containerMixerComponent2.Mixing)
      return;
    this.StopMix(entity);
    ReactionMixerComponent mixerComponent;
    BaseContainer baseContainer;
    if (!this.TryComp<ReactionMixerComponent>(Entity<SolutionContainerMixerComponent>.op_Implicit(entity), ref mixerComponent) || !this._container.TryGetContainer(entityUid2, containerMixerComponent2.ContainerId, ref baseContainer, (ContainerManagerComponent) null))
      return;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer.ContainedEntities)
    {
      Entity<SolutionComponent>? soln;
      if (this._solution.TryGetFitsInDispenser(Entity<FitsInDispenserComponent, SolutionContainerManagerComponent>.op_Implicit(containedEntity), out soln, out Solution _))
        this._solution.UpdateChemicals(soln.Value, mixerComponent: mixerComponent);
    }
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<SolutionContainerMixerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SolutionContainerMixerComponent>();
    EntityUid entityUid;
    SolutionContainerMixerComponent containerMixerComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref containerMixerComponent))
    {
      if (containerMixerComponent.Mixing && !(this._timing.CurTime < containerMixerComponent.MixTimeEnd))
        this.FinishMix(Entity<SolutionContainerMixerComponent>.op_Implicit((entityUid, containerMixerComponent)));
    }
  }
}
