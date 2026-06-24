// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Scanner.RMCStethoscopeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.UniformAccessories;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Medical.Scanner;

public sealed class RMCStethoscopeSystem : EntitySystem
{
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private InventorySystem _inventorySystem;
  [Dependency]
  private SharedContainerSystem _containerSystem;
  private static readonly EntProtoId<SkillDefinitionComponent> MedicalSkill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillMedical";
  private static readonly string[] AccessorySlots = new string[2]
  {
    "jumpsuit",
    "outerClothing"
  };

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GetVerbsEvent<ExamineVerb>>(new EntityEventHandler<GetVerbsEvent<ExamineVerb>>(this.OnGlobalStethoscopeExamineVerb), after: new Type[1]
    {
      typeof (SharedPopupSystem)
    });
    this.SubscribeLocalEvent<RMCStethoscopeComponent, AfterInteractEvent>(new ComponentEventRefHandler<RMCStethoscopeComponent, AfterInteractEvent>(this.OnStethoAfterInteract));
  }

  private void OnStethoAfterInteract(
    EntityUid uid,
    RMCStethoscopeComponent comp,
    ref AfterInteractEvent args)
  {
    if (args.Handled || !this.HasStethoscope(args.User, out EntityUid _))
      return;
    EntityUid? target1 = args.Target;
    if (!target1.HasValue)
      return;
    EntityUid user = args.User;
    target1 = args.Target;
    EntityUid target2 = target1.Value;
    this.ShowStethoPopup(user, target2);
    args.Handled = true;
  }

  private void ShowStethoPopup(EntityUid user, EntityUid target)
  {
    this._popup.PopupClient(this.GetStethoscopeResults(target, new EntityUid?(user)).ToString(), user, new EntityUid?(user));
  }

  private void OnGlobalStethoscopeExamineVerb(GetVerbsEvent<ExamineVerb> args)
  {
    EntityUid stethoscope;
    if (!args.CanInteract || !args.CanAccess || this.HasComp<XenoComponent>(args.Target) || !this.HasStethoscope(args.User, out stethoscope))
      return;
    FormattedMessage stethoscopeResults = this.GetStethoscopeResults(args.Target, new EntityUid?(args.User));
    this._examine.AddDetailedExamineVerb(args, (Component) this.Comp<RMCStethoscopeComponent>(stethoscope), stethoscopeResults, this.Loc.GetString("rmc-stethoscope-verb-text"), "/Textures/_RMC14/Objects/Medical/stethoscope.rsi/icon.png", this.Loc.GetString("rmc-stethoscope-verb-message"));
  }

  private bool HasStethoscope(EntityUid user, out EntityUid stethoscope)
  {
    stethoscope = EntityUid.Invalid;
    EntityUid? nullable;
    if (this._hands.TryGetActiveItem((Entity<HandsComponent>) user, out nullable) && this.HasComp<RMCStethoscopeComponent>(nullable.Value))
    {
      stethoscope = nullable.Value;
      return true;
    }
    foreach (string accessorySlot in RMCStethoscopeSystem.AccessorySlots)
    {
      EntityUid? entityUid;
      UniformAccessoryHolderComponent comp;
      if (this._inventorySystem.TryGetSlotEntity(user, accessorySlot, out entityUid) && this.TryComp<UniformAccessoryHolderComponent>(entityUid.Value, out comp))
      {
        string containerId = comp.ContainerId;
        BaseContainer container;
        if (this._containerSystem.TryGetContainer(entityUid.Value, containerId, out container))
        {
          foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
          {
            if (this.HasComp<RMCStethoscopeComponent>(containedEntity))
            {
              stethoscope = containedEntity;
              return true;
            }
          }
        }
      }
    }
    return false;
  }

  private FormattedMessage GetStethoscopeResults(EntityUid target, EntityUid? user = null)
  {
    float? percentHealth = this.GetPercentHealth(target);
    FormattedMessage stethoscopeResults = new FormattedMessage();
    if (user.HasValue && !this._skills.HasSkill((Entity<SkillsComponent>) user.Value, RMCStethoscopeSystem.MedicalSkill, 2))
    {
      stethoscopeResults.AddMarkupOrThrow(this.Loc.GetString("rmc-stethoscope-unskilled"));
      return stethoscopeResults;
    }
    if (!percentHealth.HasValue)
    {
      stethoscopeResults.AddMarkupOrThrow(this.Loc.GetString("rmc-stethoscope-nothing"));
    }
    else
    {
      float? nullable = percentHealth;
      float num1 = 87.5f;
      if ((double) nullable.GetValueOrDefault() >= (double) num1 & nullable.HasValue)
      {
        stethoscopeResults.AddMarkupOrThrow(this.Loc.GetString("rmc-stethoscope-normal", (nameof (target), (object) target)));
      }
      else
      {
        nullable = percentHealth;
        float num2 = 62.5f;
        if ((double) nullable.GetValueOrDefault() >= (double) num2 & nullable.HasValue)
        {
          stethoscopeResults.AddMarkupOrThrow(this.Loc.GetString("rmc-stethoscope-raggedy", (nameof (target), (object) target)));
        }
        else
        {
          nullable = percentHealth;
          float num3 = 37.5f;
          if ((double) nullable.GetValueOrDefault() >= (double) num3 & nullable.HasValue)
          {
            stethoscopeResults.AddMarkupOrThrow(this.Loc.GetString("rmc-stethoscope-hyper"));
          }
          else
          {
            nullable = percentHealth;
            float num4 = 1f;
            if ((double) nullable.GetValueOrDefault() >= (double) num4 & nullable.HasValue)
            {
              stethoscopeResults.AddMarkupOrThrow(this.Loc.GetString("rmc-stethoscope-irregular", (nameof (target), (object) target)));
            }
            else
            {
              nullable = percentHealth;
              float num5 = 0.0f;
              if ((double) nullable.GetValueOrDefault() >= (double) num5 & nullable.HasValue)
                stethoscopeResults.AddMarkupOrThrow(this.Loc.GetString("rmc-stethoscope-dead"));
            }
          }
        }
      }
    }
    return stethoscopeResults;
  }

  private float? GetPercentHealth(EntityUid target)
  {
    DamageableComponent comp1;
    MobThresholdsComponent comp2;
    if (!this.TryComp<DamageableComponent>(target, out comp1) || !this.TryComp<MobThresholdsComponent>(target, out comp2))
      return new float?();
    float num = 100f - MathF.Min((float) ((double) comp1.Damage.GetTotal().Float() / (comp2.Thresholds.Count > 0 ? (double) (float) comp2.Thresholds.Keys.Max<FixedPoint2>() : 100.0) * 100.0), 100f);
    if ((double) num > 100.0)
      num = 100f;
    return new float?(num);
  }
}
