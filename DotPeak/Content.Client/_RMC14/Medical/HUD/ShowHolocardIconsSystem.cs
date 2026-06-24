// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Medical.HUD.ShowHolocardIconsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Overlays;
using Content.Shared._RMC14.Medical.HUD;
using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client._RMC14.Medical.HUD;

public sealed class ShowHolocardIconsSystem : EquipmentHudSystem<HolocardScannerComponent>
{
  [Dependency]
  private IPrototypeManager _prototypes;
  private static readonly ProtoId<HealthIconPrototype> Urgent = ProtoId<HealthIconPrototype>.op_Implicit("UrgentHolocardIcon");
  private static readonly ProtoId<HealthIconPrototype> Emergency = ProtoId<HealthIconPrototype>.op_Implicit("EmergencyHolocardIcon");
  private static readonly ProtoId<HealthIconPrototype> Xeno = ProtoId<HealthIconPrototype>.op_Implicit("XenoHolocardIcon");
  private static readonly ProtoId<HealthIconPrototype> Permadead = ProtoId<HealthIconPrototype>.op_Implicit("PermaHolocardIcon");

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HolocardStateComponent, GetStatusIconsEvent>(new EntityEventRefHandler<HolocardStateComponent, GetStatusIconsEvent>((object) this, __methodptr(OnGetStatusIconsEvent)), (Type[]) null, (Type[]) null);
  }

  private void OnGetStatusIconsEvent(
    Entity<HolocardStateComponent> entity,
    ref GetStatusIconsEvent args)
  {
    if (!this.IsActive)
      return;
    IReadOnlyList<StatusIconData> icons = this.GetIcons(entity);
    args.StatusIcons.AddRange((IEnumerable<StatusIconData>) icons);
  }

  public IReadOnlyList<StatusIconData> GetIcons(Entity<HolocardStateComponent> entity)
  {
    List<StatusIconData> icons = new List<StatusIconData>();
    HolocardData data;
    if (this.TryGetHolocardData(entity.Comp.HolocardStatus, out data))
    {
      ProtoId<HealthIconPrototype>? holocardIcon = data.HolocardIcon;
      if (holocardIcon.HasValue)
      {
        IPrototypeManager prototypes = this._prototypes;
        holocardIcon = data.HolocardIcon;
        ProtoId<HealthIconPrototype> protoId = holocardIcon.Value;
        HealthIconPrototype healthIconPrototype = prototypes.Index<HealthIconPrototype>(protoId);
        icons.Add((StatusIconData) healthIconPrototype);
      }
    }
    return (IReadOnlyList<StatusIconData>) icons;
  }

  public bool TryGetHolocardData(HolocardStatus holocardStatus, out HolocardData data)
  {
    data = new HolocardData();
    switch (holocardStatus)
    {
      case HolocardStatus.None:
        data.HolocardIcon = new ProtoId<HealthIconPrototype>?();
        data.Description = LocId.op_Implicit(this.Loc.GetString("hc-none-description"));
        break;
      case HolocardStatus.Urgent:
        data.HolocardIcon = new ProtoId<HealthIconPrototype>?(ShowHolocardIconsSystem.Urgent);
        data.Description = LocId.op_Implicit(this.Loc.GetString("hc-urgent-description"));
        break;
      case HolocardStatus.Emergency:
        data.HolocardIcon = new ProtoId<HealthIconPrototype>?(ShowHolocardIconsSystem.Emergency);
        data.Description = LocId.op_Implicit(this.Loc.GetString("hc-emergency-description"));
        break;
      case HolocardStatus.Xeno:
        data.HolocardIcon = new ProtoId<HealthIconPrototype>?(ShowHolocardIconsSystem.Xeno);
        data.Description = LocId.op_Implicit(this.Loc.GetString("hc-xeno-description"));
        break;
      case HolocardStatus.Permadead:
        data.HolocardIcon = new ProtoId<HealthIconPrototype>?(ShowHolocardIconsSystem.Permadead);
        data.Description = LocId.op_Implicit(this.Loc.GetString("hc-permadead-description"));
        break;
      default:
        data = new HolocardData();
        return false;
    }
    return true;
  }

  public bool TryGetHolocardName(HolocardStatus holocardStatus, [NotNullWhen(true)] out string? holocardName)
  {
    holocardName = (string) null;
    switch (holocardStatus)
    {
      case HolocardStatus.None:
        holocardName = this.Loc.GetString("hc-none-name");
        break;
      case HolocardStatus.Urgent:
        holocardName = this.Loc.GetString("hc-urgent-name");
        break;
      case HolocardStatus.Emergency:
        holocardName = this.Loc.GetString("hc-emergency-name");
        break;
      case HolocardStatus.Xeno:
        holocardName = this.Loc.GetString("hc-xeno-name");
        break;
      case HolocardStatus.Permadead:
        holocardName = this.Loc.GetString("hc-permadead-name");
        break;
      default:
        return false;
    }
    return true;
  }

  public bool TryGetHolocardColor(HolocardStatus holocardStatus, [NotNullWhen(true)] out Color? holocardColor)
  {
    holocardColor = new Color?();
    switch (holocardStatus)
    {
      case HolocardStatus.Urgent:
        holocardColor = new Color?(Color.Chocolate);
        break;
      case HolocardStatus.Emergency:
        holocardColor = new Color?(Color.DarkRed);
        break;
      case HolocardStatus.Xeno:
        holocardColor = new Color?(Color.Purple);
        break;
      case HolocardStatus.Permadead:
        holocardColor = new Color?(Color.Black);
        break;
      default:
        return false;
    }
    return true;
  }

  public bool TryGetHolocardColor(Entity<HolocardStateComponent> entity, [NotNullWhen(true)] out Color? holocardColor)
  {
    holocardColor = new Color?();
    Color? holocardColor1;
    if (!this.TryGetHolocardColor(entity.Comp.HolocardStatus, out holocardColor1))
      return false;
    holocardColor = holocardColor1;
    return true;
  }

  public bool TryGetDescription(Entity<HolocardStateComponent> entity, [NotNullWhen(true)] out string? description)
  {
    description = (string) null;
    HolocardData data;
    if (!this.TryGetHolocardData(entity.Comp.HolocardStatus, out data))
      return false;
    description = LocId.op_Implicit(data.Description);
    return true;
  }
}
