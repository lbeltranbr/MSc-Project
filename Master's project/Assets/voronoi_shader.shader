// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/voronoi_shader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        //_pointsAmount("Number of Points",Int) = 1
       // _pointPos("Point Position",Vector) = (.5, .5, .5, 0)
       // _colorPoint("Point Color",Color) = (.25, .5, .5, 1)       
           
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        //Cull Off ZWrite Off ZTest Always

        Pass 
        {

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                //fixed4 color : COLOR;
            };

            struct v2f {           

                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 objectPos : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                //Transforms a point from object space to the camera’s clip space in homogeneous coordinates.
                o.pos = UnityObjectToClipPos(v.vertex);
                o.objectPos = v.vertex;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;

                return o;           
            }

            sampler2D _MainTex;
            float4 _Color;
            int _change;

            uniform int _pointsAmount;
            uniform float4 _pointPos[500];
            uniform float4 _colorPoint[500];

            float4 frag(v2f i) : SV_Target
            {
                //float col = tex2D(_MainTex, i.uv);
                float4 col = _Color;
                float m_dist = 10000;
                int index = 0;
                float4 obPos = i.objectPos;
                float4 wPos = float4(i.worldPos.x, i.worldPos.y, i.worldPos.z, 0);
                //float4 w = float4(0.5, 0.9, 0.5, 0);

                for (int i = 0; i < _pointsAmount; i++) {

                    float dist = distance(wPos.xyz, _pointPos[i].xyz);
                    //float dist = distance(wPos.xyz, w);

                    if (dist <= m_dist) {
                        m_dist = dist;
                        index = i;
                    }
                      
                }
                
                if(_change==1)
                    col = float4(m_dist,m_dist,m_dist,1);
                else
                    col = col* _colorPoint[index];
                                    
                return col;
                
            }

            ENDCG

        }
    }
}
