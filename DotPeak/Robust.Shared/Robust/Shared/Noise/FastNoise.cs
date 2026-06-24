// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Noise.FastNoise
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.Noise;

[Obsolete("Use FastNoiseLite")]
public sealed class FastNoise
{
  private const MethodImplOptions FN_INLINE = MethodImplOptions.AggressiveInlining;
  private const int FN_CELLULAR_INDEX_MAX = 3;
  private int m_seed = 1337;
  private float m_frequency = 0.01f;
  private FastNoise.Interp m_interp = FastNoise.Interp.Quintic;
  private FastNoise.NoiseType m_noiseType = FastNoise.NoiseType.Simplex;
  private int m_octaves = 3;
  private float m_lacunarity = 2f;
  private float m_gain = 0.5f;
  private FastNoise.FractalType m_fractalType;
  private float m_fractalBounding;
  private FastNoise.CellularDistanceFunction m_cellularDistanceFunction;
  private FastNoise.CellularReturnType m_cellularReturnType;
  private FastNoise m_cellularNoiseLookup;
  private int m_cellularDistanceIndex0;
  private int m_cellularDistanceIndex1 = 1;
  private float m_cellularJitter = 0.45f;
  private float m_gradientPerturbAmp = 1f;
  private static readonly FastNoise.Float2[] GRAD_2D = new FastNoise.Float2[8]
  {
    new FastNoise.Float2(-1f, -1f),
    new FastNoise.Float2(1f, -1f),
    new FastNoise.Float2(-1f, 1f),
    new FastNoise.Float2(1f, 1f),
    new FastNoise.Float2(0.0f, -1f),
    new FastNoise.Float2(-1f, 0.0f),
    new FastNoise.Float2(0.0f, 1f),
    new FastNoise.Float2(1f, 0.0f)
  };
  private static readonly FastNoise.Float3[] GRAD_3D = new FastNoise.Float3[16 /*0x10*/]
  {
    new FastNoise.Float3(1f, 1f, 0.0f),
    new FastNoise.Float3(-1f, 1f, 0.0f),
    new FastNoise.Float3(1f, -1f, 0.0f),
    new FastNoise.Float3(-1f, -1f, 0.0f),
    new FastNoise.Float3(1f, 0.0f, 1f),
    new FastNoise.Float3(-1f, 0.0f, 1f),
    new FastNoise.Float3(1f, 0.0f, -1f),
    new FastNoise.Float3(-1f, 0.0f, -1f),
    new FastNoise.Float3(0.0f, 1f, 1f),
    new FastNoise.Float3(0.0f, -1f, 1f),
    new FastNoise.Float3(0.0f, 1f, -1f),
    new FastNoise.Float3(0.0f, -1f, -1f),
    new FastNoise.Float3(1f, 1f, 0.0f),
    new FastNoise.Float3(0.0f, -1f, 1f),
    new FastNoise.Float3(-1f, 1f, 0.0f),
    new FastNoise.Float3(0.0f, -1f, -1f)
  };
  private static readonly FastNoise.Float2[] CELL_2D = new FastNoise.Float2[256 /*0x0100*/]
  {
    new FastNoise.Float2(-0.2700222f, -0.9628541f),
    new FastNoise.Float2(0.386309266f, -0.9223693f),
    new FastNoise.Float2(0.04444859f, -0.9990117f),
    new FastNoise.Float2(-0.599252343f, -0.800560236f),
    new FastNoise.Float2(-0.781928f, 0.62336874f),
    new FastNoise.Float2(0.9464672f, 0.322799921f),
    new FastNoise.Float2(-0.6514147f, -0.7587219f),
    new FastNoise.Float2(0.937847257f, 0.347048372f),
    new FastNoise.Float2(-0.8497876f, -0.527125239f),
    new FastNoise.Float2(-0.879042566f, 0.476743251f),
    new FastNoise.Float2(-0.8923003f, -0.451442361f),
    new FastNoise.Float2(-0.379844427f, -0.9250504f),
    new FastNoise.Float2(-0.9951651f, 0.09821638f),
    new FastNoise.Float2(0.7724398f, -0.635088f),
    new FastNoise.Float2(0.757328331f, -0.6530343f),
    new FastNoise.Float2(-0.9928005f, -0.119780056f),
    new FastNoise.Float2(-0.05326657f, 0.998580337f),
    new FastNoise.Float2(0.975425363f, -0.220330074f),
    new FastNoise.Float2(-0.766501844f, 0.642242134f),
    new FastNoise.Float2(0.9916367f, 0.129060611f),
    new FastNoise.Float2(-0.994696856f, 0.102850378f),
    new FastNoise.Float2(-0.537920535f, -0.8429955f),
    new FastNoise.Float2(0.502281547f, -0.864704132f),
    new FastNoise.Float2(0.455982149f, -0.8899889f),
    new FastNoise.Float2(-0.8659131f, -0.50019443f),
    new FastNoise.Float2(0.08794584f, -0.9961253f),
    new FastNoise.Float2(-0.5051685f, 0.8630207f),
    new FastNoise.Float2(0.7753185f, -0.6315704f),
    new FastNoise.Float2(-0.692194462f, 0.72171104f),
    new FastNoise.Float2(-0.519165933f, -0.854673445f),
    new FastNoise.Float2(0.8978623f, -0.4402764f),
    new FastNoise.Float2(-0.170677409f, 0.985326946f),
    new FastNoise.Float2(-0.935343f, -0.353742063f),
    new FastNoise.Float2(-0.999240458f, 0.0389674678f),
    new FastNoise.Float2(-0.2882064f, -0.9575683f),
    new FastNoise.Float2(-0.966381133f, 0.2571138f),
    new FastNoise.Float2(-0.875971437f, -0.482363015f),
    new FastNoise.Float2(-0.8303123f, -0.557298362f),
    new FastNoise.Float2(0.0511013381f, -0.998693466f),
    new FastNoise.Float2(-0.855837345f, -0.517245054f),
    new FastNoise.Float2(0.0988702551f, 0.9951003f),
    new FastNoise.Float2(0.9189016f, 0.394486785f),
    new FastNoise.Float2(-0.243937582f, -0.969790936f),
    new FastNoise.Float2(-0.812140942f, -0.5834613f),
    new FastNoise.Float2(-0.99104315f, 0.133542135f),
    new FastNoise.Float2(0.8492424f, -0.528003156f),
    new FastNoise.Float2(-0.9717839f, -0.235872954f),
    new FastNoise.Float2(0.9949457f, 0.100414209f),
    new FastNoise.Float2(0.6241065f, -0.7813392f),
    new FastNoise.Float2(0.6629103f, 0.748698831f),
    new FastNoise.Float2(-0.7197418f, 0.6942418f),
    new FastNoise.Float2(-0.8143371f, -0.580392241f),
    new FastNoise.Float2(0.104521051f, -0.9945227f),
    new FastNoise.Float2(-0.10659261f, -0.99430275f),
    new FastNoise.Float2(0.445799679f, -0.8951328f),
    new FastNoise.Float2(0.105547406f, 0.99441427f),
    new FastNoise.Float2(-0.9927903f, 0.119864449f),
    new FastNoise.Float2(-0.833436668f, 0.552615047f),
    new FastNoise.Float2(0.9115562f, -0.4111756f),
    new FastNoise.Float2(0.8285545f, -0.55990845f),
    new FastNoise.Float2(0.7217098f, -0.6921958f),
    new FastNoise.Float2(0.494049281f, -0.8694339f),
    new FastNoise.Float2(-0.36523214f, -0.9309165f),
    new FastNoise.Float2(-0.9696607f, 0.244454846f),
    new FastNoise.Float2(0.0892550945f, -0.9960088f),
    new FastNoise.Float2(0.5354071f, -0.8445941f),
    new FastNoise.Float2(-0.105357617f, 0.9944344f),
    new FastNoise.Float2(-0.989028454f, 0.1477251f),
    new FastNoise.Float2(0.004856105f, 0.9999882f),
    new FastNoise.Float2(0.988559842f, 0.150829136f),
    new FastNoise.Float2(0.928612947f, -0.371049821f),
    new FastNoise.Float2(-0.5832394f, -0.8123003f),
    new FastNoise.Float2(0.301520765f, 0.9534596f),
    new FastNoise.Float2(-0.957511067f, 0.288396567f),
    new FastNoise.Float2(0.9715802f, -0.236710548f),
    new FastNoise.Float2(0.2299818f, 0.973194957f),
    new FastNoise.Float2(0.9557638f, -0.2941352f),
    new FastNoise.Float2(0.7409561f, 0.671553433f),
    new FastNoise.Float2(-0.9971514f, -0.07542631f),
    new FastNoise.Float2(0.69057107f, -0.7232645f),
    new FastNoise.Float2(-0.2907137f, -0.9568101f),
    new FastNoise.Float2(0.5912778f, -0.80646795f),
    new FastNoise.Float2(-0.945459247f, -0.3257405f),
    new FastNoise.Float2(0.666445553f, 0.7455537f),
    new FastNoise.Float2(0.6236135f, 0.781732857f),
    new FastNoise.Float2(0.9126994f, -0.408631653f),
    new FastNoise.Float2(-0.8191762f, 0.573541939f),
    new FastNoise.Float2(-0.8812746f, -0.4726046f),
    new FastNoise.Float2(0.995331347f, 0.09651673f),
    new FastNoise.Float2(0.985565066f, -0.169296965f),
    new FastNoise.Float2(-0.8495981f, 0.527430654f),
    new FastNoise.Float2(0.6174854f, -0.786582351f),
    new FastNoise.Float2(0.850815654f, 0.5254643f),
    new FastNoise.Float2(0.998503268f, -0.0546925f),
    new FastNoise.Float2(0.197137162f, -0.980375946f),
    new FastNoise.Float2(0.660785556f, -0.7505747f),
    new FastNoise.Float2(-0.0309749413f, 0.9995202f),
    new FastNoise.Float2(-0.6731661f, 0.739491343f),
    new FastNoise.Float2(-0.719501853f, -0.694490552f),
    new FastNoise.Float2(0.972751141f, 0.2318516f),
    new FastNoise.Float2(0.9997059f, -0.02425069f),
    new FastNoise.Float2(0.442178756f, -0.896926939f),
    new FastNoise.Float2(0.9981351f, -0.0610436723f),
    new FastNoise.Float2(-0.9173661f, -0.398044556f),
    new FastNoise.Float2(-0.81500566f, -0.579453f),
    new FastNoise.Float2(-0.878933132f, 0.476945f),
    new FastNoise.Float2(0.0158605836f, 0.999874234f),
    new FastNoise.Float2(-0.8095465f, 0.5870558f),
    new FastNoise.Float2(-0.9165899f, -0.399828672f),
    new FastNoise.Float2(-0.8023543f, 0.5968481f),
    new FastNoise.Float2(-0.5176738f, 0.855578065f),
    new FastNoise.Float2(-95f * (float) Math.PI / 366f, -0.578840554f),
    new FastNoise.Float2(0.402201027f, -0.915551364f),
    new FastNoise.Float2(-0.9052557f, -0.4248672f),
    new FastNoise.Float2(0.7317446f, 0.681579f),
    new FastNoise.Float2(-0.564763248f, -0.825253f),
    new FastNoise.Float2(-0.8403276f, -0.542078853f),
    new FastNoise.Float2(-0.931428134f, 0.363925248f),
    new FastNoise.Float2(0.523819864f, 0.851829052f),
    new FastNoise.Float2(0.7432804f, -0.66898f),
    new FastNoise.Float2(-0.9853716f, -0.170419738f),
    new FastNoise.Float2(0.460146874f, 0.887842834f),
    new FastNoise.Float2(0.8258554f, 0.563881934f),
    new FastNoise.Float2(0.6182366f, 0.785992f),
    new FastNoise.Float2(0.833150268f, -0.553046644f),
    new FastNoise.Float2(0.150030747f, 0.9886813f),
    new FastNoise.Float2(-0.6623304f, -0.7492119f),
    new FastNoise.Float2(-0.668598652f, 0.743623435f),
    new FastNoise.Float2(0.7025606f, 0.7116239f),
    new FastNoise.Float2(-0.541938961f, -0.840417862f),
    new FastNoise.Float2(-0.338861644f, 0.9408362f),
    new FastNoise.Float2(0.833153f, 0.553042531f),
    new FastNoise.Float2(-0.29897207f, -0.954261839f),
    new FastNoise.Float2(0.2638523f, 0.9645631f),
    new FastNoise.Float2(0.124108739f, -0.9922686f),
    new FastNoise.Float2(-0.7282649f, -0.6852957f),
    new FastNoise.Float2(0.69625f, 0.717799366f),
    new FastNoise.Float2(-0.918353558f, 0.395761f),
    new FastNoise.Float2(-0.6326102f, -0.7744703f),
    new FastNoise.Float2(-0.9331892f, -0.35938552f),
    new FastNoise.Float2(-0.115377933f, -0.993321657f),
    new FastNoise.Float2(0.9514975f, -0.307656556f),
    new FastNoise.Float2(-0.08987977f, -0.9959526f),
    new FastNoise.Float2(0.6678497f, 0.7442962f),
    new FastNoise.Float2(0.795240045f, -0.6062947f),
    new FastNoise.Float2(-0.6462007f, -0.7631675f),
    new FastNoise.Float2(-0.273359865f, 0.961911857f),
    new FastNoise.Float2(0.966959f, -0.254931837f),
    new FastNoise.Float2(-0.9792895f, 0.202465191f),
    new FastNoise.Float2(-0.5369503f, -0.843613863f),
    new FastNoise.Float2(-0.270036459f, -0.9628501f),
    new FastNoise.Float2(-0.6400277f, 0.768351853f),
    new FastNoise.Float2(-0.785453737f, -0.6189204f),
    new FastNoise.Float2(0.0600590557f, -0.9981948f),
    new FastNoise.Float2(-0.0245577041f, 0.9996984f),
    new FastNoise.Float2(-1080f / (521f * Math.PI), 0.7514095f),
    new FastNoise.Float2(-0.625389457f, -0.7803128f),
    new FastNoise.Float2(-0.6210409f, -0.7837782f),
    new FastNoise.Float2(0.8348889f, 0.550418556f),
    new FastNoise.Float2(-0.15922752f, 0.9872419f),
    new FastNoise.Float2(0.836762249f, 0.547566354f),
    new FastNoise.Float2(-0.8675754f, -0.4973057f),
    new FastNoise.Float2(-0.202266261f, -0.97933054f),
    new FastNoise.Float2(0.939919f, 0.341397554f),
    new FastNoise.Float2(0.987740457f, -0.1561049f),
    new FastNoise.Float2(-0.903445542f, 0.428702831f),
    new FastNoise.Float2(0.126980424f, -0.9919052f),
    new FastNoise.Float2(-0.3819601f, 0.924178839f),
    new FastNoise.Float2(0.9754626f, 0.220165253f),
    new FastNoise.Float2(-0.320401579f, -0.947281837f),
    new FastNoise.Float2(-0.9874761f, 0.157768741f),
    new FastNoise.Float2(0.0253534839f, -0.999678552f),
    new FastNoise.Float2(0.4835131f, -0.8753371f),
    new FastNoise.Float2(-0.28508f, -0.9585037f),
    new FastNoise.Float2(-0.06805516f, -0.997681558f),
    new FastNoise.Float2(-0.7885244f, -0.615003467f),
    new FastNoise.Float2(0.3185392f, -0.9479097f),
    new FastNoise.Float2(0.8880043f, 0.459835142f),
    new FastNoise.Float2(0.647692144f, -0.761902153f),
    new FastNoise.Float2(0.982024133f, 0.188755423f),
    new FastNoise.Float2(0.935727537f, -0.352723718f),
    new FastNoise.Float2(-0.889489532f, 0.456955522f),
    new FastNoise.Float2(0.7922791f, 0.6101588f),
    new FastNoise.Float2(0.748381853f, 0.663268149f),
    new FastNoise.Float2(-0.728893f, -0.684627652f),
    new FastNoise.Float2(0.8729033f, -0.487893283f),
    new FastNoise.Float2(0.8288346f, 0.5594937f),
    new FastNoise.Float2(0.08074567f, 0.996734738f),
    new FastNoise.Float2(0.979914844f, -0.1994165f),
    new FastNoise.Float2(-0.5807307f, -0.814095736f),
    new FastNoise.Float2(-0.470004976f, -0.8826638f),
    new FastNoise.Float2(0.2409493f, 0.9705377f),
    new FastNoise.Float2(0.9437817f, -0.330569416f),
    new FastNoise.Float2(-0.892799854f, -0.45045355f),
    new FastNoise.Float2(-0.806962252f, 0.590603054f),
    new FastNoise.Float2(0.0625897348f, 0.998039365f),
    new FastNoise.Float2(-0.931259751f, 0.364355981f),
    new FastNoise.Float2(0.577744961f, 0.816217363f),
    new FastNoise.Float2(-0.3360096f, -0.9418586f),
    new FastNoise.Float2(0.697932065f, -0.716163933f),
    new FastNoise.Float2(-0.00200815732f, -0.999998f),
    new FastNoise.Float2(-0.182729438f, -0.983163238f),
    new FastNoise.Float2(-0.6523912f, 0.7578824f),
    new FastNoise.Float2(-0.430262685f, -0.9027037f),
    new FastNoise.Float2(-0.9985126f, -0.0545209125f),
    new FastNoise.Float2(-0.0102810217f, -0.999947131f),
    new FastNoise.Float2(-0.494607121f, 0.869116664f),
    new FastNoise.Float2(-0.299935f, 0.953959644f),
    new FastNoise.Float2(0.8165472f, 0.5772787f),
    new FastNoise.Float2(0.269746035f, 0.9629315f),
    new FastNoise.Float2(-0.7306287f, -0.682774961f),
    new FastNoise.Float2(-0.7590952f, -0.650979638f),
    new FastNoise.Float2(-0.9070538f, 0.4210146f),
    new FastNoise.Float2(-0.5104861f, -0.859886f),
    new FastNoise.Float2(0.861335039f, 0.5080373f),
    new FastNoise.Float2(0.500788152f, -0.8655699f),
    new FastNoise.Float2(-0.6541582f, 0.7563578f),
    new FastNoise.Float2(-0.838275552f, -0.54524684f),
    new FastNoise.Float2(0.6940071f, 0.7199682f),
    new FastNoise.Float2(0.06950936f, 0.9975813f),
    new FastNoise.Float2(0.170294225f, -0.9853933f),
    new FastNoise.Float2(0.269597322f, 0.9629731f),
    new FastNoise.Float2(0.551961243f, -0.833869755f),
    new FastNoise.Float2(0.2256575f, -0.9742067f),
    new FastNoise.Float2(0.421526283f, -0.9068162f),
    new FastNoise.Float2(0.488187343f, -0.872738838f),
    new FastNoise.Float2(-0.3683855f, -0.929673135f),
    new FastNoise.Float2(-0.982539058f, 0.18605645f),
    new FastNoise.Float2(0.812564731f, 0.582871f),
    new FastNoise.Float2(0.3196461f, -0.947537f),
    new FastNoise.Float2(0.9570914f, 0.289786249f),
    new FastNoise.Float2(-0.6876655f, -0.7260276f),
    new FastNoise.Float2(-0.9988771f, -0.04737673f),
    new FastNoise.Float2(-0.1250179f, 0.9921545f),
    new FastNoise.Float2(-0.828013361f, 0.560708344f),
    new FastNoise.Float2(0.932486355f, -0.361205131f),
    new FastNoise.Float2(0.639465332f, 0.7688199f),
    new FastNoise.Float2(-0.0162384715f, -0.999868155f),
    new FastNoise.Float2(-0.995501459f, -0.0947461352f),
    new FastNoise.Float2(-0.8145332f, 0.580117f),
    new FastNoise.Float2(0.4037328f, -0.914876938f),
    new FastNoise.Float2(0.9944263f, 0.10543368f),
    new FastNoise.Float2(-0.16247116f, 0.9867133f),
    new FastNoise.Float2(-0.9949488f, -0.100383878f),
    new FastNoise.Float2(-0.699530244f, 0.714603f),
    new FastNoise.Float2(0.5263415f, -0.850273252f),
    new FastNoise.Float2(-96f * (float) Math.PI / 559f, 0.8419714f),
    new FastNoise.Float2(0.65793705f, 0.7530729f),
    new FastNoise.Float2(0.014267588f, -0.9998982f),
    new FastNoise.Float2(-0.6734384f, 0.7392433f),
    new FastNoise.Float2(0.6394121f, -0.7688642f),
    new FastNoise.Float2(0.9211571f, 0.389190853f),
    new FastNoise.Float2(-0.146637216f, -0.98919034f),
    new FastNoise.Float2(-0.7823181f, 0.6228791f),
    new FastNoise.Float2(-0.5039611f, -0.8637264f),
    new FastNoise.Float2(-0.774312f, -0.632804f)
  };
  private static readonly FastNoise.Float3[] CELL_3D = new FastNoise.Float3[256 /*0x0100*/]
  {
    new FastNoise.Float3(-0.7292737f, -0.661843956f, 0.17355819f),
    new FastNoise.Float3(0.7902921f, -0.5480887f, -0.2739291f),
    new FastNoise.Float3(0.7217579f, 0.622621238f, -0.3023381f),
    new FastNoise.Float3(0.5656831f, -0.8208298f, -0.079000026f),
    new FastNoise.Float3(0.760049045f, -0.555597961f, -0.337099969f),
    new FastNoise.Float3(0.371394575f, 0.501126468f, 0.78162545f),
    new FastNoise.Float3(-0.127706245f, -0.4254439f, -0.8959289f),
    new FastNoise.Float3(-0.2881561f, -0.5815839f, 0.7607406f),
    new FastNoise.Float3(0.5849561f, -0.6628202f, -0.4674352f),
    new FastNoise.Float3(0.330717117f, 0.0391653739f, 0.94291687f),
    new FastNoise.Float3(0.8712122f, -0.411337435f, -0.267938167f),
    new FastNoise.Float3(0.580981f, 0.7021916f, 0.411567777f),
    new FastNoise.Float3(0.5037569f, 0.6330057f, -0.5878204f),
    new FastNoise.Float3(0.449371219f, 0.6013902f, 0.6606023f),
    new FastNoise.Float3(-0.6878404f, 0.0901889056f, -0.7202372f),
    new FastNoise.Float3(-0.595895648f, -0.646935046f, 0.475797653f),
    new FastNoise.Float3(-0.5127052f, 0.1946922f, -0.836198747f),
    new FastNoise.Float3(-0.991150737f, -0.0541027635f, -0.121215314f),
    new FastNoise.Float3(-0.214972109f, 0.9720882f, -0.09397608f),
    new FastNoise.Float3(-0.7518651f, -0.542805731f, 0.374246955f),
    new FastNoise.Float3(0.5237069f, 0.8516377f, -0.0210781787f),
    new FastNoise.Float3(0.6333505f, 0.192616716f, -0.749510467f),
    new FastNoise.Float3(-0.06788242f, 0.39983058f, 0.9140719f),
    new FastNoise.Float3(-0.55386287f, -0.472989678f, -0.6852129f),
    new FastNoise.Float3(-0.726145566f, -0.5911991f, 0.350993335f),
    new FastNoise.Float3(-0.9229275f, -0.178280875f, 0.341204941f),
    new FastNoise.Float3(-0.6968815f, 0.651127458f, 0.300648034f),
    new FastNoise.Float3(0.960804462f, -0.209836319f, -0.18117249f),
    new FastNoise.Float3(0.0681714639f, -0.9743405f, 0.214506909f),
    new FastNoise.Float3(-0.3577285f, -0.6697087f, -0.650784552f),
    new FastNoise.Float3(-0.186862111f, 0.7648617f, -0.616497457f),
    new FastNoise.Float3(-0.654169738f, 0.3967915f, 0.643908739f),
    new FastNoise.Float3(0.699334f, -0.6164538f, 0.361823916f),
    new FastNoise.Float3(-0.154666573f, 0.6291284f, 0.7617583f),
    new FastNoise.Float3(-0.6841613f, -0.2580482f, -0.682154238f),
    new FastNoise.Float3(0.5383981f, 0.4258655f, 0.727163f),
    new FastNoise.Float3(-0.5026988f, -0.7939833f, -0.3418837f),
    new FastNoise.Float3(0.320297182f, 0.283441544f, 0.9039196f),
    new FastNoise.Float3(0.86832273f, -0.000376265642f, -0.495999515f),
    new FastNoise.Float3(0.791120052f, -0.0851104558f, 0.605710566f),
    new FastNoise.Float3(-0.04011016f, -0.439724863f, 0.8972364f),
    new FastNoise.Float3(0.914512f, 0.357934624f, -0.188548759f),
    new FastNoise.Float3(-0.961203933f, -0.275648415f, 0.0102466689f),
    new FastNoise.Float3(0.651036143f, -0.287779927f, -0.702377856f),
    new FastNoise.Float3(-0.204178631f, 0.736523747f, 0.6448596f),
    new FastNoise.Float3(-0.7718264f, 0.379062682f, 0.5104856f),
    new FastNoise.Float3(-0.306008279f, -0.7692988f, 0.56083715f),
    new FastNoise.Float3(0.454007328f, -0.5024843f, 0.735789955f),
    new FastNoise.Float3(0.481679559f, 0.6021208f, -0.636738f),
    new FastNoise.Float3(0.696198046f, -0.322219729f, 0.6414692f),
    new FastNoise.Float3(-0.653216064f, -0.6781149f, 0.336851567f),
    new FastNoise.Float3(0.508930147f, -0.615466237f, -0.601823449f),
    new FastNoise.Float3(-0.163591981f, -0.9133605f, -0.372840881f),
    new FastNoise.Float3(0.5240802f, -0.8437664f, 0.115750588f),
    new FastNoise.Float3(0.5902587f, 0.4983818f, -0.634988368f),
    new FastNoise.Float3(0.5863228f, 0.494764745f, 0.6414308f),
    new FastNoise.Float3(0.6779335f, 0.234134525f, 0.6968409f),
    new FastNoise.Float3(0.7177054f, -0.685897946f, 0.120178632f),
    new FastNoise.Float3(-0.532882f, -0.5205125f, 0.6671608f),
    new FastNoise.Float3(-0.8654874f, -0.07007271f, -0.4960054f),
    new FastNoise.Float3(-0.286181f, 0.795208931f, 0.534549534f),
    new FastNoise.Float3(-0.0484952964f, 0.981083632f, -0.187411562f),
    new FastNoise.Float3(-0.635852158f, 0.605834842f, 0.478180021f),
    new FastNoise.Float3(0.62547946f, -0.286161959f, 0.725869656f),
    new FastNoise.Float3(-0.258526f, 0.506194949f, -0.8227582f),
    new FastNoise.Float3(0.0213630684f, 0.506401658f, -0.862033f),
    new FastNoise.Float3(0.200111777f, 0.859926343f, 0.46955505f),
    new FastNoise.Float3(0.474356145f, 0.6014985f, -0.6427953f),
    new FastNoise.Float3(0.6622994f, -0.520247459f, -0.539168f),
    new FastNoise.Float3(0.08084973f, -0.653272033f, 0.7527941f),
    new FastNoise.Float3(-0.6893687f, 0.0592860356f, 0.7219805f),
    new FastNoise.Float3(-0.112188712f, -0.967318535f, 0.227395251f),
    new FastNoise.Float3(0.7344116f, 0.59796685f, -0.3210533f),
    new FastNoise.Float3(0.5789393f, -0.248884976f, 0.776457f),
    new FastNoise.Float3(0.698818266f, 0.355716974f, -0.6205791f),
    new FastNoise.Float3(-0.863684535f, -0.274877131f, -0.4224826f),
    new FastNoise.Float3(-0.4247028f, -0.464088082f, 0.777335048f),
    new FastNoise.Float3(0.5257723f, -0.842701733f, 0.115832992f),
    new FastNoise.Float3(0.934383035f, 0.316302478f, -0.163954392f),
    new FastNoise.Float3(-0.101683639f, -0.8057303f, -0.583488762f),
    new FastNoise.Float3(-0.6529239f, 0.506021261f, -0.5635893f),
    new FastNoise.Float3(-0.246528611f, -0.9668206f, -0.06694497f),
    new FastNoise.Float3(-0.9776897f, -0.209925056f, -0.00736882538f),
    new FastNoise.Float3(0.7736893f, 0.573424459f, 0.2694238f),
    new FastNoise.Float3(-0.6095088f, 0.4995679f, 0.6155737f),
    new FastNoise.Float3(0.5794535f, 0.7434547f, 0.333929241f),
    new FastNoise.Float3(-0.8226211f, 0.0814258158f, 0.562729359f),
    new FastNoise.Float3(-0.510385454f, 0.470366776f, 0.719904f),
    new FastNoise.Float3(-0.5764972f, -0.0723165646f, -0.813892663f),
    new FastNoise.Float3(0.7250629f, 0.39499715f, -0.56414634f),
    new FastNoise.Float3(-0.1525424f, 0.486084074f, -0.8604958f),
    new FastNoise.Float3(-0.55509764f, -0.495782077f, 0.6678823f),
    new FastNoise.Float3(-0.188361436f, 0.914586961f, 0.35784173f),
    new FastNoise.Float3(0.762555659f, -0.541440845f, -0.354048967f),
    new FastNoise.Float3(-0.5870232f, -0.3226498f, -0.7424964f),
    new FastNoise.Float3(0.305112422f, 0.2262544f, -0.9250488f),
    new FastNoise.Float3(0.637957633f, 0.577242434f, -0.509707034f),
    new FastNoise.Float3(-0.5966776f, 0.145485237f, -0.7891831f),
    new FastNoise.Float3(-0.65833056f, 0.655548751f, -0.369941473f),
    new FastNoise.Float3(0.743489265f, 0.235108465f, 0.6260573f),
    new FastNoise.Float3(0.5562114f, 0.826436043f, -0.08736329f),
    new FastNoise.Float3(-0.302894f, -0.8251527f, 0.476841927f),
    new FastNoise.Float3(0.112934381f, -0.9858884f, -0.123571075f),
    new FastNoise.Float3(0.5937653f, -0.5896814f, 0.5474657f),
    new FastNoise.Float3(0.6757964f, -0.583575845f, -0.450264841f),
    new FastNoise.Float3(0.7242303f, -0.115271978f, 0.679855049f),
    new FastNoise.Float3(-0.9511914f, 0.0753624f, -0.299258083f),
    new FastNoise.Float3(0.2539471f, -0.188633934f, 0.9486454f),
    new FastNoise.Float3(0.5714336f, -0.167945087f, -0.8032796f),
    new FastNoise.Float3(-0.06778235f, 0.39782694f, 0.9149532f),
    new FastNoise.Float3(0.6074973f, 0.73306f, -0.305892259f),
    new FastNoise.Float3(-0.543547869f, 0.167582244f, 0.8224791f),
    new FastNoise.Float3(-133f * (float) Math.PI / 711f, -0.3380045f, -0.7351187f),
    new FastNoise.Float3(-0.796756268f, 0.0409782268f, -0.602909863f),
    new FastNoise.Float3(-0.199635088f, 0.8706295f, 0.4496111f),
    new FastNoise.Float3(-0.0278766025f, -0.910623252f, -0.4122962f),
    new FastNoise.Float3(-0.7797626f, -0.6257635f, 0.0197577551f),
    new FastNoise.Float3(-0.5211233f, 0.740164459f, -0.424955457f),
    new FastNoise.Float3(0.8575425f, 0.4053273f, -0.316750169f),
    new FastNoise.Float3(0.104522333f, 0.8390196f, -0.533967435f),
    new FastNoise.Float3(0.3501823f, 0.9242524f, -0.152085021f),
    new FastNoise.Float3(0.198784992f, 0.0764761344f, 0.9770547f),
    new FastNoise.Float3(0.784599662f, 0.6066257f, -0.128096417f),
    new FastNoise.Float3(0.09006737f, -0.975098968f, -0.20265691f),
    new FastNoise.Float3(-0.827434361f, -0.542299569f, 0.145820364f),
    new FastNoise.Float3(-0.348579764f, -0.41580227f, 0.8400004f),
    new FastNoise.Float3(-0.2471779f, -0.730482f, -0.6366311f),
    new FastNoise.Float3(-0.3700155f, 0.8577948f, 0.356758446f),
    new FastNoise.Float3(0.591339469f, -0.548311949f, -0.591330349f),
    new FastNoise.Float3(0.120487355f, -0.7626472f, -0.6354935f),
    new FastNoise.Float3(0.6169593f, 0.03079648f, 0.7863923f),
    new FastNoise.Float3(0.12581569f, -0.664083f, -0.73699677f),
    new FastNoise.Float3(-0.6477565f, -0.174014732f, -0.741707742f),
    new FastNoise.Float3(0.6217889f, -0.7804431f, -0.06547655f),
    new FastNoise.Float3(0.6589943f, -0.6096988f, 0.44044736f),
    new FastNoise.Float3(-0.268983752f, -0.6732403f, -0.688763559f),
    new FastNoise.Float3(-0.38497752f, 0.567654252f, 0.7277094f),
    new FastNoise.Float3(0.57544446f, 0.811047137f, -0.105196349f),
    new FastNoise.Float3(0.914159358f, 0.3832948f, 0.131900564f),
    new FastNoise.Float3(-0.107925318f, 0.9245494f, 0.365459353f),
    new FastNoise.Float3(0.3779771f, 0.304314882f, 0.874371648f),
    new FastNoise.Float3(-0.214288518f, -0.8259286f, 0.5214617f),
    new FastNoise.Float3(0.580254436f, 0.414809853f, -0.7008834f),
    new FastNoise.Float3(-0.198266089f, 0.856716156f, -0.476159662f),
    new FastNoise.Float3(-0.0338155366f, 0.377318084f, -0.9254661f),
    new FastNoise.Float3(-0.686792254f, -0.6656598f, 0.29191336f),
    new FastNoise.Float3(0.7731743f, -0.287579358f, -0.565243f),
    new FastNoise.Float3(-0.09655942f, 0.91937083f, -0.3813575f),
    new FastNoise.Float3(0.271570235f, -0.957791f, -0.09426606f),
    new FastNoise.Float3(0.245101571f, -0.6917999f, -0.6792188f),
    new FastNoise.Float3(0.97770077f, -0.175385535f, 0.115503654f),
    new FastNoise.Float3(-0.522474f, 0.8521607f, 0.0290361587f),
    new FastNoise.Float3(-0.773488045f, -0.526129246f, 0.353417963f),
    new FastNoise.Float3(-0.71344924f, -0.269547254f, 0.6467878f),
    new FastNoise.Float3(0.164403722f, 0.5105846f, -0.843963742f),
    new FastNoise.Float3(0.6494636f, 0.0558561124f, 0.7583384f),
    new FastNoise.Float3(-0.4711971f, 0.501728058f, -0.7254256f),
    new FastNoise.Float3(-0.633576453f, -0.238168627f, -0.7361091f),
    new FastNoise.Float3(-0.9021533f, -0.2709478f, -0.335718185f),
    new FastNoise.Float3(-0.3793711f, 0.8722581f, 0.3086152f),
    new FastNoise.Float3(-0.685559869f, -0.325014323f, 0.6514394f),
    new FastNoise.Float3(0.290094227f, -0.7799058f, -0.5546101f),
    new FastNoise.Float3(-0.209831938f, 0.8503707f, 0.482535154f),
    new FastNoise.Float3(-0.459260374f, 0.6598504f, -0.5947077f),
    new FastNoise.Float3(0.871594548f, 0.09616365f, -0.480703115f),
    new FastNoise.Float3(-0.6776666f, 0.711850464f, -0.1844907f),
    new FastNoise.Float3(0.7044378f, 0.3124276f, 0.637304f),
    new FastNoise.Float3(-0.7052319f, -0.240109324f, -0.6670798f),
    new FastNoise.Float3(0.0819210038f, -0.720733643f, -0.688354552f),
    new FastNoise.Float3(-0.6993681f, -0.5875763f, -0.4069869f),
    new FastNoise.Float3(-0.128145441f, 0.6419896f, 0.755928636f),
    new FastNoise.Float3(-0.6337388f, -0.678547144f, -0.3714147f),
    new FastNoise.Float3(0.5565052f, -0.216888756f, -0.8020357f),
    new FastNoise.Float3(-0.579155445f, 0.7244372f, -0.3738579f),
    new FastNoise.Float3(0.11757791f, -0.7096451f, 0.69467926f),
    new FastNoise.Float3(-0.613462f, 0.132363111f, 0.7785528f),
    new FastNoise.Float3(0.698463559f, -0.0298051629f, -0.7150247f),
    new FastNoise.Float3(0.831808269f, -0.3930172f, 0.391959757f),
    new FastNoise.Float3(0.146957636f, 0.055416517f, -0.98758924f),
    new FastNoise.Float3(0.708868563f, -0.2690504f, 0.652010143f),
    new FastNoise.Float3(0.27260533f, 0.67369765f, -0.686889946f),
    new FastNoise.Float3(-0.65912956f, 0.303545862f, -0.688046634f),
    new FastNoise.Float3(0.481513143f, -0.752827f, 0.4487723f),
    new FastNoise.Float3(0.943001f, 0.167564735f, -0.287526131f),
    new FastNoise.Float3(0.434802949f, 0.7695305f, -0.46772778f),
    new FastNoise.Float3(0.393199623f, 0.5944736f, 0.701423645f),
    new FastNoise.Float3(0.725433648f, -0.603925645f, 0.330181479f),
    new FastNoise.Float3(0.759023547f, -0.6506083f, 0.0243331324f),
    new FastNoise.Float3(-0.8552769f, -0.3430043f, 0.388393581f),
    new FastNoise.Float3(-0.6139747f, 0.6981725f, 0.368225753f),
    new FastNoise.Float3(-0.746590555f, -0.575201f, 0.334284931f),
    new FastNoise.Float3(0.5730066f, 0.8105555f, -0.121091679f),
    new FastNoise.Float3(-0.922587752f, -0.3475211f, -0.167514041f),
    new FastNoise.Float3(-0.71058166f, -0.471969217f, -0.5218417f),
    new FastNoise.Float3(-0.0856461f, 0.358300149f, 0.9296697f),
    new FastNoise.Float3(-0.8279698f, -0.2043157f, 0.5222271f),
    new FastNoise.Float3(0.427944034f, 0.278166f, 0.8599346f),
    new FastNoise.Float3(0.539908f, -0.785712063f, -0.3019204f),
    new FastNoise.Float3(0.5678404f, -0.5495414f, -0.612830758f),
    new FastNoise.Float3(-0.9896071f, 0.136563912f, -0.0450341851f),
    new FastNoise.Float3(-0.6154343f, -0.644087553f, 0.454303741f),
    new FastNoise.Float3(0.107420437f, -0.794634044f, 0.597509444f),
    new FastNoise.Float3(-0.359545f, -0.888553f, 0.284957826f),
    new FastNoise.Float3(-0.218040526f, 0.1529889f, 0.9638738f),
    new FastNoise.Float3(-0.7277432f, -0.61640507f, -0.300723463f),
    new FastNoise.Float3(0.7249729f, -0.00669719465f, 0.688744843f),
    new FastNoise.Float3(-0.5553659f, -0.5336586f, 0.6377908f),
    new FastNoise.Float3(0.5137558f, 0.797620833f, -0.316f),
    new FastNoise.Float3(-0.3794025f, 0.924560845f, -0.0352275148f),
    new FastNoise.Float3(0.822924852f, 0.27453658f, -0.497417659f),
    new FastNoise.Float3(-0.5404114f, 0.60911417f, 0.5804614f),
    new FastNoise.Float3(0.8036582f, -0.270302951f, 0.5301602f),
    new FastNoise.Float3(0.604431868f, 0.683296859f, 0.409594327f),
    new FastNoise.Float3(0.06389989f, 0.965820849f, -0.2512108f),
    new FastNoise.Float3(0.108711332f, 0.74024713f, -0.6634878f),
    new FastNoise.Float3(-0.7134277f, -0.6926784f, 0.105912849f),
    new FastNoise.Float3(0.645889759f, -0.57245487f, -0.50509584f),
    new FastNoise.Float3(-0.6553931f, 0.73814714f, 0.159995615f),
    new FastNoise.Float3(0.391096145f, 0.918887138f, -0.05186756f),
    new FastNoise.Float3(-0.487902254f, -0.5904377f, 0.642911136f),
    new FastNoise.Float3(0.601479f, 0.770744145f, -0.210182011f),
    new FastNoise.Float3(-0.5677173f, 0.7511361f, 0.336885184f),
    new FastNoise.Float3(0.7858574f, 0.226674661f, 0.5753667f),
    new FastNoise.Float3(-0.452034563f, -0.6042227f, -0.656185746f),
    new FastNoise.Float3(0.00227211625f, 0.4132844f, -0.9105992f),
    new FastNoise.Float3(-0.581575155f, -0.5162926f, 0.6286591f),
    new FastNoise.Float3(-0.03703705f, 0.8273786f, 0.5604221f),
    new FastNoise.Float3(-0.511969268f, 0.795354366f, -0.324498f),
    new FastNoise.Float3(-0.268241733f, -0.957229f, -0.10843876f),
    new FastNoise.Float3(-0.232248276f, -0.9679131f, -0.09594243f),
    new FastNoise.Float3(0.3554329f, -0.8881506f, 0.291300625f),
    new FastNoise.Float3(0.734652042f, -0.4371373f, 0.5188423f),
    new FastNoise.Float3(0.998512f, 0.0465901121f, -0.0283394456f),
    new FastNoise.Float3(-0.37276876f, -0.9082481f, 0.190075725f),
    new FastNoise.Float3(0.9173738f, -0.3483642f, 0.192529842f),
    new FastNoise.Float3(0.2714911f, 0.41475296f, -0.868488669f),
    new FastNoise.Float3(0.5131763f, -0.711633444f, 0.4798207f),
    new FastNoise.Float3(-0.873735368f, 0.188869923f, -0.448235065f),
    new FastNoise.Float3(0.846004367f, -0.3725218f, 0.38145f),
    new FastNoise.Float3(0.897872746f, -0.178020909f, -0.402657539f),
    new FastNoise.Float3(0.217806563f, -0.9698323f, -0.109478951f),
    new FastNoise.Float3(-0.151803136f, -0.7788918f, -0.6085091f),
    new FastNoise.Float3(-0.2600385f, -0.4755398f, -0.840382f),
    new FastNoise.Float3(0.5723135f, -0.7474341f, -0.337341845f),
    new FastNoise.Float3(-0.7174141f, 0.169901714f, -0.675611138f),
    new FastNoise.Float3(-0.6841808f, 0.0214570761f, -0.728996754f),
    new FastNoise.Float3(-0.2007448f, 0.06555606f, -0.9774477f),
    new FastNoise.Float3(-0.114880368f, -0.8044887f, 0.5827524f),
    new FastNoise.Float3(-0.787035f, 0.03447489f, 0.6159443f),
    new FastNoise.Float3(-0.201559648f, 0.685987234f, 0.699138939f),
    new FastNoise.Float3(-0.0858108252f, -0.10920836f, -0.990308046f),
    new FastNoise.Float3(0.5532693f, 0.732525051f, -0.396610767f),
    new FastNoise.Float3(-0.184248939f, -0.9777375f, -0.100407675f),
    new FastNoise.Float3(0.07754738f, -0.9111506f, 0.404711038f),
    new FastNoise.Float3(0.139983848f, 0.7601631f, -0.634473443f),
    new FastNoise.Float3(0.448441923f, -0.84528923f, 0.290492535f)
  };
  private const int X_PRIME = 1619;
  private const int Y_PRIME = 31337;
  private const int Z_PRIME = 6971;
  private const int W_PRIME = 1013;
  private const float F3 = 0.333333343f;
  private const float G3 = 0.166666672f;
  private const float G33 = -0.5f;
  private const float F2 = 0.5f;
  private const float G2 = 0.25f;
  private static readonly byte[] SIMPLEX_4D = new byte[256 /*0x0100*/]
  {
    (byte) 0,
    (byte) 1,
    (byte) 2,
    (byte) 3,
    (byte) 0,
    (byte) 1,
    (byte) 3,
    (byte) 2,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 2,
    (byte) 3,
    (byte) 1,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 1,
    (byte) 2,
    (byte) 3,
    (byte) 0,
    (byte) 0,
    (byte) 2,
    (byte) 1,
    (byte) 3,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 3,
    (byte) 1,
    (byte) 2,
    (byte) 0,
    (byte) 3,
    (byte) 2,
    (byte) 1,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 1,
    (byte) 3,
    (byte) 2,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 1,
    (byte) 2,
    (byte) 0,
    (byte) 3,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 1,
    (byte) 3,
    (byte) 0,
    (byte) 2,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 2,
    (byte) 3,
    (byte) 0,
    (byte) 1,
    (byte) 2,
    (byte) 3,
    (byte) 1,
    (byte) 0,
    (byte) 1,
    (byte) 0,
    (byte) 2,
    (byte) 3,
    (byte) 1,
    (byte) 0,
    (byte) 3,
    (byte) 2,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 2,
    (byte) 0,
    (byte) 3,
    (byte) 1,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 2,
    (byte) 1,
    (byte) 3,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 2,
    (byte) 0,
    (byte) 1,
    (byte) 3,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 3,
    (byte) 0,
    (byte) 1,
    (byte) 2,
    (byte) 3,
    (byte) 0,
    (byte) 2,
    (byte) 1,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 3,
    (byte) 1,
    (byte) 2,
    (byte) 0,
    (byte) 2,
    (byte) 1,
    (byte) 0,
    (byte) 3,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 3,
    (byte) 1,
    (byte) 0,
    (byte) 2,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 0,
    (byte) 3,
    (byte) 2,
    (byte) 0,
    (byte) 1,
    (byte) 3,
    (byte) 2,
    (byte) 1,
    (byte) 0
  };
  private const float F4 = 0.309017f;
  private const float G4 = 0.1381966f;
  private const float CUBIC_3D_BOUNDING = 0.2962963f;
  private const float CUBIC_2D_BOUNDING = 0.444444448f;

