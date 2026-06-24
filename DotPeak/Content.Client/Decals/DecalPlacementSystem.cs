// Decompiled with JetBrains decompiler
// Type: Content.Client.Decals.DecalPlacementSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Actions;
using Content.Client.Decals.Overlays;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Decals;

public sealed class DecalPlacementSystem : EntitySystem
{
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IOverlayManager _overlay;
  [Dependency]
  private IPrototypeManager _protoMan;
  [Dependency]
  private InputSystem _inputSystem;
  [Dependency]
  private MetaDataSystem _metaData;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SpriteSystem _sprite;
  public static readonly EntProtoId DecalAction = EntProtoId.op_Implicit("BaseMappingDecalAction");
  private string? _decalId;
  private Color _decalColor = Color.White;
  private Angle _decalAngle = Angle.Zero;
  private bool _snap;
  private int _zIndex;
  private bool _cleanable;
  private bool _active;
  private bool _placing;
  private bool _erasing;

  public (DecalPrototype? Decal, bool Snap, Angle Angle, Color Color) GetActiveDecal()
  {
    return !this._active || this._decalId == null ? ((DecalPrototype) null, false, Angle.Zero, Color.Wheat) : (this._protoMan.Index<DecalPrototype>(this._decalId), this._snap, this._decalAngle, this._decalColor);
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this._overlay.AddOverlay((Overlay) new DecalPlacementOverlay(this, this._transform, this._sprite));
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(EngineKeyFunctions.EditorPlaceObject, (InputCmdHandler) new PointerStateInputCmdHandler(new PointerInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__19_0)), new PointerInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__19_1)), true)).Bind(EngineKeyFunctions.EditorCancelPlace, (InputCmdHandler) new PointerStateInputCmdHandler(new PointerInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__19_2)), new PointerInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__19_3)), true)).Register<DecalPlacementSystem>();
    this.SubscribeLocalEvent<FillActionSlotEvent>(new EntityEventHandler<FillActionSlotEvent>(this.OnFillSlot), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<PlaceDecalActionEvent>(new EntityEventHandler<PlaceDecalActionEvent>(this.OnPlaceDecalAction), (Type[]) null, (Type[]) null);
  }

  private void OnPlaceDecalAction(PlaceDecalActionEvent args)
  {
    if (args.Handled || !this._transform.GetGrid(args.Target).HasValue)
      return;
    args.Handled = true;
    if (args.Snap)
    {
      Vector2 vector2 = new Vector2(MathF.Round(((EntityCoordinates) ref args.Target).X - 0.5f, MidpointRounding.AwayFromZero) + 0.5f, MathF.Round(((EntityCoordinates) ref args.Target).Y - 0.5f, MidpointRounding.AwayFromZero) + 0.5f);
      args.Target = ((EntityCoordinates) ref args.Target).WithPosition(vector2);
    }
    args.Target = ((EntityCoordinates) ref args.Target).Offset(new Vector2(-0.5f, -0.5f));
    this.RaiseNetworkEvent((EntityEventArgs) new RequestDecalPlacementEvent(new Decal(args.Target.Position, args.DecalId, new Color?(args.Color), Angle.FromDegrees(args.Rotation), args.ZIndex, args.Cleanable), this.GetNetCoordinates(args.Target, (MetaDataComponent) null)));
  }

  private void OnFillSlot(FillActionSlotEvent ev)
  {
    DecalPrototype decalPrototype;
    if (!this._active || this._placing || ev.Action.HasValue || this._decalId == null || !this._protoMan.TryIndex<DecalPrototype>(this._decalId, ref decalPrototype))
      return;
    PlaceDecalActionEvent ev1 = new PlaceDecalActionEvent()
    {
      DecalId = this._decalId,
      Color = this._decalColor,
      Rotation = ((Angle) ref this._decalAngle).Degrees,
      Snap = this._snap,
      ZIndex = this._zIndex,
      Cleanable = this._cleanable
    };
    EntityUid uid = this.Spawn(EntProtoId.op_Implicit(DecalPlacementSystem.DecalAction), (ComponentRegistry) null, true);
    ActionComponent actionComponent = this.Comp<ActionComponent>(uid);
    (EntityUid, ActionComponent) valueTuple = (uid, actionComponent);
    this._actions.SetEvent(uid, (BaseActionEvent) ev1);
    this._actions.SetIcon(Entity<ActionComponent>.op_Implicit(valueTuple), decalPrototype.Sprite);
    this._actions.SetIconColor(Entity<ActionComponent>.op_Implicit(valueTuple), this._decalColor);
    this._metaData.SetEntityName(uid, $"{this._decalId} ({((Color) ref this._decalColor).ToHex()}, {(int) ((Angle) ref this._decalAngle).Degrees})", (MetaDataComponent) null, true);
    ev.Action = new EntityUid?(uid);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlay.RemoveOverlay<DecalPlacementOverlay>();
    CommandBinds.Unregister<DecalPlacementSystem>();
  }

  public void UpdateDecalInfo(
    string id,
    Color color,
    float rotation,
    bool snap,
    int zIndex,
    bool cleanable)
  {
    this._decalId = id;
    this._decalColor = color;
    this._decalAngle = Angle.FromDegrees((double) rotation);
    this._snap = snap;
    this._zIndex = zIndex;
    this._cleanable = cleanable;
  }

  public void SetActive(bool active)
  {
    this._active = active;
    if (this._active)
      this._inputManager.Contexts.SetActiveContext("editor");
    else
      this._inputSystem.SetEntityContextActive();
  }
}
