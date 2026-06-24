// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceLinking.Systems.TwoWayLeverSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceLinking.Components;
using Content.Shared.Interaction;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.DeviceLinking.Systems;

public sealed class TwoWayLeverSystem : EntitySystem
{
  [Dependency]
  private SharedDeviceLinkSystem _signalSystem;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  private const string _leftToggleImage = "rotate_ccw.svg.192dpi.png";
  private const string _rightToggleImage = "rotate_cw.svg.192dpi.png";

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TwoWayLeverComponent, ComponentInit>(new ComponentEventHandler<TwoWayLeverComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TwoWayLeverComponent, ActivateInWorldEvent>(new ComponentEventHandler<TwoWayLeverComponent, ActivateInWorldEvent>((object) this, __methodptr(OnActivated)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TwoWayLeverComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<TwoWayLeverComponent, GetVerbsEvent<InteractionVerb>>((object) this, __methodptr(OnGetInteractionVerbs)), (Type[]) null, (Type[]) null);
  }

  private void OnInit(EntityUid uid, TwoWayLeverComponent component, ComponentInit args)
  {
    this._signalSystem.EnsureSourcePorts(uid, component.LeftPort, component.RightPort, component.MiddlePort);
  }

  private void OnActivated(
    EntityUid uid,
    TwoWayLeverComponent component,
    ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    TwoWayLeverComponent wayLeverComponent = component;
    TwoWayLeverState twoWayLeverState;
    switch (component.State)
    {
      case TwoWayLeverState.Middle:
        twoWayLeverState = component.NextSignalLeft ? TwoWayLeverState.Left : TwoWayLeverState.Right;
        break;
      case TwoWayLeverState.Right:
        twoWayLeverState = TwoWayLeverState.Middle;
        break;
      case TwoWayLeverState.Left:
        twoWayLeverState = TwoWayLeverState.Middle;
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    wayLeverComponent.State = twoWayLeverState;
    this.StateChanged(uid, component);
    args.Handled = true;
  }

  private void OnGetInteractionVerbs(
    EntityUid uid,
    TwoWayLeverComponent component,
    GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null)
      return;
    bool flag1 = component.State == TwoWayLeverState.Left;
    InteractionVerb interactionVerb1 = new InteractionVerb();
    interactionVerb1.Act = (Action) (() =>
    {
      TwoWayLeverComponent wayLeverComponent = component;
      TwoWayLeverState twoWayLeverState;
      switch (component.State)
      {
        case TwoWayLeverState.Middle:
          twoWayLeverState = TwoWayLeverState.Left;
          break;
        case TwoWayLeverState.Right:
          twoWayLeverState = TwoWayLeverState.Middle;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      wayLeverComponent.State = twoWayLeverState;
      this.StateChanged(uid, component);
    });
    interactionVerb1.Category = VerbCategory.Lever;
    interactionVerb1.Message = flag1 ? this.Loc.GetString("two-way-lever-cant") : (string) null;
    interactionVerb1.Disabled = flag1;
    interactionVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/rotate_ccw.svg.192dpi.png"));
    interactionVerb1.Text = this.Loc.GetString("two-way-lever-left");
    InteractionVerb interactionVerb2 = interactionVerb1;
    args.Verbs.Add(interactionVerb2);
    bool flag2 = component.State == TwoWayLeverState.Right;
    InteractionVerb interactionVerb3 = new InteractionVerb();
    interactionVerb3.Act = (Action) (() =>
    {
      TwoWayLeverComponent wayLeverComponent = component;
      TwoWayLeverState twoWayLeverState;
      switch (component.State)
      {
        case TwoWayLeverState.Middle:
          twoWayLeverState = TwoWayLeverState.Right;
          break;
        case TwoWayLeverState.Left:
          twoWayLeverState = TwoWayLeverState.Middle;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      wayLeverComponent.State = twoWayLeverState;
      this.StateChanged(uid, component);
    });
    interactionVerb3.Category = VerbCategory.Lever;
    interactionVerb3.Message = flag2 ? this.Loc.GetString("two-way-lever-cant") : (string) null;
    interactionVerb3.Disabled = flag2;
    interactionVerb3.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/rotate_cw.svg.192dpi.png"));
    interactionVerb3.Text = this.Loc.GetString("two-way-lever-right");
    InteractionVerb interactionVerb4 = interactionVerb3;
    args.Verbs.Add(interactionVerb4);
  }

  private void StateChanged(EntityUid uid, TwoWayLeverComponent component)
  {
    if (component.State == TwoWayLeverState.Middle)
      component.NextSignalLeft = !component.NextSignalLeft;
    AppearanceComponent appearanceComponent;
    if (this.TryComp<AppearanceComponent>(uid, ref appearanceComponent))
      this._appearance.SetData(uid, (Enum) TwoWayLeverVisuals.State, (object) component.State, appearanceComponent);
    ProtoId<SourcePortPrototype> protoId1;
    switch (component.State)
    {
      case TwoWayLeverState.Middle:
        protoId1 = component.MiddlePort;
        break;
      case TwoWayLeverState.Right:
        protoId1 = component.RightPort;
        break;
      case TwoWayLeverState.Left:
        protoId1 = component.LeftPort;
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    ProtoId<SourcePortPrototype> protoId2 = protoId1;
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    this._signalSystem.InvokePort(uid, ProtoId<SourcePortPrototype>.op_Implicit(protoId2));
  }
}
