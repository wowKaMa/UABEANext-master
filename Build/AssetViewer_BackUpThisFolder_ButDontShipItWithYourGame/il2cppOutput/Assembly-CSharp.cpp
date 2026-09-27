#include "pch-cpp.hpp"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <limits>


template <typename R>
struct VirtualFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename R, typename T1, typename T2, typename T3>
struct VirtualFuncInvoker3
{
	typedef R (*Func)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
struct InterfaceActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename R>
struct InterfaceFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};

struct AsyncLocal_1_t1D3339EA4C8650D2DEDDF9553E5C932B3DC2CCFD;
struct ConcurrentQueue_1_t38466BAF2E0933882FAC022407D638F7900CEED4;
struct ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E;
struct Dictionary_2_t87EDE08B2E48F793A22DE50D6B3CC2E7EBB2DB54;
struct HashSet_1_t2F33BEB06EEA4A872E2FAF464382422AA39AE885;
struct HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D;
struct IEqualityComparer_1_t346B5F8DCA5E192F9F86F529BF32D50681423E35;
struct Segment_t3ABE51CD8494216053375DA492F57A0F6F8423BC;
struct SlotU5BU5D_t15DCE773C3630ADE0EE26EE2939F4EB88FDE2D21;
struct ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031;
struct CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB;
struct DelegateU5BU5D_tC5AB7E8F745616680F337909D3A8E6C722CDF771;
struct GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF;
struct Int32U5BU5D_t19C97395396A72ECAF310612F0760F165060314C;
struct IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832;
struct ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918;
struct RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A;
struct StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF;
struct StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248;
struct UInt16U5BU5D_tEB7C42D811D999D2AA815BADC3FCCDD9C67B3F83;
struct AssetBundle_tB38418819A49060CD738CB21541649340F082943;
struct Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184;
struct CancellationTokenSource_tAAE1E0033BCFC233801F8CB4CED5C852B350CB7B;
struct CodePageDataItem_t52460FA30AE37F4F26ACB81055E58002262F19F2;
struct Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3;
struct CultureInfo_t9BA817D41AD55AC8BD07480DD8AC22F8FFA378E0;
struct DecoderFallback_t7324102215E4ED41EC065C02EB501CB0BC23CD90;
struct DelegateData_t9B286B493293CD2D23A5B2B5EF0E5B1324C2B77E;
struct EncoderFallback_tD2C40CE114AA9D8E1F7196608B2D088548015293;
struct Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095;
struct Exception_t;
struct ExecutionContext_t9D6EDFD92F0B2D391751963E2D77A8B03CB81710;
struct GameObject_t76FEDD663AB33C991A9C9A23129337651094216F;
struct IDictionary_t6D03155AF1FA9083817AA5B6AD7DEEACC26AB220;
struct IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5;
struct IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA;
struct IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484;
struct IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2;
struct IPEndPoint_t2F09CBA7A808B67724B4E2954EEDC46D910F4ECB;
struct IPrincipal_tE7AF5096287F6C3472585E124CB38FF2A51EAB5F;
struct InternalThread_tF40B7BFCBD60C82BD8475A22FF5186CA10293687;
struct LocalDataStoreHolder_t789DD474AE5141213C2105CE57830ECFC2D3C03F;
struct LocalDataStoreMgr_t205F1783D5CC2B148E829B5882E5406FF9A3AC1E;
struct MethodInfo_t;
struct MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71;
struct MulticastDelegate_t;
struct NetworkStream_tF39C3684B6D572BF47F518AD1DB1F4B12CEE4AE0;
struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C;
struct OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F;
struct ParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1;
struct Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF;
struct SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6;
struct SemaphoreSlim_t0D5CB5685D9BFA5BF95CEC6E7395490F933E8DB2;
struct SerializationInfo_t3C47F63E24BEB9FCE2DC6309E027F238DC5C5E37;
struct Socket_t1F49472CDA22B581C29A258225ABF3ADA9DED67E;
struct String_t;
struct StringBuilder_t;
struct TcpClient_t753B702EE06B59897564F75CEBFB6C8AFF10BD58;
struct TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82;
struct Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F;
struct ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2;
struct Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1;
struct UnitySourceGeneratedAssemblyMonoScriptTypes_v1_tC95F24D0C6E6B77389433852BB389F39C692926E;
struct ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628;
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915;
struct CameraCallback_t844E527BFE37BC0495E7F67993E43C07642DA9DD;
struct U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9;
struct ReadWriteTask_t0821BF49EE38596C7734E86E1A6A39D769BE2C05;

IL2CPP_EXTERN_C RuntimeClass* Application_tDB03BE91CDF0ACA614A5E0B67CFB77C44EB19B21_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Exception_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* GUI_tA9CDB3D69DB13D51AD83ABDB587EF95947EC2D2A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* GameObject_t76FEDD663AB33C991A9C9A23129337651094216F_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* RuntimeObject_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SceneManager_tA0EF56A88ACA4A15731AF7FDC10A869FA4C698FA_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Single_t4530F2FF86FCB0DC29F35385CA1BD21BE294761C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* StringBuilder_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeField* U3CPrivateImplementationDetailsU3E_t0F5473E849A5A5185A9F4C5246F0C32816C49FCA____82EF95C41CD7F156435E639A55748D8675CB4462876A899CF7A2DC6FF8925C26_FieldInfo_var;
IL2CPP_EXTERN_C RuntimeField* U3CPrivateImplementationDetailsU3E_t0F5473E849A5A5185A9F4C5246F0C32816C49FCA____EFEC78FCBA94D5281B7667D00F5FD326E4EE4C94B3639498A86DFEB7A0D540B5_FieldInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral10C85C8B45E1ABC7EA6F5FBD8A32715BEE966E5D;
IL2CPP_EXTERN_C String_t* _stringLiteral16DD21BE77B115D392226EB71A2D3A9FDC29E3F0;
IL2CPP_EXTERN_C String_t* _stringLiteral2F9062308C9D1A452D91CF6B41E9C71173E5387C;
IL2CPP_EXTERN_C String_t* _stringLiteral3B2C1C62D4D1C2A0C8A9AC42DB00D33C654F9AD0;
IL2CPP_EXTERN_C String_t* _stringLiteral3E989E783212FAC625EA2248403E9D8D78457010;
IL2CPP_EXTERN_C String_t* _stringLiteral4FEC49A2578715294749EE7FE4725044FEF11BC4;
IL2CPP_EXTERN_C String_t* _stringLiteral586BC205D60356D54AD7851D1CDAD06F92B0690D;
IL2CPP_EXTERN_C String_t* _stringLiteral662268AE378A5F5A4C0AA5809EF1C7B45437F544;
IL2CPP_EXTERN_C String_t* _stringLiteral73ED92A2DE5120A5716646E377BB552B3B4E51AF;
IL2CPP_EXTERN_C String_t* _stringLiteral8812C2A9B10FA45D1F9952B4E63A21B3398D5199;
IL2CPP_EXTERN_C String_t* _stringLiteral885FC71DFB0FF80A30E6013D905D1429AD60F467;
IL2CPP_EXTERN_C String_t* _stringLiteral88BEE283254D7094E258B3A88730F4CC4F1E4AC7;
IL2CPP_EXTERN_C String_t* _stringLiteral89D0BAC9D68CB85BDAC65115F45B67760D117A07;
IL2CPP_EXTERN_C String_t* _stringLiteral9229404B023B828641F8C179E7652573A8F786C0;
IL2CPP_EXTERN_C String_t* _stringLiteral9740BCC647899880C38FB0778880BB4E551AA428;
IL2CPP_EXTERN_C String_t* _stringLiteral996B036BC167299A08B2E9DB294082C30359A9C8;
IL2CPP_EXTERN_C String_t* _stringLiteralA1E3D75D531E0513532A591BB5814CCE7805A947;
IL2CPP_EXTERN_C String_t* _stringLiteralA24C6AB40AC1DE13B9BE17F5CBFB1B071711214B;
IL2CPP_EXTERN_C String_t* _stringLiteralA742FD2BD1D07C567492D76B8DEB157161F97B02;
IL2CPP_EXTERN_C String_t* _stringLiteralAC6D401179964D3F1472C368D58CF7AE3793FCC3;
IL2CPP_EXTERN_C String_t* _stringLiteralACE429A0AAEC7F24F9529A638C16C3AA0080E21F;
IL2CPP_EXTERN_C String_t* _stringLiteralAE6A994FF924EE125695D81F3A3A57FD5A4D9011;
IL2CPP_EXTERN_C String_t* _stringLiteralAF0F1A1ECDA829CF1C6DB15A2C2EEE6AA30E5C57;
IL2CPP_EXTERN_C String_t* _stringLiteralBB0462B9CEB66F4161BC2D92867CF94CF87B1B78;
IL2CPP_EXTERN_C String_t* _stringLiteralBB5DC6BB4FB40AFFAB95EEB08892720B278CC79A;
IL2CPP_EXTERN_C String_t* _stringLiteralD443B486F8A7F744E0D535B6A2816ACCCCABD83B;
IL2CPP_EXTERN_C String_t* _stringLiteralD9256C1C85B054E8069CFC11922DF972997469F0;
IL2CPP_EXTERN_C String_t* _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709;
IL2CPP_EXTERN_C String_t* _stringLiteralDA7E8B88100AA34B19982A11B0554BAF879AFF81;
IL2CPP_EXTERN_C String_t* _stringLiteralDBC773BB02113B1A7442300B1519C9B0D3783090;
IL2CPP_EXTERN_C String_t* _stringLiteralDED255C6188796AA0D317083FE9D01BC0BBC9276;
IL2CPP_EXTERN_C String_t* _stringLiteralE302AA9BECF9F1CB69CF2A3E5B33E0716BEA97F6;
IL2CPP_EXTERN_C String_t* _stringLiteralE5A618DB1CEDCF822A5199D3E96A878869B319FA;
IL2CPP_EXTERN_C String_t* _stringLiteralEB508B30BB5FB6A4B6F599EB52273776204BF55B;
IL2CPP_EXTERN_C String_t* _stringLiteralF881296E3AFAAF85C9495BBB45FF8BB231AB53B5;
IL2CPP_EXTERN_C String_t* _stringLiteralF91D1CBF70F2205BAFAA61A2A6140FA588BB55E7;
IL2CPP_EXTERN_C String_t* _stringLiteralFC6687DC37346CD2569888E29764F727FAF530E0;
IL2CPP_EXTERN_C const RuntimeMethod* AssetBundle_LoadAllAssets_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m61BD69AF333A9E321F6077CE3F20ED4A3AB26B58_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AssetBundle_LoadAsset_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m0216591F4BA27A483AA2C83EFDABCEA80B214063_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Component_GetComponent_TisParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1_mF2B700ECAEEEDBCC076CC66C9592C6DDFA0D36F9_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Component_GetComponentsInChildren_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_m35AC34F858BD2F34770712CD020AA0518D9409C7_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentQueue_1_Enqueue_mCFA14FE2B9D147552CD2F7F340F53A0284BB92B8_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentQueue_1_TryDequeue_m176C65C9DCE178799C348336E14CB35C748E9421_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentQueue_1__ctor_m89BBD6C7776D544A0D57C1322765E6D9BC513CB1_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_AddComponent_TisCamera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184_m0933BC50E883CDEF6FA83FE190DA37CCB2802142_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_AddComponent_TisIPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2_mADB27F1B8EE9E8A084D0EF3AA45B1A1793AEE7DE_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_AddComponent_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_m066777E29FC6E113E469A7A0F2AF7ADD38C42B46_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_GetComponentsInChildren_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mA2746088DB45856FD76C725AB403CEF5A8997734_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* HashSet_1_Add_m899003BF81A20D05B1D48080CB373E701BA64652_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* HashSet_1_Contains_m21126165891AF3C847A2737D98F1B9F853656B7E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* HashSet_1__ctor_mFC9E6E517FB114988F9789C7D16A02D65CB87F71_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* IPCServer_ListenForClients_m903C8DEE9A15C1CFCED40914F66C74E5DEFA2A76_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Object_FindObjectsOfType_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mC338358A9594B72942DC6AE839BC11C37E5CF8AD_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Object_Instantiate_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m10D87C6E0708CA912BBB02555BF7D0FBC5D7A2B3_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass9_0_U3CListenForClientsU3Eb__0_m5B6B69AD9B8AE8C69ECA544F5873515FA8708578_RuntimeMethod_var;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;

struct ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031;
struct CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB;
struct GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF;
struct ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918;
struct RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A;
struct StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
struct U3CModuleU3E_tBB65183F1134474D09FF49B95625D25472B9BA8B 
{
};
struct ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E  : public RuntimeObject
{
	RuntimeObject* ____crossSegmentLock;
	Segment_t3ABE51CD8494216053375DA492F57A0F6F8423BC* ____tail;
	Segment_t3ABE51CD8494216053375DA492F57A0F6F8423BC* ____head;
};
struct HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D  : public RuntimeObject
{
	Int32U5BU5D_t19C97395396A72ECAF310612F0760F165060314C* ____buckets;
	SlotU5BU5D_t15DCE773C3630ADE0EE26EE2939F4EB88FDE2D21* ____slots;
	int32_t ____count;
	int32_t ____lastIndex;
	int32_t ____freeList;
	RuntimeObject* ____comparer;
	int32_t ____version;
	SerializationInfo_t3C47F63E24BEB9FCE2DC6309E027F238DC5C5E37* ____siInfo;
};
struct U3CPrivateImplementationDetailsU3E_t0F5473E849A5A5185A9F4C5246F0C32816C49FCA  : public RuntimeObject
{
};
struct CriticalFinalizerObject_t1DCAB623CAEA6529A96F5F3EDE3C7048A6E313C9  : public RuntimeObject
{
};
struct Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095  : public RuntimeObject
{
	int32_t ___m_codePage;
	CodePageDataItem_t52460FA30AE37F4F26ACB81055E58002262F19F2* ___dataItem;
	bool ___m_deserializedFromEverett;
	bool ___m_isReadOnly;
	EncoderFallback_tD2C40CE114AA9D8E1F7196608B2D088548015293* ___encoderFallback;
	DecoderFallback_t7324102215E4ED41EC065C02EB501CB0BC23CD90* ___decoderFallback;
};
struct IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484  : public RuntimeObject
{
	uint32_t ____addressOrScopeId;
	UInt16U5BU5D_tEB7C42D811D999D2AA815BADC3FCCDD9C67B3F83* ____numbers;
	String_t* ____toString;
	int32_t ____hashCode;
};
struct MarshalByRefObject_t8C2F4C5854177FD60439EB1FCCFC1B3CFAFE8DCE  : public RuntimeObject
{
	RuntimeObject* ____identity;
};
struct MarshalByRefObject_t8C2F4C5854177FD60439EB1FCCFC1B3CFAFE8DCE_marshaled_pinvoke
{
	Il2CppIUnknown* ____identity;
};
struct MarshalByRefObject_t8C2F4C5854177FD60439EB1FCCFC1B3CFAFE8DCE_marshaled_com
{
	Il2CppIUnknown* ____identity;
};
struct String_t  : public RuntimeObject
{
	int32_t ____stringLength;
	Il2CppChar ____firstChar;
};
struct StringBuilder_t  : public RuntimeObject
{
	CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB* ___m_ChunkChars;
	StringBuilder_t* ___m_ChunkPrevious;
	int32_t ___m_ChunkLength;
	int32_t ___m_ChunkOffset;
	int32_t ___m_MaxCapacity;
};
struct TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82  : public RuntimeObject
{
	IPEndPoint_t2F09CBA7A808B67724B4E2954EEDC46D910F4ECB* ___m_ServerSocketEP;
	Socket_t1F49472CDA22B581C29A258225ABF3ADA9DED67E* ___m_ServerSocket;
	bool ___m_Active;
	bool ___m_ExclusiveAddressUse;
};
struct UnitySourceGeneratedAssemblyMonoScriptTypes_v1_tC95F24D0C6E6B77389433852BB389F39C692926E  : public RuntimeObject
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F  : public RuntimeObject
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_pinvoke
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_com
{
};
struct U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9  : public RuntimeObject
{
	TcpClient_t753B702EE06B59897564F75CEBFB6C8AFF10BD58* ___client;
	IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* ___U3CU3E4__this;
};
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22 
{
	bool ___m_value;
};
struct Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3 
{
	uint8_t ___m_value;
};
struct Char_t521A6F19B456D956AF452D926C32709DC03D6B17 
{
	Il2CppChar ___m_value;
};
struct Color_tD001788D726C3A7F1379BEED0260B9591F440C1F 
{
	float ___r;
	float ___g;
	float ___b;
	float ___a;
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2  : public ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F
{
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_marshaled_pinvoke
{
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_marshaled_com
{
};
struct Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C 
{
	int32_t ___m_value;
};
struct IntPtr_t 
{
	void* ___m_value;
};
struct Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 
{
	float ___x;
	float ___y;
	float ___z;
	float ___w;
};
struct Rect_tA04E0F8A1830E767F40FB27ECD8D309303571F0D 
{
	float ___m_XMin;
	float ___m_YMin;
	float ___m_Width;
	float ___m_Height;
};
struct RefreshRate_t44FF0D3F256477CC8AB97FA49D08B636AFB4F68F 
{
	uint32_t ___numerator;
	uint32_t ___denominator;
};
struct Scene_tA1DC762B79745EB5140F054C884855B922318356 
{
	int32_t ___m_Handle;
};
struct Single_t4530F2FF86FCB0DC29F35385CA1BD21BE294761C 
{
	float ___m_value;
};
struct Stream_tF844051B786E8F7F4244DBD218D74E8617B9A2DE  : public MarshalByRefObject_t8C2F4C5854177FD60439EB1FCCFC1B3CFAFE8DCE
{
	ReadWriteTask_t0821BF49EE38596C7734E86E1A6A39D769BE2C05* ____activeReadWriteTask;
	SemaphoreSlim_t0D5CB5685D9BFA5BF95CEC6E7395490F933E8DB2* ____asyncActiveSemaphore;
};
struct Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F  : public CriticalFinalizerObject_t1DCAB623CAEA6529A96F5F3EDE3C7048A6E313C9
{
	InternalThread_tF40B7BFCBD60C82BD8475A22FF5186CA10293687* ___internal_thread;
	RuntimeObject* ___m_ThreadStartArg;
	RuntimeObject* ___pending_exception;
	MulticastDelegate_t* ___m_Delegate;
	ExecutionContext_t9D6EDFD92F0B2D391751963E2D77A8B03CB81710* ___m_ExecutionContext;
	bool ___m_ExecutionContextBelongsToOuterScope;
	RuntimeObject* ___principal;
	int32_t ___principal_version;
};
struct Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 
{
	float ___x;
	float ___y;
	float ___z;
};
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915 
{
	union
	{
		struct
		{
		};
		uint8_t Void_t4861ACF8F4594C3437BB48B6E56783494B843915__padding[1];
	};
};
#pragma pack(push, tp, 1)
struct __StaticArrayInitTypeSizeU3D118_tE1F33A75C24ADA6CD71F9F33832AB43018595271 
{
	union
	{
		struct
		{
			union
			{
			};
		};
		uint8_t __StaticArrayInitTypeSizeU3D118_tE1F33A75C24ADA6CD71F9F33832AB43018595271__padding[118];
	};
};
#pragma pack(pop, tp)
#pragma pack(push, tp, 1)
struct __StaticArrayInitTypeSizeU3D55_t824C15EE0603DB8B7AAB6B3D3957B53A9ED287EA 
{
	union
	{
		struct
		{
			union
			{
			};
		};
		uint8_t __StaticArrayInitTypeSizeU3D55_t824C15EE0603DB8B7AAB6B3D3957B53A9ED287EA__padding[55];
	};
};
#pragma pack(pop, tp)
struct MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E 
{
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___FilePathsData;
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___TypesData;
	int32_t ___TotalTypes;
	int32_t ___TotalFiles;
	bool ___IsEditorOnly;
};
struct MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshaled_pinvoke
{
	Il2CppSafeArray* ___FilePathsData;
	Il2CppSafeArray* ___TypesData;
	int32_t ___TotalTypes;
	int32_t ___TotalFiles;
	int32_t ___IsEditorOnly;
};
struct MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshaled_com
{
	Il2CppSafeArray* ___FilePathsData;
	Il2CppSafeArray* ___TypesData;
	int32_t ___TotalTypes;
	int32_t ___TotalFiles;
	int32_t ___IsEditorOnly;
};
struct AddressFamily_t01AA8C9FD15E4727B241F1F453D88444337C7524 
{
	int32_t ___value__;
};
struct Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___m_Center;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___m_Extents;
};
struct Delegate_t  : public RuntimeObject
{
	intptr_t ___method_ptr;
	intptr_t ___invoke_impl;
	RuntimeObject* ___m_target;
	intptr_t ___method;
	intptr_t ___delegate_trampoline;
	intptr_t ___extra_arg;
	intptr_t ___method_code;
	intptr_t ___interp_method;
	intptr_t ___interp_invoke_impl;
	MethodInfo_t* ___method_info;
	MethodInfo_t* ___original_method_info;
	DelegateData_t9B286B493293CD2D23A5B2B5EF0E5B1324C2B77E* ___data;
	bool ___method_is_virtual;
};
struct Delegate_t_marshaled_pinvoke
{
	intptr_t ___method_ptr;
	intptr_t ___invoke_impl;
	Il2CppIUnknown* ___m_target;
	intptr_t ___method;
	intptr_t ___delegate_trampoline;
	intptr_t ___extra_arg;
	intptr_t ___method_code;
	intptr_t ___interp_method;
	intptr_t ___interp_invoke_impl;
	MethodInfo_t* ___method_info;
	MethodInfo_t* ___original_method_info;
	DelegateData_t9B286B493293CD2D23A5B2B5EF0E5B1324C2B77E* ___data;
	int32_t ___method_is_virtual;
};
struct Delegate_t_marshaled_com
{
	intptr_t ___method_ptr;
	intptr_t ___invoke_impl;
	Il2CppIUnknown* ___m_target;
	intptr_t ___method;
	intptr_t ___delegate_trampoline;
	intptr_t ___extra_arg;
	intptr_t ___method_code;
	intptr_t ___interp_method;
	intptr_t ___interp_invoke_impl;
	MethodInfo_t* ___method_info;
	MethodInfo_t* ___original_method_info;
	DelegateData_t9B286B493293CD2D23A5B2B5EF0E5B1324C2B77E* ___data;
	int32_t ___method_is_virtual;
};
struct Exception_t  : public RuntimeObject
{
	String_t* ____className;
	String_t* ____message;
	RuntimeObject* ____data;
	Exception_t* ____innerException;
	String_t* ____helpURL;
	RuntimeObject* ____stackTrace;
	String_t* ____stackTraceString;
	String_t* ____remoteStackTraceString;
	int32_t ____remoteStackIndex;
	RuntimeObject* ____dynamicMethods;
	int32_t ____HResult;
	String_t* ____source;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces;
	IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___native_trace_ips;
	int32_t ___caught_in_unmanaged;
};
struct Exception_t_marshaled_pinvoke
{
	char* ____className;
	char* ____message;
	RuntimeObject* ____data;
	Exception_t_marshaled_pinvoke* ____innerException;
	char* ____helpURL;
	Il2CppIUnknown* ____stackTrace;
	char* ____stackTraceString;
	char* ____remoteStackTraceString;
	int32_t ____remoteStackIndex;
	Il2CppIUnknown* ____dynamicMethods;
	int32_t ____HResult;
	char* ____source;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces;
	Il2CppSafeArray* ___native_trace_ips;
	int32_t ___caught_in_unmanaged;
};
struct Exception_t_marshaled_com
{
	Il2CppChar* ____className;
	Il2CppChar* ____message;
	RuntimeObject* ____data;
	Exception_t_marshaled_com* ____innerException;
	Il2CppChar* ____helpURL;
	Il2CppIUnknown* ____stackTrace;
	Il2CppChar* ____stackTraceString;
	Il2CppChar* ____remoteStackTraceString;
	int32_t ____remoteStackIndex;
	Il2CppIUnknown* ____dynamicMethods;
	int32_t ____HResult;
	Il2CppChar* ____source;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces;
	Il2CppSafeArray* ___native_trace_ips;
	int32_t ___caught_in_unmanaged;
};
struct KeyCode_t75B9ECCC26D858F55040DDFF9523681E996D17E9 
{
	int32_t ___value__;
};
struct NetworkStream_tF39C3684B6D572BF47F518AD1DB1F4B12CEE4AE0  : public Stream_tF844051B786E8F7F4244DBD218D74E8617B9A2DE
{
	Socket_t1F49472CDA22B581C29A258225ABF3ADA9DED67E* ____streamSocket;
	bool ____ownsSocket;
	bool ____readable;
	bool ____writeable;
	int32_t ____closeTimeout;
	bool ____cleanedUp;
	int32_t ____currentReadTimeout;
	int32_t ____currentWriteTimeout;
};
struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C  : public RuntimeObject
{
	intptr_t ___m_CachedPtr;
};
struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_marshaled_pinvoke
{
	intptr_t ___m_CachedPtr;
};
struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_marshaled_com
{
	intptr_t ___m_CachedPtr;
};
struct Resolution_tDF215F567EEFFD07B9A8FB7CEACC08EA6B8B9525 
{
	int32_t ___m_Width;
	int32_t ___m_Height;
	RefreshRate_t44FF0D3F256477CC8AB97FA49D08B636AFB4F68F ___m_RefreshRate;
};
struct RuntimeFieldHandle_t6E4C45B6D2EA12FC99185805A7E77527899B25C5 
{
	intptr_t ___value;
};
struct Space_tF043E93E06B702DD05199C28C6F779049B38A969 
{
	int32_t ___value__;
};
struct StringComparison_tE14A55CCFA001A5AC85D754179BF2888F45CC94D 
{
	int32_t ___value__;
};
struct StringSplitOptions_t4DD892C76C70DD4800FC1B76054D69826F770062 
{
	int32_t ___value__;
};
struct AssetBundle_tB38418819A49060CD738CB21541649340F082943  : public Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C
{
};
struct Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3  : public Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C
{
};
struct GameObject_t76FEDD663AB33C991A9C9A23129337651094216F  : public Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C
{
};
struct MulticastDelegate_t  : public Delegate_t
{
	DelegateU5BU5D_tC5AB7E8F745616680F337909D3A8E6C722CDF771* ___delegates;
};
struct MulticastDelegate_t_marshaled_pinvoke : public Delegate_t_marshaled_pinvoke
{
	Delegate_t_marshaled_pinvoke** ___delegates;
};
struct MulticastDelegate_t_marshaled_com : public Delegate_t_marshaled_com
{
	Delegate_t_marshaled_com** ___delegates;
};
struct TcpClient_t753B702EE06B59897564F75CEBFB6C8AFF10BD58  : public RuntimeObject
{
	Socket_t1F49472CDA22B581C29A258225ABF3ADA9DED67E* ___m_ClientSocket;
	bool ___m_Active;
	NetworkStream_tF39C3684B6D572BF47F518AD1DB1F4B12CEE4AE0* ___m_DataStream;
	int32_t ___m_Family;
	bool ___m_CleanedUp;
};
struct Behaviour_t01970CFBBA658497AE30F311C447DB0440BAB7FA  : public Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3
{
};
struct ParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1  : public Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3
{
};
struct Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF  : public Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3
{
};
struct ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2  : public MulticastDelegate_t
{
};
struct Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1  : public Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3
{
};
struct Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184  : public Behaviour_t01970CFBBA658497AE30F311C447DB0440BAB7FA
{
};
struct MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71  : public Behaviour_t01970CFBBA658497AE30F311C447DB0440BAB7FA
{
	CancellationTokenSource_tAAE1E0033BCFC233801F8CB4CED5C852B350CB7B* ___m_CancellationTokenSource;
};
struct IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2  : public MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71
{
	TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82* ____server;
	Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F* ____serverThread;
	ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E* ____commandQueue;
	bool ____isRunning;
	int32_t ___Port;
};
struct OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F  : public MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71
{
	float ___moveSpeed;
	float ___turboSpeed;
	float ___lookSpeed;
	float ___panSpeed;
	bool ___invertY;
	float ____rotationX;
	float ____rotationY;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ____targetPosition;
	float ____forcedAspect;
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ____highlightedObject;
	Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 ____lastBounds;
};
struct ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628  : public MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71
{
};
struct U3CPrivateImplementationDetailsU3E_t0F5473E849A5A5185A9F4C5246F0C32816C49FCA_StaticFields
{
	__StaticArrayInitTypeSizeU3D118_tE1F33A75C24ADA6CD71F9F33832AB43018595271 ___82EF95C41CD7F156435E639A55748D8675CB4462876A899CF7A2DC6FF8925C26;
	__StaticArrayInitTypeSizeU3D55_t824C15EE0603DB8B7AAB6B3D3957B53A9ED287EA ___EFEC78FCBA94D5281B7667D00F5FD326E4EE4C94B3639498A86DFEB7A0D540B5;
};
struct Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095_StaticFields
{
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___defaultEncoding;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___unicodeEncoding;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___bigEndianUnicode;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___utf7Encoding;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___utf8Encoding;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___utf32Encoding;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___asciiEncoding;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___latin1Encoding;
	Dictionary_2_t87EDE08B2E48F793A22DE50D6B3CC2E7EBB2DB54* ___encodings;
	RuntimeObject* ___s_InternalSyncObject;
};
struct IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484_StaticFields
{
	IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484* ___Any;
	IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484* ___Loopback;
	IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484* ___Broadcast;
	IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484* ___None;
	IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484* ___IPv6Any;
	IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484* ___IPv6Loopback;
	IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484* ___IPv6None;
};
struct String_t_StaticFields
{
	String_t* ___Empty;
};
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22_StaticFields
{
	String_t* ___TrueString;
	String_t* ___FalseString;
};
struct Char_t521A6F19B456D956AF452D926C32709DC03D6B17_StaticFields
{
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___s_categoryForLatin1;
};
struct IntPtr_t_StaticFields
{
	intptr_t ___Zero;
};
struct Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974_StaticFields
{
	Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 ___identityQuaternion;
};
struct Stream_tF844051B786E8F7F4244DBD218D74E8617B9A2DE_StaticFields
{
	Stream_tF844051B786E8F7F4244DBD218D74E8617B9A2DE* ___Null;
};
struct Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F_StaticFields
{
	LocalDataStoreMgr_t205F1783D5CC2B148E829B5882E5406FF9A3AC1E* ___s_LocalDataStoreMgr;
	AsyncLocal_1_t1D3339EA4C8650D2DEDDF9553E5C932B3DC2CCFD* ___s_asyncLocalCurrentCulture;
	AsyncLocal_1_t1D3339EA4C8650D2DEDDF9553E5C932B3DC2CCFD* ___s_asyncLocalCurrentUICulture;
};
struct Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F_ThreadStaticFields
{
	LocalDataStoreHolder_t789DD474AE5141213C2105CE57830ECFC2D3C03F* ___s_LocalDataStore;
	CultureInfo_t9BA817D41AD55AC8BD07480DD8AC22F8FFA378E0* ___m_CurrentCulture;
	CultureInfo_t9BA817D41AD55AC8BD07480DD8AC22F8FFA378E0* ___m_CurrentUICulture;
	Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F* ___current_thread;
};
struct Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_StaticFields
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___zeroVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___oneVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___upVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___downVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___leftVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___rightVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___forwardVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___backVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___positiveInfinityVector;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___negativeInfinityVector;
};
struct Exception_t_StaticFields
{
	RuntimeObject* ___s_EDILock;
};
struct Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_StaticFields
{
	int32_t ___OffsetOfInstanceIDInCPlusPlusObject;
};
struct Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184_StaticFields
{
	CameraCallback_t844E527BFE37BC0495E7F67993E43C07642DA9DD* ___onPreCull;
	CameraCallback_t844E527BFE37BC0495E7F67993E43C07642DA9DD* ___onPreRender;
	CameraCallback_t844E527BFE37BC0495E7F67993E43C07642DA9DD* ___onPostRender;
};
struct ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields
{
	String_t* ___TargetFile;
	String_t* ___LoadStatus;
};
#ifdef __clang__
#pragma clang diagnostic pop
#endif
struct ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031  : public RuntimeArray
{
	ALIGN_FIELD (8) uint8_t m_Items[1];

