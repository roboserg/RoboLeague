Shader "Skidmarks" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	}

	Category {
		Offset -4, -4
		ZWrite Off
		Alphatest Greater 0
		Tags{ "Queue" = "Transparent"  "RenderType" = "Transparent" }

		SubShader{
			ColorMaterial AmbientAndDiffuse
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
			Pass{
				ColorMask RGBA
				SetTexture[_MainTex]{
					Combine texture, texture * primary
				}
				SetTexture[_MainTex]{
					Combine primary * previous
				}
			}
		}
	}

	// Fallback to Alpha Vertex Lit
	Fallback "Transparent/VertexLit", 2

}