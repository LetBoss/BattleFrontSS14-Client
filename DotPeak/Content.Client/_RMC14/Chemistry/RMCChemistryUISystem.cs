// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Chemistry.RMCChemistryUISystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Chemistry;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._RMC14.Chemistry;

public sealed class RMCChemistryUISystem : SharedRMCChemistrySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private UserInterfaceSystem _ui;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCChemicalDispenserComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<RMCChemicalDispenserComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnDispenserAfterState)), (Type[]) null, (Type[]) null);
  }

  private void OnDispenserAfterState(
    Entity<RMCChemicalDispenserComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateDispenserUI(ent);
  }

  private void UpdateDispenserUI(Entity<RMCChemicalDispenserComponent> ent)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(Entity<RMCChemicalDispenserComponent>.op_Implicit(ent), ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is RMCChemicalDispenserBui chemicalDispenserBui)
          chemicalDispenserBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"RMCChemicalDispenserBui"}\n{ex}");
    }
  }

  protected override void DispenserUpdated(Entity<RMCChemicalDispenserComponent> ent)
  {
    base.DispenserUpdated(ent);
    this.UpdateDispenserUI(ent);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    foreach ((EntityUid, Enum) actorUi in ((SharedUserInterfaceSystem) this._ui).GetActorUis(Entity<UserInterfaceUserComponent>.op_Implicit(localEntity.GetValueOrDefault())))
    {
      if (actorUi.Item2 is RMCChemicalDispenserUi.Key)
      {
        UserInterfaceComponent interfaceComponent;
        if (!this.TryComp<UserInterfaceComponent>(actorUi.Item1, ref interfaceComponent))
          break;
        foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
        {
          if (boundUserInterface is RMCChemicalDispenserBui chemicalDispenserBui)
            chemicalDispenserBui.Refresh();
        }
      }
    }
  }
}
