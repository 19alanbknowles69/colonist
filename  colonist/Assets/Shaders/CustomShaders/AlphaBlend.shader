
Shader "Custom/AlphaBlend" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 0.2)
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		fixed4 _Color;
						
		struct v2f {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
		};

		v2f vert(appdata_full v) {
			v2f o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
			o.uv.xy = v.texcoord.xy;
					
			return o; 
		}
		
		fixed4 frag( v2f i ) : COLOR {	
			return tex2D (_MainTex, i.uv.xy) * _Color;
		}
	
	ENDCG
	
	SubShader {
		Tags { "RenderType" = "Transparent" "Reflection" = "RenderReflectionTransparentBlend" "Queue" = "Transparent"}
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
	Pass {
	
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}
				
	} 
	FallBack Off
}
