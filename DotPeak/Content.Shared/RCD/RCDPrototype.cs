// Decompiled with JetBrains decompiler
// Type: Content.Shared.RCD.RCDPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Physics;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.RCD;

[Robust.Shared.Prototypes.Prototype("rcd", 1)]
public sealed class RCDPrototype : IPrototype
{
  private Box2? _collisionBounds;

  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public RcdMode Mode { get; private set; }

  [DataField("name", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public string SetName { get; private set; } = "Unknown";

  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public string Category { get; private set; } = "Undefined";

  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public SpriteSpecifier? Sprite { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public string? Prototype { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public int Cost { get; private set; } = 1;

  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public float Delay { get; private set; } = 1f;

  [DataField("fx", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public EntProtoId? Effect { get; private set; }

  [DataField("rules", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public HashSet<RcdConstructionRule> ConstructionRules { get; private set; } = new HashSet<RcdConstructionRule>();

  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public CollisionGroup CollisionMask { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public Box2? CollisionBounds
  {
    get => this._collisionBounds;
    private set
    {
      this._collisionBounds = value;
      if (!this._collisionBounds.HasValue)
        return;
      PolygonShape polygonShape = new PolygonShape();
      polygonShape.SetAsBox(this._collisionBounds.Value);
      this.CollisionPolygon = polygonShape;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public PolygonShape? CollisionPolygon { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public RcdRotation Rotation { get; private set; } = RcdRotation.User;
}
