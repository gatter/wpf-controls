//=========================================================================================
// 
// Gamma correction Shader Effect
//
//=========================================================================================

float      gamma                : register(C0);
sampler2D  implicitInputSampler : register(S0);

//--------------------------------------------------------------------------------------
// Actual function
//--------------------------------------------------------------------------------------
float4 main(float2 uv : TEXCOORD) : COLOR
{
  float4       inverse = 1.0 / gamma;
  float4       rescale = 1.0 / pow(1.0, inverse);
  float4       colour = tex2D(implicitInputSampler, uv);
  float4       result;
    
  // Do the gamma
  rescale.a = 1.0;
  inverse.a = 1.0;
  result = pow(colour, inverse);// * rescale;
 
  // Return the result
  return result;
}
