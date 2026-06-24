// Decompiled with JetBrains decompiler
// Type: Content.Client.StatusEffectNew.ClientStatusEffectsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.StatusEffectNew;
using Content.Shared.StatusEffectNew.Components;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using System;

#nullable enable
namespace Content.Client.StatusEffectNew;

public sealed class ClientStatusEffectsSystem : SharedStatusEffectsSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StatusEffectContainerComponent, ComponentHandleState>(new EntityEventRefHandler<StatusEffectContainerComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    Entity<StatusEffectContainerComponent> ent,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is StatusEffectContainerComponentState current))
      return;
    ValueList<EntityUid> valueList = new ValueList<EntityUid>();
    foreach (EntityUid activeStatusEffect in ent.Comp.ActiveStatusEffects)
    {
      NetEntity? nullable;
      if (this.TryGetNetEntity(activeStatusEffect, ref nullable, (MetaDataComponent) null) && !current.ActiveStatusEffects.Contains(nullable.Value))
        valueList.Add(activeStatusEffect);
    }
    foreach (EntityUid entityUid in valueList)
    {
      ent.Comp.ActiveStatusEffects.Remove(entityUid);
      StatusEffectRemovedEvent effectRemovedEvent = new StatusEffectRemovedEvent(Entity<StatusEffectContainerComponent>.op_Implicit(ent));
      this.RaiseLocalEvent<StatusEffectRemovedEvent>(entityUid, ref effectRemovedEvent, false);
    }
    foreach (NetEntity activeStatusEffect in current.ActiveStatusEffects)
    {
      EntityUid entity = this.GetEntity(activeStatusEffect);
      if (!ent.Comp.ActiveStatusEffects.Contains(entity))
      {
        ent.Comp.ActiveStatusEffects.Add(entity);
        StatusEffectAppliedEvent effectAppliedEvent = new StatusEffectAppliedEvent(Entity<StatusEffectContainerComponent>.op_Implicit(ent));
        this.RaiseLocalEvent<StatusEffectAppliedEvent>(entity, ref effectAppliedEvent, false);
      }
    }
  }
}
