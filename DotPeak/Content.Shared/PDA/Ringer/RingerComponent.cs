// Decompiled with JetBrains decompiler
// Type: Content.Shared.PDA.Ringer.RingerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.PDA.Ringer;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedRingerSystem)})]
[AutoGenerateComponentState(true, true)]
[AutoGenerateComponentPause]
public sealed class RingerComponent : 
  Component,
  ISerializationGenerated<RingerComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Note[] Ringtone = new Note[6];
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  [AutoNetworkedField]
  public TimeSpan NextRingtoneSetTime;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  [AutoNetworkedField]
  public TimeSpan? NextNoteTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Cooldown = TimeSpan.FromMilliseconds(250L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int NoteCount;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Volume = -4f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Active;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RingerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RingerComponent) target1;
    if (serialization.TryCustomCopy<RingerComponent>(this, ref target, hookCtx, false, context))
      return;
    Note[] target2 = (Note[]) null;
    if (this.Ringtone == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Note[]>(this.Ringtone, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Note[]>(this.Ringtone, hookCtx, context);
    target.Ringtone = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextRingtoneSetTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.NextRingtoneSetTime, hookCtx, context);
    target.NextRingtoneSetTime = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextNoteTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.NextNoteTime, hookCtx, context);
    target.NextNoteTime = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.NoteCount, ref target6, hookCtx, false, context))
      target6 = this.NoteCount;
    target.NoteCount = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target7, hookCtx, false, context))
      target7 = this.Range;
    target.Range = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Volume, ref target8, hookCtx, false, context))
      target8 = this.Volume;
    target.Volume = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.Active, ref target9, hookCtx, false, context))
      target9 = this.Active;
    target.Active = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RingerComponent target,
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
    RingerComponent target1 = (RingerComponent) target;
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
    RingerComponent target1 = (RingerComponent) target;
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
    RingerComponent target1 = (RingerComponent) target;
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
    RingerComponent target1 = (RingerComponent) target;
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
  virtual RingerComponent Component.Instantiate() => new RingerComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RingerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RingerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<RingerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      RingerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextRingtoneSetTime += args.PausedTime;
      if (component.NextNoteTime.HasValue)
        component.NextNoteTime = new TimeSpan?(component.NextNoteTime.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RingerComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    Note[] Ringtone;
    public TimeSpan NextRingtoneSetTime;
    public TimeSpan? NextNoteTime;
    public int NoteCount;
    public float Range;
    public float Volume;
    public bool Active;

    public RingerComponent.RingerComponent_AutoState ShallowClone()
    {
      return new RingerComponent.RingerComponent_AutoState()
      {
        Ringtone = this.Ringtone,
        NextRingtoneSetTime = this.NextRingtoneSetTime,
        NextNoteTime = this.NextNoteTime,
        NoteCount = this.NoteCount,
        Range = this.Range,
        Volume = this.Volume,
        Active = this.Active
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RingerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<RingerComponent>("Ringtone", "NextRingtoneSetTime", "NextNoteTime", "NoteCount", "Range", "Volume", "Active");
      this.SubscribeLocalEvent<RingerComponent, ComponentGetState>(new ComponentEventRefHandler<RingerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RingerComponent, ComponentHandleState>(new ComponentEventRefHandler<RingerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, RingerComponent component, ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        uint modifiedFields = this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick);
        if (modifiedFields <= 8U)
        {
          switch ((int) modifiedFields - 1)
          {
            case 0:
              args.State = (IComponentState) new RingerComponent.Ringtone_FieldComponentState()
              {
                Ringtone = component.Ringtone
              };
              return;
            case 1:
              args.State = (IComponentState) new RingerComponent.NextRingtoneSetTime_FieldComponentState()
              {
                NextRingtoneSetTime = component.NextRingtoneSetTime
              };
              return;
            case 2:
              break;
            case 3:
              args.State = (IComponentState) new RingerComponent.NextNoteTime_FieldComponentState()
              {
                NextNoteTime = component.NextNoteTime
              };
              return;
            default:
              if (modifiedFields == 8U)
              {
                args.State = (IComponentState) new RingerComponent.NoteCount_FieldComponentState()
                {
                  NoteCount = component.NoteCount
                };
                return;
              }
              break;
          }
        }
        else if (modifiedFields != 16U /*0x10*/)
        {
          if (modifiedFields != 32U /*0x20*/)
          {
            if (modifiedFields == 64U /*0x40*/)
            {
              args.State = (IComponentState) new RingerComponent.Active_FieldComponentState()
              {
                Active = component.Active
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new RingerComponent.Volume_FieldComponentState()
            {
              Volume = component.Volume
            };
            return;
          }
        }
        else
        {
          args.State = (IComponentState) new RingerComponent.Range_FieldComponentState()
          {
            Range = component.Range
          };
          return;
        }
      }
      args.State = (IComponentState) new RingerComponent.RingerComponent_AutoState()
      {
        Ringtone = component.Ringtone,
        NextRingtoneSetTime = component.NextRingtoneSetTime,
        NextNoteTime = component.NextNoteTime,
        NoteCount = component.NoteCount,
        Range = component.Range,
        Volume = component.Volume,
        Active = component.Active
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RingerComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case RingerComponent.Ringtone_FieldComponentState fieldComponentState1:
          component.Ringtone = fieldComponentState1.Ringtone;
          break;
        case RingerComponent.NextRingtoneSetTime_FieldComponentState fieldComponentState2:
          component.NextRingtoneSetTime = fieldComponentState2.NextRingtoneSetTime;
          break;
        case RingerComponent.NextNoteTime_FieldComponentState fieldComponentState3:
          component.NextNoteTime = fieldComponentState3.NextNoteTime;
          break;
        case RingerComponent.NoteCount_FieldComponentState fieldComponentState4:
          component.NoteCount = fieldComponentState4.NoteCount;
          break;
        case RingerComponent.Range_FieldComponentState fieldComponentState5:
          component.Range = fieldComponentState5.Range;
          break;
        case RingerComponent.Volume_FieldComponentState fieldComponentState6:
          component.Volume = fieldComponentState6.Volume;
          break;
        case RingerComponent.Active_FieldComponentState fieldComponentState7:
          component.Active = fieldComponentState7.Active;
          break;
        case RingerComponent.RingerComponent_AutoState componentAutoState:
          component.Ringtone = componentAutoState.Ringtone;
          component.NextRingtoneSetTime = componentAutoState.NextRingtoneSetTime;
          component.NextNoteTime = componentAutoState.NextNoteTime;
          component.NoteCount = componentAutoState.NoteCount;
          component.Range = componentAutoState.Range;
          component.Volume = componentAutoState.Volume;
          component.Active = componentAutoState.Active;
          break;
        default:
          return;
      }
      IComponentState current = args.Current;
      if (current == null)
        return;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, RingerComponent>(uid, component, ref args1);
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Ringtone_FieldComponentState : 
    IComponentDeltaState<RingerComponent.RingerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Note[] Ringtone;

    public void ApplyToFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      fullState.Ringtone = this.Ringtone;
    }

    public RingerComponent.RingerComponent_AutoState CreateNewFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      RingerComponent.RingerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NextRingtoneSetTime_FieldComponentState : 
    IComponentDeltaState<RingerComponent.RingerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan NextRingtoneSetTime;

    public void ApplyToFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      fullState.NextRingtoneSetTime = this.NextRingtoneSetTime;
    }

    public RingerComponent.RingerComponent_AutoState CreateNewFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      RingerComponent.RingerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NextNoteTime_FieldComponentState : 
    IComponentDeltaState<RingerComponent.RingerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan? NextNoteTime;

    public void ApplyToFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      fullState.NextNoteTime = this.NextNoteTime;
    }

    public RingerComponent.RingerComponent_AutoState CreateNewFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      RingerComponent.RingerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NoteCount_FieldComponentState : 
    IComponentDeltaState<RingerComponent.RingerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int NoteCount;

    public void ApplyToFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      fullState.NoteCount = this.NoteCount;
    }

    public RingerComponent.RingerComponent_AutoState CreateNewFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      RingerComponent.RingerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Range_FieldComponentState : 
    IComponentDeltaState<RingerComponent.RingerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float Range;

    public void ApplyToFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      fullState.Range = this.Range;
    }

    public RingerComponent.RingerComponent_AutoState CreateNewFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      RingerComponent.RingerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Volume_FieldComponentState : 
    IComponentDeltaState<RingerComponent.RingerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float Volume;

    public void ApplyToFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      fullState.Volume = this.Volume;
    }

    public RingerComponent.RingerComponent_AutoState CreateNewFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      RingerComponent.RingerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Active_FieldComponentState : 
    IComponentDeltaState<RingerComponent.RingerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Active;

    public void ApplyToFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      fullState.Active = this.Active;
    }

    public RingerComponent.RingerComponent_AutoState CreateNewFullState(
      RingerComponent.RingerComponent_AutoState fullState)
    {
      RingerComponent.RingerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