  public FastNoise(int seed = 1337)
  {
    this.m_seed = seed;
    this.CalculateFractalBounding();
  }

  public static float GetDecimalType() => 0.0f;

  public float GetFrequency() => this.m_frequency;

  public int GetSeed() => this.m_seed;

  public void SetSeed(int seed) => this.m_seed = seed;

  public void SetFrequency(float frequency) => this.m_frequency = frequency;

  public void SetInterp(FastNoise.Interp interp) => this.m_interp = interp;

  public void SetNoiseType(FastNoise.NoiseType noiseType) => this.m_noiseType = noiseType;

  public void SetFractalOctaves(int octaves)
  {
    this.m_octaves = octaves;
    this.CalculateFractalBounding();
  }

  public void SetFractalLacunarity(float lacunarity) => this.m_lacunarity = lacunarity;

  public void SetFractalGain(float gain)
  {
    this.m_gain = gain;
    this.CalculateFractalBounding();
  }

  public void SetFractalType(FastNoise.FractalType fractalType) => this.m_fractalType = fractalType;

  public void SetCellularDistanceFunction(
    FastNoise.CellularDistanceFunction cellularDistanceFunction)
  {
    this.m_cellularDistanceFunction = cellularDistanceFunction;
  }

  public void SetCellularReturnType(FastNoise.CellularReturnType cellularReturnType)
  {
    this.m_cellularReturnType = cellularReturnType;
  }

