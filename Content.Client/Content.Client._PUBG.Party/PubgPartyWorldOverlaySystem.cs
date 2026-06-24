using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Interactable.Components;
using Content.Shared._PUBG.Party;
using Content.Shared.CCVar;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client._PUBG.Party;

public sealed class PubgPartyWorldOverlaySystem : EntitySystem
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private PubgPartyClientSystem _party;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private IConfigurationManager _cfg;

	private readonly Dictionary<EntityUid, ShaderInstance> _partyShaders = new Dictionary<EntityUid, ShaderInstance>();

	private readonly Dictionary<EntityUid, ShaderInstance?> _previousShaders = new Dictionary<EntityUid, ShaderInstance>();

	private readonly HashSet<EntityUid> _keep = new HashSet<EntityUid>();

	private readonly List<EntityUid> _removeScratch = new List<EntityUid>();

	private PubgPartySpriteOverlay? _overlay;

	private bool _markersEnabled = true;

	private float _markersOpacity = 1f;

	private static ShaderInstance? GetValidShader(ShaderInstance? shader)
	{
		if (shader == null || shader.Disposed)
		{
			return null;
		}
		return shader;
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_overlay = new PubgPartySpriteOverlay((IEntityManager)(object)base.EntityManager, _player, _party);
		_party.PartyStateUpdated += OnPartyStateUpdated;
		_cfg.OnValueChanged<bool>(CCVars.PubgPartyMarkersEnabled, (Action<bool>)OnMarkersEnabledChanged, true);
		_cfg.OnValueChanged<float>(CCVars.PubgPartyMarkersOpacity, (Action<float>)OnMarkersOpacityChanged, true);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_party.PartyStateUpdated -= OnPartyStateUpdated;
		ClearOutlines();
		RemoveOverlay();
	}

	private void OnPartyStateUpdated()
	{
		if (!_markersEnabled)
		{
			ClearOutlines();
			RemoveOverlay();
		}
		else
		{
			ApplyOutlines();
			UpdateOverlay();
		}
	}

	private void OnMarkersEnabledChanged(bool enabled)
	{
		_markersEnabled = enabled;
		if (!enabled)
		{
			ClearOutlines();
			RemoveOverlay();
		}
		else
		{
			ApplyOutlines();
			UpdateOverlay();
		}
	}

	private void OnMarkersOpacityChanged(float opacity)
	{
		_markersOpacity = Math.Clamp(opacity, 0f, 1f);
		if (_overlay != null)
		{
			_overlay.Opacity = _markersOpacity;
		}
		ApplyOutlines();
	}

	private void UpdateOverlay()
	{
		if (_overlay != null)
		{
			_overlay.Opacity = _markersOpacity;
			if (_party.Members.Count <= 1)
			{
				RemoveOverlay();
			}
			else if (!_overlayManager.HasOverlay<PubgPartySpriteOverlay>())
			{
				_overlayManager.AddOverlay((Overlay)(object)_overlay);
			}
		}
	}

	private void RemoveOverlay()
	{
		if (_overlayManager.HasOverlay<PubgPartySpriteOverlay>())
		{
			_overlayManager.RemoveOverlay<PubgPartySpriteOverlay>();
		}
	}

	private void ApplyOutlines()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (!_markersEnabled)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		NetEntity val = (NetEntity)(localEntity.HasValue ? ((EntitySystem)this).GetNetEntity(localEntity.Value, (MetaDataComponent)null) : default(NetEntity));
		HashSet<EntityUid> keep = _keep;
		keep.Clear();
		SpriteComponent val2 = default(SpriteComponent);
		foreach (PubgPartyMemberState member in _party.Members)
		{
			if (member.Entity == val)
			{
				continue;
			}
			EntityUid entity = ((EntitySystem)this).GetEntity(member.Entity);
			if (!((EntitySystem)this).Exists(entity) || !((EntitySystem)this).TryComp<SpriteComponent>(entity, ref val2))
			{
				continue;
			}
			keep.Add(entity);
			((EntitySystem)this).EnsureComp<InteractionOutlineComponent>(entity);
			ShaderInstance value = null;
			bool flag = !_partyShaders.TryGetValue(entity, out value) || val2.PostShader != value || (value != null && value.Disposed);
			Color partyColor = GetPartyColor(member.SlotIndex);
			Color val3 = ((Color)(ref partyColor)).WithAlpha(_markersOpacity);
			if (!flag && value != null)
			{
				try
				{
					value.SetParameter("outline_color", val3);
				}
				catch
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (value != null)
				{
					try
					{
						value.Dispose();
					}
					catch
					{
					}
				}
				ShaderInstance validShader = GetValidShader(val2.PostShader);
				if (validShader != null)
				{
					_previousShaders.TryAdd(entity, validShader);
				}
				else
				{
					_previousShaders.Remove(entity);
				}
				value = _prototypes.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCAuraOutline")).InstanceUnique();
				_partyShaders[entity] = value;
				val2.PostShader = value;
				value.SetParameter("outline_color", val3);
				value.SetParameter("outline_width", 2f);
			}
			else if (value != null)
			{
				value.SetParameter("outline_width", 2f);
			}
		}
		ClearMissing(keep);
	}

	private void ClearMissing(HashSet<EntityUid> keep)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		_removeScratch.Clear();
		foreach (EntityUid key in _partyShaders.Keys)
		{
			if (!keep.Contains(key))
			{
				_removeScratch.Add(key);
			}
		}
		foreach (EntityUid item in _removeScratch)
		{
			RestoreOutline(item);
		}
	}

	private void ClearOutlines()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid item in _partyShaders.Keys.ToList())
		{
			RestoreOutline(item);
		}
	}

	private void RestoreOutline(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (_partyShaders.TryGetValue(uid, out ShaderInstance value))
		{
			SpriteComponent val = default(SpriteComponent);
			if (((EntitySystem)this).Exists(uid) && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val) && val.PostShader == value)
			{
				if (_previousShaders.TryGetValue(uid, out ShaderInstance value2) && GetValidShader(value2) != null)
				{
					val.PostShader = value2;
				}
				else
				{
					val.PostShader = null;
				}
			}
			try
			{
				value.Dispose();
			}
			catch
			{
			}
		}
		_partyShaders.Remove(uid);
		_previousShaders.Remove(uid);
	}

	private static Color GetPartyColor(int slotIndex)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(slotIndex switch
		{
			1 => Color.FromHex((ReadOnlySpan<char>)"#00bcd4", (Color?)null), 
			2 => Color.FromHex((ReadOnlySpan<char>)"#ffeb3b", (Color?)null), 
			3 => Color.FromHex((ReadOnlySpan<char>)"#ff9800", (Color?)null), 
			_ => Color.FromHex((ReadOnlySpan<char>)"#4caf50", (Color?)null), 
		});
	}
}
