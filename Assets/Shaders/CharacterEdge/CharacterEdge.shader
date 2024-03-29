Shader "Unlit/EdgeLine_Post"
{
    Properties
    {        
        _MainTex("MainTex",2D)="white"{}
        _EdgeRange("EdgeRange",float)=0  
        _LineColor("EdgeLineColor",Color)=(1,1,1,1)
        _SobelNum("SobelNum",int)=0
        
        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
        Name "Line_Post"
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
          
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;               
                float4 pos : SV_POSITION;
                float4 viewVector : TEXCOORD1;
                float4 screenpos : TEXCOORD2;
                float3 normal : NORMAL; 
            };

            sampler2D _MainTex;
            float4 _LineColor;
            float4 _MainTex_ST;
            float2 _ScaledScreenParams;
            float4 _FoamColor;
            float _EdgeRange;
            int _SobelNum ;
            int _SobelNum_P ;
            sampler2D _CameraDepthTexture;
            sampler2D _NormalTex;
            sampler2D _WaterTex;
            sampler2D _NoiseTexA;
           /*float _SobelX[5][5] = 
           {1, 2, 3, 2, 1,
            0, 3, 5, 3, 0,
            0, 0, 0, 0, 0,
            0,-3,-5,-3, 0,
           -1,-2,-3,-2,-1            
            };
          

            float _SobelY[5][5] = 
           {1, 0, 0, 0, -1,
            2, 3, 0,-3, -2,
            3, 5, 0,-5, -3,
            2, 3, 0,-3, -2,
            1, 0, 0, 0, -1
            };*/

            //如果是矩阵的话,用一维的索引,在2之后的全都是全都没有数值,只能用[ix][iy] 
           float _SobelX[25] = 
             {1, 2, 3, 2, 1,
            0, 3, 5, 3, 0,
            0, 0, 0, 0, 0,
            0,-3,-5,-3, 0,
           -1,-2,-3,-2,-1            
            };

            float _SobelY[25] = 
             {1, 0, 0, 0, -1,
            2, 3, 0,-3, -2,
            3, 5, 0,-5, -3,
            2, 3, 0,-3, -2,
            1, 0, 0, 0, -1
            };
            

        
            v2f vert (appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;

                float3 ndcpos = o.pos.xyz/o.pos.w;

                float3 clippos = float3(ndcpos.x,ndcpos.y * -1,ndcpos.z * -1)*_ProjectionParams.z;

                o.viewVector = mul(unity_CameraInvProjection,clippos.xyzz);

                o.screenpos = ComputeScreenPos(o.pos);

                o.normal = UnityObjectToClipPos(v.normal);

              
                             
                return o;
            }


            
            fixed4 frag (v2f v) : SV_Target
            {
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,(v.pos.xy/_ScaledScreenParams.xy));
                

                float3 viewpos = depth * v.viewVector;

                float4 wpos =  mul(unity_CameraToWorld,float4(viewpos.xyz,1));//当前点的世界坐标

                float2 nearpos[25];
                
                float nearnormal[25];
                float neardepth[25];

              for(int x = 0; x<_SobelNum;x++)
                for(int y = 0;y<_SobelNum;y++)
                {                  
                  int midpos = (_SobelNum-1)/2; 
                  float2 off = float2(((x-midpos)*ddx(v.uv.x)),((y-midpos)*(-1)*ddy(v.uv.y)));
                  nearpos[y+x*_SobelNum] = (Linear01Depth(depth)+0.25)*_EdgeRange*off+v.uv;//0.25是为了防止这个深度太近了
                }//结束*/        
                
               /* nearpos[0] = float2(float2(-ddx(v.uv.x),ddy(v.uv.y)));
                nearpos[1] = float2(float2(0,ddy(v.uv.y)));
                nearpos[2] = float2(float2(ddx(v.uv.x),ddy(v.uv.y)));
                nearpos[3] = float2(float2(-ddx(v.uv.x),0));
                nearpos[4] = float2(float2(0,0));
                nearpos[5] = float2(float2(+ddx(v.uv.x),0));
                nearpos[6] = float2(float2(-ddx(v.uv.x),-ddy(v.uv.y)));
                nearpos[7] = float2(float2(0,-ddy(v.uv.y)));
                nearpos[8] = float2(float2(ddx(v.uv.x),-ddy(v.uv.y)));//得到周围9个

                for (int i =0;i < pow(_SobelNum,2);i++)
                {
                 nearpos[i] = _FoamRange*nearpos[i]+v.uv;
                }//结束*/
                for(int i = 0;i < pow(_SobelNum,2); i++)
                {
                float3 normals = tex2D(_NormalTex,nearpos[i]);
                
                nearnormal[i]=1-abs(dot(normalize(tex2D(_NormalTex,nearpos[i])),normalize(tex2D(_NormalTex,v.uv))));
                }

                for(uint j = 0;j < pow(_SobelNum,2); j++)
                {
                neardepth[j] = abs(Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,nearpos[j]))-Linear01Depth(depth));
                }
                //两步采样

                float2 normalInd = float2(0,0);
                float2 depthInd = float2(0,0);

               for(int ix = 0 ; ix < _SobelNum; ix++)       
              {              
                for(int iy = 0 ; iy < _SobelNum; iy++)
                {
                int indexx = ix*_SobelNum+iy;
                normalInd.x += nearnormal[indexx]*_SobelX[indexx];
                normalInd.y += nearnormal[indexx]*_SobelY[indexx];               
                }
              }

               for(int jx = 0 ; jx < _SobelNum; jx++)              
              {
                for(int jy = 0 ; jy < _SobelNum; jy++)
                {                 
                 int indexy = jx*_SobelNum+jy;
                 depthInd.x += neardepth[indexy]*_SobelX[indexy];
                 depthInd.y += neardepth[indexy]*_SobelY[indexy];
                }
              }
                
                float normalIndex = sqrt(pow(normalInd.x,2)+pow(normalInd.y,2));
                //float depthIndex = sqrt((normalInd.x/normalIndex)*pow(depthInd.x,2)+(normalInd.y/normalIndex)*pow(depthInd.y,2));//这里算权重
                float depthIndex = sqrt(pow(depthInd.x,2)+pow(depthInd.y,2));//没有权重的一步

                depthIndex = (step(0.01,smoothstep(0,Linear01Depth(depth)*(_SobelNum*_SobelNum)/_EdgeRange,depthIndex)))*normalIndex;
                            

                float3 JudgeInd = tex2D(_NormalTex,v.uv);

                float4 finnalColor = step(0.01,depthIndex);
               /*if(length(JudgeInd) > 0)
                {
                  finnalColor = float4(0,0,0,0);
                }*/

                
                if(finnalColor.r>0)
                {
                return _LineColor;
                }
                else
                {
                return float4(tex2D(_MainTex,v.uv));
                }
            }
            ENDCG
        }
      
    }
    FallBack "Diffuse"
}
