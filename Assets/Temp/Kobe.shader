Shader "Kobe/Shader"
{
    Properties
    {//Unity界面窗口中可操控的部分
        _MainTex("MainTexture",2D) = "white"{}
        _Angle("Rotation",Range(1,180)) = 1
        _NextTex("NextPage",2D) = "white"{}
    }
    SubShader
    {
        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct a2v// 用来命名传递从应用程序(即Application)到顶点着色器(即Vertex shader)的数据结构
            {
                float4 pos : POSITION; //定义的一个四维的向量,来取物体的坐标
                float2 uv : TEXCOORD0; //定义的UV贴图
            };
            struct v2f//代表从顶点着色器(vertex shader)到片元着色器(fragment shader)的传递数据
            {
                float4 svpos : SV_POSITION;
                float2 svuv :  TEXCOORD0;
            };
            sampler2D _NextTex;

            v2f vert(a2v i){
                
                v2f o;
    
                o.svpos = UnityObjectToClipPos(i.pos);//渲染uv贴图
                o.svuv = i.uv;
                return o;
            }
            float4 frag(v2f u) : SV_TARGET  
            {
                float4 tex = tex2D(_NextTex,u.svuv);
                return tex;
            }
            ENDCG
        }
        Pass
        {
            Cull Off//取消剔除渲染背面
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct a2v//用来命名传递从应用程序(即Application)到顶点着色器(即Vertex shader)的数据结构
            {
                float4 pos : POSITION; //定义的一个四维的向量,来取物体的坐标
                float2 uv : TEXCOORD0; //定义的UV贴图
            };
            struct v2f//代表从顶点着色器(vertex shader)到片元着色器(fragment shader)的传递数据
            {
                float4 svpos : SV_POSITION;
                float2 svuv :  TEXCOORD0;
            };
            sampler2D _MainTex;
            float _Angle;

            v2f vert(a2v i){
                
                float sins;
                float coss;

                sincos(radians(_Angle),sins,coss);
                //绕z轴旋转的矩阵
                float4x4 rotateMatrix = 
                {
                    coss,sins,0,0,
                    -sins,coss,0,0,
                    0,0,1,0,
                    0,0,0,1.
                };
                v2f o;
                i.pos -= float4(5,0,0,0);//偏移旋转轴
                i.pos.y = sin(i.pos.x * 0.5f) * sins;//弯曲书本
                i.pos = mul(rotateMatrix,i.pos);//旋转mesh
                i.pos += float4(5,0,0,0);//将mesh还原到原来的坐标上


                o.svpos = UnityObjectToClipPos(i.pos);//渲染uv贴图
                o.svuv = i.uv;
                return o;
            }
            float4 frag(v2f u) : SV_TARGET0
            {
                float4 tex = tex2D(_MainTex,u.svuv);
                return tex;
            }
            ENDCG
        }
        
    }
}
