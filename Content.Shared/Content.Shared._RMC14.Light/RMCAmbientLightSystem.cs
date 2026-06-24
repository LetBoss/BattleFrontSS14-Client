using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Dataset;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Light;

public sealed class RMCAmbientLightSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IPrototypeManager _prototype;

	public Color GetColor(Entity<RMCAmbientLightComponent> ent, TimeSpan curTime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Colors.Count == 0 || ent.Comp.Duration <= TimeSpan.Zero)
		{
			return Color.Black;
		}
		if (ent.Comp.Colors.Count == 1)
		{
			return ent.Comp.Colors[0];
		}
		double num = Math.Clamp((curTime - ent.Comp.StartTime) / ent.Comp.Duration, 0.0, 1.0);
		int segmentCount = ent.Comp.Colors.Count - 1;
		double segmentLength = 1.0 / (double)segmentCount;
		int prevColorIndex = Math.Min((int)(num / segmentLength), segmentCount - 1);
		int nextColorIndex = prevColorIndex + 1;
		double segmentProgress = Math.Clamp((num - (double)prevColorIndex * segmentLength) / segmentLength, 0.0, 1.0);
		Color val = ent.Comp.Colors[prevColorIndex];
		Color nextColor = ent.Comp.Colors[nextColorIndex];
		return Color.InterpolateBetween(val, nextColor, (float)segmentProgress);
	}

	public List<Color> ProcessPrototype(ProtoId<DatasetPrototype> protoId)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<Color> colorList = new List<Color>();
		DatasetPrototype colorDataset = default(DatasetPrototype);
		if (!_prototype.TryIndex<DatasetPrototype>(protoId, ref colorDataset))
		{
			return colorList;
		}
		return colorDataset.Values.Select((string hex) => Color.FromHex((ReadOnlySpan<char>)hex, (Color?)Color.Black)).ToList();
	}

	public void SetColor(Entity<RMCAmbientLightComponent> ent, Color colorHex, TimeSpan duration)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			MapLightComponent mapLight = ((EntitySystem)this).EnsureComp<MapLightComponent>(Entity<RMCAmbientLightComponent>.op_Implicit(ent));
			ent.Comp.Colors.Clear();
			ent.Comp.Colors.AddRange(new _003C_003Ez__ReadOnlyArray<Color>((Color[])(object)new Color[2] { mapLight.AmbientLightColor, colorHex }));
			ent.Comp.Duration = duration;
			ent.Comp.StartTime = _timing.CurTime;
			ent.Comp.IsAnimating = true;
			((EntitySystem)this).Dirty<RMCAmbientLightComponent>(ent, (MetaDataComponent)null);
		}
	}

	public void SetColor(Entity<RMCAmbientLightComponent> ent, List<Color> colorList, TimeSpan duration)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && colorList.Count != 0 && !(duration <= TimeSpan.Zero))
		{
			((EntitySystem)this).EnsureComp<MapLightComponent>(Entity<RMCAmbientLightComponent>.op_Implicit(ent));
			ent.Comp.Colors.Clear();
			ent.Comp.Colors.AddRange(colorList);
			ent.Comp.Duration = duration;
			ent.Comp.StartTime = _timing.CurTime;
			ent.Comp.IsAnimating = true;
			((EntitySystem)this).Dirty<RMCAmbientLightComponent>(ent, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityQueryEnumerator<RMCAmbientLightComponent, MapLightComponent> lightQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCAmbientLightComponent, MapLightComponent>();
		TimeSpan curTime = _timing.CurTime;
		EntityUid uid = default(EntityUid);
		RMCAmbientLightComponent animComponent = default(RMCAmbientLightComponent);
		MapLightComponent lightComponent = default(MapLightComponent);
		while (lightQuery.MoveNext(ref uid, ref animComponent, ref lightComponent))
		{
			if (animComponent.IsAnimating)
			{
				if (curTime >= animComponent.EndTime)
				{
					MapLightComponent obj = lightComponent;
					List<Color> colors = animComponent.Colors;
					obj.AmbientLightColor = colors[colors.Count - 1];
					animComponent.IsAnimating = false;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)animComponent, (MetaDataComponent)null);
					((EntitySystem)this).Dirty(uid, (IComponent)(object)lightComponent, (MetaDataComponent)null);
				}
				else
				{
					Color targetColor = GetColor(Entity<RMCAmbientLightComponent>.op_Implicit((uid, animComponent)), curTime);
					lightComponent.AmbientLightColor = targetColor;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)lightComponent, (MetaDataComponent)null);
				}
			}
		}
	}
}
