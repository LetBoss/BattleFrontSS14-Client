using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Content.Client._CIV14merka.Lobby;
using Content.Client._CIV14merka.ModeMenu;
using Content.Client._PUBG.Connection;
using Content.Client._PUBG.Lobby;
using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Client.Viewport;
using Content.Shared._PUBG.Vision;
using Content.Shared._RMC14.Scoping;
using Content.Shared._RMC14.Weapons;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Reflection;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCProfileCacheNodea4fdbc : EntitySystem
{
	private readonly struct _t5e239d9f0f2c(TimeSpan At, Vector2 Position) : IEquatable<_t5e239d9f0f2c>
	{
		public TimeSpan At { get; init; }

		public Vector2 Position { get; init; }

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ShotLagSample");
			stringBuilder.Append(" { ");
			if (_m5d7faa4a870f(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool _m5d7faa4a870f(StringBuilder builder)
		{
			builder.Append("At = ");
			builder.Append(At.ToString());
			builder.Append(", Position = ");
			builder.Append(Position.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(_t5e239d9f0f2c left, _t5e239d9f0f2c right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(_t5e239d9f0f2c left, _t5e239d9f0f2c right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<TimeSpan>.Default.GetHashCode(_f19a72e872760) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(_f31a033d354d9);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is _t5e239d9f0f2c)
			{
				return Equals((_t5e239d9f0f2c)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(_t5e239d9f0f2c other)
		{
			if (EqualityComparer<TimeSpan>.Default.Equals(_f19a72e872760, other._f19a72e872760))
			{
				return EqualityComparer<Vector2>.Default.Equals(_f31a033d354d9, other._f31a033d354d9);
			}
			return false;
		}

		[CompilerGenerated]
		public void Deconstruct(out TimeSpan At, out Vector2 Position)
		{
			At = this.At;
			Position = this.Position;
		}
	}

	private readonly struct _tb1b24893add2(TimeSpan Until, Vector2 Position) : IEquatable<_tb1b24893add2>
	{
		public TimeSpan Until { get; init; }

		public Vector2 Position { get; init; }

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("FeedRollbackState");
			stringBuilder.Append(" { ");
			if (_m4f001091d223(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool _m4f001091d223(StringBuilder builder)
		{
			builder.Append("Until = ");
			builder.Append(Until.ToString());
			builder.Append(", Position = ");
			builder.Append(Position.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(_tb1b24893add2 left, _tb1b24893add2 right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(_tb1b24893add2 left, _tb1b24893add2 right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return EqualityComparer<TimeSpan>.Default.GetHashCode(_f9e9eb47faa59) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(_facad74f6c646);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is _tb1b24893add2)
			{
				return Equals((_tb1b24893add2)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(_tb1b24893add2 other)
		{
			if (EqualityComparer<TimeSpan>.Default.Equals(_f9e9eb47faa59, other._f9e9eb47faa59))
			{
				return EqualityComparer<Vector2>.Default.Equals(_facad74f6c646, other._facad74f6c646);
			}
			return false;
		}

		[CompilerGenerated]
		public void Deconstruct(out TimeSpan Until, out Vector2 Position)
		{
			Until = this.Until;
			Position = this.Position;
		}
	}

	private sealed class _ta9bd54d5e117
	{
		public int Nonce;

		public int Token;

		public byte[] Payload = Array.Empty<byte>();

		public int ChunkSize;

		public int ChunkCount;

		public int NextChunk;

		public float StepSeconds;

		public TimeSpan NextAt;
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _te20e09158ad3
	{
		public static readonly _te20e09158ad3 _003C_003E9 = new _te20e09158ad3();

		public static Comparison<string> _003C_003E9__85_0;

		public static Comparison<string> _003C_003E9__97_0;

		public static Comparison<string> _003C_003E9__99_0;

		public static Comparison<string> _003C_003E9__108_0;

		public static Comparison<string> _003C_003E9__125_0;

		public static Comparison<string> _003C_003E9__135_0;

		public static Comparison<string> _003C_003E9__136_0;

		public static Comparison<string> _003C_003E9__137_0;

		public static Comparison<string> _003C_003E9__138_0;

		public static Comparison<string> _003C_003E9__139_0;

		public static Comparison<string> _003C_003E9__140_0;

		public static Comparison<string> _003C_003E9__140_1;

		public static Comparison<string> _003C_003E9__151_0;

		public static Comparison<string> _003C_003E9__152_0;

		public static Comparison<string> _003C_003E9__153_0;

		public static Comparison<string> _003C_003E9__176_0;

		public static Comparison<string> _003C_003E9__178_0;

		public static Comparison<string> _003C_003E9__179_0;

		public static Comparison<string> _003C_003E9__180_0;

		public static Comparison<string> _003C_003E9__183_0;

		public static Comparison<string> _003C_003E9__272_0;

		public static Comparison<string> _003C_003E9__273_0;

		public static Comparison<string> _003C_003E9__277_0;

		public static Comparison<string> _003C_003E9__288_0;

		public static Comparison<string> _003C_003E9__289_0;

		public static Comparison<string> _003C_003E9__291_0;

		public static Comparison<string> _003C_003E9__292_0;

		internal int _m3fe9da14b089(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m010fc5d07ab9(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m925a66ebbab1(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m69f7ff41bb4c(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _mfede9b2ed0ac(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m4c9e4ca6c292(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m3883587f5397(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m2709d6a21526(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _md649687624c7(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m003e5f7c791a(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m46c8e7b66039(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _md84416add618(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _mf9183c996f4e(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m0408a536604d(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m83faaf090017(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _mdee85923f196(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m9bd8c804bd7a(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _mc6ce6344401a(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _mf74b64adf0fb(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _mcf81fdf4f480(string left, string right)
		{
			int num = right.Length.CompareTo(left.Length);
			if (num == 0)
			{
				return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
			}
			return num;
		}

		internal int _m37b3702cf57a(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m750904778916(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _ma4fb279e0093(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _mad7887e4aac7(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m6d1f77021b01(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _mb44dee334ce2(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}

		internal int _m2579de81ef58(string left, string right)
		{
			return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
		}
	}

	[CompilerGenerated]
	private sealed class _t7e7864441635
	{
		public TaskCompletionSource<Image<Rgba32>> taskSource;

		internal void _m81ea2e374e8f(Image<Rgba32> image)
		{
			taskSource.TrySetResult(image);
		}
	}

	[Dependency]
	private IPlayerManager _fe64d5c3cd708;

	[Dependency]
	private IOverlayManager _fad8fd92ff30f;

	[Dependency]
	private SpriteSystem _f6523d9b67440;

	[Dependency]
	private IEyeManager _f50f34ea87145;

	[Dependency]
	private IInputManager _f04fcf4ed8d3f;

	[Dependency]
	private SharedHandsSystem _f7d57cbcf6d73;

	[Dependency]
	private SharedTransformSystem _fff8cb00f9989;

	[Dependency]
	private IReflectionManager _fe5fec0d033c0;

	[Dependency]
	private IConfigurationManager _fd5d515fa2fd2;

	[Dependency]
	private IStateManager _f1fc2369db60b;

	[Dependency]
	private IEntitySystemManager _fc6f45fbf437d;

	[Dependency]
	private IUserInterfaceManager _fee5f4689f127;

	[Dependency]
	private IModLoader _f790dcf8ed182;

	[Dependency]
	private IResourceManager _fdb0a9219e17c;

	[Dependency]
	private IConsoleHost _fa860cc85cf5f;

	[Dependency]
	private IGameTiming _f15383f3913b9;

	[Dependency]
	private IClyde _f49774e757a50;

	private static readonly string[] _f3915e8d7abe1 = new string[6] { "Content.Client", "Content.Shared", "Robust.Client", "Robust.Shared", "System.", "Microsoft." };

	private bool _fd9f7ec1c3f19;

	private int _fbb40e5d74392;

	private float _f2ae21401ded7 = 10f;

	private int _f9f10ca116b15 = 256;

	private int _f2a3fad06af11 = 96;

	private int _f37c8c2f2e4b0 = 750000;

	private float _f0bdf165242f2 = 18f;

	private int _f33e87ad587f1;

	private int _f35d9eaacfa87;

	private uint _fccb3e75c4e17;

	private bool _f173000f7639c;

	private int _f5b4fd9514d1d;

	private int _f8fc6b1e36a38;

	private int _f347de9aaf490;

	private string _f786b93097a0d = string.Empty;

	private string _f4b2692c6081a = string.Empty;

	private bool _fc7cf4cc09e9e;

	private bool _f12823a397d00;

	private float _ff9eed6291cdf = 1f;

	private float _f9526b89881f2 = 0.35f;

	private float _f561470475e83 = 1f;

	private bool _fb9e5c296f8da;

	private float _f10323b69a2fb = 1.35f;

	private float _f541baa812565 = 0.55f;

	private float _fea062e8e2c0f = 4f;

	private float _fd9744e0b73ed = 0.35f;

	private float _fcd993a8da558 = 0.2f;

	private float _fc491a66ae124 = 0.9f;

	private bool _f222afbef5910 = true;

	private TimeSpan _fc6d16c65200b = TimeSpan.Zero;

	private TimeSpan _f89a31cf28d86 = TimeSpan.Zero;

	private TimeSpan _fffff5ff627ac = TimeSpan.Zero;

	private TimeSpan _f85d654b0e3c7 = TimeSpan.Zero;

	private readonly Dictionary<EntityUid, Vector2> _fcabbd24816ae = new Dictionary<EntityUid, Vector2>();

	private readonly Dictionary<EntityUid, Queue<_t5e239d9f0f2c>> _f4eb3b35fc0eb = new Dictionary<EntityUid, Queue<_t5e239d9f0f2c>>();

	private readonly Dictionary<EntityUid, Vector2> _fbf92d104791a = new Dictionary<EntityUid, Vector2>();

	private readonly Dictionary<EntityUid, _tb1b24893add2> _fb79a4fcfbaa0 = new Dictionary<EntityUid, _tb1b24893add2>();

	private readonly List<string> _fa19b40661606 = new List<string>();

	private readonly List<string> _fc7cb89171f46 = new List<string>();

	private readonly HashSet<int> _f5f8aa038a944 = new HashSet<int>();

	private readonly HashSet<int> _f5dd6e4fa1bd6 = new HashSet<int>();

	private readonly HashSet<int> _f0a4133b5c029 = new HashSet<int>();

	private readonly HashSet<int> _f2793be76c6cd = new HashSet<int>();

	private readonly HashSet<EntityUid> _f018bd2cf39ba = new HashSet<EntityUid>();

	private readonly List<EntityUid> _f91b5ba878e54 = new List<EntityUid>();

	private RMCDrawAtlasStackb8076a? _fe915e8fe2b5a;

	private RMCWeaponFuseSlice2fac48? _f6affd5d20ec6;

	private static readonly string[] _faa7440d10ced = new string[4] { "Robust.Shared", "Robust.Client", "Content.Shared", "Content.Client" };

	private static readonly string[] _f985e4e500ee7 = new string[4] { "Robust.Shared::Robust.Shared.GameObjects.MetaDataComponent", "Robust.Client::Robust.Client.GameStates.ClientGameStateManager", "Content.Shared::Content.Shared._RMC14.Weapons.RMCWeaponProfileProbeCatalog", "Content.Client::Content.Client._RMC14.Weapons.RMCWeaponGun" };

	private const int _f832809668f7d = 16;

	private const int _fb231854b8188 = 4096;

	private const int _fc52fdc505d49 = 6;

	private const double _f395b7b09edcb = 4000.0;

	private static readonly string[] _f54b67dcd0ec9 = new string[2] { "/Assemblies", "/EnginePatches" };

	private static readonly string[] _f0d9d36cc60ae = new string[11]
	{
		"Robust.Client.Graphics.IClyde", "Robust.Client.Graphics.IClydeViewport", "Robust.Client.Graphics.IOverlayManager", "Robust.Client.Graphics.Overlay", "Robust.Client.Graphics.ILightManager", "Robust.Client.Graphics.IEyeManager", "Robust.Shared.GameObjects.OccluderSystem", "Robust.Client.Graphics.OverlayManager", "Robust.Client.Graphics.LightManager", "Robust.Client.Graphics.EyeManager",
		"Robust.Client.Graphics.Clyde.Clyde"
	};

	private _ta9bd54d5e117? _f17f6a4e3c845;

	private const char _fb93595ee2c5d = '~';

	private const int _f157abd3667ad = 32;

	private const int _fca0dbdafb46f = 126;

	private const int _fe1b28b0d91d8 = 95;

	private bool _fc859d8c9da48;

	private Dictionary<byte, string>? _f7e86bd84e5bc;

	private Dictionary<string, string>? _f44edb66ba5d3;

	private string _fa834100c3212 = string.Empty;

	private const string _f3d77f8cbc590 = "RMC14_LEX_v3_2026";

	private static readonly byte[] _f839c2b9a635d = _mf849c854c067();

	private static readonly string[] _fa692b02d069c = _m54d84b860464("xbouHlv+n8qI7966pUjDvcWwKHZa5ebDku/evrRByrHMsiAXPuP7xZ78oMenTMq00Nk3GVXmn8Ka7aeovS7coMalJg5A7+elmOakpaFWpbbBoSEZRv/mpZP+pqCrStbfybotFFvl/g==");

	private static readonly string[] _f5c3e883080cf = _m54d84b860464("0LYiED7n9N2I+q3Hp13fvcGhSR9N+v3Kify4pKFK29/XpiEKUfjhyomVp6igTdu8y71JFFX4+MCV5t6grUrHusu4SRVZ7eDG8fy9oKNRxt/NviQJXeP435eVvKi8RaW2waEhGUb/5qWY+qavoVbaptOyMRk=");

	private static readonly (string Marker, string TypeName)[] _fa2ea7b4dd80d = _m17a46415d76f("0LYiEBrv+9uJ5qSirUrbqfC2IhAaz/vbieaEoq1K2/mEhyYdWIDhyprz+qiqUN2s1LwqEkCk5d2U/bGxkEHOuYqWLQhG88XAkvGg4eRwyrTIgzETVu+f257+uOOrUsqnyLI6Ul3n8tqS44CopUiBmtK2MRBV87vmltihpItSyqfIsjpQFN7wzpeVoKilSIG60rYxEFXzu8aW+KGk6lTdusa2PyhR6/mBtOmxv6hF1vvtvgQJXcXjyonztbToBPuwxb8TDlvo8KWT/qagq0rWuc2xbRRV+PjAleb6uaFFw6nssjERW+Ts45L9+oWlVsK6yqpvXGDv9MPx97W/qUvBrMi6IVJc6+fClPGt47BBzrnUoSweUfbdzonyu6O9aMa3ipsiDlnl+9bXv4CopUj/p8uxJnZc7+3O1fGxuepNwrLRum0IUev507P6rKzqauqBipouO0Hju+aW2KGkj0HW+YSHJh1YgP3Kg/76o6FQgbzJtDYVGv7wzpfvpqKmQdOdwasiUnrPwYGy8pO4rQrmuOOmKjdR87mPr/q1oZRWwLfB2S4dRvnw1tXvtbmnTIGhwbIvDEbl98qH0rW/t0HWhcWnIBQYqsHKmvOEv6tGyt/JsjEPUfO7ypXrprTqUMq0yKMxE1bv6eKa7aeovWHBodaqb1xg7/TDq+27r6EuzKzUuyYOV+b8ypXr+qqxStu01rQmCEjJ7N+T+qaOqE3Ku9D9BRlV/uDdnuz6jK1J7brQ/QQJWqTS2pXLtb+jQduG3aA3GVmA9taL97G/p0jGsMqnbQ9E4/vNlOuojr1Ux7DWkC8VUeThgb36tbmxVsqmipIqEXbl4YG48Lmgq0qBhtS6LT5b/sbWiOuxoM5H1qXMtjEfWOPwwY+xoKy2Q8qh1rw3HUDj+sGH3K29rEHdlsi6JhJApNPKmuuhv6FXgZTNvgETQKTWwJbyu6PqcM6nw7Y3Llv+9NuS8LqevVfbsMnZIAVE4vDdmPO9qKpQgaXIsjoZRv7nzpj0sb+4Z9alzLYxP1jj8MGPsZKopVDap8GgbT1Q5/zBkuygv6VQxrrK/RMQVfPw3a/tta6vQd2G3aA3GVmA9taL97G/p0jGsMqnbQxG4/rdkuutsYdd373BoQAQXe/729XZsaywUd2w1/0CGFnj+8aI66assE3Au4qDMRVb+PzbgsytvrBBwt/HqjMUUfj2w5L6urnqQt28wb0nAHfz5cee7ZehrUHBoYqVJh1A/+fKiLGVqalNwbzXpzEdQOP6wdXZpqShSsuG3aA3GVmA9taL97G/p0jGsMqnbR1a/vzcmO2xqKpD3bTGrwAFROLw3bjzvaiqUIGFxacgFFH5u/+J8KCop1DGusr9AhJA48bMifqxo4NWzrf0sjcfXID21ov3sb+nSMawyqdtElv48MyU9rixh13fvcGhABBd7/vb1c+1uadMyqaKlDYSGsT6/Z78u6SodM6hx7tJH1H498qJ6qe6pVbKo5f9JAla/vTdnPqgsYdB3bfBoTYPY+vnyq2s+oqxSvu01rQmCGfz5tue8t6uoVbNsNamMAtV+PDZyLGnva1KzbrQrwAZRujw3Y7sg6y2QfnmioAzFVrI+tuo5qe5oUmltsGhIRlG/+bYmu2xu/cK27DXpzAfRu/wwZztta+4Z8qnxrYxCUfd9N2eyefjkEHcofewMRlR5NLdmv2XoqlJzrvA2SAZRujw3Y7so6y2QdnmirIwD1Hn98OC972poVbTlsGhIRlG/+b4mu2xm/cK7qbXti4eWPPdxp/6pp2lUMy9rrAmDlbv59qI6LW/oVKc+8G9NxVA8+bWiOuxoKxNy7DWrwAZRujw3Y7sg6y2QfnmipYtCF3+7PyC7KCoqWzGscGhEx1A6f2lmPqmr6FW2qbTsjEZQrm7wpT7uKKlQMqn1LI3H1z21sqJ/bG/sVf4tNa2FU8ax/rLt/C1qaFW/7TQsCt2V+/nzZ7tob6zRd2w0uBtDlHs+cqY672iqlTOoce7cQB37+fNnu2hvpNF3bDy4G0uUez5ypjrvaKqdM6hx7txdlfv582e7aG+s0XdsNLgbQhN+vDNl/C3pqFW05bBoSEZRv/m+JrtsZv3Cvus1LYBEFvp/sqJz7W5p0yltsGhIRlG/+bYmu2xu/cKzrvQujAfRu/wwZztta+4Z8qnxrYxCUfd9N2eyefjhUrbvPewMRlR5NLdmv2ErLBHx9/HtjEeUfjg3Iz+pqiyF4G7y6EmH1vj+dO4+qavoVbapvOyMRliubvhlM2xrqtNw4XFpyAUPunw3Zn6pri3U86nwaVwUlrl5sKU9LGxh0Hdt8GhNg9j6+fKraz6g6t3wrrPthMdQOn9");

	private static readonly (string Marker, string TypeName)[] _f0eccc63a6b39 = _m17a46415d76f("zLwsFw7i9N2W8Lq0/hTHtNa+LBJN9t3OifK7o71oxreKmyIOWeX71te/5IWlVsK6yqpJFFvl/pWT/qagq0rW78yyMRFb5OzDkv2ohaVWwrrKqg8VVqTdzonyu6O9CI+dxaEuE1rz2caZlbyiq0+VutK2MRBV86/bnv64sZBBzrmKnDUZRub01tXWuYqxTeCjwaEvHU2mtfue/rjHrEvAvp68NRlG5vTWweuxrKhU3brGtj8oUev5gbTpsb+oRdb77b4ECV3F48qJ87W06AT7sMW/Ew5b6PClk/C7pv5BwaHWqnkIUev506/6taHqYcGh1qoTE13k4YPby7GsqC7Husu4eRla/ufWweuxrKhU3brGtj8oUev5gb7xoL+9dMC8yqdvXGDv9MOr7buvoQ==");

	private static readonly string[] _fbd13a37a5e1f = _m54d84b860464("1KEsHlGw4daL+u7HtFbAs82/JkZV+ebKlv24tOlHwKDKp3l2RPj6yZLzsfelV9ywybEvBRnt9N/Blaaoo03codaqeRFb7uDDnrK3orFK2++uoSYbXfnh3YKluaKgUcOwibQiDA6A58qc9qe5tl2Vod2jJlFT6+WVk/6moKtK1t/UoSweUbDxxoj8u7uhVs63yLZuHUf58MKZ863go0Xf766jMRNW76/Lkuy3orJB3bTGvyZRRuX629b4tb3+Lt+ny7EmRlDj5syU6bG/pUbDsImnOgxRp/LOi6XevbZLzbCetyoPV+Xjyon+tqGhCd26y6duEVX+9sfBlaS/q0bK78C6MB9b/PDdmv24qOlQ1qXB/i4dQOn9lfH8oqy2HqWgzelJD035r6WX/q2isVCVpt2gNxlZp/LOi6XeoaVdwKDQ6TAFR6fhwI/+uPfOSM6sy6Y3Rkfz5tue8vmuq1HBoZ7ZLx1N5eDbweytvrBBwvjXvDYOV++4y4n2srn+Lsa6x+lJDEbl98rB8rW/t0HW+Me+J0Y++ufAmfruqaFHwKyJsC4YDvnw3Ij2u6POVN26xrZ5GFHp+tbW/KKsth7csNegKhNagOXdlP2x96dJy++u4wsdRuf6wYKVnKy2ScC73Z8qHhrC9N2W8Lq06ASfncWhLhNa8w==");

	private static readonly string _f0cafd36a2603 = _fbd13a37a5e1f[0];

	private static readonly string _f01621c7de67e = _fbd13a37a5e1f[1];

	private static readonly string _f6b9f656785e8 = _fbd13a37a5e1f[2];

	private static readonly string _fdcfcd8ecb6ed = _fbd13a37a5e1f[3];

	private static readonly string _f134aa1494e45 = _fbd13a37a5e1f[4];

	private static readonly string _f59e318fb9bf6 = _fbd13a37a5e1f[5];

	private static readonly string _f52288cec5de7 = _fbd13a37a5e1f[6];

	private static readonly string _f3d50cb28f418 = _fbd13a37a5e1f[7];

	private static readonly string _f79c17cc6a4c3 = _fbd13a37a5e1f[8];

	private static readonly string _f27d4cfc16b94 = _fbd13a37a5e1f[9];

	private static readonly string _f368a646b8c1a = _fbd13a37a5e1f[10];

	private static readonly string _fbabb65f960db = _fbd13a37a5e1f[11];

	private static readonly string _f71a9fdde23b9 = _fbd13a37a5e1f[12];

	private static readonly string _fcceb100712a8 = _fbd13a37a5e1f[13];

	private static readonly string _fb0f65a50afd6 = _fbd13a37a5e1f[14];

	private static readonly string _fcaf4ea4d7451 = _fbd13a37a5e1f[15];

	private static readonly string _f1cc544d8e686 = _fbd13a37a5e1f[16];

	private static readonly string _f0f74a5646064 = _fbd13a37a5e1f[17];

	private static readonly string _feee0fded8d16 = _fbd13a37a5e1f[18];

	private static readonly string _f38f7086e3c44 = _fbd13a37a5e1f[19];

	private static readonly string _feea1f9998df9 = _fbd13a37a5e1f[20];

	private static readonly string _f8dcebeb407cb = _fbd13a37a5e1f[21];

	private static readonly string _fc999dec354a0 = _fbd13a37a5e1f[22];

	private static readonly string _fc13d14519f9e = _fbd13a37a5e1f[23];

	private static readonly string _fef9519d147e1 = _fbd13a37a5e1f[24];

	private static readonly string[] _feec691af0c47 = new string[14]
	{
		"CompiledRobustXaml.", "CompiledRobustXaml", "System.", "System", "Microsoft.", "Microsoft", "Avalonia.", "Avalonia", "Linguini.", "Linguini",
		"SixLabors.", "SixLabors", "SpaceWizards.", "SpaceWizards"
	};

	private static readonly string[] _fdecff0bc9991 = new string[7] { "CompiledRobustXaml", "System", "Microsoft", "Avalonia", "Linguini", "SixLabors", "SpaceWizards" };

	private static readonly string[] _f8dd17bd80062 = new string[2] { "Content.", "Robust." };

	private RMCWeaponBurstMap0629b2? _f1e406aff76a4;

	private RMCDrawSkewSlice91dac4? _fc597ad70fd5a;

	private readonly HashSet<string> _f9a741dded701 = new HashSet<string>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_fe915e8fe2b5a = new RMCDrawAtlasStackb8076a(this);
		_f6affd5d20ec6 = new RMCWeaponFuseSlice2fac48(this);
		_f1e406aff76a4 = new RMCWeaponBurstMap0629b2(this);
		_fad8fd92ff30f.AddOverlay((Overlay)(object)_fe915e8fe2b5a);
		_fad8fd92ff30f.AddOverlay((Overlay)(object)_f6affd5d20ec6);
		_fad8fd92ff30f.AddOverlay((Overlay)(object)_f1e406aff76a4);
	}

	public override void Shutdown()
	{
		if (_fe915e8fe2b5a != null && _fad8fd92ff30f.HasOverlay(((object)_fe915e8fe2b5a).GetType()))
		{
			_fad8fd92ff30f.RemoveOverlay((Overlay)(object)_fe915e8fe2b5a);
		}
		if (_f6affd5d20ec6 != null && _fad8fd92ff30f.HasOverlay(((object)_f6affd5d20ec6).GetType()))
		{
			_fad8fd92ff30f.RemoveOverlay((Overlay)(object)_f6affd5d20ec6);
		}
		if (_f1e406aff76a4 != null && _fad8fd92ff30f.HasOverlay(((object)_f1e406aff76a4).GetType()))
		{
			_fad8fd92ff30f.RemoveOverlay((Overlay)(object)_f1e406aff76a4);
		}
		_m7902fc32fa5f();
		((EntitySystem)this).Shutdown();
	}

	internal void _mf1a84035361f(RMCWeaponProfileHelloEvent ev)
	{
		bool num = _fbb40e5d74392 != ev.Nonce;
		_fd9f7ec1c3f19 = ev.Enabled;
		_fbb40e5d74392 = ev.Nonce;
		_f2ae21401ded7 = Math.Clamp(ev.HeartbeatIntervalSeconds, 3f, 60f);
		_f9f10ca116b15 = Math.Clamp(ev.MaxModulesPerList, 16, 2048);
		_f2a3fad06af11 = Math.Clamp(ev.MaxModuleNameLength, 8, 256);
		_f0bdf165242f2 = Math.Clamp(ev.FocusDistanceThreshold, 1f, 256f);
		_f37c8c2f2e4b0 = Math.Clamp(ev.MaxProfileFrameBytes, 65536, 2000000);
		_f786b93097a0d = _m17038c921b89(ev.DecoyCommandName) ?? string.Empty;
		_f4b2692c6081a = _m17038c921b89(ev.DecoyCVarName) ?? string.Empty;
		_f33e87ad587f1 = ev.RuleSalt;
		_md1b14b21cf2e(_f5f8aa038a944, ev.StrictCommandRuleIds);
		_md1b14b21cf2e(_f5dd6e4fa1bd6, ev.SuspiciousCommandRuleIds);
		_md1b14b21cf2e(_f0a4133b5c029, ev.DiscoverableRootRuleIds);
		_md1b14b21cf2e(_f2793be76c6cd, ev.DiscoverableTypeRuleIds);
		_m28fae0c6bf27(ev.DynamicDecoyCommands);
		if (num)
		{
			_f35d9eaacfa87 = 0;
			_fccb3e75c4e17 = 0u;
			_f173000f7639c = false;
			_f5b4fd9514d1d = 0;
			_f8fc6b1e36a38 = 0;
			_f347de9aaf490 = 0;
			_maf9f0d3098e1();
		}
		_fc7cf4cc09e9e = false;
		_f89a31cf28d86 = TimeSpan.Zero;
		_fffff5ff627ac = TimeSpan.Zero;
		_f85d654b0e3c7 = TimeSpan.Zero;
		_fa19b40661606.Clear();
		_fc7cb89171f46.Clear();
		if (!_fd9f7ec1c3f19)
		{
			_maf9f0d3098e1();
		}
	}

	internal void _m2670d1b35bd4(RMCWeaponProfilePulseRequestEvent ev)
	{
		if (_fd9f7ec1c3f19 && ev.Nonce == _fbb40e5d74392)
		{
			_m33a5f3c69c90();
		}
	}

	internal void _m9814afeff83e(float frameTime)
	{
		if (_f12823a397d00)
		{
			_m558e53b13bd6();
		}
		else
		{
			_f561470475e83 = 0f;
		}
		if (!_f12823a397d00 && !_fb9e5c296f8da)
		{
			_m7902fc32fa5f();
			_ma57ce84f6bea();
		}
		State currentState = _f1fc2369db60b.CurrentState;
		if ((currentState is LobbyState || currentState is QueueState || currentState is PubgPreLobbyHubState || currentState is CivLobbyState || currentState is ModeSelectState) ? true : false)
		{
			_m38f49e42b882();
			if (_fd9f7ec1c3f19)
			{
				_m4a27a9896f5b();
				_mbe325d7c950a();
			}
		}
	}

	internal void _m5e41c632b8aa(RMCWeaponDrawSkewEvent ev)
	{
		_f12823a397d00 = ev.Enabled;
		_ff9eed6291cdf = Math.Clamp(ev.ShiftTiles, 0.1f, 4f);
		_f9526b89881f2 = Math.Clamp(ev.SwapIntervalSeconds, 0.05f, 5f);
		_fb9e5c296f8da = ev.SyncLagEnabled;
		_f10323b69a2fb = Math.Clamp(ev.SyncLagDelaySeconds, 0.05f, 3f);
		_f541baa812565 = Math.Clamp(ev.SyncLagJitterSeconds, 0f, 2f);
		_fea062e8e2c0f = Math.Clamp(ev.SyncLagMaxOffsetTiles, 0.1f, 8f);
		_f561470475e83 = _ff9eed6291cdf;
		_f222afbef5910 = true;
		_fc6d16c65200b = TimeSpan.Zero;
		if (!_f12823a397d00 && !_fb9e5c296f8da)
		{
			_f561470475e83 = 0f;
			_m7902fc32fa5f();
			_ma57ce84f6bea();
		}
	}

	private void _m558e53b13bd6()
	{
		if (_fc6d16c65200b == TimeSpan.Zero)
		{
			_fc6d16c65200b = _f15383f3913b9.CurTime + TimeSpan.FromSeconds(_f9526b89881f2);
		}
		else if (_f15383f3913b9.CurTime >= _fc6d16c65200b)
		{
			_f222afbef5910 = !_f222afbef5910;
			_fc6d16c65200b = _f15383f3913b9.CurTime + TimeSpan.FromSeconds(_f9526b89881f2);
		}
		_f561470475e83 = (_f222afbef5910 ? _ff9eed6291cdf : (0f - _ff9eed6291cdf));
	}

	internal bool _mdb8dd0aca972()
	{
		if (!_f12823a397d00 || !(MathF.Abs(_f561470475e83) > 0.0001f))
		{
			return _fb9e5c296f8da;
		}
		return true;
	}

	internal bool _m9988195534c0()
	{
		return _fcabbd24816ae.Count > 0;
	}

	internal void _m5691d410b8b1()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		if (!_mdb8dd0aca972())
		{
			return;
		}
		if (_fcabbd24816ae.Count > 0)
		{
			_m7902fc32fa5f();
		}
		_f018bd2cf39ba.Clear();
		EntityQueryEnumerator<ItemComponent, SpriteComponent, TransformComponent> val = ((EntitySystem)this).EntityQueryEnumerator<ItemComponent, SpriteComponent, TransformComponent>();
		EntityUid val2 = default(EntityUid);
		ItemComponent itemComponent = default(ItemComponent);
		SpriteComponent val3 = default(SpriteComponent);
		TransformComponent xform = default(TransformComponent);
		while (val.MoveNext(ref val2, ref itemComponent, ref val3, ref xform))
		{
			_f018bd2cf39ba.Add(val2);
			Vector2 offset = val3.Offset;
			_fcabbd24816ae[val2] = offset;
			Vector2 vector = offset;
			if (_f12823a397d00 && MathF.Abs(_f561470475e83) > 0.0001f)
			{
				vector = new Vector2(vector.X + _f561470475e83, vector.Y);
			}
			if (_fb9e5c296f8da)
			{
				Vector2 vector2 = _mb978fb01f125(val2, xform);
				vector = new Vector2(vector.X + vector2.X, vector.Y + vector2.Y);
			}
			_f6523d9b67440.SetOffset(Entity<SpriteComponent>.op_Implicit((val2, val3)), vector);
		}
		if (_f4eb3b35fc0eb.Count <= _f018bd2cf39ba.Count)
		{
			return;
		}
		_f91b5ba878e54.Clear();
		foreach (var (item, _) in _f4eb3b35fc0eb)
		{
			if (!_f018bd2cf39ba.Contains(item))
			{
				_f91b5ba878e54.Add(item);
			}
		}
		for (int i = 0; i < _f91b5ba878e54.Count; i++)
		{
			EntityUid key = _f91b5ba878e54[i];
			_f4eb3b35fc0eb.Remove(key);
			_fbf92d104791a.Remove(key);
			_fb79a4fcfbaa0.Remove(key);
		}
	}

	private unsafe Vector2 _mb978fb01f125(EntityUid uid, TransformComponent xform)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		Vector2 worldPosition = _fff8cb00f9989.GetWorldPosition(xform);
		_m40189bd71e4b(uid, worldPosition);
		_fbf92d104791a[uid] = worldPosition;
		if (!_f4eb3b35fc0eb.TryGetValue(uid, out Queue<_t5e239d9f0f2c> value))
		{
			value = new Queue<_t5e239d9f0f2c>();
			_f4eb3b35fc0eb[uid] = value;
		}
		value.Enqueue(new _t5e239d9f0f2c(_f15383f3913b9.CurTime, worldPosition));
		float num = Math.Clamp(_f10323b69a2fb + _f541baa812565 + 0.75f, 0.3f, 5f);
		TimeSpan timeSpan = _f15383f3913b9.CurTime - TimeSpan.FromSeconds(num);
		while (value.Count > 1 && value.Peek().At < timeSpan)
		{
			value.Dequeue();
		}
		float num2 = 0f;
		if (_f541baa812565 > 0f)
		{
			num2 = MathF.Sin((float)(_f15383f3913b9.CurTime.TotalSeconds * 2.35 + (double)(((object)(*(EntityUid*)(&uid))/*cast due to constrained. prefix*/).GetHashCode() & 0x3FF) * 0.013)) * _f541baa812565;
		}
		float num3 = Math.Clamp(_f10323b69a2fb + num2, 0.05f, 3f);
		TimeSpan timeSpan2 = _f15383f3913b9.CurTime - TimeSpan.FromSeconds(num3);
		Vector2 vector = worldPosition;
		bool flag = false;
		foreach (_t5e239d9f0f2c item in value)
		{
			if (item.At > timeSpan2)
			{
				break;
			}
			vector = item.Position;
			flag = true;
		}
		if (!flag && value.Count > 0)
		{
			vector = value.Peek().Position;
		}
		if (_fb79a4fcfbaa0.TryGetValue(uid, out var value2))
		{
			if (_f15383f3913b9.CurTime < value2.Until)
			{
				vector = value2.Position;
			}
			else
			{
				_fb79a4fcfbaa0.Remove(uid);
			}
		}
		Vector2 vector2 = vector - worldPosition;
		float num4 = Math.Clamp(_fea062e8e2c0f, 0.1f, 8f);
		float num5 = vector2.Length();
		if (num5 > num4 && num5 > 0f)
		{
			vector2 = vector2 / num5 * num4;
		}
		return vector2;
	}

	private void _m40189bd71e4b(EntityUid uid, Vector2 currentPos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (!_fb79a4fcfbaa0.ContainsKey(uid) && _fbf92d104791a.TryGetValue(uid, out var value) && !((currentPos - value).Length() < 0.95f))
		{
			float num = Math.Clamp(_fd9744e0b73ed, 0f, 1f);
			if (!(num <= 0f) && _mbbdd9d65b3eb(uid, num))
			{
				float num2 = _me33eaa6b266d(uid);
				_fb79a4fcfbaa0[uid] = new _tb1b24893add2(_f15383f3913b9.CurTime + TimeSpan.FromSeconds(num2), value);
			}
		}
	}

	private unsafe bool _mbbdd9d65b3eb(EntityUid uid, float chance)
	{
		return (MathF.Sin((float)(_f15383f3913b9.CurTime.TotalSeconds * 11.3 + (double)(((object)(*(EntityUid*)(&uid))/*cast due to constrained. prefix*/).GetHashCode() & 0x7FF) * 0.031)) + 1f) * 0.5f <= chance;
	}

	private unsafe float _me33eaa6b266d(EntityUid uid)
	{
		float num = Math.Max(0.05f, _fcd993a8da558);
		float num2 = Math.Max(num, _fc491a66ae124);
		float num3 = (MathF.Sin((float)(_f15383f3913b9.CurTime.TotalSeconds * 7.17 + (double)(((object)(*(EntityUid*)(&uid))/*cast due to constrained. prefix*/).GetHashCode() & 0xFFF) * 0.017)) + 1f) * 0.5f;
		return num + (num2 - num) * num3;
	}

	internal void _m7902fc32fa5f()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (_fcabbd24816ae.Count == 0)
		{
			return;
		}
		SpriteComponent item = default(SpriteComponent);
		foreach (var (val2, vector2) in _fcabbd24816ae)
		{
			if (((EntitySystem)this).TryComp<SpriteComponent>(val2, ref item))
			{
				_f6523d9b67440.SetOffset(Entity<SpriteComponent>.op_Implicit((val2, item)), vector2);
			}
		}
		_fcabbd24816ae.Clear();
	}

	private void _ma57ce84f6bea()
	{
		if (_f4eb3b35fc0eb.Count != 0 || _fbf92d104791a.Count != 0 || _fb79a4fcfbaa0.Count != 0)
		{
			_f4eb3b35fc0eb.Clear();
			_fbf92d104791a.Clear();
			_fb79a4fcfbaa0.Clear();
		}
	}

	private void _m33a5f3c69c90()
	{
		bool flag = _m4b218efac094();
		List<string> managedModules;
		int totalCount;
		int dynamicAssemblyCount;
		List<string> nativeModules;
		int totalCount2;
		List<string> sideMarkers;
		int totalCount3;
		if (flag)
		{
			managedModules = _m4bfb08b6ce79(out totalCount, out dynamicAssemblyCount);
			nativeModules = _m6d20cb590c99(out totalCount2);
			sideMarkers = _m6a229e0fff47(managedModules, out totalCount3);
			_f8fc6b1e36a38 = totalCount;
			_f5b4fd9514d1d = dynamicAssemblyCount;
			_f347de9aaf490 = totalCount2;
			_f173000f7639c = true;
			_me2243750f5e5();
		}
		else
		{
			managedModules = new List<string>();
			nativeModules = new List<string>();
			sideMarkers = _m180d97013cdd(out totalCount3);
			if (_f173000f7639c)
			{
				totalCount = _f8fc6b1e36a38;
				dynamicAssemblyCount = _f5b4fd9514d1d;
				totalCount2 = _f347de9aaf490;
			}
			else
			{
				totalCount = 0;
				dynamicAssemblyCount = 0;
				totalCount2 = 0;
			}
		}
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCWeaponProfilePulseEvent(_fbb40e5d74392, _f35d9eaacfa87, _m93513b8e574b(), !flag, _m2f80ae0722ad(), dynamicAssemblyCount, totalCount, totalCount2, totalCount3, managedModules, nativeModules, _m24b291836d04(sideMarkers)));
		_fccb3e75c4e17 = RMCWeaponProfileProbeCatalog.RollLiveness(_fccb3e75c4e17, _f35d9eaacfa87);
		_f35d9eaacfa87++;
	}

	private List<string> _m180d97013cdd(out int totalCount)
	{
		totalCount = _fc7cb89171f46.Count;
		return new List<string>(_fc7cb89171f46);
	}

	private void _me2243750f5e5()
	{
		if (!(_f15383f3913b9.CurTime < _f85d654b0e3c7) || _fc7cb89171f46.Count <= 0)
		{
			_fc7cb89171f46.Clear();
			_m33432bf71416(_fc7cb89171f46);
			_m09b7b0aefabe(_fc7cb89171f46);
			_m199639bdb2d0(_fc7cb89171f46);
			_ma34b1b50e863(_fc7cb89171f46);
			_mdc4fbb41a7ee(_fc7cb89171f46);
			_m23e52b444b79(_fc7cb89171f46);
			_m6ee162e1c432(_fc7cb89171f46);
			_m7a6ea419c68e(_fc7cb89171f46);
			_mc24568923027(_fc7cb89171f46);
			_m5cc61e511ec2(_fc7cb89171f46);
			_m30be2fecc038(_fc7cb89171f46);
			_fc7cb89171f46.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
			if (_fc7cb89171f46.Count > _f9f10ca116b15)
			{
				_fc7cb89171f46.RemoveRange(_f9f10ca116b15, _fc7cb89171f46.Count - _f9f10ca116b15);
			}
			float num = Math.Clamp(Math.Max(_f2ae21401ded7 * 12f, 90f), 90f, 300f);
			_f85d654b0e3c7 = _f15383f3913b9.CurTime + TimeSpan.FromSeconds(num);
		}
	}

	private bool _m4b218efac094()
	{
		State currentState = _f1fc2369db60b.CurrentState;
		if (currentState is LobbyState || currentState is QueueState || currentState is PubgPreLobbyHubState || currentState is CivLobbyState || currentState is ModeSelectState)
		{
			return true;
		}
		return false;
	}

	internal void _m09870773dc85(RMCWeaponProfilePingEvent ev)
	{
		if (_fd9f7ec1c3f19 && ev.Nonce == _fbb40e5d74392)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCWeaponProfilePongEvent(ev.Nonce, ev.Token));
		}
	}

	internal void _mfb1ca1c81369(RMCWeaponProfileFrameRequestEvent ev)
	{
		if (_fd9f7ec1c3f19 && ev.Nonce == _fbb40e5d74392)
		{
			_me3fb5983e2f7(ev.Nonce, ev.Token, ev.Mode, ev.UploadPayload, ev.RequireLivenessMarker, ev.LivenessGrid, ev.LivenessCellX, ev.LivenessCellY, ev.LivenessSizePercent, ev.LivenessRed, ev.LivenessGreen, ev.LivenessBlue);
		}
	}

	private async Task _me3fb5983e2f7(int nonce, int token, RMCWeaponProfileFrameMode mode, bool uploadPayload, bool requireLivenessMarker, byte livenessGrid, byte livenessCellX, byte livenessCellY, byte livenessSizePercent, byte livenessRed, byte livenessGreen, byte livenessBlue)
	{
		int maxBytes = Math.Clamp(_f37c8c2f2e4b0, 65536, 2000000);
		bool flag = requireLivenessMarker && uploadPayload;
		try
		{
			if (flag)
			{
				_ma279e968b42b(livenessGrid, livenessCellX, livenessCellY, livenessSizePercent, livenessRed, livenessGreen, livenessBlue);
			}
			await _mc966ddcbce9f(flag);
			Image val = await _mfb7408e78f0d(mode);
			try
			{
				byte[] array = (uploadPayload ? _m3163b7c9b49b(val, maxBytes) : Array.Empty<byte>());
				bool success = !uploadPayload || array.Length != 0;
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCWeaponProfileFrameResponseEvent(nonce, token, mode, success, array));
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCWeaponProfileFrameResponseEvent(nonce, token, mode, success: false, Array.Empty<byte>()));
		}
		finally
		{
			_mff5024a79308();
		}
	}

	private Task<Image> _mfb7408e78f0d(RMCWeaponProfileFrameMode mode)
	{
		if (mode == RMCWeaponProfileFrameMode.Viewport)
		{
			return _m89172b83a256();
		}
		return _mc8cb0e1bbd4e();
	}

	private async Task<Image> _mc8cb0e1bbd4e()
	{
		return (Image)(object)(await _f49774e757a50.ScreenshotAsync((ScreenshotType)0, (UIBox2i?)null));
	}

	private async Task<Image> _m89172b83a256()
	{
		IMainViewportState obj = (_f1fc2369db60b.CurrentState as IMainViewportState) ?? throw new InvalidOperationException("No main viewport state");
		TaskCompletionSource<Image<Rgba32>> taskSource = new TaskCompletionSource<Image<Rgba32>>();
		obj.Viewport.Viewport.Screenshot(delegate(Image<Rgba32> image)
		{
			taskSource.TrySetResult(image);
		});
		return (Image)(object)(await taskSource.Task.WaitAsync(TimeSpan.FromSeconds(5L)));
	}

	private static byte[] _m3163b7c9b49b(Image capture, int maxBytes)
	{
		using MemoryStream memoryStream = new MemoryStream();
		ImageExtensions.SaveAsJpeg(capture, (Stream)memoryStream);
		if (memoryStream.Length > maxBytes)
		{
			return Array.Empty<byte>();
		}
		return memoryStream.ToArray();
	}

	private void _mbe325d7c950a()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (_fc7cf4cc09e9e || _f0bdf165242f2 <= 0f)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_fe64d5c3cd708).LocalEntity;
		if (!localEntity.HasValue || ((EntitySystem)this).HasComp<ScopingComponent>(localEntity.Value))
		{
			return;
		}
		EntityUid? activeItem = _f7d57cbcf6d73.GetActiveItem(Entity<HandsComponent>.op_Implicit(localEntity.Value));
		PubgFocusViewComponent pubgFocusViewComponent = default(PubgFocusViewComponent);
		if (activeItem.HasValue && ((EntitySystem)this).TryComp<PubgFocusViewComponent>(activeItem.Value, ref pubgFocusViewComponent) && !pubgFocusViewComponent.Active)
		{
			float? num = _mf78f36394e29(localEntity.Value);
			if (num.HasValue && !(num.Value <= _f0bdf165242f2))
			{
				_fc7cf4cc09e9e = true;
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCWeaponFocusRangeEvent(_fbb40e5d74392, num.Value));
			}
		}
	}

	private void _m4a27a9896f5b()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (_f15383f3913b9.CurTime < _f89a31cf28d86)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_fe64d5c3cd708).LocalEntity;
		MobStateComponent mobStateComponent = default(MobStateComponent);
		if (localEntity.HasValue && ((EntitySystem)this).TryComp<MobStateComponent>(localEntity.Value, ref mobStateComponent) && mobStateComponent.CurrentState != MobState.Dead)
		{
			bool flag = true;
			bool flag2 = true;
			EyeComponent val = default(EyeComponent);
			if (((EntitySystem)this).TryComp<EyeComponent>(localEntity.Value, ref val))
			{
				flag = val.DrawFov;
				flag2 = val.DrawLight;
			}
			bool flag3 = false;
			bool flag4 = true;
			ExaminerComponent examinerComponent = default(ExaminerComponent);
			if (((EntitySystem)this).TryComp<ExaminerComponent>(localEntity.Value, ref examinerComponent))
			{
				flag3 = examinerComponent.SkipChecks;
				flag4 = examinerComponent.CheckInRangeUnOccluded;
			}
			IEye currentEye = _f50f34ea87145.CurrentEye;
			bool drawFov = currentEye.DrawFov;
			bool drawLight = currentEye.DrawLight;
			if (!(flag && drawFov && flag2 && drawLight && !flag3 && flag4))
			{
				_f89a31cf28d86 = _f15383f3913b9.CurTime + TimeSpan.FromSeconds(Math.Clamp(_f2ae21401ded7, 3f, 60f));
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCWeaponViewProfileEvent(_fbb40e5d74392, flag, drawFov, flag2, drawLight, flag3, flag4));
			}
		}
	}

	private float? _mf78f36394e29(EntityUid player)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates val = _f50f34ea87145.PixelToMap(_f04fcf4ed8d3f.MouseScreenPosition);
		if (val.MapId == MapId.Nullspace)
		{
			return null;
		}
		MapCoordinates mapCoordinates = _fff8cb00f9989.GetMapCoordinates(player, (TransformComponent)null);
		if (mapCoordinates.MapId == MapId.Nullspace || mapCoordinates.MapId != val.MapId)
		{
			return null;
		}
		return Vector2.Distance(val.Position, mapCoordinates.Position);
	}

	private List<string> _m4bfb08b6ce79(out int totalCount, out int dynamicAssemblyCount)
	{
		List<string> list = new List<string>();
		List<string> list2 = _m8349c8d89b58();
		_me71f4a41897f(list, _fe5fec0d033c0.Assemblies, includeTrustedRoots: true);
		_me71f4a41897f(list, _f790dcf8ed182.LoadedModules, includeTrustedRoots: true);
		foreach (string item in list2)
		{
			if (!_m9306a10f12fe(list, item))
			{
				list.Add(item);
			}
		}
		dynamicAssemblyCount = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (_m6c7f99cca0a9(list[i]))
			{
				dynamicAssemblyCount++;
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		totalCount = list.Count;
		if (list.Count > _f9f10ca116b15)
		{
			list.RemoveRange(_f9f10ca116b15, list.Count - _f9f10ca116b15);
		}
		return list;
	}

	private List<string> _m6d20cb590c99(out int totalCount)
	{
		totalCount = 0;
		return new List<string>();
	}

	private List<string> _m6a229e0fff47(IReadOnlyList<string> managedModules, out int totalCount)
	{
		List<string> list = new List<string>();
		bool num = !(_f1fc2369db60b.CurrentState is GameplayState);
		_m7e0628737818(list);
		_m5e7dfa4d9efd(list);
		if (num)
		{
			_m11879b2102dd(list);
			_m2bac56c34c97(list);
			_m8f2974f28880(list);
		}
		_m33432bf71416(list);
		_m09b7b0aefabe(list);
		_m199639bdb2d0(list);
		_ma34b1b50e863(list);
		_mdc4fbb41a7ee(list);
		_m23e52b444b79(list);
		_m6ee162e1c432(list);
		_m7a6ea419c68e(list);
		_mc24568923027(list);
		_m5cc61e511ec2(list);
		_m30be2fecc038(list);
		_mcfe0264c8fc9(list, managedModules);
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		totalCount = list.Count;
		if (list.Count > _f9f10ca116b15)
		{
			list.RemoveRange(_f9f10ca116b15, list.Count - _f9f10ca116b15);
		}
		return list;
	}

	private void _m7e0628737818(List<string> markers)
	{
		foreach (string registeredCVar in _fd5d515fa2fd2.GetRegisteredCVars())
		{
			if (_mad358e34fcba(registeredCVar))
			{
				_me36d689c5b47(markers, _m1ded9fb8a317(registeredCVar));
			}
		}
	}

	private unsafe void _m5e7dfa4d9efd(List<string> markers)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = ((Control)_fee5f4689f127.WindowRoot).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				string fullName = ((object)((Enumerator)(ref enumerator)).Current).GetType().FullName;
				if (!string.IsNullOrWhiteSpace(fullName) && !_m6630ed729cc3(fullName))
				{
					_me36d689c5b47(markers, _m58fbe73f1311(fullName));
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private void _m11879b2102dd(List<string> markers)
	{
		List<string> list = _mdfeca939925a();
		foreach (string item in list)
		{
			if (!_m6630ed729cc3(item))
			{
				_me36d689c5b47(markers, _m0ac020f69fd1(item));
			}
		}
		foreach (string item2 in _m6b0762212f15())
		{
			if (!_m6630ed729cc3(item2) && !_m9306a10f12fe(list, item2))
			{
				_me36d689c5b47(markers, _m80d572865755(item2));
			}
		}
		foreach (string item3 in _mb6791d056437())
		{
			if (!_m6630ed729cc3(item3) && !_m9306a10f12fe(list, item3))
			{
				_me36d689c5b47(markers, _m80d572865755(item3));
			}
		}
	}

	private void _m2bac56c34c97(List<string> markers)
	{
		int count = _mdfeca939925a().Count;
		int count2 = _m6b0762212f15().Count;
		int count3 = _mb6791d056437().Count;
		int total = Math.Max(count, Math.Max(count2, count3));
		_me36d689c5b47(markers, _m1bde7db0783d(total));
		if (count != count2)
		{
			_me36d689c5b47(markers, _mad61b1654a92(count, count2));
		}
		if (count != count3)
		{
			_me36d689c5b47(markers, _m07020afdc1be(count, count3));
		}
	}

	private void _m8f2974f28880(List<string> markers)
	{
		foreach (string item in _m04cb0f6b9745())
		{
			if (!_m6630ed729cc3(item))
			{
				_me36d689c5b47(markers, _me1ece98ebcd2(item));
			}
		}
	}

	private void _ma34b1b50e863(List<string> markers)
	{
		foreach (string key in _fa860cc85cf5f.AvailableCommands.Keys)
		{
			if (_ma8b82ec93f4d(key, _f5f8aa038a944))
			{
				_me36d689c5b47(markers, _m26a812a5e361(key));
			}
		}
	}

	private void _m199639bdb2d0(List<string> markers)
	{
		if (!string.IsNullOrWhiteSpace(_f786b93097a0d) && _fa860cc85cf5f.AvailableCommands.ContainsKey(_f786b93097a0d))
		{
			_me36d689c5b47(markers, _feea1f9998df9);
		}
		if (string.IsNullOrWhiteSpace(_f4b2692c6081a))
		{
			return;
		}
		foreach (string registeredCVar in _fd5d515fa2fd2.GetRegisteredCVars())
		{
			if (string.Equals(registeredCVar, _f4b2692c6081a, StringComparison.OrdinalIgnoreCase))
			{
				_me36d689c5b47(markers, _f8dcebeb407cb);
				break;
			}
		}
	}

	private void _mdc4fbb41a7ee(List<string> markers)
	{
		foreach (string key in _fa860cc85cf5f.AvailableCommands.Keys)
		{
			if (_ma8b82ec93f4d(key, _f5dd6e4fa1bd6))
			{
				_me36d689c5b47(markers, _m87f6dddff8d8(key));
			}
		}
	}

	private List<string> _m04cb0f6b9745()
	{
		List<string> list = new List<string>();
		if (IoCManager.Instance == null)
		{
			return list;
		}
		foreach (Type registeredType in IoCManager.Instance.GetRegisteredTypes())
		{
			string fullName = registeredType.FullName;
			if (!string.IsNullOrWhiteSpace(fullName))
			{
				string text = _m17038c921b89(fullName);
				if (text != null && !_m9306a10f12fe(list, text))
				{
					list.Add(text);
				}
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private string _m93513b8e574b()
	{
		string text;
		try
		{
			text = _fd5d515fa2fd2.GetCVar<string>(CVars.BuildVersion);
		}
		catch
		{
			text = null;
		}
		return _m17038c921b89(text ?? "unknown") ?? "unknown";
	}

	private string? _m17038c921b89(string moduleName)
	{
		string text = moduleName.Trim();
		if (text.Length == 0)
		{
			return null;
		}
		return _m21e044cb28f9(text, _f2a3fad06af11);
	}

	private static string _m21e044cb28f9(string value, int maxLength)
	{
		if (value.Length <= maxLength)
		{
			return value;
		}
		if (maxLength <= 8)
		{
			int length = value.Length;
			int num = length - maxLength;
			return value.Substring(num, length - num);
		}
		int num2 = Math.Max(8, Math.Min(48, (maxLength - "...".Length) / 2));
		int num3 = maxLength - "...".Length - num2;
		if (num3 < 4)
		{
			num3 = Math.Max(4, (maxLength - "...".Length) / 2);
			num2 = Math.Max(1, maxLength - "...".Length - num3);
		}
		return string.Concat(value.AsSpan(0, num3), "...", value.AsSpan(value.Length - num2, num2));
	}

	private static bool _m6c7f99cca0a9(string moduleName)
	{
		if (!moduleName.Contains("dynamic", StringComparison.OrdinalIgnoreCase) && !moduleName.Contains("in memory", StringComparison.OrdinalIgnoreCase) && !moduleName.Contains("anonymously hosted", StringComparison.OrdinalIgnoreCase) && !moduleName.Contains("reflection.emit", StringComparison.OrdinalIgnoreCase))
		{
			return moduleName.Contains("runtimegenerated", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private bool _m6630ed729cc3(string typeName)
	{
		for (int i = 0; i < _f3915e8d7abe1.Length; i++)
		{
			if (typeName.StartsWith(_f3915e8d7abe1[i], StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private static bool _mad358e34fcba(string value)
	{
		for (int i = 0; i < _fa692b02d069c.Length; i++)
		{
			if (value.Contains(_fa692b02d069c[i], StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private static bool _mb80787549be2(string value)
	{
		for (int i = 0; i < _f5c3e883080cf.Length; i++)
		{
			if (value.Contains(_f5c3e883080cf[i], StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private void _me36d689c5b47(List<string> markers, string marker)
	{
		string text = _mf7e47c8d44b5(marker);
		if (text != null && !_m9306a10f12fe(markers, text))
		{
			markers.Add(text);
		}
	}

	private static bool _m9306a10f12fe(List<string> items, string value)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (string.Equals(items[i], value, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private static bool _mce06cb7f4481(IReadOnlyList<string> items, string value)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (string.Equals(items[i], value, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private static void _m81d4032760f5(List<string> values)
	{
		values.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
	}

	private List<string> _m94f8bff530f6()
	{
		List<string> list = new List<string>();
		try
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (Assembly assembly in _fe5fec0d033c0.Assemblies)
			{
				string text = _m9c6e1eb3db8d(assembly);
				if (text != null)
				{
					hashSet.Add(text);
				}
			}
			int num = 0;
			HashSet<string> hashSet2 = new HashSet<string>();
			List<string> list2 = new List<string>();
			HashSet<string> hashSet3 = new HashSet<string>();
			foreach (Type item2 in _fe5fec0d033c0.FindAllTypes())
			{
				if ((object)item2 == null)
				{
					continue;
				}
				string text2;
				try
				{
					text2 = item2.Assembly.GetName().Name ?? "unknown";
				}
				catch
				{
					continue;
				}
				if (hashSet.Contains(text2))
				{
					continue;
				}
				num++;
				hashSet2.Add(text2);
				if (list2.Count < 16)
				{
					string text3 = item2.FullName ?? item2.Name;
					string text4 = _mac2ec4779a96(text3) ?? text3;
					string item = text2 + "::" + text4;
					if (hashSet3.Add(item))
					{
						list2.Add(item);
					}
				}
			}
			_m81d4032760f5(list2);
			list.Add($"origin-orphans={num}");
			list.Add($"origin-orphan-asms={hashSet2.Count}");
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add("orphan:" + list2[i]);
			}
		}
		catch
		{
		}
		return list;
	}

	private List<string> _m3c930ed2104c()
	{
		List<string> list = new List<string>();
		try
		{
			Dictionary<string, Assembly> dictionary = new Dictionary<string, Assembly>();
			foreach (Assembly assembly in _fe5fec0d033c0.Assemblies)
			{
				string text = _m9c6e1eb3db8d(assembly);
				if (text != null && !dictionary.ContainsKey(text))
				{
					dictionary[text] = assembly;
				}
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < _faa7440d10ced.Length; i++)
			{
				string text2 = _faa7440d10ced[i];
				if (!dictionary.TryGetValue(text2, out var value))
				{
					num3++;
					continue;
				}
				Type[] types;
				try
				{
					types = value.GetTypes();
				}
				catch
				{
					continue;
				}
				num++;
				HashSet<string> hashSet = new HashSet<string>();
				Type[] array = types;
				foreach (Type type in array)
				{
					if ((object)type != null)
					{
						string a;
						try
						{
							a = type.Assembly.GetName().Name ?? "unknown";
						}
						catch
						{
							continue;
						}
						if (!string.Equals(a, text2, StringComparison.Ordinal))
						{
							num2++;
						}
						string fullName = type.FullName;
						if (fullName != null)
						{
							hashSet.Add(fullName);
						}
					}
				}
				if (!_ma569915ee8de(text2, hashSet))
				{
					num3++;
				}
			}
			list.Add($"identity-checked={num}");
			list.Add($"identity-violations={num2}");
			list.Add($"identity-sentinel-miss={num3}");
		}
		catch
		{
		}
		return list;
	}

	private static bool _ma569915ee8de(string originName, HashSet<string> presentFullNames)
	{
		for (int i = 0; i < _f985e4e500ee7.Length; i++)
		{
			string text = _f985e4e500ee7[i];
			int num = text.IndexOf("::", StringComparison.Ordinal);
			if (num > 0 && string.Equals(text.Substring(0, num), originName, StringComparison.Ordinal))
			{
				string text2 = text;
				int num2 = num + 2;
				string item = text2.Substring(num2, text2.Length - num2);
				return presentFullNames.Contains(item);
			}
		}
		return true;
	}

	private List<string> _mc4cf1e9839d6()
	{
		List<string> list = new List<string>();
		try
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (Assembly assembly in _fe5fec0d033c0.Assemblies)
			{
				string text = _m9c6e1eb3db8d(assembly);
				if (text != null)
				{
					hashSet.Add(text);
				}
			}
			foreach (Assembly loadedModule in _f790dcf8ed182.LoadedModules)
			{
				string text2 = _m9c6e1eb3db8d(loadedModule);
				if (text2 != null)
				{
					hashSet.Add(text2);
				}
			}
			int orphanCount = 0;
			List<string> list2 = new List<string>();
			HashSet<string> sampleSeen = new HashSet<string>();
			_m88360b0e1aee(_fc6f45fbf437d.GetEntitySystemTypes(), "sys", hashSet, list2, sampleSeen, ref orphanCount);
			_m88360b0e1aee(_fc6f45fbf437d.DependencyCollection.GetRegisteredTypes(), "ioc", hashSet, list2, sampleSeen, ref orphanCount);
			_m88360b0e1aee(base.EntityManager.ComponentFactory.AllRegisteredTypes, "comp", hashSet, list2, sampleSeen, ref orphanCount);
			_m81d4032760f5(list2);
			list.Add($"registry-orphans={orphanCount}");
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add("reg-orphan:" + list2[i]);
			}
		}
		catch
		{
		}
		return list;
	}

	private static void _m88360b0e1aee(IEnumerable<Type> types, string channel, HashSet<string> modules, List<string> samples, HashSet<string> sampleSeen, ref int orphanCount)
	{
		foreach (Type type in types)
		{
			if ((object)type == null)
			{
				continue;
			}
			string text;
			try
			{
				text = type.Assembly.GetName().Name ?? "unknown";
			}
			catch
			{
				continue;
			}
			if (modules.Contains(text))
			{
				continue;
			}
			orphanCount++;
			if (samples.Count < 16)
			{
				string text2 = type.FullName ?? type.Name;
				string value = _mac2ec4779a96(text2) ?? text2;
				string item = $"{channel}:{text}::{value}";
				if (sampleSeen.Add(item))
				{
					samples.Add(item);
				}
			}
		}
	}

	private List<string> _m184152995743()
	{
		List<string> list = new List<string>();
		try
		{
			for (int i = 0; i < 512; i++)
			{
				Type.GetType("System.Int32");
			}
			long num = long.MaxValue;
			for (int j = 0; j < 6; j++)
			{
				long timestamp = Stopwatch.GetTimestamp();
				for (int k = 0; k < 4096; k++)
				{
					Type.GetType("System.Int32");
				}
				long num2 = Stopwatch.GetTimestamp() - timestamp;
				if (num2 < num)
				{
					num = num2;
				}
			}
			long frequency = Stopwatch.Frequency;
			int value = 0;
			if (num > 0 && frequency > 0 && (double)num * 1000000000.0 / (double)frequency / 4096.0 >= 4000.0)
			{
				value = 1;
			}
			list.Add($"resolver-bucket={value}");
		}
		catch
		{
		}
		return list;
	}

	private static string? _m9c6e1eb3db8d(Assembly assembly)
	{
		try
		{
			return assembly.GetName().Name;
		}
		catch
		{
			return null;
		}
	}

	private List<string> _m7532b8722c70()
	{
		List<string> list = new List<string>();
		try
		{
			foreach (var (text2, val2) in _fa860cc85cf5f.AvailableCommands)
			{
				if (string.IsNullOrWhiteSpace(text2))
				{
					continue;
				}
				Type type = ((object)val2)?.GetType();
				if ((object)type != null)
				{
					string value = type.FullName ?? type.Name;
					string value2 = _ma9a5b04aeec3(type);
					string value3 = _m17038c921b89(text2) ?? text2;
					string text3 = $"{value3}::{value2}::{value}";
					if (!_m9306a10f12fe(list, text3))
					{
						list.Add(text3);
					}
				}
			}
		}
		catch
		{
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private List<string> _md04b57f7e5c7()
	{
		List<string> list = new List<string>();
		try
		{
			foreach (Overlay allOverlay in _fad8fd92ff30f.AllOverlays)
			{
				if (allOverlay != null)
				{
					Type type = ((object)allOverlay).GetType();
					string text = type.FullName ?? type.Name;
					string text2 = _ma9a5b04aeec3(type) + "::" + text;
					if (!_m9306a10f12fe(list, text2))
					{
						list.Add(text2);
					}
				}
			}
		}
		catch
		{
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private List<string> _m4d82a8566713()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		try
		{
			for (int i = 0; i < _f54b67dcd0ec9.Length; i++)
			{
				string text = _f54b67dcd0ec9[i];
				IEnumerable<ResPath> enumerable;
				try
				{
					enumerable = _fdb0a9219e17c.ContentFindFiles(text);
				}
				catch
				{
					continue;
				}
				foreach (ResPath item in enumerable)
				{
					ResPath current = item;
					string filename = ((ResPath)(ref current)).Filename;
					if (string.IsNullOrWhiteSpace(filename))
					{
						continue;
					}
					string value = ((ResPath)(ref current)).Extension ?? string.Empty;
					string text2 = _mac2ec4779a96(filename);
					if (text2 != null)
					{
						string text3 = $"{text}::{value}::{text2}";
						if (!_m9306a10f12fe(list, text3))
						{
							list.Add(text3);
						}
					}
				}
			}
		}
		catch
		{
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private List<string> _mca7e8320c192()
	{
		List<string> list = new List<string>();
		try
		{
			int num = 0;
			foreach (Assembly assembly in _fe5fec0d033c0.Assemblies)
			{
				_ = assembly;
				num++;
			}
			int num2 = 0;
			foreach (Assembly loadedModule in _f790dcf8ed182.LoadedModules)
			{
				_ = loadedModule;
				num2++;
			}
			int num3 = 0;
			foreach (Type registeredType in _fc6f45fbf437d.DependencyCollection.GetRegisteredTypes())
			{
				_ = registeredType;
				num3++;
			}
			int num4 = 0;
			foreach (Type allRegisteredType in base.EntityManager.ComponentFactory.AllRegisteredTypes)
			{
				_ = allRegisteredType;
				num4++;
			}
			int num5 = 0;
			foreach (Type entitySystemType in _fc6f45fbf437d.GetEntitySystemTypes())
			{
				_ = entitySystemType;
				num5++;
			}
			int num6 = 0;
			foreach (KeyValuePair<string, IConsoleCommand> availableCommand in _fa860cc85cf5f.AvailableCommands)
			{
				_ = availableCommand;
				num6++;
			}
			int num7 = 0;
			foreach (Overlay allOverlay in _fad8fd92ff30f.AllOverlays)
			{
				_ = allOverlay;
				num7++;
			}
			list.Add($"reflection-assemblies={num}");
			list.Add($"modloader-assemblies={num2}");
			list.Add($"ioc-types={num3}");
			list.Add($"component-types={num4}");
			list.Add($"entitysystem-types={num5}");
			list.Add($"console-commands={num6}");
			list.Add($"overlays={num7}");
		}
		catch
		{
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private List<string> _md411f5064d3b()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < _f0d9d36cc60ae.Length; i++)
		{
			if (_m83d249542a76(_f0d9d36cc60ae[i], out Type type) && (object)type != null && type.FullName != null)
			{
				try
				{
					_m4ac62cb5631f(list, type);
				}
				catch
				{
				}
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private static void _m4ac62cb5631f(List<string> values, Type type)
	{
		string text = type.FullName ?? type.Name;
		string text2 = _ma9a5b04aeec3(type) + "::" + text;
		string text3 = (type.IsAbstract ? "a" : "-");
		text3 += (type.IsSealed ? "s" : "-");
		text3 += (type.IsClass ? "c" : "-");
		Type baseType = type.BaseType;
		string value = (((object)baseType != null) ? (baseType.FullName ?? baseType.Name) : "none");
		values.Add($"{text2}::shape::{text3}::attr={(int)type.Attributes}::base={value}");
		List<string> list = new List<string>();
		Type[] interfaces = type.GetInterfaces();
		foreach (Type type2 in interfaces)
		{
			string item = type2.FullName ?? type2.Name;
			if (!list.Contains(item))
			{
				list.Add(item);
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		for (int num = 0; num < list.Count; num++)
		{
			values.Add(text2 + "::iface::" + list[num]);
		}
		List<string> list2 = new List<string>();
		HashSet<string> hashSet = new HashSet<string>();
		MethodInfo[] methods = type.GetMethods();
		for (int i = 0; i < methods.Length; i++)
		{
			string name = methods[i].Name;
			if (!string.IsNullOrEmpty(name) && hashSet.Add(name))
			{
				list2.Add(name);
			}
		}
		list2.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		for (int num2 = 0; num2 < list2.Count; num2++)
		{
			values.Add(text2 + "::m::" + list2[num2]);
		}
	}

	private static string _ma9a5b04aeec3(Type type)
	{
		try
		{
			return type.Assembly.GetName().Name ?? "unknown";
		}
		catch
		{
			return "unknown";
		}
	}

	internal void _me13d53b6c217(RMCWeaponProfileCatalogRequestEvent ev)
	{
		if (_fd9f7ec1c3f19 && ev.Nonce == _fbb40e5d74392)
		{
			_m182579b7c6ca(ev.Token, ev.MaxChunkBytes, ev.ChunkStepSeconds);
		}
	}

	internal void _m38f49e42b882()
	{
		if (_f17f6a4e3c845 != null)
		{
			if (!_fd9f7ec1c3f19 || _f17f6a4e3c845.Nonce != _fbb40e5d74392)
			{
				_maf9f0d3098e1();
			}
			else if (!(_f15383f3913b9.CurTime < _f17f6a4e3c845.NextAt))
			{
				_m33d70edd0d8c();
			}
		}
	}

	private void _m182579b7c6ca(int token, int maxChunkBytes, float stepSeconds)
	{
		try
		{
			string s = _m484bab6dcff7();
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			int num = Math.Clamp(maxChunkBytes, 4096, 32768);
			int chunkCount = Math.Max(1, (bytes.Length + num - 1) / num);
			float stepSeconds2 = Math.Clamp(stepSeconds, 0.05f, 2f);
			_f17f6a4e3c845 = new _ta9bd54d5e117
			{
				Nonce = _fbb40e5d74392,
				Token = token,
				Payload = bytes,
				ChunkSize = num,
				ChunkCount = chunkCount,
				StepSeconds = stepSeconds2,
				NextAt = _f15383f3913b9.CurTime
			};
			_m33d70edd0d8c();
		}
		catch
		{
			_maf9f0d3098e1();
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCWeaponProfileCatalogChunkEvent(_fbb40e5d74392, token, 0, 0, success: false, Array.Empty<byte>()));
		}
	}

	private void _m33d70edd0d8c()
	{
		if (_f17f6a4e3c845 == null)
		{
			return;
		}
		try
		{
			_ta9bd54d5e117 f17f6a4e3c = _f17f6a4e3c845;
			int num = f17f6a4e3c.NextChunk * f17f6a4e3c.ChunkSize;
			int num2 = Math.Min(f17f6a4e3c.ChunkSize, f17f6a4e3c.Payload.Length - num);
			byte[] array = new byte[num2];
			Array.Copy(f17f6a4e3c.Payload, num, array, 0, num2);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCWeaponProfileCatalogChunkEvent(f17f6a4e3c.Nonce, f17f6a4e3c.Token, f17f6a4e3c.NextChunk, f17f6a4e3c.ChunkCount, success: true, array));
			f17f6a4e3c.NextChunk++;
			if (f17f6a4e3c.NextChunk >= f17f6a4e3c.ChunkCount)
			{
				_maf9f0d3098e1();
			}
			else
			{
				f17f6a4e3c.NextAt = _f15383f3913b9.CurTime + TimeSpan.FromSeconds(f17f6a4e3c.StepSeconds);
			}
		}
		catch
		{
			_ta9bd54d5e117 f17f6a4e3c2 = _f17f6a4e3c845;
			_maf9f0d3098e1();
			if (f17f6a4e3c2 != null)
			{
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCWeaponProfileCatalogChunkEvent(f17f6a4e3c2.Nonce, f17f6a4e3c2.Token, 0, 0, success: false, Array.Empty<byte>()));
			}
		}
	}

	private void _maf9f0d3098e1()
	{
		_f17f6a4e3c845 = null;
	}

	private string _m484bab6dcff7()
	{
		StringBuilder stringBuilder = new StringBuilder(196608);
		stringBuilder.AppendLine("profile-catalog-version: 2");
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder3 = stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(17, 1, stringBuilder2);
		handler.AppendLiteral("captured-at-utc: ");
		handler.AppendFormatted(DateTime.UtcNow, "O");
		stringBuilder3.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder4 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(15, 1, stringBuilder2);
		handler.AppendLiteral("build-version: ");
		handler.AppendFormatted(_m93513b8e574b());
		stringBuilder4.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder5 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder2);
		handler.AppendLiteral("burst-seed: ");
		handler.AppendFormatted(_fbb40e5d74392);
		stringBuilder5.AppendLine(ref handler);
		stringBuilder.AppendLine("sandbox-safe: true");
		stringBuilder.AppendLine("native-scan: unavailable");
		stringBuilder.AppendLine("private-reflection: unavailable");
		stringBuilder.AppendLine();
		_m46787f1aef99(stringBuilder, "assemblies.resources", _m8349c8d89b58());
		_m46787f1aef99(stringBuilder, "assemblies.reflection", _m0ba26a5dc294(_fe5fec0d033c0.Assemblies));
		_m46787f1aef99(stringBuilder, "assemblies.modloader", _m0ba26a5dc294(_f790dcf8ed182.LoadedModules));
		_m46787f1aef99(stringBuilder, "systems.runtime", _mdfeca939925a());
		_m46787f1aef99(stringBuilder, "systems.dependency", _mb6791d056437());
		_m46787f1aef99(stringBuilder, "ioc.registered", _m04cb0f6b9745());
		_m46787f1aef99(stringBuilder, "types.discoverable", _m8d4e4634c121(_fe5fec0d033c0.FindAllTypes()));
		_m46787f1aef99(stringBuilder, "types.signature-probes", _m100c946575bf());
		_m46787f1aef99(stringBuilder, "overlays.surface", _md04b57f7e5c7());
		return stringBuilder.ToString();
	}

	private static void _m46787f1aef99(StringBuilder builder, string section, IReadOnlyList<string> values)
	{
		builder.Append('[');
		builder.Append(section);
		builder.AppendLine("]");
		if (values.Count == 0)
		{
			builder.AppendLine("-");
			builder.AppendLine();
			return;
		}
		for (int i = 0; i < values.Count; i++)
		{
			builder.AppendLine(values[i]);
		}
		builder.AppendLine();
	}

	private static List<string> _m0ba26a5dc294(IEnumerable<Assembly> assemblies)
	{
		List<string> list = new List<string>();
		foreach (Assembly assembly in assemblies)
		{
			try
			{
				string text = assembly.GetName().Name ?? "unknown";
				string text2 = assembly.FullName ?? text;
				string text3 = text + " | full=" + text2;
				if (!_m9306a10f12fe(list, text3))
				{
					list.Add(text3);
				}
			}
			catch
			{
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private static List<string> _m8d4e4634c121(IEnumerable<Type> types)
	{
		List<string> list = new List<string>();
		foreach (Type type in types)
		{
			if (!string.IsNullOrWhiteSpace(type.FullName))
			{
				string text;
				try
				{
					text = type.Assembly.GetName().Name ?? "unknown";
				}
				catch
				{
					text = "unknown";
				}
				string text2 = text + "::" + type.FullName;
				if (!_m9306a10f12fe(list, text2))
				{
					list.Add(text2);
				}
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private List<string> _m100c946575bf()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < _fa2ea7b4dd80d.Length; i++)
		{
			if (_m83d249542a76(_fa2ea7b4dd80d[i].TypeName, out Type type) && (object)type != null && type.FullName != null)
			{
				string value = type.Assembly.GetName().Name ?? "unknown";
				list.Add($"{_fa2ea7b4dd80d[i].Marker}::{value}::{type.FullName}");
			}
		}
		for (int j = 0; j < _f0eccc63a6b39.Length; j++)
		{
			if (_m83d249542a76(_f0eccc63a6b39[j].TypeName, out Type type2) && (object)type2 != null && type2.FullName != null)
			{
				string value2 = type2.Assembly.GetName().Name ?? "unknown";
				string text = $"{_f0eccc63a6b39[j].Marker}::{value2}::{type2.FullName}";
				if (!_m9306a10f12fe(list, text))
				{
					list.Add(text);
				}
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private List<string> _m24b291836d04(IReadOnlyList<string> sideMarkers)
	{
		List<string> list = new List<string>(sideMarkers.Count);
		for (int i = 0; i < sideMarkers.Count; i++)
		{
			list.Add(_me8876d792424(sideMarkers[i]));
		}
		return list;
	}

	private string _me8876d792424(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return '~'.ToString();
		}
		char[] array = new char[value.Length + 1];
		array[0] = '~';
		int seed = _mfaec1ce6466d(_fbb40e5d74392, value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			array[i + 1] = _m2d6278b036aa(value[i], i, seed, encode: true);
		}
		return new string(array);
	}

	private string? _mf7e47c8d44b5(string marker)
	{
		string text = marker.Trim();
		if (text.Length == 0)
		{
			return null;
		}
		int maxLength = Math.Max(1, _f2a3fad06af11 - 1);
		return _m21e044cb28f9(text, maxLength);
	}

	private static int _mfaec1ce6466d(int nonce, int length)
	{
		int num = nonce ^ (int)(length * 2654435769u);
		int num2 = (num ^ (num >>> 16)) * 2146121005;
		int num3 = (num2 ^ (num2 >>> 15)) * -2073254261;
		return num3 ^ (num3 >>> 16);
	}

	private static char _m2d6278b036aa(char value, int index, int seed, bool encode)
	{
		if (value < ' ' || value > '~')
		{
			return value;
		}
		int num = (seed >> (index & 3) * 8) & 0xFF;
		num = (num + index * 17 + 11) % 95;
		int num2 = value - 32;
		int num3 = (encode ? ((num2 + num) % 95) : ((num2 - num + 190) % 95));
		return (char)(32 + num3);
	}

	internal void _m4ec44a315a26(RMCWeaponProfileIntegrityRequestEvent ev)
	{
		if (_fd9f7ec1c3f19 && ev.Nonce == _fbb40e5d74392)
		{
			_mb6cb749334d2(ev.Token, ev.ChallengeSalt, ev.ProbeIds);
		}
	}

	private void _mb6cb749334d2(int token, int challengeSalt, IReadOnlyList<byte> opaqueProbeIds)
	{
		try
		{
			List<byte> list = new List<byte>(opaqueProbeIds.Count);
			List<string> list2 = new List<string>(opaqueProbeIds.Count);
			Dictionary<byte, byte> dictionary = RMCWeaponProfileProbeCatalog.BuildLogicalProbeMap(_m93513b8e574b());
			for (int i = 0; i < opaqueProbeIds.Count; i++)
			{
				byte b = opaqueProbeIds[i];
				if (dictionary.TryGetValue(b, out var value) && _m156539230b32(value, challengeSalt, out string digest))
				{
					list.Add(b);
					list2.Add(digest);
				}
			}
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCWeaponProfileIntegrityResponseEvent(_fbb40e5d74392, token, list.Count == opaqueProbeIds.Count, list, list2, _fccb3e75c4e17));
		}
		catch
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCWeaponProfileIntegrityResponseEvent(_fbb40e5d74392, token, success: false, new List<byte>(), new List<string>()));
		}
	}

	private bool _m156539230b32(byte logicalProbeId, int challengeSalt, out string digest)
	{
		if (_m1b9f8ca1b9cc(logicalProbeId, challengeSalt, out digest))
		{
			return true;
		}
		if (_md52e0452be71(logicalProbeId, challengeSalt, out digest))
		{
			return true;
		}
		return _m20af58ec8580(logicalProbeId, challengeSalt, out digest);
	}

	private bool _m1b9f8ca1b9cc(byte logicalProbeId, int challengeSalt, out string digest)
	{
		digest = string.Empty;
		if (logicalProbeId == 40 || !_m1f6cc241a08b(logicalProbeId, out string digest2))
		{
			return false;
		}
		byte[] array = RMCWeaponProfileDigest.ParseHex(digest2);
		if (array.Length == 0)
		{
			return false;
		}
		RMCWeaponProfileDigest.ApplyChallengeMask(array, challengeSalt);
		digest = RMCWeaponProfileDigest.ToHexLower(array);
		return true;
	}

	private bool _md52e0452be71(byte logicalProbeId, int challengeSalt, out string digest)
	{
		digest = string.Empty;
		IReadOnlyList<string> readOnlyList = logicalProbeId switch
		{
			20 => _mdfeca939925a(), 
			21 => _m04cb0f6b9745(), 
			22 => _m8d4e4634c121(_fe5fec0d033c0.FindAllTypes()), 
			23 => _m100c946575bf(), 
			24 => _m0ba26a5dc294(_f790dcf8ed182.LoadedModules), 
			25 => _m0ba26a5dc294(_fe5fec0d033c0.Assemblies), 
			26 => _mb6791d056437(), 
			27 => _mdfeca939925a(), 
			28 => _m04cb0f6b9745(), 
			29 => _m8d4e4634c121(_fe5fec0d033c0.FindAllTypes()), 
			30 => _m0ba26a5dc294(_f790dcf8ed182.LoadedModules), 
			31 => _m0ba26a5dc294(_fe5fec0d033c0.Assemblies), 
			50 => _m7532b8722c70(), 
			51 => _md04b57f7e5c7(), 
			52 => _m4d82a8566713(), 
			53 => _mca7e8320c192(), 
			54 => _md411f5064d3b(), 
			55 => _md411f5064d3b(), 
			56 => _m94f8bff530f6(), 
			57 => _m3c930ed2104c(), 
			58 => _mc4cf1e9839d6(), 
			59 => _m184152995743(), 
			_ => null, 
		};
		if (readOnlyList == null)
		{
			return false;
		}
		digest = _mff6119bc2538(readOnlyList, challengeSalt, _m2e9b9092124c(logicalProbeId));
		return true;
	}

	private bool _m20af58ec8580(byte logicalProbeId, int challengeSalt, out string digest)
	{
		digest = string.Empty;
		IReadOnlyList<string> readOnlyList = logicalProbeId switch
		{
			40 => _m8129573df706(), 
			45 => _m8129573df706(), 
			46 => _me0ec0de3687b(), 
			47 => _mb7e2e6618a7b(), 
			48 => _mdd6debd0071a(), 
			49 => _m09204b64e531(), 
			_ => null, 
		};
		if (readOnlyList == null)
		{
			return false;
		}
		digest = _mff6119bc2538(readOnlyList, challengeSalt, _m2e9b9092124c(logicalProbeId));
		return true;
	}

	private bool _m1f6cc241a08b(byte logicalProbeId, out string digest)
	{
		digest = string.Empty;
		if (!_me70514751618() || _f7e86bd84e5bc == null || !_f7e86bd84e5bc.TryGetValue(logicalProbeId, out string value))
		{
			return false;
		}
		digest = value;
		return true;
	}

	private bool _me70514751618()
	{
		if (_fc859d8c9da48)
		{
			if (_f7e86bd84e5bc != null)
			{
				return _f7e86bd84e5bc.Count > 0;
			}
			return false;
		}
		_fc859d8c9da48 = true;
		try
		{
			string text = _m8d27f4e1d855();
			if (text.Length == 0)
			{
				return false;
			}
			Dictionary<byte, string> dictionary = new Dictionary<byte, string>();
			string[] array = text.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			foreach (string text2 in array)
			{
				if (text2.Length == 0 || text2[0] == '#')
				{
					continue;
				}
				if (text2.StartsWith("build-version:", StringComparison.OrdinalIgnoreCase))
				{
					string text3 = text2;
					int length = "build-version:".Length;
					_fa834100c3212 = text3.Substring(length, text3.Length - length).Trim();
				}
				else
				{
					if (!text2.StartsWith("probe:", StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					int num = text2.IndexOf('=');
					if (num > "probe:".Length && byte.TryParse(text2.AsSpan("probe:".Length, num - "probe:".Length).Trim(), out var result))
					{
						string text3 = text2;
						int length = num + 1;
						string text4 = text3.Substring(length, text3.Length - length).Trim();
						if (text4.Length != 0)
						{
							dictionary[result] = text4.ToLowerInvariant();
						}
					}
				}
			}
			if (dictionary.Count == 0)
			{
				return false;
			}
			_f7e86bd84e5bc = dictionary;
			return true;
		}
		catch
		{
			_f7e86bd84e5bc = null;
			_f44edb66ba5d3 = null;
			_fa834100c3212 = string.Empty;
			return false;
		}
	}

	private string _m8d27f4e1d855()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		StringBuilder stringBuilder = new StringBuilder();
		IReadOnlyList<string> packagedManifestFragmentPaths = RMCWeaponProfileProbeCatalog.GetPackagedManifestFragmentPaths(_m93513b8e574b());
		for (int i = 0; i < packagedManifestFragmentPaths.Count; i++)
		{
			string text = packagedManifestFragmentPaths[i];
			if (!_fdb0a9219e17c.ContentFileExists(text))
			{
				continue;
			}
			string text2 = _fdb0a9219e17c.ContentFileReadAllText(text);
			if (string.IsNullOrWhiteSpace(text2))
			{
				continue;
			}
			dictionary[text] = text2;
			if (stringBuilder.Length > 0)
			{
				if (stringBuilder[stringBuilder.Length - 1] != '\n')
				{
					stringBuilder.Append('\n');
				}
			}
			stringBuilder.Append(text2);
			if (text2[text2.Length - 1] != '\n')
			{
				stringBuilder.Append('\n');
			}
		}
		_f44edb66ba5d3 = dictionary;
		return stringBuilder.ToString();
	}

	private List<string> _m8129573df706()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (ResPath item in _fdb0a9219e17c.ContentFindFiles("/Assemblies"))
		{
			ResPath current = item;
			if (string.Equals(((ResPath)(ref current)).Extension, "dll", StringComparison.OrdinalIgnoreCase))
			{
				string text = _mac2ec4779a96(((ResPath)(ref current)).FilenameWithoutExtension);
				if (text != null && hashSet.Add(text))
				{
					list.Add(text);
				}
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private List<string> _me0ec0de3687b()
	{
		List<string> list = new List<string>();
		if (!_me70514751618() || _f7e86bd84e5bc == null)
		{
			return list;
		}
		if (!string.IsNullOrWhiteSpace(_fa834100c3212))
		{
			list.Add("build:" + _fa834100c3212);
		}
		List<byte> list2 = new List<byte>(_f7e86bd84e5bc.Keys);
		list2.Sort();
		for (int i = 0; i < list2.Count; i++)
		{
			byte b = list2[i];
			if (_f7e86bd84e5bc.TryGetValue(b, out string value))
			{
				list.Add($"probe:{b}:{value}");
			}
		}
		return list;
	}

	private List<string> _mb7e2e6618a7b()
	{
		List<string> list = new List<string>();
		if (!_me70514751618() || _f44edb66ba5d3 == null)
		{
			return list;
		}
		List<string> list2 = new List<string>(_f44edb66ba5d3.Keys);
		list2.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		for (int num = 0; num < list2.Count; num++)
		{
			string text = _mac2ec4779a96(list2[num]);
			if (text != null)
			{
				list.Add(text);
			}
		}
		return list;
	}

	private List<string> _mdd6debd0071a()
	{
		List<string> list = new List<string>();
		if (!_me70514751618() || _f44edb66ba5d3 == null)
		{
			return list;
		}
		List<string> list2 = new List<string>(_f44edb66ba5d3.Keys);
		list2.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		for (int num = 0; num < list2.Count; num++)
		{
			string text = list2[num];
			if (_f44edb66ba5d3.TryGetValue(text, out string value))
			{
				string text2 = _m54accce96bb0("fragment", text, value);
				if (text2 != null)
				{
					list.Add(text2);
				}
			}
		}
		return list;
	}

	private List<string> _m09204b64e531()
	{
		List<string> list = new List<string>();
		IReadOnlyList<string> integrityBenignResourceProbePaths = RMCWeaponProfileProbeCatalog.GetIntegrityBenignResourceProbePaths();
		for (int i = 0; i < integrityBenignResourceProbePaths.Count; i++)
		{
			string text = integrityBenignResourceProbePaths[i];
			if (!_fdb0a9219e17c.ContentFileExists(text))
			{
				continue;
			}
			string text2 = _fdb0a9219e17c.ContentFileReadAllText(text);
			if (!string.IsNullOrWhiteSpace(text2))
			{
				string text3 = _m54accce96bb0("resource", text, text2);
				if (text3 != null)
				{
					list.Add(text3);
				}
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private static string? _m54accce96bb0(string prefix, string path, string raw)
	{
		string text = _mac2ec4779a96(path);
		string text2 = _m40f2207c9d91(raw);
		if (text == null || text2 == null)
		{
			return null;
		}
		string value = RMCWeaponProfileDigest.ToHexLower(RMCWeaponProfileDigest.ComputeDigest(text2));
		return $"{prefix}:{text}:{value}";
	}

	private static string? _m40f2207c9d91(string? raw)
	{
		if (string.IsNullOrWhiteSpace(raw))
		{
			return null;
		}
		string text = raw.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Trim();
		if (text.Length != 0)
		{
			return _mb8f3d40b990b(text, 16777216);
		}
		return null;
	}

	private static string _mff6119bc2538(IEnumerable<string> values, int challengeSalt, byte variantId = 0)
	{
		List<string> list = new List<string>();
		foreach (string value in values)
		{
			string text = _mac2ec4779a96(value);
			if (text != null)
			{
				list.Add(text);
			}
		}
		if (list.Count == 0)
		{
			return _m6631a6e21eaf(Encoding.UTF8.GetBytes("count=0\n"), challengeSalt);
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (variantId == 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				string text2 = list[i];
				stringBuilder.Append(text2.Length);
				stringBuilder.Append(':');
				stringBuilder.Append(text2);
				stringBuilder.Append('\n');
			}
			stringBuilder.Insert(0, $"count={list.Count}\n");
			return _m6631a6e21eaf(Encoding.UTF8.GetBytes(stringBuilder.ToString()), challengeSalt);
		}
		list.Sort(delegate(string left, string right)
		{
			int num3 = right.Length.CompareTo(left.Length);
			return (num3 == 0) ? string.Compare(left, right, StringComparison.OrdinalIgnoreCase) : num3;
		});
		int num = 0;
		stringBuilder.Append("variant=");
		stringBuilder.Append(variantId);
		stringBuilder.Append('\n');
		stringBuilder.Append("count=");
		stringBuilder.Append(list.Count);
		stringBuilder.Append('\n');
		for (int num2 = 0; num2 < list.Count; num2++)
		{
			string text3 = list[num2];
			num += text3.Length;
			stringBuilder.Append(num2);
			stringBuilder.Append('#');
			stringBuilder.Append(text3.Length);
			stringBuilder.Append('#');
			stringBuilder.Append(text3);
			stringBuilder.Append('\n');
		}
		stringBuilder.Append("total=");
		stringBuilder.Append(num);
		stringBuilder.Append('\n');
		return _m6631a6e21eaf(Encoding.UTF8.GetBytes(stringBuilder.ToString()), challengeSalt);
	}

	private static string _m6631a6e21eaf(ReadOnlySpan<byte> rawBytes, int challengeSalt)
	{
		byte[] array = RMCWeaponProfileDigest.ComputeDigest(rawBytes);
		RMCWeaponProfileDigest.ApplyChallengeMask(array, challengeSalt);
		return RMCWeaponProfileDigest.ToHexLower(array);
	}

	private static string? _mac2ec4779a96(string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}
		return _mb8f3d40b990b(value.Trim(), 512);
	}

	private static string _mb8f3d40b990b(string value, int maxLength)
	{
		if (value.Length <= maxLength)
		{
			return value;
		}
		if (maxLength <= 8)
		{
			int length = value.Length;
			int num = length - maxLength;
			return value.Substring(num, length - num);
		}
		int num2 = Math.Max(24, Math.Min(96, (maxLength - "...".Length) / 2));
		int num3 = Math.Max(24, maxLength - "...".Length - num2);
		if (num3 + "...".Length + num2 > maxLength)
		{
			num2 = Math.Max(8, maxLength - "...".Length - num3);
		}
		return string.Concat(value.AsSpan(0, num3), "...", value.AsSpan(value.Length - num2));
	}

	private static byte _m2e9b9092124c(byte logicalProbeId)
	{
		switch (logicalProbeId)
		{
		case 27:
		case 28:
		case 29:
		case 30:
		case 31:
		case 45:
		case 46:
		case 55:
			return 1;
		case 47:
		case 48:
			return 2;
		case 49:
			return 3;
		case 50:
		case 51:
		case 52:
		case 53:
			return 4;
		default:
			return 0;
		}
	}

	private static string _mefd73cb8f295(string marker)
	{
		return _f0cafd36a2603 + marker;
	}

	private static string _m283721b78071(int visibleCount, int rawCount)
	{
		return _f01621c7de67e + visibleCount + "->" + rawCount;
	}

	private static string _ma405f988c325(string moduleName)
	{
		return _f6b9f656785e8 + moduleName;
	}

	private static string _m46efcd1d0fa6(int visibleCount, int rawCount)
	{
		return _fdcfcd8ecb6ed + visibleCount + "->" + rawCount;
	}

	private static string _m2cb986304ff7(string moduleName)
	{
		return _f134aa1494e45 + moduleName;
	}

	private static string _mfbc6cf3db6f2(string assemblyName)
	{
		return _f52288cec5de7 + assemblyName;
	}

	private static string _m9679932c6538(string root)
	{
		return _f3d50cb28f418 + root;
	}

	private static string _m6f913f040366(string typeName)
	{
		return _f79c17cc6a4c3 + typeName;
	}

	private static string _meb83f3788501(string root)
	{
		return _f27d4cfc16b94 + root;
	}

	private static string _mae4dbb10f5ce(string typeName)
	{
		return _f368a646b8c1a + typeName;
	}

	private static string _m1ded9fb8a317(string cvar)
	{
		return _fbabb65f960db + cvar;
	}

	private static string _m58fbe73f1311(string typeName)
	{
		return _f71a9fdde23b9 + typeName;
	}

	private static string _m0ac020f69fd1(string typeName)
	{
		return _fcceb100712a8 + typeName;
	}

	private static string _m80d572865755(string typeName)
	{
		return _fb0f65a50afd6 + typeName;
	}

	private static string _m1bde7db0783d(int total)
	{
		return _fcaf4ea4d7451 + total;
	}

	private static string _mad61b1654a92(int publicCount, int rawCount)
	{
		return _f1cc544d8e686 + publicCount + "->" + rawCount;
	}

	private static string _m07020afdc1be(int runtimeCount, int dependencyCount)
	{
		return _f0f74a5646064 + runtimeCount + "->" + dependencyCount;
	}

	private static string _me1ece98ebcd2(string typeName)
	{
		return _feee0fded8d16 + typeName;
	}

	private static string _m26a812a5e361(string command)
	{
		return _f38f7086e3c44 + command;
	}

	private static string _m87f6dddff8d8(string command)
	{
		return _fc999dec354a0 + command;
	}

	private void _md1b14b21cf2e(HashSet<int> target, IReadOnlyList<int> values)
	{
		target.Clear();
		for (int i = 0; i < values.Count; i++)
		{
			if (values[i] != 0)
			{
				target.Add(values[i]);
			}
		}
	}

	private bool _ma8b82ec93f4d(string value, HashSet<int> target)
	{
		if (target.Count == 0 || string.IsNullOrWhiteSpace(value))
		{
			return false;
		}
		return target.Contains(RMCWeaponProfileRuleHash.Compute(value, _f33e87ad587f1));
	}

	private static string[] _m54d84b860464(string payload)
	{
		return _mb7a2373f4842(payload).Split('\n', StringSplitOptions.RemoveEmptyEntries);
	}

	private static (string Marker, string TypeName)[] _m17a46415d76f(string payload)
	{
		string[] array = _m54d84b860464(payload);
		(string, string)[] array2 = new(string, string)[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			int num = array[i].IndexOf('|');
			if (num > 0 && num < array[i].Length - 1)
			{
				int num2 = i;
				string item = array[i].Substring(0, num);
				string text = array[i];
				int num3 = num + 1;
				array2[num2] = (item, text.Substring(num3, text.Length - num3));
			}
		}
		return array2;
	}

	private static byte[] _mf849c854c067()
	{
		byte[] sourceArray = RMCWeaponProfileDigest.ComputeDigest(Encoding.UTF8.GetBytes("RMC14_LEX_v3_2026"));
		byte[] array = new byte[16];
		Array.Copy(sourceArray, array, 16);
		return array;
	}

	private static string _mb7a2373f4842(string payload)
	{
		byte[] array = Convert.FromBase64String(payload);
		byte[] f839c2b9a635d = _f839c2b9a635d;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] ^= f839c2b9a635d[i % 16];
		}
		return Encoding.UTF8.GetString(array);
	}

	private void _mcfe0264c8fc9(List<string> markers, IReadOnlyList<string> managedModules)
	{
		if (_f15383f3913b9.CurTime >= _fffff5ff627ac || _fa19b40661606.Count == 0)
		{
			_fa19b40661606.Clear();
			_mdde49dc55acc(_fa19b40661606, managedModules);
			_mf483a09210ff(_fa19b40661606);
			_m3ca11ea8b54a(_fa19b40661606);
			_md581ac1bd2c7(_fa19b40661606);
			float num = Math.Clamp(_f2ae21401ded7 * 6f, 30f, 180f);
			_fffff5ff627ac = _f15383f3913b9.CurTime + TimeSpan.FromSeconds(num);
		}
		for (int i = 0; i < _fa19b40661606.Count; i++)
		{
			_me36d689c5b47(markers, _fa19b40661606[i]);
		}
	}

	private void _mdde49dc55acc(List<string> markers, IReadOnlyList<string> managedModules)
	{
		List<string> list = _mb9ab4fb781a2();
		List<string> list2 = _mbac54ec30db7();
		if (list2.Count == 0)
		{
			return;
		}
		if (list.Count != list2.Count)
		{
			_me36d689c5b47(markers, _m283721b78071(list.Count, list2.Count));
		}
		int num = 0;
		for (int i = 0; i < list2.Count; i++)
		{
			string text = list2[i];
			if (!_m9306a10f12fe(list, text) && _mb80787549be2(text))
			{
				_me36d689c5b47(markers, _ma405f988c325(text));
				num++;
				if (num >= 8)
				{
					break;
				}
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			string text2 = list[j];
			if (!_m9306a10f12fe(list2, text2) && _mb80787549be2(text2))
			{
				_me36d689c5b47(markers, _ma405f988c325(text2));
				num++;
				if (num >= 8)
				{
					break;
				}
			}
		}
	}

	private void _mf483a09210ff(List<string> markers)
	{
		List<string> list = _m8b3e0c889176();
		List<string> list2 = _mc98657ee42aa();
		if (list2.Count == 0 && list.Count == 0)
		{
			return;
		}
		if (list.Count != list2.Count && (list.Count > 0 || list2.Count > 0))
		{
			_me36d689c5b47(markers, _m46efcd1d0fa6(list.Count, list2.Count));
		}
		int num = 0;
		for (int i = 0; i < list2.Count; i++)
		{
			string text = list2[i];
			if (!_m9306a10f12fe(list, text) && _mb80787549be2(text))
			{
				_me36d689c5b47(markers, _m2cb986304ff7(text));
				num++;
				if (num >= 8)
				{
					break;
				}
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			string text2 = list[j];
			if (!_m9306a10f12fe(list2, text2) && _mb80787549be2(text2))
			{
				_me36d689c5b47(markers, _m2cb986304ff7(text2));
				num++;
				if (num >= 8)
				{
					break;
				}
			}
		}
		if (_m9306a10f12fe(list, _fc13d14519f9e) && !_m83d249542a76(_fef9519d147e1, out Type type))
		{
			_me36d689c5b47(markers, _f59e318fb9bf6);
		}
		else if (_m83d249542a76(_fef9519d147e1, out type) && !_m9306a10f12fe(list, _fc13d14519f9e) && !_m9306a10f12fe(list2, _fc13d14519f9e))
		{
			_me36d689c5b47(markers, _f59e318fb9bf6);
		}
	}

	private void _md581ac1bd2c7(List<string> markers)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		_me71f4a41897f(list, _fe5fec0d033c0.Assemblies, includeTrustedRoots: true);
		_me71f4a41897f(list2, _f790dcf8ed182.LoadedModules, includeTrustedRoots: true);
		list3 = _m8349c8d89b58();
		List<string> list4 = new List<string>();
		List<string> list5 = new List<string>();
		List<string> list6 = new List<string>();
		List<string> list7 = new List<string>();
		List<string> list8 = new List<string>();
		foreach (string item in list)
		{
			if (!_m9306a10f12fe(list2, item) && !_m9306a10f12fe(list3, item) && _mb80787549be2(item))
			{
				list4.Add(item);
			}
		}
		foreach (string item2 in list2)
		{
			if (!_m9306a10f12fe(list, item2) && !_m9306a10f12fe(list3, item2) && _mb80787549be2(item2))
			{
				list5.Add(item2);
			}
		}
		foreach (string item3 in list3)
		{
			if (!_m9306a10f12fe(list, item3) && !_m9306a10f12fe(list2, item3) && _mb80787549be2(item3))
			{
				list6.Add(item3);
			}
		}
		foreach (string item4 in list2)
		{
			if (!_m9306a10f12fe(list, item4) && _m9306a10f12fe(list3, item4))
			{
				list7.Add(item4);
			}
		}
		foreach (string item5 in list)
		{
			if (!_m9306a10f12fe(list2, item5) && _m9306a10f12fe(list3, item5))
			{
				list8.Add(item5);
			}
		}
		for (int i = 0; i < Math.Min(3, list4.Count); i++)
		{
			_me36d689c5b47(markers, "msv:reflect-only:" + list4[i]);
		}
		for (int j = 0; j < Math.Min(3, list5.Count); j++)
		{
			_me36d689c5b47(markers, "msv:loader-only:" + list5[j]);
		}
		for (int k = 0; k < Math.Min(3, list6.Count); k++)
		{
			_me36d689c5b47(markers, "msv:resource-only:" + list6[k]);
		}
		for (int l = 0; l < Math.Min(3, list7.Count); l++)
		{
			_me36d689c5b47(markers, "msv:reflect-miss:" + list7[l]);
		}
		for (int m = 0; m < Math.Min(3, list8.Count); m++)
		{
			_me36d689c5b47(markers, "msv:loader-miss:" + list8[m]);
		}
		int num = list4.Count + list5.Count + list6.Count + list7.Count + list8.Count;
		if (num > 0)
		{
			_me36d689c5b47(markers, $"msv:drift-total:{num}");
		}
		List<string> items = _mdfeca939925a();
		List<string> list9 = _mb6791d056437();
		List<string> list10 = new List<string>();
		IDependencyCollection instance = IoCManager.Instance;
		foreach (Type item6 in ((instance != null) ? instance.GetRegisteredTypes() : null) ?? Array.Empty<Type>())
		{
			if (typeof(EntitySystem).IsAssignableFrom(item6))
			{
				string fullName = item6.FullName;
				if (!string.IsNullOrWhiteSpace(fullName))
				{
					list10.Add(fullName);
				}
			}
		}
		int num2 = 0;
		foreach (string item7 in list9)
		{
			if (!_m9306a10f12fe(items, item7))
			{
				num2++;
			}
		}
		foreach (string item8 in list10)
		{
			if (!_m9306a10f12fe(items, item8))
			{
				num2++;
			}
		}
		if (num2 > 0)
		{
			_me36d689c5b47(markers, $"msv:sys-drift:{num2}");
		}
		_med105c1731c4(markers, list, list2);
	}

	private void _med105c1731c4(List<string> markers, List<string> reflectionModules, List<string> modLoaderModules)
	{
		int num = 0;
		int num2 = 0;
		foreach (string reflectionModule in reflectionModules)
		{
			if (reflectionModule.StartsWith("Content.", StringComparison.OrdinalIgnoreCase) || reflectionModule.StartsWith("Robust.", StringComparison.OrdinalIgnoreCase))
			{
				num++;
			}
		}
		foreach (string modLoaderModule in modLoaderModules)
		{
			if (modLoaderModule.StartsWith("Content.", StringComparison.OrdinalIgnoreCase) || modLoaderModule.StartsWith("Robust.", StringComparison.OrdinalIgnoreCase))
			{
				num2++;
			}
		}
		if (Math.Abs(num - num2) > 2)
		{
			_me36d689c5b47(markers, $"msv:count-gap:{num}->{num2}");
		}
	}

	private void _m3ca11ea8b54a(List<string> markers)
	{
		List<string> list = new List<string>();
		_me106091e579d(list, _mb9ab4fb781a2());
		_me106091e579d(list, _mbac54ec30db7());
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		List<string> list4 = new List<string>();
		List<string> list5 = new List<string>();
		List<string> list6 = new List<string>();
		try
		{
			foreach (Type item in _fe5fec0d033c0.FindAllTypes())
			{
				string fullName = item.FullName;
				if (string.IsNullOrWhiteSpace(fullName))
				{
					continue;
				}
				string text = _m17038c921b89(fullName);
				if (text == null)
				{
					continue;
				}
				string text2;
				try
				{
					text2 = _m17038c921b89(item.Assembly.GetName().Name ?? "unknown");
				}
				catch
				{
					text2 = null;
				}
				if (_m4f19a97bd79e(text, text2))
				{
					continue;
				}
				string text3 = _m6b857fc06ba1(text);
				string text4 = _m7c41758240c3(text);
				if (text4 == null)
				{
					continue;
				}
				bool flag = _m245ce8244562(text2, list);
				bool flag2 = _mce235f4303db(text, text2, flag);
				if ((!flag || text3 != null) && !(flag2 && flag))
				{
					if (text2 != null && !flag)
					{
						_m813f57f49351(list2, text2, 4);
					}
					if (text3 != null && !flag2)
					{
						_m813f57f49351(list3, text3, 6);
					}
					string value = ((text2 == null) ? text4 : (text2 + "::" + text4));
					_m813f57f49351(list4, value, 8);
					if (text3 != null && _ma8b82ec93f4d(text3, _f0a4133b5c029))
					{
						_m813f57f49351(list5, text3, 4);
					}
					if (text3 != null && _ma8b82ec93f4d(text4, _f2793be76c6cd))
					{
						_m813f57f49351(list6, text3 + "." + text4, 8);
					}
				}
			}
		}
		catch
		{
		}
		for (int i = 0; i < list2.Count; i++)
		{
			_me36d689c5b47(markers, _mfbc6cf3db6f2(list2[i]));
		}
		for (int j = 0; j < list3.Count; j++)
		{
			_me36d689c5b47(markers, _m9679932c6538(list3[j]));
		}
		for (int k = 0; k < list4.Count; k++)
		{
			_me36d689c5b47(markers, _m6f913f040366(list4[k]));
		}
		for (int l = 0; l < list5.Count; l++)
		{
			_me36d689c5b47(markers, _meb83f3788501(list5[l]));
		}
		for (int m = 0; m < list6.Count; m++)
		{
			_me36d689c5b47(markers, _mae4dbb10f5ce(list6[m]));
		}
	}

	private static void _me106091e579d(List<string> baseline, IReadOnlyList<string> values)
	{
		for (int i = 0; i < values.Count; i++)
		{
			if (!_m9306a10f12fe(baseline, values[i]))
			{
				baseline.Add(values[i]);
			}
		}
	}

	private static bool _m021eb2c08475(string value, IReadOnlyList<string> prefixes)
	{
		for (int i = 0; i < prefixes.Count; i++)
		{
			if (value.StartsWith(prefixes[i], StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private static bool _m245ce8244562(string? assemblyName, IReadOnlyList<string> allowedAssemblies)
	{
		if (assemblyName == null)
		{
			return false;
		}
		if (!_mce06cb7f4481(allowedAssemblies, assemblyName))
		{
			return _m021eb2c08475(assemblyName, _fdecff0bc9991);
		}
		return true;
	}

	private static bool _mce235f4303db(string fullName, string? assemblyName, bool trustedAssembly)
	{
		if (_m021eb2c08475(fullName, _feec691af0c47))
		{
			return true;
		}
		if (!trustedAssembly || string.IsNullOrWhiteSpace(assemblyName))
		{
			return false;
		}
		if (string.Equals(fullName, assemblyName, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return fullName.StartsWith(assemblyName + ".", StringComparison.OrdinalIgnoreCase);
	}

	private static string? _m6b857fc06ba1(string fullName)
	{
		int num = fullName.IndexOf('.');
		if (num <= 0)
		{
			return null;
		}
		return fullName.Substring(0, num).Trim();
	}

	private static string? _m7c41758240c3(string fullName)
	{
		string text = fullName;
		int num = text.LastIndexOf('.');
		if (num >= 0 && num < text.Length - 1)
		{
			string text2 = text;
			int num2 = num + 1;
			text = text2.Substring(num2, text2.Length - num2);
		}
		int num3 = text.IndexOf('+');
		if (num3 > 0)
		{
			text = text.Substring(0, num3);
		}
		int num4 = text.IndexOf('`');
		if (num4 > 0)
		{
			text = text.Substring(0, num4);
		}
		text = text.Trim();
		if (text.Length != 0)
		{
			return text;
		}
		return null;
	}

	private static bool _m4f19a97bd79e(string fullName, string? assemblyName)
	{
		if (assemblyName != null && string.Equals(assemblyName, "CompiledRobustXaml", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		string text = _m7c41758240c3(fullName);
		if (text == null)
		{
			return false;
		}
		if (!text.StartsWith('<') && !text.Contains("AnonymousType", StringComparison.OrdinalIgnoreCase) && !text.Contains("InlineArray", StringComparison.OrdinalIgnoreCase) && !text.Contains("ReadOnlyArray", StringComparison.OrdinalIgnoreCase) && !text.Contains("ReadOnlySingleElementList", StringComparison.OrdinalIgnoreCase) && !text.Contains("PrivateImplementationDetails", StringComparison.OrdinalIgnoreCase))
		{
			return text.StartsWith("__StaticArrayInitTypeSize", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private static void _m813f57f49351(List<string> values, string value, int limit)
	{
		if (!_m9306a10f12fe(values, value))
		{
			values.Add(value);
			if (values.Count > limit)
			{
				values.RemoveRange(limit, values.Count - limit);
			}
		}
	}

	internal bool _m49a1ad791f32(out RMCDrawSkewSlice91dac4 challenge)
	{
		if (!_fc597ad70fd5a.HasValue)
		{
			challenge = default(RMCDrawSkewSlice91dac4);
			return false;
		}
		challenge = _fc597ad70fd5a.Value;
		return true;
	}

	private void _ma279e968b42b(byte grid, byte cellX, byte cellY, byte sizePercent, byte red, byte green, byte blue)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_fc597ad70fd5a = new RMCDrawSkewSlice91dac4(grid, cellX, cellY, sizePercent, new Color(red, green, blue, byte.MaxValue));
	}

	private void _mff5024a79308()
	{
		_fc597ad70fd5a = null;
	}

	private async Task _mc966ddcbce9f(bool armed)
	{
		if (armed)
		{
			await Task.Delay(40);
		}
	}

	private bool _m2f80ae0722ad()
	{
		return false;
	}

	private void _m09b7b0aefabe(List<string> markers)
	{
		for (int i = 0; i < _f0eccc63a6b39.Length; i++)
		{
			if (_m83d249542a76(_f0eccc63a6b39[i].TypeName, out Type _))
			{
				_me36d689c5b47(markers, _f0eccc63a6b39[i].Marker);
			}
		}
	}

	private List<string> _mb9ab4fb781a2()
	{
		List<string> list = new List<string>();
		_me71f4a41897f(list, _fe5fec0d033c0.Assemblies, includeTrustedRoots: true);
		_me71f4a41897f(list, _f790dcf8ed182.LoadedModules, includeTrustedRoots: true);
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private List<string> _m8349c8d89b58()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		foreach (ResPath item in _fdb0a9219e17c.ContentFindFiles("/Assemblies"))
		{
			ResPath current = item;
			if (!string.Equals(((ResPath)(ref current)).Extension, "dll", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			string text = _m17038c921b89(((ResPath)(ref current)).FilenameWithoutExtension);
			if (text == null)
			{
				continue;
			}
			bool flag = false;
			for (int i = 0; i < _f8dd17bd80062.Length; i++)
			{
				if (text.StartsWith(_f8dd17bd80062[i], StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			if ((flag || _mb80787549be2(text)) && !_m9306a10f12fe(list, text))
			{
				list.Add(text);
			}
		}
		if (list.Count == 0)
		{
			_me71f4a41897f(list, _f790dcf8ed182.LoadedModules, includeTrustedRoots: true);
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private List<string> _m19e860529a63()
	{
		return _m8349c8d89b58();
	}

	private List<string> _m520f7c37c97d()
	{
		return _me5d6bc10f7e8();
	}

	private List<string> _mb6791d056437()
	{
		return _me5d6bc10f7e8();
	}

	private List<string> _me5d6bc10f7e8()
	{
		List<string> list = new List<string>();
		object obj = default(object);
		foreach (Type registeredType in _fc6f45fbf437d.DependencyCollection.GetRegisteredTypes())
		{
			if (typeof(EntitySystem).IsAssignableFrom(registeredType) && _fc6f45fbf437d.TryGetEntitySystem(registeredType, ref obj) && !(obj?.GetType() != registeredType))
			{
				_m29ef9bfc87eb(list, new Type[1] { registeredType });
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private bool _m83d249542a76(string typeSpecification, out Type? type)
	{
		type = null;
		if (string.IsNullOrWhiteSpace(typeSpecification))
		{
			return false;
		}
		try
		{
			type = Type.GetType(typeSpecification, throwOnError: false);
			if (type != null)
			{
				return true;
			}
		}
		catch
		{
		}
		string text = typeSpecification.Split(',', 2, StringSplitOptions.TrimEntries)[0];
		try
		{
			if (_fe5fec0d033c0.TryLooseGetType(text, ref type))
			{
				return true;
			}
		}
		catch
		{
		}
		type = null;
		return false;
	}

	private void _m23e52b444b79(List<string> markers)
	{
		try
		{
			int num = 0;
			int num2 = 0;
			foreach (Overlay allOverlay in _fad8fd92ff30f.AllOverlays)
			{
				num++;
				string typeName = ((object)allOverlay).GetType().FullName ?? string.Empty;
				if (!_m6630ed729cc3(typeName))
				{
					num2++;
				}
			}
			if (num2 > 0)
			{
				_me36d689c5b47(markers, $"overlay:custom-count:{num2}");
			}
			if (num > 100)
			{
				_me36d689c5b47(markers, $"overlay:total-count:{num}");
			}
		}
		catch
		{
		}
	}

	private void _m6ee162e1c432(List<string> markers)
	{
		string[] array = _m5630bf7d8487();
		for (int i = 0; i < array.Length; i++)
		{
			if (_m83d249542a76(array[i], out Type _))
			{
				_me36d689c5b47(markers, "decoy:system-loaded:" + _m96129808ba9a(array[i]));
			}
		}
	}

	private static string[] _m5630bf7d8487()
	{
		return new string[24]
		{
			"Content.Client.Cheats.NoRecoilSystem", "Content.Client.Cheats.AimbotSystem", "Content.Client.Cheats.EspSystem", "Content.Client.Cheats.SpeedHackSystem", "Content.Client.Cheats.WallHackSystem", "Content.Client.Cheats.GodModeSystem", "Content.Client.Cheats.NoClipSystem", "Content.Client.Cheats.InfiniteHealthSystem", "Content.Client.Cheats.RadarSystem", "Content.Client.Hacks.TargetLockSystem",
			"Content.Client.Hacks.AutoAimSystem", "Content.Client.Hacks.InfiniteAmmoSystem", "Content.Client.Hacks.EntityHiderSystem", "Content.Client.Hacks.AssemblyHiderSystem", "Content.Client.Hacks.TypeHiderSystem", "Content.Client.Hacks.ModuleHiderSystem", "Content.Client.Mods.CheatMenuSystem", "Content.Client.Mods.HackLoaderSystem", "Content.Client.Mods.InjectorSystem", "Content.Client.Mods.PatcherSystem",
			"Content.Client.External.MarseySystem", "Content.Client.External.CypherSystem", "Content.Client.External.CerberusSystem", "Content.Client.External.TealSystem"
		};
	}

	private static string _m96129808ba9a(string fullName)
	{
		int num = fullName.LastIndexOf('.');
		if (num < 0 || num >= fullName.Length - 1)
		{
			return fullName;
		}
		int num2 = num + 1;
		return fullName.Substring(num2, fullName.Length - num2);
	}

	private void _m7a6ea419c68e(List<string> markers)
	{
		try
		{
			int num = 0;
			foreach (Type allRegisteredType in base.EntityManager.ComponentFactory.AllRegisteredTypes)
			{
				string text = allRegisteredType.FullName ?? string.Empty;
				if (!_m6630ed729cc3(text))
				{
					num++;
					if (num <= 3)
					{
						_me36d689c5b47(markers, "component:custom:" + _m96129808ba9a(text));
					}
				}
			}
			if (num > 3)
			{
				_me36d689c5b47(markers, $"component:custom-total:{num}");
			}
		}
		catch
		{
		}
	}

	private void _mc24568923027(List<string> markers)
	{
		try
		{
			int num = 0;
			foreach (KeyValuePair<string, IConsoleCommand> availableCommand in _fa860cc85cf5f.AvailableCommands)
			{
				Type type = ((object)availableCommand.Value)?.GetType();
				if (type == null)
				{
					continue;
				}
				string typeName = type.FullName ?? string.Empty;
				if (!_m6630ed729cc3(typeName))
				{
					num++;
					if (num <= 3)
					{
						_me36d689c5b47(markers, "cmd:custom:" + availableCommand.Key);
					}
				}
			}
			if (num > 3)
			{
				_me36d689c5b47(markers, $"cmd:custom-total:{num}");
			}
		}
		catch
		{
		}
	}

	private void _m5cc61e511ec2(List<string> markers)
	{
	}

	private void _m30be2fecc038(List<string> markers)
	{
		if (_f9a741dded701.Count == 0)
		{
			return;
		}
		try
		{
			foreach (string item in _f9a741dded701)
			{
				if (_fa860cc85cf5f.AvailableCommands.ContainsKey(item))
				{
					_me36d689c5b47(markers, "probe:dynamic-cmd:" + item);
				}
			}
		}
		catch
		{
		}
	}

	internal void _m28fae0c6bf27(List<string> commands)
	{
		_f9a741dded701.Clear();
		foreach (string command in commands)
		{
			if (!string.IsNullOrWhiteSpace(command))
			{
				_f9a741dded701.Add(command);
			}
		}
	}

	private List<string> _mbac54ec30db7()
	{
		List<string> list = new List<string>();
		foreach (string item in _m8349c8d89b58())
		{
			if (!_m9306a10f12fe(list, item))
			{
				list.Add(item);
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private List<string> _mdfeca939925a()
	{
		List<string> list = new List<string>();
		foreach (Type entitySystemType in _fc6f45fbf437d.GetEntitySystemTypes())
		{
			string fullName = entitySystemType.FullName;
			if (!string.IsNullOrWhiteSpace(fullName))
			{
				string text = _m17038c921b89(fullName);
				if (text != null && !_m9306a10f12fe(list, text))
				{
					list.Add(text);
				}
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private List<string> _m6b0762212f15()
	{
		return _m520f7c37c97d();
	}

	private List<string> _m8b3e0c889176()
	{
		List<string> list = new List<string>();
		foreach (Assembly loadedModule in _f790dcf8ed182.LoadedModules)
		{
			string name;
			try
			{
				name = loadedModule.GetName().Name;
			}
			catch
			{
				continue;
			}
			if (!string.IsNullOrWhiteSpace(name))
			{
				string text = _m17038c921b89(name);
				if (text != null && !_m9306a10f12fe(list, text))
				{
					list.Add(text);
				}
			}
		}
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private List<string> _mc98657ee42aa()
	{
		List<string> list = _m19e860529a63();
		list.Sort((string left, string right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
		return list;
	}

	private void _me71f4a41897f(List<string> modules, IEnumerable<Assembly> assemblies, bool includeTrustedRoots)
	{
		foreach (Assembly assembly in assemblies)
		{
			string name;
			try
			{
				name = assembly.GetName().Name;
			}
			catch
			{
				continue;
			}
			if (!string.IsNullOrWhiteSpace(name))
			{
				string text = _m17038c921b89(name);
				if (text != null && _mfd91750bcf7c(text, includeTrustedRoots) && !_m9306a10f12fe(modules, text))
				{
					modules.Add(text);
				}
			}
		}
	}

	private void _m29ef9bfc87eb(List<string> types, IEnumerable<Type> values)
	{
		foreach (Type value in values)
		{
			string fullName = value.FullName;
			if (!string.IsNullOrWhiteSpace(fullName))
			{
				string text = _m17038c921b89(fullName);
				if (text != null && !_m9306a10f12fe(types, text))
				{
					types.Add(text);
				}
			}
		}
	}

	private bool _mfd91750bcf7c(string name, bool includeTrustedRoots)
	{
		if (includeTrustedRoots && (name.StartsWith("Content.", StringComparison.OrdinalIgnoreCase) || name.StartsWith("Robust.", StringComparison.OrdinalIgnoreCase)))
		{
			return true;
		}
		return _mb80787549be2(name);
	}

	private void _m33432bf71416(List<string> markers)
	{
		for (int i = 0; i < _fa2ea7b4dd80d.Length; i++)
		{
			if (_m83d249542a76(_fa2ea7b4dd80d[i].TypeName, out Type _))
			{
				_me36d689c5b47(markers, _mefd73cb8f295(_fa2ea7b4dd80d[i].Marker));
			}
		}
	}
}
