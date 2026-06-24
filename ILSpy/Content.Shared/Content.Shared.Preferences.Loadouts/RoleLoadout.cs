using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared.CCVar;
using Content.Shared.Preferences.Loadouts.Effects;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Preferences.Loadouts;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class RoleLoadout : IEquatable<RoleLoadout>, ISerializationGenerated<RoleLoadout>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<RoleLoadoutPrototype> Role;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>> SelectedLoadouts = new Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>>();

	public string? EntityName;

	public int? Points;

	public RoleLoadout(ProtoId<RoleLoadoutPrototype> role)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Role = role;
	}

	public RoleLoadout Clone()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		RoleLoadout weh = new RoleLoadout(Role);
		foreach (KeyValuePair<ProtoId<LoadoutGroupPrototype>, List<Loadout>> selected in SelectedLoadouts)
		{
			weh.SelectedLoadouts.Add(selected.Key, new List<Loadout>(selected.Value));
		}
		weh.EntityName = EntityName;
		weh.Points = Points;
		return weh;
	}

	public void EnsureValid(HumanoidCharacterProfile profile, ICommonSession session, IDependencyCollection collection)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		ValueList<string> groupRemove = default(ValueList<string>);
		IPrototypeManager protoManager = collection.Resolve<IPrototypeManager>();
		IConfigurationManager configManager = collection.Resolve<IConfigurationManager>();
		RoleLoadoutPrototype roleProto = default(RoleLoadoutPrototype);
		if (!protoManager.TryIndex<RoleLoadoutPrototype>(Role, ref roleProto))
		{
			EntityName = null;
			SelectedLoadouts.Clear();
			return;
		}
		if (!roleProto.CanCustomizeName)
		{
			EntityName = null;
		}
		if (EntityName != null)
		{
			string name = EntityName.Trim();
			int maxNameLength = configManager.GetCVar<int>(CCVars.MaxNameLength);
			if (name.Length > maxNameLength)
			{
				EntityName = name.Substring(0, maxNameLength);
			}
			if (name.Length == 0)
			{
				EntityName = null;
			}
		}
		foreach (ProtoId<LoadoutGroupPrototype> groupProto in roleProto.Groups)
		{
			if (!SelectedLoadouts.ContainsKey(groupProto))
			{
				SelectedLoadouts[groupProto] = new List<Loadout>();
			}
		}
		Points = roleProto.Points;
		LoadoutGroupPrototype groupProto2 = default(LoadoutGroupPrototype);
		LoadoutPrototype loadoutProto = default(LoadoutPrototype);
		LoadoutPrototype loadoutProto2 = default(LoadoutPrototype);
		foreach (var (group, groupLoadouts) in SelectedLoadouts)
		{
			if (!roleProto.Groups.Contains(group))
			{
				groupRemove.Add(ProtoId<LoadoutGroupPrototype>.op_Implicit(group));
				continue;
			}
			if (!protoManager.TryIndex<LoadoutGroupPrototype>(group, ref groupProto2))
			{
				groupRemove.Add(ProtoId<LoadoutGroupPrototype>.op_Implicit(group));
				continue;
			}
			List<Loadout> loadouts = groupLoadouts.Slice(0, Math.Min(groupLoadouts.Count, groupProto2.MaxLimit));
			FormattedMessage reason;
			for (int i = loadouts.Count - 1; i >= 0; i--)
			{
				Loadout loadout = loadouts[i];
				if (!protoManager.TryIndex<LoadoutPrototype>(loadout.Prototype, ref loadoutProto))
				{
					loadouts.RemoveAt(i);
				}
				else if (!groupProto2.Loadouts.Contains(loadout.Prototype))
				{
					loadouts.RemoveAt(i);
				}
				else if (!IsValid(profile, session, loadout.Prototype, collection, out reason))
				{
					loadouts.RemoveAt(i);
				}
				else
				{
					Apply(loadoutProto);
				}
			}
			if (loadouts.Count < groupProto2.MinLimit)
			{
				foreach (ProtoId<LoadoutPrototype> protoId in groupProto2.Loadouts)
				{
					if (loadouts.Count >= groupProto2.MinLimit)
					{
						break;
					}
					if (protoManager.TryIndex<LoadoutPrototype>(protoId, ref loadoutProto2))
					{
						Loadout defaultLoadout = new Loadout
						{
							Prototype = ProtoId<LoadoutPrototype>.op_Implicit(loadoutProto2.ID)
						};
						if (!loadouts.Contains(defaultLoadout) && IsValid(profile, session, defaultLoadout.Prototype, collection, out reason))
						{
							loadouts.Add(defaultLoadout);
							Apply(loadoutProto2);
						}
					}
				}
			}
			SelectedLoadouts[group] = loadouts;
		}
		Enumerator<string> enumerator4 = groupRemove.GetEnumerator();
		try
		{
			while (enumerator4.MoveNext())
			{
				string value = enumerator4.Current;
				SelectedLoadouts.Remove(ProtoId<LoadoutGroupPrototype>.op_Implicit(value));
			}
		}
		finally
		{
			((IDisposable)enumerator4/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private void Apply(LoadoutPrototype loadoutProto)
	{
		foreach (LoadoutEffect effect in loadoutProto.Effects)
		{
			effect.Apply(this);
		}
		if (loadoutProto.Cost.HasValue && Points.HasValue)
		{
			Points -= loadoutProto.Cost;
		}
	}

	public void SetDefault(HumanoidCharacterProfile? profile, ICommonSession? session, IPrototypeManager protoManager, bool force = false)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (profile == null)
		{
			return;
		}
		if (force)
		{
			SelectedLoadouts.Clear();
		}
		IDependencyCollection collection = IoCManager.Instance;
		RoleLoadoutPrototype roleProto = protoManager.Index<RoleLoadoutPrototype>(Role);
		LoadoutGroupPrototype groupProto = default(LoadoutGroupPrototype);
		LoadoutPrototype loadoutProto = default(LoadoutPrototype);
		for (int i = roleProto.Groups.Count - 1; i >= 0; i--)
		{
			ProtoId<LoadoutGroupPrototype> group = roleProto.Groups[i];
			if (protoManager.TryIndex<LoadoutGroupPrototype>(group, ref groupProto) && !SelectedLoadouts.ContainsKey(group))
			{
				List<Loadout> loadouts = new List<Loadout>();
				SelectedLoadouts[group] = loadouts;
				Points = roleProto.Points;
				if (groupProto.MinLimit > 0)
				{
					foreach (ProtoId<LoadoutPrototype> protoId in groupProto.Loadouts)
					{
						if (loadouts.Count >= groupProto.MinLimit)
						{
							break;
						}
						if (protoManager.TryIndex<LoadoutPrototype>(protoId, ref loadoutProto))
						{
							Loadout defaultLoadout = new Loadout
							{
								Prototype = ProtoId<LoadoutPrototype>.op_Implicit(loadoutProto.ID)
							};
							if (IsValid(profile, session, defaultLoadout.Prototype, collection, out FormattedMessage _))
							{
								loadouts.Add(defaultLoadout);
								Apply(loadoutProto);
							}
						}
					}
				}
			}
		}
	}

	public bool IsValid(HumanoidCharacterProfile profile, ICommonSession? session, ProtoId<LoadoutPrototype> loadout, IDependencyCollection collection, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		reason = null;
		IPrototypeManager protoManager = collection.Resolve<IPrototypeManager>();
		LoadoutPrototype loadoutProto = default(LoadoutPrototype);
		if (!protoManager.TryIndex<LoadoutPrototype>(loadout, ref loadoutProto))
		{
			reason = FormattedMessage.FromMarkupOrThrow("");
			return false;
		}
		if (!protoManager.HasIndex<RoleLoadoutPrototype>(Role))
		{
			reason = FormattedMessage.FromUnformatted("loadouts-prototype-missing");
			return false;
		}
		if (loadoutProto.Cost.HasValue && Points.HasValue && Points < loadoutProto.Cost)
		{
			reason = FormattedMessage.FromUnformatted(Loc.GetString("loadout-group-points-insufficient"));
			return false;
		}
		bool valid = true;
		foreach (LoadoutEffect effect in loadoutProto.Effects)
		{
			valid = valid && effect.Validate(profile, this, session, collection, out reason);
		}
		return valid;
	}

	public bool AddLoadout(ProtoId<LoadoutGroupPrototype> selectedGroup, ProtoId<LoadoutPrototype> selectedLoadout, IPrototypeManager protoManager)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		List<Loadout> groupLoadouts = SelectedLoadouts[selectedGroup];
		int limit = Math.Max(0, groupLoadouts.Count + 1 - protoManager.Index<LoadoutGroupPrototype>(selectedGroup).MaxLimit);
		for (int i = 0; i < groupLoadouts.Count; i++)
		{
			if (groupLoadouts[i].Prototype != selectedLoadout)
			{
				if (limit > 0)
				{
					limit--;
					groupLoadouts.RemoveAt(i);
					i--;
				}
				continue;
			}
			return false;
		}
		groupLoadouts.Add(new Loadout
		{
			Prototype = selectedLoadout
		});
		return true;
	}

	public bool RemoveLoadout(ProtoId<LoadoutGroupPrototype> selectedGroup, ProtoId<LoadoutPrototype> selectedLoadout, IPrototypeManager protoManager)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		List<Loadout> groupLoadouts = SelectedLoadouts[selectedGroup];
		for (int i = 0; i < groupLoadouts.Count; i++)
		{
			if (!(groupLoadouts[i].Prototype != selectedLoadout))
			{
				groupLoadouts.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public bool Equals(RoleLoadout? other)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (!Role.Equals(other.Role) || SelectedLoadouts.Count != other.SelectedLoadouts.Count || Points != other.Points || EntityName != other.EntityName)
		{
			return false;
		}
		foreach (var (key, value) in SelectedLoadouts)
		{
			if (!other.SelectedLoadouts.TryGetValue(key, out List<Loadout> otherValue) || !otherValue.SequenceEqual(value))
			{
				return false;
			}
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (this != obj)
		{
			if (obj is RoleLoadout other)
			{
				return Equals(other);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return HashCode.Combine<ProtoId<RoleLoadoutPrototype>, Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>>, int?>(Role, SelectedLoadouts, Points);
	}

	public RoleLoadout()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RoleLoadout target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RoleLoadout>(this, ref target, hookCtx, false, context))
		{
			ProtoId<RoleLoadoutPrototype> RoleTemp = default(ProtoId<RoleLoadoutPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<RoleLoadoutPrototype>>(Role, ref RoleTemp, hookCtx, false, context))
			{
				RoleTemp = serialization.CreateCopy<ProtoId<RoleLoadoutPrototype>>(Role, hookCtx, context, false);
			}
			target.Role = RoleTemp;
			Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>> SelectedLoadoutsTemp = null;
			if (SelectedLoadouts == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>>>(SelectedLoadouts, ref SelectedLoadoutsTemp, hookCtx, true, context))
			{
				SelectedLoadoutsTemp = serialization.CreateCopy<Dictionary<ProtoId<LoadoutGroupPrototype>, List<Loadout>>>(SelectedLoadouts, hookCtx, context, false);
			}
			target.SelectedLoadouts = SelectedLoadoutsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RoleLoadout target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RoleLoadout cast = (RoleLoadout)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RoleLoadout Instantiate()
	{
		return new RoleLoadout();
	}
}
