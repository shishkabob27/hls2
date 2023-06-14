//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	Description = "Sprite";
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
    #define BLEND_MODE_ALREADY_SET
	RenderState( BlendEnable, true );	
	RenderState( SrcBlend, SRC_COLOR );
	RenderState( DstBlend, INV_SRC_COLOR );
	RenderState( BlendOp, ADD );
	BoolAttribute( translucent, true );
    #include "common/pixel.hlsl"
 

	float4 MainPs( PixelInput i ) : SV_Target0
	{
		Material m = Material::From( i );
		return float4( m.Albedo, 1.0 );
	}
}