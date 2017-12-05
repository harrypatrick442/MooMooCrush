Shader "Hole" {
	Properties
{
	_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
}

SubShader
{
	Tags
{
	"Queue" = "Transparent"
	"IgnoreProjector" = "True"
	"RenderType" = "Transparent"
}

LOD 100
Cull Off
Lighting Off
ZWrite On
Fog{ Mode Off }
ColorMask 0
AlphaTest Greater .01
Blend OneMinusSrcAlpha SrcAlpha

Pass
{
	ColorMaterial AmbientAndDiffuse

	SetTexture[_MainTex]
{
	Combine Texture * Primary
}
}
}
}