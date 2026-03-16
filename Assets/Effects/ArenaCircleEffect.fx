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

float4 ApplyArenaTexture(float4 color, float factor)
{
    float2 uv = color.xy / uScreenResolution;
    float2 texOffset = float2(uTime * 0.1, uTime * 0.1);
    uv = frac(uv + texOffset);

    float4 overlay = tex2D(uImage1, uv);
    float3 result = lerp(color.rgb, overlay.rgb, factor * uOpacity);

    return float4(result, color.a);
}

float4 FilterArenaCircle(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pCoords = coords * uScreenResolution;
    float2 pTarget = uTargetPosition - uScreenPosition;
    float dist = distance(pCoords, pTarget);

    float outsideFactor = step(uIntensity, dist); // 1 if outside, 0 inside
    float4 originalColor = tex2D(uImage0, coords);

    float4 processedColor = ApplyArenaTexture(originalColor, outsideFactor);

    return processedColor;
}

technique ArenaCircle
{
    pass ArenaCirclePass
    {
        PixelShader = compile ps_2_0 FilterArenaCircle();
    }
}