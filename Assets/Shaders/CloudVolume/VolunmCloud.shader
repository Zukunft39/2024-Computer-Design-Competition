
Shader "Custom/VolunmCloud"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTexA("NoiseTextureA",2D) = "white"{}
        _NoiseTexB("NoiseTextureB",2D) = "white"{}
        _NoiseTexC("3DNoiseTex",3D)="white"{}
        _Color ("LightColor",Color) = (1,1,1,1)
        _DarkColor("DarkColor",Color) = (1,1,1,1)
        _CloudIntensity("CloudIntensity",float) = 0
        _LightIntensity("LightIntensity",float) =0
        _LightMarchCount("LightMarch Count",int)=0
        _RayMarchCount("RayMarch Count",int)=0
        _BoundMin("Boundmin",vector) = (0,0,0)
        _BoundMax("Boundmax",vector) = (20,20,20)
        _CloudDes("CloundDens",Range(0,1)) =0
        _MoveSpeed("CloudSpeed",vector) = (0,0,0)
        _DepthCurve("DepthCurve",2D) = "white"{}
        _Tilling("CloudTiling",vector)=(0,0,0)

    }
    SubShader
    {
    Blend SrcAlpha OneMinusSrcAlpha
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Cull Back
        Pass
        {
            CGPROGRAM            
            #pragma vertex vert
            #pragma fragment frag            
            #include "UnityCG.cginc"
            /*深度引动模式(Depth Priming Mode):如果你的项目中使用了自定义的一些shader来实现特殊效果,这时如果直接使用新建材质与无光照着色器(Unlit shader)
            ,可能会发生一个对于新手而言意料之外的问题_物体消失了.这是因为深度引动模式需要使用一种LightMode为DepthOnly的特殊Pass来生成场景中不透明物体的深度图,
            而我们所创建的无光照着色器是不具有该类型Pass的(因为压根就没写这东西),这就导致在开启了该技术的渲染流程中,这些使用了"不具备 DepthOnlyPass 
            的shader"的物体无法生成深度信息从而被Unity过早剔除无法进行后续的渲染,造成了物体的消失.*/
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float viewVec : TEXCOORD1;
                float3 screenpos: TEXCOORD2;
                float3 wpos : TEXCOORD3;
                };

            float3 _BoundMin;
            float3 _BoundMax;
            float4 _Color;
            sampler2D _NoiseTexA;
            sampler2D _NoiseTexB;
            sampler3D _NoiseTexC;
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            fixed2 _ScaledScreenParams;
            float _CloudIntensity;
            int _RayMarchCount;
            int _LightMarchCount;
            float _CloudDes;
            float3 _MoveSpeed;
            float3 _Tilling;
            sampler2D _DepthCurve;
            float4 _LightColor0;                                            
            float _LightIntensity;
            float4 _DarkColor;
            
            
            float Random3DTo1D(float3 value, float a, float3 b) {
                float3 smallValue = sin(value);
                float random = dot(smallValue, b);
                random = frac(sin(random) * a);
                return random;
                }
              float RayMarchToLight(float3 dir, float3 pos,float liglength)//对光深度的获得,得到这个点被光照亮的概率,只是这个边界限制太小(现在是这里有问题)
            {
               // float steplength = /_LightMarchCount;
                float3 stepvec = liglength*dir;
                float3 curpos = pos + tex2D(_NoiseTexA,pos.yx);
                float totaldepth = 0;
                
              [unroll(10)] for(int i = 0; i < _LightMarchCount; i++)
                {                                                                                                                               
                     if(curpos.x<_BoundMax.x&&curpos.x>_BoundMin.x&&curpos.y<_BoundMax.y&&curpos.y>_BoundMin.y&&curpos.z<_BoundMax.z&&curpos.z>_BoundMin.z)                  
                     {   //我真的不知道为什么要在这里加限制范围,我觉得这个点直接采样如果不在云里就行了
                        float Temp = tex3D(_NoiseTexC, (curpos/length(_BoundMax-_BoundMin)+_MoveSpeed*_Time.y)*_Tilling);
                        Temp = smoothstep(0,1,Temp);
                        totaldepth += Temp;    
                      }
                        curpos += stepvec;
                        
                }

                return pow((1-(totaldepth/_LightMarchCount)),_LightIntensity);
            }

            float GetAttenution(float depth)
            {
               float2 UV = float2(depth,0);
               float atten = tex2D(_DepthCurve,UV);

               return atten;
            }
            float2 RayMarch(float3 dir , float3 pos , float raylength , v2f o)//光线步进,这里实际上是得到这个点的密度,如何解释呢?
            {
            float steplength = raylength/_RayMarchCount;
            float3 stepvec = dir*steplength;
            float3 curpos = pos+tex2D(_NoiseTexB,o.uv);
            float totalnum = 0;
            float totallight = 0;
            
                for(int i=0;i<_RayMarchCount;i++)
                {       
                    if(curpos.x<_BoundMax.x&&curpos.x>_BoundMin.x&&curpos.y<_BoundMax.y&&curpos.y>_BoundMin.y&&curpos.z<_BoundMax.z&&curpos.z>_BoundMin.z)
                    {
                       float3 Tolight = -normalize(_WorldSpaceLightPos0).xyz;

                       float lightLength = length(_BoundMax-_BoundMin)/(0.1*Random3DTo1D(curpos,_LightMarchCount,_WorldSpaceLightPos0)*length(curpos));//每个点步进的长度不应该是一样的,和当前位置有一些关系,而用random是为了放大差异有一些xyz很相近

                       float3 TDUV =(curpos/length(_BoundMax-_BoundMin)+_MoveSpeed*_Time.y)*_Tilling;                   

                       float Dcolor = tex3D(_NoiseTexC,TDUV).r;        
                        
                       float lightdepth = RayMarchToLight(Tolight,curpos,lightLength);

                       

                        Dcolor = step(1-_CloudDes,Dcolor); 
                                            

                         totalnum += Dcolor;    
                         totallight += lightdepth;
                        
                        }
                       
                       curpos += stepvec;
                       
                }
                
                return float2(totalnum/_RayMarchCount,totallight/_RayMarchCount);//x是深度,y是亮度
            }

          
            float2 rayBoxDst(float3 boundsMin ,float3 boundsMax,float3 rayOrigin,float3 invRaydir)
            {
                float3 t0 = (_BoundMin - rayOrigin) * invRaydir;//我个人觉得这里有点问题
                float3 t1 = (_BoundMax - rayOrigin) * invRaydir;
                float3 tmin = min(t0,t1);
                float3 tmax = max(t0,t1);

                float dstEn = max(max(tmin.x,tmin.y),tmin.z);
                float dstOu = min(min(tmax.x,tmax.y),tmax.z);

                float destToBox = max(0,dstEn);//
                float destInsideBox = max(0,dstOu-destToBox);

                return float2(destToBox,destInsideBox);
            
            }
            v2f vert (appdata v)
            {
               v2f o;

               o.pos = UnityObjectToClipPos(v.vertex);

               o.uv = v.uv;

               o.screenpos = ComputeScreenPos(v.vertex);


                float4 ndcPos = o.pos / o.pos.w;//ndc坐标变换公式

                float far = _ProjectionParams.z; //获取投影信息的z值,代表远平面距离

               

                float3 clipVec = float3(ndcPos.x , ndcPos.y * -1 , ndcPos.z * -1) * far; //裁切空间下的视锥顶点坐标//观察空间下Y轴从上往下 裁剪里Y由下往上 观察里Z朝向相机,也就是反向的//从左手坐标系转换到右手坐标系,我们需要交换 Y 轴和 Z 轴的方向

                o.viewVec = mul(unity_CameraInvProjection, clipVec.xyzz).xyz; //观察空间下的视锥向量           

                o.wpos = mul(unity_ObjectToWorld,v.vertex);

               return o;
            }

            float4 frag (v2f v) : SV_Target0
            {          
              float lineardepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,(v.pos.xy/_ScaledScreenParams.xy))); 

               
              float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,v.pos.xy/v.pos.w/_ScaledScreenParams);//采样深度图

              depth = Linear01Depth(depth);

              float3 viewPos = v.viewVec * depth;

             float3 worldpos = mul(unity_CameraToWorld,float4(viewPos,1)).xyz;         

              float raylength = length(v.wpos-_WorldSpaceCameraPos);
            
              float3 dir = normalize(v.wpos - _WorldSpaceCameraPos);

               float2 dst = rayBoxDst(_BoundMin,_BoundMax,_WorldSpaceCameraPos,1/dir);

               raylength = raylength - dst.x + dst.y;

               float d = RayMarchToLight(-_WorldSpaceLightPos0,v.wpos,80);
                          
              float2 cloudDns = RayMarch(dir,v.wpos,raylength,v);

              //cloudDns = clamp(cloudDns,0,1);
              //cloudDns.x = smoothstep(0.2,1,cloudDns.x);
              float4 NormalColor = lerp(_DarkColor,_Color,cloudDns.x)*_CloudIntensity;

             

              float alpha = step(0.2,cloudDns.x);

              float test = smoothstep(1,1-_CloudDes,cloudDns.y);//这个是光深度,越大越暗

              

              float4 LightColor = NormalColor/2 + _LightColor0*cloudDns.y*_LightIntensity;

              ///float LDepth = RayMarchToLight(-normalize(_WorldSpaceLightPos0),v.wpos,length(v.wpos-_WorldSpaceLightPos0));
              
              float3 finnalColor = lerp(NormalColor,LightColor,test);
                           

              return float4(finnalColor,alpha);
            }
            ENDCG
        }        
    }
     FallBack "Diffuse"
}
