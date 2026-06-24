using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Atmos.Components;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.SubFloor;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedAtmosPipeLayersSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private IPrototypeManager _protoManager;

	[Dependency]
	private SharedToolSystem _tool;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AtmosPipeLayersComponent, ExaminedEvent>((EntityEventRefHandler<AtmosPipeLayersComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AtmosPipeLayersComponent, GetVerbsEvent<Verb>>((EntityEventRefHandler<AtmosPipeLayersComponent, GetVerbsEvent<Verb>>)OnGetVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AtmosPipeLayersComponent, InteractUsingEvent>((EntityEventRefHandler<AtmosPipeLayersComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AtmosPipeLayersComponent, UseInHandEvent>((EntityEventRefHandler<AtmosPipeLayersComponent, UseInHandEvent>)OnUseInHandEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AtmosPipeLayersComponent, TrySetNextPipeLayerCompletedEvent>((EntityEventRefHandler<AtmosPipeLayersComponent, TrySetNextPipeLayerCompletedEvent>)OnSetNextPipeLayerCompleted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AtmosPipeLayersComponent, TrySettingPipeLayerCompletedEvent>((EntityEventRefHandler<AtmosPipeLayersComponent, TrySettingPipeLayerCompletedEvent>)OnSettingPipeLayerCompleted, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<AtmosPipeLayersComponent> ent, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		string layerName = GetPipeLayerName(ent.Comp.CurrentPipeLayer);
		args.PushMarkup(base.Loc.GetString("atmos-pipe-layers-component-current-layer", (ValueTuple<string, object>)("layerName", layerName)));
	}

	private void OnGetVerb(Entity<AtmosPipeLayersComponent> ent, ref GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		ToolQualityPrototype toolProto = default(ToolQualityPrototype);
		if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract || ent.Comp.NumberOfPipeLayers <= 1 || ent.Comp.PipeLayersLocked || !_protoManager.TryIndex<ToolQualityPrototype>(ent.Comp.Tool, ref toolProto))
		{
			return;
		}
		EntityUid user = args.User;
		SubFloorHideComponent subFloorHide = default(SubFloorHideComponent);
		if (((EntitySystem)this).TryComp<SubFloorHideComponent>(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), ref subFloorHide) && subFloorHide.IsUnderCover)
		{
			Verb v = new Verb
			{
				Priority = 1,
				Category = VerbCategory.Adjust,
				Text = base.Loc.GetString("atmos-pipe-layers-component-pipes-are-covered"),
				Disabled = true,
				Impact = LogImpact.Low,
				DoContactInteraction = true
			};
			args.Verbs.Add(v);
			return;
		}
		if (!TryGetHeldTool(user, ent.Comp.Tool, out var tool))
		{
			Verb v2 = new Verb
			{
				Priority = 1,
				Category = VerbCategory.Adjust,
				Text = base.Loc.GetString("atmos-pipe-layers-component-tool-missing", (ValueTuple<string, object>)("toolName", base.Loc.GetString(toolProto.ToolName).ToLower())),
				Disabled = true,
				Impact = LogImpact.Low,
				DoContactInteraction = true
			};
			args.Verbs.Add(v2);
			return;
		}
		for (int i = 0; i < ent.Comp.NumberOfPipeLayers; i++)
		{
			int index = i;
			string layerName = GetPipeLayerName((AtmosPipeLayer)index);
			string label = base.Loc.GetString("atmos-pipe-layers-component-select-layer", (ValueTuple<string, object>)("layerName", layerName));
			Verb v3 = new Verb
			{
				Priority = 1,
				Category = VerbCategory.Adjust,
				Text = label,
				Disabled = (index == (int)ent.Comp.CurrentPipeLayer),
				Impact = LogImpact.Low,
				DoContactInteraction = true,
				Act = delegate
				{
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0065: Unknown result type (might be due to invalid IL or missing references)
					_tool.UseTool(Entity<ToolComponent>.op_Implicit(tool.Value), user, Entity<AtmosPipeLayersComponent>.op_Implicit(ent), ent.Comp.Delay, (IEnumerable<string>)tool.Value.Comp.Qualities, new TrySettingPipeLayerCompletedEvent((AtmosPipeLayer)index));
				}
			};
			args.Verbs.Add(v3);
		}
	}

	private void OnInteractUsing(Entity<AtmosPipeLayersComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		ToolComponent tool = default(ToolComponent);
		if (ent.Comp.NumberOfPipeLayers > 1 && !ent.Comp.PipeLayersLocked && ((EntitySystem)this).TryComp<ToolComponent>(args.Used, ref tool) && _tool.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.Tool), tool))
		{
			SubFloorHideComponent subFloorHide = default(SubFloorHideComponent);
			if (((EntitySystem)this).TryComp<SubFloorHideComponent>(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), ref subFloorHide) && subFloorHide.IsUnderCover)
			{
				_popup.PopupClient(base.Loc.GetString("atmos-pipe-layers-component-cannot-adjust-pipes"), Entity<AtmosPipeLayersComponent>.op_Implicit(ent), args.User);
			}
			else
			{
				_tool.UseTool(args.Used, args.User, Entity<AtmosPipeLayersComponent>.op_Implicit(ent), ent.Comp.Delay, (IEnumerable<string>)tool.Qualities, new TrySetNextPipeLayerCompletedEvent());
			}
		}
	}

	private void OnUseInHandEvent(Entity<AtmosPipeLayersComponent> ent, ref UseInHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.NumberOfPipeLayers <= 1 || ent.Comp.PipeLayersLocked)
		{
			return;
		}
		if (!TryGetHeldTool(args.User, ent.Comp.Tool, out Entity<ToolComponent>? tool))
		{
			ToolQualityPrototype toolProto = default(ToolQualityPrototype);
			if (_protoManager.TryIndex<ToolQualityPrototype>(ent.Comp.Tool, ref toolProto))
			{
				string toolName = base.Loc.GetString(toolProto.ToolName).ToLower();
				string message = base.Loc.GetString("atmos-pipe-layers-component-tool-missing", (ValueTuple<string, object>)("toolName", toolName));
				_popup.PopupClient(message, Entity<AtmosPipeLayersComponent>.op_Implicit(ent), args.User);
			}
		}
		else
		{
			_tool.UseTool(Entity<ToolComponent>.op_Implicit(tool.Value), args.User, Entity<AtmosPipeLayersComponent>.op_Implicit(ent), ent.Comp.Delay, (IEnumerable<string>)tool.Value.Comp.Qualities, new TrySetNextPipeLayerCompletedEvent());
		}
	}

	private void OnSetNextPipeLayerCompleted(Entity<AtmosPipeLayersComponent> ent, ref TrySetNextPipeLayerCompletedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			SetNextPipeLayer(ent, args.User, args.Used);
		}
	}

	private void OnSettingPipeLayerCompleted(Entity<AtmosPipeLayersComponent> ent, ref TrySettingPipeLayerCompletedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			SetPipeLayer(ent, args.PipeLayer, args.User, args.Used);
		}
	}

	public void SetNextPipeLayer(Entity<AtmosPipeLayersComponent> ent, EntityUid? user = null, EntityUid? used = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		int newLayer = (int)(ent.Comp.CurrentPipeLayer + 1) % (int)ent.Comp.NumberOfPipeLayers;
		SetPipeLayer(ent, (AtmosPipeLayer)newLayer, user, used);
	}

	public virtual void SetPipeLayer(Entity<AtmosPipeLayersComponent> ent, AtmosPipeLayer layer, EntityUid? user = null, EntityUid? used = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.PipeLayersLocked)
		{
			return;
		}
		ent.Comp.CurrentPipeLayer = (AtmosPipeLayer)Math.Clamp((int)layer, 0, ent.Comp.NumberOfPipeLayers - 1);
		((EntitySystem)this).Dirty<AtmosPipeLayersComponent>(ent, (MetaDataComponent)null);
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), ref appearance))
		{
			if (ent.Comp.SpriteRsiPaths.TryGetValue(ent.Comp.CurrentPipeLayer, out string path))
			{
				_appearance.SetData(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), (Enum)AtmosPipeLayerVisuals.Sprite, (object)path, appearance);
			}
			if (ent.Comp.SpriteLayersRsiPaths.Count > 0)
			{
				Dictionary<string, string> data = new Dictionary<string, string>();
				foreach (var (layerKey, dictionary2) in ent.Comp.SpriteLayersRsiPaths)
				{
					if (dictionary2.TryGetValue(ent.Comp.CurrentPipeLayer, out path))
					{
						data.TryAdd(layerKey, path);
					}
				}
				_appearance.SetData(Entity<AtmosPipeLayersComponent>.op_Implicit(ent), (Enum)AtmosPipeLayerVisuals.SpriteLayers, (object)data, appearance);
			}
		}
		if (user.HasValue)
		{
			string layerName = GetPipeLayerName(ent.Comp.CurrentPipeLayer);
			string message = base.Loc.GetString("atmos-pipe-layers-component-change-layer", (ValueTuple<string, object>)("layerName", layerName));
			_popup.PopupClient(message, Entity<AtmosPipeLayersComponent>.op_Implicit(ent), user);
		}
	}

	public bool TryGetAlternativePrototype(AtmosPipeLayersComponent component, AtmosPipeLayer layer, out EntProtoId proto)
	{
		return component.AlternativePrototypes.TryGetValue(layer, out proto);
	}

	private bool TryGetHeldTool(EntityUid user, ProtoId<ToolQualityPrototype> toolQuality, [NotNullWhen(true)] out Entity<ToolComponent>? heldTool)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		heldTool = null;
		ToolComponent tool = default(ToolComponent);
		foreach (EntityUid heldItem in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(user)))
		{
			if (((EntitySystem)this).TryComp<ToolComponent>(heldItem, ref tool) && _tool.HasQuality(heldItem, ProtoId<ToolQualityPrototype>.op_Implicit(toolQuality), tool))
			{
				heldTool = new Entity<ToolComponent>(heldItem, tool);
				return true;
			}
		}
		return false;
	}

	private string GetPipeLayerName(AtmosPipeLayer layer)
	{
		return base.Loc.GetString("atmos-pipe-layers-component-layer-" + layer.ToString().ToLower());
	}
}