  public void SetCellularDistance2Indices(int cellularDistanceIndex0, int cellularDistanceIndex1)
  {
    this.m_cellularDistanceIndex0 = Math.Min(cellularDistanceIndex0, cellularDistanceIndex1);
    this.m_cellularDistanceIndex1 = Math.Max(cellularDistanceIndex0, cellularDistanceIndex1);
    this.m_cellularDistanceIndex0 = Math.Min(Math.Max(this.m_cellularDistanceIndex0, 0), 3);
    this.m_cellularDistanceIndex1 = Math.Min(Math.Max(this.m_cellularDistanceIndex1, 0), 3);
  }

  public void SetCellularJitter(float cellularJitter) => this.m_cellularJitter = cellularJitter;

  public void SetCellularNoiseLookup(FastNoise noise) => this.m_cellularNoiseLookup = noise;

  public void SetGradientPerturbAmp(float gradientPerturbAmp)
  {
    this.m_gradientPerturbAmp = gradientPerturbAmp;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int FastFloor(float f) => (double) f < 0.0 ? (int) f - 1 : (int) f;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int FastRound(float f)
  {
    return (double) f < 0.0 ? (int) ((double) f - 0.5) : (int) ((double) f + 0.5);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static float Lerp(float a, float b, float t) => a + t * (b - a);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static float InterpHermiteFunc(float t)
  {
    return (float) ((double) t * (double) t * (3.0 - 2.0 * (double) t));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static float InterpQuinticFunc(float t)
  {
    return (float) ((double) t * (double) t * (double) t * ((double) t * ((double) t * 6.0 - 15.0) + 10.0));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static float CubicLerp(float a, float b, float c, float d, float t)
  {
    float num = (float) ((double) d - (double) c - ((double) a - (double) b));
    return (float) ((double) t * (double) t * (double) t * (double) num + (double) t * (double) t * ((double) a - (double) b - (double) num) + (double) t * ((double) c - (double) a)) + b;
  }

  private void CalculateFractalBounding()
  {
    float gain = this.m_gain;
    float num = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      num += gain;
      gain *= this.m_gain;
    }
    this.m_fractalBounding = 1f / num;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int Hash2D(int seed, int x, int y)
  {
    int num1 = seed ^ 1619 * x ^ 31337 * y;
    int num2 = num1 * num1 * num1 * 60493;
    return num2 >> 13 ^ num2;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int Hash3D(int seed, int x, int y, int z)
  {
    int num1 = seed ^ 1619 * x ^ 31337 * y ^ 6971 * z;
    int num2 = num1 * num1 * num1 * 60493;
    return num2 >> 13 ^ num2;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int Hash4D(int seed, int x, int y, int z, int w)
  {
    int num1 = seed ^ 1619 * x ^ 31337 * y ^ 6971 * z ^ 1013 * w;
    int num2 = num1 * num1 * num1 * 60493;
    return num2 >> 13 ^ num2;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static float ValCoord2D(int seed, int x, int y)
  {
    int num = seed ^ 1619 * x ^ 31337 * y;
    return (float) (num * num * num * 60493) / (float) int.MaxValue;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static float ValCoord3D(int seed, int x, int y, int z)
  {
    int num = seed ^ 1619 * x ^ 31337 * y ^ 6971 * z;
    return (float) (num * num * num * 60493) / (float) int.MaxValue;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static float ValCoord4D(int seed, int x, int y, int z, int w)
  {
    int num = seed ^ 1619 * x ^ 31337 * y ^ 6971 * z ^ 1013 * w;
    return (float) (num * num * num * 60493) / (float) int.MaxValue;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static float GradCoord2D(int seed, int x, int y, float xd, float yd)
  {
    int num1 = seed ^ 1619 * x ^ 31337 * y;
    int num2 = num1 * num1 * num1 * 60493;
    int num3 = num2 >> 13 ^ num2;
    FastNoise.Float2 float2 = FastNoise.GRAD_2D[num3 & 7];
    return (float) ((double) xd * (double) float2.x + (double) yd * (double) float2.y);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static float GradCoord3D(int seed, int x, int y, int z, float xd, float yd, float zd)
  {
    int num1 = seed ^ 1619 * x ^ 31337 * y ^ 6971 * z;
    int num2 = num1 * num1 * num1 * 60493;
    int num3 = num2 >> 13 ^ num2;
    FastNoise.Float3 float3 = FastNoise.GRAD_3D[num3 & 15];
    return (float) ((double) xd * (double) float3.x + (double) yd * (double) float3.y + (double) zd * (double) float3.z);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static float GradCoord4D(
    int seed,
    int x,
    int y,
    int z,
    int w,
    float xd,
    float yd,
    float zd,
    float wd)
  {
    int num1 = seed ^ 1619 * x ^ 31337 * y ^ 6971 * z ^ 1013 * w;
    int num2 = num1 * num1 * num1 * 60493;
    int num3 = (num2 >> 13 ^ num2) & 31 /*0x1F*/;
    float num4 = yd;
    float num5 = zd;
    float num6 = wd;
    switch (num3 >> 3)
    {
      case 1:
        num4 = wd;
        num5 = xd;
        num6 = yd;
        break;
      case 2:
        num4 = zd;
        num5 = wd;
        num6 = xd;
        break;
      case 3:
        num4 = yd;
        num5 = zd;
        num6 = wd;
        break;
    }
    return (float) (((num3 & 4) == 0 ? -(double) num4 : (double) num4) + ((num3 & 2) == 0 ? -(double) num5 : (double) num5) + ((num3 & 1) == 0 ? -(double) num6 : (double) num6));
  }

  public float GetNoise(float x, float y, float z)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    z *= this.m_frequency;
    switch (this.m_noiseType)
    {
      case FastNoise.NoiseType.Value:
        return this.SingleValue(this.m_seed, x, y, z);
      case FastNoise.NoiseType.ValueFractal:
        switch (this.m_fractalType)
        {
          case FastNoise.FractalType.FBM:
            return this.SingleValueFractalFBM(x, y, z);
          case FastNoise.FractalType.Billow:
            return this.SingleValueFractalBillow(x, y, z);
          case FastNoise.FractalType.RigidMulti:
            return this.SingleValueFractalRigidMulti(x, y, z);
          default:
            return 0.0f;
        }
      case FastNoise.NoiseType.Perlin:
        return this.SinglePerlin(this.m_seed, x, y, z);
      case FastNoise.NoiseType.PerlinFractal:
        switch (this.m_fractalType)
        {
          case FastNoise.FractalType.FBM:
            return this.SinglePerlinFractalFBM(x, y, z);
          case FastNoise.FractalType.Billow:
            return this.SinglePerlinFractalBillow(x, y, z);
          case FastNoise.FractalType.RigidMulti:
            return this.SinglePerlinFractalRigidMulti(x, y, z);
          default:
            return 0.0f;
        }
      case FastNoise.NoiseType.Simplex:
        return this.SingleSimplex(this.m_seed, x, y, z);
      case FastNoise.NoiseType.SimplexFractal:
        switch (this.m_fractalType)
        {
          case FastNoise.FractalType.FBM:
            return this.SingleSimplexFractalFBM(x, y, z);
          case FastNoise.FractalType.Billow:
            return this.SingleSimplexFractalBillow(x, y, z);
          case FastNoise.FractalType.RigidMulti:
            return this.SingleSimplexFractalRigidMulti(x, y, z);
          default:
            return 0.0f;
        }
      case FastNoise.NoiseType.Cellular:
        return (uint) this.m_cellularReturnType <= 2U ? this.SingleCellular(x, y, z) : this.SingleCellular2Edge(x, y, z);
      case FastNoise.NoiseType.WhiteNoise:
        return this.GetWhiteNoise(x, y, z);
      case FastNoise.NoiseType.Cubic:
        return this.SingleCubic(this.m_seed, x, y, z);
      case FastNoise.NoiseType.CubicFractal:
        switch (this.m_fractalType)
        {
          case FastNoise.FractalType.FBM:
            return this.SingleCubicFractalFBM(x, y, z);
          case FastNoise.FractalType.Billow:
            return this.SingleCubicFractalBillow(x, y, z);
          case FastNoise.FractalType.RigidMulti:
            return this.SingleCubicFractalRigidMulti(x, y, z);
          default:
            return 0.0f;
        }
      default:
        return 0.0f;
    }
  }

  public float GetNoise(float x, float y)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    switch (this.m_noiseType)
    {
      case FastNoise.NoiseType.Value:
        return this.SingleValue(this.m_seed, x, y);
      case FastNoise.NoiseType.ValueFractal:
        switch (this.m_fractalType)
        {
          case FastNoise.FractalType.FBM:
            return this.SingleValueFractalFBM(x, y);
          case FastNoise.FractalType.Billow:
            return this.SingleValueFractalBillow(x, y);
          case FastNoise.FractalType.RigidMulti:
            return this.SingleValueFractalRigidMulti(x, y);
          default:
            return 0.0f;
        }
      case FastNoise.NoiseType.Perlin:
        return this.SinglePerlin(this.m_seed, x, y);
      case FastNoise.NoiseType.PerlinFractal:
        switch (this.m_fractalType)
        {
          case FastNoise.FractalType.FBM:
            return this.SinglePerlinFractalFBM(x, y);
          case FastNoise.FractalType.Billow:
            return this.SinglePerlinFractalBillow(x, y);
          case FastNoise.FractalType.RigidMulti:
            return this.SinglePerlinFractalRigidMulti(x, y);
          default:
            return 0.0f;
        }
      case FastNoise.NoiseType.Simplex:
        return this.SingleSimplex(this.m_seed, x, y);
      case FastNoise.NoiseType.SimplexFractal:
        switch (this.m_fractalType)
        {
          case FastNoise.FractalType.FBM:
            return this.SingleSimplexFractalFBM(x, y);
          case FastNoise.FractalType.Billow:
            return this.SingleSimplexFractalBillow(x, y);
          case FastNoise.FractalType.RigidMulti:
            return this.SingleSimplexFractalRigidMulti(x, y);
          default:
            return 0.0f;
        }
      case FastNoise.NoiseType.Cellular:
        return (uint) this.m_cellularReturnType <= 2U ? this.SingleCellular(x, y) : this.SingleCellular2Edge(x, y);
      case FastNoise.NoiseType.WhiteNoise:
        return this.GetWhiteNoise(x, y);
      case FastNoise.NoiseType.Cubic:
        return this.SingleCubic(this.m_seed, x, y);
      case FastNoise.NoiseType.CubicFractal:
        switch (this.m_fractalType)
        {
          case FastNoise.FractalType.FBM:
            return this.SingleCubicFractalFBM(x, y);
          case FastNoise.FractalType.Billow:
            return this.SingleCubicFractalBillow(x, y);
          case FastNoise.FractalType.RigidMulti:
            return this.SingleCubicFractalRigidMulti(x, y);
          default:
            return 0.0f;
        }
      default:
        return 0.0f;
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private int FloatCast2Int(float f)
  {
    long int64Bits = BitConverter.DoubleToInt64Bits((double) f);
    return (int) (int64Bits ^ int64Bits >> 32 /*0x20*/);
  }

  public float GetWhiteNoise(float x, float y, float z, float w)
  {
    return FastNoise.ValCoord4D(this.m_seed, this.FloatCast2Int(x), this.FloatCast2Int(y), this.FloatCast2Int(z), this.FloatCast2Int(w));
  }

  public float GetWhiteNoise(float x, float y, float z)
  {
    return FastNoise.ValCoord3D(this.m_seed, this.FloatCast2Int(x), this.FloatCast2Int(y), this.FloatCast2Int(z));
  }

  public float GetWhiteNoise(float x, float y)
  {
    return FastNoise.ValCoord2D(this.m_seed, this.FloatCast2Int(x), this.FloatCast2Int(y));
  }

  public float GetWhiteNoiseInt(int x, int y, int z, int w)
  {
    return FastNoise.ValCoord4D(this.m_seed, x, y, z, w);
  }

  public float GetWhiteNoiseInt(int x, int y, int z) => FastNoise.ValCoord3D(this.m_seed, x, y, z);

  public float GetWhiteNoiseInt(int x, int y) => FastNoise.ValCoord2D(this.m_seed, x, y);

  public float GetValueFractal(float x, float y, float z)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    z *= this.m_frequency;
    switch (this.m_fractalType)
    {
      case FastNoise.FractalType.FBM:
        return this.SingleValueFractalFBM(x, y, z);
      case FastNoise.FractalType.Billow:
        return this.SingleValueFractalBillow(x, y, z);
      case FastNoise.FractalType.RigidMulti:
        return this.SingleValueFractalRigidMulti(x, y, z);
      default:
        return 0.0f;
    }
  }

  private float SingleValueFractalFBM(float x, float y, float z)
  {
    int seed = this.m_seed;
    float num1 = this.SingleValue(seed, x, y, z);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += this.SingleValue(++seed, x, y, z) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleValueFractalBillow(float x, float y, float z)
  {
    int seed = this.m_seed;
    float num1 = (float) ((double) Math.Abs(this.SingleValue(seed, x, y, z)) * 2.0 - 1.0);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += (float) ((double) Math.Abs(this.SingleValue(++seed, x, y, z)) * 2.0 - 1.0) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleValueFractalRigidMulti(float x, float y, float z)
  {
    int seed = this.m_seed;
    float num1 = 1f - Math.Abs(this.SingleValue(seed, x, y, z));
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 -= (1f - Math.Abs(this.SingleValue(++seed, x, y, z))) * num2;
    }
    return num1;
  }

  public float GetValue(float x, float y, float z)
  {
    return this.SingleValue(this.m_seed, x * this.m_frequency, y * this.m_frequency, z * this.m_frequency);
  }

  private float SingleValue(int seed, float x, float y, float z)
  {
    int x1 = FastNoise.FastFloor(x);
    int y1 = FastNoise.FastFloor(y);
    int z1 = FastNoise.FastFloor(z);
    int x2 = x1 + 1;
    int y2 = y1 + 1;
    int z2 = z1 + 1;
    float t1;
    float t2;
    float t3;
    switch (this.m_interp)
    {
      case FastNoise.Interp.Hermite:
        t1 = FastNoise.InterpHermiteFunc(x - (float) x1);
        t2 = FastNoise.InterpHermiteFunc(y - (float) y1);
        t3 = FastNoise.InterpHermiteFunc(z - (float) z1);
        break;
      case FastNoise.Interp.Quintic:
        t1 = FastNoise.InterpQuinticFunc(x - (float) x1);
        t2 = FastNoise.InterpQuinticFunc(y - (float) y1);
        t3 = FastNoise.InterpQuinticFunc(z - (float) z1);
        break;
      default:
        t1 = x - (float) x1;
        t2 = y - (float) y1;
        t3 = z - (float) z1;
        break;
    }
    double a1 = (double) FastNoise.Lerp(FastNoise.ValCoord3D(seed, x1, y1, z1), FastNoise.ValCoord3D(seed, x2, y1, z1), t1);
    float num = FastNoise.Lerp(FastNoise.ValCoord3D(seed, x1, y2, z1), FastNoise.ValCoord3D(seed, x2, y2, z1), t1);
    float a2 = FastNoise.Lerp(FastNoise.ValCoord3D(seed, x1, y1, z2), FastNoise.ValCoord3D(seed, x2, y1, z2), t1);
    float b1 = FastNoise.Lerp(FastNoise.ValCoord3D(seed, x1, y2, z2), FastNoise.ValCoord3D(seed, x2, y2, z2), t1);
    double b2 = (double) num;
    double t4 = (double) t2;
    return FastNoise.Lerp(FastNoise.Lerp((float) a1, (float) b2, (float) t4), FastNoise.Lerp(a2, b1, t2), t3);
  }

  public float GetValueFractal(float x, float y)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    switch (this.m_fractalType)
    {
      case FastNoise.FractalType.FBM:
        return this.SingleValueFractalFBM(x, y);
      case FastNoise.FractalType.Billow:
        return this.SingleValueFractalBillow(x, y);
      case FastNoise.FractalType.RigidMulti:
        return this.SingleValueFractalRigidMulti(x, y);
      default:
        return 0.0f;
    }
  }

  private float SingleValueFractalFBM(float x, float y)
  {
    int seed = this.m_seed;
    float num1 = this.SingleValue(seed, x, y);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += this.SingleValue(++seed, x, y) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleValueFractalBillow(float x, float y)
  {
    int seed = this.m_seed;
    float num1 = (float) ((double) Math.Abs(this.SingleValue(seed, x, y)) * 2.0 - 1.0);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += (float) ((double) Math.Abs(this.SingleValue(++seed, x, y)) * 2.0 - 1.0) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleValueFractalRigidMulti(float x, float y)
  {
    int seed = this.m_seed;
    float num1 = 1f - Math.Abs(this.SingleValue(seed, x, y));
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 -= (1f - Math.Abs(this.SingleValue(++seed, x, y))) * num2;
    }
    return num1;
  }

  public float GetValue(float x, float y)
  {
    return this.SingleValue(this.m_seed, x * this.m_frequency, y * this.m_frequency);
  }

  private float SingleValue(int seed, float x, float y)
  {
    int x1 = FastNoise.FastFloor(x);
    int y1 = FastNoise.FastFloor(y);
    int x2 = x1 + 1;
    int y2 = y1 + 1;
    float t1;
    float t2;
    switch (this.m_interp)
    {
      case FastNoise.Interp.Hermite:
        t1 = FastNoise.InterpHermiteFunc(x - (float) x1);
        t2 = FastNoise.InterpHermiteFunc(y - (float) y1);
        break;
      case FastNoise.Interp.Quintic:
        t1 = FastNoise.InterpQuinticFunc(x - (float) x1);
        t2 = FastNoise.InterpQuinticFunc(y - (float) y1);
        break;
      default:
        t1 = x - (float) x1;
        t2 = y - (float) y1;
        break;
    }
    return FastNoise.Lerp(FastNoise.Lerp(FastNoise.ValCoord2D(seed, x1, y1), FastNoise.ValCoord2D(seed, x2, y1), t1), FastNoise.Lerp(FastNoise.ValCoord2D(seed, x1, y2), FastNoise.ValCoord2D(seed, x2, y2), t1), t2);
  }

  public float GetPerlinFractal(float x, float y, float z)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    z *= this.m_frequency;
    switch (this.m_fractalType)
    {
      case FastNoise.FractalType.FBM:
        return this.SinglePerlinFractalFBM(x, y, z);
      case FastNoise.FractalType.Billow:
        return this.SinglePerlinFractalBillow(x, y, z);
      case FastNoise.FractalType.RigidMulti:
        return this.SinglePerlinFractalRigidMulti(x, y, z);
      default:
        return 0.0f;
    }
  }

  private float SinglePerlinFractalFBM(float x, float y, float z)
  {
    int seed = this.m_seed;
    float num1 = this.SinglePerlin(seed, x, y, z);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += this.SinglePerlin(++seed, x, y, z) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SinglePerlinFractalBillow(float x, float y, float z)
  {
    int seed = this.m_seed;
    float num1 = (float) ((double) Math.Abs(this.SinglePerlin(seed, x, y, z)) * 2.0 - 1.0);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += (float) ((double) Math.Abs(this.SinglePerlin(++seed, x, y, z)) * 2.0 - 1.0) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SinglePerlinFractalRigidMulti(float x, float y, float z)
  {
    int seed = this.m_seed;
    float num1 = 1f - Math.Abs(this.SinglePerlin(seed, x, y, z));
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 -= (1f - Math.Abs(this.SinglePerlin(++seed, x, y, z))) * num2;
    }
    return num1;
  }

  public float GetPerlin(float x, float y, float z)
  {
    return this.SinglePerlin(this.m_seed, x * this.m_frequency, y * this.m_frequency, z * this.m_frequency);
  }

  private float SinglePerlin(int seed, float x, float y, float z)
  {
    int x1 = FastNoise.FastFloor(x);
    int y1 = FastNoise.FastFloor(y);
    int z1 = FastNoise.FastFloor(z);
    int x2 = x1 + 1;
    int y2 = y1 + 1;
    int z2 = z1 + 1;
    float t1;
    float t2;
    float t3;
    switch (this.m_interp)
    {
      case FastNoise.Interp.Hermite:
        t1 = FastNoise.InterpHermiteFunc(x - (float) x1);
        t2 = FastNoise.InterpHermiteFunc(y - (float) y1);
        t3 = FastNoise.InterpHermiteFunc(z - (float) z1);
        break;
      case FastNoise.Interp.Quintic:
        t1 = FastNoise.InterpQuinticFunc(x - (float) x1);
        t2 = FastNoise.InterpQuinticFunc(y - (float) y1);
        t3 = FastNoise.InterpQuinticFunc(z - (float) z1);
        break;
      default:
        t1 = x - (float) x1;
        t2 = y - (float) y1;
        t3 = z - (float) z1;
        break;
    }
    float xd1 = x - (float) x1;
    float yd1 = y - (float) y1;
    float zd1 = z - (float) z1;
    float xd2 = xd1 - 1f;
    float yd2 = yd1 - 1f;
    float zd2 = zd1 - 1f;
    double a1 = (double) FastNoise.Lerp(FastNoise.GradCoord3D(seed, x1, y1, z1, xd1, yd1, zd1), FastNoise.GradCoord3D(seed, x2, y1, z1, xd2, yd1, zd1), t1);
    float num = FastNoise.Lerp(FastNoise.GradCoord3D(seed, x1, y2, z1, xd1, yd2, zd1), FastNoise.GradCoord3D(seed, x2, y2, z1, xd2, yd2, zd1), t1);
    float a2 = FastNoise.Lerp(FastNoise.GradCoord3D(seed, x1, y1, z2, xd1, yd1, zd2), FastNoise.GradCoord3D(seed, x2, y1, z2, xd2, yd1, zd2), t1);
    float b1 = FastNoise.Lerp(FastNoise.GradCoord3D(seed, x1, y2, z2, xd1, yd2, zd2), FastNoise.GradCoord3D(seed, x2, y2, z2, xd2, yd2, zd2), t1);
    double b2 = (double) num;
    double t4 = (double) t2;
    return FastNoise.Lerp(FastNoise.Lerp((float) a1, (float) b2, (float) t4), FastNoise.Lerp(a2, b1, t2), t3);
  }

  public float GetPerlinFractal(float x, float y)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    switch (this.m_fractalType)
    {
      case FastNoise.FractalType.FBM:
        return this.SinglePerlinFractalFBM(x, y);
      case FastNoise.FractalType.Billow:
        return this.SinglePerlinFractalBillow(x, y);
      case FastNoise.FractalType.RigidMulti:
        return this.SinglePerlinFractalRigidMulti(x, y);
      default:
        return 0.0f;
    }
  }

  private float SinglePerlinFractalFBM(float x, float y)
  {
    int seed = this.m_seed;
    float num1 = this.SinglePerlin(seed, x, y);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += this.SinglePerlin(++seed, x, y) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SinglePerlinFractalBillow(float x, float y)
  {
    int seed = this.m_seed;
    float num1 = (float) ((double) Math.Abs(this.SinglePerlin(seed, x, y)) * 2.0 - 1.0);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += (float) ((double) Math.Abs(this.SinglePerlin(++seed, x, y)) * 2.0 - 1.0) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SinglePerlinFractalRigidMulti(float x, float y)
  {
    int seed = this.m_seed;
    float num1 = 1f - Math.Abs(this.SinglePerlin(seed, x, y));
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 -= (1f - Math.Abs(this.SinglePerlin(++seed, x, y))) * num2;
    }
    return num1;
  }

  public float GetPerlin(float x, float y)
  {
    return this.SinglePerlin(this.m_seed, x * this.m_frequency, y * this.m_frequency);
  }

  private float SinglePerlin(int seed, float x, float y)
  {
    int x1 = FastNoise.FastFloor(x);
    int y1 = FastNoise.FastFloor(y);
    int x2 = x1 + 1;
    int y2 = y1 + 1;
    float t1;
    float t2;
    switch (this.m_interp)
    {
      case FastNoise.Interp.Hermite:
        t1 = FastNoise.InterpHermiteFunc(x - (float) x1);
        t2 = FastNoise.InterpHermiteFunc(y - (float) y1);
        break;
      case FastNoise.Interp.Quintic:
        t1 = FastNoise.InterpQuinticFunc(x - (float) x1);
        t2 = FastNoise.InterpQuinticFunc(y - (float) y1);
        break;
      default:
        t1 = x - (float) x1;
        t2 = y - (float) y1;
        break;
    }
    float xd1 = x - (float) x1;
    float yd1 = y - (float) y1;
    float xd2 = xd1 - 1f;
    float yd2 = yd1 - 1f;
    return FastNoise.Lerp(FastNoise.Lerp(FastNoise.GradCoord2D(seed, x1, y1, xd1, yd1), FastNoise.GradCoord2D(seed, x2, y1, xd2, yd1), t1), FastNoise.Lerp(FastNoise.GradCoord2D(seed, x1, y2, xd1, yd2), FastNoise.GradCoord2D(seed, x2, y2, xd2, yd2), t1), t2);
  }

  public float GetSimplexFractal(float x, float y, float z)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    z *= this.m_frequency;
    switch (this.m_fractalType)
    {
      case FastNoise.FractalType.FBM:
        return this.SingleSimplexFractalFBM(x, y, z);
      case FastNoise.FractalType.Billow:
        return this.SingleSimplexFractalBillow(x, y, z);
      case FastNoise.FractalType.RigidMulti:
        return this.SingleSimplexFractalRigidMulti(x, y, z);
      default:
        return 0.0f;
    }
  }

  private float SingleSimplexFractalFBM(float x, float y, float z)
  {
    int seed = this.m_seed;
    float num1 = this.SingleSimplex(seed, x, y, z);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += this.SingleSimplex(++seed, x, y, z) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleSimplexFractalBillow(float x, float y, float z)
  {
    int seed = this.m_seed;
    float num1 = (float) ((double) Math.Abs(this.SingleSimplex(seed, x, y, z)) * 2.0 - 1.0);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += (float) ((double) Math.Abs(this.SingleSimplex(++seed, x, y, z)) * 2.0 - 1.0) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleSimplexFractalRigidMulti(float x, float y, float z)
  {
    int seed = this.m_seed;
    float num1 = 1f - Math.Abs(this.SingleSimplex(seed, x, y, z));
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 -= (1f - Math.Abs(this.SingleSimplex(++seed, x, y, z))) * num2;
    }
    return num1;
  }

  public float GetSimplexFractal(float x, float y, float z, float w)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    z *= this.m_frequency;
    w *= this.m_frequency;
    switch (this.m_fractalType)
    {
      case FastNoise.FractalType.FBM:
        return this.SingleSimplexFractalFBM(x, y, z, w);
      case FastNoise.FractalType.Billow:
        return this.SingleSimplexFractalBillow(x, y, z, w);
      case FastNoise.FractalType.RigidMulti:
        return this.SingleSimplexFractalRigidMulti(x, y, z, w);
      default:
        return 0.0f;
    }
  }

  private float SingleSimplexFractalFBM(float x, float y, float z, float w)
  {
    int seed = this.m_seed;
    float num1 = this.SingleSimplex(seed, x, y, z, w);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      w *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += this.SingleSimplex(++seed, x, y, z, w) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleSimplexFractalBillow(float x, float y, float z, float w)
  {
    int seed = this.m_seed;
    float num1 = (float) ((double) Math.Abs(this.SingleSimplex(seed, x, y, z, w)) * 2.0 - 1.0);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      w *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += (float) ((double) Math.Abs(this.SingleSimplex(++seed, x, y, z, w)) * 2.0 - 1.0) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleSimplexFractalRigidMulti(float x, float y, float z, float w)
  {
    int seed = this.m_seed;
    float num1 = 1f - Math.Abs(this.SingleSimplex(seed, x, y, z, w));
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      w *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 -= (1f - Math.Abs(this.SingleSimplex(++seed, x, y, z, w))) * num2;
    }
    return num1;
  }

  public float GetSimplex(float x, float y, float z)
  {
    return this.SingleSimplex(this.m_seed, x * this.m_frequency, y * this.m_frequency, z * this.m_frequency);
  }

  private float SingleSimplex(int seed, float x, float y, float z)
  {
    float num1 = (float) (((double) x + (double) y + (double) z) * 0.3333333432674408);
    int x1 = FastNoise.FastFloor(x + num1);
    int y1 = FastNoise.FastFloor(y + num1);
    int z1 = FastNoise.FastFloor(z + num1);
    float num2 = (float) (x1 + y1 + z1) * 0.166666672f;
    float xd1 = x - ((float) x1 - num2);
    float yd1 = y - ((float) y1 - num2);
    float zd1 = z - ((float) z1 - num2);
    int num3;
    int num4;
    int num5;
    int num6;
    int num7;
    int num8;
    if ((double) xd1 >= (double) yd1)
    {
      if ((double) yd1 >= (double) zd1)
      {
        num3 = 1;
        num4 = 0;
        num5 = 0;
        num6 = 1;
        num7 = 1;
        num8 = 0;
      }
      else if ((double) xd1 >= (double) zd1)
      {
        num3 = 1;
        num4 = 0;
        num5 = 0;
        num6 = 1;
        num7 = 0;
        num8 = 1;
      }
      else
      {
        num3 = 0;
        num4 = 0;
        num5 = 1;
        num6 = 1;
        num7 = 0;
        num8 = 1;
      }
    }
    else if ((double) yd1 < (double) zd1)
    {
      num3 = 0;
      num4 = 0;
      num5 = 1;
      num6 = 0;
      num7 = 1;
      num8 = 1;
    }
    else if ((double) xd1 < (double) zd1)
    {
      num3 = 0;
      num4 = 1;
      num5 = 0;
      num6 = 0;
      num7 = 1;
      num8 = 1;
    }
    else
    {
      num3 = 0;
      num4 = 1;
      num5 = 0;
      num6 = 1;
      num7 = 1;
      num8 = 0;
    }
    float xd2 = (float) ((double) xd1 - (double) num3 + 0.1666666716337204);
    float yd2 = (float) ((double) yd1 - (double) num4 + 0.1666666716337204);
    float zd2 = (float) ((double) zd1 - (double) num5 + 0.1666666716337204);
    float xd3 = (float) ((double) xd1 - (double) num6 + 0.3333333432674408);
    float yd3 = (float) ((double) yd1 - (double) num7 + 0.3333333432674408);
    float zd3 = (float) ((double) zd1 - (double) num8 + 0.3333333432674408);
    float xd4 = xd1 - 0.5f;
    float yd4 = yd1 - 0.5f;
    float zd4 = zd1 - 0.5f;
    float num9 = (float) (0.60000002384185791 - (double) xd1 * (double) xd1 - (double) yd1 * (double) yd1 - (double) zd1 * (double) zd1);
    float num10;
    if ((double) num9 < 0.0)
    {
      num10 = 0.0f;
    }
    else
    {
      float num11 = num9 * num9;
      num10 = num11 * num11 * FastNoise.GradCoord3D(seed, x1, y1, z1, xd1, yd1, zd1);
    }
    float num12 = (float) (0.60000002384185791 - (double) xd2 * (double) xd2 - (double) yd2 * (double) yd2 - (double) zd2 * (double) zd2);
    float num13;
    if ((double) num12 < 0.0)
    {
      num13 = 0.0f;
    }
    else
    {
      float num14 = num12 * num12;
      num13 = num14 * num14 * FastNoise.GradCoord3D(seed, x1 + num3, y1 + num4, z1 + num5, xd2, yd2, zd2);
    }
    float num15 = (float) (0.60000002384185791 - (double) xd3 * (double) xd3 - (double) yd3 * (double) yd3 - (double) zd3 * (double) zd3);
    float num16;
    if ((double) num15 < 0.0)
    {
      num16 = 0.0f;
    }
    else
    {
      float num17 = num15 * num15;
      num16 = num17 * num17 * FastNoise.GradCoord3D(seed, x1 + num6, y1 + num7, z1 + num8, xd3, yd3, zd3);
    }
    float num18 = (float) (0.60000002384185791 - (double) xd4 * (double) xd4 - (double) yd4 * (double) yd4 - (double) zd4 * (double) zd4);
    float num19;
    if ((double) num18 < 0.0)
    {
      num19 = 0.0f;
    }
    else
    {
      float num20 = num18 * num18;
      num19 = num20 * num20 * FastNoise.GradCoord3D(seed, x1 + 1, y1 + 1, z1 + 1, xd4, yd4, zd4);
    }
    return (float) (32.0 * ((double) num10 + (double) num13 + (double) num16 + (double) num19));
  }

  public float GetSimplexFractal(float x, float y)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    switch (this.m_fractalType)
    {
      case FastNoise.FractalType.FBM:
        return this.SingleSimplexFractalFBM(x, y);
      case FastNoise.FractalType.Billow:
        return this.SingleSimplexFractalBillow(x, y);
      case FastNoise.FractalType.RigidMulti:
        return this.SingleSimplexFractalRigidMulti(x, y);
      default:
        return 0.0f;
    }
  }

  private float SingleSimplexFractalFBM(float x, float y)
  {
    int seed = this.m_seed;
    float num1 = this.SingleSimplex(seed, x, y);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += this.SingleSimplex(++seed, x, y) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleSimplexFractalBillow(float x, float y)
  {
    int seed = this.m_seed;
    float num1 = (float) ((double) Math.Abs(this.SingleSimplex(seed, x, y)) * 2.0 - 1.0);
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += (float) ((double) Math.Abs(this.SingleSimplex(++seed, x, y)) * 2.0 - 1.0) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleSimplexFractalRigidMulti(float x, float y)
  {
    int seed = this.m_seed;
    float num1 = 1f - Math.Abs(this.SingleSimplex(seed, x, y));
    float num2 = 1f;
    for (int index = 1; index < this.m_octaves; ++index)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 -= (1f - Math.Abs(this.SingleSimplex(++seed, x, y))) * num2;
    }
    return num1;
  }

  public float GetSimplex(float x, float y)
  {
    return this.SingleSimplex(this.m_seed, x * this.m_frequency, y * this.m_frequency);
  }

  private float SingleSimplex(int seed, float x, float y)
  {
    float num1 = (float) (((double) x + (double) y) * 0.5);
    int x1 = FastNoise.FastFloor(x + num1);
    int y1 = FastNoise.FastFloor(y + num1);
    float num2 = (float) (x1 + y1) * 0.25f;
    float num3 = (float) x1 - num2;
    float num4 = (float) y1 - num2;
    float xd1 = x - num3;
    float yd1 = y - num4;
    int num5;
    int num6;
    if ((double) xd1 > (double) yd1)
    {
      num5 = 1;
      num6 = 0;
    }
    else
    {
      num5 = 0;
      num6 = 1;
    }
    float xd2 = (float) ((double) xd1 - (double) num5 + 0.25);
    float yd2 = (float) ((double) yd1 - (double) num6 + 0.25);
    float xd3 = (float) ((double) xd1 - 1.0 + 0.5);
    float yd3 = (float) ((double) yd1 - 1.0 + 0.5);
    float num7 = (float) (0.5 - (double) xd1 * (double) xd1 - (double) yd1 * (double) yd1);
    float num8;
    if ((double) num7 < 0.0)
    {
      num8 = 0.0f;
    }
    else
    {
      float num9 = num7 * num7;
      num8 = num9 * num9 * FastNoise.GradCoord2D(seed, x1, y1, xd1, yd1);
    }
    float num10 = (float) (0.5 - (double) xd2 * (double) xd2 - (double) yd2 * (double) yd2);
    float num11;
    if ((double) num10 < 0.0)
    {
      num11 = 0.0f;
    }
    else
    {
      float num12 = num10 * num10;
      num11 = num12 * num12 * FastNoise.GradCoord2D(seed, x1 + num5, y1 + num6, xd2, yd2);
    }
    float num13 = (float) (0.5 - (double) xd3 * (double) xd3 - (double) yd3 * (double) yd3);
    float num14;
    if ((double) num13 < 0.0)
    {
      num14 = 0.0f;
    }
    else
    {
      float num15 = num13 * num13;
      num14 = num15 * num15 * FastNoise.GradCoord2D(seed, x1 + 1, y1 + 1, xd3, yd3);
    }
    return (float) (50.0 * ((double) num8 + (double) num11 + (double) num14));
  }

  public float GetSimplex(float x, float y, float z, float w)
  {
    return this.SingleSimplex(this.m_seed, x * this.m_frequency, y * this.m_frequency, z * this.m_frequency, w * this.m_frequency);
  }

  private float SingleSimplex(int seed, float x, float y, float z, float w)
  {
    float num1 = (float) (((double) x + (double) y + (double) z + (double) w) * 0.30901700258255005);
    int x1 = FastNoise.FastFloor(x + num1);
    int y1 = FastNoise.FastFloor(y + num1);
    int z1 = FastNoise.FastFloor(z + num1);
    int w1 = FastNoise.FastFloor(w + num1);
    float num2 = (float) (x1 + y1 + z1 + w1) * 0.1381966f;
    float num3 = (float) x1 - num2;
    float num4 = (float) y1 - num2;
    float num5 = (float) z1 - num2;
    float num6 = (float) w1 - num2;
    float xd1 = x - num3;
    float yd1 = y - num4;
    float zd1 = z - num5;
    float wd1 = w - num6;
    int index1 = ((double) xd1 > (double) yd1 ? 32 /*0x20*/ : 0) + ((double) xd1 > (double) zd1 ? 16 /*0x10*/ : 0) + ((double) yd1 > (double) zd1 ? 8 : 0) + ((double) xd1 > (double) wd1 ? 4 : 0) + ((double) yd1 > (double) wd1 ? 2 : 0) + ((double) zd1 > (double) wd1 ? 1 : 0) << 2;
    int num7 = FastNoise.SIMPLEX_4D[index1] >= (byte) 3 ? 1 : 0;
    int num8 = FastNoise.SIMPLEX_4D[index1] >= (byte) 2 ? 1 : 0;
    byte[] simplex4D1 = FastNoise.SIMPLEX_4D;
    int index2 = index1;
    int index3 = index2 + 1;
    int num9 = simplex4D1[index2] >= (byte) 1 ? 1 : 0;
    int num10 = FastNoise.SIMPLEX_4D[index3] >= (byte) 3 ? 1 : 0;
    int num11 = FastNoise.SIMPLEX_4D[index3] >= (byte) 2 ? 1 : 0;
    byte[] simplex4D2 = FastNoise.SIMPLEX_4D;
    int index4 = index3;
    int index5 = index4 + 1;
    int num12 = simplex4D2[index4] >= (byte) 1 ? 1 : 0;
    int num13 = FastNoise.SIMPLEX_4D[index5] >= (byte) 3 ? 1 : 0;
    int num14 = FastNoise.SIMPLEX_4D[index5] >= (byte) 2 ? 1 : 0;
    byte[] simplex4D3 = FastNoise.SIMPLEX_4D;
    int index6 = index5;
    int index7 = index6 + 1;
    int num15 = simplex4D3[index6] >= (byte) 1 ? 1 : 0;
    int num16 = FastNoise.SIMPLEX_4D[index7] >= (byte) 3 ? 1 : 0;
    int num17 = FastNoise.SIMPLEX_4D[index7] >= (byte) 2 ? 1 : 0;
    int num18 = FastNoise.SIMPLEX_4D[index7] >= (byte) 1 ? 1 : 0;
    float xd2 = (float) ((double) xd1 - (double) num7 + 0.13819660246372223);
    float yd2 = (float) ((double) yd1 - (double) num10 + 0.13819660246372223);
    float zd2 = (float) ((double) zd1 - (double) num13 + 0.13819660246372223);
    float wd2 = (float) ((double) wd1 - (double) num16 + 0.13819660246372223);
    float xd3 = (float) ((double) xd1 - (double) num8 + 0.27639320492744446);
    float yd3 = (float) ((double) yd1 - (double) num11 + 0.27639320492744446);
    float zd3 = (float) ((double) zd1 - (double) num14 + 0.27639320492744446);
    float wd3 = (float) ((double) wd1 - (double) num17 + 0.27639320492744446);
    float xd4 = (float) ((double) xd1 - (double) num9 + 0.41458982229232788);
    float yd4 = (float) ((double) yd1 - (double) num12 + 0.41458982229232788);
    float zd4 = (float) ((double) zd1 - (double) num15 + 0.41458982229232788);
    float wd4 = (float) ((double) wd1 - (double) num18 + 0.41458982229232788);
    float xd5 = (float) ((double) xd1 - 1.0 + 0.55278640985488892);
    float yd5 = (float) ((double) yd1 - 1.0 + 0.55278640985488892);
    float zd5 = (float) ((double) zd1 - 1.0 + 0.55278640985488892);
    float wd5 = (float) ((double) wd1 - 1.0 + 0.55278640985488892);
    float num19 = (float) (0.60000002384185791 - (double) xd1 * (double) xd1 - (double) yd1 * (double) yd1 - (double) zd1 * (double) zd1 - (double) wd1 * (double) wd1);
    float num20;
    if ((double) num19 < 0.0)
    {
      num20 = 0.0f;
    }
    else
    {
      float num21 = num19 * num19;
      num20 = num21 * num21 * FastNoise.GradCoord4D(seed, x1, y1, z1, w1, xd1, yd1, zd1, wd1);
    }
    float num22 = (float) (0.60000002384185791 - (double) xd2 * (double) xd2 - (double) yd2 * (double) yd2 - (double) zd2 * (double) zd2 - (double) wd2 * (double) wd2);
    float num23;
    if ((double) num22 < 0.0)
    {
      num23 = 0.0f;
    }
    else
    {
      float num24 = num22 * num22;
      num23 = num24 * num24 * FastNoise.GradCoord4D(seed, x1 + num7, y1 + num10, z1 + num13, w1 + num16, xd2, yd2, zd2, wd2);
    }
    float num25 = (float) (0.60000002384185791 - (double) xd3 * (double) xd3 - (double) yd3 * (double) yd3 - (double) zd3 * (double) zd3 - (double) wd3 * (double) wd3);
    float num26;
    if ((double) num25 < 0.0)
    {
      num26 = 0.0f;
    }
    else
    {
      float num27 = num25 * num25;
      num26 = num27 * num27 * FastNoise.GradCoord4D(seed, x1 + num8, y1 + num11, z1 + num14, w1 + num17, xd3, yd3, zd3, wd3);
    }
    float num28 = (float) (0.60000002384185791 - (double) xd4 * (double) xd4 - (double) yd4 * (double) yd4 - (double) zd4 * (double) zd4 - (double) wd4 * (double) wd4);
    float num29;
    if ((double) num28 < 0.0)
    {
      num29 = 0.0f;
    }
    else
    {
      float num30 = num28 * num28;
      num29 = num30 * num30 * FastNoise.GradCoord4D(seed, x1 + num9, y1 + num12, z1 + num15, w1 + num18, xd4, yd4, zd4, wd4);
    }
    float num31 = (float) (0.60000002384185791 - (double) xd5 * (double) xd5 - (double) yd5 * (double) yd5 - (double) zd5 * (double) zd5 - (double) wd5 * (double) wd5);
    float num32;
    if ((double) num31 < 0.0)
    {
      num32 = 0.0f;
    }
    else
    {
      float num33 = num31 * num31;
      num32 = num33 * num33 * FastNoise.GradCoord4D(seed, x1 + 1, y1 + 1, z1 + 1, w1 + 1, xd5, yd5, zd5, wd5);
    }
    return (float) (27.0 * ((double) num20 + (double) num23 + (double) num26 + (double) num29 + (double) num32));
  }

  public float GetCubicFractal(float x, float y, float z)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    z *= this.m_frequency;
    switch (this.m_fractalType)
    {
      case FastNoise.FractalType.FBM:
        return this.SingleCubicFractalFBM(x, y, z);
      case FastNoise.FractalType.Billow:
        return this.SingleCubicFractalBillow(x, y, z);
      case FastNoise.FractalType.RigidMulti:
        return this.SingleCubicFractalRigidMulti(x, y, z);
      default:
        return 0.0f;
    }
  }

  private float SingleCubicFractalFBM(float x, float y, float z)
  {
    int seed = this.m_seed;
    float num1 = this.SingleCubic(seed, x, y, z);
    float num2 = 1f;
    int num3 = 0;
    while (++num3 < this.m_octaves)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += this.SingleCubic(++seed, x, y, z) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleCubicFractalBillow(float x, float y, float z)
  {
    int seed = this.m_seed;
    float num1 = (float) ((double) Math.Abs(this.SingleCubic(seed, x, y, z)) * 2.0 - 1.0);
    float num2 = 1f;
    int num3 = 0;
    while (++num3 < this.m_octaves)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += (float) ((double) Math.Abs(this.SingleCubic(++seed, x, y, z)) * 2.0 - 1.0) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleCubicFractalRigidMulti(float x, float y, float z)
  {
    int seed = this.m_seed;
    float num1 = 1f - Math.Abs(this.SingleCubic(seed, x, y, z));
    float num2 = 1f;
    int num3 = 0;
    while (++num3 < this.m_octaves)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      z *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 -= (1f - Math.Abs(this.SingleCubic(++seed, x, y, z))) * num2;
    }
    return num1;
  }

  public float GetCubic(float x, float y, float z)
  {
    return this.SingleCubic(this.m_seed, x * this.m_frequency, y * this.m_frequency, z * this.m_frequency);
  }

  private float SingleCubic(int seed, float x, float y, float z)
  {
    int x1 = FastNoise.FastFloor(x);
    int y1 = FastNoise.FastFloor(y);
    int z1 = FastNoise.FastFloor(z);
    int x2 = x1 - 1;
    int y2 = y1 - 1;
    int z2 = z1 - 1;
    int x3 = x1 + 1;
    int y3 = y1 + 1;
    int z3 = z1 + 1;
    int x4 = x1 + 2;
    int y4 = y1 + 2;
    int z4 = z1 + 2;
    float t1 = x - (float) x1;
    float t2 = y - (float) y1;
    float t3 = z - (float) z1;
    return FastNoise.CubicLerp(FastNoise.CubicLerp(FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y2, z2), FastNoise.ValCoord3D(seed, x1, y2, z2), FastNoise.ValCoord3D(seed, x3, y2, z2), FastNoise.ValCoord3D(seed, x4, y2, z2), t1), FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y1, z2), FastNoise.ValCoord3D(seed, x1, y1, z2), FastNoise.ValCoord3D(seed, x3, y1, z2), FastNoise.ValCoord3D(seed, x4, y1, z2), t1), FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y3, z2), FastNoise.ValCoord3D(seed, x1, y3, z2), FastNoise.ValCoord3D(seed, x3, y3, z2), FastNoise.ValCoord3D(seed, x4, y3, z2), t1), FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y4, z2), FastNoise.ValCoord3D(seed, x1, y4, z2), FastNoise.ValCoord3D(seed, x3, y4, z2), FastNoise.ValCoord3D(seed, x4, y4, z2), t1), t2), FastNoise.CubicLerp(FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y2, z1), FastNoise.ValCoord3D(seed, x1, y2, z1), FastNoise.ValCoord3D(seed, x3, y2, z1), FastNoise.ValCoord3D(seed, x4, y2, z1), t1), FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y1, z1), FastNoise.ValCoord3D(seed, x1, y1, z1), FastNoise.ValCoord3D(seed, x3, y1, z1), FastNoise.ValCoord3D(seed, x4, y1, z1), t1), FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y3, z1), FastNoise.ValCoord3D(seed, x1, y3, z1), FastNoise.ValCoord3D(seed, x3, y3, z1), FastNoise.ValCoord3D(seed, x4, y3, z1), t1), FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y4, z1), FastNoise.ValCoord3D(seed, x1, y4, z1), FastNoise.ValCoord3D(seed, x3, y4, z1), FastNoise.ValCoord3D(seed, x4, y4, z1), t1), t2), FastNoise.CubicLerp(FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y2, z3), FastNoise.ValCoord3D(seed, x1, y2, z3), FastNoise.ValCoord3D(seed, x3, y2, z3), FastNoise.ValCoord3D(seed, x4, y2, z3), t1), FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y1, z3), FastNoise.ValCoord3D(seed, x1, y1, z3), FastNoise.ValCoord3D(seed, x3, y1, z3), FastNoise.ValCoord3D(seed, x4, y1, z3), t1), FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y3, z3), FastNoise.ValCoord3D(seed, x1, y3, z3), FastNoise.ValCoord3D(seed, x3, y3, z3), FastNoise.ValCoord3D(seed, x4, y3, z3), t1), FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y4, z3), FastNoise.ValCoord3D(seed, x1, y4, z3), FastNoise.ValCoord3D(seed, x3, y4, z3), FastNoise.ValCoord3D(seed, x4, y4, z3), t1), t2), FastNoise.CubicLerp(FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y2, z4), FastNoise.ValCoord3D(seed, x1, y2, z4), FastNoise.ValCoord3D(seed, x3, y2, z4), FastNoise.ValCoord3D(seed, x4, y2, z4), t1), FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y1, z4), FastNoise.ValCoord3D(seed, x1, y1, z4), FastNoise.ValCoord3D(seed, x3, y1, z4), FastNoise.ValCoord3D(seed, x4, y1, z4), t1), FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y3, z4), FastNoise.ValCoord3D(seed, x1, y3, z4), FastNoise.ValCoord3D(seed, x3, y3, z4), FastNoise.ValCoord3D(seed, x4, y3, z4), t1), FastNoise.CubicLerp(FastNoise.ValCoord3D(seed, x2, y4, z4), FastNoise.ValCoord3D(seed, x1, y4, z4), FastNoise.ValCoord3D(seed, x3, y4, z4), FastNoise.ValCoord3D(seed, x4, y4, z4), t1), t2), t3) * 0.2962963f;
  }

  public float GetCubicFractal(float x, float y)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    switch (this.m_fractalType)
    {
      case FastNoise.FractalType.FBM:
        return this.SingleCubicFractalFBM(x, y);
      case FastNoise.FractalType.Billow:
        return this.SingleCubicFractalBillow(x, y);
      case FastNoise.FractalType.RigidMulti:
        return this.SingleCubicFractalRigidMulti(x, y);
      default:
        return 0.0f;
    }
  }

  private float SingleCubicFractalFBM(float x, float y)
  {
    int seed = this.m_seed;
    float num1 = this.SingleCubic(seed, x, y);
    float num2 = 1f;
    int num3 = 0;
    while (++num3 < this.m_octaves)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += this.SingleCubic(++seed, x, y) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleCubicFractalBillow(float x, float y)
  {
    int seed = this.m_seed;
    float num1 = (float) ((double) Math.Abs(this.SingleCubic(seed, x, y)) * 2.0 - 1.0);
    float num2 = 1f;
    int num3 = 0;
    while (++num3 < this.m_octaves)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 += (float) ((double) Math.Abs(this.SingleCubic(++seed, x, y)) * 2.0 - 1.0) * num2;
    }
    return num1 * this.m_fractalBounding;
  }

  private float SingleCubicFractalRigidMulti(float x, float y)
  {
    int seed = this.m_seed;
    float num1 = 1f - Math.Abs(this.SingleCubic(seed, x, y));
    float num2 = 1f;
    int num3 = 0;
    while (++num3 < this.m_octaves)
    {
      x *= this.m_lacunarity;
      y *= this.m_lacunarity;
      num2 *= this.m_gain;
      num1 -= (1f - Math.Abs(this.SingleCubic(++seed, x, y))) * num2;
    }
    return num1;
  }

  public float GetCubic(float x, float y)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    return this.SingleCubic(0, x, y);
  }

  private float SingleCubic(int seed, float x, float y)
  {
    int x1 = FastNoise.FastFloor(x);
    int y1 = FastNoise.FastFloor(y);
    int x2 = x1 - 1;
    int y2 = y1 - 1;
    int x3 = x1 + 1;
    int y3 = y1 + 1;
    int x4 = x1 + 2;
    int y4 = y1 + 2;
    float t1 = x - (float) x1;
    float t2 = y - (float) y1;
    return FastNoise.CubicLerp(FastNoise.CubicLerp(FastNoise.ValCoord2D(seed, x2, y2), FastNoise.ValCoord2D(seed, x1, y2), FastNoise.ValCoord2D(seed, x3, y2), FastNoise.ValCoord2D(seed, x4, y2), t1), FastNoise.CubicLerp(FastNoise.ValCoord2D(seed, x2, y1), FastNoise.ValCoord2D(seed, x1, y1), FastNoise.ValCoord2D(seed, x3, y1), FastNoise.ValCoord2D(seed, x4, y1), t1), FastNoise.CubicLerp(FastNoise.ValCoord2D(seed, x2, y3), FastNoise.ValCoord2D(seed, x1, y3), FastNoise.ValCoord2D(seed, x3, y3), FastNoise.ValCoord2D(seed, x4, y3), t1), FastNoise.CubicLerp(FastNoise.ValCoord2D(seed, x2, y4), FastNoise.ValCoord2D(seed, x1, y4), FastNoise.ValCoord2D(seed, x3, y4), FastNoise.ValCoord2D(seed, x4, y4), t1), t2) * 0.444444448f;
  }

  public float GetCellular(float x, float y, float z)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    z *= this.m_frequency;
    return (uint) this.m_cellularReturnType <= 2U ? this.SingleCellular(x, y, z) : this.SingleCellular2Edge(x, y, z);
  }

  private float SingleCellular(float x, float y, float z)
  {
    int num1 = FastNoise.FastRound(x);
    int num2 = FastNoise.FastRound(y);
    int num3 = FastNoise.FastRound(z);
    float num4 = 999999f;
    int x1 = 0;
    int y1 = 0;
    int z1 = 0;
    switch (this.m_cellularDistanceFunction)
    {
      case FastNoise.CellularDistanceFunction.Euclidean:
        for (int x2 = num1 - 1; x2 <= num1 + 1; ++x2)
        {
          for (int y2 = num2 - 1; y2 <= num2 + 1; ++y2)
          {
            for (int z2 = num3 - 1; z2 <= num3 + 1; ++z2)
            {
              FastNoise.Float3 float3 = FastNoise.CELL_3D[FastNoise.Hash3D(this.m_seed, x2, y2, z2) & (int) byte.MaxValue];
              double num5 = (double) x2 - (double) x + (double) float3.x * (double) this.m_cellularJitter;
              float num6 = (float) ((double) y2 - (double) y + (double) float3.y * (double) this.m_cellularJitter);
              float num7 = (float) ((double) z2 - (double) z + (double) float3.z * (double) this.m_cellularJitter);
              float num8 = (float) (num5 * num5 + (double) num6 * (double) num6 + (double) num7 * (double) num7);
              if ((double) num8 < (double) num4)
              {
                num4 = num8;
                x1 = x2;
                y1 = y2;
                z1 = z2;
              }
            }
          }
        }
        break;
      case FastNoise.CellularDistanceFunction.Manhattan:
        for (int x3 = num1 - 1; x3 <= num1 + 1; ++x3)
        {
          for (int y3 = num2 - 1; y3 <= num2 + 1; ++y3)
          {
            for (int z3 = num3 - 1; z3 <= num3 + 1; ++z3)
            {
              FastNoise.Float3 float3 = FastNoise.CELL_3D[FastNoise.Hash3D(this.m_seed, x3, y3, z3) & (int) byte.MaxValue];
              double num9 = (double) x3 - (double) x + (double) float3.x * (double) this.m_cellularJitter;
              float num10 = (float) ((double) y3 - (double) y + (double) float3.y * (double) this.m_cellularJitter);
              float num11 = (float) ((double) z3 - (double) z + (double) float3.z * (double) this.m_cellularJitter);
              float num12 = Math.Abs((float) num9) + Math.Abs(num10) + Math.Abs(num11);
              if ((double) num12 < (double) num4)
              {
                num4 = num12;
                x1 = x3;
                y1 = y3;
                z1 = z3;
              }
            }
          }
        }
        break;
      case FastNoise.CellularDistanceFunction.Natural:
        for (int x4 = num1 - 1; x4 <= num1 + 1; ++x4)
        {
          for (int y4 = num2 - 1; y4 <= num2 + 1; ++y4)
          {
            for (int z4 = num3 - 1; z4 <= num3 + 1; ++z4)
            {
              FastNoise.Float3 float3 = FastNoise.CELL_3D[FastNoise.Hash3D(this.m_seed, x4, y4, z4) & (int) byte.MaxValue];
              float num13 = (float) ((double) x4 - (double) x + (double) float3.x * (double) this.m_cellularJitter);
              float num14 = (float) ((double) y4 - (double) y + (double) float3.y * (double) this.m_cellularJitter);
              float num15 = (float) ((double) z4 - (double) z + (double) float3.z * (double) this.m_cellularJitter);
              float num16 = (float) ((double) Math.Abs(num13) + (double) Math.Abs(num14) + (double) Math.Abs(num15) + ((double) num13 * (double) num13 + (double) num14 * (double) num14 + (double) num15 * (double) num15));
              if ((double) num16 < (double) num4)
              {
                num4 = num16;
                x1 = x4;
                y1 = y4;
                z1 = z4;
              }
            }
          }
        }
        break;
    }
    switch (this.m_cellularReturnType)
    {
      case FastNoise.CellularReturnType.CellValue:
        return FastNoise.ValCoord3D(this.m_seed, x1, y1, z1);
      case FastNoise.CellularReturnType.NoiseLookup:
        FastNoise.Float3 float3_1 = FastNoise.CELL_3D[FastNoise.Hash3D(this.m_seed, x1, y1, z1) & (int) byte.MaxValue];
        return this.m_cellularNoiseLookup.GetNoise((float) x1 + float3_1.x * this.m_cellularJitter, (float) y1 + float3_1.y * this.m_cellularJitter, (float) z1 + float3_1.z * this.m_cellularJitter);
      case FastNoise.CellularReturnType.Distance:
        return num4;
      default:
        return 0.0f;
    }
  }

  private float SingleCellular2Edge(float x, float y, float z)
  {
    int num1 = FastNoise.FastRound(x);
    int num2 = FastNoise.FastRound(y);
    int num3 = FastNoise.FastRound(z);
    float[] numArray = new float[4]
    {
      999999f,
      999999f,
      999999f,
      999999f
    };
    switch (this.m_cellularDistanceFunction)
    {
      case FastNoise.CellularDistanceFunction.Euclidean:
        for (int x1 = num1 - 1; x1 <= num1 + 1; ++x1)
        {
          for (int y1 = num2 - 1; y1 <= num2 + 1; ++y1)
          {
            for (int z1 = num3 - 1; z1 <= num3 + 1; ++z1)
            {
              FastNoise.Float3 float3 = FastNoise.CELL_3D[FastNoise.Hash3D(this.m_seed, x1, y1, z1) & (int) byte.MaxValue];
              double num4 = (double) x1 - (double) x + (double) float3.x * (double) this.m_cellularJitter;
              float num5 = (float) ((double) y1 - (double) y + (double) float3.y * (double) this.m_cellularJitter);
              float num6 = (float) ((double) z1 - (double) z + (double) float3.z * (double) this.m_cellularJitter);
              float val2 = (float) (num4 * num4 + (double) num5 * (double) num5 + (double) num6 * (double) num6);
              for (int cellularDistanceIndex1 = this.m_cellularDistanceIndex1; cellularDistanceIndex1 > 0; --cellularDistanceIndex1)
                numArray[cellularDistanceIndex1] = Math.Max(Math.Min(numArray[cellularDistanceIndex1], val2), numArray[cellularDistanceIndex1 - 1]);
              numArray[0] = Math.Min(numArray[0], val2);
            }
          }
        }
        break;
      case FastNoise.CellularDistanceFunction.Manhattan:
        for (int x2 = num1 - 1; x2 <= num1 + 1; ++x2)
        {
          for (int y2 = num2 - 1; y2 <= num2 + 1; ++y2)
          {
            for (int z2 = num3 - 1; z2 <= num3 + 1; ++z2)
            {
              FastNoise.Float3 float3 = FastNoise.CELL_3D[FastNoise.Hash3D(this.m_seed, x2, y2, z2) & (int) byte.MaxValue];
              double num7 = (double) x2 - (double) x + (double) float3.x * (double) this.m_cellularJitter;
              float num8 = (float) ((double) y2 - (double) y + (double) float3.y * (double) this.m_cellularJitter);
              float num9 = (float) ((double) z2 - (double) z + (double) float3.z * (double) this.m_cellularJitter);
              float val2 = Math.Abs((float) num7) + Math.Abs(num8) + Math.Abs(num9);
              for (int cellularDistanceIndex1 = this.m_cellularDistanceIndex1; cellularDistanceIndex1 > 0; --cellularDistanceIndex1)
                numArray[cellularDistanceIndex1] = Math.Max(Math.Min(numArray[cellularDistanceIndex1], val2), numArray[cellularDistanceIndex1 - 1]);
              numArray[0] = Math.Min(numArray[0], val2);
            }
          }
        }
        break;
      case FastNoise.CellularDistanceFunction.Natural:
        for (int x3 = num1 - 1; x3 <= num1 + 1; ++x3)
        {
          for (int y3 = num2 - 1; y3 <= num2 + 1; ++y3)
          {
            for (int z3 = num3 - 1; z3 <= num3 + 1; ++z3)
            {
              FastNoise.Float3 float3 = FastNoise.CELL_3D[FastNoise.Hash3D(this.m_seed, x3, y3, z3) & (int) byte.MaxValue];
              float num10 = (float) ((double) x3 - (double) x + (double) float3.x * (double) this.m_cellularJitter);
              float num11 = (float) ((double) y3 - (double) y + (double) float3.y * (double) this.m_cellularJitter);
              float num12 = (float) ((double) z3 - (double) z + (double) float3.z * (double) this.m_cellularJitter);
              float val2 = (float) ((double) Math.Abs(num10) + (double) Math.Abs(num11) + (double) Math.Abs(num12) + ((double) num10 * (double) num10 + (double) num11 * (double) num11 + (double) num12 * (double) num12));
              for (int cellularDistanceIndex1 = this.m_cellularDistanceIndex1; cellularDistanceIndex1 > 0; --cellularDistanceIndex1)
                numArray[cellularDistanceIndex1] = Math.Max(Math.Min(numArray[cellularDistanceIndex1], val2), numArray[cellularDistanceIndex1 - 1]);
              numArray[0] = Math.Min(numArray[0], val2);
            }
          }
        }
        break;
    }
    switch (this.m_cellularReturnType)
    {
      case FastNoise.CellularReturnType.Distance2:
        return numArray[this.m_cellularDistanceIndex1];
      case FastNoise.CellularReturnType.Distance2Add:
        return numArray[this.m_cellularDistanceIndex1] + numArray[this.m_cellularDistanceIndex0];
      case FastNoise.CellularReturnType.Distance2Sub:
        return numArray[this.m_cellularDistanceIndex1] - numArray[this.m_cellularDistanceIndex0];
      case FastNoise.CellularReturnType.Distance2Mul:
        return numArray[this.m_cellularDistanceIndex1] * numArray[this.m_cellularDistanceIndex0];
      case FastNoise.CellularReturnType.Distance2Div:
        return numArray[this.m_cellularDistanceIndex0] / numArray[this.m_cellularDistanceIndex1];
      default:
        return 0.0f;
    }
  }

  public float GetCellular(float x, float y)
  {
    x *= this.m_frequency;
    y *= this.m_frequency;
    return (uint) this.m_cellularReturnType <= 2U ? this.SingleCellular(x, y) : this.SingleCellular2Edge(x, y);
  }

  private float SingleCellular(float x, float y)
  {
    int num1 = FastNoise.FastRound(x);
    int num2 = FastNoise.FastRound(y);
    float num3 = 999999f;
    int x1 = 0;
    int y1 = 0;
    switch (this.m_cellularDistanceFunction)
    {
      case FastNoise.CellularDistanceFunction.Manhattan:
        for (int x2 = num1 - 1; x2 <= num1 + 1; ++x2)
        {
          for (int y2 = num2 - 1; y2 <= num2 + 1; ++y2)
          {
            FastNoise.Float2 float2 = FastNoise.CELL_2D[FastNoise.Hash2D(this.m_seed, x2, y2) & (int) byte.MaxValue];
            double num4 = (double) x2 - (double) x + (double) float2.x * (double) this.m_cellularJitter;
            float num5 = (float) ((double) y2 - (double) y + (double) float2.y * (double) this.m_cellularJitter);
            float num6 = Math.Abs((float) num4) + Math.Abs(num5);
            if ((double) num6 < (double) num3)
            {
              num3 = num6;
              x1 = x2;
              y1 = y2;
            }
          }
        }
        break;
      case FastNoise.CellularDistanceFunction.Natural:
        for (int x3 = num1 - 1; x3 <= num1 + 1; ++x3)
        {
          for (int y3 = num2 - 1; y3 <= num2 + 1; ++y3)
          {
            FastNoise.Float2 float2 = FastNoise.CELL_2D[FastNoise.Hash2D(this.m_seed, x3, y3) & (int) byte.MaxValue];
            float num7 = (float) ((double) x3 - (double) x + (double) float2.x * (double) this.m_cellularJitter);
            float num8 = (float) ((double) y3 - (double) y + (double) float2.y * (double) this.m_cellularJitter);
            float num9 = (float) ((double) Math.Abs(num7) + (double) Math.Abs(num8) + ((double) num7 * (double) num7 + (double) num8 * (double) num8));
            if ((double) num9 < (double) num3)
            {
              num3 = num9;
              x1 = x3;
              y1 = y3;
            }
          }
        }
        break;
      default:
        for (int x4 = num1 - 1; x4 <= num1 + 1; ++x4)
        {
          for (int y4 = num2 - 1; y4 <= num2 + 1; ++y4)
          {
            FastNoise.Float2 float2 = FastNoise.CELL_2D[FastNoise.Hash2D(this.m_seed, x4, y4) & (int) byte.MaxValue];
            double num10 = (double) x4 - (double) x + (double) float2.x * (double) this.m_cellularJitter;
            float num11 = (float) ((double) y4 - (double) y + (double) float2.y * (double) this.m_cellularJitter);
            float num12 = (float) (num10 * num10 + (double) num11 * (double) num11);
            if ((double) num12 < (double) num3)
            {
              num3 = num12;
              x1 = x4;
              y1 = y4;
            }
          }
        }
        break;
    }
    switch (this.m_cellularReturnType)
    {
      case FastNoise.CellularReturnType.CellValue:
        return FastNoise.ValCoord2D(this.m_seed, x1, y1);
      case FastNoise.CellularReturnType.NoiseLookup:
        FastNoise.Float2 float2_1 = FastNoise.CELL_2D[FastNoise.Hash2D(this.m_seed, x1, y1) & (int) byte.MaxValue];
        return this.m_cellularNoiseLookup.GetNoise((float) x1 + float2_1.x * this.m_cellularJitter, (float) y1 + float2_1.y * this.m_cellularJitter);
      case FastNoise.CellularReturnType.Distance:
        return num3;
      default:
        return 0.0f;
    }
  }

  private float SingleCellular2Edge(float x, float y)
  {
    int num1 = FastNoise.FastRound(x);
    int num2 = FastNoise.FastRound(y);
    float[] numArray = new float[4]
    {
      999999f,
      999999f,
      999999f,
      999999f
    };
    switch (this.m_cellularDistanceFunction)
    {
      case FastNoise.CellularDistanceFunction.Manhattan:
        for (int x1 = num1 - 1; x1 <= num1 + 1; ++x1)
        {
          for (int y1 = num2 - 1; y1 <= num2 + 1; ++y1)
          {
            FastNoise.Float2 float2 = FastNoise.CELL_2D[FastNoise.Hash2D(this.m_seed, x1, y1) & (int) byte.MaxValue];
            double num3 = (double) x1 - (double) x + (double) float2.x * (double) this.m_cellularJitter;
            float num4 = (float) ((double) y1 - (double) y + (double) float2.y * (double) this.m_cellularJitter);
            float val2 = Math.Abs((float) num3) + Math.Abs(num4);
            for (int cellularDistanceIndex1 = this.m_cellularDistanceIndex1; cellularDistanceIndex1 > 0; --cellularDistanceIndex1)
              numArray[cellularDistanceIndex1] = Math.Max(Math.Min(numArray[cellularDistanceIndex1], val2), numArray[cellularDistanceIndex1 - 1]);
            numArray[0] = Math.Min(numArray[0], val2);
          }
        }
        break;
      case FastNoise.CellularDistanceFunction.Natural:
        for (int x2 = num1 - 1; x2 <= num1 + 1; ++x2)
        {
          for (int y2 = num2 - 1; y2 <= num2 + 1; ++y2)
          {
            FastNoise.Float2 float2 = FastNoise.CELL_2D[FastNoise.Hash2D(this.m_seed, x2, y2) & (int) byte.MaxValue];
            float num5 = (float) ((double) x2 - (double) x + (double) float2.x * (double) this.m_cellularJitter);
            float num6 = (float) ((double) y2 - (double) y + (double) float2.y * (double) this.m_cellularJitter);
            float val2 = (float) ((double) Math.Abs(num5) + (double) Math.Abs(num6) + ((double) num5 * (double) num5 + (double) num6 * (double) num6));
            for (int cellularDistanceIndex1 = this.m_cellularDistanceIndex1; cellularDistanceIndex1 > 0; --cellularDistanceIndex1)
              numArray[cellularDistanceIndex1] = Math.Max(Math.Min(numArray[cellularDistanceIndex1], val2), numArray[cellularDistanceIndex1 - 1]);
            numArray[0] = Math.Min(numArray[0], val2);
          }
        }
        break;
      default:
        for (int x3 = num1 - 1; x3 <= num1 + 1; ++x3)
        {
          for (int y3 = num2 - 1; y3 <= num2 + 1; ++y3)
          {
            FastNoise.Float2 float2 = FastNoise.CELL_2D[FastNoise.Hash2D(this.m_seed, x3, y3) & (int) byte.MaxValue];
            double num7 = (double) x3 - (double) x + (double) float2.x * (double) this.m_cellularJitter;
            float num8 = (float) ((double) y3 - (double) y + (double) float2.y * (double) this.m_cellularJitter);
            float val2 = (float) (num7 * num7 + (double) num8 * (double) num8);
            for (int cellularDistanceIndex1 = this.m_cellularDistanceIndex1; cellularDistanceIndex1 > 0; --cellularDistanceIndex1)
              numArray[cellularDistanceIndex1] = Math.Max(Math.Min(numArray[cellularDistanceIndex1], val2), numArray[cellularDistanceIndex1 - 1]);
            numArray[0] = Math.Min(numArray[0], val2);
          }
        }
        break;
    }
    switch (this.m_cellularReturnType)
    {
      case FastNoise.CellularReturnType.Distance2:
        return numArray[this.m_cellularDistanceIndex1];
      case FastNoise.CellularReturnType.Distance2Add:
        return numArray[this.m_cellularDistanceIndex1] + numArray[this.m_cellularDistanceIndex0];
      case FastNoise.CellularReturnType.Distance2Sub:
        return numArray[this.m_cellularDistanceIndex1] - numArray[this.m_cellularDistanceIndex0];
      case FastNoise.CellularReturnType.Distance2Mul:
        return numArray[this.m_cellularDistanceIndex1] * numArray[this.m_cellularDistanceIndex0];
      case FastNoise.CellularReturnType.Distance2Div:
        return numArray[this.m_cellularDistanceIndex0] / numArray[this.m_cellularDistanceIndex1];
      default:
        return 0.0f;
    }
  }

  public void GradientPerturb(ref float x, ref float y, ref float z)
  {
    this.SingleGradientPerturb(this.m_seed, this.m_gradientPerturbAmp, this.m_frequency, ref x, ref y, ref z);
  }

  public void GradientPerturbFractal(ref float x, ref float y, ref float z)
  {
    int seed = this.m_seed;
    float perturbAmp = this.m_gradientPerturbAmp * this.m_fractalBounding;
    float frequency = this.m_frequency;
    this.SingleGradientPerturb(seed, perturbAmp, this.m_frequency, ref x, ref y, ref z);
    for (int index = 1; index < this.m_octaves; ++index)
    {
      frequency *= this.m_lacunarity;
      perturbAmp *= this.m_gain;
      this.SingleGradientPerturb(++seed, perturbAmp, frequency, ref x, ref y, ref z);
    }
  }

  private void SingleGradientPerturb(
    int seed,
    float perturbAmp,
    float frequency,
    ref float x,
    ref float y,
    ref float z)
  {
    float f1 = x * frequency;
    float f2 = y * frequency;
    float f3 = z * frequency;
    int x1 = FastNoise.FastFloor(f1);
    int y1 = FastNoise.FastFloor(f2);
    int z1 = FastNoise.FastFloor(f3);
    int x2 = x1 + 1;
    int y2 = y1 + 1;
    int z2 = z1 + 1;
    float t1;
    float t2;
    float t3;
    switch (this.m_interp)
    {
      case FastNoise.Interp.Hermite:
        t1 = FastNoise.InterpHermiteFunc(f1 - (float) x1);
        t2 = FastNoise.InterpHermiteFunc(f2 - (float) y1);
        t3 = FastNoise.InterpHermiteFunc(f3 - (float) z1);
        break;
      case FastNoise.Interp.Quintic:
        t1 = FastNoise.InterpQuinticFunc(f1 - (float) x1);
        t2 = FastNoise.InterpQuinticFunc(f2 - (float) y1);
        t3 = FastNoise.InterpQuinticFunc(f3 - (float) z1);
        break;
      default:
        t1 = f1 - (float) x1;
        t2 = f2 - (float) y1;
        t3 = f3 - (float) z1;
        break;
    }
    FastNoise.Float3 float3_1 = FastNoise.CELL_3D[FastNoise.Hash3D(seed, x1, y1, z1) & (int) byte.MaxValue];
    FastNoise.Float3 float3_2 = FastNoise.CELL_3D[FastNoise.Hash3D(seed, x2, y1, z1) & (int) byte.MaxValue];
    float a1 = FastNoise.Lerp(float3_1.x, float3_2.x, t1);
    float a2 = FastNoise.Lerp(float3_1.y, float3_2.y, t1);
    float a3 = FastNoise.Lerp(float3_1.z, float3_2.z, t1);
    FastNoise.Float3 float3_3 = FastNoise.CELL_3D[FastNoise.Hash3D(seed, x1, y2, z1) & (int) byte.MaxValue];
    FastNoise.Float3 float3_4 = FastNoise.CELL_3D[FastNoise.Hash3D(seed, x2, y2, z1) & (int) byte.MaxValue];
    float b1 = FastNoise.Lerp(float3_3.x, float3_4.x, t1);
    float b2 = FastNoise.Lerp(float3_3.y, float3_4.y, t1);
    float b3 = FastNoise.Lerp(float3_3.z, float3_4.z, t1);
    float a4 = FastNoise.Lerp(a1, b1, t2);
    float a5 = FastNoise.Lerp(a2, b2, t2);
    float a6 = FastNoise.Lerp(a3, b3, t2);
    FastNoise.Float3 float3_5 = FastNoise.CELL_3D[FastNoise.Hash3D(seed, x1, y1, z2) & (int) byte.MaxValue];
    FastNoise.Float3 float3_6 = FastNoise.CELL_3D[FastNoise.Hash3D(seed, x2, y1, z2) & (int) byte.MaxValue];
    float a7 = FastNoise.Lerp(float3_5.x, float3_6.x, t1);
    float a8 = FastNoise.Lerp(float3_5.y, float3_6.y, t1);
    float a9 = FastNoise.Lerp(float3_5.z, float3_6.z, t1);
    FastNoise.Float3 float3_7 = FastNoise.CELL_3D[FastNoise.Hash3D(seed, x1, y2, z2) & (int) byte.MaxValue];
    FastNoise.Float3 float3_8 = FastNoise.CELL_3D[FastNoise.Hash3D(seed, x2, y2, z2) & (int) byte.MaxValue];
    float b4 = FastNoise.Lerp(float3_7.x, float3_8.x, t1);
    float b5 = FastNoise.Lerp(float3_7.y, float3_8.y, t1);
    float b6 = FastNoise.Lerp(float3_7.z, float3_8.z, t1);
    x += FastNoise.Lerp(a4, FastNoise.Lerp(a7, b4, t2), t3) * perturbAmp;
    y += FastNoise.Lerp(a5, FastNoise.Lerp(a8, b5, t2), t3) * perturbAmp;
    z += FastNoise.Lerp(a6, FastNoise.Lerp(a9, b6, t2), t3) * perturbAmp;
  }

  public void GradientPerturb(ref float x, ref float y)
  {
    this.SingleGradientPerturb(this.m_seed, this.m_gradientPerturbAmp, this.m_frequency, ref x, ref y);
  }

  public void GradientPerturbFractal(ref float x, ref float y)
  {
    int seed = this.m_seed;
    float perturbAmp = this.m_gradientPerturbAmp * this.m_fractalBounding;
    float frequency = this.m_frequency;
    this.SingleGradientPerturb(seed, perturbAmp, this.m_frequency, ref x, ref y);
    for (int index = 1; index < this.m_octaves; ++index)
    {
      frequency *= this.m_lacunarity;
      perturbAmp *= this.m_gain;
      this.SingleGradientPerturb(++seed, perturbAmp, frequency, ref x, ref y);
    }
  }

  private void SingleGradientPerturb(
    int seed,
    float perturbAmp,
    float frequency,
    ref float x,
    ref float y)
  {
    float f1 = x * frequency;
    float f2 = y * frequency;
    int x1 = FastNoise.FastFloor(f1);
    int y1 = FastNoise.FastFloor(f2);
    int x2 = x1 + 1;
    int y2 = y1 + 1;
    float t1;
    float t2;
    switch (this.m_interp)
    {
      case FastNoise.Interp.Hermite:
        t1 = FastNoise.InterpHermiteFunc(f1 - (float) x1);
        t2 = FastNoise.InterpHermiteFunc(f2 - (float) y1);
        break;
      case FastNoise.Interp.Quintic:
        t1 = FastNoise.InterpQuinticFunc(f1 - (float) x1);
        t2 = FastNoise.InterpQuinticFunc(f2 - (float) y1);
        break;
      default:
        t1 = f1 - (float) x1;
        t2 = f2 - (float) y1;
        break;
    }
    FastNoise.Float2 float2_1 = FastNoise.CELL_2D[FastNoise.Hash2D(seed, x1, y1) & (int) byte.MaxValue];
    FastNoise.Float2 float2_2 = FastNoise.CELL_2D[FastNoise.Hash2D(seed, x2, y1) & (int) byte.MaxValue];
    float a1 = FastNoise.Lerp(float2_1.x, float2_2.x, t1);
    float a2 = FastNoise.Lerp(float2_1.y, float2_2.y, t1);
    FastNoise.Float2 float2_3 = FastNoise.CELL_2D[FastNoise.Hash2D(seed, x1, y2) & (int) byte.MaxValue];
    FastNoise.Float2 float2_4 = FastNoise.CELL_2D[FastNoise.Hash2D(seed, x2, y2) & (int) byte.MaxValue];
    float b1 = FastNoise.Lerp(float2_3.x, float2_4.x, t1);
    float b2 = FastNoise.Lerp(float2_3.y, float2_4.y, t1);
    x += FastNoise.Lerp(a1, b1, t2) * perturbAmp;
    y += FastNoise.Lerp(a2, b2, t2) * perturbAmp;
  }

  public enum NoiseType : byte
  {
    Value,
    ValueFractal,
    Perlin,
    PerlinFractal,
    Simplex,
    SimplexFractal,
    Cellular,
    WhiteNoise,
    Cubic,
    CubicFractal,
  }

  public enum Interp : byte
  {
    Linear,
    Hermite,
    Quintic,
  }

  public enum FractalType : byte
  {
    FBM,
    Billow,
    RigidMulti,
  }

  public enum CellularDistanceFunction : byte
  {
    Euclidean,
    Manhattan,
    Natural,
  }

  public enum CellularReturnType : byte
  {
    CellValue,
    NoiseLookup,
    Distance,
    Distance2,
    Distance2Add,
    Distance2Sub,
    Distance2Mul,
    Distance2Div,
  }

  private struct Float2(float x, float y)
  {
    public readonly float x = x;
    public readonly float y = y;
  }

  private struct Float3(float x, float y, float z)
  {
    public readonly float x = x;
    public readonly float y = y;
    public readonly float z = z;
  }
}
