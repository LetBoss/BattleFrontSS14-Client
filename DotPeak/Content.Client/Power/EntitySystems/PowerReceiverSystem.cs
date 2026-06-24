// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.EntitySystems.PowerReceiverSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Power.Components;
using Content.Shared.Examine;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.Power.EntitySystems;

public sealed class PowerReceiverSystem : SharedPowerReceiverSystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ApcPowerReceiverComponent, ExaminedEvent>(new EntityEventRefHandler<ApcPowerReceiverComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ApcPowerReceiverComponent, ComponentHandleState>(new ComponentEventRefHandler<ApcPowerReceiverComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnExamined(Entity<ApcPowerReceiverComponent> ent, ref ExaminedEvent args)
  {
    args.PushMarkup(this.GetExamineText(ent.Comp.Powered));
  }

  private void OnHandleState(
    EntityUid uid,
    ApcPowerReceiverComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is ApcPowerReceiverComponentState current))
      return;
    int num = component.Powered != current.Powered ? 1 : 0;
    component.Powered = current.Powered;
    component.NeedsPower = current.NeedsPower;
    component.PowerDisabled = current.PowerDisabled;
    if (num == 0)
      return;
    this.RaisePower(Entity<SharedApcPowerReceiverComponent>.op_Implicit((uid, (SharedApcPowerReceiverComponent) component)));
  }

  protected override void RaisePower(Entity<SharedApcPowerReceiverComponent> entity)
  {
    PowerChangedEvent powerChangedEvent = new PowerChangedEvent(entity.Comp.Powered, 0.0f);
    this.RaiseLocalEvent<PowerChangedEvent>(entity.Owner, ref powerChangedEvent, false);
  }

  public override bool ResolveApc(EntityUid entity, [NotNullWhen(true)] ref SharedApcPowerReceiverComponent? component)
  {
    if (component != null)
      return true;
    ApcPowerReceiverComponent receiverComponent;
    if (!this.TryComp<ApcPowerReceiverComponent>(entity, ref receiverComponent))
      return false;
    component = (SharedApcPowerReceiverComponent) receiverComponent;
    return true;
  }
}
