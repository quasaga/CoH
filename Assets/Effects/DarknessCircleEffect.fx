sampler uImage0 : register(s0); // The contents of the screen.
sampler uImage1 : register(s1); // Up to three extra textures you can use for various purposes (for instance as an overlay).
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution; // Resolution in pixels
float2 uScreenPosition; // The position of the camera.
float2 uTargetPosition; // The "target" of the shader
float2 uDirection; //
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect; // Doesn't seem to be used, but included for parity.
float2 uZoom;

float4 FilterDarkness(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 pCoords = coords * uScreenResolution;
    float2 pTarget = uTargetPosition - uScreenPosition;
    float radius = uIntensity * uProgress;
    float innerRadius = uDirection.x;
    float dist = distance(pCoords, pTarget);
    float factor = saturate((dist - innerRadius) / (radius - innerRadius));

    color.rgb = lerp(color.rgb, uColor, factor * uOpacity);

    return color;
}

technique DarknessCircle
{
    pass DarknessCirclePass
    {
        PixelShader = compile ps_2_0 FilterDarkness();
    }
}