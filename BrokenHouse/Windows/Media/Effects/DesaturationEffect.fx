//=========================================================================================
// 
// Desaturate Shader Effect
//
//=========================================================================================

float      amount               : register(C0);
sampler2D  implicitInputSampler : register(S0);

//--------------------------------------------------------------------------------------
// Actual function
//--------------------------------------------------------------------------------------
float4 main(float2 uv : TEXCOORD) : COLOR
{
  const float3 coeff  = float3(0.3, 0.59, 0.11);
  float4       colour = tex2D(implicitInputSampler, uv);
  
  // Do the desaturation
  colour.rgb = lerp(colour.rgb, dot(colour.rgb, coeff), amount);
 
  // Return the result
  return colour;
}
