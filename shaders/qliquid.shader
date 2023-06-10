//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	Description = "Quake Liquid";
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
    Depth( "vr_depth_only.shader" ); 									// Shader that will be used for shadowing and depth prepass
    ToolsVis( S_MODE_TOOLS_VIS ); 									// Ability to see in the editor
    ToolsWireframe( "vr_tools_wireframe.shader" ); 					// Allows for mat_wireframe to work
	ToolsShadingComplexity( "vr_tools_shading_complexity.shader" ); 	// Shows how expensive drawing is in debug view
}

//=========================================================================================================================
COMMON
{
    #include "common/shared.hlsl"
    #define S_SPECULAR 1
    #define S_SPECULAR_CUBE_MAP 1
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
    #define BLEND_MODE_ALREADY_SET
	RenderState( BlendEnable, true );
	RenderState( SrcBlend, SRC_ALPHA );
	RenderState( DstBlend, INV_SRC_ALPHA );
	BoolAttribute( translucent, true );
    #include "common/pixel.hlsl"
	
	//
	// Main
	//

    float WaveStrength < UiType( Slider ); Default1(1.0); Range( 0.0f, 8.0f ); UiGroup( "Wave,10/20" ); >;
		
    float WaveSize < UiType( Slider ); Default1(1.0); Range( 0.0f, 8.0f ); UiGroup( "Wave,10/20" ); >;
    float WaveSpeed < UiType( Slider ); Default1(1.0); Range( 0.0f, 8.0f ); UiGroup( "Wave,10/20" ); >;
    float WaveAlpha < UiType( Slider ); Default1(1.0); Range( 0.0f, 1.0f ); UiGroup( "Color,10/20" ); >;


	CreateInputTexture2D( WaveTexMapInput, Srgb, 8, "", "_color",  "Wave,10/20", Default3( 1, 1, 1) );
	CreateTexture2D( WaveTexMap )  < Channel( RGB,  Box( WaveTexMapInput ), Srgb ); OutputFormat( BC7 ); SrgbRead( true ); >; 
	TextureAttribute( WaveTexMap, WaveTexMap );


	float4 MainPs( PixelInput i ) : SV_Target0
	{

		Material m = Material::From( i );
		float2 inputUV = i.vTextureCoords.xy;
		

		float2 DeltaTime = (inputUV.yx * (WaveSize) + g_flTime * (WaveSpeed / 10)) * 3.14159265359;


		
        float2 uv = inputUV.xy + float2(sin(DeltaTime.x), sin(DeltaTime.y)) * (WaveStrength / 10);


		
		

		float4 wave = Tex2D( WaveTexMap, uv.xy );

		m.Albedo.rgb = wave.rgb; 
		float4 p = ShadingModelStandard::Shade( i, m );
		p.a = WaveAlpha;
		return p;
	}
}