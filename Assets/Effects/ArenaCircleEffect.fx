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

float4 ApplyArenaTexture(float4 color, float2 coords, float dist, float factor)
{
    // UV for moving Voronoi texture
    float2 uv = coords;
    float2 texOffset = float2(uTime, uTime);
    uv = frac(uv + texOffset);

    float overlayValue = tex2D(uImage1, uv).r; // grayscale texture
    float3 coloredOverlay = uColor * overlayValue;

    float rimWidth = 100;
    float rim = saturate(1.0 - (dist - uIntensity) / rimWidth); 

    coloredOverlay *= (1.0 - rim); // Darken overlay near rim

    float3 result = lerp(color.rgb, coloredOverlay, factor * uOpacity);

    return float4(result, color.a);
}

float4 FilterArenaCircle(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pCoords = coords * uScreenResolution;
    float2 pTarget = uTargetPosition - uScreenPosition;
    float dist = distance(pCoords, pTarget);

    float factor = step(uIntensity, dist); // 1 if outside, 0 inside
    float4 color = tex2D(uImage0, coords);

    float4 processedColor = ApplyArenaTexture(color, coords, dist, factor);

    return processedColor;
}

technique ArenaCircle
{
    pass ArenaCirclePass
    {
        PixelShader = compile ps_2_0 FilterArenaCircle();
    }
}