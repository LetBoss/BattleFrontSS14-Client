// Decompiled with JetBrains decompiler
// Type: Content.Shared.Delivery.DeliveryComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Delivery;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
public sealed class DeliveryComponent : 
  Component,
  ISerializationGenerated<DeliveryComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsOpened;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsLocked = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BaseSpesoReward = 500;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BaseSpesoPenalty = 250;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? RecipientName;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? RecipientJobTitle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? RecipientStation;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<CargoAccountPrototype> PenaltyBankAccount = ProtoId<CargoAccountPrototype>.op_Implicit("Cargo");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool WasPenalized;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? UnlockSound = (SoundSpecifier) new SoundCollectionSpecifier("DeliveryUnlockSounds", new AudioParams?(((AudioParams) ref AudioParams.Default).WithVolume(-10f)));
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? OpenSound = (SoundSpecifier) new SoundCollectionSpecifier("DeliveryOpenSounds", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public string Container = "delivery";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DeliveryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DeliveryComponent) component;
    if (serialization.TryCustomCopy<DeliveryComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsOpened, ref flag1, hookCtx, false, context))
      flag1 = this.IsOpened;
    target.IsOpened = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsLocked, ref flag2, hookCtx, false, context))
      flag2 = this.IsLocked;
    target.IsLocked = flag2;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.BaseSpesoReward, ref num1, hookCtx, false, context))
      num1 = this.BaseSpesoReward;
    target.BaseSpesoReward = num1;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.BaseSpesoPenalty, ref num2, hookCtx, false, context))
      num2 = this.BaseSpesoPenalty;
    target.BaseSpesoPenalty = num2;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.RecipientName, ref str1, hookCtx, false, context))
      str1 = this.RecipientName;
    target.RecipientName = str1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.RecipientJobTitle, ref str2, hookCtx, false, context))
      str2 = this.RecipientJobTitle;
    target.RecipientJobTitle = str2;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.RecipientStation, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.RecipientStation, hookCtx, context, false);
    target.RecipientStation = nullable;
    ProtoId<CargoAccountPrototype> protoId = new ProtoId<CargoAccountPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<CargoAccountPrototype>>(this.PenaltyBankAccount, ref protoId, hookCtx, false, context))
      protoId = serialization.CreateCopy<ProtoId<CargoAccountPrototype>>(this.PenaltyBankAccount, hookCtx, context, false);
    target.PenaltyBankAccount = protoId;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.WasPenalized, ref flag3, hookCtx, false, context))
      flag3 = this.WasPenalized;
    target.WasPenalized = flag3;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UnlockSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.UnlockSound, hookCtx, context, false);
    target.UnlockSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.OpenSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.OpenSound, hookCtx, context, false);
    target.OpenSound = soundSpecifier2;
    string str3 = (string) null;
    if (this.Container == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Container, ref str3, hookCtx, false, context))
      str3 = this.Container;
    target.Container = str3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DeliveryComponent target,
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
    DeliveryComponent target1 = (DeliveryComponent) target;
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
    DeliveryComponent target1 = (DeliveryComponent) target;
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
    DeliveryComponent target1 = (DeliveryComponent) target;
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

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DeliveryComponent target1 = (DeliveryComponent) target;
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
  virtual DeliveryComponent Component.Instantiate() => new DeliveryComponent();

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
  public sealed class DeliveryComponent_AutoState : IComponentState
  {
    public bool IsOpened;
    public bool IsLocked;
    public int BaseSpesoReward;
    public int BaseSpesoPenalty;
    public string? RecipientName;
    public string? RecipientJobTitle;
    public NetEntity? RecipientStation;
    public ProtoId<CargoAccountPrototype> PenaltyBankAccount;
    public bool WasPenalized;

    public DeliveryComponent.DeliveryComponent_AutoState ShallowClone()
    {
      return new DeliveryComponent.DeliveryComponent_AutoState()
      {
        IsOpened = this.IsOpened,
        IsLocked = this.IsLocked,
        BaseSpesoReward = this.BaseSpesoReward,
        BaseSpesoPenalty = this.BaseSpesoPenalty,
        RecipientName = this.RecipientName,
        RecipientJobTitle = this.RecipientJobTitle,
        RecipientStation = this.RecipientStation,
        PenaltyBankAccount = this.PenaltyBankAccount,
        WasPenalized = this.WasPenalized
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeliveryComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<DeliveryComponent>(new string[9]
      {
        "IsOpened",
        "IsLocked",
        "BaseSpesoReward",
        "BaseSpesoPenalty",
        "RecipientName",
        "RecipientJobTitle",
        "RecipientStation",
        "PenaltyBankAccount",
        "WasPenalized"
      });
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DeliveryComponent, ComponentGetState>(new ComponentEventRefHandler<DeliveryComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DeliveryComponent, ComponentHandleState>(new ComponentEventRefHandler<DeliveryComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, DeliveryComponent component, ref ComponentGetState args)
    {
      IComponentDelta icomponentDelta = (IComponentDelta) component;
      if (icomponentDelta != null && GameTick.op_GreaterThan(((ComponentGetState) ref args).FromTick, component.CreationTick) && GameTick.op_GreaterThanOrEqual(icomponentDelta.LastFieldUpdate, ((ComponentGetState) ref args).FromTick))
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, ((ComponentGetState) ref args).FromTick))
        {
          case 1:
            ((ComponentGetState) ref args).State = (IComponentState) new DeliveryComponent.IsOpened_FieldComponentState()
            {
              IsOpened = component.IsOpened
            };
            return;
          case 2:
            ((ComponentGetState) ref args).State = (IComponentState) new DeliveryComponent.IsLocked_FieldComponentState()
            {
              IsLocked = component.IsLocked
            };
            return;
          case 4:
            ((ComponentGetState) ref args).State = (IComponentState) new DeliveryComponent.BaseSpesoReward_FieldComponentState()
            {
              BaseSpesoReward = component.BaseSpesoReward
            };
            return;
          case 8:
            ((ComponentGetState) ref args).State = (IComponentState) new DeliveryComponent.BaseSpesoPenalty_FieldComponentState()
            {
              BaseSpesoPenalty = component.BaseSpesoPenalty
            };
            return;
          case 16 /*0x10*/:
            ((ComponentGetState) ref args).State = (IComponentState) new DeliveryComponent.RecipientName_FieldComponentState()
            {
              RecipientName = component.RecipientName
            };
            return;
          case 32 /*0x20*/:
            ((ComponentGetState) ref args).State = (IComponentState) new DeliveryComponent.RecipientJobTitle_FieldComponentState()
            {
              RecipientJobTitle = component.RecipientJobTitle
            };
            return;
          case 64 /*0x40*/:
            ((ComponentGetState) ref args).State = (IComponentState) new DeliveryComponent.RecipientStation_FieldComponentState()
            {
              RecipientStation = this.GetNetEntity(component.RecipientStation, (MetaDataComponent) null)
            };
            return;
          case 128 /*0x80*/:
            ((ComponentGetState) ref args).State = (IComponentState) new DeliveryComponent.PenaltyBankAccount_FieldComponentState()
            {
              PenaltyBankAccount = component.PenaltyBankAccount
            };
            return;
          case 256 /*0x0100*/:
            ((ComponentGetState) ref args).State = (IComponentState) new DeliveryComponent.WasPenalized_FieldComponentState()
            {
              WasPenalized = component.WasPenalized
            };
            return;
        }
      }
      ((ComponentGetState) ref args).State = (IComponentState) new DeliveryComponent.DeliveryComponent_AutoState()
      {
        IsOpened = component.IsOpened,
        IsLocked = component.IsLocked,
        BaseSpesoReward = component.BaseSpesoReward,
        BaseSpesoPenalty = component.BaseSpesoPenalty,
        RecipientName = component.RecipientName,
        RecipientJobTitle = component.RecipientJobTitle,
        RecipientStation = this.GetNetEntity(component.RecipientStation, (MetaDataComponent) null),
        PenaltyBankAccount = component.PenaltyBankAccount,
        WasPenalized = component.WasPenalized
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DeliveryComponent component,
      ref ComponentHandleState args)
    {
      switch (((ComponentHandleState) ref args).Current)
      {
        case DeliveryComponent.IsOpened_FieldComponentState fieldComponentState1:
          component.IsOpened = fieldComponentState1.IsOpened;
          break;
        case DeliveryComponent.IsLocked_FieldComponentState fieldComponentState2:
          component.IsLocked = fieldComponentState2.IsLocked;
          break;
        case DeliveryComponent.BaseSpesoReward_FieldComponentState fieldComponentState3:
          component.BaseSpesoReward = fieldComponentState3.BaseSpesoReward;
          break;
        case DeliveryComponent.BaseSpesoPenalty_FieldComponentState fieldComponentState4:
          component.BaseSpesoPenalty = fieldComponentState4.BaseSpesoPenalty;
          break;
        case DeliveryComponent.RecipientName_FieldComponentState fieldComponentState5:
          component.RecipientName = fieldComponentState5.RecipientName;
          break;
        case DeliveryComponent.RecipientJobTitle_FieldComponentState fieldComponentState6:
          component.RecipientJobTitle = fieldComponentState6.RecipientJobTitle;
          break;
        case DeliveryComponent.RecipientStation_FieldComponentState fieldComponentState7:
          component.RecipientStation = this.EnsureEntity<DeliveryComponent>(fieldComponentState7.RecipientStation, uid);
          break;
        case DeliveryComponent.PenaltyBankAccount_FieldComponentState fieldComponentState8:
          component.PenaltyBankAccount = fieldComponentState8.PenaltyBankAccount;
          break;
        case DeliveryComponent.WasPenalized_FieldComponentState fieldComponentState9:
          component.WasPenalized = fieldComponentState9.WasPenalized;
          break;
        case DeliveryComponent.DeliveryComponent_AutoState componentAutoState:
          component.IsOpened = componentAutoState.IsOpened;
          component.IsLocked = componentAutoState.IsLocked;
          component.BaseSpesoReward = componentAutoState.BaseSpesoReward;
          component.BaseSpesoPenalty = componentAutoState.BaseSpesoPenalty;
          component.RecipientName = componentAutoState.RecipientName;
          component.RecipientJobTitle = componentAutoState.RecipientJobTitle;
          component.RecipientStation = this.EnsureEntity<DeliveryComponent>(componentAutoState.RecipientStation, uid);
          component.PenaltyBankAccount = componentAutoState.PenaltyBankAccount;
          component.WasPenalized = componentAutoState.WasPenalized;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class IsOpened_FieldComponentState : 
    IComponentDeltaState<DeliveryComponent.DeliveryComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool IsOpened;

    public void ApplyToFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      fullState.IsOpened = this.IsOpened;
    }

    public DeliveryComponent.DeliveryComponent_AutoState CreateNewFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      DeliveryComponent.DeliveryComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class IsLocked_FieldComponentState : 
    IComponentDeltaState<DeliveryComponent.DeliveryComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool IsLocked;

    public void ApplyToFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      fullState.IsLocked = this.IsLocked;
    }

    public DeliveryComponent.DeliveryComponent_AutoState CreateNewFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      DeliveryComponent.DeliveryComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BaseSpesoReward_FieldComponentState : 
    IComponentDeltaState<DeliveryComponent.DeliveryComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int BaseSpesoReward;

    public void ApplyToFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      fullState.BaseSpesoReward = this.BaseSpesoReward;
    }

    public DeliveryComponent.DeliveryComponent_AutoState CreateNewFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      DeliveryComponent.DeliveryComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BaseSpesoPenalty_FieldComponentState : 
    IComponentDeltaState<DeliveryComponent.DeliveryComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int BaseSpesoPenalty;

    public void ApplyToFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      fullState.BaseSpesoPenalty = this.BaseSpesoPenalty;
    }

    public DeliveryComponent.DeliveryComponent_AutoState CreateNewFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      DeliveryComponent.DeliveryComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class RecipientName_FieldComponentState : 
    IComponentDeltaState<DeliveryComponent.DeliveryComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public string? RecipientName;

    public void ApplyToFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      fullState.RecipientName = this.RecipientName;
    }

    public DeliveryComponent.DeliveryComponent_AutoState CreateNewFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      DeliveryComponent.DeliveryComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class RecipientJobTitle_FieldComponentState : 
    IComponentDeltaState<DeliveryComponent.DeliveryComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public string? RecipientJobTitle;

    public void ApplyToFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      fullState.RecipientJobTitle = this.RecipientJobTitle;
    }

    public DeliveryComponent.DeliveryComponent_AutoState CreateNewFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      DeliveryComponent.DeliveryComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class RecipientStation_FieldComponentState : 
    IComponentDeltaState<DeliveryComponent.DeliveryComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public NetEntity? RecipientStation;

    public void ApplyToFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      fullState.RecipientStation = this.RecipientStation;
    }

    public DeliveryComponent.DeliveryComponent_AutoState CreateNewFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      DeliveryComponent.DeliveryComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class PenaltyBankAccount_FieldComponentState : 
    IComponentDeltaState<DeliveryComponent.DeliveryComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ProtoId<CargoAccountPrototype> PenaltyBankAccount;

    public void ApplyToFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      fullState.PenaltyBankAccount = this.PenaltyBankAccount;
    }

    public DeliveryComponent.DeliveryComponent_AutoState CreateNewFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      DeliveryComponent.DeliveryComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class WasPenalized_FieldComponentState : 
    IComponentDeltaState<DeliveryComponent.DeliveryComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool WasPenalized;

    public void ApplyToFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      fullState.WasPenalized = this.WasPenalized;
    }

    public DeliveryComponent.DeliveryComponent_AutoState CreateNewFullState(
      DeliveryComponent.DeliveryComponent_AutoState fullState)
    {
      DeliveryComponent.DeliveryComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
