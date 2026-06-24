// Decompiled with JetBrains decompiler
// Type: Content.Client.Charges.ChargesSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Charges;

public sealed class ChargesSystem : SharedChargesSystem
{
  [Dependency]
  private ActionsSystem _actions;
  private Dictionary<EntityUid, int> _lastCharges = new Dictionary<EntityUid, int>();
  private Dictionary<EntityUid, int> _tempLastCharges = new Dictionary<EntityUid, int>();

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    AllEntityQueryEnumerator<AutoRechargeComponent, LimitedChargesComponent> entityQueryEnumerator = this.AllEntityQuery<AutoRechargeComponent, LimitedChargesComponent>();
    EntityUid key1;
    AutoRechargeComponent rechargeComponent;
    LimitedChargesComponent chargesComponent;
    while (entityQueryEnumerator.MoveNext(ref key1, ref rechargeComponent, ref chargesComponent))
    {
      Entity<ActionComponent>? action = this._actions.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(key1)), false);
      if (action.HasValue)
      {
        Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
        int currentCharges = this.GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit((key1, chargesComponent, rechargeComponent)));
        int num;
        if (!this._lastCharges.TryGetValue(key1, out num) || currentCharges != num)
          this._actions.UpdateAction(valueOrDefault);
        this._tempLastCharges[key1] = currentCharges;
      }
    }
    this._lastCharges.Clear();
    foreach (KeyValuePair<EntityUid, int> tempLastCharge in this._tempLastCharges)
    {
      EntityUid key2;
      (key2, this._lastCharges[key2]) = tempLastCharge;
    }
    this._tempLastCharges.Clear();
  }
}
