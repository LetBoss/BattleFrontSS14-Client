// Decompiled with JetBrains decompiler
// Type: Content.Client.Rendering.Profiled08b.RMCProfileCacheNodea4fdbc
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Lobby;
using Content.Client._CIV14merka.ModeMenu;
using Content.Client._PUBG.Connection;
using Content.Client._PUBG.Lobby;
using Content.Client.Gameplay;
using Content.Client.Lobby;
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace Content.Client.Rendering.Profiled08b;

public sealed class RMCProfileCacheNodea4fdbc : EntitySystem
{
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
  private static readonly string[] _f3915e8d7abe1 = new string[6]
  {
    "Content.Client",
    "Content.Shared",
    "Robust.Client",
    "Robust.Shared",
    "System.",
    "Microsoft."
  };
  private bool _fd9f7ec1c3f19;
  private int _fbb40e5d74392;
  private float _f2ae21401ded7 = 10f;
  private int _f9f10ca116b15 = 256 /*0x0100*/;
  private int _f2a3fad06af11 = 96 /*0x60*/;
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
  private readonly Dictionary<EntityUid, Queue<RMCProfileCacheNodea4fdbc._t5e239d9f0f2c>> _f4eb3b35fc0eb = new Dictionary<EntityUid, Queue<RMCProfileCacheNodea4fdbc._t5e239d9f0f2c>>();
  private readonly Dictionary<EntityUid, Vector2> _fbf92d104791a = new Dictionary<EntityUid, Vector2>();
  private readonly Dictionary<EntityUid, RMCProfileCacheNodea4fdbc._tb1b24893add2> _fb79a4fcfbaa0 = new Dictionary<EntityUid, RMCProfileCacheNodea4fdbc._tb1b24893add2>();
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
  private static readonly string[] _faa7440d10ced = new string[4]
  {
    "Robust.Shared",
    "Robust.Client",
    "Content.Shared",
    "Content.Client"
  };
  private static readonly string[] _f985e4e500ee7 = new string[4]
  {
    "Robust.Shared::Robust.Shared.GameObjects.MetaDataComponent",
    "Robust.Client::Robust.Client.GameStates.ClientGameStateManager",
    "Content.Shared::Content.Shared._RMC14.Weapons.RMCWeaponProfileProbeCatalog",
    "Content.Client::Content.Client._RMC14.Weapons.RMCWeaponGun"
  };
  private const int _f832809668f7d = 16 /*0x10*/;
  private const int _fb231854b8188 = 4096 /*0x1000*/;
  private const int _fc52fdc505d49 = 6;
  private const double _f395b7b09edcb = 4000.0;
  private static readonly string[] _f54b67dcd0ec9 = new string[2]
  {
    "/Assemblies",
    "/EnginePatches"
  };
  private static readonly string[] _f0d9d36cc60ae = new string[11]
  {
    "Robust.Client.Graphics.IClyde",
    "Robust.Client.Graphics.IClydeViewport",
    "Robust.Client.Graphics.IOverlayManager",
    "Robust.Client.Graphics.Overlay",
    "Robust.Client.Graphics.ILightManager",
    "Robust.Client.Graphics.IEyeManager",
    "Robust.Shared.GameObjects.OccluderSystem",
    "Robust.Client.Graphics.OverlayManager",
    "Robust.Client.Graphics.LightManager",
    "Robust.Client.Graphics.EyeManager",
    "Robust.Client.Graphics.Clyde.Clyde"
  };
  private RMCProfileCacheNodea4fdbc._ta9bd54d5e117? _f17f6a4e3c845;
  private const char _fb93595ee2c5d = '~';
  private const int _f157abd3667ad = 32 /*0x20*/;
  private const int _fca0dbdafb46f = 126;
  private const int _fe1b28b0d91d8 = 95;
  private bool _fc859d8c9da48;
  private Dictionary<byte, string>? _f7e86bd84e5bc;
  private Dictionary<string, string>? _f44edb66ba5d3;
  private string _fa834100c3212 = string.Empty;
  private const string _f3d77f8cbc590 = "RMC14_LEX_v3_2026";
  private static readonly byte[] _f839c2b9a635d = RMCProfileCacheNodea4fdbc._mf849c854c067();
  private static readonly string[] _fa692b02d069c = RMCProfileCacheNodea4fdbc._m54d84b860464("xbouHlv+n8qI7966pUjDvcWwKHZa5ebDku/evrRByrHMsiAXPuP7xZ78oMenTMq00Nk3GVXmn8Ka7aeovS7coMalJg5A7+elmOakpaFWpbbBoSEZRv/mpZP+pqCrStbfybotFFvl/g==");
  private static readonly string[] _f5c3e883080cf = RMCProfileCacheNodea4fdbc._m54d84b860464("0LYiED7n9N2I+q3Hp13fvcGhSR9N+v3Kify4pKFK29/XpiEKUfjhyomVp6igTdu8y71JFFX4+MCV5t6grUrHusu4SRVZ7eDG8fy9oKNRxt/NviQJXeP435eVvKi8RaW2waEhGUb/5qWY+qavoVbaptOyMRk=");
  private static readonly (string Marker, string TypeName)[] _fa2ea7b4dd80d = RMCProfileCacheNodea4fdbc._m17a46415d76f("0LYiEBrv+9uJ5qSirUrbqfC2IhAaz/vbieaEoq1K2/mEhyYdWIDhyprz+qiqUN2s1LwqEkCk5d2U/bGxkEHOuYqWLQhG88XAkvGg4eRwyrTIgzETVu+f257+uOOrUsqnyLI6Ul3n8tqS44CopUiBmtK2MRBV87vmltihpItSyqfIsjpQFN7wzpeVoKilSIG60rYxEFXzu8aW+KGk6lTdusa2PyhR6/mBtOmxv6hF1vvtvgQJXcXjyonztbToBPuwxb8TDlvo8KWT/qagq0rWuc2xbRRV+PjAleb6uaFFw6nssjERW+Ts45L9+oWlVsK6yqpvXGDv9MPx97W/qUvBrMi6IVJc6+fClPGt47BBzrnUoSweUfbdzonyu6O9aMa3ipsiDlnl+9bXv4CopUj/p8uxJnZc7+3O1fGxuepNwrLRum0IUev507P6rKzqauqBipouO0Hju+aW2KGkj0HW+YSHJh1YgP3Kg/76o6FQgbzJtDYVGv7wzpfvpqKmQdOdwasiUnrPwYGy8pO4rQrmuOOmKjdR87mPr/q1oZRWwLfB2S4dRvnw1tXvtbmnTIGhwbIvDEbl98qH0rW/t0HWhcWnIBQYqsHKmvOEv6tGyt/JsjEPUfO7ypXrprTqUMq0yKMxE1bv6eKa7aeovWHBodaqb1xg7/TDq+27r6EuzKzUuyYOV+b8ypXr+qqxStu01rQmCEjJ7N+T+qaOqE3Ku9D9BRlV/uDdnuz6jK1J7brQ/QQJWqTS2pXLtb+jQduG3aA3GVmA9taL97G/p0jGsMqnbQ9E4/vNlOuojr1Ux7DWkC8VUeThgb36tbmxVsqmipIqEXbl4YG48Lmgq0qBhtS6LT5b/sbWiOuxoM5H1qXMtjEfWOPwwY+xoKy2Q8qh1rw3HUDj+sGH3K29rEHdlsi6JhJApNPKmuuhv6FXgZTNvgETQKTWwJbyu6PqcM6nw7Y3Llv+9NuS8LqevVfbsMnZIAVE4vDdmPO9qKpQgaXIsjoZRv7nzpj0sb+4Z9alzLYxP1jj8MGPsZKopVDap8GgbT1Q5/zBkuygv6VQxrrK/RMQVfPw3a/tta6vQd2G3aA3GVmA9taL97G/p0jGsMqnbQxG4/rdkuutsYdd373BoQAQXe/729XZsaywUd2w1/0CGFnj+8aI66assE3Au4qDMRVb+PzbgsytvrBBwt/HqjMUUfj2w5L6urnqQt28wb0nAHfz5cee7ZehrUHBoYqVJh1A/+fKiLGVqalNwbzXpzEdQOP6wdXZpqShSsuG3aA3GVmA9taL97G/p0jGsMqnbR1a/vzcmO2xqKpD3bTGrwAFROLw3bjzvaiqUIGFxacgFFH5u/+J8KCop1DGusr9AhJA48bMifqxo4NWzrf0sjcfXID21ov3sb+nSMawyqdtElv48MyU9rixh13fvcGhABBd7/vb1c+1uadMyqaKlDYSGsT6/Z78u6SodM6hx7tJH1H498qJ6qe6pVbKo5f9JAla/vTdnPqgsYdB3bfBoTYPY+vnyq2s+oqxSvu01rQmCGfz5tue8t6uoVbNsNamMAtV+PDZyLGnva1KzbrQrwAZRujw3Y7sg6y2QfnmioAzFVrI+tuo5qe5oUmltsGhIRlG/+bYmu2xu/cK27DXpzAfRu/wwZztta+4Z8qnxrYxCUfd9N2eyefjkEHcofewMRlR5NLdmv2XoqlJzrvA2SAZRujw3Y7so6y2QdnmirIwD1Hn98OC972poVbTlsGhIRlG/+b4mu2xm/cK7qbXti4eWPPdxp/6pp2lUMy9rrAmDlbv59qI6LW/oVKc+8G9NxVA8+bWiOuxoKxNy7DWrwAZRujw3Y7sg6y2QfnmipYtCF3+7PyC7KCoqWzGscGhEx1A6f2lmPqmr6FW2qbTsjEZQrm7wpT7uKKlQMqn1LI3H1z21sqJ/bG/sVf4tNa2FU8ax/rLt/C1qaFW/7TQsCt2V+/nzZ7tob6zRd2w0uBtDlHs+cqY672iqlTOoce7cQB37+fNnu2hvpNF3bDy4G0uUez5ypjrvaKqdM6hx7txdlfv582e7aG+s0XdsNLgbQhN+vDNl/C3pqFW05bBoSEZRv/m+JrtsZv3Cvus1LYBEFvp/sqJz7W5p0yltsGhIRlG/+bYmu2xu/cKzrvQujAfRu/wwZztta+4Z8qnxrYxCUfd9N2eyefjhUrbvPewMRlR5NLdmv2ErLBHx9/HtjEeUfjg3Iz+pqiyF4G7y6EmH1vj+dO4+qavoVbapvOyMRliubvhlM2xrqtNw4XFpyAUPunw3Zn6pri3U86nwaVwUlrl5sKU9LGxh0Hdt8GhNg9j6+fKraz6g6t3wrrPthMdQOn9");
  private static readonly (string Marker, string TypeName)[] _f0eccc63a6b39 = RMCProfileCacheNodea4fdbc._m17a46415d76f("zLwsFw7i9N2W8Lq0/hTHtNa+LBJN9t3OifK7o71oxreKmyIOWeX71te/5IWlVsK6yqpJFFvl/pWT/qagq0rW78yyMRFb5OzDkv2ohaVWwrrKqg8VVqTdzonyu6O9CI+dxaEuE1rz2caZlbyiq0+VutK2MRBV86/bnv64sZBBzrmKnDUZRub01tXWuYqxTeCjwaEvHU2mtfue/rjHrEvAvp68NRlG5vTWweuxrKhU3brGtj8oUev5gbTpsb+oRdb77b4ECV3F48qJ87W06AT7sMW/Ew5b6PClk/C7pv5BwaHWqnkIUev506/6taHqYcGh1qoTE13k4YPby7GsqC7Husu4eRla/ufWweuxrKhU3brGtj8oUev5gb7xoL+9dMC8yqdvXGDv9MOr7buvoQ==");
  private static readonly string[] _fbd13a37a5e1f = RMCProfileCacheNodea4fdbc._m54d84b860464("1KEsHlGw4daL+u7HtFbAs82/JkZV+ebKlv24tOlHwKDKp3l2RPj6yZLzsfelV9ywybEvBRnt9N/Blaaoo03codaqeRFb7uDDnrK3orFK2++uoSYbXfnh3YKluaKgUcOwibQiDA6A58qc9qe5tl2Vod2jJlFT6+WVk/6moKtK1t/UoSweUbDxxoj8u7uhVs63yLZuHUf58MKZ863go0Xf766jMRNW76/Lkuy3orJB3bTGvyZRRuX629b4tb3+Lt+ny7EmRlDj5syU6bG/pUbDsImnOgxRp/LOi6XevbZLzbCetyoPV+Xjyon+tqGhCd26y6duEVX+9sfBlaS/q0bK78C6MB9b/PDdmv24qOlQ1qXB/i4dQOn9lfH8oqy2HqWgzelJD035r6WX/q2isVCVpt2gNxlZp/LOi6XeoaVdwKDQ6TAFR6fhwI/+uPfOSM6sy6Y3Rkfz5tue8vmuq1HBoZ7ZLx1N5eDbweytvrBBwvjXvDYOV++4y4n2srn+Lsa6x+lJDEbl98rB8rW/t0HW+Me+J0Y++ufAmfruqaFHwKyJsC4YDvnw3Ij2u6POVN26xrZ5GFHp+tbW/KKsth7csNegKhNagOXdlP2x96dJy++u4wsdRuf6wYKVnKy2ScC73Z8qHhrC9N2W8Lq06ASfncWhLhNa8w==");
  private static readonly string _f0cafd36a2603 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[0];
  private static readonly string _f01621c7de67e = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[1];
  private static readonly string _f6b9f656785e8 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[2];
  private static readonly string _fdcfcd8ecb6ed = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[3];
  private static readonly string _f134aa1494e45 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[4];
  private static readonly string _f59e318fb9bf6 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[5];
  private static readonly string _f52288cec5de7 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[6];
  private static readonly string _f3d50cb28f418 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[7];
  private static readonly string _f79c17cc6a4c3 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[8];
  private static readonly string _f27d4cfc16b94 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[9];
  private static readonly string _f368a646b8c1a = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[10];
  private static readonly string _fbabb65f960db = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[11];
  private static readonly string _f71a9fdde23b9 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[12];
  private static readonly string _fcceb100712a8 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[13];
  private static readonly string _fb0f65a50afd6 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[14];
  private static readonly string _fcaf4ea4d7451 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[15];
  private static readonly string _f1cc544d8e686 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[16 /*0x10*/];
  private static readonly string _f0f74a5646064 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[17];
  private static readonly string _feee0fded8d16 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[18];
  private static readonly string _f38f7086e3c44 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[19];
  private static readonly string _feea1f9998df9 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[20];
  private static readonly string _f8dcebeb407cb = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[21];
  private static readonly string _fc999dec354a0 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[22];
  private static readonly string _fc13d14519f9e = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[23];
  private static readonly string _fef9519d147e1 = RMCProfileCacheNodea4fdbc._fbd13a37a5e1f[24];
  private static readonly string[] _feec691af0c47 = new string[14]
  {
    "CompiledRobustXaml.",
    "CompiledRobustXaml",
    "System.",
    "System",
    "Microsoft.",
    "Microsoft",
    "Avalonia.",
    "Avalonia",
    "Linguini.",
    "Linguini",
    "SixLabors.",
    "SixLabors",
    "SpaceWizards.",
    "SpaceWizards"
  };
  private static readonly string[] _fdecff0bc9991 = new string[7]
  {
    "CompiledRobustXaml",
    "System",
    "Microsoft",
    "Avalonia",
    "Linguini",
    "SixLabors",
    "SpaceWizards"
  };
  private static readonly string[] _f8dd17bd80062 = new string[2]
  {
    "Content.",
    "Robust."
  };
  private RMCWeaponBurstMap0629b2? _f1e406aff76a4;
  private RMCDrawSkewSlice91dac4? _fc597ad70fd5a;
  private readonly HashSet<string> _f9a741dded701 = new HashSet<string>();

  public virtual void Initialize()
  {
    base.Initialize();
    this._fe915e8fe2b5a = new RMCDrawAtlasStackb8076a(this);
    this._f6affd5d20ec6 = new RMCWeaponFuseSlice2fac48(this);
    this._f1e406aff76a4 = new RMCWeaponBurstMap0629b2(this);
    this._fad8fd92ff30f.AddOverlay((Overlay) this._fe915e8fe2b5a);
    this._fad8fd92ff30f.AddOverlay((Overlay) this._f6affd5d20ec6);
    this._fad8fd92ff30f.AddOverlay((Overlay) this._f1e406aff76a4);
  }

