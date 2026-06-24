// Decompiled with JetBrains decompiler
// Type: Content.Client.Doors.Electronics.DoorElectronicsBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Access;
using Content.Shared.Doors.Electronics;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Doors.Electronics;

public sealed class DoorElectronicsBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private DoorElectronicsConfigurationMenu? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<DoorElectronicsConfigurationMenu>((BoundUserInterface) this);
    this._window.OnAccessChanged += new Action<List<ProtoId<AccessLevelPrototype>>>(this.UpdateConfiguration);
    this.Reset();
  }

  public virtual void OnProtoReload(PrototypesReloadedEventArgs args)
  {
    base.OnProtoReload(args);
    if (!args.WasModified<AccessLevelPrototype>())
      return;
    this.Reset();
  }

  private void Reset()
  {
    List<ProtoId<AccessLevelPrototype>> accessLevels = new List<ProtoId<AccessLevelPrototype>>();
    foreach (AccessLevelPrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<AccessLevelPrototype>())
    {
      if (enumeratePrototype.Name != null)
        accessLevels.Add(ProtoId<AccessLevelPrototype>.op_Implicit(enumeratePrototype.ID));
    }
    accessLevels.Sort();
    this._window?.Reset(this._prototypeManager, accessLevels);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    this._window?.UpdateState((DoorElectronicsConfigurationState) state);
  }

  public void UpdateConfiguration(List<ProtoId<AccessLevelPrototype>> newAccessList)
  {
    this.SendMessage((BoundUserInterfaceMessage) new DoorElectronicsUpdateConfigurationMessage(newAccessList));
  }
}
