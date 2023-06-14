//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	Description = "Chrome";
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
FEATURES
{
    #include "common/features.hlsl"
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
MODES
{
    VrForward();													// Indicates this shader will be used for main rendering
    Depth( "depth_only.shader" ); 									// Shader that will be used for shadowing and depth prepass
    ToolsVis( S_MODE_TOOLS_VIS ); 									// Ability to see in the editor
    ToolsWireframe( "tools_wireframe.shader" ); 					// Allows for mat_wireframe to work
	ToolsShadingComplexity( "tools_shading_complexity.shader" ); 	// Shows how expensive drawing is in debug view
}

//=========================================================================================================================
COMMON
{
	#include "common/shared.hlsl"
}

//=========================================================================================================================

struct VertexInput
{
	#include "common/vertexinput.hlsl"
};

//=========================================================================================================================

struct PixelInput
{
	#include "common/pixelinput.hlsl"
};

//=========================================================================================================================

VS
{
	#include "common/vertex.hlsl"
	//
	// Main
	//
	PixelInput MainVs( INSTANCED_SHADER_PARAMS( VS_INPUT i ) )
	{
		PixelInput o = ProcessVertex( i );
		// Add your vertex manipulation functions here
		return FinalizeVertex( o );
	}
}

//=========================================================================================================================

PS
{
    #include "common/pixel.hlsl"
	
	//
	// Main
	//

float ChromeOffsetX < UiType( Slider ); Default(0.0); Range( 0.0f, 8.0f ); UiGroup( "Chrome,10/20" ); >;
float ChromeOffsetY < UiType( Slider ); Default(0.0); Range( 0.0f, 8.0f ); UiGroup( "Chrome,10/20" ); >;
		
		

	CreateInputTexture2D( ChromeMapTexture, Srgb, 8, "", "_color",  "Shader Vars,10/10", Default3( 1, 1, 1) );
	CreateTexture2D( ChromeMap )  < Channel( RGB,  Box( ChromeMapTexture ), Srgb ); OutputFormat( BC7 ); SrgbRead( true ); >;
    //CreateTexture2D( ChromeMap ) < COLOR_TEXTURE_CHANNELS; OutputFormat( BC7 ); SrgbRead( true ); >;
	TextureAttribute( ChromeMap, ChromeMap );

	float4 MainPs( PixelInput i ) : SV_Target0
	{ 
		Material m = Material::From( i );

		float3 vPositionWs = i.vPositionWithOffsetWs.xyz + g_vHighPrecisionLightingOffsetWs.xyz;
		float3 vCameraToPositionDirWs = CalculateCameraToPositionDirWs( vPositionWs.xyz );

		float3 vNormalWs = normalize( i.vNormalWs.xyz );
		float3 vTangentUWs = i.vTangentUWs.xyz;
		float3 vTangentVWs = i.vTangentVWs.xyz;
		float3 vTangentViewVector = Vec3WsToTs( vCameraToPositionDirWs.xyz, vNormalWs.xyz, vTangentUWs.xyz, vTangentVWs.xyz );
		float2 chromeUV = frac( i.vTextureCoords.xy );

		// this is not how Half-Life 1 does it lol, I just did this because it looks close enough, please revisit :)

		

		float3 pos = float3(vTangentViewVector * 0.3 + (i.vTextureCoords * 0.02), 1 );

		float n1 = pos.x + 0.35 + ChromeOffsetX;
		float n2 = pos.y + 0.35 + ChromeOffsetY;
		float2 reflectUV = float2(n1,n2) * 1.3;
		
		

		float4 chrome = Tex2D( ChromeMap, reflectUV.xy );

		m.Albedo.rgb = chrome.rgb; 
		float4 p = ShadingModelStandard::Shade( i, m );
		return p;
	}
}