  public virtual void Shutdown()
  {
    if (this._fe915e8fe2b5a != null && this._fad8fd92ff30f.HasOverlay(((object) this._fe915e8fe2b5a).GetType()))
      this._fad8fd92ff30f.RemoveOverlay((Overlay) this._fe915e8fe2b5a);
    if (this._f6affd5d20ec6 != null && this._fad8fd92ff30f.HasOverlay(((object) this._f6affd5d20ec6).GetType()))
      this._fad8fd92ff30f.RemoveOverlay((Overlay) this._f6affd5d20ec6);
    if (this._f1e406aff76a4 != null && this._fad8fd92ff30f.HasOverlay(((object) this._f1e406aff76a4).GetType()))
      this._fad8fd92ff30f.RemoveOverlay((Overlay) this._f1e406aff76a4);
    this._m7902fc32fa5f();
    base.Shutdown();
  }

  internal void _mf1a84035361f(RMCWeaponProfileHelloEvent ev)
  {
    int num = this._fbb40e5d74392 != ev.Nonce ? 1 : 0;
    this._fd9f7ec1c3f19 = ev.Enabled;
    this._fbb40e5d74392 = ev.Nonce;
    this._f2ae21401ded7 = Math.Clamp(ev.HeartbeatIntervalSeconds, 3f, 60f);
    this._f9f10ca116b15 = Math.Clamp(ev.MaxModulesPerList, 16 /*0x10*/, 2048 /*0x0800*/);
    this._f2a3fad06af11 = Math.Clamp(ev.MaxModuleNameLength, 8, 256 /*0x0100*/);
    this._f0bdf165242f2 = Math.Clamp(ev.FocusDistanceThreshold, 1f, 256f);
    this._f37c8c2f2e4b0 = Math.Clamp(ev.MaxProfileFrameBytes, 65536 /*0x010000*/, 2000000);
    this._f786b93097a0d = this._m17038c921b89(ev.DecoyCommandName) ?? string.Empty;
    this._f4b2692c6081a = this._m17038c921b89(ev.DecoyCVarName) ?? string.Empty;
    this._f33e87ad587f1 = ev.RuleSalt;
    this._md1b14b21cf2e(this._f5f8aa038a944, (IReadOnlyList<int>) ev.StrictCommandRuleIds);
    this._md1b14b21cf2e(this._f5dd6e4fa1bd6, (IReadOnlyList<int>) ev.SuspiciousCommandRuleIds);
    this._md1b14b21cf2e(this._f0a4133b5c029, (IReadOnlyList<int>) ev.DiscoverableRootRuleIds);
    this._md1b14b21cf2e(this._f2793be76c6cd, (IReadOnlyList<int>) ev.DiscoverableTypeRuleIds);
    this._m28fae0c6bf27(ev.DynamicDecoyCommands);
    if (num != 0)
    {
      this._f35d9eaacfa87 = 0;
      this._fccb3e75c4e17 = 0U;
      this._f173000f7639c = false;
      this._f5b4fd9514d1d = 0;
      this._f8fc6b1e36a38 = 0;
      this._f347de9aaf490 = 0;
      this._maf9f0d3098e1();
    }
    this._fc7cf4cc09e9e = false;
    this._f89a31cf28d86 = TimeSpan.Zero;
    this._fffff5ff627ac = TimeSpan.Zero;
    this._f85d654b0e3c7 = TimeSpan.Zero;
    this._fa19b40661606.Clear();
    this._fc7cb89171f46.Clear();
    if (this._fd9f7ec1c3f19)
      return;
    this._maf9f0d3098e1();
  }

  internal void _m2670d1b35bd4(RMCWeaponProfilePulseRequestEvent ev)
  {
    if (!this._fd9f7ec1c3f19 || ev.Nonce != this._fbb40e5d74392)
      return;
    this._m33a5f3c69c90();
  }

