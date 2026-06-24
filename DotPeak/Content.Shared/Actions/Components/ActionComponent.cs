// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.Components.ActionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Actions.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedActionsSystem)})]
[AutoGenerateComponentState(true, true)]
[EntityCategory(new string[] {"Actions"})]
public sealed class ActionComponent : 
  Component,
  ISerializationGenerated<ActionComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier? Icon;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SpriteSpecifier? IconOn;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier? BackgroundOn;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color IconColor = Color.White;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color OriginalIconColor;
  [DataField(null, false, 1, false, false, null)]
  public Color DisabledIconColor = Color.DimGray;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<string> Keywords = new HashSet<string>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Toggled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ActionCooldown? Cooldown;
  [DataField(null, false, 1, false, false, null)]
  public bool StartDelay;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? UseDelay;
  [Access(new Type[] {typeof (ActionContainerSystem), typeof (SharedActionsSystem)})]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Container;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? EntIcon;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CheckCanInteract = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CheckConsciousness = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ClientExclusive;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Priority;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? AttachedEntity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RaiseOnUser;
  [DataField(null, false, 1, false, false, null)]
  [Obsolete("This datafield will be reworked in an upcoming action refactor")]
  public bool RaiseOnAction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AutoPopulate = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Temporary;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ItemActionIconStyle ItemIconStyle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound;

  public EntityUid? EntityIcon
  {
    get
    {
      if (this.EntIcon.HasValue)
        return this.EntIcon;
      EntityUid? attachedEntity = this.AttachedEntity;
      EntityUid? container = this.Container;
      return (attachedEntity.HasValue == container.HasValue ? (attachedEntity.HasValue ? (EntityUid.op_Inequality(attachedEntity.GetValueOrDefault(), container.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0 ? this.Container : new EntityUid?();
    }
    set => this.EntIcon = value;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ActionComponent) component;
    if (serialization.TryCustomCopy<ActionComponent>(this, ref target, hookCtx, false, context))
      return;
    SpriteSpecifier spriteSpecifier1 = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Icon, ref spriteSpecifier1, hookCtx, true, context))
      spriteSpecifier1 = serialization.CreateCopy<SpriteSpecifier>(this.Icon, hookCtx, context, false);
    target.Icon = spriteSpecifier1;
    SpriteSpecifier spriteSpecifier2 = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.IconOn, ref spriteSpecifier2, hookCtx, true, context))
      spriteSpecifier2 = serialization.CreateCopy<SpriteSpecifier>(this.IconOn, hookCtx, context, false);
    target.IconOn = spriteSpecifier2;
    SpriteSpecifier spriteSpecifier3 = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.BackgroundOn, ref spriteSpecifier3, hookCtx, true, context))
      spriteSpecifier3 = serialization.CreateCopy<SpriteSpecifier>(this.BackgroundOn, hookCtx, context, false);
    target.BackgroundOn = spriteSpecifier3;
    Color color1 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.IconColor, ref color1, hookCtx, false, context))
      color1 = serialization.CreateCopy<Color>(this.IconColor, hookCtx, context, false);
    target.IconColor = color1;
    Color color2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.OriginalIconColor, ref color2, hookCtx, false, context))
      color2 = serialization.CreateCopy<Color>(this.OriginalIconColor, hookCtx, context, false);
    target.OriginalIconColor = color2;
    Color color3 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.DisabledIconColor, ref color3, hookCtx, false, context))
      color3 = serialization.CreateCopy<Color>(this.DisabledIconColor, hookCtx, context, false);
    target.DisabledIconColor = color3;
    HashSet<string> stringSet = (HashSet<string>) null;
    if (this.Keywords == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.Keywords, ref stringSet, hookCtx, true, context))
      stringSet = serialization.CreateCopy<HashSet<string>>(this.Keywords, hookCtx, context, false);
    target.Keywords = stringSet;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag1, hookCtx, false, context))
      flag1 = this.Enabled;
    target.Enabled = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Toggled, ref flag2, hookCtx, false, context))
      flag2 = this.Toggled;
    target.Toggled = flag2;
    ActionCooldown? nullable1 = new ActionCooldown?();
    if (!serialization.TryCustomCopy<ActionCooldown?>(this.Cooldown, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<ActionCooldown?>(this.Cooldown, hookCtx, context, false);
    target.Cooldown = nullable1;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.StartDelay, ref flag3, hookCtx, false, context))
      flag3 = this.StartDelay;
    target.StartDelay = flag3;
    TimeSpan? nullable2 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.UseDelay, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<TimeSpan?>(this.UseDelay, hookCtx, context, false);
    target.UseDelay = nullable2;
    EntityUid? nullable3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Container, ref nullable3, hookCtx, false, context))
      nullable3 = serialization.CreateCopy<EntityUid?>(this.Container, hookCtx, context, false);
    target.Container = nullable3;
    EntityUid? nullable4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.EntIcon, ref nullable4, hookCtx, false, context))
      nullable4 = serialization.CreateCopy<EntityUid?>(this.EntIcon, hookCtx, context, false);
    target.EntIcon = nullable4;
    bool flag4 = false;
    if (!serialization.TryCustomCopy<bool>(this.CheckCanInteract, ref flag4, hookCtx, false, context))
      flag4 = this.CheckCanInteract;
    target.CheckCanInteract = flag4;
    bool flag5 = false;
    if (!serialization.TryCustomCopy<bool>(this.CheckConsciousness, ref flag5, hookCtx, false, context))
      flag5 = this.CheckConsciousness;
    target.CheckConsciousness = flag5;
    bool flag6 = false;
    if (!serialization.TryCustomCopy<bool>(this.ClientExclusive, ref flag6, hookCtx, false, context))
      flag6 = this.ClientExclusive;
    target.ClientExclusive = flag6;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.Priority, ref num, hookCtx, false, context))
      num = this.Priority;
    target.Priority = num;
    EntityUid? nullable5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.AttachedEntity, ref nullable5, hookCtx, false, context))
      nullable5 = serialization.CreateCopy<EntityUid?>(this.AttachedEntity, hookCtx, context, false);
    target.AttachedEntity = nullable5;
    bool flag7 = false;
    if (!serialization.TryCustomCopy<bool>(this.RaiseOnUser, ref flag7, hookCtx, false, context))
      flag7 = this.RaiseOnUser;
    target.RaiseOnUser = flag7;
    bool flag8 = false;
    if (!serialization.TryCustomCopy<bool>(this.RaiseOnAction, ref flag8, hookCtx, false, context))
      flag8 = this.RaiseOnAction;
    target.RaiseOnAction = flag8;
    bool flag9 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoPopulate, ref flag9, hookCtx, false, context))
      flag9 = this.AutoPopulate;
    target.AutoPopulate = flag9;
    bool flag10 = false;
    if (!serialization.TryCustomCopy<bool>(this.Temporary, ref flag10, hookCtx, false, context))
      flag10 = this.Temporary;
    target.Temporary = flag10;
    ItemActionIconStyle itemActionIconStyle = ItemActionIconStyle.BigItem;
    if (!serialization.TryCustomCopy<ItemActionIconStyle>(this.ItemIconStyle, ref itemActionIconStyle, hookCtx, false, context))
      itemActionIconStyle = this.ItemIconStyle;
    target.ItemIconStyle = itemActionIconStyle;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context, false);
    target.Sound = soundSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActionComponent target,
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
    ActionComponent target1 = (ActionComponent) target;
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
    ActionComponent target1 = (ActionComponent) target;
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
    ActionComponent target1 = (ActionComponent) target;
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
    ActionComponent target1 = (ActionComponent) target;
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
  virtual ActionComponent Component.Instantiate() => new ActionComponent();

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
  public sealed class ActionComponent_AutoState : IComponentState
  {
    public SpriteSpecifier? Icon;
    public SpriteSpecifier? IconOn;
    public Color IconColor;
    public Color OriginalIconColor;
    public HashSet<string> Keywords;
    public bool Enabled;
    public bool Toggled;
    public ActionCooldown? Cooldown;
    public TimeSpan? UseDelay;
    public NetEntity? Container;
    public NetEntity? EntIcon;
    public bool CheckCanInteract;
    public bool CheckConsciousness;
    public bool ClientExclusive;
    public int Priority;
    public NetEntity? AttachedEntity;
    public bool RaiseOnUser;
    public bool AutoPopulate;
    public bool Temporary;
    public ItemActionIconStyle ItemIconStyle;
    public SoundSpecifier? Sound;

    public ActionComponent.ActionComponent_AutoState ShallowClone()
    {
      return new ActionComponent.ActionComponent_AutoState()
      {
        Icon = this.Icon,
        IconOn = this.IconOn,
        IconColor = this.IconColor,
        OriginalIconColor = this.OriginalIconColor,
        Keywords = this.Keywords,
        Enabled = this.Enabled,
        Toggled = this.Toggled,
        Cooldown = this.Cooldown,
        UseDelay = this.UseDelay,
        Container = this.Container,
        EntIcon = this.EntIcon,
        CheckCanInteract = this.CheckCanInteract,
        CheckConsciousness = this.CheckConsciousness,
        ClientExclusive = this.ClientExclusive,
        Priority = this.Priority,
        AttachedEntity = this.AttachedEntity,
        RaiseOnUser = this.RaiseOnUser,
        AutoPopulate = this.AutoPopulate,
        Temporary = this.Temporary,
        ItemIconStyle = this.ItemIconStyle,
        Sound = this.Sound
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ActionComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<ActionComponent>(new string[21]
      {
        "Icon",
        "IconOn",
        "IconColor",
        "OriginalIconColor",
        "Keywords",
        "Enabled",
        "Toggled",
        "Cooldown",
        "UseDelay",
        "Container",
        "EntIcon",
        "CheckCanInteract",
        "CheckConsciousness",
        "ClientExclusive",
        "Priority",
        "AttachedEntity",
        "RaiseOnUser",
        "AutoPopulate",
        "Temporary",
        "ItemIconStyle",
        "Sound"
      });
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ActionComponent, ComponentGetState>(new ComponentEventRefHandler<ActionComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ActionComponent, ComponentHandleState>(new ComponentEventRefHandler<ActionComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, ActionComponent component, ref ComponentGetState args)
    {
      IComponentDelta icomponentDelta = (IComponentDelta) component;
      if (icomponentDelta != null && GameTick.op_GreaterThan(((ComponentGetState) ref args).FromTick, component.CreationTick) && GameTick.op_GreaterThanOrEqual(icomponentDelta.LastFieldUpdate, ((ComponentGetState) ref args).FromTick))
      {
        uint modifiedFields = this.EntityManager.GetModifiedFields((IComponentDelta) component, ((ComponentGetState) ref args).FromTick);
        if (modifiedFields <= 1024U /*0x0400*/)
        {
          if (modifiedFields <= 32U /*0x20*/)
          {
            if (modifiedFields <= 8U)
            {
              switch ((int) modifiedFields - 1)
              {
                case 0:
                  ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.Icon_FieldComponentState()
                  {
                    Icon = component.Icon
                  };
                  return;
                case 1:
                  ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.IconOn_FieldComponentState()
                  {
                    IconOn = component.IconOn
                  };
                  return;
                case 2:
                  break;
                case 3:
                  ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.IconColor_FieldComponentState()
                  {
                    IconColor = component.IconColor
                  };
                  return;
                default:
                  if (modifiedFields == 8U)
                  {
                    ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.OriginalIconColor_FieldComponentState()
                    {
                      OriginalIconColor = component.OriginalIconColor
                    };
                    return;
                  }
                  break;
              }
            }
            else if (modifiedFields != 16U /*0x10*/)
            {
              if (modifiedFields == 32U /*0x20*/)
              {
                ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.Enabled_FieldComponentState()
                {
                  Enabled = component.Enabled
                };
                return;
              }
            }
            else
            {
              ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.Keywords_FieldComponentState()
              {
                Keywords = component.Keywords
              };
              return;
            }
          }
          else if (modifiedFields <= 128U /*0x80*/)
          {
            if (modifiedFields != 64U /*0x40*/)
            {
              if (modifiedFields == 128U /*0x80*/)
              {
                ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.Cooldown_FieldComponentState()
                {
                  Cooldown = component.Cooldown
                };
                return;
              }
            }
            else
            {
              ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.Toggled_FieldComponentState()
              {
                Toggled = component.Toggled
              };
              return;
            }
          }
          else if (modifiedFields != 256U /*0x0100*/)
          {
            if (modifiedFields != 512U /*0x0200*/)
            {
              if (modifiedFields == 1024U /*0x0400*/)
              {
                ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.EntIcon_FieldComponentState()
                {
                  EntIcon = this.GetNetEntity(component.EntIcon, (MetaDataComponent) null)
                };
                return;
              }
            }
            else
            {
              ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.Container_FieldComponentState()
              {
                Container = this.GetNetEntity(component.Container, (MetaDataComponent) null)
              };
              return;
            }
          }
          else
          {
            ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.UseDelay_FieldComponentState()
            {
              UseDelay = component.UseDelay
            };
            return;
          }
        }
        else if (modifiedFields <= 32768U /*0x8000*/)
        {
          if (modifiedFields <= 4096U /*0x1000*/)
          {
            if (modifiedFields != 2048U /*0x0800*/)
            {
              if (modifiedFields == 4096U /*0x1000*/)
              {
                ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.CheckConsciousness_FieldComponentState()
                {
                  CheckConsciousness = component.CheckConsciousness
                };
                return;
              }
            }
            else
            {
              ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.CheckCanInteract_FieldComponentState()
              {
                CheckCanInteract = component.CheckCanInteract
              };
              return;
            }
          }
          else if (modifiedFields != 8192U /*0x2000*/)
          {
            if (modifiedFields != 16384U /*0x4000*/)
            {
              if (modifiedFields == 32768U /*0x8000*/)
              {
                ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.AttachedEntity_FieldComponentState()
                {
                  AttachedEntity = this.GetNetEntity(component.AttachedEntity, (MetaDataComponent) null)
                };
                return;
              }
            }
            else
            {
              ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.Priority_FieldComponentState()
              {
                Priority = component.Priority
              };
              return;
            }
          }
          else
          {
            ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.ClientExclusive_FieldComponentState()
            {
              ClientExclusive = component.ClientExclusive
            };
            return;
          }
        }
        else if (modifiedFields <= 131072U /*0x020000*/)
        {
          if (modifiedFields != 65536U /*0x010000*/)
          {
            if (modifiedFields == 131072U /*0x020000*/)
            {
              ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.AutoPopulate_FieldComponentState()
              {
                AutoPopulate = component.AutoPopulate
              };
              return;
            }
          }
          else
          {
            ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.RaiseOnUser_FieldComponentState()
            {
              RaiseOnUser = component.RaiseOnUser
            };
            return;
          }
        }
        else if (modifiedFields != 262144U /*0x040000*/)
        {
          if (modifiedFields != 524288U /*0x080000*/)
          {
            if (modifiedFields == 1048576U /*0x100000*/)
            {
              ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.Sound_FieldComponentState()
              {
                Sound = component.Sound
              };
              return;
            }
          }
          else
          {
            ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.ItemIconStyle_FieldComponentState()
            {
              ItemIconStyle = component.ItemIconStyle
            };
            return;
          }
        }
        else
        {
          ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.Temporary_FieldComponentState()
          {
            Temporary = component.Temporary
          };
          return;
        }
      }
      ((ComponentGetState) ref args).State = (IComponentState) new ActionComponent.ActionComponent_AutoState()
      {
        Icon = component.Icon,
        IconOn = component.IconOn,
        IconColor = component.IconColor,
        OriginalIconColor = component.OriginalIconColor,
        Keywords = component.Keywords,
        Enabled = component.Enabled,
        Toggled = component.Toggled,
        Cooldown = component.Cooldown,
        UseDelay = component.UseDelay,
        Container = this.GetNetEntity(component.Container, (MetaDataComponent) null),
        EntIcon = this.GetNetEntity(component.EntIcon, (MetaDataComponent) null),
        CheckCanInteract = component.CheckCanInteract,
        CheckConsciousness = component.CheckConsciousness,
        ClientExclusive = component.ClientExclusive,
        Priority = component.Priority,
        AttachedEntity = this.GetNetEntity(component.AttachedEntity, (MetaDataComponent) null),
        RaiseOnUser = component.RaiseOnUser,
        AutoPopulate = component.AutoPopulate,
        Temporary = component.Temporary,
        ItemIconStyle = component.ItemIconStyle,
        Sound = component.Sound
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ActionComponent component,
      ref ComponentHandleState args)
    {
      switch (((ComponentHandleState) ref args).Current)
      {
        case ActionComponent.Icon_FieldComponentState fieldComponentState1:
          component.Icon = fieldComponentState1.Icon;
          break;
        case ActionComponent.IconOn_FieldComponentState fieldComponentState2:
          component.IconOn = fieldComponentState2.IconOn;
          break;
        case ActionComponent.IconColor_FieldComponentState fieldComponentState3:
          component.IconColor = fieldComponentState3.IconColor;
          break;
        case ActionComponent.OriginalIconColor_FieldComponentState fieldComponentState4:
          component.OriginalIconColor = fieldComponentState4.OriginalIconColor;
          break;
        case ActionComponent.Keywords_FieldComponentState fieldComponentState5:
          HashSet<string> keywords = fieldComponentState5.Keywords;
          component.Keywords = keywords != null ? new HashSet<string>((IEnumerable<string>) keywords) : (HashSet<string>) null;
          break;
        case ActionComponent.Enabled_FieldComponentState fieldComponentState6:
          component.Enabled = fieldComponentState6.Enabled;
          break;
        case ActionComponent.Toggled_FieldComponentState fieldComponentState7:
          component.Toggled = fieldComponentState7.Toggled;
          break;
        case ActionComponent.Cooldown_FieldComponentState fieldComponentState8:
          component.Cooldown = fieldComponentState8.Cooldown;
          break;
        case ActionComponent.UseDelay_FieldComponentState fieldComponentState9:
          component.UseDelay = fieldComponentState9.UseDelay;
          break;
        case ActionComponent.Container_FieldComponentState fieldComponentState10:
          component.Container = this.EnsureEntity<ActionComponent>(fieldComponentState10.Container, uid);
          break;
        case ActionComponent.EntIcon_FieldComponentState fieldComponentState11:
          component.EntIcon = this.EnsureEntity<ActionComponent>(fieldComponentState11.EntIcon, uid);
          break;
        case ActionComponent.CheckCanInteract_FieldComponentState fieldComponentState12:
          component.CheckCanInteract = fieldComponentState12.CheckCanInteract;
          break;
        case ActionComponent.CheckConsciousness_FieldComponentState fieldComponentState13:
          component.CheckConsciousness = fieldComponentState13.CheckConsciousness;
          break;
        case ActionComponent.ClientExclusive_FieldComponentState fieldComponentState14:
          component.ClientExclusive = fieldComponentState14.ClientExclusive;
          break;
        case ActionComponent.Priority_FieldComponentState fieldComponentState15:
          component.Priority = fieldComponentState15.Priority;
          break;
        case ActionComponent.AttachedEntity_FieldComponentState fieldComponentState16:
          component.AttachedEntity = this.EnsureEntity<ActionComponent>(fieldComponentState16.AttachedEntity, uid);
          break;
        case ActionComponent.RaiseOnUser_FieldComponentState fieldComponentState17:
          component.RaiseOnUser = fieldComponentState17.RaiseOnUser;
          break;
        case ActionComponent.AutoPopulate_FieldComponentState fieldComponentState18:
          component.AutoPopulate = fieldComponentState18.AutoPopulate;
          break;
        case ActionComponent.Temporary_FieldComponentState fieldComponentState19:
          component.Temporary = fieldComponentState19.Temporary;
          break;
        case ActionComponent.ItemIconStyle_FieldComponentState fieldComponentState20:
          component.ItemIconStyle = fieldComponentState20.ItemIconStyle;
          break;
        case ActionComponent.Sound_FieldComponentState fieldComponentState21:
          component.Sound = fieldComponentState21.Sound;
          break;
        case ActionComponent.ActionComponent_AutoState componentAutoState:
          component.Icon = componentAutoState.Icon;
          component.IconOn = componentAutoState.IconOn;
          component.IconColor = componentAutoState.IconColor;
          component.OriginalIconColor = componentAutoState.OriginalIconColor;
          component.Keywords = componentAutoState.Keywords == null ? (HashSet<string>) null : new HashSet<string>((IEnumerable<string>) componentAutoState.Keywords);
          component.Enabled = componentAutoState.Enabled;
          component.Toggled = componentAutoState.Toggled;
          component.Cooldown = componentAutoState.Cooldown;
          component.UseDelay = componentAutoState.UseDelay;
          component.Container = this.EnsureEntity<ActionComponent>(componentAutoState.Container, uid);
          component.EntIcon = this.EnsureEntity<ActionComponent>(componentAutoState.EntIcon, uid);
          component.CheckCanInteract = componentAutoState.CheckCanInteract;
          component.CheckConsciousness = componentAutoState.CheckConsciousness;
          component.ClientExclusive = componentAutoState.ClientExclusive;
          component.Priority = componentAutoState.Priority;
          component.AttachedEntity = this.EnsureEntity<ActionComponent>(componentAutoState.AttachedEntity, uid);
          component.RaiseOnUser = componentAutoState.RaiseOnUser;
          component.AutoPopulate = componentAutoState.AutoPopulate;
          component.Temporary = componentAutoState.Temporary;
          component.ItemIconStyle = componentAutoState.ItemIconStyle;
          component.Sound = componentAutoState.Sound;
          break;
        default:
          return;
      }
      IComponentState current = ((ComponentHandleState) ref args).Current;
      if (current == null)
        return;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, ActionComponent>(uid, component, ref handleStateEvent);
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Icon_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SpriteSpecifier? Icon;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.Icon = this.Icon;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class IconOn_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SpriteSpecifier? IconOn;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.IconOn = this.IconOn;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class IconColor_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Color IconColor;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.IconColor = this.IconColor;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class OriginalIconColor_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Color OriginalIconColor;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.OriginalIconColor = this.OriginalIconColor;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Keywords_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public HashSet<string> Keywords;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.Keywords = this.Keywords == null ? (HashSet<string>) null : new HashSet<string>((IEnumerable<string>) this.Keywords);
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Enabled_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Enabled;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.Enabled = this.Enabled;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Toggled_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Toggled;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.Toggled = this.Toggled;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Cooldown_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ActionCooldown? Cooldown;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.Cooldown = this.Cooldown;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class UseDelay_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan? UseDelay;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.UseDelay = this.UseDelay;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Container_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public NetEntity? Container;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.Container = this.Container;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class EntIcon_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public NetEntity? EntIcon;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.EntIcon = this.EntIcon;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CheckCanInteract_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool CheckCanInteract;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.CheckCanInteract = this.CheckCanInteract;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CheckConsciousness_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool CheckConsciousness;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.CheckConsciousness = this.CheckConsciousness;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ClientExclusive_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool ClientExclusive;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.ClientExclusive = this.ClientExclusive;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Priority_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int Priority;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.Priority = this.Priority;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AttachedEntity_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public NetEntity? AttachedEntity;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.AttachedEntity = this.AttachedEntity;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class RaiseOnUser_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool RaiseOnUser;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.RaiseOnUser = this.RaiseOnUser;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class AutoPopulate_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool AutoPopulate;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.AutoPopulate = this.AutoPopulate;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Temporary_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Temporary;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.Temporary = this.Temporary;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ItemIconStyle_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ItemActionIconStyle ItemIconStyle;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.ItemIconStyle = this.ItemIconStyle;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Sound_FieldComponentState : 
    IComponentDeltaState<ActionComponent.ActionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public SoundSpecifier? Sound;

    public void ApplyToFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      fullState.Sound = this.Sound;
    }

    public ActionComponent.ActionComponent_AutoState CreateNewFullState(
      ActionComponent.ActionComponent_AutoState fullState)
    {
      ActionComponent.ActionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
