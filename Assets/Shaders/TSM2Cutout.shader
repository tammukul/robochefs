// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "TSM2/Cutout" 
{
    Properties 
    {
		[MaterialToggle(_TEX_ON)] _DetailTex ("Enable Detail texture", Float) = 0 	//1
		_MainTex ("Detail", 2D) = "white" {}        								//2
		_ToonShade ("Shade", 2D) = "white" {}  										//3
		[MaterialToggle(_COLOR_ON)] _TintColor ("Enable Color Tint", Float) = 0 	//4
		_Color ("Base Color", Color) = (1,1,1,1)									//5	
		[MaterialToggle(_VCOLOR_ON)] _VertexColor ("Enable Vertex Color", Float) = 0//6        
		_Brightness ("Brightness 1 = neutral", Float) = 1.0							//7	
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5                                  //16
    }
   
    Subshader 
    {
    	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 200
        
        Pass 
        {
        Tags {"LightMode" = "ForwardBase"}
		ZWrite On ZTest LEqual
		Cull Off
		Lighting Off

            Name "BASE"
           	
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
                #pragma glsl_no_auto_normalization
                #pragma multi_compile _COLOR_OFF _COLOR_ON
                #pragma multi_compile _VCOLOR_OFF _VCOLOR_ON
                
                sampler2D _MainTex;
				half4 _MainTex_ST;
				
				struct appdata_base0 
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 texcoord : TEXCOORD0;
					#if _VCOLOR_ON
					fixed4 color : COLOR;
					#endif
				};
								
                struct v2f 
                {
                 	float4 pos : SV_POSITION;
                 	half2 uv : TEXCOORD0;
                 	half2 uvn : TEXCOORD1;
                 	#if _VCOLOR_ON
					fixed4 color : COLOR;
					#endif
                };
               
                v2f vert (appdata_base0 v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos ( v.vertex );
                   	float3 n = mul((float3x3)UNITY_MATRIX_IT_MV,normalize(v.normal));
					normalize(n);
                    n = n * float3(0.5,0.5,0.5) + float3(0.5,0.5,0.5);
                    o.uvn = n.xy;
                    o.uv = TRANSFORM_TEX ( v.texcoord, _MainTex );
                    
                    #if _VCOLOR_ON
					o.color = v.color;
					#endif
					
                    return o;
                }

              	sampler2D _ToonShade;
              	fixed _Brightness;
              	
              	#if _COLOR_ON
                fixed4 _Color;
                #endif   
          
                uniform fixed _Cutoff;
                
                fixed4 frag (v2f i) : COLOR
                {
					#if _COLOR_ON
					fixed4 toonShade = tex2D( _ToonShade,  i.uvn )*_Color;
					#else
					fixed4 toonShade = tex2D( _ToonShade,  i.uvn );
					#endif
					
					#if _VCOLOR_ON
					fixed4 detail = tex2D ( _MainTex, i.uv )*i.color;
					#else
					fixed4 detail = tex2D ( _MainTex, i.uv );
					#endif

               		clip( detail.a-_Cutoff );	
                  	return toonShade *detail*_Brightness;
                }
                
            ENDCG
        }
    }
    Fallback "Legacy Shaders/Transparent/Cutout/Diffuse"
}