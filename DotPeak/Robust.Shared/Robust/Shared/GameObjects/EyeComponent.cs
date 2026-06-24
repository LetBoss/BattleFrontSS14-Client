// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EyeComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameStates;
using Robust.Shared.Graphics;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedEyeSystem)})]
[AutoGenerateComponentState(true, true)]
public sealed class EyeComponent : 
  Component,
  ISerializationGenerated<EyeComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  public const int DefaultVisibilityMask = 1;
  [Robust.Shared.ViewVariables.ViewVariables]
  public readonly Eye Eye = new Eye();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DrawFov = true;
  [AutoNetworkedField]
  public bool DrawLight = true;
  [DataField(null, false, 1, false, false, null)]
  public Angle Rotation;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 Zoom = Vector2.One;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 Offset;
  [DataField("visMask", false, 1, false, false, typeof (FlagSerializer<VisibilityMaskLayer>))]
  [AutoNetworkedField]
  public int VisibilityMask = 1;
  [Access(new Type[] {typeof (SharedEyeSystem)})]
  [DataField(null, false, 1, false, false, null)]
  public float PvsScale = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EyeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EyeComponent) target1;
    if (serialization.TryCustomCopy<EyeComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Target, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Target, hookCtx, context);
    target.Target = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.DrawFov, ref target3, hookCtx, false, context))
      target3 = this.DrawFov;
    target.DrawFov = target3;
    Angle target4 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.Rotation, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Angle>(this.Rotation, hookCtx, context);
    target.Rotation = target4;
    Vector2 target5 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Zoom, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Vector2>(this.Zoom, hookCtx, context);
    target.Zoom = target5;
    Vector2 target6 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target6;
    int copy = serialization.CreateCopy<int, FlagSerializer<VisibilityMaskLayer>>(this.VisibilityMask, hookCtx, context);
    target.VisibilityMask = copy;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PvsScale, ref target7, hookCtx, false, context))
      target7 = this.PvsScale;
    target.PvsScale = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EyeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EyeComponent target1 = (EyeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EyeComponent target1 = (EyeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EyeComponent target1 = (EyeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EyeComponent target1 = (EyeComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponentDelta) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EyeComponent Component.Instantiate() => new EyeComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EyeComponent_AutoState : IComponentState
  {
    public NetEntity? Target;
    public bool DrawFov;
    public bool DrawLight;
    public Vector2 Offset;
    public int VisibilityMask;

    public EyeComponent.EyeComponent_AutoState ShallowClone()
    {
      return new EyeComponent.EyeComponent_AutoState()
      {
        Target = this.Target,
        DrawFov = this.DrawFov,
        DrawLight = this.DrawLight,
        Offset = this.Offset,
        VisibilityMask = this.VisibilityMask
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EyeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<EyeComponent>("Target", "DrawFov", "DrawLight", "Offset", "VisibilityMask");
      this.SubscribeLocalEvent<EyeComponent, ComponentGetState>(new ComponentEventRefHandler<EyeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EyeComponent, ComponentHandleState>(new ComponentEventRefHandler<EyeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, EyeComponent component, ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick))
        {
          case 1:
            args.State = (IComponentState) new EyeComponent.Target_FieldComponentState()
            {
              Target = this.GetNetEntity(component.Target)
            };
            return;
          case 2:
            args.State = (IComponentState) new EyeComponent.DrawFov_FieldComponentState()
            {
              DrawFov = component.DrawFov
            };
            return;
          case 4:
            args.State = (IComponentState) new EyeComponent.DrawLight_FieldComponentState()
            {
              DrawLight = component.DrawLight
            };
            return;
          case 8:
            args.State = (IComponentState) new EyeComponent.Offset_FieldComponentState()
            {
              Offset = component.Offset
            };
            return;
          case 16 /*0x10*/:
            args.State = (IComponentState) new EyeComponent.VisibilityMask_FieldComponentState()
            {
              VisibilityMask = component.VisibilityMask
            };
            return;
        }
      }
      args.State = (IComponentState) new EyeComponent.EyeComponent_AutoState()
      {
        Target = this.GetNetEntity(component.Target),
        DrawFov = component.DrawFov,
        DrawLight = component.DrawLight,
        Offset = component.Offset,
        VisibilityMask = component.VisibilityMask
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EyeComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case EyeComponent.Target_FieldComponentState fieldComponentState1:
          component.Target = this.EnsureEntity<EyeComponent>(fieldComponentState1.Target, uid);
          break;
        case EyeComponent.DrawFov_FieldComponentState fieldComponentState2:
          component.DrawFov = fieldComponentState2.DrawFov;
          break;
        case EyeComponent.DrawLight_FieldComponentState fieldComponentState3:
          component.DrawLight = fieldComponentState3.DrawLight;
          break;
        case EyeComponent.Offset_FieldComponentState fieldComponentState4:
          component.Offset = fieldComponentState4.Offset;
          break;
        case EyeComponent.VisibilityMask_FieldComponentState fieldComponentState5:
          component.VisibilityMask = fieldComponentState5.VisibilityMask;
          break;
        case EyeComponent.EyeComponent_AutoState componentAutoState:
          component.Target = this.EnsureEntity<EyeComponent>(componentAutoState.Target, uid);
          component.DrawFov = componentAutoState.DrawFov;
          component.DrawLight = componentAutoState.DrawLight;
          component.Offset = componentAutoState.Offset;
          component.VisibilityMask = componentAutoState.VisibilityMask;
          break;
        default:
          return;
      }
      IComponentState current = args.Current;
      if (current == null)
        return;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, EyeComponent>(uid, component, ref args1);
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Target_FieldComponentState : 
    IComponentDeltaState<EyeComponent.EyeComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public NetEntity? Target;

    public void ApplyToFullState(EyeComponent.EyeComponent_AutoState fullState)
    {
      fullState.Target = this.Target;
    }

    public EyeComponent.EyeComponent_AutoState CreateNewFullState(
      EyeComponent.EyeComponent_AutoState fullState)
    {
      EyeComponent.EyeComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DrawFov_FieldComponentState : 
    IComponentDeltaState<EyeComponent.EyeComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool DrawFov;

    public void ApplyToFullState(EyeComponent.EyeComponent_AutoState fullState)
    {
      fullState.DrawFov = this.DrawFov;
    }

    public EyeComponent.EyeComponent_AutoState CreateNewFullState(
      EyeComponent.EyeComponent_AutoState fullState)
    {
      EyeComponent.EyeComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DrawLight_FieldComponentState : 
    IComponentDeltaState<EyeComponent.EyeComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool DrawLight;

    public void ApplyToFullState(EyeComponent.EyeComponent_AutoState fullState)
    {
      fullState.DrawLight = this.DrawLight;
    }

    public EyeComponent.EyeComponent_AutoState CreateNewFullState(
      EyeComponent.EyeComponent_AutoState fullState)
    {
      EyeComponent.EyeComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Offset_FieldComponentState : 
    IComponentDeltaState<EyeComponent.EyeComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Vector2 Offset;

    public void ApplyToFullState(EyeComponent.EyeComponent_AutoState fullState)
    {
      fullState.Offset = this.Offset;
    }

    public EyeComponent.EyeComponent_AutoState CreateNewFullState(
      EyeComponent.EyeComponent_AutoState fullState)
    {
      EyeComponent.EyeComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class VisibilityMask_FieldComponentState : 
    IComponentDeltaState<EyeComponent.EyeComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int VisibilityMask;

    public void ApplyToFullState(EyeComponent.EyeComponent_AutoState fullState)
    {
      fullState.VisibilityMask = this.VisibilityMask;
    }

    public EyeComponent.EyeComponent_AutoState CreateNewFullState(
      EyeComponent.EyeComponent_AutoState fullState)
    {
      EyeComponent.EyeComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
