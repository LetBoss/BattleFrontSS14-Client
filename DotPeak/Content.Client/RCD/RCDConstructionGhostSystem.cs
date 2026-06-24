// Decompiled with JetBrains decompiler
// Type: Content.Client.RCD.RCDConstructionGhostSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Hands.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.RCD;
using Content.Shared.RCD.Components;
using Robust.Client.Placement;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.RCD;

public sealed class RCDConstructionGhostSystem : EntitySystem
{
  private const string PlacementMode = "AlignRCDConstruction";
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IPlacementManager _placementManager;
  [Dependency]
  private IPrototypeManager _protoManager;
  [Dependency]
  private HandsSystem _hands;
  private Direction _placementDirection;

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityUid? mobUid = this._placementManager.CurrentPermission?.MobUid;
    string entityType = this._placementManager.CurrentPermission?.EntityType;
    bool flag = this.HasComp<RCDComponent>(mobUid);
    if (this._placementManager.Eraser || mobUid.HasValue && !flag)
      return;
    EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._playerManager).LocalSession?.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid? activeItem = this._hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(attachedEntity.GetValueOrDefault()));
    RCDComponent rcdComponent;
    if (!this.TryComp<RCDComponent>(activeItem, ref rcdComponent))
    {
      if (!flag)
        return;
      this._placementManager.Clear();
    }
    else
    {
      RCDPrototype rcdPrototype = this._protoManager.Index<RCDPrototype>(rcdComponent.ProtoId);
      if (this._placementDirection != this._placementManager.Direction)
      {
        this._placementDirection = this._placementManager.Direction;
        this.RaiseNetworkEvent((EntityEventArgs) new RCDConstructionGhostRotationEvent(this.GetNetEntity(activeItem.Value, (MetaDataComponent) null), this._placementDirection));
      }
      EntityUid? nullable1 = activeItem;
      EntityUid? nullable2 = mobUid;
      if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (EntityUid.op_Equality(nullable1.GetValueOrDefault(), nullable2.GetValueOrDefault()) ? 1 : 0) : 1) : 0) != 0 && rcdPrototype.Prototype == entityType)
        return;
      PlacementInformation placementInformation = new PlacementInformation()
      {
        MobUid = activeItem.Value,
        PlacementOption = "AlignRCDConstruction",
        EntityType = rcdPrototype.Prototype,
        Range = (int) Math.Ceiling(1.5),
        IsTile = rcdPrototype.Mode == RcdMode.ConstructTile,
        UseEditorContext = false
      };
      this._placementManager.Clear();
      this._placementManager.BeginPlacing(placementInformation, (PlacementHijack) null);
    }
  }
}
