// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.GasTankComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class GasTankComponent : 
  Component,
  IGasMixtureHolder,
  ISerializationGenerated<GasTankComponent>,
  ISerializationGenerated
{
  public const float MaxExplosionRange = 26f;
  private const float DefaultLowPressure = 0.0f;
  private const float DefaultOutputPressure = 101.325f;
  public int Integrity;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier RuptureSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? ConnectSound;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? DisconnectSound;
  public EntityUid? ConnectStream;
  public EntityUid? DisconnectStream;
  [DataField(null, false, 1, false, false, null)]
  public float TankLowPressure;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float OutputPressure;
  [DataField(null, false, 1, false, false, null)]
  public float MaxOutputPressure;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? User;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool CheckUser;
  [DataField(null, false, 1, false, false, null)]
  public float TankLeakPressure;
  [DataField(null, false, 1, false, false, null)]
  public float TankRupturePressure;
  [DataField(null, false, 1, false, false, null)]
  public float TankFragmentPressure;
  [DataField(null, false, 1, false, false, null)]
  public float TankFragmentScale;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId ToggleAction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ToggleActionEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsValveOpen;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ValveOutputRate;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier ValveSound;

  public bool IsLowPressure => (double) this.Air.Pressure <= (double) this.TankLowPressure;

  [DataField(null, false, 1, false, false, null)]
  public GasMixture Air { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsConnected => this.User.HasValue;

  public GasTankComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Effects/internals.ogg", new AudioParams?());
    ((SoundSpecifier) soundPathSpecifier).Params = ((AudioParams) ref AudioParams.Default).WithVolume(5f);
    this.ConnectSound = (SoundSpecifier) soundPathSpecifier;
    // ISSUE: reference to a compiler-generated field
    this.\u003CAir\u003Ek__BackingField = new GasMixture();
    this.OutputPressure = 101.325f;
    this.MaxOutputPressure = 303.974976f;
    this.TankLeakPressure = 3039.75f;
    this.TankRupturePressure = 4053f;
    this.TankFragmentPressure = 5066.25f;
    this.TankFragmentScale = 227.981247f;
    this.ToggleAction = EntProtoId.op_Implicit("ActionToggleInternals");
    this.ValveOutputRate = 100f;
    SoundCollectionSpecifier collectionSpecifier = new SoundCollectionSpecifier("valveSqueak", new AudioParams?());
    ((SoundSpecifier) collectionSpecifier).Params = ((AudioParams) ref AudioParams.Default).WithVolume(-5f);
    this.ValveSound = (SoundSpecifier) collectionSpecifier;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GasTankComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GasTankComponent) component;
    if (serialization.TryCustomCopy<GasTankComponent>(this, ref target, hookCtx, false, context))
      return;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (this.RuptureSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RuptureSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.RuptureSound, hookCtx, context, false);
    target.RuptureSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ConnectSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.ConnectSound, hookCtx, context, false);
    target.ConnectSound = soundSpecifier2;
    SoundSpecifier soundSpecifier3 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DisconnectSound, ref soundSpecifier3, hookCtx, true, context))
      soundSpecifier3 = serialization.CreateCopy<SoundSpecifier>(this.DisconnectSound, hookCtx, context, false);
    target.DisconnectSound = soundSpecifier3;
    GasMixture gasMixture = (GasMixture) null;
    if (this.Air == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<GasMixture>(this.Air, ref gasMixture, hookCtx, true, context))
    {
      if (this.Air == null)
        gasMixture = (GasMixture) null;
      else
        serialization.CopyTo<GasMixture>(this.Air, ref gasMixture, hookCtx, context, true);
    }
    target.Air = gasMixture;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TankLowPressure, ref num1, hookCtx, false, context))
      num1 = this.TankLowPressure;
    target.TankLowPressure = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OutputPressure, ref num2, hookCtx, false, context))
      num2 = this.OutputPressure;
    target.OutputPressure = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxOutputPressure, ref num3, hookCtx, false, context))
      num3 = this.MaxOutputPressure;
    target.MaxOutputPressure = num3;
    EntityUid? nullable1 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.User, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<EntityUid?>(this.User, hookCtx, context, false);
    target.User = nullable1;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TankLeakPressure, ref num4, hookCtx, false, context))
      num4 = this.TankLeakPressure;
    target.TankLeakPressure = num4;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TankRupturePressure, ref num5, hookCtx, false, context))
      num5 = this.TankRupturePressure;
    target.TankRupturePressure = num5;
    float num6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TankFragmentPressure, ref num6, hookCtx, false, context))
      num6 = this.TankFragmentPressure;
    target.TankFragmentPressure = num6;
    float num7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TankFragmentScale, ref num7, hookCtx, false, context))
      num7 = this.TankFragmentScale;
    target.TankFragmentScale = num7;
    EntProtoId entProtoId = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.ToggleAction, ref entProtoId, hookCtx, false, context))
      entProtoId = serialization.CreateCopy<EntProtoId>(this.ToggleAction, hookCtx, context, false);
    target.ToggleAction = entProtoId;
    EntityUid? nullable2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ToggleActionEntity, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<EntityUid?>(this.ToggleActionEntity, hookCtx, context, false);
    target.ToggleActionEntity = nullable2;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.IsValveOpen, ref flag, hookCtx, false, context))
      flag = this.IsValveOpen;
    target.IsValveOpen = flag;
    float num8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ValveOutputRate, ref num8, hookCtx, false, context))
      num8 = this.ValveOutputRate;
    target.ValveOutputRate = num8;
    SoundSpecifier soundSpecifier4 = (SoundSpecifier) null;
    if (this.ValveSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.ValveSound, ref soundSpecifier4, hookCtx, true, context))
      soundSpecifier4 = serialization.CreateCopy<SoundSpecifier>(this.ValveSound, hookCtx, context, false);
    target.ValveSound = soundSpecifier4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GasTankComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GasTankComponent target1 = (GasTankComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GasTankComponent target1 = (GasTankComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GasTankComponent target1 = (GasTankComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual GasTankComponent Component.Instantiate() => new GasTankComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GasTankComponent_AutoState : IComponentState
  {
    public float OutputPressure;
    public NetEntity? User;
    public NetEntity? ToggleActionEntity;
    public bool IsValveOpen;
    public float ValveOutputRate;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GasTankComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasTankComponent, ComponentGetState>(new ComponentEventRefHandler<GasTankComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasTankComponent, ComponentHandleState>(new ComponentEventRefHandler<GasTankComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, GasTankComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new GasTankComponent.GasTankComponent_AutoState()
      {
        OutputPressure = component.OutputPressure,
        User = this.GetNetEntity(component.User, (MetaDataComponent) null),
        ToggleActionEntity = this.GetNetEntity(component.ToggleActionEntity, (MetaDataComponent) null),
        IsValveOpen = component.IsValveOpen,
        ValveOutputRate = component.ValveOutputRate
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GasTankComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is GasTankComponent.GasTankComponent_AutoState current))
        return;
      component.OutputPressure = current.OutputPressure;
      component.User = this.EnsureEntity<GasTankComponent>(current.User, uid);
      component.ToggleActionEntity = this.EnsureEntity<GasTankComponent>(current.ToggleActionEntity, uid);
      component.IsValveOpen = current.IsValveOpen;
      component.ValveOutputRate = current.ValveOutputRate;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, GasTankComponent>(uid, component, ref handleStateEvent);
    }
  }
}