	inline uint8_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline uint8_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, uint8_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline uint8_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline uint8_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, uint8_t value)
	{
		m_Items[index] = value;
	}
};
struct StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248  : public RuntimeArray
{
	ALIGN_FIELD (8) String_t* m_Items[1];

	inline String_t* GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline String_t** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, String_t* value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline String_t* GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline String_t** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, String_t* value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
struct CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB  : public RuntimeArray
{
	ALIGN_FIELD (8) Il2CppChar m_Items[1];

	inline Il2CppChar GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Il2CppChar* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Il2CppChar value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline Il2CppChar GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Il2CppChar* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Il2CppChar value)
	{
		m_Items[index] = value;
	}
};
struct GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF  : public RuntimeArray
{
	ALIGN_FIELD (8) GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* m_Items[1];

	inline GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline GameObject_t76FEDD663AB33C991A9C9A23129337651094216F** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline GameObject_t76FEDD663AB33C991A9C9A23129337651094216F** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
struct RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A  : public RuntimeArray
{
	ALIGN_FIELD (8) Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* m_Items[1];

	inline Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
struct ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918  : public RuntimeArray
{
	ALIGN_FIELD (8) RuntimeObject* m_Items[1];

	inline RuntimeObject* GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline RuntimeObject** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, RuntimeObject* value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline RuntimeObject* GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline RuntimeObject** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, RuntimeObject* value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};


IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ConcurrentQueue_1_Enqueue_m6E14BFDDF046BB59B8EEF4A398D5AB7F4720D0A4_gshared (ConcurrentQueue_1_t38466BAF2E0933882FAC022407D638F7900CEED4* __this, RuntimeObject* ___0_item, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ConcurrentQueue_1_TryDequeue_m04C6B697D83A4AA4BAD38344201C140ED7DC5CAA_gshared (ConcurrentQueue_1_t38466BAF2E0933882FAC022407D638F7900CEED4* __this, RuntimeObject** ___0_result, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Object_FindObjectOfType_TisRuntimeObject_m02DFBF011F3B59F777A5E521DB2A116DD496E968_gshared (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ConcurrentQueue_1__ctor_m45A2A2E43BA42D4368773A687E7BB6CBCD510345_gshared (ConcurrentQueue_1_t38466BAF2E0933882FAC022407D638F7900CEED4* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918* Object_FindObjectsOfType_TisRuntimeObject_m0B4DF4B8AB4C71E0F471BC9D0440B40844DA221D_gshared (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HashSet_1__ctor_m9132EE1422BAA45E44B7FFF495F378790D36D90E_gshared (HashSet_1_t2F33BEB06EEA4A872E2FAF464382422AA39AE885* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918* GameObject_GetComponentsInChildren_TisRuntimeObject_m6F69570C0224EE6620FD43C4DDB0F0AB152A1B20_gshared (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool HashSet_1_Add_m2CD7657B3459B61DD4BBA47024AC71F7D319658B_gshared (HashSet_1_t2F33BEB06EEA4A872E2FAF464382422AA39AE885* __this, RuntimeObject* ___0_item, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Component_GetComponent_TisRuntimeObject_m7181F81CAEC2CF53F5D2BC79B7425C16E1F80D33_gshared (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool HashSet_1_Contains_m9BACE52BFA0BD83C601529D3629118453E459BBB_gshared (HashSet_1_t2F33BEB06EEA4A872E2FAF464382422AA39AE885* __this, RuntimeObject* ___0_item, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918* Component_GetComponentsInChildren_TisRuntimeObject_m1F5B6FC0689B07D4FAAC0C605D9B2933A9B32543_gshared (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* GameObject_AddComponent_TisRuntimeObject_m69B93700FACCF372F5753371C6E8FB780800B824_gshared (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918* AssetBundle_LoadAllAssets_TisRuntimeObject_m11AD3CC703881F7388E89C5F5930BB9409753570_gshared (AssetBundle_tB38418819A49060CD738CB21541649340F082943* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Object_Instantiate_TisRuntimeObject_m90A1E6C4C2B445D2E848DB75C772D1B95AAC046A_gshared (RuntimeObject* ___0_original, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* AssetBundle_LoadAsset_TisRuntimeObject_m7F36E1C08FC32ADB47C34513522D958FA6E09C47_gshared (AssetBundle_tB38418819A49060CD738CB21541649340F082943* __this, String_t* ___0_name, const RuntimeMethod* method) ;

IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_StartServer_mEEED2CC485A6F8F480485D1450EEA2DCACE51721 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_StopServer_mAF9E0D6CF67851666CDCF92728BB246AA66649C6 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ThreadStart__ctor_m35CE51F76358F20E4A0C8F2963C3AE14E5FF7E71 (ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Thread__ctor_m1FA23A179DF4F5B659689D6FA40DE903086212E2 (Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F* __this, ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2* ___0_start, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Thread_set_IsBackground_m45F00BD4C46F9B8A7C46A20A170B22BABB8FBA30 (Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F* __this, bool ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_NO_INLINE IL2CPP_METHOD_ATTR void Thread_Start_m2230799AA331CA9ACEF49D5500281D0C1A9F72FA (Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Format_mA8DBB4C2516B9723C5A41E6CB1E2FAF4BBE96DD8 (String_t* ___0_format, RuntimeObject* ___1_arg0, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_Log_m87A9A3C761FF5C43ED8A53B16190A53D08F818BB (RuntimeObject* ___0_message, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TcpListener_Stop_mBF4B354EB52138AC9A0184F186894EDBAE3BA5FD (TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TcpListener__ctor_m6EDEF45E8F8F2872F3828E801806D9FEC3FF003B (TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82* __this, IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484* ___0_localaddr, int32_t ___1_port, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TcpListener_Start_m919D559B138B311CFFBBE4BF66E326EABD8F8712 (TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass9_0__ctor_m1F9EC985F755B288612CD7C04F5667362415DDC8 (U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TcpListener_Pending_m30EF809BFB0838CDFEAFA5A5065FB7C13E41DA11 (TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Thread_Sleep_m71DE163765BF465EC4A0163F2ED4D43143094549 (int32_t ___0_millisecondsTimeout, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TcpClient_t753B702EE06B59897564F75CEBFB6C8AFF10BD58* TcpListener_AcceptTcpClient_mD7DFF1228EAB3F886B5BBC6175A0856C84F2B419 (TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_m9E3155FB84015C823606188F53B47CB44C444991 (String_t* ___0_str0, String_t* ___1_str1, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2 (RuntimeObject* ___0_message, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TcpClient_Close_m03E0ED4E4BA87B3F1ED17585AB1327ED76F5FE89 (TcpClient_t753B702EE06B59897564F75CEBFB6C8AFF10BD58* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR NetworkStream_tF39C3684B6D572BF47F518AD1DB1F4B12CEE4AE0* TcpClient_GetStream_mDD54336B17D1267BD593E0A1EB9EDF3E9506AEBA (TcpClient_t753B702EE06B59897564F75CEBFB6C8AFF10BD58* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StringBuilder__ctor_m1D99713357DE05DAFA296633639DB55F8C30587D (StringBuilder_t* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* Encoding_get_UTF8_m9FA98A53CE96FD6D02982625C5246DD36C1235C9 (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StringBuilder_t* StringBuilder_Append_m08904D74E0C78E5F36DCD9C9303BDD07886D9F7D (StringBuilder_t* __this, String_t* ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t StringBuilder_get_Length_mDEA041E7357C68CC3B5885276BB403676DAAE0D8 (StringBuilder_t* __this, const RuntimeMethod* method) ;
inline void ConcurrentQueue_1_Enqueue_mCFA14FE2B9D147552CD2F7F340F53A0284BB92B8 (ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E* __this, String_t* ___0_item, const RuntimeMethod* method)
{
	((  void (*) (ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E*, String_t*, const RuntimeMethod*))ConcurrentQueue_1_Enqueue_m6E14BFDDF046BB59B8EEF4A398D5AB7F4720D0A4_gshared)(__this, ___0_item, method);
}
inline bool ConcurrentQueue_1_TryDequeue_m176C65C9DCE178799C348336E14CB35C748E9421 (ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E* __this, String_t** ___0_result, const RuntimeMethod* method)
{
	return ((  bool (*) (ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E*, String_t**, const RuntimeMethod*))ConcurrentQueue_1_TryDequeue_m04C6B697D83A4AA4BAD38344201C140ED7DC5CAA_gshared)(__this, ___0_result, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_ProcessCommand_m6588DEA3CD42C3EE99E6CF76F1B16758BCDFEDAF (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, String_t* ___0_command, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Trim_mCD6D8C6D4CFD15225D12DB7D3E0544CA80FB8DA5 (String_t* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool String_StartsWith_mF75DBA1EB709811E711B44E26FF919C88A8E65C0 (String_t* __this, String_t* ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Substring_m6BA4A3FA3800FE92662D0847CC8E1EEF940DF472 (String_t* __this, int32_t ___0_startIndex, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_SelectObject_m1D2832DC5B52158338859DF584D3364C4F9AB1E5 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, String_t* ___0_name, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* String_Split_m9530B73D02054692283BF35C3A27C8F2230946F4 (String_t* __this, Il2CppChar ___0_separator, int32_t ___1_options, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Single_TryParse_mC39FC2D8F1F33DE64685F3A8CE6BDD2164FB9423 (String_t* ___0_s, float* ___1_result, const RuntimeMethod* method) ;
inline OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E (const RuntimeMethod* method)
{
	return ((  OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* (*) (const RuntimeMethod*))Object_FindObjectOfType_TisRuntimeObject_m02DFBF011F3B59F777A5E521DB2A116DD496E968_gshared)(method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602 (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C* ___0_x, Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C* ___1_y, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_SetForcedResolution_mB14EB0E2178E7384E0AAB5E50D7AD2B22416982D (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, float ___0_width, float ___1_height, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* String_Split_m6811F7CF4DE7C68019D76E9DC6D17B2293BA8B12 (String_t* __this, CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB* ___0_separator, int32_t ___1_options, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_ShowMultipleObjects_m11BE9D20EA8BD547EA9D2F50AB350E48A26A3775 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___0_names, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool String_op_Equality_m030E1B219352228970A076136E455C4E568C02C1 (String_t* ___0_a, String_t* ___1_b, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* IPCServer_FindGameObjectByName_mFC62A5F91177B967D029E7309E7C888C09499BC4 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, String_t* ___0_name, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_SetTargetHighlight_m1065C1EDBCA87BC32062E9A592F04080954855E9 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___0_obj, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_LogWarning_m33EF1B897E0C7C6FF538989610BFAFFEF4628CA9 (RuntimeObject* ___0_message, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* GameObject_Find_m7A669B4EEC2617AB82F6E3FF007CDCD9F21DB300 (String_t* ___0_name, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Scene_tA1DC762B79745EB5140F054C884855B922318356 SceneManager_GetActiveScene_m0B320EC4302F51A71495D1CCD1A0FF9C2ED1FDC8 (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* Scene_GetRootGameObjects_mFDE0BF9EA926F30EC9AE71F33E0AE4D5D4EC5C5B (Scene_tA1DC762B79745EB5140F054C884855B922318356* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* GameObject_get_transform_m0BC10ADFA1632166AE5544BDF9038A2650C2AE56 (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* IPCServer_FindRecursive_m67FBDCAA54FE211A941B39A8859B8136795B6DB2 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* ___0_parent, String_t* ___1_name, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* Component_get_gameObject_m57AEFBB14DB39EC476F740BA000E170355DE691B (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Object_get_name_mAC2F6B897CF1303BA4249B4CB55271AFACBB6392 (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Transform_GetEnumerator_mA7E1C882ACA0C33E284711CD09971DEA3FFEF404 (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* __this, const RuntimeMethod* method) ;
inline void ConcurrentQueue_1__ctor_m89BBD6C7776D544A0D57C1322765E6D9BC513CB1 (ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E* __this, const RuntimeMethod* method)
{
	((  void (*) (ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E*, const RuntimeMethod*))ConcurrentQueue_1__ctor_m45A2A2E43BA42D4368773A687E7BB6CBCD510345_gshared)(__this, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MonoBehaviour__ctor_m592DB0105CA0BC97AA1C5F4AD27B12D68A3B7C1E (MonoBehaviour_t532A11E69716D348D8AA7F854AFCBFCB8AD17F71* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2 (RuntimeObject* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_HandleClient_m581A8782BD4EFF003E93063317145E12678EF63D (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, TcpClient_t753B702EE06B59897564F75CEBFB6C8AFF10BD58* ___0_client, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371 (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Transform_get_eulerAngles_mCAAF48EFCF628F1ED91C2FFE75A4FD19C039DD6A (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* Camera_get_main_m52C992F18E05355ABB9EEB64A4BF2215E12762DF (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Screen_get_width_mF608FF3252213E7EFA1F0D2F744C28110E9E5AC9 (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Screen_get_height_m01A3102DE71EE1FBEA51D09D6B0261CF864FE8F9 (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Camera_set_aspect_m537F21B48FDD5C060DFF9D87F34F4FB2B0F9BEB6 (Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* __this, float ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Input_GetMouseButton_m4995DD4A2D4F916565C1B1B5AAF7DF17C126B3EA (int32_t ___0_button, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Input_GetAxis_m10372E6C5FF591668D2DC5F58C58D213CC598A62 (String_t* ___0_axisName, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR float Mathf_Clamp_mEB9AEA827D27D20FCC787F7375156AF46BB12BBF_inline (float ___0_value, float ___1_min, float ___2_max, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 Quaternion_Euler_m9262AB29E3E9CE94EF71051F38A28E82AEC73F90_inline (float ___0_x, float ___1_y, float ___2_z, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Transform_set_rotation_m61340DE74726CF0F9946743A727C4D444397331D (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* __this, Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Time_get_deltaTime_mC3195000401F0FD167DD2F948FD2BC58330D0865 (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Transform_Translate_m8FF175E92671A19466A7728FEB92EFB4E999EB56 (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* __this, float ___0_x, float ___1_y, float ___2_z, int32_t ___3_relativeTo, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Input_GetKey_mE5681EF775F3CEBA7EAD7C63984F7B34C8E8D434 (int32_t ___0_key, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_get_zero_m0C1249C3F25B1C70EAD3CC8B31259975A457AE39_inline (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Transform_get_forward_mFCFACF7165FDAB21E80E384C494DF278386CEE2F (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_op_Addition_m78C0EC70CB66E8DCAC225743D82B268DAEE92067_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_a, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___1_b, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_op_Subtraction_mE42023FF80067CB44A1D4A27EB7CF2B24CABB828_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_a, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___1_b, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Transform_get_right_mC6DC057C23313802E2186A9E0DB760D795A758A4 (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_get_up_m128AF3FDC820BF59D5DE86D973E7DE3F20C3AEBA_inline (const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_get_down_mF62B2AE7C5AC31EAC9CB62797C7190C90A7A8599_inline (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Transform_get_position_m69CD5FA214FDAE7BB701552943674846C220FDE1 (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_op_Multiply_m87BA7C578F96C8E49BB07088DAAC4649F83B0353_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_a, float ___1_d, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Transform_set_position_mA1A817124BB41B685043DED2A9BA48CDF37C4156 (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Input_GetKeyDown_mB237DEA6244132670D38990BAB77D813FBB028D2 (int32_t ___0_key, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_FrameAll_m2447ACDD9F3F04FC9B3EE6A55987AB693DC96CB6 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605 (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C* ___0_x, Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C* ___1_y, const RuntimeMethod* method) ;
inline RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* Object_FindObjectsOfType_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mC338358A9594B72942DC6AE839BC11C37E5CF8AD (const RuntimeMethod* method)
{
	return ((  RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* (*) (const RuntimeMethod*))Object_FindObjectsOfType_TisRuntimeObject_m0B4DF4B8AB4C71E0F471BC9D0440B40844DA221D_gshared)(method);
}
inline void HashSet_1__ctor_mFC9E6E517FB114988F9789C7D16A02D65CB87F71 (HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D* __this, const RuntimeMethod* method)
{
	((  void (*) (HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D*, const RuntimeMethod*))HashSet_1__ctor_m9132EE1422BAA45E44B7FFF495F378790D36D90E_gshared)(__this, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* OrbitalCamera_FindGameObjectByName_m2A404C82E24BC33BA073574D123D743F4534CC50 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, String_t* ___0_name, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* Transform_get_parent_m65354E28A4C94EC00EBCF03532F7B0718380791E (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* __this, const RuntimeMethod* method) ;
inline RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* GameObject_GetComponentsInChildren_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mA2746088DB45856FD76C725AB403CEF5A8997734 (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method)
{
	return ((  RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* (*) (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*, const RuntimeMethod*))GameObject_GetComponentsInChildren_TisRuntimeObject_m6F69570C0224EE6620FD43C4DDB0F0AB152A1B20_gshared)(__this, method);
}
inline bool HashSet_1_Add_m899003BF81A20D05B1D48080CB373E701BA64652 (HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D* __this, Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* ___0_item, const RuntimeMethod* method)
{
	return ((  bool (*) (HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D*, Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF*, const RuntimeMethod*))HashSet_1_Add_m2CD7657B3459B61DD4BBA47024AC71F7D319658B_gshared)(__this, ___0_item, method);
}
inline ParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1* Component_GetComponent_TisParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1_mF2B700ECAEEEDBCC076CC66C9592C6DDFA0D36F9 (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method)
{
	return ((  ParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1* (*) (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3*, const RuntimeMethod*))Component_GetComponent_TisRuntimeObject_m7181F81CAEC2CF53F5D2BC79B7425C16E1F80D33_gshared)(__this, method);
}
inline bool HashSet_1_Contains_m21126165891AF3C847A2737D98F1B9F853656B7E (HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D* __this, Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* ___0_item, const RuntimeMethod* method)
{
	return ((  bool (*) (HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D*, Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF*, const RuntimeMethod*))HashSet_1_Contains_m9BACE52BFA0BD83C601529D3629118453E459BBB_gshared)(__this, ___0_item, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Renderer_set_enabled_m015E6D7B825528A31182F267234CC6A925F71DA8 (Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* __this, bool ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* OrbitalCamera_FindRecursive_m60A3567100CA4F64F45BC40735CC4A013F16DC6E (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* ___0_parent, String_t* ___1_name, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Color_tD001788D726C3A7F1379BEED0260B9591F440C1F Color_get_green_mEB001F2CD8C68C6BBAEF9101990B779D3AA2A6EF_inline (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GUI_set_color_mA44927D3F2ACB1E228815F10042A6F62B7645648 (Color_tD001788D726C3A7F1379BEED0260B9591F440C1F ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Format_mA0534D6E2AE4D67A6BD8D45B3321323930EB930C (String_t* ___0_format, RuntimeObject* ___1_arg0, RuntimeObject* ___2_arg1, RuntimeObject* ___3_arg2, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Bounds_get_center_m5B05F81CB835EB6DD8628FDA24B638F477984DC3_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Bounds_get_size_m0699A53A55A78B3201D7270D6F338DFA91B6FAD4_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Format_mFB7DA489BD99F4670881FF50EC017BFB0A5C0987 (String_t* ___0_format, RuntimeObject* ___1_arg0, RuntimeObject* ___2_arg1, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Resolution_tDF215F567EEFFD07B9A8FB7CEACC08EA6B8B9525 Screen_get_currentResolution_m8FE4C43E4F6EF28E0B85EB94B6C69D1EC5687CCD (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Resolution_get_width_mC55423852D453EEC0B66E692AA3E7F5DE772FB24 (Resolution_tDF215F567EEFFD07B9A8FB7CEACC08EA6B8B9525* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Resolution_get_height_mD46754FA3634CA816BE91D1610D1FEBDECCEBE5B (Resolution_tDF215F567EEFFD07B9A8FB7CEACC08EA6B8B9525* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Format_m918500C1EFB475181349A79989BB79BB36102894 (String_t* ___0_format, ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918* ___1_args, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Camera_get_aspect_m48BF8820EA2D55BE0D154BC5546819FB65BE257D (Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_m093934F71A9B351911EE46311674ED463B180006 (String_t* ___0_str0, String_t* ___1_str1, String_t* ___2_str2, String_t* ___3_str3, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Rect__ctor_m18C3033D135097BEE424AAA68D91C706D2647F23_inline (Rect_tA04E0F8A1830E767F40FB27ECD8D309303571F0D* __this, float ___0_x, float ___1_y, float ___2_width, float ___3_height, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GUI_Label_m4A951E57C7DCCF95A0306240144CA2713F546526 (Rect_tA04E0F8A1830E767F40FB27ECD8D309303571F0D ___0_position, String_t* ___1_text, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Color_tD001788D726C3A7F1379BEED0260B9591F440C1F Color_get_cyan_m182A29E7475C0A98ACC03E1CF5252BAB83F0BA31_inline (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 OrbitalCamera_CalculateBounds_m196814D27B1ABA374964A41F4C4FF946CA2E3E56 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* ___0_root, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_FrameBounds_m897E753DA62BA59A9E0DA6D81C912FE285F58930 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 ___0_b, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds__ctor_mAF7B238B9FBF90C495E5D7951760085A93119C5A_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_center, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___1_size, const RuntimeMethod* method) ;
inline RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* Component_GetComponentsInChildren_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_m35AC34F858BD2F34770712CD020AA0518D9409C7 (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3* __this, const RuntimeMethod* method)
{
	return ((  RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* (*) (Component_t39FBE53E5EFCF4409111FB22C15FF73717632EC3*, const RuntimeMethod*))Component_GetComponentsInChildren_TisRuntimeObject_m1F5B6FC0689B07D4FAAC0C605D9B2933A9B32543_gshared)(__this, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 Renderer_get_bounds_m390CF334730C3C34E45CE59F1D08C3B9F3109C7C (Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds_Encapsulate_m7C70C382B9380A8C962074C78E189B53CE8F7A22_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 ___0_bounds, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds_set_center_m891869DD5B1BEEE2D17907BBFB7EB79AAE44884B_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_value, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Vector3__ctor_m376936E6B999EF1ECBE57D990A386303E2283DE0_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2* __this, float ___0_x, float ___1_y, float ___2_z, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds_set_size_m950CFB68CDD1BF409E770509A38B958E1AE68128_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_value, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR float Mathf_Max_mF5379E63D2BBAC76D090748695D833934F8AD051_inline (float ___0_a, float ___1_b, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Camera_get_fieldOfView_m9A93F17BBF89F496AE231C21817AFD1C1E833FBB (Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* Environment_GetCommandLineArgs_mD29CFA1CD3C84F9BD91152E70302E908114A831D (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Trim_m81BD35659E6F89DDD56816975E6E05390D023FE5 (String_t* __this, CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB* ___0_trimChars, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool String_EndsWith_m5E5D307CA6AEB7C08CE782B4693B19D07ADC9075 (String_t* __this, String_t* ___0_value, int32_t ___1_comparisonType, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool File_Exists_m95E329ABBE3EAD6750FE1989BBA6884457136D4A (String_t* ___0_path, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Application_set_runInBackground_m4CD0DF59011994C4DB76D7A2FE7A02BC07B4AF67 (bool ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool String_IsNullOrEmpty_mEA9E3FB005AC28FE02E69FCF95A7B8456192B478 (String_t* ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ViewerBootstrap_LoadModel_mD2EA0E216CFAE1D70E5334ACC1B48895BACE1761 (ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628* __this, String_t* ___0_path, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameObject__ctor_m37D512B05D292F954792225E6C6EEE95293A9B88 (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, String_t* ___0_name, const RuntimeMethod* method) ;
inline Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* GameObject_AddComponent_TisCamera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184_m0933BC50E883CDEF6FA83FE190DA37CCB2802142 (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method)
{
	return ((  Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* (*) (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*, const RuntimeMethod*))GameObject_AddComponent_TisRuntimeObject_m69B93700FACCF372F5753371C6E8FB780800B824_gshared)(__this, method);
}
inline OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* GameObject_AddComponent_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_m066777E29FC6E113E469A7A0F2AF7ADD38C42B46 (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method)
{
	return ((  OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* (*) (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*, const RuntimeMethod*))GameObject_AddComponent_TisRuntimeObject_m69B93700FACCF372F5753371C6E8FB780800B824_gshared)(__this, method);
}
inline IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* GameObject_AddComponent_TisIPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2_mADB27F1B8EE9E8A084D0EF3AA45B1A1793AEE7DE (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, const RuntimeMethod* method)
{
	return ((  IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* (*) (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*, const RuntimeMethod*))GameObject_AddComponent_TisRuntimeObject_m69B93700FACCF372F5753371C6E8FB780800B824_gshared)(__this, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GameObject_set_tag_m0A41528AFD8C83E1CEC5D769921159897CDD2B24 (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* __this, String_t* ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AssetBundle_tB38418819A49060CD738CB21541649340F082943* AssetBundle_LoadFromFile_mAFD7B0E8F0A0A88E9BB60180EBA0A99B8B3F2629 (String_t* ___0_path, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* AssetBundle_GetAllAssetNames_m44504DB9E055412F0DF2071A769A243219708CC3 (AssetBundle_tB38418819A49060CD738CB21541649340F082943* __this, const RuntimeMethod* method) ;
inline GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* AssetBundle_LoadAllAssets_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m61BD69AF333A9E321F6077CE3F20ED4A3AB26B58 (AssetBundle_tB38418819A49060CD738CB21541649340F082943* __this, const RuntimeMethod* method)
{
	return ((  GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* (*) (AssetBundle_tB38418819A49060CD738CB21541649340F082943*, const RuntimeMethod*))AssetBundle_LoadAllAssets_TisRuntimeObject_m11AD3CC703881F7388E89C5F5930BB9409753570_gshared)(__this, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Transform_get_childCount_mE9C29C702AB662CC540CA053EDE48BDAFA35B4B0 (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* __this, const RuntimeMethod* method) ;
inline GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* Object_Instantiate_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m10D87C6E0708CA912BBB02555BF7D0FBC5D7A2B3 (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___0_original, const RuntimeMethod* method)
{
	return ((  GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* (*) (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*, const RuntimeMethod*))Object_Instantiate_TisRuntimeObject_m90A1E6C4C2B445D2E848DB75C772D1B95AAC046A_gshared)(___0_original, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object_set_name_mC79E6DC8FFD72479C90F0C4CC7F42A0FEAF5AE47 (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C* __this, String_t* ___0_value, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 Quaternion_get_identity_m7E701AE095ED10FD5EA0B50ABCFDE2EEFF2173A5_inline (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_FrameObject_m4F80B03857670CDAA5790C33318C434551757518 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___0_obj, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool String_EndsWith_mCD3754F5401E19CE7821CD398986E4EAA6AD87DC (String_t* __this, String_t* ___0_value, const RuntimeMethod* method) ;
inline GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* AssetBundle_LoadAsset_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m0216591F4BA27A483AA2C83EFDABCEA80B214063 (AssetBundle_tB38418819A49060CD738CB21541649340F082943* __this, String_t* ___0_name, const RuntimeMethod* method)
{
	return ((  GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* (*) (AssetBundle_tB38418819A49060CD738CB21541649340F082943*, String_t*, const RuntimeMethod*))AssetBundle_LoadAsset_TisRuntimeObject_m7F36E1C08FC32ADB47C34513522D958FA6E09C47_gshared)(__this, ___0_name, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_LogException_mAB3F4DC7297ED8FBB49DAA718B70E59A6B0171B0 (Exception_t* ___0_exception, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Color_tD001788D726C3A7F1379BEED0260B9591F440C1F Color_get_yellow_m66637FA14383E8D74F24AE256B577CE1D55D469F_inline (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RuntimeHelpers_InitializeArray_m751372AA3F24FBF6DA9B9D687CBFA2DE436CAB9B (RuntimeArray* ___0_array, RuntimeFieldHandle_t6E4C45B6D2EA12FC99185805A7E77527899B25C5 ___1_fldHandle, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 Quaternion_Internal_FromEulerRad_m66D4475341F53949471E6870FB5C5E4A5E9BA93E (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_euler, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Color__ctor_m3786F0D6E510D9CFA544523A955870BD2A514C8C_inline (Color_tD001788D726C3A7F1379BEED0260B9591F440C1F* __this, float ___0_r, float ___1_g, float ___2_b, float ___3_a, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Bounds_get_extents_mFE6DC407FCE2341BE2C750CB554055D211281D25_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds_Encapsulate_m1FCA57C58536ADB67B85A703470C6F5BFB837C2F_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_point, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Bounds_get_min_m465AC9BBE1DE5D8E8AD95AC19B9899068FEEBB13_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_Min_m1CAC3499F14EA87366C0C3C1F501B4FB2863CDB4_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_lhs, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___1_rhs, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Bounds_get_max_m6446F2AB97C1E57CA89467B9DE52D4EB61F1CB09_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_Max_m9B6D8FEE7F4CE32C0AAB682606FFBA59E1F37C74_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_lhs, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___1_rhs, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds_SetMinMax_mB5F7DDF18EDB7F3F25FA6D2B36824F28978C540F_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_min, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___1_max, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR float Mathf_Min_m747CA71A9483CDB394B13BD0AD048EE17E48FFE4_inline (float ___0_a, float ___1_b, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds_set_extents_m09496358547B86A93EFE7BE6371E7A6FE937C46F_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_value, const RuntimeMethod* method) ;
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_Start_m0E9258907AD8B616B3BEF8EB0EAB10D0F0CF6DA0 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, const RuntimeMethod* method) 
{
	{
		IPCServer_StartServer_mEEED2CC485A6F8F480485D1450EEA2DCACE51721(__this, NULL);
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_OnDestroy_m244D701ABC71101CC61140F0A74BFF8A45D546A0 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, const RuntimeMethod* method) 
{
	{
		IPCServer_StopServer_mAF9E0D6CF67851666CDCF92728BB246AA66649C6(__this, NULL);
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_StartServer_mEEED2CC485A6F8F480485D1450EEA2DCACE51721 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IPCServer_ListenForClients_m903C8DEE9A15C1CFCED40914F66C74E5DEFA2A76_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralAF0F1A1ECDA829CF1C6DB15A2C2EEE6AA30E5C57);
		s_Il2CppMethodInitialized = true;
	}
	{
		bool L_0 = __this->____isRunning;
		if (!L_0)
		{
			goto IL_0009;
		}
	}
	{
		return;
	}

IL_0009:
	{
		__this->____isRunning = (bool)1;
		ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2* L_1 = (ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2*)il2cpp_codegen_object_new(ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2_il2cpp_TypeInfo_var);
		ThreadStart__ctor_m35CE51F76358F20E4A0C8F2963C3AE14E5FF7E71(L_1, __this, (intptr_t)((void*)IPCServer_ListenForClients_m903C8DEE9A15C1CFCED40914F66C74E5DEFA2A76_RuntimeMethod_var), NULL);
		Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F* L_2 = (Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F*)il2cpp_codegen_object_new(Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F_il2cpp_TypeInfo_var);
		Thread__ctor_m1FA23A179DF4F5B659689D6FA40DE903086212E2(L_2, L_1, NULL);
		__this->____serverThread = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____serverThread), (void*)L_2);
		Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F* L_3 = __this->____serverThread;
		NullCheck(L_3);
		Thread_set_IsBackground_m45F00BD4C46F9B8A7C46A20A170B22BABB8FBA30(L_3, (bool)1, NULL);
		Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F* L_4 = __this->____serverThread;
		NullCheck(L_4);
		Thread_Start_m2230799AA331CA9ACEF49D5500281D0C1A9F72FA(L_4, NULL);
		int32_t L_5 = __this->___Port;
		int32_t L_6 = L_5;
		RuntimeObject* L_7 = Box(Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_il2cpp_TypeInfo_var, &L_6);
		String_t* L_8;
		L_8 = String_Format_mA8DBB4C2516B9723C5A41E6CB1E2FAF4BBE96DD8(_stringLiteralAF0F1A1ECDA829CF1C6DB15A2C2EEE6AA30E5C57, L_7, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_Log_m87A9A3C761FF5C43ED8A53B16190A53D08F818BB(L_8, NULL);
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_StopServer_mAF9E0D6CF67851666CDCF92728BB246AA66649C6 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, const RuntimeMethod* method) 
{
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	{
		__this->____isRunning = (bool)0;
		TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82* L_0 = __this->____server;
		if (!L_0)
		{
			goto IL_001f;
		}
	}
	try
	{
		TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82* L_1 = __this->____server;
		NullCheck(L_1);
		TcpListener_Stop_mBF4B354EB52138AC9A0184F186894EDBAE3BA5FD(L_1, NULL);
		goto IL_001f;
	}
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&RuntimeObject_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_001c;
		}
		throw e;
	}

CATCH_001c:
	{
		RuntimeObject* L_2 = ((RuntimeObject*)IL2CPP_GET_ACTIVE_EXCEPTION(RuntimeObject*));;
		IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
		goto IL_001f;
	}

IL_001f:
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_ListenForClients_m903C8DEE9A15C1CFCED40914F66C74E5DEFA2A76 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass9_0_U3CListenForClientsU3Eb__0_m5B6B69AD9B8AE8C69ECA544F5873515FA8708578_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9* V_0 = NULL;
	Exception_t* V_1 = NULL;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	try
	{
		{
			il2cpp_codegen_runtime_class_init_inline(IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484_il2cpp_TypeInfo_var);
			IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484* L_0 = ((IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484_StaticFields*)il2cpp_codegen_static_fields_for(IPAddress_t2F4486449B0D73FF2D3B36A9FE5E9C3F63116484_il2cpp_TypeInfo_var))->___Loopback;
			int32_t L_1 = __this->___Port;
			TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82* L_2 = (TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82*)il2cpp_codegen_object_new(TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82_il2cpp_TypeInfo_var);
			TcpListener__ctor_m6EDEF45E8F8F2872F3828E801806D9FEC3FF003B(L_2, L_0, L_1, NULL);
			__this->____server = L_2;
			Il2CppCodeGenWriteBarrier((void**)(&__this->____server), (void*)L_2);
			TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82* L_3 = __this->____server;
			NullCheck(L_3);
			TcpListener_Start_m919D559B138B311CFFBBE4BF66E326EABD8F8712(L_3, NULL);
			goto IL_0074_1;
		}

IL_0023_1:
		{
			U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9* L_4 = (U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9_il2cpp_TypeInfo_var);
			U3CU3Ec__DisplayClass9_0__ctor_m1F9EC985F755B288612CD7C04F5667362415DDC8(L_4, NULL);
			V_0 = L_4;
			U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9* L_5 = V_0;
			NullCheck(L_5);
			L_5->___U3CU3E4__this = __this;
			Il2CppCodeGenWriteBarrier((void**)(&L_5->___U3CU3E4__this), (void*)__this);
			TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82* L_6 = __this->____server;
			NullCheck(L_6);
			bool L_7;
			L_7 = TcpListener_Pending_m30EF809BFB0838CDFEAFA5A5065FB7C13E41DA11(L_6, NULL);
			if (L_7)
			{
				goto IL_0046_1;
			}
		}
		{
			Thread_Sleep_m71DE163765BF465EC4A0163F2ED4D43143094549(((int32_t)100), NULL);
			goto IL_0074_1;
		}

IL_0046_1:
		{
			U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9* L_8 = V_0;
			TcpListener_t306B041DAC7763F1A05DAA9FA9F4BAADEF94EF82* L_9 = __this->____server;
			NullCheck(L_9);
			TcpClient_t753B702EE06B59897564F75CEBFB6C8AFF10BD58* L_10;
			L_10 = TcpListener_AcceptTcpClient_mD7DFF1228EAB3F886B5BBC6175A0856C84F2B419(L_9, NULL);
			NullCheck(L_8);
			L_8->___client = L_10;
			Il2CppCodeGenWriteBarrier((void**)(&L_8->___client), (void*)L_10);
			U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9* L_11 = V_0;
			ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2* L_12 = (ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2*)il2cpp_codegen_object_new(ThreadStart_t7662D98F1793EDE90192D7304D1C3C44119328A2_il2cpp_TypeInfo_var);
			ThreadStart__ctor_m35CE51F76358F20E4A0C8F2963C3AE14E5FF7E71(L_12, L_11, (intptr_t)((void*)U3CU3Ec__DisplayClass9_0_U3CListenForClientsU3Eb__0_m5B6B69AD9B8AE8C69ECA544F5873515FA8708578_RuntimeMethod_var), NULL);
			Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F* L_13 = (Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F*)il2cpp_codegen_object_new(Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F_il2cpp_TypeInfo_var);
			Thread__ctor_m1FA23A179DF4F5B659689D6FA40DE903086212E2(L_13, L_12, NULL);
			Thread_t0A773B9DE873D2DCAA7D229EAB36757B500E207F* L_14 = L_13;
			NullCheck(L_14);
			Thread_set_IsBackground_m45F00BD4C46F9B8A7C46A20A170B22BABB8FBA30(L_14, (bool)1, NULL);
			NullCheck(L_14);
			Thread_Start_m2230799AA331CA9ACEF49D5500281D0C1A9F72FA(L_14, NULL);
		}

IL_0074_1:
		{
			bool L_15 = __this->____isRunning;
			if (L_15)
			{
				goto IL_0023_1;
			}
		}
		{
			goto IL_0096;
		}
	}
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Exception_t_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_007e;
		}
		throw e;
	}

CATCH_007e:
	{
		Exception_t* L_16 = ((Exception_t*)IL2CPP_GET_ACTIVE_EXCEPTION(Exception_t*));;
		V_1 = L_16;
		Exception_t* L_17 = V_1;
		NullCheck(L_17);
		String_t* L_18;
		L_18 = VirtualFuncInvoker0< String_t* >::Invoke(5, L_17);
		String_t* L_19;
		L_19 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralDBC773BB02113B1A7442300B1519C9B0D3783090)), L_18, NULL);
		il2cpp_codegen_runtime_class_init_inline(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var)));
		Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(L_19, NULL);
		IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
		goto IL_0096;
	}

IL_0096:
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_HandleClient_m581A8782BD4EFF003E93063317145E12678EF63D (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, TcpClient_t753B702EE06B59897564F75CEBFB6C8AFF10BD58* ___0_client, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentQueue_1_Enqueue_mCFA14FE2B9D147552CD2F7F340F53A0284BB92B8_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&StringBuilder_t_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	NetworkStream_tF39C3684B6D572BF47F518AD1DB1F4B12CEE4AE0* V_0 = NULL;
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* V_1 = NULL;
	int32_t V_2 = 0;
	StringBuilder_t* V_3 = NULL;
	String_t* V_4 = NULL;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	{
		auto __finallyBlock = il2cpp::utils::Finally([&]
		{

FINALLY_0060:
			{
				TcpClient_t753B702EE06B59897564F75CEBFB6C8AFF10BD58* L_0 = ___0_client;
				NullCheck(L_0);
				TcpClient_Close_m03E0ED4E4BA87B3F1ED17585AB1327ED76F5FE89(L_0, NULL);
				return;
			}
		});
		try
		{
			try
			{
				{
					TcpClient_t753B702EE06B59897564F75CEBFB6C8AFF10BD58* L_1 = ___0_client;
					NullCheck(L_1);
					NetworkStream_tF39C3684B6D572BF47F518AD1DB1F4B12CEE4AE0* L_2;
					L_2 = TcpClient_GetStream_mDD54336B17D1267BD593E0A1EB9EDF3E9506AEBA(L_1, NULL);
					V_0 = L_2;
					ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_3 = (ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)(ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)SZArrayNew(ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031_il2cpp_TypeInfo_var, (uint32_t)((int32_t)1024));
					V_1 = L_3;
					StringBuilder_t* L_4 = (StringBuilder_t*)il2cpp_codegen_object_new(StringBuilder_t_il2cpp_TypeInfo_var);
					StringBuilder__ctor_m1D99713357DE05DAFA296633639DB55F8C30587D(L_4, NULL);
					V_3 = L_4;
					goto IL_0032_2;
				}

IL_001a_2:
				{
					Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* L_5;
					L_5 = Encoding_get_UTF8_m9FA98A53CE96FD6D02982625C5246DD36C1235C9(NULL);
					ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_6 = V_1;
					int32_t L_7 = V_2;
					NullCheck(L_5);
					String_t* L_8;
					L_8 = VirtualFuncInvoker3< String_t*, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*, int32_t, int32_t >::Invoke(34, L_5, L_6, 0, L_7);
					V_4 = L_8;
					StringBuilder_t* L_9 = V_3;
					String_t* L_10 = V_4;
					NullCheck(L_9);
					StringBuilder_t* L_11;
					L_11 = StringBuilder_Append_m08904D74E0C78E5F36DCD9C9303BDD07886D9F7D(L_9, L_10, NULL);
				}

IL_0032_2:
				{
					NetworkStream_tF39C3684B6D572BF47F518AD1DB1F4B12CEE4AE0* L_12 = V_0;
					ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_13 = V_1;
					ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_14 = V_1;
					NullCheck(L_14);
					NullCheck(L_12);
					int32_t L_15;
					L_15 = VirtualFuncInvoker3< int32_t, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*, int32_t, int32_t >::Invoke(33, L_12, L_13, 0, ((int32_t)(((RuntimeArray*)L_14)->max_length)));
					int32_t L_16 = L_15;
					V_2 = L_16;
					if (L_16)
					{
						goto IL_001a_2;
					}
				}
				{
					StringBuilder_t* L_17 = V_3;
					NullCheck(L_17);
					int32_t L_18;
					L_18 = StringBuilder_get_Length_mDEA041E7357C68CC3B5885276BB403676DAAE0D8(L_17, NULL);
					if ((((int32_t)L_18) <= ((int32_t)0)))
					{
						goto IL_005b_2;
					}
				}
				{
					ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E* L_19 = __this->____commandQueue;
					StringBuilder_t* L_20 = V_3;
					NullCheck(L_20);
					String_t* L_21;
					L_21 = VirtualFuncInvoker0< String_t* >::Invoke(3, L_20);
					NullCheck(L_19);
					ConcurrentQueue_1_Enqueue_mCFA14FE2B9D147552CD2F7F340F53A0284BB92B8(L_19, L_21, ConcurrentQueue_1_Enqueue_mCFA14FE2B9D147552CD2F7F340F53A0284BB92B8_RuntimeMethod_var);
				}

IL_005b_2:
				{
					goto IL_0067;
				}
			}
			catch(Il2CppExceptionWrapper& e)
			{
				if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Exception_t_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
				{
					IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
					goto CATCH_005d_1;
				}
				throw e;
			}

CATCH_005d_1:
			{
				Exception_t* L_22 = ((Exception_t*)IL2CPP_GET_ACTIVE_EXCEPTION(Exception_t*));;
				IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
				goto IL_0067;
			}
		}
		catch(Il2CppExceptionWrapper& e)
		{
			__finallyBlock.StoreException(e.ex);
		}
	}

IL_0067:
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_Update_m0F5050359BEF3DD38B97EE3BD21E7A2CE849CCAB (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentQueue_1_TryDequeue_m176C65C9DCE178799C348336E14CB35C748E9421_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;

IL_0000:
	{
		ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E* L_0 = __this->____commandQueue;
		NullCheck(L_0);
		bool L_1;
		L_1 = ConcurrentQueue_1_TryDequeue_m176C65C9DCE178799C348336E14CB35C748E9421(L_0, (&V_0), ConcurrentQueue_1_TryDequeue_m176C65C9DCE178799C348336E14CB35C748E9421_RuntimeMethod_var);
		if (!L_1)
		{
			goto IL_0018;
		}
	}
	{
		String_t* L_2 = V_0;
		IPCServer_ProcessCommand_m6588DEA3CD42C3EE99E6CF76F1B16758BCDFEDAF(__this, L_2, NULL);
		goto IL_0000;
	}

IL_0018:
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_ProcessCommand_m6588DEA3CD42C3EE99E6CF76F1B16758BCDFEDAF (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, String_t* ___0_command, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral586BC205D60356D54AD7851D1CDAD06F92B0690D);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralA1E3D75D531E0513532A591BB5814CCE7805A947);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralD9256C1C85B054E8069CFC11922DF972997469F0);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralDED255C6188796AA0D317083FE9D01BC0BBC9276);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE5A618DB1CEDCF822A5199D3E96A878869B319FA);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* V_1 = NULL;
	float V_2 = 0.0f;
	float V_3 = 0.0f;
	OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* V_4 = NULL;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* V_5 = NULL;
	OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* V_6 = NULL;
	{
		String_t* L_0 = ___0_command;
		NullCheck(L_0);
		String_t* L_1;
		L_1 = String_Trim_mCD6D8C6D4CFD15225D12DB7D3E0544CA80FB8DA5(L_0, NULL);
		___0_command = L_1;
		String_t* L_2 = ___0_command;
		String_t* L_3;
		L_3 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteralA1E3D75D531E0513532A591BB5814CCE7805A947, L_2, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_Log_m87A9A3C761FF5C43ED8A53B16190A53D08F818BB(L_3, NULL);
		String_t* L_4 = ___0_command;
		NullCheck(L_4);
		bool L_5;
		L_5 = String_StartsWith_mF75DBA1EB709811E711B44E26FF919C88A8E65C0(L_4, _stringLiteral586BC205D60356D54AD7851D1CDAD06F92B0690D, NULL);
		if (!L_5)
		{
			goto IL_0035;
		}
	}
	{
		String_t* L_6 = ___0_command;
		NullCheck(L_6);
		String_t* L_7;
		L_7 = String_Substring_m6BA4A3FA3800FE92662D0847CC8E1EEF940DF472(L_6, 7, NULL);
		V_0 = L_7;
		String_t* L_8 = V_0;
		IPCServer_SelectObject_m1D2832DC5B52158338859DF584D3364C4F9AB1E5(__this, L_8, NULL);
		return;
	}

IL_0035:
	{
		String_t* L_9 = ___0_command;
		NullCheck(L_9);
		bool L_10;
		L_10 = String_StartsWith_mF75DBA1EB709811E711B44E26FF919C88A8E65C0(L_9, _stringLiteralD9256C1C85B054E8069CFC11922DF972997469F0, NULL);
		if (!L_10)
		{
			goto IL_008e;
		}
	}
	{
		String_t* L_11 = ___0_command;
		NullCheck(L_11);
		String_t* L_12;
		L_12 = String_Substring_m6BA4A3FA3800FE92662D0847CC8E1EEF940DF472(L_11, 7, NULL);
		NullCheck(L_12);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_13;
		L_13 = String_Split_m9530B73D02054692283BF35C3A27C8F2230946F4(L_12, ((int32_t)58), 0, NULL);
		V_1 = L_13;
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_14 = V_1;
		NullCheck(L_14);
		if ((!(((uint32_t)((int32_t)(((RuntimeArray*)L_14)->max_length))) == ((uint32_t)2))))
		{
			goto IL_00dc;
		}
	}
	{
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_15 = V_1;
		NullCheck(L_15);
		int32_t L_16 = 0;
		String_t* L_17 = (L_15)->GetAt(static_cast<il2cpp_array_size_t>(L_16));
		bool L_18;
		L_18 = Single_TryParse_mC39FC2D8F1F33DE64685F3A8CE6BDD2164FB9423(L_17, (&V_2), NULL);
		if (!L_18)
		{
			goto IL_00dc;
		}
	}
	{
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_19 = V_1;
		NullCheck(L_19);
		int32_t L_20 = 1;
		String_t* L_21 = (L_19)->GetAt(static_cast<il2cpp_array_size_t>(L_20));
		bool L_22;
		L_22 = Single_TryParse_mC39FC2D8F1F33DE64685F3A8CE6BDD2164FB9423(L_21, (&V_3), NULL);
		if (!L_22)
		{
			goto IL_00dc;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_23;
		L_23 = Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E(Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E_RuntimeMethod_var);
		V_4 = L_23;
		OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_24 = V_4;
		bool L_25;
		L_25 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_24, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_25)
		{
			goto IL_00dc;
		}
	}
	{
		OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_26 = V_4;
		float L_27 = V_2;
		float L_28 = V_3;
		NullCheck(L_26);
		OrbitalCamera_SetForcedResolution_mB14EB0E2178E7384E0AAB5E50D7AD2B22416982D(L_26, L_27, L_28, NULL);
		return;
	}

IL_008e:
	{
		String_t* L_29 = ___0_command;
		NullCheck(L_29);
		bool L_30;
		L_30 = String_StartsWith_mF75DBA1EB709811E711B44E26FF919C88A8E65C0(L_29, _stringLiteralE5A618DB1CEDCF822A5199D3E96A878869B319FA, NULL);
		if (!L_30)
		{
			goto IL_00d0;
		}
	}
	{
		String_t* L_31 = ___0_command;
		NullCheck(L_31);
		String_t* L_32;
		L_32 = String_Substring_m6BA4A3FA3800FE92662D0847CC8E1EEF940DF472(L_31, 5, NULL);
		CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB* L_33 = (CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB*)(CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB*)SZArrayNew(CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB_il2cpp_TypeInfo_var, (uint32_t)1);
		CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB* L_34 = L_33;
		NullCheck(L_34);
		(L_34)->SetAt(static_cast<il2cpp_array_size_t>(0), (Il2CppChar)((int32_t)124));
		NullCheck(L_32);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_35;
		L_35 = String_Split_m6811F7CF4DE7C68019D76E9DC6D17B2293BA8B12(L_32, L_34, 1, NULL);
		V_5 = L_35;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_36;
		L_36 = Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E(Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E_RuntimeMethod_var);
		V_6 = L_36;
		OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_37 = V_6;
		bool L_38;
		L_38 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_37, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_38)
		{
			goto IL_00dc;
		}
	}
	{
		OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_39 = V_6;
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_40 = V_5;
		NullCheck(L_39);
		OrbitalCamera_ShowMultipleObjects_m11BE9D20EA8BD547EA9D2F50AB350E48A26A3775(L_39, L_40, NULL);
		return;
	}

IL_00d0:
	{
		String_t* L_41 = ___0_command;
		bool L_42;
		L_42 = String_op_Equality_m030E1B219352228970A076136E455C4E568C02C1(L_41, _stringLiteralDED255C6188796AA0D317083FE9D01BC0BBC9276, NULL);
	}

IL_00dc:
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer_SelectObject_m1D2832DC5B52158338859DF584D3364C4F9AB1E5 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, String_t* ___0_name, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral10C85C8B45E1ABC7EA6F5FBD8A32715BEE966E5D);
		s_Il2CppMethodInitialized = true;
	}
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* V_0 = NULL;
	OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* V_1 = NULL;
	{
		String_t* L_0 = ___0_name;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_1;
		L_1 = IPCServer_FindGameObjectByName_mFC62A5F91177B967D029E7309E7C888C09499BC4(__this, L_0, NULL);
		V_0 = L_1;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_2 = V_0;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_3;
		L_3 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_2, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_3)
		{
			goto IL_0028;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_4;
		L_4 = Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E(Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E_RuntimeMethod_var);
		V_1 = L_4;
		OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_5 = V_1;
		bool L_6;
		L_6 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_5, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_6)
		{
			goto IL_0038;
		}
	}
	{
		OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_7 = V_1;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_8 = V_0;
		NullCheck(L_7);
		OrbitalCamera_SetTargetHighlight_m1065C1EDBCA87BC32062E9A592F04080954855E9(L_7, L_8, NULL);
		return;
	}

IL_0028:
	{
		String_t* L_9 = ___0_name;
		String_t* L_10;
		L_10 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteral10C85C8B45E1ABC7EA6F5FBD8A32715BEE966E5D, L_9, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_LogWarning_m33EF1B897E0C7C6FF538989610BFAFFEF4628CA9(L_10, NULL);
	}

IL_0038:
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* IPCServer_FindGameObjectByName_mFC62A5F91177B967D029E7309E7C888C09499BC4 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, String_t* ___0_name, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&SceneManager_tA0EF56A88ACA4A15731AF7FDC10A869FA4C698FA_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* V_0 = NULL;
	GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* V_1 = NULL;
	int32_t V_2 = 0;
	Scene_tA1DC762B79745EB5140F054C884855B922318356 V_3;
	memset((&V_3), 0, sizeof(V_3));
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* V_4 = NULL;
	Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* V_5 = NULL;
	{
		String_t* L_0 = ___0_name;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_1;
		L_1 = GameObject_Find_m7A669B4EEC2617AB82F6E3FF007CDCD9F21DB300(L_0, NULL);
		V_0 = L_1;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_2 = V_0;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_3;
		L_3 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_2, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_3)
		{
			goto IL_0012;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_4 = V_0;
		return L_4;
	}

IL_0012:
	{
		il2cpp_codegen_runtime_class_init_inline(SceneManager_tA0EF56A88ACA4A15731AF7FDC10A869FA4C698FA_il2cpp_TypeInfo_var);
		Scene_tA1DC762B79745EB5140F054C884855B922318356 L_5;
		L_5 = SceneManager_GetActiveScene_m0B320EC4302F51A71495D1CCD1A0FF9C2ED1FDC8(NULL);
		V_3 = L_5;
		GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_6;
		L_6 = Scene_GetRootGameObjects_mFDE0BF9EA926F30EC9AE71F33E0AE4D5D4EC5C5B((&V_3), NULL);
		V_1 = L_6;
		V_2 = 0;
		goto IL_004f;
	}

IL_0024:
	{
		GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_7 = V_1;
		int32_t L_8 = V_2;
		NullCheck(L_7);
		int32_t L_9 = L_8;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_10 = (L_7)->GetAt(static_cast<il2cpp_array_size_t>(L_9));
		V_4 = L_10;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_11 = V_4;
		NullCheck(L_11);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_12;
		L_12 = GameObject_get_transform_m0BC10ADFA1632166AE5544BDF9038A2650C2AE56(L_11, NULL);
		String_t* L_13 = ___0_name;
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_14;
		L_14 = IPCServer_FindRecursive_m67FBDCAA54FE211A941B39A8859B8136795B6DB2(__this, L_12, L_13, NULL);
		V_5 = L_14;
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_15 = V_5;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_16;
		L_16 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_15, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_16)
		{
			goto IL_004b;
		}
	}
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_17 = V_5;
		NullCheck(L_17);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_18;
		L_18 = Component_get_gameObject_m57AEFBB14DB39EC476F740BA000E170355DE691B(L_17, NULL);
		return L_18;
	}

IL_004b:
	{
		int32_t L_19 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add(L_19, 1));
	}

IL_004f:
	{
		int32_t L_20 = V_2;
		GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_21 = V_1;
		NullCheck(L_21);
		if ((((int32_t)L_20) < ((int32_t)((int32_t)(((RuntimeArray*)L_21)->max_length)))))
		{
			goto IL_0024;
		}
	}
	{
		return (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* IPCServer_FindRecursive_m67FBDCAA54FE211A941B39A8859B8136795B6DB2 (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* ___0_parent, String_t* ___1_name, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* V_0 = NULL;
	Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* V_1 = NULL;
	Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* V_2 = NULL;
	Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* V_3 = NULL;
	RuntimeObject* V_4 = NULL;
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_0 = ___0_parent;
		NullCheck(L_0);
		String_t* L_1;
		L_1 = Object_get_name_mAC2F6B897CF1303BA4249B4CB55271AFACBB6392(L_0, NULL);
		String_t* L_2 = ___1_name;
		bool L_3;
		L_3 = String_op_Equality_m030E1B219352228970A076136E455C4E568C02C1(L_1, L_2, NULL);
		if (!L_3)
		{
			goto IL_0010;
		}
	}
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_4 = ___0_parent;
		return L_4;
	}

IL_0010:
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_5 = ___0_parent;
		NullCheck(L_5);
		RuntimeObject* L_6;
		L_6 = Transform_GetEnumerator_mA7E1C882ACA0C33E284711CD09971DEA3FFEF404(L_5, NULL);
		V_0 = L_6;
	}
	{
		auto __finallyBlock = il2cpp::utils::Finally([&]
		{

FINALLY_0045:
			{
				{
					RuntimeObject* L_7 = V_0;
					V_4 = ((RuntimeObject*)IsInst((RuntimeObject*)L_7, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var));
					RuntimeObject* L_8 = V_4;
					if (!L_8)
					{
						goto IL_0058;
					}
				}
				{
					RuntimeObject* L_9 = V_4;
					NullCheck(L_9);
					InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_9);
				}

IL_0058:
				{
					return;
				}
			}
		});
		try
		{
			{
				goto IL_003b_1;
			}

IL_0019_1:
			{
				RuntimeObject* L_10 = V_0;
				NullCheck(L_10);
				RuntimeObject* L_11;
				L_11 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(1, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_10);
				V_1 = ((Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1*)CastclassClass((RuntimeObject*)L_11, Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1_il2cpp_TypeInfo_var));
				Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_12 = V_1;
				String_t* L_13 = ___1_name;
				Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_14;
				L_14 = IPCServer_FindRecursive_m67FBDCAA54FE211A941B39A8859B8136795B6DB2(__this, L_12, L_13, NULL);
				V_2 = L_14;
				Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_15 = V_2;
				il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
				bool L_16;
				L_16 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_15, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
				if (!L_16)
				{
					goto IL_003b_1;
				}
			}
			{
				Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_17 = V_2;
				V_3 = L_17;
				goto IL_005b;
			}

IL_003b_1:
			{
				RuntimeObject* L_18 = V_0;
				NullCheck(L_18);
				bool L_19;
				L_19 = InterfaceFuncInvoker0< bool >::Invoke(0, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_18);
				if (L_19)
				{
					goto IL_0019_1;
				}
			}
			{
				goto IL_0059;
			}
		}
		catch(Il2CppExceptionWrapper& e)
		{
			__finallyBlock.StoreException(e.ex);
		}
	}

IL_0059:
	{
		return (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1*)NULL;
	}

IL_005b:
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_20 = V_3;
		return L_20;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IPCServer__ctor_mA4F49A1966C3F124B4057050EB5FBA6C349A384A (IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentQueue_1__ctor_m89BBD6C7776D544A0D57C1322765E6D9BC513CB1_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E* L_0 = (ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E*)il2cpp_codegen_object_new(ConcurrentQueue_1_t5FC3B6978FAD20F80C05DC1CC2B9F6FAD0ABA64E_il2cpp_TypeInfo_var);
		ConcurrentQueue_1__ctor_m89BBD6C7776D544A0D57C1322765E6D9BC513CB1(L_0, ConcurrentQueue_1__ctor_m89BBD6C7776D544A0D57C1322765E6D9BC513CB1_RuntimeMethod_var);
		__this->____commandQueue = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____commandQueue), (void*)L_0);
		__this->___Port = ((int32_t)39215);
		MonoBehaviour__ctor_m592DB0105CA0BC97AA1C5F4AD27B12D68A3B7C1E(__this, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass9_0__ctor_m1F9EC985F755B288612CD7C04F5667362415DDC8 (U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9* __this, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass9_0_U3CListenForClientsU3Eb__0_m5B6B69AD9B8AE8C69ECA544F5873515FA8708578 (U3CU3Ec__DisplayClass9_0_t637ECCD70A9803B1F2AED837160451C7DBDC54D9* __this, const RuntimeMethod* method) 
{
	{
		IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* L_0 = __this->___U3CU3E4__this;
		TcpClient_t753B702EE06B59897564F75CEBFB6C8AFF10BD58* L_1 = __this->___client;
		NullCheck(L_0);
		IPCServer_HandleClient_m581A8782BD4EFF003E93063317145E12678EF63D(L_0, L_1, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_SetForcedResolution_mB14EB0E2178E7384E0AAB5E50D7AD2B22416982D (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, float ___0_width, float ___1_height, const RuntimeMethod* method) 
{
	{
		float L_0 = ___1_height;
		if ((!(((float)L_0) > ((float)(0.0f)))))
		{
			goto IL_0011;
		}
	}
	{
		float L_1 = ___0_width;
		float L_2 = ___1_height;
		__this->____forcedAspect = ((float)(L_1/L_2));
	}

IL_0011:
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_Start_m453ED87C48822D84F84306104ADE3CCD89194215 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_0;
		L_0 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		NullCheck(L_0);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1;
		L_1 = Transform_get_eulerAngles_mCAAF48EFCF628F1ED91C2FFE75A4FD19C039DD6A(L_0, NULL);
		V_0 = L_1;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2 = V_0;
		float L_3 = L_2.___y;
		__this->____rotationY = L_3;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_4 = V_0;
		float L_5 = L_4.___x;
		__this->____rotationX = L_5;
		Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* L_6;
		L_6 = Camera_get_main_m52C992F18E05355ABB9EEB64A4BF2215E12762DF(NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_7;
		L_7 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_6, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_7)
		{
			goto IL_0048;
		}
	}
	{
		Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* L_8;
		L_8 = Camera_get_main_m52C992F18E05355ABB9EEB64A4BF2215E12762DF(NULL);
		int32_t L_9;
		L_9 = Screen_get_width_mF608FF3252213E7EFA1F0D2F744C28110E9E5AC9(NULL);
		int32_t L_10;
		L_10 = Screen_get_height_m01A3102DE71EE1FBEA51D09D6B0261CF864FE8F9(NULL);
		NullCheck(L_8);
		Camera_set_aspect_m537F21B48FDD5C060DFF9D87F34F4FB2B0F9BEB6(L_8, ((float)(((float)L_9)/((float)L_10))), NULL);
	}

IL_0048:
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_Update_mEC94DF74A6FE5F3013EAF93947F47AD0D93AAB6E (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral16DD21BE77B115D392226EB71A2D3A9FDC29E3F0);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral88BEE283254D7094E258B3A88730F4CC4F1E4AC7);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralFC6687DC37346CD2569888E29764F727FAF530E0);
		s_Il2CppMethodInitialized = true;
	}
	float V_0 = 0.0f;
	float V_1 = 0.0f;
	float V_2 = 0.0f;
	float V_3 = 0.0f;
	float V_4 = 0.0f;
	float V_5 = 0.0f;
	float V_6 = 0.0f;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_7;
	memset((&V_7), 0, sizeof(V_7));
	float G_B7_0 = 0.0f;
	float G_B7_1 = 0.0f;
	OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* G_B7_2 = NULL;
	float G_B6_0 = 0.0f;
	float G_B6_1 = 0.0f;
	OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* G_B6_2 = NULL;
	int32_t G_B8_0 = 0;
	float G_B8_1 = 0.0f;
	float G_B8_2 = 0.0f;
	OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* G_B8_3 = NULL;
	float G_B17_0 = 0.0f;
	{
		Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* L_0;
		L_0 = Camera_get_main_m52C992F18E05355ABB9EEB64A4BF2215E12762DF(NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_1;
		L_1 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_0, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_1)
		{
			goto IL_0043;
		}
	}
	{
		float L_2 = __this->____forcedAspect;
		if ((!(((float)L_2) > ((float)(0.0f)))))
		{
			goto IL_002c;
		}
	}
	{
		Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* L_3;
		L_3 = Camera_get_main_m52C992F18E05355ABB9EEB64A4BF2215E12762DF(NULL);
		float L_4 = __this->____forcedAspect;
		NullCheck(L_3);
		Camera_set_aspect_m537F21B48FDD5C060DFF9D87F34F4FB2B0F9BEB6(L_3, L_4, NULL);
		goto IL_0043;
	}

IL_002c:
	{
		Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* L_5;
		L_5 = Camera_get_main_m52C992F18E05355ABB9EEB64A4BF2215E12762DF(NULL);
		int32_t L_6;
		L_6 = Screen_get_width_mF608FF3252213E7EFA1F0D2F744C28110E9E5AC9(NULL);
		int32_t L_7;
		L_7 = Screen_get_height_m01A3102DE71EE1FBEA51D09D6B0261CF864FE8F9(NULL);
		NullCheck(L_5);
		Camera_set_aspect_m537F21B48FDD5C060DFF9D87F34F4FB2B0F9BEB6(L_5, ((float)(((float)L_6)/((float)L_7))), NULL);
	}

IL_0043:
	{
		bool L_8;
		L_8 = Input_GetMouseButton_m4995DD4A2D4F916565C1B1B5AAF7DF17C126B3EA(1, NULL);
		if (!L_8)
		{
			goto IL_00d8;
		}
	}
	{
		float L_9;
		L_9 = Input_GetAxis_m10372E6C5FF591668D2DC5F58C58D213CC598A62(_stringLiteral88BEE283254D7094E258B3A88730F4CC4F1E4AC7, NULL);
		float L_10 = __this->___lookSpeed;
		V_1 = ((float)il2cpp_codegen_multiply(L_9, L_10));
		float L_11;
		L_11 = Input_GetAxis_m10372E6C5FF591668D2DC5F58C58D213CC598A62(_stringLiteral16DD21BE77B115D392226EB71A2D3A9FDC29E3F0, NULL);
		float L_12 = __this->___lookSpeed;
		V_2 = ((float)il2cpp_codegen_multiply(L_11, L_12));
		float L_13 = __this->____rotationY;
		float L_14 = V_1;
		__this->____rotationY = ((float)il2cpp_codegen_add(L_13, L_14));
		float L_15 = __this->____rotationX;
		float L_16 = V_2;
		bool L_17 = __this->___invertY;
		if (L_17)
		{
			G_B7_0 = L_16;
			G_B7_1 = L_15;
			G_B7_2 = __this;
			goto IL_0093;
		}
		G_B6_0 = L_16;
		G_B6_1 = L_15;
		G_B6_2 = __this;
	}
	{
		G_B8_0 = 1;
		G_B8_1 = G_B6_0;
		G_B8_2 = G_B6_1;
		G_B8_3 = G_B6_2;
		goto IL_0094;
	}

IL_0093:
	{
		G_B8_0 = (-1);
		G_B8_1 = G_B7_0;
		G_B8_2 = G_B7_1;
		G_B8_3 = G_B7_2;
	}

IL_0094:
	{
		NullCheck(G_B8_3);
		G_B8_3->____rotationX = ((float)il2cpp_codegen_subtract(G_B8_2, ((float)il2cpp_codegen_multiply(G_B8_1, ((float)G_B8_0)))));
		float L_18 = __this->____rotationX;
		float L_19;
		L_19 = Mathf_Clamp_mEB9AEA827D27D20FCC787F7375156AF46BB12BBF_inline(L_18, (-90.0f), (90.0f), NULL);
		__this->____rotationX = L_19;
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_20;
		L_20 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		float L_21 = __this->____rotationX;
		float L_22 = __this->____rotationY;
		Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 L_23;
		L_23 = Quaternion_Euler_m9262AB29E3E9CE94EF71051F38A28E82AEC73F90_inline(L_21, L_22, (0.0f), NULL);
		NullCheck(L_20);
		Transform_set_rotation_m61340DE74726CF0F9946743A727C4D444397331D(L_20, L_23, NULL);
	}

IL_00d8:
	{
		bool L_24;
		L_24 = Input_GetMouseButton_m4995DD4A2D4F916565C1B1B5AAF7DF17C126B3EA(2, NULL);
		if (!L_24)
		{
			goto IL_0127;
		}
	}
	{
		float L_25;
		L_25 = Input_GetAxis_m10372E6C5FF591668D2DC5F58C58D213CC598A62(_stringLiteral88BEE283254D7094E258B3A88730F4CC4F1E4AC7, NULL);
		float L_26 = __this->___panSpeed;
		float L_27;
		L_27 = Time_get_deltaTime_mC3195000401F0FD167DD2F948FD2BC58330D0865(NULL);
		V_3 = ((float)il2cpp_codegen_multiply(((float)il2cpp_codegen_multiply(((-L_25)), L_26)), L_27));
		float L_28;
		L_28 = Input_GetAxis_m10372E6C5FF591668D2DC5F58C58D213CC598A62(_stringLiteral16DD21BE77B115D392226EB71A2D3A9FDC29E3F0, NULL);
		float L_29 = __this->___panSpeed;
		float L_30;
		L_30 = Time_get_deltaTime_mC3195000401F0FD167DD2F948FD2BC58330D0865(NULL);
		V_4 = ((float)il2cpp_codegen_multiply(((float)il2cpp_codegen_multiply(((-L_28)), L_29)), L_30));
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_31;
		L_31 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		float L_32 = V_3;
		float L_33 = V_4;
		NullCheck(L_31);
		Transform_Translate_m8FF175E92671A19466A7728FEB92EFB4E999EB56(L_31, L_32, L_33, (0.0f), 1, NULL);
	}

IL_0127:
	{
		float L_34;
		L_34 = Input_GetAxis_m10372E6C5FF591668D2DC5F58C58D213CC598A62(_stringLiteralFC6687DC37346CD2569888E29764F727FAF530E0, NULL);
		V_0 = L_34;
		float L_35 = V_0;
		float L_36;
		L_36 = fabsf(L_35);
		if ((!(((float)L_36) > ((float)(0.00100000005f)))))
		{
			goto IL_016d;
		}
	}
	{
		float L_37 = __this->___moveSpeed;
		V_5 = ((float)il2cpp_codegen_multiply(L_37, (5.0f)));
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_38;
		L_38 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		float L_39 = V_0;
		float L_40 = V_5;
		float L_41;
		L_41 = Time_get_deltaTime_mC3195000401F0FD167DD2F948FD2BC58330D0865(NULL);
		NullCheck(L_38);
		Transform_Translate_m8FF175E92671A19466A7728FEB92EFB4E999EB56(L_38, (0.0f), (0.0f), ((float)il2cpp_codegen_multiply(((float)il2cpp_codegen_multiply(L_39, L_40)), L_41)), 1, NULL);
	}

IL_016d:
	{
		bool L_42;
		L_42 = Input_GetMouseButton_m4995DD4A2D4F916565C1B1B5AAF7DF17C126B3EA(1, NULL);
		if (!L_42)
		{
			goto IL_0266;
		}
	}
	{
		bool L_43;
		L_43 = Input_GetKey_mE5681EF775F3CEBA7EAD7C63984F7B34C8E8D434(((int32_t)304), NULL);
		if (L_43)
		{
			goto IL_018c;
		}
	}
	{
		float L_44 = __this->___moveSpeed;
		G_B17_0 = L_44;
		goto IL_0192;
	}

IL_018c:
	{
		float L_45 = __this->___turboSpeed;
		G_B17_0 = L_45;
	}

IL_0192:
	{
		V_6 = G_B17_0;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_46;
		L_46 = Vector3_get_zero_m0C1249C3F25B1C70EAD3CC8B31259975A457AE39_inline(NULL);
		V_7 = L_46;
		bool L_47;
		L_47 = Input_GetKey_mE5681EF775F3CEBA7EAD7C63984F7B34C8E8D434(((int32_t)119), NULL);
		if (!L_47)
		{
			goto IL_01b8;
		}
	}
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_48 = V_7;
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_49;
		L_49 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		NullCheck(L_49);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_50;
		L_50 = Transform_get_forward_mFCFACF7165FDAB21E80E384C494DF278386CEE2F(L_49, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_51;
		L_51 = Vector3_op_Addition_m78C0EC70CB66E8DCAC225743D82B268DAEE92067_inline(L_48, L_50, NULL);
		V_7 = L_51;
	}

IL_01b8:
	{
		bool L_52;
		L_52 = Input_GetKey_mE5681EF775F3CEBA7EAD7C63984F7B34C8E8D434(((int32_t)115), NULL);
		if (!L_52)
		{
			goto IL_01d5;
		}
	}
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_53 = V_7;
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_54;
		L_54 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		NullCheck(L_54);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_55;
		L_55 = Transform_get_forward_mFCFACF7165FDAB21E80E384C494DF278386CEE2F(L_54, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_56;
		L_56 = Vector3_op_Subtraction_mE42023FF80067CB44A1D4A27EB7CF2B24CABB828_inline(L_53, L_55, NULL);
		V_7 = L_56;
	}

IL_01d5:
	{
		bool L_57;
		L_57 = Input_GetKey_mE5681EF775F3CEBA7EAD7C63984F7B34C8E8D434(((int32_t)97), NULL);
		if (!L_57)
		{
			goto IL_01f2;
		}
	}
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_58 = V_7;
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_59;
		L_59 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		NullCheck(L_59);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_60;
		L_60 = Transform_get_right_mC6DC057C23313802E2186A9E0DB760D795A758A4(L_59, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_61;
		L_61 = Vector3_op_Subtraction_mE42023FF80067CB44A1D4A27EB7CF2B24CABB828_inline(L_58, L_60, NULL);
		V_7 = L_61;
	}

IL_01f2:
	{
		bool L_62;
		L_62 = Input_GetKey_mE5681EF775F3CEBA7EAD7C63984F7B34C8E8D434(((int32_t)100), NULL);
		if (!L_62)
		{
			goto IL_020f;
		}
	}
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_63 = V_7;
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_64;
		L_64 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		NullCheck(L_64);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_65;
		L_65 = Transform_get_right_mC6DC057C23313802E2186A9E0DB760D795A758A4(L_64, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_66;
		L_66 = Vector3_op_Addition_m78C0EC70CB66E8DCAC225743D82B268DAEE92067_inline(L_63, L_65, NULL);
		V_7 = L_66;
	}

IL_020f:
	{
		bool L_67;
		L_67 = Input_GetKey_mE5681EF775F3CEBA7EAD7C63984F7B34C8E8D434(((int32_t)101), NULL);
		if (!L_67)
		{
			goto IL_0226;
		}
	}
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_68 = V_7;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_69;
		L_69 = Vector3_get_up_m128AF3FDC820BF59D5DE86D973E7DE3F20C3AEBA_inline(NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_70;
		L_70 = Vector3_op_Addition_m78C0EC70CB66E8DCAC225743D82B268DAEE92067_inline(L_68, L_69, NULL);
		V_7 = L_70;
	}

IL_0226:
	{
		bool L_71;
		L_71 = Input_GetKey_mE5681EF775F3CEBA7EAD7C63984F7B34C8E8D434(((int32_t)113), NULL);
		if (!L_71)
		{
			goto IL_023d;
		}
	}
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_72 = V_7;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_73;
		L_73 = Vector3_get_down_mF62B2AE7C5AC31EAC9CB62797C7190C90A7A8599_inline(NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_74;
		L_74 = Vector3_op_Addition_m78C0EC70CB66E8DCAC225743D82B268DAEE92067_inline(L_72, L_73, NULL);
		V_7 = L_74;
	}

IL_023d:
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_75;
		L_75 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_76 = L_75;
		NullCheck(L_76);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_77;
		L_77 = Transform_get_position_m69CD5FA214FDAE7BB701552943674846C220FDE1(L_76, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_78 = V_7;
		float L_79 = V_6;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_80;
		L_80 = Vector3_op_Multiply_m87BA7C578F96C8E49BB07088DAAC4649F83B0353_inline(L_78, L_79, NULL);
		float L_81;
		L_81 = Time_get_deltaTime_mC3195000401F0FD167DD2F948FD2BC58330D0865(NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_82;
		L_82 = Vector3_op_Multiply_m87BA7C578F96C8E49BB07088DAAC4649F83B0353_inline(L_80, L_81, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_83;
		L_83 = Vector3_op_Addition_m78C0EC70CB66E8DCAC225743D82B268DAEE92067_inline(L_77, L_82, NULL);
		NullCheck(L_76);
		Transform_set_position_mA1A817124BB41B685043DED2A9BA48CDF37C4156(L_76, L_83, NULL);
	}

IL_0266:
	{
		bool L_84;
		L_84 = Input_GetKeyDown_mB237DEA6244132670D38990BAB77D813FBB028D2(((int32_t)102), NULL);
		if (!L_84)
		{
			goto IL_0275;
		}
	}
	{
		OrbitalCamera_FrameAll_m2447ACDD9F3F04FC9B3EE6A55987AB693DC96CB6(__this, NULL);
	}

IL_0275:
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_SetTargetHighlight_m1065C1EDBCA87BC32062E9A592F04080954855E9 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___0_obj, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_0 = ___0_obj;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_1;
		L_1 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_0, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_1)
		{
			goto IL_000a;
		}
	}
	{
		return;
	}

IL_000a:
	{
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_2 = (StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)SZArrayNew(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var, (uint32_t)1);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_3 = L_2;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_4 = ___0_obj;
		NullCheck(L_4);
		String_t* L_5;
		L_5 = Object_get_name_mAC2F6B897CF1303BA4249B4CB55271AFACBB6392(L_4, NULL);
		NullCheck(L_3);
		(L_3)->SetAt(static_cast<il2cpp_array_size_t>(0), (String_t*)L_5);
		OrbitalCamera_ShowMultipleObjects_m11BE9D20EA8BD547EA9D2F50AB350E48A26A3775(__this, L_3, NULL);
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_ShowMultipleObjects_m11BE9D20EA8BD547EA9D2F50AB350E48A26A3775 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___0_names, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponent_TisParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1_mF2B700ECAEEEDBCC076CC66C9592C6DDFA0D36F9_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_GetComponentsInChildren_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mA2746088DB45856FD76C725AB403CEF5A8997734_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&HashSet_1_Add_m899003BF81A20D05B1D48080CB373E701BA64652_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&HashSet_1_Contains_m21126165891AF3C847A2737D98F1B9F853656B7E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&HashSet_1__ctor_mFC9E6E517FB114988F9789C7D16A02D65CB87F71_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_FindObjectsOfType_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mC338358A9594B72942DC6AE839BC11C37E5CF8AD_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* V_0 = NULL;
	HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D* V_1 = NULL;
	bool V_2 = false;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* V_3 = NULL;
	int32_t V_4 = 0;
	String_t* V_5 = NULL;
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* V_6 = NULL;
	RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* V_7 = NULL;
	int32_t V_8 = 0;
	Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* V_9 = NULL;
	Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* V_10 = NULL;
	bool V_11 = false;
	int32_t G_B3_0 = 0;
	int32_t G_B18_0 = 0;
	{
		__this->____highlightedObject = (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____highlightedObject), (void*)(GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* L_0;
		L_0 = Object_FindObjectsOfType_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mC338358A9594B72942DC6AE839BC11C37E5CF8AD(Object_FindObjectsOfType_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mC338358A9594B72942DC6AE839BC11C37E5CF8AD_RuntimeMethod_var);
		V_0 = L_0;
		HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D* L_1 = (HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D*)il2cpp_codegen_object_new(HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D_il2cpp_TypeInfo_var);
		HashSet_1__ctor_mFC9E6E517FB114988F9789C7D16A02D65CB87F71(L_1, HashSet_1__ctor_mFC9E6E517FB114988F9789C7D16A02D65CB87F71_RuntimeMethod_var);
		V_1 = L_1;
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_2 = ___0_names;
		if (!L_2)
		{
			goto IL_001d;
		}
	}
	{
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_3 = ___0_names;
		NullCheck(L_3);
		G_B3_0 = ((((int32_t)(((RuntimeArray*)L_3)->max_length)) == ((int32_t)0))? 1 : 0);
		goto IL_001e;
	}

IL_001d:
	{
		G_B3_0 = 1;
	}

IL_001e:
	{
		V_2 = (bool)G_B3_0;
		bool L_4 = V_2;
		if (L_4)
		{
			goto IL_0094;
		}
	}
	{
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_5 = ___0_names;
		V_3 = L_5;
		V_4 = 0;
		goto IL_008d;
	}

IL_0029:
	{
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_6 = V_3;
		int32_t L_7 = V_4;
		NullCheck(L_6);
		int32_t L_8 = L_7;
		String_t* L_9 = (L_6)->GetAt(static_cast<il2cpp_array_size_t>(L_8));
		V_5 = L_9;
		String_t* L_10 = V_5;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_11;
		L_11 = OrbitalCamera_FindGameObjectByName_m2A404C82E24BC33BA073574D123D743F4534CC50(__this, L_10, NULL);
		V_6 = L_11;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_12 = V_6;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_13;
		L_13 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_12, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_13)
		{
			goto IL_0087;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_14 = V_6;
		NullCheck(L_14);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_15;
		L_15 = GameObject_get_transform_m0BC10ADFA1632166AE5544BDF9038A2650C2AE56(L_14, NULL);
		NullCheck(L_15);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_16;
		L_16 = Transform_get_parent_m65354E28A4C94EC00EBCF03532F7B0718380791E(L_15, NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_17;
		L_17 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_16, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_17)
		{
			goto IL_005b;
		}
	}
	{
		V_2 = (bool)1;
		goto IL_0094;
	}

IL_005b:
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_18 = V_6;
		NullCheck(L_18);
		RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* L_19;
		L_19 = GameObject_GetComponentsInChildren_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mA2746088DB45856FD76C725AB403CEF5A8997734(L_18, GameObject_GetComponentsInChildren_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mA2746088DB45856FD76C725AB403CEF5A8997734_RuntimeMethod_var);
		V_7 = L_19;
		V_8 = 0;
		goto IL_007f;
	}

IL_0069:
	{
		RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* L_20 = V_7;
		int32_t L_21 = V_8;
		NullCheck(L_20);
		int32_t L_22 = L_21;
		Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* L_23 = (L_20)->GetAt(static_cast<il2cpp_array_size_t>(L_22));
		V_9 = L_23;
		HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D* L_24 = V_1;
		Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* L_25 = V_9;
		NullCheck(L_24);
		bool L_26;
		L_26 = HashSet_1_Add_m899003BF81A20D05B1D48080CB373E701BA64652(L_24, L_25, HashSet_1_Add_m899003BF81A20D05B1D48080CB373E701BA64652_RuntimeMethod_var);
		int32_t L_27 = V_8;
		V_8 = ((int32_t)il2cpp_codegen_add(L_27, 1));
	}

IL_007f:
	{
		int32_t L_28 = V_8;
		RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* L_29 = V_7;
		NullCheck(L_29);
		if ((((int32_t)L_28) < ((int32_t)((int32_t)(((RuntimeArray*)L_29)->max_length)))))
		{
			goto IL_0069;
		}
	}

IL_0087:
	{
		int32_t L_30 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add(L_30, 1));
	}

IL_008d:
	{
		int32_t L_31 = V_4;
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_32 = V_3;
		NullCheck(L_32);
		if ((((int32_t)L_31) < ((int32_t)((int32_t)(((RuntimeArray*)L_32)->max_length)))))
		{
			goto IL_0029;
		}
	}

IL_0094:
	{
		RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* L_33 = V_0;
		V_7 = L_33;
		V_4 = 0;
		goto IL_00d1;
	}

IL_009c:
	{
		RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* L_34 = V_7;
		int32_t L_35 = V_4;
		NullCheck(L_34);
		int32_t L_36 = L_35;
		Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* L_37 = (L_34)->GetAt(static_cast<il2cpp_array_size_t>(L_36));
		V_10 = L_37;
		Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* L_38 = V_10;
		NullCheck(L_38);
		ParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1* L_39;
		L_39 = Component_GetComponent_TisParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1_mF2B700ECAEEEDBCC076CC66C9592C6DDFA0D36F9(L_38, Component_GetComponent_TisParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1_mF2B700ECAEEEDBCC076CC66C9592C6DDFA0D36F9_RuntimeMethod_var);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_40;
		L_40 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_39, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_40)
		{
			goto IL_00cb;
		}
	}
	{
		bool L_41 = V_2;
		if (L_41)
		{
			goto IL_00bf;
		}
	}
	{
		HashSet_1_tF34A155AE402650450E48C460D73603A08C7C81D* L_42 = V_1;
		Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* L_43 = V_10;
		NullCheck(L_42);
		bool L_44;
		L_44 = HashSet_1_Contains_m21126165891AF3C847A2737D98F1B9F853656B7E(L_42, L_43, HashSet_1_Contains_m21126165891AF3C847A2737D98F1B9F853656B7E_RuntimeMethod_var);
		G_B18_0 = ((int32_t)(L_44));
		goto IL_00c0;
	}

IL_00bf:
	{
		G_B18_0 = 1;
	}

IL_00c0:
	{
		V_11 = (bool)G_B18_0;
		Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* L_45 = V_10;
		bool L_46 = V_11;
		NullCheck(L_45);
		Renderer_set_enabled_m015E6D7B825528A31182F267234CC6A925F71DA8(L_45, L_46, NULL);
	}

IL_00cb:
	{
		int32_t L_47 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add(L_47, 1));
	}

IL_00d1:
	{
		int32_t L_48 = V_4;
		RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* L_49 = V_7;
		NullCheck(L_49);
		if ((((int32_t)L_48) < ((int32_t)((int32_t)(((RuntimeArray*)L_49)->max_length)))))
		{
			goto IL_009c;
		}
	}
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* OrbitalCamera_FindGameObjectByName_m2A404C82E24BC33BA073574D123D743F4534CC50 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, String_t* ___0_name, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&SceneManager_tA0EF56A88ACA4A15731AF7FDC10A869FA4C698FA_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* V_0 = NULL;
	GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* V_1 = NULL;
	int32_t V_2 = 0;
	Scene_tA1DC762B79745EB5140F054C884855B922318356 V_3;
	memset((&V_3), 0, sizeof(V_3));
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* V_4 = NULL;
	Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* V_5 = NULL;
	{
		String_t* L_0 = ___0_name;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_1;
		L_1 = GameObject_Find_m7A669B4EEC2617AB82F6E3FF007CDCD9F21DB300(L_0, NULL);
		V_0 = L_1;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_2 = V_0;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_3;
		L_3 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_2, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_3)
		{
			goto IL_0012;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_4 = V_0;
		return L_4;
	}

IL_0012:
	{
		il2cpp_codegen_runtime_class_init_inline(SceneManager_tA0EF56A88ACA4A15731AF7FDC10A869FA4C698FA_il2cpp_TypeInfo_var);
		Scene_tA1DC762B79745EB5140F054C884855B922318356 L_5;
		L_5 = SceneManager_GetActiveScene_m0B320EC4302F51A71495D1CCD1A0FF9C2ED1FDC8(NULL);
		V_3 = L_5;
		GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_6;
		L_6 = Scene_GetRootGameObjects_mFDE0BF9EA926F30EC9AE71F33E0AE4D5D4EC5C5B((&V_3), NULL);
		V_1 = L_6;
		V_2 = 0;
		goto IL_004f;
	}

IL_0024:
	{
		GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_7 = V_1;
		int32_t L_8 = V_2;
		NullCheck(L_7);
		int32_t L_9 = L_8;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_10 = (L_7)->GetAt(static_cast<il2cpp_array_size_t>(L_9));
		V_4 = L_10;
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_11 = V_4;
		NullCheck(L_11);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_12;
		L_12 = GameObject_get_transform_m0BC10ADFA1632166AE5544BDF9038A2650C2AE56(L_11, NULL);
		String_t* L_13 = ___0_name;
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_14;
		L_14 = OrbitalCamera_FindRecursive_m60A3567100CA4F64F45BC40735CC4A013F16DC6E(__this, L_12, L_13, NULL);
		V_5 = L_14;
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_15 = V_5;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_16;
		L_16 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_15, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_16)
		{
			goto IL_004b;
		}
	}
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_17 = V_5;
		NullCheck(L_17);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_18;
		L_18 = Component_get_gameObject_m57AEFBB14DB39EC476F740BA000E170355DE691B(L_17, NULL);
		return L_18;
	}

IL_004b:
	{
		int32_t L_19 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add(L_19, 1));
	}

IL_004f:
	{
		int32_t L_20 = V_2;
		GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_21 = V_1;
		NullCheck(L_21);
		if ((((int32_t)L_20) < ((int32_t)((int32_t)(((RuntimeArray*)L_21)->max_length)))))
		{
			goto IL_0024;
		}
	}
	{
		return (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* OrbitalCamera_FindRecursive_m60A3567100CA4F64F45BC40735CC4A013F16DC6E (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* ___0_parent, String_t* ___1_name, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* V_0 = NULL;
	Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* V_1 = NULL;
	Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* V_2 = NULL;
	Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* V_3 = NULL;
	RuntimeObject* V_4 = NULL;
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_0 = ___0_parent;
		NullCheck(L_0);
		String_t* L_1;
		L_1 = Object_get_name_mAC2F6B897CF1303BA4249B4CB55271AFACBB6392(L_0, NULL);
		String_t* L_2 = ___1_name;
		bool L_3;
		L_3 = String_op_Equality_m030E1B219352228970A076136E455C4E568C02C1(L_1, L_2, NULL);
		if (!L_3)
		{
			goto IL_0010;
		}
	}
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_4 = ___0_parent;
		return L_4;
	}

IL_0010:
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_5 = ___0_parent;
		NullCheck(L_5);
		RuntimeObject* L_6;
		L_6 = Transform_GetEnumerator_mA7E1C882ACA0C33E284711CD09971DEA3FFEF404(L_5, NULL);
		V_0 = L_6;
	}
	{
		auto __finallyBlock = il2cpp::utils::Finally([&]
		{

FINALLY_0045:
			{
				{
					RuntimeObject* L_7 = V_0;
					V_4 = ((RuntimeObject*)IsInst((RuntimeObject*)L_7, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var));
					RuntimeObject* L_8 = V_4;
					if (!L_8)
					{
						goto IL_0058;
					}
				}
				{
					RuntimeObject* L_9 = V_4;
					NullCheck(L_9);
					InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_9);
				}

IL_0058:
				{
					return;
				}
			}
		});
		try
		{
			{
				goto IL_003b_1;
			}

IL_0019_1:
			{
				RuntimeObject* L_10 = V_0;
				NullCheck(L_10);
				RuntimeObject* L_11;
				L_11 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(1, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_10);
				V_1 = ((Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1*)CastclassClass((RuntimeObject*)L_11, Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1_il2cpp_TypeInfo_var));
				Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_12 = V_1;
				String_t* L_13 = ___1_name;
				Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_14;
				L_14 = OrbitalCamera_FindRecursive_m60A3567100CA4F64F45BC40735CC4A013F16DC6E(__this, L_12, L_13, NULL);
				V_2 = L_14;
				Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_15 = V_2;
				il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
				bool L_16;
				L_16 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_15, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
				if (!L_16)
				{
					goto IL_003b_1;
				}
			}
			{
				Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_17 = V_2;
				V_3 = L_17;
				goto IL_005b;
			}

IL_003b_1:
			{
				RuntimeObject* L_18 = V_0;
				NullCheck(L_18);
				bool L_19;
				L_19 = InterfaceFuncInvoker0< bool >::Invoke(0, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_18);
				if (L_19)
				{
					goto IL_0019_1;
				}
			}
			{
				goto IL_0059;
			}
		}
		catch(Il2CppExceptionWrapper& e)
		{
			__finallyBlock.StoreException(e.ex);
		}
	}

IL_0059:
	{
		return (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1*)NULL;
	}

IL_005b:
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_20 = V_3;
		return L_20;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_OnGUI_m2B18308772BAEDFEDEA74A69798917473E01A4C5 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GUI_tA9CDB3D69DB13D51AD83ABDB587EF95947EC2D2A_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Single_t4530F2FF86FCB0DC29F35385CA1BD21BE294761C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral3E989E783212FAC625EA2248403E9D8D78457010);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral89D0BAC9D68CB85BDAC65115F45B67760D117A07);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral996B036BC167299A08B2E9DB294082C30359A9C8);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralACE429A0AAEC7F24F9529A638C16C3AA0080E21F);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralEB508B30BB5FB6A4B6F599EB52273776204BF55B);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	Resolution_tDF215F567EEFFD07B9A8FB7CEACC08EA6B8B9525 V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_0;
		L_0 = Color_get_green_mEB001F2CD8C68C6BBAEF9101990B779D3AA2A6EF_inline(NULL);
		il2cpp_codegen_runtime_class_init_inline(GUI_tA9CDB3D69DB13D51AD83ABDB587EF95947EC2D2A_il2cpp_TypeInfo_var);
		GUI_set_color_mA44927D3F2ACB1E228815F10042A6F62B7645648(L_0, NULL);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_1;
		L_1 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		NullCheck(L_1);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2;
		L_2 = Transform_get_position_m69CD5FA214FDAE7BB701552943674846C220FDE1(L_1, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_3 = L_2;
		RuntimeObject* L_4 = Box(Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var, &L_3);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_5;
		L_5 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		NullCheck(L_5);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_6;
		L_6 = Transform_get_eulerAngles_mCAAF48EFCF628F1ED91C2FFE75A4FD19C039DD6A(L_5, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_7 = L_6;
		RuntimeObject* L_8 = Box(Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var, &L_7);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_9 = __this->____targetPosition;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_10 = L_9;
		RuntimeObject* L_11 = Box(Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var, &L_10);
		String_t* L_12;
		L_12 = String_Format_mA0534D6E2AE4D67A6BD8D45B3321323930EB930C(_stringLiteral3E989E783212FAC625EA2248403E9D8D78457010, L_4, L_8, L_11, NULL);
		Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* L_13 = (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3*)(&__this->____lastBounds);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_14;
		L_14 = Bounds_get_center_m5B05F81CB835EB6DD8628FDA24B638F477984DC3_inline(L_13, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_15 = L_14;
		RuntimeObject* L_16 = Box(Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var, &L_15);
		Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* L_17 = (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3*)(&__this->____lastBounds);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_18;
		L_18 = Bounds_get_size_m0699A53A55A78B3201D7270D6F338DFA91B6FAD4_inline(L_17, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_19 = L_18;
		RuntimeObject* L_20 = Box(Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var, &L_19);
		String_t* L_21;
		L_21 = String_Format_mFB7DA489BD99F4670881FF50EC017BFB0A5C0987(_stringLiteralEB508B30BB5FB6A4B6F599EB52273776204BF55B, L_16, L_20, NULL);
		ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918* L_22 = (ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918*)(ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918*)SZArrayNew(ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918_il2cpp_TypeInfo_var, (uint32_t)4);
		ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918* L_23 = L_22;
		int32_t L_24;
		L_24 = Screen_get_width_mF608FF3252213E7EFA1F0D2F744C28110E9E5AC9(NULL);
		int32_t L_25 = L_24;
		RuntimeObject* L_26 = Box(Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_il2cpp_TypeInfo_var, &L_25);
		NullCheck(L_23);
		ArrayElementTypeCheck (L_23, L_26);
		(L_23)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject*)L_26);
		ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918* L_27 = L_23;
		int32_t L_28;
		L_28 = Screen_get_height_m01A3102DE71EE1FBEA51D09D6B0261CF864FE8F9(NULL);
		int32_t L_29 = L_28;
		RuntimeObject* L_30 = Box(Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_il2cpp_TypeInfo_var, &L_29);
		NullCheck(L_27);
		ArrayElementTypeCheck (L_27, L_30);
		(L_27)->SetAt(static_cast<il2cpp_array_size_t>(1), (RuntimeObject*)L_30);
		ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918* L_31 = L_27;
		Resolution_tDF215F567EEFFD07B9A8FB7CEACC08EA6B8B9525 L_32;
		L_32 = Screen_get_currentResolution_m8FE4C43E4F6EF28E0B85EB94B6C69D1EC5687CCD(NULL);
		V_1 = L_32;
		int32_t L_33;
		L_33 = Resolution_get_width_mC55423852D453EEC0B66E692AA3E7F5DE772FB24((&V_1), NULL);
		int32_t L_34 = L_33;
		RuntimeObject* L_35 = Box(Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_il2cpp_TypeInfo_var, &L_34);
		NullCheck(L_31);
		ArrayElementTypeCheck (L_31, L_35);
		(L_31)->SetAt(static_cast<il2cpp_array_size_t>(2), (RuntimeObject*)L_35);
		ObjectU5BU5D_t8061030B0A12A55D5AD8652A20C922FE99450918* L_36 = L_31;
		Resolution_tDF215F567EEFFD07B9A8FB7CEACC08EA6B8B9525 L_37;
		L_37 = Screen_get_currentResolution_m8FE4C43E4F6EF28E0B85EB94B6C69D1EC5687CCD(NULL);
		V_1 = L_37;
		int32_t L_38;
		L_38 = Resolution_get_height_mD46754FA3634CA816BE91D1610D1FEBDECCEBE5B((&V_1), NULL);
		int32_t L_39 = L_38;
		RuntimeObject* L_40 = Box(Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_il2cpp_TypeInfo_var, &L_39);
		NullCheck(L_36);
		ArrayElementTypeCheck (L_36, L_40);
		(L_36)->SetAt(static_cast<il2cpp_array_size_t>(3), (RuntimeObject*)L_40);
		String_t* L_41;
		L_41 = String_Format_m918500C1EFB475181349A79989BB79BB36102894(_stringLiteral996B036BC167299A08B2E9DB294082C30359A9C8, L_36, NULL);
		Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* L_42;
		L_42 = Camera_get_main_m52C992F18E05355ABB9EEB64A4BF2215E12762DF(NULL);
		NullCheck(L_42);
		float L_43;
		L_43 = Camera_get_aspect_m48BF8820EA2D55BE0D154BC5546819FB65BE257D(L_42, NULL);
		float L_44 = L_43;
		RuntimeObject* L_45 = Box(Single_t4530F2FF86FCB0DC29F35385CA1BD21BE294761C_il2cpp_TypeInfo_var, &L_44);
		int32_t L_46;
		L_46 = Screen_get_width_mF608FF3252213E7EFA1F0D2F744C28110E9E5AC9(NULL);
		int32_t L_47;
		L_47 = Screen_get_height_m01A3102DE71EE1FBEA51D09D6B0261CF864FE8F9(NULL);
		float L_48 = ((float)(((float)L_46)/((float)L_47)));
		RuntimeObject* L_49 = Box(Single_t4530F2FF86FCB0DC29F35385CA1BD21BE294761C_il2cpp_TypeInfo_var, &L_48);
		String_t* L_50;
		L_50 = String_Format_mFB7DA489BD99F4670881FF50EC017BFB0A5C0987(_stringLiteralACE429A0AAEC7F24F9529A638C16C3AA0080E21F, L_45, L_49, NULL);
		String_t* L_51;
		L_51 = String_Concat_m093934F71A9B351911EE46311674ED463B180006(L_12, L_21, L_41, L_50, NULL);
		V_0 = L_51;
		Rect_tA04E0F8A1830E767F40FB27ECD8D309303571F0D L_52;
		memset((&L_52), 0, sizeof(L_52));
		Rect__ctor_m18C3033D135097BEE424AAA68D91C706D2647F23_inline((&L_52), (10.0f), (10.0f), (500.0f), (150.0f), NULL);
		String_t* L_53 = V_0;
		GUI_Label_m4A951E57C7DCCF95A0306240144CA2713F546526(L_52, L_53, NULL);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_54 = __this->____highlightedObject;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_55;
		L_55 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_54, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_55)
		{
			goto IL_0158;
		}
	}
	{
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_56;
		L_56 = Color_get_cyan_m182A29E7475C0A98ACC03E1CF5252BAB83F0BA31_inline(NULL);
		il2cpp_codegen_runtime_class_init_inline(GUI_tA9CDB3D69DB13D51AD83ABDB587EF95947EC2D2A_il2cpp_TypeInfo_var);
		GUI_set_color_mA44927D3F2ACB1E228815F10042A6F62B7645648(L_56, NULL);
		Rect_tA04E0F8A1830E767F40FB27ECD8D309303571F0D L_57;
		memset((&L_57), 0, sizeof(L_57));
		Rect__ctor_m18C3033D135097BEE424AAA68D91C706D2647F23_inline((&L_57), (10.0f), (150.0f), (500.0f), (30.0f), NULL);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_58 = __this->____highlightedObject;
		NullCheck(L_58);
		String_t* L_59;
		L_59 = Object_get_name_mAC2F6B897CF1303BA4249B4CB55271AFACBB6392(L_58, NULL);
		String_t* L_60;
		L_60 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteral89D0BAC9D68CB85BDAC65115F45B67760D117A07, L_59, NULL);
		GUI_Label_m4A951E57C7DCCF95A0306240144CA2713F546526(L_57, L_60, NULL);
	}

IL_0158:
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_FrameAll_m2447ACDD9F3F04FC9B3EE6A55987AB693DC96CB6 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, const RuntimeMethod* method) 
{
	{
		Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 L_0;
		L_0 = OrbitalCamera_CalculateBounds_m196814D27B1ABA374964A41F4C4FF946CA2E3E56(__this, (Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1*)NULL, NULL);
		OrbitalCamera_FrameBounds_m897E753DA62BA59A9E0DA6D81C912FE285F58930(__this, L_0, NULL);
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_FrameObject_m4F80B03857670CDAA5790C33318C434551757518 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* ___0_obj, const RuntimeMethod* method) 
{
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_0 = ___0_obj;
		NullCheck(L_0);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_1;
		L_1 = GameObject_get_transform_m0BC10ADFA1632166AE5544BDF9038A2650C2AE56(L_0, NULL);
		Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 L_2;
		L_2 = OrbitalCamera_CalculateBounds_m196814D27B1ABA374964A41F4C4FF946CA2E3E56(__this, L_1, NULL);
		OrbitalCamera_FrameBounds_m897E753DA62BA59A9E0DA6D81C912FE285F58930(__this, L_2, NULL);
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 OrbitalCamera_CalculateBounds_m196814D27B1ABA374964A41F4C4FF946CA2E3E56 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* ___0_root, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponent_TisParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1_mF2B700ECAEEEDBCC076CC66C9592C6DDFA0D36F9_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Component_GetComponentsInChildren_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_m35AC34F858BD2F34770712CD020AA0518D9409C7_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_FindObjectsOfType_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mC338358A9594B72942DC6AE839BC11C37E5CF8AD_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 V_0;
	memset((&V_0), 0, sizeof(V_0));
	bool V_1 = false;
	RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* V_2 = NULL;
	RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* V_3 = NULL;
	int32_t V_4 = 0;
	Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* V_5 = NULL;
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0;
		L_0 = Vector3_get_zero_m0C1249C3F25B1C70EAD3CC8B31259975A457AE39_inline(NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1;
		L_1 = Vector3_get_zero_m0C1249C3F25B1C70EAD3CC8B31259975A457AE39_inline(NULL);
		Bounds__ctor_mAF7B238B9FBF90C495E5D7951760085A93119C5A_inline((&V_0), L_0, L_1, NULL);
		V_1 = (bool)1;
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_2 = ___0_root;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_3;
		L_3 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_2, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_3)
		{
			goto IL_0025;
		}
	}
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_4 = ___0_root;
		NullCheck(L_4);
		RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* L_5;
		L_5 = Component_GetComponentsInChildren_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_m35AC34F858BD2F34770712CD020AA0518D9409C7(L_4, Component_GetComponentsInChildren_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_m35AC34F858BD2F34770712CD020AA0518D9409C7_RuntimeMethod_var);
		V_2 = L_5;
		goto IL_002b;
	}

IL_0025:
	{
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* L_6;
		L_6 = Object_FindObjectsOfType_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mC338358A9594B72942DC6AE839BC11C37E5CF8AD(Object_FindObjectsOfType_TisRenderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF_mC338358A9594B72942DC6AE839BC11C37E5CF8AD_RuntimeMethod_var);
		V_2 = L_6;
	}

IL_002b:
	{
		RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* L_7 = V_2;
		V_3 = L_7;
		V_4 = 0;
		goto IL_006a;
	}

IL_0032:
	{
		RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* L_8 = V_3;
		int32_t L_9 = V_4;
		NullCheck(L_8);
		int32_t L_10 = L_9;
		Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* L_11 = (L_8)->GetAt(static_cast<il2cpp_array_size_t>(L_10));
		V_5 = L_11;
		Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* L_12 = V_5;
		NullCheck(L_12);
		ParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1* L_13;
		L_13 = Component_GetComponent_TisParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1_mF2B700ECAEEEDBCC076CC66C9592C6DDFA0D36F9(L_12, Component_GetComponent_TisParticleSystem_tB19986EE308BD63D36FB6025EEEAFBEDB97C67C1_mF2B700ECAEEEDBCC076CC66C9592C6DDFA0D36F9_RuntimeMethod_var);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_14;
		L_14 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_13, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_14)
		{
			goto IL_0064;
		}
	}
	{
		bool L_15 = V_1;
		if (!L_15)
		{
			goto IL_0056;
		}
	}
	{
		Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* L_16 = V_5;
		NullCheck(L_16);
		Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 L_17;
		L_17 = Renderer_get_bounds_m390CF334730C3C34E45CE59F1D08C3B9F3109C7C(L_16, NULL);
		V_0 = L_17;
		V_1 = (bool)0;
		goto IL_0064;
	}

IL_0056:
	{
		Renderer_t320575F223BCB177A982E5DDB5DB19FAA89E7FBF* L_18 = V_5;
		NullCheck(L_18);
		Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 L_19;
		L_19 = Renderer_get_bounds_m390CF334730C3C34E45CE59F1D08C3B9F3109C7C(L_18, NULL);
		Bounds_Encapsulate_m7C70C382B9380A8C962074C78E189B53CE8F7A22_inline((&V_0), L_19, NULL);
	}

IL_0064:
	{
		int32_t L_20 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add(L_20, 1));
	}

IL_006a:
	{
		int32_t L_21 = V_4;
		RendererU5BU5D_t32FDD782F67917B2291EA4FF242719877440A02A* L_22 = V_3;
		NullCheck(L_22);
		if ((((int32_t)L_21) < ((int32_t)((int32_t)(((RuntimeArray*)L_22)->max_length)))))
		{
			goto IL_0032;
		}
	}
	{
		bool L_23 = V_1;
		if (!L_23)
		{
			goto IL_00a5;
		}
	}
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_24 = ___0_root;
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_25;
		L_25 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_24, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_25)
		{
			goto IL_00a5;
		}
	}
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_26 = ___0_root;
		NullCheck(L_26);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_27;
		L_27 = Transform_get_position_m69CD5FA214FDAE7BB701552943674846C220FDE1(L_26, NULL);
		Bounds_set_center_m891869DD5B1BEEE2D17907BBFB7EB79AAE44884B_inline((&V_0), L_27, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_28;
		memset((&L_28), 0, sizeof(L_28));
		Vector3__ctor_m376936E6B999EF1ECBE57D990A386303E2283DE0_inline((&L_28), (1.0f), (2.0f), (1.0f), NULL);
		Bounds_set_size_m950CFB68CDD1BF409E770509A38B958E1AE68128_inline((&V_0), L_28, NULL);
	}

IL_00a5:
	{
		Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 L_29 = V_0;
		__this->____lastBounds = L_29;
		Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 L_30 = V_0;
		return L_30;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera_FrameBounds_m897E753DA62BA59A9E0DA6D81C912FE285F58930 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 ___0_b, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	float V_0 = 0.0f;
	float V_1 = 0.0f;
	float V_2 = 0.0f;
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_3;
	memset((&V_3), 0, sizeof(V_3));
	float G_B5_0 = 0.0f;
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0;
		L_0 = Bounds_get_size_m0699A53A55A78B3201D7270D6F338DFA91B6FAD4_inline((&___0_b), NULL);
		float L_1 = L_0.___x;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2;
		L_2 = Bounds_get_size_m0699A53A55A78B3201D7270D6F338DFA91B6FAD4_inline((&___0_b), NULL);
		float L_3 = L_2.___y;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_4;
		L_4 = Bounds_get_size_m0699A53A55A78B3201D7270D6F338DFA91B6FAD4_inline((&___0_b), NULL);
		float L_5 = L_4.___z;
		float L_6;
		L_6 = Mathf_Max_mF5379E63D2BBAC76D090748695D833934F8AD051_inline(L_3, L_5, NULL);
		float L_7;
		L_7 = Mathf_Max_mF5379E63D2BBAC76D090748695D833934F8AD051_inline(L_1, L_6, NULL);
		V_0 = L_7;
		float L_8 = V_0;
		if ((!(((float)L_8) < ((float)(0.100000001f)))))
		{
			goto IL_003d;
		}
	}
	{
		V_0 = (1.0f);
	}

IL_003d:
	{
		Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* L_9;
		L_9 = Camera_get_main_m52C992F18E05355ABB9EEB64A4BF2215E12762DF(NULL);
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		bool L_10;
		L_10 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_9, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (L_10)
		{
			goto IL_0051;
		}
	}
	{
		G_B5_0 = (60.0f);
		goto IL_005b;
	}

IL_0051:
	{
		Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* L_11;
		L_11 = Camera_get_main_m52C992F18E05355ABB9EEB64A4BF2215E12762DF(NULL);
		NullCheck(L_11);
		float L_12;
		L_12 = Camera_get_fieldOfView_m9A93F17BBF89F496AE231C21817AFD1C1E833FBB(L_11, NULL);
		G_B5_0 = L_12;
	}

IL_005b:
	{
		V_1 = G_B5_0;
		float L_13 = V_0;
		float L_14 = V_1;
		float L_15;
		L_15 = sinf(((float)(((float)il2cpp_codegen_multiply(L_14, (0.0174532924f)))/(2.0f))));
		V_2 = ((float)(L_13/L_15));
		__this->____rotationY = (180.0f);
		__this->____rotationX = (0.0f);
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_16;
		L_16 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		float L_17 = __this->____rotationX;
		float L_18 = __this->____rotationY;
		Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 L_19;
		L_19 = Quaternion_Euler_m9262AB29E3E9CE94EF71051F38A28E82AEC73F90_inline(L_17, L_18, (0.0f), NULL);
		NullCheck(L_16);
		Transform_set_rotation_m61340DE74726CF0F9946743A727C4D444397331D(L_16, L_19, NULL);
		float L_20 = V_2;
		V_2 = ((float)il2cpp_codegen_multiply(L_20, (1.25f)));
		float L_21 = V_2;
		if ((!(((float)L_21) < ((float)(2.0f)))))
		{
			goto IL_00be;
		}
	}
	{
		V_2 = (2.0f);
	}

IL_00be:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_22;
		L_22 = Bounds_get_center_m5B05F81CB835EB6DD8628FDA24B638F477984DC3_inline((&___0_b), NULL);
		V_3 = L_22;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_23;
		L_23 = Bounds_get_size_m0699A53A55A78B3201D7270D6F338DFA91B6FAD4_inline((&___0_b), NULL);
		float L_24 = L_23.___y;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_25;
		L_25 = Bounds_get_size_m0699A53A55A78B3201D7270D6F338DFA91B6FAD4_inline((&___0_b), NULL);
		float L_26 = L_25.___x;
		if ((!(((float)L_24) > ((float)L_26))))
		{
			goto IL_010c;
		}
	}
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_27;
		L_27 = Bounds_get_size_m0699A53A55A78B3201D7270D6F338DFA91B6FAD4_inline((&___0_b), NULL);
		float L_28 = L_27.___y;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_29;
		L_29 = Bounds_get_size_m0699A53A55A78B3201D7270D6F338DFA91B6FAD4_inline((&___0_b), NULL);
		float L_30 = L_29.___z;
		if ((!(((float)L_28) > ((float)L_30))))
		{
			goto IL_010c;
		}
	}
	{
		float* L_31 = (float*)(&(&V_3)->___y);
		float* L_32 = L_31;
		float L_33 = *((float*)L_32);
		float L_34 = V_0;
		*((float*)L_32) = (float)((float)il2cpp_codegen_add(L_33, ((float)il2cpp_codegen_multiply(L_34, (0.100000001f)))));
	}

IL_010c:
	{
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_35;
		L_35 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_36 = V_3;
		Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_37;
		L_37 = Component_get_transform_m2919A1D81931E6932C7F06D4C2F0AB8DDA9A5371(__this, NULL);
		NullCheck(L_37);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_38;
		L_38 = Transform_get_forward_mFCFACF7165FDAB21E80E384C494DF278386CEE2F(L_37, NULL);
		float L_39 = V_2;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_40;
		L_40 = Vector3_op_Multiply_m87BA7C578F96C8E49BB07088DAAC4649F83B0353_inline(L_38, L_39, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_41;
		L_41 = Vector3_op_Subtraction_mE42023FF80067CB44A1D4A27EB7CF2B24CABB828_inline(L_36, L_40, NULL);
		NullCheck(L_35);
		Transform_set_position_mA1A817124BB41B685043DED2A9BA48CDF37C4156(L_35, L_41, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_42 = V_3;
		__this->____targetPosition = L_42;
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void OrbitalCamera__ctor_mA2473AFB5A78E5B728DB2D709A50DEC631ECDEF6 (OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* __this, const RuntimeMethod* method) 
{
	{
		__this->___moveSpeed = (10.0f);
		__this->___turboSpeed = (20.0f);
		__this->___lookSpeed = (2.0f);
		__this->___panSpeed = (10.0f);
		__this->____forcedAspect = (-1.0f);
		MonoBehaviour__ctor_m592DB0105CA0BC97AA1C5F4AD27B12D68A3B7C1E(__this, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ViewerBootstrap_Init_m4D31DEDC930F488D4BE1DE8B6C1F800E287D87F1 (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Application_tDB03BE91CDF0ACA614A5E0B67CFB77C44EB19B21_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral3B2C1C62D4D1C2A0C8A9AC42DB00D33C654F9AD0);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralF881296E3AFAAF85C9495BBB45FF8BB231AB53B5);
		s_Il2CppMethodInitialized = true;
	}
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* V_0 = NULL;
	int32_t V_1 = 0;
	String_t* V_2 = NULL;
	{
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_0;
		L_0 = Environment_GetCommandLineArgs_mD29CFA1CD3C84F9BD91152E70302E908114A831D(NULL);
		V_0 = L_0;
		V_1 = 1;
		goto IL_0057;
	}

IL_000a:
	{
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_1 = V_0;
		int32_t L_2 = V_1;
		NullCheck(L_1);
		int32_t L_3 = L_2;
		String_t* L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		NullCheck(L_4);
		String_t* L_5;
		L_5 = String_Trim_mCD6D8C6D4CFD15225D12DB7D3E0544CA80FB8DA5(L_4, NULL);
		CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB* L_6 = (CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB*)(CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB*)SZArrayNew(CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB_il2cpp_TypeInfo_var, (uint32_t)2);
		CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB* L_7 = L_6;
		NullCheck(L_7);
		(L_7)->SetAt(static_cast<il2cpp_array_size_t>(0), (Il2CppChar)((int32_t)34));
		CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB* L_8 = L_7;
		NullCheck(L_8);
		(L_8)->SetAt(static_cast<il2cpp_array_size_t>(1), (Il2CppChar)((int32_t)39));
		NullCheck(L_5);
		String_t* L_9;
		L_9 = String_Trim_m81BD35659E6F89DDD56816975E6E05390D023FE5(L_5, L_8, NULL);
		V_2 = L_9;
		String_t* L_10 = V_2;
		NullCheck(L_10);
		bool L_11;
		L_11 = String_StartsWith_mF75DBA1EB709811E711B44E26FF919C88A8E65C0(L_10, _stringLiteral3B2C1C62D4D1C2A0C8A9AC42DB00D33C654F9AD0, NULL);
		if (L_11)
		{
			goto IL_0053;
		}
	}
	{
		String_t* L_12 = V_2;
		NullCheck(L_12);
		bool L_13;
		L_13 = String_EndsWith_m5E5D307CA6AEB7C08CE782B4693B19D07ADC9075(L_12, _stringLiteralF881296E3AFAAF85C9495BBB45FF8BB231AB53B5, 5, NULL);
		if (L_13)
		{
			goto IL_004b;
		}
	}
	{
		String_t* L_14 = V_2;
		bool L_15;
		L_15 = File_Exists_m95E329ABBE3EAD6750FE1989BBA6884457136D4A(L_14, NULL);
		if (!L_15)
		{
			goto IL_0053;
		}
	}

IL_004b:
	{
		String_t* L_16 = V_2;
		il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
		((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___TargetFile = L_16;
		Il2CppCodeGenWriteBarrier((void**)(&((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___TargetFile), (void*)L_16);
		goto IL_005d;
	}

IL_0053:
	{
		int32_t L_17 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add(L_17, 1));
	}

IL_0057:
	{
		int32_t L_18 = V_1;
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_19 = V_0;
		NullCheck(L_19);
		if ((((int32_t)L_18) < ((int32_t)((int32_t)(((RuntimeArray*)L_19)->max_length)))))
		{
			goto IL_000a;
		}
	}

IL_005d:
	{
		il2cpp_codegen_runtime_class_init_inline(Application_tDB03BE91CDF0ACA614A5E0B67CFB77C44EB19B21_il2cpp_TypeInfo_var);
		Application_set_runInBackground_m4CD0DF59011994C4DB76D7A2FE7A02BC07B4AF67((bool)1, NULL);
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ViewerBootstrap_Start_m745431CAB58769971197B3F0CB743C17CE99CC6B (ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_AddComponent_TisCamera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184_m0933BC50E883CDEF6FA83FE190DA37CCB2802142_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_AddComponent_TisIPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2_mADB27F1B8EE9E8A084D0EF3AA45B1A1793AEE7DE_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_AddComponent_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_m066777E29FC6E113E469A7A0F2AF7ADD38C42B46_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GameObject_t76FEDD663AB33C991A9C9A23129337651094216F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralBB0462B9CEB66F4161BC2D92867CF94CF87B1B78);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralDA7E8B88100AA34B19982A11B0554BAF879AFF81);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralE302AA9BECF9F1CB69CF2A3E5B33E0716BEA97F6);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
		String_t* L_0 = ((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___TargetFile;
		bool L_1;
		L_1 = String_IsNullOrEmpty_mEA9E3FB005AC28FE02E69FCF95A7B8456192B478(L_0, NULL);
		if (L_1)
		{
			goto IL_0025;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
		String_t* L_2 = ((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___TargetFile;
		bool L_3;
		L_3 = File_Exists_m95E329ABBE3EAD6750FE1989BBA6884457136D4A(L_2, NULL);
		if (!L_3)
		{
			goto IL_0025;
		}
	}
	{
		il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
		String_t* L_4 = ((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___TargetFile;
		ViewerBootstrap_LoadModel_mD2EA0E216CFAE1D70E5334ACC1B48895BACE1761(__this, L_4, NULL);
		goto IL_0039;
	}

IL_0025:
	{
		il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
		String_t* L_5 = ((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___TargetFile;
		String_t* L_6;
		L_6 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteralBB0462B9CEB66F4161BC2D92867CF94CF87B1B78, L_5, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_Log_m87A9A3C761FF5C43ED8A53B16190A53D08F818BB(L_6, NULL);
	}

IL_0039:
	{
		il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_7;
		L_7 = Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E(Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E_RuntimeMethod_var);
		bool L_8;
		L_8 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_7, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
		if (!L_8)
		{
			goto IL_006f;
		}
	}
	{
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_9 = (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)il2cpp_codegen_object_new(GameObject_t76FEDD663AB33C991A9C9A23129337651094216F_il2cpp_TypeInfo_var);
		GameObject__ctor_m37D512B05D292F954792225E6C6EEE95293A9B88(L_9, _stringLiteralDA7E8B88100AA34B19982A11B0554BAF879AFF81, NULL);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_10 = L_9;
		NullCheck(L_10);
		Camera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184* L_11;
		L_11 = GameObject_AddComponent_TisCamera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184_m0933BC50E883CDEF6FA83FE190DA37CCB2802142(L_10, GameObject_AddComponent_TisCamera_tA92CC927D7439999BC82DBEDC0AA45B470F9E184_m0933BC50E883CDEF6FA83FE190DA37CCB2802142_RuntimeMethod_var);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_12 = L_10;
		NullCheck(L_12);
		OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_13;
		L_13 = GameObject_AddComponent_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_m066777E29FC6E113E469A7A0F2AF7ADD38C42B46(L_12, GameObject_AddComponent_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_m066777E29FC6E113E469A7A0F2AF7ADD38C42B46_RuntimeMethod_var);
		GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_14 = L_12;
		NullCheck(L_14);
		IPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2* L_15;
		L_15 = GameObject_AddComponent_TisIPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2_mADB27F1B8EE9E8A084D0EF3AA45B1A1793AEE7DE(L_14, GameObject_AddComponent_TisIPCServer_tFCD58FCC9FE51900E9FF1DC04475F0DC89E5EEF2_mADB27F1B8EE9E8A084D0EF3AA45B1A1793AEE7DE_RuntimeMethod_var);
		NullCheck(L_14);
		GameObject_set_tag_m0A41528AFD8C83E1CEC5D769921159897CDD2B24(L_14, _stringLiteralE302AA9BECF9F1CB69CF2A3E5B33E0716BEA97F6, NULL);
	}

IL_006f:
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ViewerBootstrap_LoadModel_mD2EA0E216CFAE1D70E5334ACC1B48895BACE1761 (ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628* __this, String_t* ___0_path, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&AssetBundle_LoadAllAssets_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m61BD69AF333A9E321F6077CE3F20ED4A3AB26B58_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&AssetBundle_LoadAsset_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m0216591F4BA27A483AA2C83EFDABCEA80B214063_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_Instantiate_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m10D87C6E0708CA912BBB02555BF7D0FBC5D7A2B3_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral2F9062308C9D1A452D91CF6B41E9C71173E5387C);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral4FEC49A2578715294749EE7FE4725044FEF11BC4);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral8812C2A9B10FA45D1F9952B4E63A21B3398D5199);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral885FC71DFB0FF80A30E6013D905D1429AD60F467);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral9229404B023B828641F8C179E7652573A8F786C0);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral9740BCC647899880C38FB0778880BB4E551AA428);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralA742FD2BD1D07C567492D76B8DEB157161F97B02);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralAE6A994FF924EE125695D81F3A3A57FD5A4D9011);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralBB5DC6BB4FB40AFFAB95EEB08892720B278CC79A);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralD443B486F8A7F744E0D535B6A2816ACCCCABD83B);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralF91D1CBF70F2205BAFAA61A2A6140FA588BB55E7);
		s_Il2CppMethodInitialized = true;
	}
	AssetBundle_tB38418819A49060CD738CB21541649340F082943* V_0 = NULL;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* V_1 = NULL;
	GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* V_2 = NULL;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* V_3 = NULL;
	int32_t V_4 = 0;
	String_t* V_5 = NULL;
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* V_6 = NULL;
	int32_t V_7 = 0;
	GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* V_8 = NULL;
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* V_9 = NULL;
	int32_t V_10 = 0;
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* V_11 = NULL;
	OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* V_12 = NULL;
	String_t* V_13 = NULL;
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* V_14 = NULL;
	GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* V_15 = NULL;
	OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* V_16 = NULL;
	Exception_t* V_17 = NULL;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 1> __active_exceptions;
	{
		String_t* L_0 = ___0_path;
		String_t* L_1;
		L_1 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteral4FEC49A2578715294749EE7FE4725044FEF11BC4, L_0, NULL);
		il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
		((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus), (void*)L_1);
		String_t* L_2 = ___0_path;
		String_t* L_3;
		L_3 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteralBB5DC6BB4FB40AFFAB95EEB08892720B278CC79A, L_2, NULL);
		il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
		Debug_Log_m87A9A3C761FF5C43ED8A53B16190A53D08F818BB(L_3, NULL);
	}
	try
	{
		{
			String_t* L_4 = ___0_path;
			AssetBundle_tB38418819A49060CD738CB21541649340F082943* L_5;
			L_5 = AssetBundle_LoadFromFile_mAFD7B0E8F0A0A88E9BB60180EBA0A99B8B3F2629(L_4, NULL);
			V_0 = L_5;
			AssetBundle_tB38418819A49060CD738CB21541649340F082943* L_6 = V_0;
			il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
			bool L_7;
			L_7 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_6, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
			if (!L_7)
			{
				goto IL_0049_1;
			}
		}
		{
			il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
			((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus = _stringLiteral9740BCC647899880C38FB0778880BB4E551AA428;
			Il2CppCodeGenWriteBarrier((void**)(&((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus), (void*)_stringLiteral9740BCC647899880C38FB0778880BB4E551AA428);
			il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
			Debug_LogError_mB00B2B4468EF3CAF041B038D840820FB84C924B2(_stringLiteral9740BCC647899880C38FB0778880BB4E551AA428, NULL);
			goto IL_027c;
		}

IL_0049_1:
		{
			il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
			((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus = _stringLiteralD443B486F8A7F744E0D535B6A2816ACCCCABD83B;
			Il2CppCodeGenWriteBarrier((void**)(&((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus), (void*)_stringLiteralD443B486F8A7F744E0D535B6A2816ACCCCABD83B);
			AssetBundle_tB38418819A49060CD738CB21541649340F082943* L_8 = V_0;
			NullCheck(L_8);
			StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_9;
			L_9 = AssetBundle_GetAllAssetNames_m44504DB9E055412F0DF2071A769A243219708CC3(L_8, NULL);
			V_1 = L_9;
			StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_10 = V_1;
			NullCheck(L_10);
			int32_t L_11 = ((int32_t)(((RuntimeArray*)L_10)->max_length));
			RuntimeObject* L_12 = Box(Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_il2cpp_TypeInfo_var, &L_11);
			String_t* L_13;
			L_13 = String_Format_mA8DBB4C2516B9723C5A41E6CB1E2FAF4BBE96DD8(_stringLiteralF91D1CBF70F2205BAFAA61A2A6140FA588BB55E7, L_12, NULL);
			il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
			Debug_Log_m87A9A3C761FF5C43ED8A53B16190A53D08F818BB(L_13, NULL);
			StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_14 = V_1;
			V_3 = L_14;
			V_4 = 0;
			goto IL_0095_1;
		}

IL_0078_1:
		{
			StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_15 = V_3;
			int32_t L_16 = V_4;
			NullCheck(L_15);
			int32_t L_17 = L_16;
			String_t* L_18 = (L_15)->GetAt(static_cast<il2cpp_array_size_t>(L_17));
			V_5 = L_18;
			String_t* L_19 = V_5;
			String_t* L_20;
			L_20 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteral2F9062308C9D1A452D91CF6B41E9C71173E5387C, L_19, NULL);
			il2cpp_codegen_runtime_class_init_inline(Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var);
			Debug_Log_m87A9A3C761FF5C43ED8A53B16190A53D08F818BB(L_20, NULL);
			int32_t L_21 = V_4;
			V_4 = ((int32_t)il2cpp_codegen_add(L_21, 1));
		}

IL_0095_1:
		{
			int32_t L_22 = V_4;
			StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_23 = V_3;
			NullCheck(L_23);
			if ((((int32_t)L_22) < ((int32_t)((int32_t)(((RuntimeArray*)L_23)->max_length)))))
			{
				goto IL_0078_1;
			}
		}
		{
			AssetBundle_tB38418819A49060CD738CB21541649340F082943* L_24 = V_0;
			NullCheck(L_24);
			GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_25;
			L_25 = AssetBundle_LoadAllAssets_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m61BD69AF333A9E321F6077CE3F20ED4A3AB26B58(L_24, AssetBundle_LoadAllAssets_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m61BD69AF333A9E321F6077CE3F20ED4A3AB26B58_RuntimeMethod_var);
			V_2 = L_25;
			GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_26 = V_2;
			NullCheck(L_26);
			int32_t L_27 = ((int32_t)(((RuntimeArray*)L_26)->max_length));
			RuntimeObject* L_28 = Box(Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C_il2cpp_TypeInfo_var, &L_27);
			String_t* L_29;
			L_29 = String_Format_mA8DBB4C2516B9723C5A41E6CB1E2FAF4BBE96DD8(_stringLiteral885FC71DFB0FF80A30E6013D905D1429AD60F467, L_28, NULL);
			il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
			((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus = L_29;
			Il2CppCodeGenWriteBarrier((void**)(&((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus), (void*)L_29);
			GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_30 = V_2;
			NullCheck(L_30);
			if (!(((RuntimeArray*)L_30)->max_length))
			{
				goto IL_01b1_1;
			}
		}
		{
			V_6 = (GameObject_t76FEDD663AB33C991A9C9A23129337651094216F*)NULL;
			V_7 = (-1);
			GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_31 = V_2;
			V_8 = L_31;
			V_4 = 0;
			goto IL_010c_1;
		}

IL_00cf_1:
		{
			GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_32 = V_8;
			int32_t L_33 = V_4;
			NullCheck(L_32);
			int32_t L_34 = L_33;
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_35 = (L_32)->GetAt(static_cast<il2cpp_array_size_t>(L_34));
			V_9 = L_35;
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_36 = V_9;
			NullCheck(L_36);
			Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_37;
			L_37 = GameObject_get_transform_m0BC10ADFA1632166AE5544BDF9038A2650C2AE56(L_36, NULL);
			NullCheck(L_37);
			Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_38;
			L_38 = Transform_get_parent_m65354E28A4C94EC00EBCF03532F7B0718380791E(L_37, NULL);
			il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
			bool L_39;
			L_39 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_38, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
			if (L_39)
			{
				goto IL_0106_1;
			}
		}
		{
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_40 = V_9;
			NullCheck(L_40);
			Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_41;
			L_41 = GameObject_get_transform_m0BC10ADFA1632166AE5544BDF9038A2650C2AE56(L_40, NULL);
			NullCheck(L_41);
			int32_t L_42;
			L_42 = Transform_get_childCount_mE9C29C702AB662CC540CA053EDE48BDAFA35B4B0(L_41, NULL);
			V_10 = L_42;
			int32_t L_43 = V_10;
			int32_t L_44 = V_7;
			if ((((int32_t)L_43) <= ((int32_t)L_44)))
			{
				goto IL_0106_1;
			}
		}
		{
			int32_t L_45 = V_10;
			V_7 = L_45;
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_46 = V_9;
			V_6 = L_46;
		}

IL_0106_1:
		{
			int32_t L_47 = V_4;
			V_4 = ((int32_t)il2cpp_codegen_add(L_47, 1));
		}

IL_010c_1:
		{
			int32_t L_48 = V_4;
			GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_49 = V_8;
			NullCheck(L_49);
			if ((((int32_t)L_48) < ((int32_t)((int32_t)(((RuntimeArray*)L_49)->max_length)))))
			{
				goto IL_00cf_1;
			}
		}
		{
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_50 = V_6;
			il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
			bool L_51;
			L_51 = Object_op_Equality_mB6120F782D83091EF56A198FCEBCF066DB4A9605(L_50, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
			if (!L_51)
			{
				goto IL_0127_1;
			}
		}
		{
			GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_52 = V_2;
			NullCheck(L_52);
			if (!(((RuntimeArray*)L_52)->max_length))
			{
				goto IL_0127_1;
			}
		}
		{
			GameObjectU5BU5D_tFF67550DFCE87096D7A3734EA15B75896B2722CF* L_53 = V_2;
			NullCheck(L_53);
			int32_t L_54 = 0;
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_55 = (L_53)->GetAt(static_cast<il2cpp_array_size_t>(L_54));
			V_6 = L_55;
		}

IL_0127_1:
		{
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_56 = V_6;
			il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
			bool L_57;
			L_57 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_56, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
			if (!L_57)
			{
				goto IL_01a2_1;
			}
		}
		{
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_58 = V_6;
			il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_59;
			L_59 = Object_Instantiate_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m10D87C6E0708CA912BBB02555BF7D0FBC5D7A2B3(L_58, Object_Instantiate_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m10D87C6E0708CA912BBB02555BF7D0FBC5D7A2B3_RuntimeMethod_var);
			V_11 = L_59;
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_60 = V_11;
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_61 = V_6;
			NullCheck(L_61);
			String_t* L_62;
			L_62 = Object_get_name_mAC2F6B897CF1303BA4249B4CB55271AFACBB6392(L_61, NULL);
			NullCheck(L_60);
			Object_set_name_mC79E6DC8FFD72479C90F0C4CC7F42A0FEAF5AE47(L_60, L_62, NULL);
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_63 = V_11;
			NullCheck(L_63);
			Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_64;
			L_64 = GameObject_get_transform_m0BC10ADFA1632166AE5544BDF9038A2650C2AE56(L_63, NULL);
			Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_65;
			L_65 = Vector3_get_zero_m0C1249C3F25B1C70EAD3CC8B31259975A457AE39_inline(NULL);
			NullCheck(L_64);
			Transform_set_position_mA1A817124BB41B685043DED2A9BA48CDF37C4156(L_64, L_65, NULL);
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_66 = V_11;
			NullCheck(L_66);
			Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_67;
			L_67 = GameObject_get_transform_m0BC10ADFA1632166AE5544BDF9038A2650C2AE56(L_66, NULL);
			Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 L_68;
			L_68 = Quaternion_get_identity_m7E701AE095ED10FD5EA0B50ABCFDE2EEFF2173A5_inline(NULL);
			NullCheck(L_67);
			Transform_set_rotation_m61340DE74726CF0F9946743A727C4D444397331D(L_67, L_68, NULL);
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_69 = V_6;
			NullCheck(L_69);
			String_t* L_70;
			L_70 = Object_get_name_mAC2F6B897CF1303BA4249B4CB55271AFACBB6392(L_69, NULL);
			String_t* L_71;
			L_71 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteral9229404B023B828641F8C179E7652573A8F786C0, L_70, NULL);
			il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
			((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus = L_71;
			Il2CppCodeGenWriteBarrier((void**)(&((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus), (void*)L_71);
			OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_72;
			L_72 = Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E(Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E_RuntimeMethod_var);
			V_12 = L_72;
			OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_73 = V_12;
			bool L_74;
			L_74 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_73, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
			if (!L_74)
			{
				goto IL_0259_1;
			}
		}
		{
			OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_75 = V_12;
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_76 = V_11;
			NullCheck(L_75);
			OrbitalCamera_FrameObject_m4F80B03857670CDAA5790C33318C434551757518(L_75, L_76, NULL);
			goto IL_0259_1;
		}

IL_01a2_1:
		{
			il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
			((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus = _stringLiteralAE6A994FF924EE125695D81F3A3A57FD5A4D9011;
			Il2CppCodeGenWriteBarrier((void**)(&((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus), (void*)_stringLiteralAE6A994FF924EE125695D81F3A3A57FD5A4D9011);
			goto IL_0259_1;
		}

IL_01b1_1:
		{
			StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_77 = V_1;
			V_3 = L_77;
			V_4 = 0;
			goto IL_024f_1;
		}

IL_01bb_1:
		{
			StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_78 = V_3;
			int32_t L_79 = V_4;
			NullCheck(L_78);
			int32_t L_80 = L_79;
			String_t* L_81 = (L_78)->GetAt(static_cast<il2cpp_array_size_t>(L_80));
			V_13 = L_81;
			String_t* L_82 = V_13;
			NullCheck(L_82);
			bool L_83;
			L_83 = String_EndsWith_mCD3754F5401E19CE7821CD398986E4EAA6AD87DC(L_82, _stringLiteralA742FD2BD1D07C567492D76B8DEB157161F97B02, NULL);
			if (!L_83)
			{
				goto IL_0249_1;
			}
		}
		{
			AssetBundle_tB38418819A49060CD738CB21541649340F082943* L_84 = V_0;
			String_t* L_85 = V_13;
			NullCheck(L_84);
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_86;
			L_86 = AssetBundle_LoadAsset_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m0216591F4BA27A483AA2C83EFDABCEA80B214063(L_84, L_85, AssetBundle_LoadAsset_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m0216591F4BA27A483AA2C83EFDABCEA80B214063_RuntimeMethod_var);
			V_14 = L_86;
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_87 = V_14;
			il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
			bool L_88;
			L_88 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_87, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
			if (!L_88)
			{
				goto IL_0249_1;
			}
		}
		{
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_89 = V_14;
			il2cpp_codegen_runtime_class_init_inline(Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C_il2cpp_TypeInfo_var);
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_90;
			L_90 = Object_Instantiate_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m10D87C6E0708CA912BBB02555BF7D0FBC5D7A2B3(L_89, Object_Instantiate_TisGameObject_t76FEDD663AB33C991A9C9A23129337651094216F_m10D87C6E0708CA912BBB02555BF7D0FBC5D7A2B3_RuntimeMethod_var);
			V_15 = L_90;
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_91 = V_15;
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_92 = V_14;
			NullCheck(L_92);
			String_t* L_93;
			L_93 = Object_get_name_mAC2F6B897CF1303BA4249B4CB55271AFACBB6392(L_92, NULL);
			NullCheck(L_91);
			Object_set_name_mC79E6DC8FFD72479C90F0C4CC7F42A0FEAF5AE47(L_91, L_93, NULL);
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_94 = V_15;
			NullCheck(L_94);
			Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_95;
			L_95 = GameObject_get_transform_m0BC10ADFA1632166AE5544BDF9038A2650C2AE56(L_94, NULL);
			Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_96;
			L_96 = Vector3_get_zero_m0C1249C3F25B1C70EAD3CC8B31259975A457AE39_inline(NULL);
			NullCheck(L_95);
			Transform_set_position_mA1A817124BB41B685043DED2A9BA48CDF37C4156(L_95, L_96, NULL);
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_97 = V_15;
			NullCheck(L_97);
			Transform_tB27202C6F4E36D225EE28A13E4D662BF99785DB1* L_98;
			L_98 = GameObject_get_transform_m0BC10ADFA1632166AE5544BDF9038A2650C2AE56(L_97, NULL);
			Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 L_99;
			L_99 = Quaternion_get_identity_m7E701AE095ED10FD5EA0B50ABCFDE2EEFF2173A5_inline(NULL);
			NullCheck(L_98);
			Transform_set_rotation_m61340DE74726CF0F9946743A727C4D444397331D(L_98, L_99, NULL);
			String_t* L_100 = V_13;
			String_t* L_101;
			L_101 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteral8812C2A9B10FA45D1F9952B4E63A21B3398D5199, L_100, NULL);
			il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
			((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus = L_101;
			Il2CppCodeGenWriteBarrier((void**)(&((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus), (void*)L_101);
			OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_102;
			L_102 = Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E(Object_FindObjectOfType_TisOrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F_mAACC9A772B0F5D2DC937BF7991D76D29EA75531E_RuntimeMethod_var);
			V_16 = L_102;
			OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_103 = V_16;
			bool L_104;
			L_104 = Object_op_Inequality_mD0BE578448EAA61948F25C32F8DD55AB1F778602(L_103, (Object_tC12DECB6760A7F2CBF65D9DCF18D044C2D97152C*)NULL, NULL);
			if (!L_104)
			{
				goto IL_0259_1;
			}
		}
		{
			OrbitalCamera_t122F542D545CB8D67FC45E86F4C90D59A3B10F4F* L_105 = V_16;
			GameObject_t76FEDD663AB33C991A9C9A23129337651094216F* L_106 = V_15;
			NullCheck(L_105);
			OrbitalCamera_FrameObject_m4F80B03857670CDAA5790C33318C434551757518(L_105, L_106, NULL);
			goto IL_0259_1;
		}

IL_0249_1:
		{
			int32_t L_107 = V_4;
			V_4 = ((int32_t)il2cpp_codegen_add(L_107, 1));
		}

IL_024f_1:
		{
			int32_t L_108 = V_4;
			StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_109 = V_3;
			NullCheck(L_109);
			if ((((int32_t)L_108) < ((int32_t)((int32_t)(((RuntimeArray*)L_109)->max_length)))))
			{
				goto IL_01bb_1;
			}
		}

IL_0259_1:
		{
			goto IL_027c;
		}
	}
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Exception_t_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_025b;
		}
		throw e;
	}

CATCH_025b:
	{
		Exception_t* L_110 = ((Exception_t*)IL2CPP_GET_ACTIVE_EXCEPTION(Exception_t*));;
		V_17 = L_110;
		Exception_t* L_111 = V_17;
		NullCheck(L_111);
		String_t* L_112;
		L_112 = VirtualFuncInvoker0< String_t* >::Invoke(5, L_111);
		String_t* L_113;
		L_113 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral73ED92A2DE5120A5716646E377BB552B3B4E51AF)), L_112, NULL);
		il2cpp_codegen_runtime_class_init_inline(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var)));
		((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))))->___LoadStatus = L_113;
		Il2CppCodeGenWriteBarrier((void**)(&((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))))->___LoadStatus), (void*)L_113);
		Exception_t* L_114 = V_17;
		il2cpp_codegen_runtime_class_init_inline(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Debug_t8394C7EEAECA3689C2C9B9DE9C7166D73596276F_il2cpp_TypeInfo_var)));
		Debug_LogException_mAB3F4DC7297ED8FBB49DAA718B70E59A6B0171B0(L_114, NULL);
		IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
		goto IL_027c;
	}

IL_027c:
	{
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ViewerBootstrap_OnGUI_mFD0E2DD97C0D5C26E1CF8A5DACFE61CB916DB862 (ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&GUI_tA9CDB3D69DB13D51AD83ABDB587EF95947EC2D2A_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralA24C6AB40AC1DE13B9BE17F5CBFB1B071711214B);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralAC6D401179964D3F1472C368D58CF7AE3793FCC3);
		s_Il2CppMethodInitialized = true;
	}
	{
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_0;
		L_0 = Color_get_yellow_m66637FA14383E8D74F24AE256B577CE1D55D469F_inline(NULL);
		il2cpp_codegen_runtime_class_init_inline(GUI_tA9CDB3D69DB13D51AD83ABDB587EF95947EC2D2A_il2cpp_TypeInfo_var);
		GUI_set_color_mA44927D3F2ACB1E228815F10042A6F62B7645648(L_0, NULL);
		Rect_tA04E0F8A1830E767F40FB27ECD8D309303571F0D L_1;
		memset((&L_1), 0, sizeof(L_1));
		Rect__ctor_m18C3033D135097BEE424AAA68D91C706D2647F23_inline((&L_1), (10.0f), (120.0f), (600.0f), (25.0f), NULL);
		il2cpp_codegen_runtime_class_init_inline(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
		String_t* L_2 = ((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus;
		String_t* L_3;
		L_3 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteralA24C6AB40AC1DE13B9BE17F5CBFB1B071711214B, L_2, NULL);
		GUI_Label_m4A951E57C7DCCF95A0306240144CA2713F546526(L_1, L_3, NULL);
		Rect_tA04E0F8A1830E767F40FB27ECD8D309303571F0D L_4;
		memset((&L_4), 0, sizeof(L_4));
		Rect__ctor_m18C3033D135097BEE424AAA68D91C706D2647F23_inline((&L_4), (10.0f), (145.0f), (600.0f), (25.0f), NULL);
		String_t* L_5 = ((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___TargetFile;
		String_t* L_6;
		L_6 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(_stringLiteralAC6D401179964D3F1472C368D58CF7AE3793FCC3, L_5, NULL);
		GUI_Label_m4A951E57C7DCCF95A0306240144CA2713F546526(L_4, L_6, NULL);
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ViewerBootstrap__ctor_m4ECCD39299DE78E944947407F2DAB0EC1F3AFD5F (ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628* __this, const RuntimeMethod* method) 
{
	{
		MonoBehaviour__ctor_m592DB0105CA0BC97AA1C5F4AD27B12D68A3B7C1E(__this, NULL);
		return;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ViewerBootstrap__cctor_m5157BE9BDE9B50EF323595059796BC80A592215B (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteral662268AE378A5F5A4C0AA5809EF1C7B45437F544);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709);
		s_Il2CppMethodInitialized = true;
	}
	{
		((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___TargetFile = _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709;
		Il2CppCodeGenWriteBarrier((void**)(&((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___TargetFile), (void*)_stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709);
		((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus = _stringLiteral662268AE378A5F5A4C0AA5809EF1C7B45437F544;
		Il2CppCodeGenWriteBarrier((void**)(&((ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_StaticFields*)il2cpp_codegen_static_fields_for(ViewerBootstrap_tA93C1912CC240EBC3793103528B51B6B6F3C2628_il2cpp_TypeInfo_var))->___LoadStatus), (void*)_stringLiteral662268AE378A5F5A4C0AA5809EF1C7B45437F544);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E UnitySourceGeneratedAssemblyMonoScriptTypes_v1_Get_mBEB95BEB954BB63E9710BBC7AD5E78C4CB0A0033 (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CPrivateImplementationDetailsU3E_t0F5473E849A5A5185A9F4C5246F0C32816C49FCA____82EF95C41CD7F156435E639A55748D8675CB4462876A899CF7A2DC6FF8925C26_FieldInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CPrivateImplementationDetailsU3E_t0F5473E849A5A5185A9F4C5246F0C32816C49FCA____EFEC78FCBA94D5281B7667D00F5FD326E4EE4C94B3639498A86DFEB7A0D540B5_FieldInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		il2cpp_codegen_initobj((&V_0), sizeof(MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E));
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = (ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)(ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)SZArrayNew(ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031_il2cpp_TypeInfo_var, (uint32_t)((int32_t)118));
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1 = L_0;
		RuntimeFieldHandle_t6E4C45B6D2EA12FC99185805A7E77527899B25C5 L_2 = { reinterpret_cast<intptr_t> (U3CPrivateImplementationDetailsU3E_t0F5473E849A5A5185A9F4C5246F0C32816C49FCA____82EF95C41CD7F156435E639A55748D8675CB4462876A899CF7A2DC6FF8925C26_FieldInfo_var) };
		RuntimeHelpers_InitializeArray_m751372AA3F24FBF6DA9B9D687CBFA2DE436CAB9B((RuntimeArray*)L_1, L_2, NULL);
		(&V_0)->___FilePathsData = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&(&V_0)->___FilePathsData), (void*)L_1);
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_3 = (ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)(ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)SZArrayNew(ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031_il2cpp_TypeInfo_var, (uint32_t)((int32_t)55));
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_4 = L_3;
		RuntimeFieldHandle_t6E4C45B6D2EA12FC99185805A7E77527899B25C5 L_5 = { reinterpret_cast<intptr_t> (U3CPrivateImplementationDetailsU3E_t0F5473E849A5A5185A9F4C5246F0C32816C49FCA____EFEC78FCBA94D5281B7667D00F5FD326E4EE4C94B3639498A86DFEB7A0D540B5_FieldInfo_var) };
		RuntimeHelpers_InitializeArray_m751372AA3F24FBF6DA9B9D687CBFA2DE436CAB9B((RuntimeArray*)L_4, L_5, NULL);
		(&V_0)->___TypesData = L_4;
		Il2CppCodeGenWriteBarrier((void**)(&(&V_0)->___TypesData), (void*)L_4);
		(&V_0)->___TotalFiles = 3;
		(&V_0)->___TotalTypes = 3;
		(&V_0)->___IsEditorOnly = (bool)0;
		MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E L_6 = V_0;
		return L_6;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnitySourceGeneratedAssemblyMonoScriptTypes_v1__ctor_mE70FB23ACC1EA12ABC948AA22C2E78B2D0AA39B1 (UnitySourceGeneratedAssemblyMonoScriptTypes_v1_tC95F24D0C6E6B77389433852BB389F39C692926E* __this, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C void MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshal_pinvoke(const MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E& unmarshaled, MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshaled_pinvoke& marshaled)
{
	marshaled.___FilePathsData = il2cpp_codegen_com_marshal_safe_array(IL2CPP_VT_I1, unmarshaled.___FilePathsData);
	marshaled.___TypesData = il2cpp_codegen_com_marshal_safe_array(IL2CPP_VT_I1, unmarshaled.___TypesData);
	marshaled.___TotalTypes = unmarshaled.___TotalTypes;
	marshaled.___TotalFiles = unmarshaled.___TotalFiles;
	marshaled.___IsEditorOnly = static_cast<int32_t>(unmarshaled.___IsEditorOnly);
}
IL2CPP_EXTERN_C void MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshal_pinvoke_back(const MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshaled_pinvoke& marshaled, MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E& unmarshaled)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	unmarshaled.___FilePathsData = (ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)il2cpp_codegen_com_marshal_safe_array_result(IL2CPP_VT_I1, Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_il2cpp_TypeInfo_var, marshaled.___FilePathsData);
	Il2CppCodeGenWriteBarrier((void**)(&unmarshaled.___FilePathsData), (void*)(ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)il2cpp_codegen_com_marshal_safe_array_result(IL2CPP_VT_I1, Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_il2cpp_TypeInfo_var, marshaled.___FilePathsData));
	unmarshaled.___TypesData = (ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)il2cpp_codegen_com_marshal_safe_array_result(IL2CPP_VT_I1, Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_il2cpp_TypeInfo_var, marshaled.___TypesData);
	Il2CppCodeGenWriteBarrier((void**)(&unmarshaled.___TypesData), (void*)(ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)il2cpp_codegen_com_marshal_safe_array_result(IL2CPP_VT_I1, Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_il2cpp_TypeInfo_var, marshaled.___TypesData));
	int32_t unmarshaledTotalTypes_temp_2 = 0;
	unmarshaledTotalTypes_temp_2 = marshaled.___TotalTypes;
	unmarshaled.___TotalTypes = unmarshaledTotalTypes_temp_2;
	int32_t unmarshaledTotalFiles_temp_3 = 0;
	unmarshaledTotalFiles_temp_3 = marshaled.___TotalFiles;
	unmarshaled.___TotalFiles = unmarshaledTotalFiles_temp_3;
	bool unmarshaledIsEditorOnly_temp_4 = false;
	unmarshaledIsEditorOnly_temp_4 = static_cast<bool>(marshaled.___IsEditorOnly);
	unmarshaled.___IsEditorOnly = unmarshaledIsEditorOnly_temp_4;
}
IL2CPP_EXTERN_C void MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshal_pinvoke_cleanup(MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshaled_pinvoke& marshaled)
{
	il2cpp_codegen_com_destroy_safe_array(marshaled.___FilePathsData);
	marshaled.___FilePathsData = NULL;
	il2cpp_codegen_com_destroy_safe_array(marshaled.___TypesData);
	marshaled.___TypesData = NULL;
}
IL2CPP_EXTERN_C void MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshal_com(const MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E& unmarshaled, MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshaled_com& marshaled)
{
	marshaled.___FilePathsData = il2cpp_codegen_com_marshal_safe_array(IL2CPP_VT_I1, unmarshaled.___FilePathsData);
	marshaled.___TypesData = il2cpp_codegen_com_marshal_safe_array(IL2CPP_VT_I1, unmarshaled.___TypesData);
	marshaled.___TotalTypes = unmarshaled.___TotalTypes;
	marshaled.___TotalFiles = unmarshaled.___TotalFiles;
	marshaled.___IsEditorOnly = static_cast<int32_t>(unmarshaled.___IsEditorOnly);
}
IL2CPP_EXTERN_C void MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshal_com_back(const MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshaled_com& marshaled, MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E& unmarshaled)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	unmarshaled.___FilePathsData = (ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)il2cpp_codegen_com_marshal_safe_array_result(IL2CPP_VT_I1, Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_il2cpp_TypeInfo_var, marshaled.___FilePathsData);
	Il2CppCodeGenWriteBarrier((void**)(&unmarshaled.___FilePathsData), (void*)(ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)il2cpp_codegen_com_marshal_safe_array_result(IL2CPP_VT_I1, Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_il2cpp_TypeInfo_var, marshaled.___FilePathsData));
	unmarshaled.___TypesData = (ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)il2cpp_codegen_com_marshal_safe_array_result(IL2CPP_VT_I1, Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_il2cpp_TypeInfo_var, marshaled.___TypesData);
	Il2CppCodeGenWriteBarrier((void**)(&unmarshaled.___TypesData), (void*)(ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)il2cpp_codegen_com_marshal_safe_array_result(IL2CPP_VT_I1, Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3_il2cpp_TypeInfo_var, marshaled.___TypesData));
	int32_t unmarshaledTotalTypes_temp_2 = 0;
	unmarshaledTotalTypes_temp_2 = marshaled.___TotalTypes;
	unmarshaled.___TotalTypes = unmarshaledTotalTypes_temp_2;
	int32_t unmarshaledTotalFiles_temp_3 = 0;
	unmarshaledTotalFiles_temp_3 = marshaled.___TotalFiles;
	unmarshaled.___TotalFiles = unmarshaledTotalFiles_temp_3;
	bool unmarshaledIsEditorOnly_temp_4 = false;
	unmarshaledIsEditorOnly_temp_4 = static_cast<bool>(marshaled.___IsEditorOnly);
	unmarshaled.___IsEditorOnly = unmarshaledIsEditorOnly_temp_4;
}
IL2CPP_EXTERN_C void MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshal_com_cleanup(MonoScriptData_t8F50E352855B96FFFC1D9CB07EACC90C99D73A3E_marshaled_com& marshaled)
{
	il2cpp_codegen_com_destroy_safe_array(marshaled.___FilePathsData);
	marshaled.___FilePathsData = NULL;
	il2cpp_codegen_com_destroy_safe_array(marshaled.___TypesData);
	marshaled.___TypesData = NULL;
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR float Mathf_Clamp_mEB9AEA827D27D20FCC787F7375156AF46BB12BBF_inline (float ___0_value, float ___1_min, float ___2_max, const RuntimeMethod* method) 
{
	bool V_0 = false;
	bool V_1 = false;
	float V_2 = 0.0f;
	{
		float L_0 = ___0_value;
		float L_1 = ___1_min;
		V_0 = (bool)((((float)L_0) < ((float)L_1))? 1 : 0);
		bool L_2 = V_0;
		if (!L_2)
		{
			goto IL_000e;
		}
	}
	{
		float L_3 = ___1_min;
		___0_value = L_3;
		goto IL_0019;
	}

IL_000e:
	{
		float L_4 = ___0_value;
		float L_5 = ___2_max;
		V_1 = (bool)((((float)L_4) > ((float)L_5))? 1 : 0);
		bool L_6 = V_1;
		if (!L_6)
		{
			goto IL_0019;
		}
	}
	{
		float L_7 = ___2_max;
		___0_value = L_7;
	}

IL_0019:
	{
		float L_8 = ___0_value;
		V_2 = L_8;
		goto IL_001d;
	}

IL_001d:
	{
		float L_9 = V_2;
		return L_9;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 Quaternion_Euler_m9262AB29E3E9CE94EF71051F38A28E82AEC73F90_inline (float ___0_x, float ___1_y, float ___2_z, const RuntimeMethod* method) 
{
	Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		float L_0 = ___0_x;
		float L_1 = ___1_y;
		float L_2 = ___2_z;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_3;
		memset((&L_3), 0, sizeof(L_3));
		Vector3__ctor_m376936E6B999EF1ECBE57D990A386303E2283DE0_inline((&L_3), L_0, L_1, L_2, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_4;
		L_4 = Vector3_op_Multiply_m87BA7C578F96C8E49BB07088DAAC4649F83B0353_inline(L_3, (0.0174532924f), NULL);
		Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 L_5;
		L_5 = Quaternion_Internal_FromEulerRad_m66D4475341F53949471E6870FB5C5E4A5E9BA93E(L_4, NULL);
		V_0 = L_5;
		goto IL_001b;
	}

IL_001b:
	{
		Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 L_6 = V_0;
		return L_6;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_get_zero_m0C1249C3F25B1C70EAD3CC8B31259975A457AE39_inline (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ((Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_StaticFields*)il2cpp_codegen_static_fields_for(Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var))->___zeroVector;
		V_0 = L_0;
		goto IL_0009;
	}

IL_0009:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1 = V_0;
		return L_1;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_op_Addition_m78C0EC70CB66E8DCAC225743D82B268DAEE92067_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_a, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___1_b, const RuntimeMethod* method) 
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ___0_a;
		float L_1 = L_0.___x;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2 = ___1_b;
		float L_3 = L_2.___x;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_4 = ___0_a;
		float L_5 = L_4.___y;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_6 = ___1_b;
		float L_7 = L_6.___y;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_8 = ___0_a;
		float L_9 = L_8.___z;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_10 = ___1_b;
		float L_11 = L_10.___z;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_12;
		memset((&L_12), 0, sizeof(L_12));
		Vector3__ctor_m376936E6B999EF1ECBE57D990A386303E2283DE0_inline((&L_12), ((float)il2cpp_codegen_add(L_1, L_3)), ((float)il2cpp_codegen_add(L_5, L_7)), ((float)il2cpp_codegen_add(L_9, L_11)), NULL);
		V_0 = L_12;
		goto IL_0030;
	}

IL_0030:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_13 = V_0;
		return L_13;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_op_Subtraction_mE42023FF80067CB44A1D4A27EB7CF2B24CABB828_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_a, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___1_b, const RuntimeMethod* method) 
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ___0_a;
		float L_1 = L_0.___x;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2 = ___1_b;
		float L_3 = L_2.___x;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_4 = ___0_a;
		float L_5 = L_4.___y;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_6 = ___1_b;
		float L_7 = L_6.___y;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_8 = ___0_a;
		float L_9 = L_8.___z;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_10 = ___1_b;
		float L_11 = L_10.___z;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_12;
		memset((&L_12), 0, sizeof(L_12));
		Vector3__ctor_m376936E6B999EF1ECBE57D990A386303E2283DE0_inline((&L_12), ((float)il2cpp_codegen_subtract(L_1, L_3)), ((float)il2cpp_codegen_subtract(L_5, L_7)), ((float)il2cpp_codegen_subtract(L_9, L_11)), NULL);
		V_0 = L_12;
		goto IL_0030;
	}

IL_0030:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_13 = V_0;
		return L_13;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_get_up_m128AF3FDC820BF59D5DE86D973E7DE3F20C3AEBA_inline (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ((Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_StaticFields*)il2cpp_codegen_static_fields_for(Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var))->___upVector;
		V_0 = L_0;
		goto IL_0009;
	}

IL_0009:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1 = V_0;
		return L_1;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_get_down_mF62B2AE7C5AC31EAC9CB62797C7190C90A7A8599_inline (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ((Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_StaticFields*)il2cpp_codegen_static_fields_for(Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2_il2cpp_TypeInfo_var))->___downVector;
		V_0 = L_0;
		goto IL_0009;
	}

IL_0009:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1 = V_0;
		return L_1;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_op_Multiply_m87BA7C578F96C8E49BB07088DAAC4649F83B0353_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_a, float ___1_d, const RuntimeMethod* method) 
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ___0_a;
		float L_1 = L_0.___x;
		float L_2 = ___1_d;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_3 = ___0_a;
		float L_4 = L_3.___y;
		float L_5 = ___1_d;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_6 = ___0_a;
		float L_7 = L_6.___z;
		float L_8 = ___1_d;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_9;
		memset((&L_9), 0, sizeof(L_9));
		Vector3__ctor_m376936E6B999EF1ECBE57D990A386303E2283DE0_inline((&L_9), ((float)il2cpp_codegen_multiply(L_1, L_2)), ((float)il2cpp_codegen_multiply(L_4, L_5)), ((float)il2cpp_codegen_multiply(L_7, L_8)), NULL);
		V_0 = L_9;
		goto IL_0021;
	}

IL_0021:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_10 = V_0;
		return L_10;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Color_tD001788D726C3A7F1379BEED0260B9591F440C1F Color_get_green_mEB001F2CD8C68C6BBAEF9101990B779D3AA2A6EF_inline (const RuntimeMethod* method) 
{
	Color_tD001788D726C3A7F1379BEED0260B9591F440C1F V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_0;
		memset((&L_0), 0, sizeof(L_0));
		Color__ctor_m3786F0D6E510D9CFA544523A955870BD2A514C8C_inline((&L_0), (0.0f), (1.0f), (0.0f), (1.0f), NULL);
		V_0 = L_0;
		goto IL_001d;
	}

IL_001d:
	{
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_1 = V_0;
		return L_1;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Bounds_get_center_m5B05F81CB835EB6DD8628FDA24B638F477984DC3_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, const RuntimeMethod* method) 
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = __this->___m_Center;
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1 = V_0;
		return L_1;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Bounds_get_size_m0699A53A55A78B3201D7270D6F338DFA91B6FAD4_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, const RuntimeMethod* method) 
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = __this->___m_Extents;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1;
		L_1 = Vector3_op_Multiply_m87BA7C578F96C8E49BB07088DAAC4649F83B0353_inline(L_0, (2.0f), NULL);
		V_0 = L_1;
		goto IL_0014;
	}

IL_0014:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2 = V_0;
		return L_2;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Rect__ctor_m18C3033D135097BEE424AAA68D91C706D2647F23_inline (Rect_tA04E0F8A1830E767F40FB27ECD8D309303571F0D* __this, float ___0_x, float ___1_y, float ___2_width, float ___3_height, const RuntimeMethod* method) 
{
	{
		float L_0 = ___0_x;
		__this->___m_XMin = L_0;
		float L_1 = ___1_y;
		__this->___m_YMin = L_1;
		float L_2 = ___2_width;
		__this->___m_Width = L_2;
		float L_3 = ___3_height;
		__this->___m_Height = L_3;
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Color_tD001788D726C3A7F1379BEED0260B9591F440C1F Color_get_cyan_m182A29E7475C0A98ACC03E1CF5252BAB83F0BA31_inline (const RuntimeMethod* method) 
{
	Color_tD001788D726C3A7F1379BEED0260B9591F440C1F V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_0;
		memset((&L_0), 0, sizeof(L_0));
		Color__ctor_m3786F0D6E510D9CFA544523A955870BD2A514C8C_inline((&L_0), (0.0f), (1.0f), (1.0f), (1.0f), NULL);
		V_0 = L_0;
		goto IL_001d;
	}

IL_001d:
	{
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_1 = V_0;
		return L_1;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds__ctor_mAF7B238B9FBF90C495E5D7951760085A93119C5A_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_center, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___1_size, const RuntimeMethod* method) 
{
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ___0_center;
		__this->___m_Center = L_0;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1 = ___1_size;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2;
		L_2 = Vector3_op_Multiply_m87BA7C578F96C8E49BB07088DAAC4649F83B0353_inline(L_1, (0.5f), NULL);
		__this->___m_Extents = L_2;
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds_Encapsulate_m7C70C382B9380A8C962074C78E189B53CE8F7A22_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3 ___0_bounds, const RuntimeMethod* method) 
{
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0;
		L_0 = Bounds_get_center_m5B05F81CB835EB6DD8628FDA24B638F477984DC3_inline((&___0_bounds), NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1;
		L_1 = Bounds_get_extents_mFE6DC407FCE2341BE2C750CB554055D211281D25_inline((&___0_bounds), NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2;
		L_2 = Vector3_op_Subtraction_mE42023FF80067CB44A1D4A27EB7CF2B24CABB828_inline(L_0, L_1, NULL);
		Bounds_Encapsulate_m1FCA57C58536ADB67B85A703470C6F5BFB837C2F_inline(__this, L_2, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_3;
		L_3 = Bounds_get_center_m5B05F81CB835EB6DD8628FDA24B638F477984DC3_inline((&___0_bounds), NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_4;
		L_4 = Bounds_get_extents_mFE6DC407FCE2341BE2C750CB554055D211281D25_inline((&___0_bounds), NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_5;
		L_5 = Vector3_op_Addition_m78C0EC70CB66E8DCAC225743D82B268DAEE92067_inline(L_3, L_4, NULL);
		Bounds_Encapsulate_m1FCA57C58536ADB67B85A703470C6F5BFB837C2F_inline(__this, L_5, NULL);
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds_set_center_m891869DD5B1BEEE2D17907BBFB7EB79AAE44884B_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_value, const RuntimeMethod* method) 
{
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ___0_value;
		__this->___m_Center = L_0;
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Vector3__ctor_m376936E6B999EF1ECBE57D990A386303E2283DE0_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2* __this, float ___0_x, float ___1_y, float ___2_z, const RuntimeMethod* method) 
{
	{
		float L_0 = ___0_x;
		__this->___x = L_0;
		float L_1 = ___1_y;
		__this->___y = L_1;
		float L_2 = ___2_z;
		__this->___z = L_2;
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds_set_size_m950CFB68CDD1BF409E770509A38B958E1AE68128_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_value, const RuntimeMethod* method) 
{
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ___0_value;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1;
		L_1 = Vector3_op_Multiply_m87BA7C578F96C8E49BB07088DAAC4649F83B0353_inline(L_0, (0.5f), NULL);
		__this->___m_Extents = L_1;
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR float Mathf_Max_mF5379E63D2BBAC76D090748695D833934F8AD051_inline (float ___0_a, float ___1_b, const RuntimeMethod* method) 
{
	float V_0 = 0.0f;
	float G_B3_0 = 0.0f;
	{
		float L_0 = ___0_a;
		float L_1 = ___1_b;
		if ((((float)L_0) > ((float)L_1)))
		{
			goto IL_0008;
		}
	}
	{
		float L_2 = ___1_b;
		G_B3_0 = L_2;
		goto IL_0009;
	}

IL_0008:
	{
		float L_3 = ___0_a;
		G_B3_0 = L_3;
	}

IL_0009:
	{
		V_0 = G_B3_0;
		goto IL_000c;
	}

IL_000c:
	{
		float L_4 = V_0;
		return L_4;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 Quaternion_get_identity_m7E701AE095ED10FD5EA0B50ABCFDE2EEFF2173A5_inline (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 L_0 = ((Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974_StaticFields*)il2cpp_codegen_static_fields_for(Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974_il2cpp_TypeInfo_var))->___identityQuaternion;
		V_0 = L_0;
		goto IL_0009;
	}

IL_0009:
	{
		Quaternion_tDA59F214EF07D7700B26E40E562F267AF7306974 L_1 = V_0;
		return L_1;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Color_tD001788D726C3A7F1379BEED0260B9591F440C1F Color_get_yellow_m66637FA14383E8D74F24AE256B577CE1D55D469F_inline (const RuntimeMethod* method) 
{
	Color_tD001788D726C3A7F1379BEED0260B9591F440C1F V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_0;
		memset((&L_0), 0, sizeof(L_0));
		Color__ctor_m3786F0D6E510D9CFA544523A955870BD2A514C8C_inline((&L_0), (1.0f), (0.921568632f), (0.0156862754f), (1.0f), NULL);
		V_0 = L_0;
		goto IL_001d;
	}

IL_001d:
	{
		Color_tD001788D726C3A7F1379BEED0260B9591F440C1F L_1 = V_0;
		return L_1;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Color__ctor_m3786F0D6E510D9CFA544523A955870BD2A514C8C_inline (Color_tD001788D726C3A7F1379BEED0260B9591F440C1F* __this, float ___0_r, float ___1_g, float ___2_b, float ___3_a, const RuntimeMethod* method) 
{
	{
		float L_0 = ___0_r;
		__this->___r = L_0;
		float L_1 = ___1_g;
		__this->___g = L_1;
		float L_2 = ___2_b;
		__this->___b = L_2;
		float L_3 = ___3_a;
		__this->___a = L_3;
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Bounds_get_extents_mFE6DC407FCE2341BE2C750CB554055D211281D25_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, const RuntimeMethod* method) 
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = __this->___m_Extents;
		V_0 = L_0;
		goto IL_000a;
	}

IL_000a:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1 = V_0;
		return L_1;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds_Encapsulate_m1FCA57C58536ADB67B85A703470C6F5BFB837C2F_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_point, const RuntimeMethod* method) 
{
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0;
		L_0 = Bounds_get_min_m465AC9BBE1DE5D8E8AD95AC19B9899068FEEBB13_inline(__this, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1 = ___0_point;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2;
		L_2 = Vector3_Min_m1CAC3499F14EA87366C0C3C1F501B4FB2863CDB4_inline(L_0, L_1, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_3;
		L_3 = Bounds_get_max_m6446F2AB97C1E57CA89467B9DE52D4EB61F1CB09_inline(__this, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_4 = ___0_point;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_5;
		L_5 = Vector3_Max_m9B6D8FEE7F4CE32C0AAB682606FFBA59E1F37C74_inline(L_3, L_4, NULL);
		Bounds_SetMinMax_mB5F7DDF18EDB7F3F25FA6D2B36824F28978C540F_inline(__this, L_2, L_5, NULL);
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Bounds_get_min_m465AC9BBE1DE5D8E8AD95AC19B9899068FEEBB13_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, const RuntimeMethod* method) 
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0;
		L_0 = Bounds_get_center_m5B05F81CB835EB6DD8628FDA24B638F477984DC3_inline(__this, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1;
		L_1 = Bounds_get_extents_mFE6DC407FCE2341BE2C750CB554055D211281D25_inline(__this, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2;
		L_2 = Vector3_op_Subtraction_mE42023FF80067CB44A1D4A27EB7CF2B24CABB828_inline(L_0, L_1, NULL);
		V_0 = L_2;
		goto IL_0015;
	}

IL_0015:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_3 = V_0;
		return L_3;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_Min_m1CAC3499F14EA87366C0C3C1F501B4FB2863CDB4_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_lhs, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___1_rhs, const RuntimeMethod* method) 
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ___0_lhs;
		float L_1 = L_0.___x;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2 = ___1_rhs;
		float L_3 = L_2.___x;
		float L_4;
		L_4 = Mathf_Min_m747CA71A9483CDB394B13BD0AD048EE17E48FFE4_inline(L_1, L_3, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_5 = ___0_lhs;
		float L_6 = L_5.___y;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_7 = ___1_rhs;
		float L_8 = L_7.___y;
		float L_9;
		L_9 = Mathf_Min_m747CA71A9483CDB394B13BD0AD048EE17E48FFE4_inline(L_6, L_8, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_10 = ___0_lhs;
		float L_11 = L_10.___z;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_12 = ___1_rhs;
		float L_13 = L_12.___z;
		float L_14;
		L_14 = Mathf_Min_m747CA71A9483CDB394B13BD0AD048EE17E48FFE4_inline(L_11, L_13, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_15;
		memset((&L_15), 0, sizeof(L_15));
		Vector3__ctor_m376936E6B999EF1ECBE57D990A386303E2283DE0_inline((&L_15), L_4, L_9, L_14, NULL);
		V_0 = L_15;
		goto IL_003c;
	}

IL_003c:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_16 = V_0;
		return L_16;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Bounds_get_max_m6446F2AB97C1E57CA89467B9DE52D4EB61F1CB09_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, const RuntimeMethod* method) 
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0;
		L_0 = Bounds_get_center_m5B05F81CB835EB6DD8628FDA24B638F477984DC3_inline(__this, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1;
		L_1 = Bounds_get_extents_mFE6DC407FCE2341BE2C750CB554055D211281D25_inline(__this, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2;
		L_2 = Vector3_op_Addition_m78C0EC70CB66E8DCAC225743D82B268DAEE92067_inline(L_0, L_1, NULL);
		V_0 = L_2;
		goto IL_0015;
	}

IL_0015:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_3 = V_0;
		return L_3;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 Vector3_Max_m9B6D8FEE7F4CE32C0AAB682606FFBA59E1F37C74_inline (Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_lhs, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___1_rhs, const RuntimeMethod* method) 
{
	Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ___0_lhs;
		float L_1 = L_0.___x;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2 = ___1_rhs;
		float L_3 = L_2.___x;
		float L_4;
		L_4 = Mathf_Max_mF5379E63D2BBAC76D090748695D833934F8AD051_inline(L_1, L_3, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_5 = ___0_lhs;
		float L_6 = L_5.___y;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_7 = ___1_rhs;
		float L_8 = L_7.___y;
		float L_9;
		L_9 = Mathf_Max_mF5379E63D2BBAC76D090748695D833934F8AD051_inline(L_6, L_8, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_10 = ___0_lhs;
		float L_11 = L_10.___z;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_12 = ___1_rhs;
		float L_13 = L_12.___z;
		float L_14;
		L_14 = Mathf_Max_mF5379E63D2BBAC76D090748695D833934F8AD051_inline(L_11, L_13, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_15;
		memset((&L_15), 0, sizeof(L_15));
		Vector3__ctor_m376936E6B999EF1ECBE57D990A386303E2283DE0_inline((&L_15), L_4, L_9, L_14, NULL);
		V_0 = L_15;
		goto IL_003c;
	}

IL_003c:
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_16 = V_0;
		return L_16;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds_SetMinMax_mB5F7DDF18EDB7F3F25FA6D2B36824F28978C540F_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_min, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___1_max, const RuntimeMethod* method) 
{
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ___1_max;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_1 = ___0_min;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_2;
		L_2 = Vector3_op_Subtraction_mE42023FF80067CB44A1D4A27EB7CF2B24CABB828_inline(L_0, L_1, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_3;
		L_3 = Vector3_op_Multiply_m87BA7C578F96C8E49BB07088DAAC4649F83B0353_inline(L_2, (0.5f), NULL);
		Bounds_set_extents_m09496358547B86A93EFE7BE6371E7A6FE937C46F_inline(__this, L_3, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_4 = ___0_min;
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_5;
		L_5 = Bounds_get_extents_mFE6DC407FCE2341BE2C750CB554055D211281D25_inline(__this, NULL);
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_6;
		L_6 = Vector3_op_Addition_m78C0EC70CB66E8DCAC225743D82B268DAEE92067_inline(L_4, L_5, NULL);
		Bounds_set_center_m891869DD5B1BEEE2D17907BBFB7EB79AAE44884B_inline(__this, L_6, NULL);
		return;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR float Mathf_Min_m747CA71A9483CDB394B13BD0AD048EE17E48FFE4_inline (float ___0_a, float ___1_b, const RuntimeMethod* method) 
{
	float V_0 = 0.0f;
	float G_B3_0 = 0.0f;
	{
		float L_0 = ___0_a;
		float L_1 = ___1_b;
		if ((((float)L_0) < ((float)L_1)))
		{
			goto IL_0008;
		}
	}
	{
		float L_2 = ___1_b;
		G_B3_0 = L_2;
		goto IL_0009;
	}

IL_0008:
	{
		float L_3 = ___0_a;
		G_B3_0 = L_3;
	}

IL_0009:
	{
		V_0 = G_B3_0;
		goto IL_000c;
	}

IL_000c:
	{
		float L_4 = V_0;
		return L_4;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Bounds_set_extents_m09496358547B86A93EFE7BE6371E7A6FE937C46F_inline (Bounds_t367E830C64BBF235ED8C3B2F8CF6254FDCAD39C3* __this, Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 ___0_value, const RuntimeMethod* method) 
{
	{
		Vector3_t24C512C7B96BBABAD472002D0BA2BDA40A5A80B2 L_0 = ___0_value;
		__this->___m_Extents = L_0;
		return;
	}
}