  internal void _m9814afeff83e(float frameTime)
  {
    if (this._f12823a397d00)
      this._m558e53b13bd6();
    else
      this._f561470475e83 = 0.0f;
    if (!this._f12823a397d00 && !this._fb9e5c296f8da)
    {
      this._m7902fc32fa5f();
      this._ma57ce84f6bea();
    }
    bool flag;
    switch (this._f1fc2369db60b.CurrentState)
    {
      case LobbyState _:
      case QueueState _:
      case PubgPreLobbyHubState _:
      case CivLobbyState _:
      case ModeSelectState _:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    if (!flag)
      return;
    this._m38f49e42b882();
    if (!this._fd9f7ec1c3f19)
      return;
    this._m4a27a9896f5b();
    this._mbe325d7c950a();
  }

  internal void _m5e41c632b8aa(RMCWeaponDrawSkewEvent ev)
  {
    this._f12823a397d00 = ev.Enabled;
    this._ff9eed6291cdf = Math.Clamp(ev.ShiftTiles, 0.1f, 4f);
    this._f9526b89881f2 = Math.Clamp(ev.SwapIntervalSeconds, 0.05f, 5f);
    this._fb9e5c296f8da = ev.SyncLagEnabled;
    this._f10323b69a2fb = Math.Clamp(ev.SyncLagDelaySeconds, 0.05f, 3f);
    this._f541baa812565 = Math.Clamp(ev.SyncLagJitterSeconds, 0.0f, 2f);
    this._fea062e8e2c0f = Math.Clamp(ev.SyncLagMaxOffsetTiles, 0.1f, 8f);
    this._f561470475e83 = this._ff9eed6291cdf;
    this._f222afbef5910 = true;
    this._fc6d16c65200b = TimeSpan.Zero;
    if (this._f12823a397d00 || this._fb9e5c296f8da)
      return;
    this._f561470475e83 = 0.0f;
    this._m7902fc32fa5f();
    this._ma57ce84f6bea();
  }

  private void _m558e53b13bd6()
  {
    if (this._fc6d16c65200b == TimeSpan.Zero)
      this._fc6d16c65200b = this._f15383f3913b9.CurTime + TimeSpan.FromSeconds((double) this._f9526b89881f2);
    else if (this._f15383f3913b9.CurTime >= this._fc6d16c65200b)
    {
      this._f222afbef5910 = !this._f222afbef5910;
      this._fc6d16c65200b = this._f15383f3913b9.CurTime + TimeSpan.FromSeconds((double) this._f9526b89881f2);
    }
    this._f561470475e83 = this._f222afbef5910 ? this._ff9eed6291cdf : -this._ff9eed6291cdf;
  }

  internal bool _mdb8dd0aca972()
  {
    return this._f12823a397d00 && (double) MathF.Abs(this._f561470475e83) > 9.9999997473787516E-05 || this._fb9e5c296f8da;
  }

  internal bool _m9988195534c0() => this._fcabbd24816ae.Count > 0;

  internal void _m5691d410b8b1()
  {
    if (!this._mdb8dd0aca972())
      return;
    if (this._fcabbd24816ae.Count > 0)
      this._m7902fc32fa5f();
    this._f018bd2cf39ba.Clear();
    EntityQueryEnumerator<ItemComponent, SpriteComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ItemComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid;
    ItemComponent itemComponent;
    SpriteComponent spriteComponent;
    TransformComponent xform;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref itemComponent, ref spriteComponent, ref xform))
    {
      this._f018bd2cf39ba.Add(entityUid);
      Vector2 offset = spriteComponent.Offset;
      this._fcabbd24816ae[entityUid] = offset;
      Vector2 vector2_1 = offset;
      if (this._f12823a397d00 && (double) MathF.Abs(this._f561470475e83) > 9.9999997473787516E-05)
        vector2_1 = new Vector2(vector2_1.X + this._f561470475e83, vector2_1.Y);
      if (this._fb9e5c296f8da)
      {
        Vector2 vector2_2 = this._mb978fb01f125(entityUid, xform);
        vector2_1 = new Vector2(vector2_1.X + vector2_2.X, vector2_1.Y + vector2_2.Y);
      }
      this._f6523d9b67440.SetOffset(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), vector2_1);
    }
    if (this._f4eb3b35fc0eb.Count <= this._f018bd2cf39ba.Count)
      return;
    this._f91b5ba878e54.Clear();
    foreach ((EntityUid key, Queue<RMCProfileCacheNodea4fdbc._t5e239d9f0f2c> _) in this._f4eb3b35fc0eb)
    {
      if (!this._f018bd2cf39ba.Contains(key))
        this._f91b5ba878e54.Add(key);
    }
    for (int index = 0; index < this._f91b5ba878e54.Count; ++index)
    {
      EntityUid key = this._f91b5ba878e54[index];
      this._f4eb3b35fc0eb.Remove(key);
      this._fbf92d104791a.Remove(key);
      this._fb79a4fcfbaa0.Remove(key);
    }
  }

  private Vector2 _mb978fb01f125(EntityUid uid, TransformComponent xform)
  {
    Vector2 worldPosition = this._fff8cb00f9989.GetWorldPosition(xform);
    this._m40189bd71e4b(uid, worldPosition);
    this._fbf92d104791a[uid] = worldPosition;
    Queue<RMCProfileCacheNodea4fdbc._t5e239d9f0f2c> t5e239d9f0f2cQueue;
    if (!this._f4eb3b35fc0eb.TryGetValue(uid, out t5e239d9f0f2cQueue))
    {
      t5e239d9f0f2cQueue = new Queue<RMCProfileCacheNodea4fdbc._t5e239d9f0f2c>();
      this._f4eb3b35fc0eb[uid] = t5e239d9f0f2cQueue;
    }
    t5e239d9f0f2cQueue.Enqueue(new RMCProfileCacheNodea4fdbc._t5e239d9f0f2c(this._f15383f3913b9.CurTime, worldPosition));
    TimeSpan timeSpan1 = this._f15383f3913b9.CurTime - TimeSpan.FromSeconds((double) Math.Clamp((float) ((double) this._f10323b69a2fb + (double) this._f541baa812565 + 0.75), 0.3f, 5f));
    while (t5e239d9f0f2cQueue.Count > 1 && t5e239d9f0f2cQueue.Peek().At < timeSpan1)
      t5e239d9f0f2cQueue.Dequeue();
    float num1 = 0.0f;
    if ((double) this._f541baa812565 > 0.0)
      num1 = MathF.Sin((float) (this._f15383f3913b9.CurTime.TotalSeconds * 2.35 + (double) (uid.GetHashCode() & 1023 /*0x03FF*/) * 0.013)) * this._f541baa812565;
    TimeSpan timeSpan2 = this._f15383f3913b9.CurTime - TimeSpan.FromSeconds((double) Math.Clamp(this._f10323b69a2fb + num1, 0.05f, 3f));
    Vector2 vector2_1 = worldPosition;
    bool flag = false;
    foreach (RMCProfileCacheNodea4fdbc._t5e239d9f0f2c t5e239d9f0f2c in t5e239d9f0f2cQueue)
    {
      if (!(t5e239d9f0f2c.At > timeSpan2))
      {
        vector2_1 = t5e239d9f0f2c.Position;
        flag = true;
      }
      else
        break;
    }
    if (!flag && t5e239d9f0f2cQueue.Count > 0)
      vector2_1 = t5e239d9f0f2cQueue.Peek().Position;
    RMCProfileCacheNodea4fdbc._tb1b24893add2 tb1b24893add2;
    if (this._fb79a4fcfbaa0.TryGetValue(uid, out tb1b24893add2))
    {
      if (this._f15383f3913b9.CurTime < tb1b24893add2.Until)
        vector2_1 = tb1b24893add2.Position;
      else
        this._fb79a4fcfbaa0.Remove(uid);
    }
    Vector2 vector2_2 = vector2_1 - worldPosition;
    float num2 = Math.Clamp(this._fea062e8e2c0f, 0.1f, 8f);
    float num3 = vector2_2.Length();
    if ((double) num3 > (double) num2 && (double) num3 > 0.0)
      vector2_2 = vector2_2 / num3 * num2;
    return vector2_2;
  }

  private void _m40189bd71e4b(EntityUid uid, Vector2 currentPos)
  {
    Vector2 Position;
    if (this._fb79a4fcfbaa0.ContainsKey(uid) || !this._fbf92d104791a.TryGetValue(uid, out Position) || (double) (currentPos - Position).Length() < 0.949999988079071)
      return;
    float chance = Math.Clamp(this._fd9744e0b73ed, 0.0f, 1f);
    if ((double) chance <= 0.0 || !this._mbbdd9d65b3eb(uid, chance))
      return;
    float num = this._me33eaa6b266d(uid);
    this._fb79a4fcfbaa0[uid] = new RMCProfileCacheNodea4fdbc._tb1b24893add2(this._f15383f3913b9.CurTime + TimeSpan.FromSeconds((double) num), Position);
  }

  private bool _mbbdd9d65b3eb(EntityUid uid, float chance)
  {
    return ((double) MathF.Sin((float) (this._f15383f3913b9.CurTime.TotalSeconds * 11.3 + (double) (uid.GetHashCode() & 2047 /*0x07FF*/) * 0.031)) + 1.0) * 0.5 <= (double) chance;
  }

  private float _me33eaa6b266d(EntityUid uid)
  {
    float val1 = Math.Max(0.05f, this._fcd993a8da558);
    float num1 = Math.Max(val1, this._fc491a66ae124);
    float num2 = (float) (((double) MathF.Sin((float) (this._f15383f3913b9.CurTime.TotalSeconds * 7.17 + (double) (uid.GetHashCode() & 4095 /*0x0FFF*/) * 0.017)) + 1.0) * 0.5);
    return val1 + (num1 - val1) * num2;
  }

  internal void _m7902fc32fa5f()
  {
    if (this._fcabbd24816ae.Count == 0)
      return;
    foreach ((EntityUid key, Vector2 vector2) in this._fcabbd24816ae)
    {
      SpriteComponent spriteComponent;
      if (this.TryComp<SpriteComponent>(key, ref spriteComponent))
        this._f6523d9b67440.SetOffset(Entity<SpriteComponent>.op_Implicit((key, spriteComponent)), vector2);
    }
    this._fcabbd24816ae.Clear();
  }

  private void _ma57ce84f6bea()
  {
    if (this._f4eb3b35fc0eb.Count == 0 && this._fbf92d104791a.Count == 0 && this._fb79a4fcfbaa0.Count == 0)
      return;
    this._f4eb3b35fc0eb.Clear();
    this._fbf92d104791a.Clear();
    this._fb79a4fcfbaa0.Clear();
  }

  private void _m33a5f3c69c90()
  {
    bool flag = this._m4b218efac094();
    int totalCount1;
    int dynamicAssemblyCount;
    List<string> managedModules;
    int totalCount2;
    List<string> nativeModules;
    int totalCount3;
    List<string> sideMarkers;
    if (flag)
    {
      managedModules = this._m4bfb08b6ce79(out totalCount1, out dynamicAssemblyCount);
      nativeModules = this._m6d20cb590c99(out totalCount2);
      sideMarkers = this._m6a229e0fff47((IReadOnlyList<string>) managedModules, out totalCount3);
      this._f8fc6b1e36a38 = totalCount1;
      this._f5b4fd9514d1d = dynamicAssemblyCount;
      this._f347de9aaf490 = totalCount2;
      this._f173000f7639c = true;
      this._me2243750f5e5();
    }
    else
    {
      managedModules = new List<string>();
      nativeModules = new List<string>();
      sideMarkers = this._m180d97013cdd(out totalCount3);
      if (this._f173000f7639c)
      {
        totalCount1 = this._f8fc6b1e36a38;
        dynamicAssemblyCount = this._f5b4fd9514d1d;
        totalCount2 = this._f347de9aaf490;
      }
      else
      {
        totalCount1 = 0;
        dynamicAssemblyCount = 0;
        totalCount2 = 0;
      }
    }
    this.RaiseNetworkEvent((EntityEventArgs) new RMCWeaponProfilePulseEvent(this._fbb40e5d74392, this._f35d9eaacfa87, this._m93513b8e574b(), !flag, this._m2f80ae0722ad(), dynamicAssemblyCount, totalCount1, totalCount2, totalCount3, managedModules, nativeModules, this._m24b291836d04((IReadOnlyList<string>) sideMarkers)));
    this._fccb3e75c4e17 = RMCWeaponProfileProbeCatalog.RollLiveness(this._fccb3e75c4e17, this._f35d9eaacfa87);
    ++this._f35d9eaacfa87;
  }

  private List<string> _m180d97013cdd(out int totalCount)
  {
    totalCount = this._fc7cb89171f46.Count;
    return new List<string>((IEnumerable<string>) this._fc7cb89171f46);
  }

  private void _me2243750f5e5()
  {
    if (this._f15383f3913b9.CurTime < this._f85d654b0e3c7 && this._fc7cb89171f46.Count > 0)
      return;
    this._fc7cb89171f46.Clear();
    this._m33432bf71416(this._fc7cb89171f46);
    this._m09b7b0aefabe(this._fc7cb89171f46);
    this._m199639bdb2d0(this._fc7cb89171f46);
    this._ma34b1b50e863(this._fc7cb89171f46);
    this._mdc4fbb41a7ee(this._fc7cb89171f46);
    this._m23e52b444b79(this._fc7cb89171f46);
    this._m6ee162e1c432(this._fc7cb89171f46);
    this._m7a6ea419c68e(this._fc7cb89171f46);
    this._mc24568923027(this._fc7cb89171f46);
    this._m5cc61e511ec2(this._fc7cb89171f46);
    this._m30be2fecc038(this._fc7cb89171f46);
    this._fc7cb89171f46.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    if (this._fc7cb89171f46.Count > this._f9f10ca116b15)
      this._fc7cb89171f46.RemoveRange(this._f9f10ca116b15, this._fc7cb89171f46.Count - this._f9f10ca116b15);
    this._f85d654b0e3c7 = this._f15383f3913b9.CurTime + TimeSpan.FromSeconds((double) Math.Clamp(Math.Max(this._f2ae21401ded7 * 12f, 90f), 90f, 300f));
  }

  private bool _m4b218efac094()
  {
    bool flag;
    switch (this._f1fc2369db60b.CurrentState)
    {
      case LobbyState _:
      case QueueState _:
      case PubgPreLobbyHubState _:
      case CivLobbyState _:
      case ModeSelectState _:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    return flag;
  }

  internal void _m09870773dc85(RMCWeaponProfilePingEvent ev)
  {
    if (!this._fd9f7ec1c3f19 || ev.Nonce != this._fbb40e5d74392)
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new RMCWeaponProfilePongEvent(ev.Nonce, ev.Token));
  }

  internal void _mfb1ca1c81369(RMCWeaponProfileFrameRequestEvent ev)
  {
    if (!this._fd9f7ec1c3f19 || ev.Nonce != this._fbb40e5d74392)
      return;
    this._me3fb5983e2f7(ev.Nonce, ev.Token, ev.Mode, ev.UploadPayload, ev.RequireLivenessMarker, ev.LivenessGrid, ev.LivenessCellX, ev.LivenessCellY, ev.LivenessSizePercent, ev.LivenessRed, ev.LivenessGreen, ev.LivenessBlue);
  }

  private Task _me3fb5983e2f7(
    int nonce,
    int token,
    RMCWeaponProfileFrameMode mode,
    bool uploadPayload,
    bool requireLivenessMarker,
    byte livenessGrid,
    byte livenessCellX,
    byte livenessCellY,
    byte livenessSizePercent,
    byte livenessRed,
    byte livenessGreen,
    byte livenessBlue)
  {
    // ISSUE: variable of a compiler-generated type
    RMCProfileCacheNodea4fdbc._te47fa7eed0f6 stateMachine;
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003Et__builder = AsyncTaskMethodBuilder.Create();
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003E4__this = this;
    // ISSUE: reference to a compiler-generated field
    stateMachine.nonce = nonce;
    // ISSUE: reference to a compiler-generated field
    stateMachine.token = token;
    // ISSUE: reference to a compiler-generated field
    stateMachine.mode = mode;
    // ISSUE: reference to a compiler-generated field
    stateMachine.uploadPayload = uploadPayload;
    // ISSUE: reference to a compiler-generated field
    stateMachine.requireLivenessMarker = requireLivenessMarker;
    // ISSUE: reference to a compiler-generated field
    stateMachine.livenessGrid = livenessGrid;
    // ISSUE: reference to a compiler-generated field
    stateMachine.livenessCellX = livenessCellX;
    // ISSUE: reference to a compiler-generated field
    stateMachine.livenessCellY = livenessCellY;
    // ISSUE: reference to a compiler-generated field
    stateMachine.livenessSizePercent = livenessSizePercent;
    // ISSUE: reference to a compiler-generated field
    stateMachine.livenessRed = livenessRed;
    // ISSUE: reference to a compiler-generated field
    stateMachine.livenessGreen = livenessGreen;
    // ISSUE: reference to a compiler-generated field
    stateMachine.livenessBlue = livenessBlue;
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003Et__builder.Start<RMCProfileCacheNodea4fdbc._te47fa7eed0f6>(ref stateMachine);
    // ISSUE: reference to a compiler-generated field
    return stateMachine.\u003C\u003Et__builder.Task;
  }

  private Task<Image> _mfb7408e78f0d(RMCWeaponProfileFrameMode mode)
  {
    return mode != RMCWeaponProfileFrameMode.Viewport ? this._mc8cb0e1bbd4e() : this._m89172b83a256();
  }

  private Task<Image> _mc8cb0e1bbd4e()
  {
    // ISSUE: variable of a compiler-generated type
    RMCProfileCacheNodea4fdbc._t8120cabfe291 stateMachine;
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003Et__builder = AsyncTaskMethodBuilder<Image>.Create();
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003E4__this = this;
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003Et__builder.Start<RMCProfileCacheNodea4fdbc._t8120cabfe291>(ref stateMachine);
    // ISSUE: reference to a compiler-generated field
    return stateMachine.\u003C\u003Et__builder.Task;
  }

  private Task<Image> _m89172b83a256()
  {
    // ISSUE: variable of a compiler-generated type
    RMCProfileCacheNodea4fdbc._tac3f55d2d433 stateMachine;
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003Et__builder = AsyncTaskMethodBuilder<Image>.Create();
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003E4__this = this;
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003Et__builder.Start<RMCProfileCacheNodea4fdbc._tac3f55d2d433>(ref stateMachine);
    // ISSUE: reference to a compiler-generated field
    return stateMachine.\u003C\u003Et__builder.Task;
  }

  private static byte[] _m3163b7c9b49b(Image capture, int maxBytes)
  {
    using (MemoryStream memoryStream = new MemoryStream())
    {
      ImageExtensions.SaveAsJpeg(capture, (Stream) memoryStream);
      return memoryStream.Length > (long) maxBytes ? Array.Empty<byte>() : memoryStream.ToArray();
    }
  }

  private void _mbe325d7c950a()
  {
    if (this._fc7cf4cc09e9e || (double) this._f0bdf165242f2 <= 0.0)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._fe64d5c3cd708).LocalEntity;
    if (!localEntity.HasValue || this.HasComp<ScopingComponent>(localEntity.Value))
      return;
    EntityUid? activeItem = this._f7d57cbcf6d73.GetActiveItem(Entity<HandsComponent>.op_Implicit(localEntity.Value));
    PubgFocusViewComponent focusViewComponent;
    if (!activeItem.HasValue || !this.TryComp<PubgFocusViewComponent>(activeItem.Value, ref focusViewComponent) || focusViewComponent.Active)
      return;
    float? nullable = this._mf78f36394e29(localEntity.Value);
    if (!nullable.HasValue || (double) nullable.Value <= (double) this._f0bdf165242f2)
      return;
    this._fc7cf4cc09e9e = true;
    this.RaiseNetworkEvent((EntityEventArgs) new RMCWeaponFocusRangeEvent(this._fbb40e5d74392, nullable.Value));
  }

  private void _m4a27a9896f5b()
  {
    if (this._f15383f3913b9.CurTime < this._f89a31cf28d86)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._fe64d5c3cd708).LocalEntity;
    MobStateComponent mobStateComponent;
    if (!localEntity.HasValue || !this.TryComp<MobStateComponent>(localEntity.Value, ref mobStateComponent) || mobStateComponent.CurrentState == MobState.Dead)
      return;
    bool componentDrawFov = true;
    bool componentDrawLight = true;
    EyeComponent eyeComponent;
    if (this.TryComp<EyeComponent>(localEntity.Value, ref eyeComponent))
    {
      componentDrawFov = eyeComponent.DrawFov;
      componentDrawLight = eyeComponent.DrawLight;
    }
    bool examinerSkipChecks = false;
    bool examinerCheckInRangeUnOccluded = true;
    ExaminerComponent examinerComponent;
    if (this.TryComp<ExaminerComponent>(localEntity.Value, ref examinerComponent))
    {
      examinerSkipChecks = examinerComponent.SkipChecks;
      examinerCheckInRangeUnOccluded = examinerComponent.CheckInRangeUnOccluded;
    }
    IEye currentEye = this._f50f34ea87145.CurrentEye;
    bool drawFov = currentEye.DrawFov;
    bool drawLight = currentEye.DrawLight;
    if (((!(componentDrawFov & drawFov & componentDrawLight & drawLight) ? 0 : (!examinerSkipChecks ? 1 : 0)) & (examinerCheckInRangeUnOccluded ? 1 : 0)) != 0)
      return;
    this._f89a31cf28d86 = this._f15383f3913b9.CurTime + TimeSpan.FromSeconds((double) Math.Clamp(this._f2ae21401ded7, 3f, 60f));
    this.RaiseNetworkEvent((EntityEventArgs) new RMCWeaponViewProfileEvent(this._fbb40e5d74392, componentDrawFov, drawFov, componentDrawLight, drawLight, examinerSkipChecks, examinerCheckInRangeUnOccluded));
  }

  private float? _mf78f36394e29(EntityUid player)
  {
    MapCoordinates map = this._f50f34ea87145.PixelToMap(this._f04fcf4ed8d3f.MouseScreenPosition);
    if (MapId.op_Equality(map.MapId, MapId.Nullspace))
      return new float?();
    MapCoordinates mapCoordinates = this._fff8cb00f9989.GetMapCoordinates(player, (TransformComponent) null);
    return MapId.op_Equality(mapCoordinates.MapId, MapId.Nullspace) || MapId.op_Inequality(mapCoordinates.MapId, map.MapId) ? new float?() : new float?(Vector2.Distance(map.Position, mapCoordinates.Position));
  }

  private List<string> _m4bfb08b6ce79(out int totalCount, out int dynamicAssemblyCount)
  {
    List<string> stringList1 = new List<string>();
    List<string> stringList2 = this._m8349c8d89b58();
    this._me71f4a41897f(stringList1, (IEnumerable<Assembly>) this._fe5fec0d033c0.Assemblies, true);
    this._me71f4a41897f(stringList1, this._f790dcf8ed182.LoadedModules, true);
    foreach (string str in stringList2)
    {
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(stringList1, str))
        stringList1.Add(str);
    }
    dynamicAssemblyCount = 0;
    for (int index = 0; index < stringList1.Count; ++index)
    {
      if (RMCProfileCacheNodea4fdbc._m6c7f99cca0a9(stringList1[index]))
        ++dynamicAssemblyCount;
    }
    stringList1.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    totalCount = stringList1.Count;
    if (stringList1.Count > this._f9f10ca116b15)
      stringList1.RemoveRange(this._f9f10ca116b15, stringList1.Count - this._f9f10ca116b15);
    return stringList1;
  }

  private List<string> _m6d20cb590c99(out int totalCount)
  {
    totalCount = 0;
    return new List<string>();
  }

  private List<string> _m6a229e0fff47(IReadOnlyList<string> managedModules, out int totalCount)
  {
    List<string> markers = new List<string>();
    int num = !(this._f1fc2369db60b.CurrentState is GameplayState) ? 1 : 0;
    this._m7e0628737818(markers);
    this._m5e7dfa4d9efd(markers);
    if (num != 0)
    {
      this._m11879b2102dd(markers);
      this._m2bac56c34c97(markers);
      this._m8f2974f28880(markers);
    }
    this._m33432bf71416(markers);
    this._m09b7b0aefabe(markers);
    this._m199639bdb2d0(markers);
    this._ma34b1b50e863(markers);
    this._mdc4fbb41a7ee(markers);
    this._m23e52b444b79(markers);
    this._m6ee162e1c432(markers);
    this._m7a6ea419c68e(markers);
    this._mc24568923027(markers);
    this._m5cc61e511ec2(markers);
    this._m30be2fecc038(markers);
    this._mcfe0264c8fc9(markers, managedModules);
    markers.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    totalCount = markers.Count;
    if (markers.Count > this._f9f10ca116b15)
      markers.RemoveRange(this._f9f10ca116b15, markers.Count - this._f9f10ca116b15);
    return markers;
  }

  private void _m7e0628737818(List<string> markers)
  {
    foreach (string registeredCvar in this._fd5d515fa2fd2.GetRegisteredCVars())
    {
      if (RMCProfileCacheNodea4fdbc._mad358e34fcba(registeredCvar))
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m1ded9fb8a317(registeredCvar));
    }
  }

  private void _m5e7dfa4d9efd(List<string> markers)
  {
    foreach (object child in ((Control) this._fee5f4689f127.WindowRoot).Children)
    {
      string fullName = child.GetType().FullName;
      if (!string.IsNullOrWhiteSpace(fullName) && !this._m6630ed729cc3(fullName))
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m58fbe73f1311(fullName));
    }
  }

  private void _m11879b2102dd(List<string> markers)
  {
    List<string> items = this._mdfeca939925a();
    foreach (string typeName in items)
    {
      if (!this._m6630ed729cc3(typeName))
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m0ac020f69fd1(typeName));
    }
    foreach (string typeName in this._m6b0762212f15())
    {
      if (!this._m6630ed729cc3(typeName) && !RMCProfileCacheNodea4fdbc._m9306a10f12fe(items, typeName))
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m80d572865755(typeName));
    }
    foreach (string typeName in this._mb6791d056437())
    {
      if (!this._m6630ed729cc3(typeName) && !RMCProfileCacheNodea4fdbc._m9306a10f12fe(items, typeName))
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m80d572865755(typeName));
    }
  }

  private void _m2bac56c34c97(List<string> markers)
  {
    int count1 = this._mdfeca939925a().Count;
    int count2 = this._m6b0762212f15().Count;
    int count3 = this._mb6791d056437().Count;
    int total = Math.Max(count1, Math.Max(count2, count3));
    this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m1bde7db0783d(total));
    if (count1 != count2)
      this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._mad61b1654a92(count1, count2));
    if (count1 == count3)
      return;
    this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m07020afdc1be(count1, count3));
  }

  private void _m8f2974f28880(List<string> markers)
  {
    foreach (string typeName in this._m04cb0f6b9745())
    {
      if (!this._m6630ed729cc3(typeName))
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._me1ece98ebcd2(typeName));
    }
  }

  private void _ma34b1b50e863(List<string> markers)
  {
    foreach (string key in this._fa860cc85cf5f.AvailableCommands.Keys)
    {
      if (this._ma8b82ec93f4d(key, this._f5f8aa038a944))
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m26a812a5e361(key));
    }
  }

  private void _m199639bdb2d0(List<string> markers)
  {
    if (!string.IsNullOrWhiteSpace(this._f786b93097a0d) && this._fa860cc85cf5f.AvailableCommands.ContainsKey(this._f786b93097a0d))
      this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._feea1f9998df9);
    if (string.IsNullOrWhiteSpace(this._f4b2692c6081a))
      return;
    foreach (string registeredCvar in this._fd5d515fa2fd2.GetRegisteredCVars())
    {
      if (string.Equals(registeredCvar, this._f4b2692c6081a, StringComparison.OrdinalIgnoreCase))
      {
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._f8dcebeb407cb);
        break;
      }
    }
  }

  private void _mdc4fbb41a7ee(List<string> markers)
  {
    foreach (string key in this._fa860cc85cf5f.AvailableCommands.Keys)
    {
      if (this._ma8b82ec93f4d(key, this._f5dd6e4fa1bd6))
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m87f6dddff8d8(key));
    }
  }

  private List<string> _m04cb0f6b9745()
  {
    List<string> items = new List<string>();
    if (IoCManager.Instance == null)
      return items;
    foreach (Type registeredType in IoCManager.Instance.GetRegisteredTypes())
    {
      string fullName = registeredType.FullName;
      if (!string.IsNullOrWhiteSpace(fullName))
      {
        string str = this._m17038c921b89(fullName);
        if (str != null && !RMCProfileCacheNodea4fdbc._m9306a10f12fe(items, str))
          items.Add(str);
      }
    }
    items.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return items;
  }

  private string _m93513b8e574b()
  {
    string str;
    try
    {
      str = this._fd5d515fa2fd2.GetCVar<string>(CVars.BuildVersion);
    }
    catch
    {
      str = (string) null;
    }
    return this._m17038c921b89(str ?? "unknown") ?? "unknown";
  }

  private string? _m17038c921b89(string moduleName)
  {
    string str = moduleName.Trim();
    return str.Length == 0 ? (string) null : RMCProfileCacheNodea4fdbc._m21e044cb28f9(str, this._f2a3fad06af11);
  }

  private static string _m21e044cb28f9(string value, int maxLength)
  {
    if (value.Length <= maxLength)
      return value;
    if (maxLength <= 8)
    {
      string str = value;
      int num = maxLength;
      int length = str.Length;
      int startIndex = length - num;
      return str.Substring(startIndex, length - startIndex);
    }
    int length1 = Math.Max(8, Math.Min(48 /*0x30*/, (maxLength - "...".Length) / 2));
    int length2 = maxLength - "...".Length - length1;
    if (length2 < 4)
    {
      length2 = Math.Max(4, (maxLength - "...".Length) / 2);
      length1 = Math.Max(1, maxLength - "...".Length - length2);
    }
    return value.AsSpan(0, length2).ToString() + (ReadOnlySpan<char>) "..." + value.AsSpan(value.Length - length1, length1);
  }

  private static bool _m6c7f99cca0a9(string moduleName)
  {
    return moduleName.Contains("dynamic", StringComparison.OrdinalIgnoreCase) || moduleName.Contains("in memory", StringComparison.OrdinalIgnoreCase) || moduleName.Contains("anonymously hosted", StringComparison.OrdinalIgnoreCase) || moduleName.Contains("reflection.emit", StringComparison.OrdinalIgnoreCase) || moduleName.Contains("runtimegenerated", StringComparison.OrdinalIgnoreCase);
  }

  private bool _m6630ed729cc3(string typeName)
  {
    for (int index = 0; index < RMCProfileCacheNodea4fdbc._f3915e8d7abe1.Length; ++index)
    {
      if (typeName.StartsWith(RMCProfileCacheNodea4fdbc._f3915e8d7abe1[index], StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }

  private static bool _mad358e34fcba(string value)
  {
    for (int index = 0; index < RMCProfileCacheNodea4fdbc._fa692b02d069c.Length; ++index)
    {
      if (value.Contains(RMCProfileCacheNodea4fdbc._fa692b02d069c[index], StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }

  private static bool _mb80787549be2(string value)
  {
    for (int index = 0; index < RMCProfileCacheNodea4fdbc._f5c3e883080cf.Length; ++index)
    {
      if (value.Contains(RMCProfileCacheNodea4fdbc._f5c3e883080cf[index], StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }

  private void _me36d689c5b47(List<string> markers, string marker)
  {
    string str = this._mf7e47c8d44b5(marker);
    if (str == null || RMCProfileCacheNodea4fdbc._m9306a10f12fe(markers, str))
      return;
    markers.Add(str);
  }

  private static bool _m9306a10f12fe(List<string> items, string value)
  {
    for (int index = 0; index < items.Count; ++index)
    {
      if (string.Equals(items[index], value, StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }

  private static bool _mce06cb7f4481(IReadOnlyList<string> items, string value)
  {
    for (int index = 0; index < items.Count; ++index)
    {
      if (string.Equals(items[index], value, StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }

  private static void _m81d4032760f5(List<string> values)
  {
    values.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
  }

  private List<string> _m94f8bff530f6()
  {
    List<string> stringList = new List<string>();
    try
    {
      HashSet<string> stringSet1 = new HashSet<string>();
      foreach (Assembly assembly in (IEnumerable<Assembly>) this._fe5fec0d033c0.Assemblies)
      {
        string str = RMCProfileCacheNodea4fdbc._m9c6e1eb3db8d(assembly);
        if (str != null)
          stringSet1.Add(str);
      }
      int num = 0;
      HashSet<string> stringSet2 = new HashSet<string>();
      List<string> values = new List<string>();
      HashSet<string> stringSet3 = new HashSet<string>();
      foreach (Type allType in this._fe5fec0d033c0.FindAllTypes())
      {
        if ((object) allType != null)
        {
          string str1;
          try
          {
            str1 = allType.Assembly.GetName().Name ?? "unknown";
          }
          catch
          {
            continue;
          }
          if (!stringSet1.Contains(str1))
          {
            ++num;
            stringSet2.Add(str1);
            if (values.Count < 16 /*0x10*/)
            {
              string str2 = allType.FullName ?? allType.Name;
              string str3 = RMCProfileCacheNodea4fdbc._mac2ec4779a96(str2) ?? str2;
              string str4 = $"{str1}::{str3}";
              if (stringSet3.Add(str4))
                values.Add(str4);
            }
          }
        }
      }
      RMCProfileCacheNodea4fdbc._m81d4032760f5(values);
      stringList.Add($"origin-orphans={num}");
      stringList.Add($"origin-orphan-asms={stringSet2.Count}");
      for (int index = 0; index < values.Count; ++index)
        stringList.Add("orphan:" + values[index]);
    }
    catch
    {
    }
    return stringList;
  }

  private List<string> _m3c930ed2104c()
  {
    List<string> stringList = new List<string>();
    try
    {
      Dictionary<string, Assembly> dictionary = new Dictionary<string, Assembly>();
      foreach (Assembly assembly in (IEnumerable<Assembly>) this._fe5fec0d033c0.Assemblies)
      {
        string key = RMCProfileCacheNodea4fdbc._m9c6e1eb3db8d(assembly);
        if (key != null && !dictionary.ContainsKey(key))
          dictionary[key] = assembly;
      }
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      for (int index = 0; index < RMCProfileCacheNodea4fdbc._faa7440d10ced.Length; ++index)
      {
        string str = RMCProfileCacheNodea4fdbc._faa7440d10ced[index];
        Assembly assembly;
        if (!dictionary.TryGetValue(str, out assembly))
        {
          ++num3;
        }
        else
        {
          Type[] types;
          try
          {
            types = assembly.GetTypes();
          }
          catch
          {
            continue;
          }
          ++num1;
          HashSet<string> presentFullNames = new HashSet<string>();
          foreach (Type type in types)
          {
            if ((object) type != null)
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
              if (!string.Equals(a, str, StringComparison.Ordinal))
                ++num2;
              string fullName = type.FullName;
              if (fullName != null)
                presentFullNames.Add(fullName);
            }
          }
          if (!RMCProfileCacheNodea4fdbc._ma569915ee8de(str, presentFullNames))
            ++num3;
        }
      }
      stringList.Add($"identity-checked={num1}");
      stringList.Add($"identity-violations={num2}");
      stringList.Add($"identity-sentinel-miss={num3}");
    }
    catch
    {
    }
    return stringList;
  }

  private static bool _ma569915ee8de(string originName, HashSet<string> presentFullNames)
  {
    for (int index = 0; index < RMCProfileCacheNodea4fdbc._f985e4e500ee7.Length; ++index)
    {
      string str1 = RMCProfileCacheNodea4fdbc._f985e4e500ee7[index];
      int length = str1.IndexOf("::", StringComparison.Ordinal);
      if (length > 0 && string.Equals(str1.Substring(0, length), originName, StringComparison.Ordinal))
      {
        string str2 = str1;
        int startIndex = length + 2;
        string str3 = str2.Substring(startIndex, str2.Length - startIndex);
        return presentFullNames.Contains(str3);
      }
    }
    return true;
  }

  private List<string> _mc4cf1e9839d6()
  {
    List<string> stringList1 = new List<string>();
    try
    {
      HashSet<string> modules = new HashSet<string>();
      foreach (Assembly assembly in (IEnumerable<Assembly>) this._fe5fec0d033c0.Assemblies)
      {
        string str = RMCProfileCacheNodea4fdbc._m9c6e1eb3db8d(assembly);
        if (str != null)
          modules.Add(str);
      }
      foreach (Assembly loadedModule in this._f790dcf8ed182.LoadedModules)
      {
        string str = RMCProfileCacheNodea4fdbc._m9c6e1eb3db8d(loadedModule);
        if (str != null)
          modules.Add(str);
      }
      int orphanCount = 0;
      List<string> stringList2 = new List<string>();
      HashSet<string> sampleSeen = new HashSet<string>();
      RMCProfileCacheNodea4fdbc._m88360b0e1aee(this._fc6f45fbf437d.GetEntitySystemTypes(), "sys", modules, stringList2, sampleSeen, ref orphanCount);
      RMCProfileCacheNodea4fdbc._m88360b0e1aee(this._fc6f45fbf437d.DependencyCollection.GetRegisteredTypes(), "ioc", modules, stringList2, sampleSeen, ref orphanCount);
      RMCProfileCacheNodea4fdbc._m88360b0e1aee(this.EntityManager.ComponentFactory.AllRegisteredTypes, "comp", modules, stringList2, sampleSeen, ref orphanCount);
      RMCProfileCacheNodea4fdbc._m81d4032760f5(stringList2);
      stringList1.Add($"registry-orphans={orphanCount}");
      for (int index = 0; index < stringList2.Count; ++index)
        stringList1.Add("reg-orphan:" + stringList2[index]);
    }
    catch
    {
    }
    return stringList1;
  }

  private static void _m88360b0e1aee(
    IEnumerable<Type> types,
    string channel,
    HashSet<string> modules,
    List<string> samples,
    HashSet<string> sampleSeen,
    ref int orphanCount)
  {
    foreach (Type type in types)
    {
      if ((object) type != null)
      {
        string str1;
        try
        {
          str1 = type.Assembly.GetName().Name ?? "unknown";
        }
        catch
        {
          continue;
        }
        if (!modules.Contains(str1))
        {
          ++orphanCount;
          if (samples.Count < 16 /*0x10*/)
          {
            string str2 = type.FullName ?? type.Name;
            string str3 = RMCProfileCacheNodea4fdbc._mac2ec4779a96(str2) ?? str2;
            string str4 = $"{channel}:{str1}::{str3}";
            if (sampleSeen.Add(str4))
              samples.Add(str4);
          }
        }
      }
    }
  }

  private List<string> _m184152995743()
  {
    List<string> stringList = new List<string>();
    try
    {
      for (int index = 0; index < 512 /*0x0200*/; ++index)
        Type.GetType("System.Int32");
      long num1 = long.MaxValue;
      for (int index1 = 0; index1 < 6; ++index1)
      {
        long timestamp = Stopwatch.GetTimestamp();
        for (int index2 = 0; index2 < 4096 /*0x1000*/; ++index2)
          Type.GetType("System.Int32");
        long num2 = Stopwatch.GetTimestamp() - timestamp;
        if (num2 < num1)
          num1 = num2;
      }
      long frequency = Stopwatch.Frequency;
      int num3 = 0;
      if (num1 > 0L && frequency > 0L && (double) num1 * 1000000000.0 / (double) frequency / 4096.0 >= 4000.0)
        num3 = 1;
      stringList.Add($"resolver-bucket={num3}");
    }
    catch
    {
    }
    return stringList;
  }

  private static string? _m9c6e1eb3db8d(Assembly assembly)
  {
    try
    {
      return assembly.GetName().Name;
    }
    catch
    {
      return (string) null;
    }
  }

  private List<string> _m7532b8722c70()
  {
    List<string> items = new List<string>();
    try
    {
      foreach ((string str1, IConsoleCommand iconsoleCommand) in (IEnumerable<KeyValuePair<string, IConsoleCommand>>) this._fa860cc85cf5f.AvailableCommands)
      {
        if (!string.IsNullOrWhiteSpace(str1))
        {
          Type type = iconsoleCommand?.GetType();
          if ((object) type != null)
          {
            string str2 = type.FullName ?? type.Name;
            string str3 = RMCProfileCacheNodea4fdbc._ma9a5b04aeec3(type);
            string str4 = $"{this._m17038c921b89(str1) ?? str1}::{str3}::{str2}";
            if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items, str4))
              items.Add(str4);
          }
        }
      }
    }
    catch
    {
    }
    items.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return items;
  }

  private List<string> _md04b57f7e5c7()
  {
    List<string> items = new List<string>();
    try
    {
      foreach (Overlay allOverlay in this._fad8fd92ff30f.AllOverlays)
      {
        if (allOverlay != null)
        {
          Type type = allOverlay.GetType();
          string str1 = type.FullName ?? type.Name;
          string str2 = $"{RMCProfileCacheNodea4fdbc._ma9a5b04aeec3(type)}::{str1}";
          if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items, str2))
            items.Add(str2);
        }
      }
    }
    catch
    {
    }
    items.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return items;
  }

  private List<string> _m4d82a8566713()
  {
    List<string> items = new List<string>();
    try
    {
      for (int index = 0; index < RMCProfileCacheNodea4fdbc._f54b67dcd0ec9.Length; ++index)
      {
        string str1 = RMCProfileCacheNodea4fdbc._f54b67dcd0ec9[index];
        IEnumerable<ResPath> files;
        try
        {
          files = this._fdb0a9219e17c.ContentFindFiles(str1);
        }
        catch
        {
          continue;
        }
        foreach (ResPath resPath in files)
        {
          string filename = ((ResPath) ref resPath).Filename;
          if (!string.IsNullOrWhiteSpace(filename))
          {
            string str2 = ((ResPath) ref resPath).Extension ?? string.Empty;
            string str3 = RMCProfileCacheNodea4fdbc._mac2ec4779a96(filename);
            if (str3 != null)
            {
              string str4 = $"{str1}::{str2}::{str3}";
              if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items, str4))
                items.Add(str4);
            }
          }
        }
      }
    }
    catch
    {
    }
    items.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return items;
  }

  private List<string> _mca7e8320c192()
  {
    List<string> stringList = new List<string>();
    try
    {
      int num1 = 0;
      foreach (Assembly assembly in (IEnumerable<Assembly>) this._fe5fec0d033c0.Assemblies)
        ++num1;
      int num2 = 0;
      foreach (Assembly loadedModule in this._f790dcf8ed182.LoadedModules)
        ++num2;
      int num3 = 0;
      foreach (Type registeredType in this._fc6f45fbf437d.DependencyCollection.GetRegisteredTypes())
        ++num3;
      int num4 = 0;
      foreach (Type allRegisteredType in this.EntityManager.ComponentFactory.AllRegisteredTypes)
        ++num4;
      int num5 = 0;
      foreach (Type entitySystemType in this._fc6f45fbf437d.GetEntitySystemTypes())
        ++num5;
      int num6 = 0;
      foreach (KeyValuePair<string, IConsoleCommand> availableCommand in (IEnumerable<KeyValuePair<string, IConsoleCommand>>) this._fa860cc85cf5f.AvailableCommands)
        ++num6;
      int num7 = 0;
      foreach (Overlay allOverlay in this._fad8fd92ff30f.AllOverlays)
        ++num7;
      stringList.Add($"reflection-assemblies={num1}");
      stringList.Add($"modloader-assemblies={num2}");
      stringList.Add($"ioc-types={num3}");
      stringList.Add($"component-types={num4}");
      stringList.Add($"entitysystem-types={num5}");
      stringList.Add($"console-commands={num6}");
      stringList.Add($"overlays={num7}");
    }
    catch
    {
    }
    stringList.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return stringList;
  }

  private List<string> _md411f5064d3b()
  {
    List<string> values = new List<string>();
    for (int index = 0; index < RMCProfileCacheNodea4fdbc._f0d9d36cc60ae.Length; ++index)
    {
      Type type;
      if (this._m83d249542a76(RMCProfileCacheNodea4fdbc._f0d9d36cc60ae[index], out type) && (object) type != null)
      {
        if (type.FullName != null)
        {
          try
          {
            RMCProfileCacheNodea4fdbc._m4ac62cb5631f(values, type);
          }
          catch
          {
          }
        }
      }
    }
    values.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return values;
  }

  private static void _m4ac62cb5631f(List<string> values, Type type)
  {
    string str1 = type.FullName ?? type.Name;
    string str2 = $"{RMCProfileCacheNodea4fdbc._ma9a5b04aeec3(type)}::{str1}";
    string str3 = (type.IsAbstract ? "a" : "-") + (type.IsSealed ? "s" : "-") + (type.IsClass ? "c" : "-");
    Type baseType = type.BaseType;
    string str4 = (object) baseType != null ? baseType.FullName ?? baseType.Name : "none";
    values.Add($"{str2}::shape::{str3}::attr={(int) type.Attributes}::base={str4}");
    List<string> stringList1 = new List<string>();
    foreach (Type type1 in type.GetInterfaces())
    {
      string str5 = type1.FullName ?? type1.Name;
      if (!stringList1.Contains(str5))
        stringList1.Add(str5);
    }
    stringList1.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    for (int index = 0; index < stringList1.Count; ++index)
      values.Add($"{str2}::iface::{stringList1[index]}");
    List<string> stringList2 = new List<string>();
    HashSet<string> stringSet = new HashSet<string>();
    foreach (MemberInfo method in type.GetMethods())
    {
      string name = method.Name;
      if (!string.IsNullOrEmpty(name) && stringSet.Add(name))
        stringList2.Add(name);
    }
    stringList2.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    for (int index = 0; index < stringList2.Count; ++index)
      values.Add($"{str2}::m::{stringList2[index]}");
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
    if (!this._fd9f7ec1c3f19 || ev.Nonce != this._fbb40e5d74392)
      return;
    this._m182579b7c6ca(ev.Token, ev.MaxChunkBytes, ev.ChunkStepSeconds);
  }

  internal void _m38f49e42b882()
  {
    if (this._f17f6a4e3c845 == null)
      return;
    if (!this._fd9f7ec1c3f19 || this._f17f6a4e3c845.Nonce != this._fbb40e5d74392)
    {
      this._maf9f0d3098e1();
    }
    else
    {
      if (this._f15383f3913b9.CurTime < this._f17f6a4e3c845.NextAt)
        return;
      this._m33d70edd0d8c();
    }
  }

  private void _m182579b7c6ca(int token, int maxChunkBytes, float stepSeconds)
  {
    try
    {
      byte[] bytes = Encoding.UTF8.GetBytes(this._m484bab6dcff7());
      int num1 = Math.Clamp(maxChunkBytes, 4096 /*0x1000*/, 32768 /*0x8000*/);
      int num2 = Math.Max(1, (bytes.Length + num1 - 1) / num1);
      float num3 = Math.Clamp(stepSeconds, 0.05f, 2f);
      this._f17f6a4e3c845 = new RMCProfileCacheNodea4fdbc._ta9bd54d5e117()
      {
        Nonce = this._fbb40e5d74392,
        Token = token,
        Payload = bytes,
        ChunkSize = num1,
        ChunkCount = num2,
        StepSeconds = num3,
        NextAt = this._f15383f3913b9.CurTime
      };
      this._m33d70edd0d8c();
    }
    catch
    {
      this._maf9f0d3098e1();
      this.RaiseNetworkEvent((EntityEventArgs) new RMCWeaponProfileCatalogChunkEvent(this._fbb40e5d74392, token, 0, 0, false, Array.Empty<byte>()));
    }
  }

  private void _m33d70edd0d8c()
  {
    if (this._f17f6a4e3c845 == null)
      return;
    try
    {
      RMCProfileCacheNodea4fdbc._ta9bd54d5e117 f17f6a4e3c845 = this._f17f6a4e3c845;
      int sourceIndex = f17f6a4e3c845.NextChunk * f17f6a4e3c845.ChunkSize;
      int length = Math.Min(f17f6a4e3c845.ChunkSize, f17f6a4e3c845.Payload.Length - sourceIndex);
      byte[] numArray = new byte[length];
      Array.Copy((Array) f17f6a4e3c845.Payload, sourceIndex, (Array) numArray, 0, length);
      this.RaiseNetworkEvent((EntityEventArgs) new RMCWeaponProfileCatalogChunkEvent(f17f6a4e3c845.Nonce, f17f6a4e3c845.Token, f17f6a4e3c845.NextChunk, f17f6a4e3c845.ChunkCount, true, numArray));
      ++f17f6a4e3c845.NextChunk;
      if (f17f6a4e3c845.NextChunk >= f17f6a4e3c845.ChunkCount)
        this._maf9f0d3098e1();
      else
        f17f6a4e3c845.NextAt = this._f15383f3913b9.CurTime + TimeSpan.FromSeconds((double) f17f6a4e3c845.StepSeconds);
    }
    catch
    {
      RMCProfileCacheNodea4fdbc._ta9bd54d5e117 f17f6a4e3c845 = this._f17f6a4e3c845;
      this._maf9f0d3098e1();
      if (f17f6a4e3c845 == null)
        return;
      this.RaiseNetworkEvent((EntityEventArgs) new RMCWeaponProfileCatalogChunkEvent(f17f6a4e3c845.Nonce, f17f6a4e3c845.Token, 0, 0, false, Array.Empty<byte>()));
    }
  }

  private void _maf9f0d3098e1()
  {
    this._f17f6a4e3c845 = (RMCProfileCacheNodea4fdbc._ta9bd54d5e117) null;
  }

  private string _m484bab6dcff7()
  {
    StringBuilder builder = new StringBuilder(196608 /*0x030000*/);
    builder.AppendLine("profile-catalog-version: 2");
    StringBuilder stringBuilder1 = builder;
    StringBuilder stringBuilder2 = stringBuilder1;
    StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(17, 1, stringBuilder1);
    interpolatedStringHandler.AppendLiteral("captured-at-utc: ");
    interpolatedStringHandler.AppendFormatted<DateTime>(DateTime.UtcNow, "O");
    ref StringBuilder.AppendInterpolatedStringHandler local1 = ref interpolatedStringHandler;
    stringBuilder2.AppendLine(ref local1);
    StringBuilder stringBuilder3 = builder;
    StringBuilder stringBuilder4 = stringBuilder3;
    interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(15, 1, stringBuilder3);
    interpolatedStringHandler.AppendLiteral("build-version: ");
    interpolatedStringHandler.AppendFormatted(this._m93513b8e574b());
    ref StringBuilder.AppendInterpolatedStringHandler local2 = ref interpolatedStringHandler;
    stringBuilder4.AppendLine(ref local2);
    StringBuilder stringBuilder5 = builder;
    StringBuilder stringBuilder6 = stringBuilder5;
    interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder5);
    interpolatedStringHandler.AppendLiteral("burst-seed: ");
    interpolatedStringHandler.AppendFormatted<int>(this._fbb40e5d74392);
    ref StringBuilder.AppendInterpolatedStringHandler local3 = ref interpolatedStringHandler;
    stringBuilder6.AppendLine(ref local3);
    builder.AppendLine("sandbox-safe: true");
    builder.AppendLine("native-scan: unavailable");
    builder.AppendLine("private-reflection: unavailable");
    builder.AppendLine();
    RMCProfileCacheNodea4fdbc._m46787f1aef99(builder, "assemblies.resources", (IReadOnlyList<string>) this._m8349c8d89b58());
    RMCProfileCacheNodea4fdbc._m46787f1aef99(builder, "assemblies.reflection", (IReadOnlyList<string>) RMCProfileCacheNodea4fdbc._m0ba26a5dc294((IEnumerable<Assembly>) this._fe5fec0d033c0.Assemblies));
    RMCProfileCacheNodea4fdbc._m46787f1aef99(builder, "assemblies.modloader", (IReadOnlyList<string>) RMCProfileCacheNodea4fdbc._m0ba26a5dc294(this._f790dcf8ed182.LoadedModules));
    RMCProfileCacheNodea4fdbc._m46787f1aef99(builder, "systems.runtime", (IReadOnlyList<string>) this._mdfeca939925a());
    RMCProfileCacheNodea4fdbc._m46787f1aef99(builder, "systems.dependency", (IReadOnlyList<string>) this._mb6791d056437());
    RMCProfileCacheNodea4fdbc._m46787f1aef99(builder, "ioc.registered", (IReadOnlyList<string>) this._m04cb0f6b9745());
    RMCProfileCacheNodea4fdbc._m46787f1aef99(builder, "types.discoverable", (IReadOnlyList<string>) RMCProfileCacheNodea4fdbc._m8d4e4634c121(this._fe5fec0d033c0.FindAllTypes()));
    RMCProfileCacheNodea4fdbc._m46787f1aef99(builder, "types.signature-probes", (IReadOnlyList<string>) this._m100c946575bf());
    RMCProfileCacheNodea4fdbc._m46787f1aef99(builder, "overlays.surface", (IReadOnlyList<string>) this._md04b57f7e5c7());
    return builder.ToString();
  }

  private static void _m46787f1aef99(
    StringBuilder builder,
    string section,
    IReadOnlyList<string> values)
  {
    builder.Append('[');
    builder.Append(section);
    builder.AppendLine("]");
    if (values.Count == 0)
    {
      builder.AppendLine("-");
      builder.AppendLine();
    }
    else
    {
      for (int index = 0; index < values.Count; ++index)
        builder.AppendLine(values[index]);
      builder.AppendLine();
    }
  }

  private static List<string> _m0ba26a5dc294(IEnumerable<Assembly> assemblies)
  {
    List<string> items = new List<string>();
    foreach (Assembly assembly in assemblies)
    {
      try
      {
        string str1 = assembly.GetName().Name ?? "unknown";
        string str2 = assembly.FullName ?? str1;
        string str3 = $"{str1} | full={str2}";
        if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items, str3))
          items.Add(str3);
      }
      catch
      {
      }
    }
    items.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return items;
  }

  private static List<string> _m8d4e4634c121(IEnumerable<Type> types)
  {
    List<string> items = new List<string>();
    foreach (Type type in types)
    {
      if (!string.IsNullOrWhiteSpace(type.FullName))
      {
        string str1;
        try
        {
          str1 = type.Assembly.GetName().Name ?? "unknown";
        }
        catch
        {
          str1 = "unknown";
        }
        string str2 = $"{str1}::{type.FullName}";
        if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items, str2))
          items.Add(str2);
      }
    }
    items.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return items;
  }

  private List<string> _m100c946575bf()
  {
    List<string> items = new List<string>();
    for (int index = 0; index < RMCProfileCacheNodea4fdbc._fa2ea7b4dd80d.Length; ++index)
    {
      Type type;
      if (this._m83d249542a76(RMCProfileCacheNodea4fdbc._fa2ea7b4dd80d[index].TypeName, out type) && (object) type != null && type.FullName != null)
      {
        string str = type.Assembly.GetName().Name ?? "unknown";
        items.Add($"{RMCProfileCacheNodea4fdbc._fa2ea7b4dd80d[index].Marker}::{str}::{type.FullName}");
      }
    }
    for (int index = 0; index < RMCProfileCacheNodea4fdbc._f0eccc63a6b39.Length; ++index)
    {
      Type type;
      if (this._m83d249542a76(RMCProfileCacheNodea4fdbc._f0eccc63a6b39[index].TypeName, out type) && (object) type != null && type.FullName != null)
      {
        string str1 = type.Assembly.GetName().Name ?? "unknown";
        string str2 = $"{RMCProfileCacheNodea4fdbc._f0eccc63a6b39[index].Marker}::{str1}::{type.FullName}";
        if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items, str2))
          items.Add(str2);
      }
    }
    items.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return items;
  }

  private List<string> _m24b291836d04(IReadOnlyList<string> sideMarkers)
  {
    List<string> stringList = new List<string>(sideMarkers.Count);
    for (int index = 0; index < sideMarkers.Count; ++index)
      stringList.Add(this._me8876d792424(sideMarkers[index]));
    return stringList;
  }

  private string _me8876d792424(string value)
  {
    if (string.IsNullOrEmpty(value))
      return '~'.ToString();
    char[] chArray = new char[value.Length + 1];
    chArray[0] = '~';
    int seed = RMCProfileCacheNodea4fdbc._mfaec1ce6466d(this._fbb40e5d74392, value.Length);
    for (int index = 0; index < value.Length; ++index)
      chArray[index + 1] = RMCProfileCacheNodea4fdbc._m2d6278b036aa(value[index], index, seed, true);
    return new string(chArray);
  }

  private string? _mf7e47c8d44b5(string marker)
  {
    string str = marker.Trim();
    if (str.Length == 0)
      return (string) null;
    int maxLength = Math.Max(1, this._f2a3fad06af11 - 1);
    return RMCProfileCacheNodea4fdbc._m21e044cb28f9(str, maxLength);
  }

  private static int _mfaec1ce6466d(int nonce, int length)
  {
    int num1 = nonce ^ (int) (uint) ((ulong) length * 2654435769UL);
    int num2 = (num1 ^ num1 >>> 16 /*0x10*/) * 2146121005;
    int num3 = (num2 ^ num2 >>> 15) * -2073254261;
    return num3 ^ num3 >>> 16 /*0x10*/;
  }

  private static char _m2d6278b036aa(char value, int index, int seed, bool encode)
  {
    if (value < ' ' || value > '~')
      return value;
    int num1 = ((seed >> (index & 3) * 8 & (int) byte.MaxValue) + index * 17 + 11) % 95;
    int num2 = (int) value - 32 /*0x20*/;
    return (char) (32 /*0x20*/ + (encode ? (num2 + num1) % 95 : (num2 - num1 + 190) % 95));
  }

  internal void _m4ec44a315a26(RMCWeaponProfileIntegrityRequestEvent ev)
  {
    if (!this._fd9f7ec1c3f19 || ev.Nonce != this._fbb40e5d74392)
      return;
    this._mb6cb749334d2(ev.Token, ev.ChallengeSalt, (IReadOnlyList<byte>) ev.ProbeIds);
  }

  private void _mb6cb749334d2(int token, int challengeSalt, IReadOnlyList<byte> opaqueProbeIds)
  {
    try
    {
      List<byte> probeIds = new List<byte>(opaqueProbeIds.Count);
      List<string> probeDigests = new List<string>(opaqueProbeIds.Count);
      Dictionary<byte, byte> dictionary = RMCWeaponProfileProbeCatalog.BuildLogicalProbeMap(this._m93513b8e574b());
      for (int index = 0; index < opaqueProbeIds.Count; ++index)
      {
        byte opaqueProbeId = opaqueProbeIds[index];
        byte logicalProbeId;
        string digest;
        if (dictionary.TryGetValue(opaqueProbeId, out logicalProbeId) && this._m156539230b32(logicalProbeId, challengeSalt, out digest))
        {
          probeIds.Add(opaqueProbeId);
          probeDigests.Add(digest);
        }
      }
      this.RaiseNetworkEvent((EntityEventArgs) new RMCWeaponProfileIntegrityResponseEvent(this._fbb40e5d74392, token, probeIds.Count == opaqueProbeIds.Count, probeIds, probeDigests, this._fccb3e75c4e17));
    }
    catch
    {
      this.RaiseNetworkEvent((EntityEventArgs) new RMCWeaponProfileIntegrityResponseEvent(this._fbb40e5d74392, token, false, new List<byte>(), new List<string>()));
    }
  }

  private bool _m156539230b32(byte logicalProbeId, int challengeSalt, out string digest)
  {
    return this._m1b9f8ca1b9cc(logicalProbeId, challengeSalt, out digest) || this._md52e0452be71(logicalProbeId, challengeSalt, out digest) || this._m20af58ec8580(logicalProbeId, challengeSalt, out digest);
  }

  private bool _m1b9f8ca1b9cc(byte logicalProbeId, int challengeSalt, out string digest)
  {
    digest = string.Empty;
    string digest1;
    if (logicalProbeId == (byte) 40 || !this._m1f6cc241a08b(logicalProbeId, out digest1))
      return false;
    byte[] hex = RMCWeaponProfileDigest.ParseHex((ReadOnlySpan<char>) digest1);
    if (hex.Length == 0)
      return false;
    RMCWeaponProfileDigest.ApplyChallengeMask(hex, challengeSalt);
    digest = RMCWeaponProfileDigest.ToHexLower((ReadOnlySpan<byte>) hex);
    return true;
  }

  private bool _md52e0452be71(byte logicalProbeId, int challengeSalt, out string digest)
  {
    digest = string.Empty;
    List<string> stringList;
    switch (logicalProbeId)
    {
      case 20:
        stringList = this._mdfeca939925a();
        break;
      case 21:
        stringList = this._m04cb0f6b9745();
        break;
      case 22:
        stringList = RMCProfileCacheNodea4fdbc._m8d4e4634c121(this._fe5fec0d033c0.FindAllTypes());
        break;
      case 23:
        stringList = this._m100c946575bf();
        break;
      case 24:
        stringList = RMCProfileCacheNodea4fdbc._m0ba26a5dc294(this._f790dcf8ed182.LoadedModules);
        break;
      case 25:
        stringList = RMCProfileCacheNodea4fdbc._m0ba26a5dc294((IEnumerable<Assembly>) this._fe5fec0d033c0.Assemblies);
        break;
      case 26:
        stringList = this._mb6791d056437();
        break;
      case 27:
        stringList = this._mdfeca939925a();
        break;
      case 28:
        stringList = this._m04cb0f6b9745();
        break;
      case 29:
        stringList = RMCProfileCacheNodea4fdbc._m8d4e4634c121(this._fe5fec0d033c0.FindAllTypes());
        break;
      case 30:
        stringList = RMCProfileCacheNodea4fdbc._m0ba26a5dc294(this._f790dcf8ed182.LoadedModules);
        break;
      case 31 /*0x1F*/:
        stringList = RMCProfileCacheNodea4fdbc._m0ba26a5dc294((IEnumerable<Assembly>) this._fe5fec0d033c0.Assemblies);
        break;
      case 50:
        stringList = this._m7532b8722c70();
        break;
      case 51:
        stringList = this._md04b57f7e5c7();
        break;
      case 52:
        stringList = this._m4d82a8566713();
        break;
      case 53:
        stringList = this._mca7e8320c192();
        break;
      case 54:
        stringList = this._md411f5064d3b();
        break;
      case 55:
        stringList = this._md411f5064d3b();
        break;
      case 56:
        stringList = this._m94f8bff530f6();
        break;
      case 57:
        stringList = this._m3c930ed2104c();
        break;
      case 58:
        stringList = this._mc4cf1e9839d6();
        break;
      case 59:
        stringList = this._m184152995743();
        break;
      default:
        stringList = (List<string>) null;
        break;
    }
    IReadOnlyList<string> values = (IReadOnlyList<string>) stringList;
    if (values == null)
      return false;
    digest = RMCProfileCacheNodea4fdbc._mff6119bc2538((IEnumerable<string>) values, challengeSalt, RMCProfileCacheNodea4fdbc._m2e9b9092124c(logicalProbeId));
    return true;
  }

  private bool _m20af58ec8580(byte logicalProbeId, int challengeSalt, out string digest)
  {
    digest = string.Empty;
    List<string> stringList;
    switch (logicalProbeId)
    {
      case 40:
        stringList = this._m8129573df706();
        break;
      case 45:
        stringList = this._m8129573df706();
        break;
      case 46:
        stringList = this._me0ec0de3687b();
        break;
      case 47:
        stringList = this._mb7e2e6618a7b();
        break;
      case 48 /*0x30*/:
        stringList = this._mdd6debd0071a();
        break;
      case 49:
        stringList = this._m09204b64e531();
        break;
      default:
        stringList = (List<string>) null;
        break;
    }
    IReadOnlyList<string> values = (IReadOnlyList<string>) stringList;
    if (values == null)
      return false;
    digest = RMCProfileCacheNodea4fdbc._mff6119bc2538((IEnumerable<string>) values, challengeSalt, RMCProfileCacheNodea4fdbc._m2e9b9092124c(logicalProbeId));
    return true;
  }

  private bool _m1f6cc241a08b(byte logicalProbeId, out string digest)
  {
    digest = string.Empty;
    string str;
    if (!this._me70514751618() || this._f7e86bd84e5bc == null || !this._f7e86bd84e5bc.TryGetValue(logicalProbeId, out str))
      return false;
    digest = str;
    return true;
  }

  private bool _me70514751618()
  {
    if (this._fc859d8c9da48)
      return this._f7e86bd84e5bc != null && this._f7e86bd84e5bc.Count > 0;
    this._fc859d8c9da48 = true;
    try
    {
      string str1 = this._m8d27f4e1d855();
      if (str1.Length == 0)
        return false;
      Dictionary<byte, string> dictionary = new Dictionary<byte, string>();
      string str2 = str1;
      char[] separator = new char[2]{ '\r', '\n' };
      foreach (string text in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
      {
        if (text.Length != 0 && text[0] != '#')
        {
          if (text.StartsWith("build-version:", StringComparison.OrdinalIgnoreCase))
          {
            string str3 = text;
            int length = "build-version:".Length;
            this._fa834100c3212 = str3.Substring(length, str3.Length - length).Trim();
          }
          else if (text.StartsWith("probe:", StringComparison.OrdinalIgnoreCase))
          {
            int num = text.IndexOf('=');
            byte result;
            if (num > "probe:".Length && byte.TryParse(text.AsSpan("probe:".Length, num - "probe:".Length).Trim(), out result))
            {
              string str4 = text;
              int startIndex = num + 1;
              string str5 = str4.Substring(startIndex, str4.Length - startIndex).Trim();
              if (str5.Length != 0)
                dictionary[result] = str5.ToLowerInvariant();
            }
          }
        }
      }
      if (dictionary.Count == 0)
        return false;
      this._f7e86bd84e5bc = dictionary;
      return true;
    }
    catch
    {
      this._f7e86bd84e5bc = (Dictionary<byte, string>) null;
      this._f44edb66ba5d3 = (Dictionary<string, string>) null;
      this._fa834100c3212 = string.Empty;
      return false;
    }
  }

  private string _m8d27f4e1d855()
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    StringBuilder stringBuilder1 = new StringBuilder();
    IReadOnlyList<string> manifestFragmentPaths = RMCWeaponProfileProbeCatalog.GetPackagedManifestFragmentPaths(this._m93513b8e574b());
    for (int index = 0; index < manifestFragmentPaths.Count; ++index)
    {
      string key = manifestFragmentPaths[index];
      if (this._fdb0a9219e17c.ContentFileExists(key))
      {
        string str1 = this._fdb0a9219e17c.ContentFileReadAllText(key);
        if (!string.IsNullOrWhiteSpace(str1))
        {
          dictionary[key] = str1;
          if (stringBuilder1.Length > 0)
          {
            StringBuilder stringBuilder2 = stringBuilder1;
            if (stringBuilder2[stringBuilder2.Length - 1] != '\n')
              stringBuilder1.Append('\n');
          }
          stringBuilder1.Append(str1);
          string str2 = str1;
          if (str2[str2.Length - 1] != '\n')
            stringBuilder1.Append('\n');
        }
      }
    }
    this._f44edb66ba5d3 = dictionary;
    return stringBuilder1.ToString();
  }

  private List<string> _m8129573df706()
  {
    List<string> stringList = new List<string>();
    HashSet<string> stringSet = new HashSet<string>();
    foreach (ResPath file in this._fdb0a9219e17c.ContentFindFiles("/Assemblies"))
    {
      if (string.Equals(((ResPath) ref file).Extension, "dll", StringComparison.OrdinalIgnoreCase))
      {
        string str = RMCProfileCacheNodea4fdbc._mac2ec4779a96(((ResPath) ref file).FilenameWithoutExtension);
        if (str != null && stringSet.Add(str))
          stringList.Add(str);
      }
    }
    stringList.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return stringList;
  }

  private List<string> _me0ec0de3687b()
  {
    List<string> stringList = new List<string>();
    if (!this._me70514751618() || this._f7e86bd84e5bc == null)
      return stringList;
    if (!string.IsNullOrWhiteSpace(this._fa834100c3212))
      stringList.Add("build:" + this._fa834100c3212);
    List<byte> byteList = new List<byte>((IEnumerable<byte>) this._f7e86bd84e5bc.Keys);
    byteList.Sort();
    for (int index = 0; index < byteList.Count; ++index)
    {
      byte key = byteList[index];
      string str;
      if (this._f7e86bd84e5bc.TryGetValue(key, out str))
        stringList.Add($"probe:{key}:{str}");
    }
    return stringList;
  }

  private List<string> _mb7e2e6618a7b()
  {
    List<string> stringList1 = new List<string>();
    if (!this._me70514751618() || this._f44edb66ba5d3 == null)
      return stringList1;
    List<string> stringList2 = new List<string>((IEnumerable<string>) this._f44edb66ba5d3.Keys);
    stringList2.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    for (int index = 0; index < stringList2.Count; ++index)
    {
      string str = RMCProfileCacheNodea4fdbc._mac2ec4779a96(stringList2[index]);
      if (str != null)
        stringList1.Add(str);
    }
    return stringList1;
  }

  private List<string> _mdd6debd0071a()
  {
    List<string> stringList1 = new List<string>();
    if (!this._me70514751618() || this._f44edb66ba5d3 == null)
      return stringList1;
    List<string> stringList2 = new List<string>((IEnumerable<string>) this._f44edb66ba5d3.Keys);
    stringList2.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    for (int index = 0; index < stringList2.Count; ++index)
    {
      string str1 = stringList2[index];
      string raw;
      if (this._f44edb66ba5d3.TryGetValue(str1, out raw))
      {
        string str2 = RMCProfileCacheNodea4fdbc._m54accce96bb0("fragment", str1, raw);
        if (str2 != null)
          stringList1.Add(str2);
      }
    }
    return stringList1;
  }

  private List<string> _m09204b64e531()
  {
    List<string> stringList = new List<string>();
    IReadOnlyList<string> resourceProbePaths = RMCWeaponProfileProbeCatalog.GetIntegrityBenignResourceProbePaths();
    for (int index = 0; index < resourceProbePaths.Count; ++index)
    {
      string path = resourceProbePaths[index];
      if (this._fdb0a9219e17c.ContentFileExists(path))
      {
        string raw = this._fdb0a9219e17c.ContentFileReadAllText(path);
        if (!string.IsNullOrWhiteSpace(raw))
        {
          string str = RMCProfileCacheNodea4fdbc._m54accce96bb0("resource", path, raw);
          if (str != null)
            stringList.Add(str);
        }
      }
    }
    stringList.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return stringList;
  }

  private static string? _m54accce96bb0(string prefix, string path, string raw)
  {
    string str1 = RMCProfileCacheNodea4fdbc._mac2ec4779a96(path);
    string str2 = RMCProfileCacheNodea4fdbc._m40f2207c9d91(raw);
    if (str1 == null || str2 == null)
      return (string) null;
    string hexLower = RMCWeaponProfileDigest.ToHexLower((ReadOnlySpan<byte>) RMCWeaponProfileDigest.ComputeDigest(str2));
    return $"{prefix}:{str1}:{hexLower}";
  }

  private static string? _m40f2207c9d91(string? raw)
  {
    if (string.IsNullOrWhiteSpace(raw))
      return (string) null;
    string str = raw.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Trim();
    return str.Length != 0 ? RMCProfileCacheNodea4fdbc._mb8f3d40b990b(str, 16777216 /*0x01000000*/) : (string) null;
  }

  private static string _mff6119bc2538(
    IEnumerable<string> values,
    int challengeSalt,
    byte variantId = 0)
  {
    List<string> stringList = new List<string>();
    foreach (string str1 in values)
    {
      string str2 = RMCProfileCacheNodea4fdbc._mac2ec4779a96(str1);
      if (str2 != null)
        stringList.Add(str2);
    }
    if (stringList.Count == 0)
      return RMCProfileCacheNodea4fdbc._m6631a6e21eaf((ReadOnlySpan<byte>) Encoding.UTF8.GetBytes("count=0\n"), challengeSalt);
    StringBuilder stringBuilder = new StringBuilder();
    if (variantId == (byte) 0)
    {
      for (int index = 0; index < stringList.Count; ++index)
      {
        string str = stringList[index];
        stringBuilder.Append(str.Length);
        stringBuilder.Append(':');
        stringBuilder.Append(str);
        stringBuilder.Append('\n');
      }
      stringBuilder.Insert(0, $"count={stringList.Count}\n");
      return RMCProfileCacheNodea4fdbc._m6631a6e21eaf((ReadOnlySpan<byte>) Encoding.UTF8.GetBytes(stringBuilder.ToString()), challengeSalt);
    }
    stringList.Sort((Comparison<string>) ((left, right) =>
    {
      int num = right.Length.CompareTo(left.Length);
      return num == 0 ? string.Compare(left, right, StringComparison.OrdinalIgnoreCase) : num;
    }));
    int num1 = 0;
    stringBuilder.Append("variant=");
    stringBuilder.Append(variantId);
    stringBuilder.Append('\n');
    stringBuilder.Append("count=");
    stringBuilder.Append(stringList.Count);
    stringBuilder.Append('\n');
    for (int index = 0; index < stringList.Count; ++index)
    {
      string str = stringList[index];
      num1 += str.Length;
      stringBuilder.Append(index);
      stringBuilder.Append('#');
      stringBuilder.Append(str.Length);
      stringBuilder.Append('#');
      stringBuilder.Append(str);
      stringBuilder.Append('\n');
    }
    stringBuilder.Append("total=");
    stringBuilder.Append(num1);
    stringBuilder.Append('\n');
    return RMCProfileCacheNodea4fdbc._m6631a6e21eaf((ReadOnlySpan<byte>) Encoding.UTF8.GetBytes(stringBuilder.ToString()), challengeSalt);
  }

  private static string _m6631a6e21eaf(ReadOnlySpan<byte> rawBytes, int challengeSalt)
  {
    byte[] digest = RMCWeaponProfileDigest.ComputeDigest(rawBytes);
    RMCWeaponProfileDigest.ApplyChallengeMask(digest, challengeSalt);
    return RMCWeaponProfileDigest.ToHexLower((ReadOnlySpan<byte>) digest);
  }

  private static string? _mac2ec4779a96(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? (string) null : RMCProfileCacheNodea4fdbc._mb8f3d40b990b(value.Trim(), 512 /*0x0200*/);
  }

  private static string _mb8f3d40b990b(string value, int maxLength)
  {
    if (value.Length <= maxLength)
      return value;
    if (maxLength <= 8)
    {
      string str = value;
      int num = maxLength;
      int length = str.Length;
      int startIndex = length - num;
      return str.Substring(startIndex, length - startIndex);
    }
    int num1 = Math.Max(24, Math.Min(96 /*0x60*/, (maxLength - "...".Length) / 2));
    int length1 = Math.Max(24, maxLength - "...".Length - num1);
    if (length1 + "...".Length + num1 > maxLength)
      num1 = Math.Max(8, maxLength - "...".Length - length1);
    return value.AsSpan(0, length1).ToString() + (ReadOnlySpan<char>) "..." + value.AsSpan(value.Length - num1);
  }

  private static byte _m2e9b9092124c(byte logicalProbeId)
  {
    byte num;
    switch (logicalProbeId)
    {
      case 27:
      case 28:
      case 29:
      case 30:
      case 31 /*0x1F*/:
      case 45:
      case 46:
      case 55:
        num = (byte) 1;
        break;
      case 47:
      case 48 /*0x30*/:
        num = (byte) 2;
        break;
      case 49:
        num = (byte) 3;
        break;
      case 50:
      case 51:
      case 52:
      case 53:
        num = (byte) 4;
        break;
      default:
        num = (byte) 0;
        break;
    }
    return num;
  }

  private static string _mefd73cb8f295(string marker)
  {
    return RMCProfileCacheNodea4fdbc._f0cafd36a2603 + marker;
  }

  private static string _m283721b78071(int visibleCount, int rawCount)
  {
    return $"{RMCProfileCacheNodea4fdbc._f01621c7de67e}{visibleCount.ToString()}->{rawCount.ToString()}";
  }

  private static string _ma405f988c325(string moduleName)
  {
    return RMCProfileCacheNodea4fdbc._f6b9f656785e8 + moduleName;
  }

  private static string _m46efcd1d0fa6(int visibleCount, int rawCount)
  {
    return $"{RMCProfileCacheNodea4fdbc._fdcfcd8ecb6ed}{visibleCount.ToString()}->{rawCount.ToString()}";
  }

  private static string _m2cb986304ff7(string moduleName)
  {
    return RMCProfileCacheNodea4fdbc._f134aa1494e45 + moduleName;
  }

  private static string _mfbc6cf3db6f2(string assemblyName)
  {
    return RMCProfileCacheNodea4fdbc._f52288cec5de7 + assemblyName;
  }

  private static string _m9679932c6538(string root)
  {
    return RMCProfileCacheNodea4fdbc._f3d50cb28f418 + root;
  }

  private static string _m6f913f040366(string typeName)
  {
    return RMCProfileCacheNodea4fdbc._f79c17cc6a4c3 + typeName;
  }

  private static string _meb83f3788501(string root)
  {
    return RMCProfileCacheNodea4fdbc._f27d4cfc16b94 + root;
  }

  private static string _mae4dbb10f5ce(string typeName)
  {
    return RMCProfileCacheNodea4fdbc._f368a646b8c1a + typeName;
  }

  private static string _m1ded9fb8a317(string cvar)
  {
    return RMCProfileCacheNodea4fdbc._fbabb65f960db + cvar;
  }

  private static string _m58fbe73f1311(string typeName)
  {
    return RMCProfileCacheNodea4fdbc._f71a9fdde23b9 + typeName;
  }

  private static string _m0ac020f69fd1(string typeName)
  {
    return RMCProfileCacheNodea4fdbc._fcceb100712a8 + typeName;
  }

  private static string _m80d572865755(string typeName)
  {
    return RMCProfileCacheNodea4fdbc._fb0f65a50afd6 + typeName;
  }

  private static string _m1bde7db0783d(int total)
  {
    return RMCProfileCacheNodea4fdbc._fcaf4ea4d7451 + total.ToString();
  }

  private static string _mad61b1654a92(int publicCount, int rawCount)
  {
    return $"{RMCProfileCacheNodea4fdbc._f1cc544d8e686}{publicCount.ToString()}->{rawCount.ToString()}";
  }

  private static string _m07020afdc1be(int runtimeCount, int dependencyCount)
  {
    return $"{RMCProfileCacheNodea4fdbc._f0f74a5646064}{runtimeCount.ToString()}->{dependencyCount.ToString()}";
  }

  private static string _me1ece98ebcd2(string typeName)
  {
    return RMCProfileCacheNodea4fdbc._feee0fded8d16 + typeName;
  }

  private static string _m26a812a5e361(string command)
  {
    return RMCProfileCacheNodea4fdbc._f38f7086e3c44 + command;
  }

  private static string _m87f6dddff8d8(string command)
  {
    return RMCProfileCacheNodea4fdbc._fc999dec354a0 + command;
  }

  private void _md1b14b21cf2e(HashSet<int> target, IReadOnlyList<int> values)
  {
    target.Clear();
    for (int index = 0; index < values.Count; ++index)
    {
      if (values[index] != 0)
        target.Add(values[index]);
    }
  }

  private bool _ma8b82ec93f4d(string value, HashSet<int> target)
  {
    return target.Count != 0 && !string.IsNullOrWhiteSpace(value) && target.Contains(RMCWeaponProfileRuleHash.Compute(value, this._f33e87ad587f1));
  }

  private static string[] _m54d84b860464(string payload)
  {
    return RMCProfileCacheNodea4fdbc._mb7a2373f4842(payload).Split('\n', StringSplitOptions.RemoveEmptyEntries);
  }

  private static (string Marker, string TypeName)[] _m17a46415d76f(string payload)
  {
    string[] strArray = RMCProfileCacheNodea4fdbc._m54d84b860464(payload);
    (string, string)[] valueTupleArray1 = new (string, string)[strArray.Length];
    for (int index1 = 0; index1 < strArray.Length; ++index1)
    {
      int length = strArray[index1].IndexOf('|');
      if (length > 0 && length < strArray[index1].Length - 1)
      {
        (string, string)[] valueTupleArray2 = valueTupleArray1;
        int index2 = index1;
        string str1 = strArray[index1].Substring(0, length);
        string str2 = strArray[index1];
        int startIndex = length + 1;
        string str3 = str2.Substring(startIndex, str2.Length - startIndex);
        (_, _) = (str1, str3);
        (string, string) valueTuple;
        valueTupleArray2[index2] = valueTuple;
      }
    }
    return valueTupleArray1;
  }

  private static byte[] _mf849c854c067()
  {
    byte[] digest = RMCWeaponProfileDigest.ComputeDigest((ReadOnlySpan<byte>) Encoding.UTF8.GetBytes("RMC14_LEX_v3_2026"));
    byte[] numArray = new byte[16 /*0x10*/];
    byte[] destinationArray = numArray;
    Array.Copy((Array) digest, (Array) destinationArray, 16 /*0x10*/);
    return numArray;
  }

  private static string _mb7a2373f4842(string payload)
  {
    byte[] bytes = Convert.FromBase64String(payload);
    byte[] f839c2b9a635d = RMCProfileCacheNodea4fdbc._f839c2b9a635d;
    for (int index = 0; index < bytes.Length; ++index)
      bytes[index] ^= f839c2b9a635d[index % 16 /*0x10*/];
    return Encoding.UTF8.GetString(bytes);
  }

  private void _mcfe0264c8fc9(List<string> markers, IReadOnlyList<string> managedModules)
  {
    if (this._f15383f3913b9.CurTime >= this._fffff5ff627ac || this._fa19b40661606.Count == 0)
    {
      this._fa19b40661606.Clear();
      this._mdde49dc55acc(this._fa19b40661606, managedModules);
      this._mf483a09210ff(this._fa19b40661606);
      this._m3ca11ea8b54a(this._fa19b40661606);
      this._md581ac1bd2c7(this._fa19b40661606);
      this._fffff5ff627ac = this._f15383f3913b9.CurTime + TimeSpan.FromSeconds((double) Math.Clamp(this._f2ae21401ded7 * 6f, 30f, 180f));
    }
    for (int index = 0; index < this._fa19b40661606.Count; ++index)
      this._me36d689c5b47(markers, this._fa19b40661606[index]);
  }

  private void _mdde49dc55acc(List<string> markers, IReadOnlyList<string> managedModules)
  {
    List<string> items1 = this._mb9ab4fb781a2();
    List<string> items2 = this._mbac54ec30db7();
    if (items2.Count == 0)
      return;
    if (items1.Count != items2.Count)
      this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m283721b78071(items1.Count, items2.Count));
    int num = 0;
    for (int index = 0; index < items2.Count; ++index)
    {
      string moduleName = items2[index];
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items1, moduleName) && RMCProfileCacheNodea4fdbc._mb80787549be2(moduleName))
      {
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._ma405f988c325(moduleName));
        ++num;
        if (num >= 8)
          break;
      }
    }
    for (int index = 0; index < items1.Count; ++index)
    {
      string moduleName = items1[index];
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items2, moduleName) && RMCProfileCacheNodea4fdbc._mb80787549be2(moduleName))
      {
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._ma405f988c325(moduleName));
        ++num;
        if (num >= 8)
          break;
      }
    }
  }

  private void _mf483a09210ff(List<string> markers)
  {
    List<string> items1 = this._m8b3e0c889176();
    List<string> items2 = this._mc98657ee42aa();
    if (items2.Count == 0 && items1.Count == 0)
      return;
    if (items1.Count != items2.Count && (items1.Count > 0 || items2.Count > 0))
      this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m46efcd1d0fa6(items1.Count, items2.Count));
    int num = 0;
    for (int index = 0; index < items2.Count; ++index)
    {
      string moduleName = items2[index];
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items1, moduleName) && RMCProfileCacheNodea4fdbc._mb80787549be2(moduleName))
      {
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m2cb986304ff7(moduleName));
        ++num;
        if (num >= 8)
          break;
      }
    }
    for (int index = 0; index < items1.Count; ++index)
    {
      string moduleName = items1[index];
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items2, moduleName) && RMCProfileCacheNodea4fdbc._mb80787549be2(moduleName))
      {
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m2cb986304ff7(moduleName));
        ++num;
        if (num >= 8)
          break;
      }
    }
    Type type;
    if (RMCProfileCacheNodea4fdbc._m9306a10f12fe(items1, RMCProfileCacheNodea4fdbc._fc13d14519f9e) && !this._m83d249542a76(RMCProfileCacheNodea4fdbc._fef9519d147e1, out type))
    {
      this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._f59e318fb9bf6);
    }
    else
    {
      if (!this._m83d249542a76(RMCProfileCacheNodea4fdbc._fef9519d147e1, out type) || RMCProfileCacheNodea4fdbc._m9306a10f12fe(items1, RMCProfileCacheNodea4fdbc._fc13d14519f9e) || RMCProfileCacheNodea4fdbc._m9306a10f12fe(items2, RMCProfileCacheNodea4fdbc._fc13d14519f9e))
        return;
      this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._f59e318fb9bf6);
    }
  }

  private void _md581ac1bd2c7(List<string> markers)
  {
    List<string> stringList1 = new List<string>();
    List<string> stringList2 = new List<string>();
    List<string> stringList3 = new List<string>();
    this._me71f4a41897f(stringList1, (IEnumerable<Assembly>) this._fe5fec0d033c0.Assemblies, true);
    this._me71f4a41897f(stringList2, this._f790dcf8ed182.LoadedModules, true);
    List<string> items1 = this._m8349c8d89b58();
    List<string> stringList4 = new List<string>();
    List<string> stringList5 = new List<string>();
    List<string> stringList6 = new List<string>();
    List<string> stringList7 = new List<string>();
    List<string> stringList8 = new List<string>();
    foreach (string str in stringList1)
    {
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(stringList2, str) && !RMCProfileCacheNodea4fdbc._m9306a10f12fe(items1, str) && RMCProfileCacheNodea4fdbc._mb80787549be2(str))
        stringList4.Add(str);
    }
    foreach (string str in stringList2)
    {
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(stringList1, str) && !RMCProfileCacheNodea4fdbc._m9306a10f12fe(items1, str) && RMCProfileCacheNodea4fdbc._mb80787549be2(str))
        stringList5.Add(str);
    }
    foreach (string str in items1)
    {
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(stringList1, str) && !RMCProfileCacheNodea4fdbc._m9306a10f12fe(stringList2, str) && RMCProfileCacheNodea4fdbc._mb80787549be2(str))
        stringList6.Add(str);
    }
    foreach (string str in stringList2)
    {
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(stringList1, str) && RMCProfileCacheNodea4fdbc._m9306a10f12fe(items1, str))
        stringList7.Add(str);
    }
    foreach (string str in stringList1)
    {
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(stringList2, str) && RMCProfileCacheNodea4fdbc._m9306a10f12fe(items1, str))
        stringList8.Add(str);
    }
    for (int index = 0; index < Math.Min(3, stringList4.Count); ++index)
      this._me36d689c5b47(markers, "msv:reflect-only:" + stringList4[index]);
    for (int index = 0; index < Math.Min(3, stringList5.Count); ++index)
      this._me36d689c5b47(markers, "msv:loader-only:" + stringList5[index]);
    for (int index = 0; index < Math.Min(3, stringList6.Count); ++index)
      this._me36d689c5b47(markers, "msv:resource-only:" + stringList6[index]);
    for (int index = 0; index < Math.Min(3, stringList7.Count); ++index)
      this._me36d689c5b47(markers, "msv:reflect-miss:" + stringList7[index]);
    for (int index = 0; index < Math.Min(3, stringList8.Count); ++index)
      this._me36d689c5b47(markers, "msv:loader-miss:" + stringList8[index]);
    int num1 = stringList4.Count + stringList5.Count + stringList6.Count + stringList7.Count + stringList8.Count;
    if (num1 > 0)
      this._me36d689c5b47(markers, $"msv:drift-total:{num1}");
    List<string> items2 = this._mdfeca939925a();
    List<string> stringList9 = this._mb6791d056437();
    List<string> stringList10 = new List<string>();
    IDependencyCollection instance = IoCManager.Instance;
    foreach (Type c in (IEnumerable<Type>) ((instance != null ? (object) instance.GetRegisteredTypes() : (object) null) ?? (object) Array.Empty<Type>()))
    {
      if (typeof (EntitySystem).IsAssignableFrom(c))
      {
        string fullName = c.FullName;
        if (!string.IsNullOrWhiteSpace(fullName))
          stringList10.Add(fullName);
      }
    }
    int num2 = 0;
    foreach (string str in stringList9)
    {
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items2, str))
        ++num2;
    }
    foreach (string str in stringList10)
    {
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items2, str))
        ++num2;
    }
    if (num2 > 0)
      this._me36d689c5b47(markers, $"msv:sys-drift:{num2}");
    this._med105c1731c4(markers, stringList1, stringList2);
  }

  private void _med105c1731c4(
    List<string> markers,
    List<string> reflectionModules,
    List<string> modLoaderModules)
  {
    int num1 = 0;
    int num2 = 0;
    foreach (string reflectionModule in reflectionModules)
    {
      if (reflectionModule.StartsWith("Content.", StringComparison.OrdinalIgnoreCase) || reflectionModule.StartsWith("Robust.", StringComparison.OrdinalIgnoreCase))
        ++num1;
    }
    foreach (string modLoaderModule in modLoaderModules)
    {
      if (modLoaderModule.StartsWith("Content.", StringComparison.OrdinalIgnoreCase) || modLoaderModule.StartsWith("Robust.", StringComparison.OrdinalIgnoreCase))
        ++num2;
    }
    if (Math.Abs(num1 - num2) <= 2)
      return;
    this._me36d689c5b47(markers, $"msv:count-gap:{num1}->{num2}");
  }

  private void _m3ca11ea8b54a(List<string> markers)
  {
    List<string> stringList = new List<string>();
    RMCProfileCacheNodea4fdbc._me106091e579d(stringList, (IReadOnlyList<string>) this._mb9ab4fb781a2());
    RMCProfileCacheNodea4fdbc._me106091e579d(stringList, (IReadOnlyList<string>) this._mbac54ec30db7());
    List<string> values1 = new List<string>();
    List<string> values2 = new List<string>();
    List<string> values3 = new List<string>();
    List<string> values4 = new List<string>();
    List<string> values5 = new List<string>();
    try
    {
      foreach (Type allType in this._fe5fec0d033c0.FindAllTypes())
      {
        string fullName1 = allType.FullName;
        if (!string.IsNullOrWhiteSpace(fullName1))
        {
          string fullName2 = this._m17038c921b89(fullName1);
          if (fullName2 != null)
          {
            string assemblyName;
            try
            {
              assemblyName = this._m17038c921b89(allType.Assembly.GetName().Name ?? "unknown");
            }
            catch
            {
              assemblyName = (string) null;
            }
            if (!RMCProfileCacheNodea4fdbc._m4f19a97bd79e(fullName2, assemblyName))
            {
              string str1 = RMCProfileCacheNodea4fdbc._m6b857fc06ba1(fullName2);
              string str2 = RMCProfileCacheNodea4fdbc._m7c41758240c3(fullName2);
              if (str2 != null)
              {
                bool trustedAssembly = RMCProfileCacheNodea4fdbc._m245ce8244562(assemblyName, (IReadOnlyList<string>) stringList);
                bool flag = RMCProfileCacheNodea4fdbc._mce235f4303db(fullName2, assemblyName, trustedAssembly);
                if ((!trustedAssembly || str1 != null) && !(flag & trustedAssembly))
                {
                  if (assemblyName != null && !trustedAssembly)
                    RMCProfileCacheNodea4fdbc._m813f57f49351(values1, assemblyName, 4);
                  if (str1 != null && !flag)
                    RMCProfileCacheNodea4fdbc._m813f57f49351(values2, str1, 6);
                  string str3 = assemblyName == null ? str2 : $"{assemblyName}::{str2}";
                  RMCProfileCacheNodea4fdbc._m813f57f49351(values3, str3, 8);
                  if (str1 != null && this._ma8b82ec93f4d(str1, this._f0a4133b5c029))
                    RMCProfileCacheNodea4fdbc._m813f57f49351(values4, str1, 4);
                  if (str1 != null && this._ma8b82ec93f4d(str2, this._f2793be76c6cd))
                    RMCProfileCacheNodea4fdbc._m813f57f49351(values5, $"{str1}.{str2}", 8);
                }
              }
            }
          }
        }
      }
    }
    catch
    {
    }
    for (int index = 0; index < values1.Count; ++index)
      this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._mfbc6cf3db6f2(values1[index]));
    for (int index = 0; index < values2.Count; ++index)
      this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m9679932c6538(values2[index]));
    for (int index = 0; index < values3.Count; ++index)
      this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._m6f913f040366(values3[index]));
    for (int index = 0; index < values4.Count; ++index)
      this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._meb83f3788501(values4[index]));
    for (int index = 0; index < values5.Count; ++index)
      this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._mae4dbb10f5ce(values5[index]));
  }

  private static void _me106091e579d(List<string> baseline, IReadOnlyList<string> values)
  {
    for (int index = 0; index < values.Count; ++index)
    {
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(baseline, values[index]))
        baseline.Add(values[index]);
    }
  }

  private static bool _m021eb2c08475(string value, IReadOnlyList<string> prefixes)
  {
    for (int index = 0; index < prefixes.Count; ++index)
    {
      if (value.StartsWith(prefixes[index], StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }

  private static bool _m245ce8244562(string? assemblyName, IReadOnlyList<string> allowedAssemblies)
  {
    if (assemblyName == null)
      return false;
    return RMCProfileCacheNodea4fdbc._mce06cb7f4481(allowedAssemblies, assemblyName) || RMCProfileCacheNodea4fdbc._m021eb2c08475(assemblyName, (IReadOnlyList<string>) RMCProfileCacheNodea4fdbc._fdecff0bc9991);
  }

  private static bool _mce235f4303db(string fullName, string? assemblyName, bool trustedAssembly)
  {
    if (RMCProfileCacheNodea4fdbc._m021eb2c08475(fullName, (IReadOnlyList<string>) RMCProfileCacheNodea4fdbc._feec691af0c47))
      return true;
    if (!trustedAssembly || string.IsNullOrWhiteSpace(assemblyName))
      return false;
    return string.Equals(fullName, assemblyName, StringComparison.OrdinalIgnoreCase) || fullName.StartsWith(assemblyName + ".", StringComparison.OrdinalIgnoreCase);
  }

  private static string? _m6b857fc06ba1(string fullName)
  {
    int length = fullName.IndexOf('.');
    return length <= 0 ? (string) null : fullName.Substring(0, length).Trim();
  }

  private static string? _m7c41758240c3(string fullName)
  {
    string str1 = fullName;
    int num = str1.LastIndexOf('.');
    if (num >= 0 && num < str1.Length - 1)
    {
      string str2 = str1;
      int startIndex = num + 1;
      str1 = str2.Substring(startIndex, str2.Length - startIndex);
    }
    int length1 = str1.IndexOf('+');
    if (length1 > 0)
      str1 = str1.Substring(0, length1);
    int length2 = str1.IndexOf('`');
    if (length2 > 0)
      str1 = str1.Substring(0, length2);
    string str3 = str1.Trim();
    return str3.Length != 0 ? str3 : (string) null;
  }

  private static bool _m4f19a97bd79e(string fullName, string? assemblyName)
  {
    if (assemblyName != null && string.Equals(assemblyName, "CompiledRobustXaml", StringComparison.OrdinalIgnoreCase))
      return true;
    string str = RMCProfileCacheNodea4fdbc._m7c41758240c3(fullName);
    if (str == null)
      return false;
    return str.StartsWith('<') || str.Contains("AnonymousType", StringComparison.OrdinalIgnoreCase) || str.Contains("InlineArray", StringComparison.OrdinalIgnoreCase) || str.Contains("ReadOnlyArray", StringComparison.OrdinalIgnoreCase) || str.Contains("ReadOnlySingleElementList", StringComparison.OrdinalIgnoreCase) || str.Contains("PrivateImplementationDetails", StringComparison.OrdinalIgnoreCase) || str.StartsWith("__StaticArrayInitTypeSize", StringComparison.OrdinalIgnoreCase);
  }

  private static void _m813f57f49351(List<string> values, string value, int limit)
  {
    if (RMCProfileCacheNodea4fdbc._m9306a10f12fe(values, value))
      return;
    values.Add(value);
    if (values.Count <= limit)
      return;
    values.RemoveRange(limit, values.Count - limit);
  }

  internal bool _m49a1ad791f32(out RMCDrawSkewSlice91dac4 challenge)
  {
    if (!this._fc597ad70fd5a.HasValue)
    {
      challenge = new RMCDrawSkewSlice91dac4();
      return false;
    }
    challenge = this._fc597ad70fd5a.Value;
    return true;
  }

  private void _ma279e968b42b(
    byte grid,
    byte cellX,
    byte cellY,
    byte sizePercent,
    byte red,
    byte green,
    byte blue)
  {
    this._fc597ad70fd5a = new RMCDrawSkewSlice91dac4?(new RMCDrawSkewSlice91dac4(grid, cellX, cellY, sizePercent, new Color(red, green, blue, byte.MaxValue)));
  }

  private void _mff5024a79308() => this._fc597ad70fd5a = new RMCDrawSkewSlice91dac4?();

  private Task _mc966ddcbce9f(bool armed)
  {
    // ISSUE: variable of a compiler-generated type
    RMCProfileCacheNodea4fdbc._t6228e2737c62 stateMachine;
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003Et__builder = AsyncTaskMethodBuilder.Create();
    // ISSUE: reference to a compiler-generated field
    stateMachine.armed = armed;
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    stateMachine.\u003C\u003Et__builder.Start<RMCProfileCacheNodea4fdbc._t6228e2737c62>(ref stateMachine);
    // ISSUE: reference to a compiler-generated field
    return stateMachine.\u003C\u003Et__builder.Task;
  }

  private bool _m2f80ae0722ad() => false;

  private void _m09b7b0aefabe(List<string> markers)
  {
    for (int index = 0; index < RMCProfileCacheNodea4fdbc._f0eccc63a6b39.Length; ++index)
    {
      if (this._m83d249542a76(RMCProfileCacheNodea4fdbc._f0eccc63a6b39[index].TypeName, out Type _))
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._f0eccc63a6b39[index].Marker);
    }
  }

  private List<string> _mb9ab4fb781a2()
  {
    List<string> modules = new List<string>();
    this._me71f4a41897f(modules, (IEnumerable<Assembly>) this._fe5fec0d033c0.Assemblies, true);
    this._me71f4a41897f(modules, this._f790dcf8ed182.LoadedModules, true);
    modules.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return modules;
  }

  private List<string> _m8349c8d89b58()
  {
    List<string> stringList = new List<string>();
    foreach (ResPath file in this._fdb0a9219e17c.ContentFindFiles("/Assemblies"))
    {
      if (string.Equals(((ResPath) ref file).Extension, "dll", StringComparison.OrdinalIgnoreCase))
      {
        string str = this._m17038c921b89(((ResPath) ref file).FilenameWithoutExtension);
        if (str != null)
        {
          bool flag = false;
          for (int index = 0; index < RMCProfileCacheNodea4fdbc._f8dd17bd80062.Length; ++index)
          {
            if (str.StartsWith(RMCProfileCacheNodea4fdbc._f8dd17bd80062[index], StringComparison.OrdinalIgnoreCase))
            {
              flag = true;
              break;
            }
          }
          if ((flag || RMCProfileCacheNodea4fdbc._mb80787549be2(str)) && !RMCProfileCacheNodea4fdbc._m9306a10f12fe(stringList, str))
            stringList.Add(str);
        }
      }
    }
    if (stringList.Count == 0)
      this._me71f4a41897f(stringList, this._f790dcf8ed182.LoadedModules, true);
    stringList.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return stringList;
  }

  private List<string> _m19e860529a63() => this._m8349c8d89b58();

  private List<string> _m520f7c37c97d() => this._me5d6bc10f7e8();

  private List<string> _mb6791d056437() => this._me5d6bc10f7e8();

  private List<string> _me5d6bc10f7e8()
  {
    List<string> types = new List<string>();
    foreach (Type registeredType in this._fc6f45fbf437d.DependencyCollection.GetRegisteredTypes())
    {
      object obj;
      if (typeof (EntitySystem).IsAssignableFrom(registeredType) && this._fc6f45fbf437d.TryGetEntitySystem(registeredType, ref obj) && !(obj?.GetType() != registeredType))
        this._m29ef9bfc87eb(types, (IEnumerable<Type>) new Type[1]
        {
          registeredType
        });
    }
    types.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return types;
  }

  private bool _m83d249542a76(string typeSpecification, out Type? type)
  {
    type = (Type) null;
    if (string.IsNullOrWhiteSpace(typeSpecification))
      return false;
    try
    {
      type = Type.GetType(typeSpecification, false);
      if (type != (Type) null)
        return true;
    }
    catch
    {
    }
    string str = typeSpecification.Split(',', 2, StringSplitOptions.TrimEntries)[0];
    try
    {
      if (this._fe5fec0d033c0.TryLooseGetType(str, ref type))
        return true;
    }
    catch
    {
    }
    type = (Type) null;
    return false;
  }

  private void _m23e52b444b79(List<string> markers)
  {
    try
    {
      int num1 = 0;
      int num2 = 0;
      foreach (Overlay allOverlay in this._fad8fd92ff30f.AllOverlays)
      {
        ++num1;
        if (!this._m6630ed729cc3(allOverlay.GetType().FullName ?? string.Empty))
          ++num2;
      }
      if (num2 > 0)
        this._me36d689c5b47(markers, $"overlay:custom-count:{num2}");
      if (num1 <= 100)
        return;
      this._me36d689c5b47(markers, $"overlay:total-count:{num1}");
    }
    catch
    {
    }
  }

  private void _m6ee162e1c432(List<string> markers)
  {
    string[] strArray = RMCProfileCacheNodea4fdbc._m5630bf7d8487();
    for (int index = 0; index < strArray.Length; ++index)
    {
      if (this._m83d249542a76(strArray[index], out Type _))
        this._me36d689c5b47(markers, "decoy:system-loaded:" + RMCProfileCacheNodea4fdbc._m96129808ba9a(strArray[index]));
    }
  }

  private static string[] _m5630bf7d8487()
  {
    return new string[24]
    {
      "Content.Client.Cheats.NoRecoilSystem",
      "Content.Client.Cheats.AimbotSystem",
      "Content.Client.Cheats.EspSystem",
      "Content.Client.Cheats.SpeedHackSystem",
      "Content.Client.Cheats.WallHackSystem",
      "Content.Client.Cheats.GodModeSystem",
      "Content.Client.Cheats.NoClipSystem",
      "Content.Client.Cheats.InfiniteHealthSystem",
      "Content.Client.Cheats.RadarSystem",
      "Content.Client.Hacks.TargetLockSystem",
      "Content.Client.Hacks.AutoAimSystem",
      "Content.Client.Hacks.InfiniteAmmoSystem",
      "Content.Client.Hacks.EntityHiderSystem",
      "Content.Client.Hacks.AssemblyHiderSystem",
      "Content.Client.Hacks.TypeHiderSystem",
      "Content.Client.Hacks.ModuleHiderSystem",
      "Content.Client.Mods.CheatMenuSystem",
      "Content.Client.Mods.HackLoaderSystem",
      "Content.Client.Mods.InjectorSystem",
      "Content.Client.Mods.PatcherSystem",
      "Content.Client.External.MarseySystem",
      "Content.Client.External.CypherSystem",
      "Content.Client.External.CerberusSystem",
      "Content.Client.External.TealSystem"
    };
  }

  private static string _m96129808ba9a(string fullName)
  {
    int num = fullName.LastIndexOf('.');
    if (num < 0 || num >= fullName.Length - 1)
      return fullName;
    string str = fullName;
    int startIndex = num + 1;
    return str.Substring(startIndex, str.Length - startIndex);
  }

  private void _m7a6ea419c68e(List<string> markers)
  {
    try
    {
      int num = 0;
      foreach (Type allRegisteredType in this.EntityManager.ComponentFactory.AllRegisteredTypes)
      {
        string str = allRegisteredType.FullName ?? string.Empty;
        if (!this._m6630ed729cc3(str))
        {
          ++num;
          if (num <= 3)
            this._me36d689c5b47(markers, "component:custom:" + RMCProfileCacheNodea4fdbc._m96129808ba9a(str));
        }
      }
      if (num <= 3)
        return;
      this._me36d689c5b47(markers, $"component:custom-total:{num}");
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
      foreach (KeyValuePair<string, IConsoleCommand> availableCommand in (IEnumerable<KeyValuePair<string, IConsoleCommand>>) this._fa860cc85cf5f.AvailableCommands)
      {
        Type type = availableCommand.Value?.GetType();
        if (!(type == (Type) null) && !this._m6630ed729cc3(type.FullName ?? string.Empty))
        {
          ++num;
          if (num <= 3)
            this._me36d689c5b47(markers, "cmd:custom:" + availableCommand.Key);
        }
      }
      if (num <= 3)
        return;
      this._me36d689c5b47(markers, $"cmd:custom-total:{num}");
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
    if (this._f9a741dded701.Count == 0)
      return;
    try
    {
      foreach (string key in this._f9a741dded701)
      {
        if (this._fa860cc85cf5f.AvailableCommands.ContainsKey(key))
          this._me36d689c5b47(markers, "probe:dynamic-cmd:" + key);
      }
    }
    catch
    {
    }
  }

  internal void _m28fae0c6bf27(List<string> commands)
  {
    this._f9a741dded701.Clear();
    foreach (string command in commands)
    {
      if (!string.IsNullOrWhiteSpace(command))
        this._f9a741dded701.Add(command);
    }
  }

  private List<string> _mbac54ec30db7()
  {
    List<string> items = new List<string>();
    foreach (string str in this._m8349c8d89b58())
    {
      if (!RMCProfileCacheNodea4fdbc._m9306a10f12fe(items, str))
        items.Add(str);
    }
    items.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return items;
  }

  private List<string> _mdfeca939925a()
  {
    List<string> items = new List<string>();
    foreach (Type entitySystemType in this._fc6f45fbf437d.GetEntitySystemTypes())
    {
      string fullName = entitySystemType.FullName;
      if (!string.IsNullOrWhiteSpace(fullName))
      {
        string str = this._m17038c921b89(fullName);
        if (str != null && !RMCProfileCacheNodea4fdbc._m9306a10f12fe(items, str))
          items.Add(str);
      }
    }
    items.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return items;
  }

  private List<string> _m6b0762212f15() => this._m520f7c37c97d();

  private List<string> _m8b3e0c889176()
  {
    List<string> items = new List<string>();
    foreach (Assembly loadedModule in this._f790dcf8ed182.LoadedModules)
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
        string str = this._m17038c921b89(name);
        if (str != null && !RMCProfileCacheNodea4fdbc._m9306a10f12fe(items, str))
          items.Add(str);
      }
    }
    items.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return items;
  }

  private List<string> _mc98657ee42aa()
  {
    List<string> stringList = this._m19e860529a63();
    stringList.Sort((Comparison<string>) ((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase)));
    return stringList;
  }

  private void _me71f4a41897f(
    List<string> modules,
    IEnumerable<Assembly> assemblies,
    bool includeTrustedRoots)
  {
    foreach (Assembly assembly in assemblies)
    {
      string name1;
      try
      {
        name1 = assembly.GetName().Name;
      }
      catch
      {
        continue;
      }
      if (!string.IsNullOrWhiteSpace(name1))
      {
        string name2 = this._m17038c921b89(name1);
        if (name2 != null && this._mfd91750bcf7c(name2, includeTrustedRoots) && !RMCProfileCacheNodea4fdbc._m9306a10f12fe(modules, name2))
          modules.Add(name2);
      }
    }
  }

  private void _m29ef9bfc87eb(List<string> types, IEnumerable<Type> values)
  {
    foreach (Type type in values)
    {
      string fullName = type.FullName;
      if (!string.IsNullOrWhiteSpace(fullName))
      {
        string str = this._m17038c921b89(fullName);
        if (str != null && !RMCProfileCacheNodea4fdbc._m9306a10f12fe(types, str))
          types.Add(str);
      }
    }
  }

  private bool _mfd91750bcf7c(string name, bool includeTrustedRoots)
  {
    return includeTrustedRoots && (name.StartsWith("Content.", StringComparison.OrdinalIgnoreCase) || name.StartsWith("Robust.", StringComparison.OrdinalIgnoreCase)) || RMCProfileCacheNodea4fdbc._mb80787549be2(name);
  }

  private void _m33432bf71416(List<string> markers)
  {
    for (int index = 0; index < RMCProfileCacheNodea4fdbc._fa2ea7b4dd80d.Length; ++index)
    {
      if (this._m83d249542a76(RMCProfileCacheNodea4fdbc._fa2ea7b4dd80d[index].TypeName, out Type _))
        this._me36d689c5b47(markers, RMCProfileCacheNodea4fdbc._mefd73cb8f295(RMCProfileCacheNodea4fdbc._fa2ea7b4dd80d[index].Marker));
    }
  }

  private readonly struct _t5e239d9f0f2c(TimeSpan At, Vector2 Position) : 
    IEquatable<RMCProfileCacheNodea4fdbc._t5e239d9f0f2c>
  {
    public TimeSpan At { get; init; } = At;

    public Vector2 Position { get; init; } = Position;

    [CompilerGenerated]
    public override 
    #nullable disable
    string ToString()
    {
      StringBuilder builder = new StringBuilder();
      builder.Append("ShotLagSample");
      builder.Append(" { ");
      // ISSUE: reference to a compiler-generated method
      if (this._m5d7faa4a870f(builder))
        builder.Append(' ');
      builder.Append('}');
      return builder.ToString();
    }

    [CompilerGenerated]
    public static bool operator !=(
      RMCProfileCacheNodea4fdbc._t5e239d9f0f2c left,
      RMCProfileCacheNodea4fdbc._t5e239d9f0f2c right)
    {
      return !(left == right);
    }

    [CompilerGenerated]
    public static bool operator ==(
      RMCProfileCacheNodea4fdbc._t5e239d9f0f2c left,
      RMCProfileCacheNodea4fdbc._t5e239d9f0f2c right)
    {
      return left.Equals(right);
    }

    [CompilerGenerated]
    public override int GetHashCode()
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return EqualityComparer<TimeSpan>.Default.GetHashCode(this._f19a72e872760) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this._f31a033d354d9);
    }

    [CompilerGenerated]
    public override bool Equals(object obj)
    {
      return obj is RMCProfileCacheNodea4fdbc._t5e239d9f0f2c other && this.Equals(other);
    }

    [CompilerGenerated]
    public bool Equals(RMCProfileCacheNodea4fdbc._t5e239d9f0f2c other)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return EqualityComparer<TimeSpan>.Default.Equals(this._f19a72e872760, other._f19a72e872760) && EqualityComparer<Vector2>.Default.Equals(this._f31a033d354d9, other._f31a033d354d9);
    }

    [CompilerGenerated]
    public void Deconstruct(out TimeSpan At, out Vector2 Position)
    {
      At = this.At;
      Position = this.Position;
    }
  }

  private readonly struct _tb1b24893add2(TimeSpan Until, Vector2 Position) : 
    IEquatable<RMCProfileCacheNodea4fdbc._tb1b24893add2>
  {
    public TimeSpan Until { get; init; } = Until;

    public Vector2 Position { get; init; } = Position;

    [CompilerGenerated]
    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();
      builder.Append("FeedRollbackState");
      builder.Append(" { ");
      // ISSUE: reference to a compiler-generated method
      if (this._m4f001091d223(builder))
        builder.Append(' ');
      builder.Append('}');
      return builder.ToString();
    }

    [CompilerGenerated]
    public static bool operator !=(
      RMCProfileCacheNodea4fdbc._tb1b24893add2 left,
      RMCProfileCacheNodea4fdbc._tb1b24893add2 right)
    {
      return !(left == right);
    }

    [CompilerGenerated]
    public static bool operator ==(
      RMCProfileCacheNodea4fdbc._tb1b24893add2 left,
      RMCProfileCacheNodea4fdbc._tb1b24893add2 right)
    {
      return left.Equals(right);
    }

    [CompilerGenerated]
    public override int GetHashCode()
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return EqualityComparer<TimeSpan>.Default.GetHashCode(this._f9e9eb47faa59) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this._facad74f6c646);
    }

    [CompilerGenerated]
    public override bool Equals(object obj)
    {
      return obj is RMCProfileCacheNodea4fdbc._tb1b24893add2 other && this.Equals(other);
    }

    [CompilerGenerated]
    public bool Equals(RMCProfileCacheNodea4fdbc._tb1b24893add2 other)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return EqualityComparer<TimeSpan>.Default.Equals(this._f9e9eb47faa59, other._f9e9eb47faa59) && EqualityComparer<Vector2>.Default.Equals(this._facad74f6c646, other._facad74f6c646);
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
    public 
    #nullable enable
    byte[] Payload = Array.Empty<byte>();
    public int ChunkSize;
    public int ChunkCount;
    public int NextChunk;
    public float StepSeconds;
    public TimeSpan NextAt;
  }
}
