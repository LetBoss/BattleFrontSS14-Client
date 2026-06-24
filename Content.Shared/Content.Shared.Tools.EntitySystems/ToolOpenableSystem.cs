using System;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Tools.EntitySystems;

public sealed class ToolOpenableSystem : EntitySystem
{
	[Dependency]
	private SharedToolSystem _tool;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ToolOpenableComponent, ComponentInit>((EntityEventRefHandler<ToolOpenableComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToolOpenableComponent, ToolOpenableDoAfterEventToggleOpen>((EntityEventRefHandler<ToolOpenableComponent, ToolOpenableDoAfterEventToggleOpen>)OnOpenableStateToggled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToolOpenableComponent, InteractUsingEvent>((EntityEventRefHandler<ToolOpenableComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToolOpenableComponent, ExaminedEvent>((EntityEventRefHandler<ToolOpenableComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToolOpenableComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<ToolOpenableComponent, GetVerbsEvent<InteractionVerb>>)OnGetVerb, (Type[])null, (Type[])null);
	}

	private void OnInit(Entity<ToolOpenableComponent> entity, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(entity);
		((EntitySystem)this).Dirty<ToolOpenableComponent>(entity, (MetaDataComponent)null);
	}

	private void OnInteractUsing(Entity<ToolOpenableComponent> entity, ref InteractUsingEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !entity.Comp.VerbOnly && TryOpenClose(entity, args.Used, args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private bool TryOpenClose(Entity<ToolOpenableComponent> entity, EntityUid? toolToToggle, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		ProtoId<ToolQualityPrototype>? neededToolQuantity = (entity.Comp.IsOpen ? entity.Comp.CloseToolQualityNeeded : entity.Comp.OpenToolQualityNeeded);
		float time = (entity.Comp.IsOpen ? entity.Comp.CloseTime : entity.Comp.OpenTime);
		ToolOpenableDoAfterEventToggleOpen evt = new ToolOpenableDoAfterEventToggleOpen();
		if (!toolToToggle.HasValue || !neededToolQuantity.HasValue)
		{
			return false;
		}
		SharedToolSystem tool = _tool;
		EntityUid value = toolToToggle.Value;
		EntityUid? target = Entity<ToolOpenableComponent>.op_Implicit(entity);
		ProtoId<ToolQualityPrototype>? val = neededToolQuantity;
		return tool.UseTool(value, user, target, time, val.HasValue ? ProtoId<ToolQualityPrototype>.op_Implicit(val.GetValueOrDefault()) : null, evt);
	}

	private void OnOpenableStateToggled(Entity<ToolOpenableComponent> entity, ref ToolOpenableDoAfterEventToggleOpen args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			ToggleState(entity);
		}
	}

	private void ToggleState(Entity<ToolOpenableComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		entity.Comp.IsOpen = !entity.Comp.IsOpen;
		UpdateAppearance(entity);
		((EntitySystem)this).Dirty<ToolOpenableComponent>(entity, (MetaDataComponent)null);
	}

	private string GetName(Entity<ToolOpenableComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.Name == null)
		{
			return Identity.Name(Entity<ToolOpenableComponent>.op_Implicit(entity), (IEntityManager)(object)base.EntityManager);
		}
		return base.Loc.GetString(entity.Comp.Name);
	}

	public bool IsOpen(EntityUid uid, ToolOpenableComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ToolOpenableComponent>(uid, ref component, false))
		{
			return true;
		}
		return component.IsOpen;
	}

	private void UpdateAppearance(Entity<ToolOpenableComponent> entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<ToolOpenableComponent>.op_Implicit(entity), (Enum)ToolOpenableVisuals.ToolOpenableVisualState, (object)((!entity.Comp.IsOpen) ? ToolOpenableVisualState.Closed : ToolOpenableVisualState.Open), (AppearanceComponent)null);
	}

	private void OnExamine(Entity<ToolOpenableComponent> entity, ref ExaminedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (args.IsInDetailsRange)
		{
			string name = GetName(entity);
			string msg = ((!entity.Comp.IsOpen) ? base.Loc.GetString("tool-openable-component-examine-closed", (ValueTuple<string, object>)("name", name)) : base.Loc.GetString("tool-openable-component-examine-opened", (ValueTuple<string, object>)("name", name)));
			args.PushMarkup(msg);
		}
	}

	private void OnGetVerb(Entity<ToolOpenableComponent> entity, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanInteract || !args.CanAccess || !entity.Comp.HasVerbs)
		{
			return;
		}
		EntityUid user = args.User;
		EntityUid? item = args.Using;
		string name = GetName(entity);
		InteractionVerb toggleVerb = new InteractionVerb
		{
			IconEntity = ((EntitySystem)this).GetNetEntity(item, (MetaDataComponent)null)
		};
		ProtoId<ToolQualityPrototype>? neededQual;
		if (entity.Comp.IsOpen)
		{
			toggleVerb.Text = (toggleVerb.Message = base.Loc.GetString("tool-openable-component-verb-close"));
			neededQual = entity.Comp.CloseToolQualityNeeded;
			if (neededQual.HasValue)
			{
				if (item.HasValue)
				{
					SharedToolSystem tool = _tool;
					EntityUid value = item.Value;
					ProtoId<ToolQualityPrototype>? val = neededQual;
					if (tool.HasQuality(value, val.HasValue ? ProtoId<ToolQualityPrototype>.op_Implicit(val.GetValueOrDefault()) : null))
					{
						goto IL_0132;
					}
				}
				toggleVerb.Disabled = true;
				toggleVerb.Message = base.Loc.GetString("tool-openable-component-verb-cant-close", (ValueTuple<string, object>)("name", name));
			}
			goto IL_0132;
		}
		toggleVerb.Text = (toggleVerb.Message = base.Loc.GetString("tool-openable-component-verb-open"));
		ProtoId<ToolQualityPrototype>? neededQual2 = entity.Comp.OpenToolQualityNeeded;
		if (!neededQual2.HasValue)
		{
			toggleVerb.Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				ToggleState(entity);
			};
			args.Verbs.Add(toggleVerb);
		}
		else
		{
			if (!item.HasValue)
			{
				return;
			}
			SharedToolSystem tool2 = _tool;
			EntityUid value2 = item.Value;
			ProtoId<ToolQualityPrototype>? val = neededQual2;
			if (tool2.HasQuality(value2, val.HasValue ? ProtoId<ToolQualityPrototype>.op_Implicit(val.GetValueOrDefault()) : null))
			{
				toggleVerb.Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					TryOpenClose(entity, item, user);
				};
				args.Verbs.Add(toggleVerb);
			}
		}
		return;
		IL_0132:
		if (!neededQual.HasValue)
		{
			toggleVerb.Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				ToggleState(entity);
			};
		}
		else
		{
			toggleVerb.Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				TryOpenClose(entity, item, user);
			};
		}
		args.Verbs.Add(toggleVerb);
	}
}
