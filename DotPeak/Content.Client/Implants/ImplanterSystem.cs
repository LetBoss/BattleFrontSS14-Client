// Decompiled with JetBrains decompiler
// Type: Content.Client.Implants.ImplanterSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Implants.UI;
using Content.Client.Items;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Robust.Client.UserInterface;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Implants;

public sealed class ImplanterSystem : SharedImplanterSystem
{
  [Dependency]
  private SharedUserInterfaceSystem _uiSystem;
  [Dependency]
  private IPrototypeManager _proto;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ImplanterComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<ImplanterComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleImplanterState)), (Type[]) null, (Type[]) null);
    this.Subs.ItemStatus<ImplanterComponent>((Func<Entity<ImplanterComponent>, Control>) (ent => (Control) new ImplanterStatusControl(Entity<ImplanterComponent>.op_Implicit(ent))));
  }

  private void OnHandleImplanterState(
    EntityUid uid,
    ImplanterComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    DeimplantBoundUserInterface boundUserInterface1;
    if (this._uiSystem.TryGetOpenUi<DeimplantBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) DeimplantUiKey.Key, ref boundUserInterface1))
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (EntProtoId entProtoId in component.DeimplantWhitelist)
      {
        EntityPrototype entityPrototype;
        if (this._proto.TryIndex(entProtoId, ref entityPrototype))
          dictionary.Add(entityPrototype.ID, entityPrototype.Name);
      }
      DeimplantBoundUserInterface boundUserInterface2 = boundUserInterface1;
      Dictionary<string, string> implantList = dictionary;
      EntProtoId? deimplantChosen = component.DeimplantChosen;
      string implant = deimplantChosen.HasValue ? EntProtoId.op_Implicit(deimplantChosen.GetValueOrDefault()) : (string) null;
      boundUserInterface2.UpdateState(implantList, implant);
    }
    component.UiUpdateNeeded = true;
  }
}
