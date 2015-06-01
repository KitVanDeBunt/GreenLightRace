using System;
using UnityEngine;

public static class ShaderUtil
{
	public static Vector4 FloatToRGBA(float input){
		int distanceInt = (int)(input*10000000);
		
		int inputFloatInInt = distanceInt.GetHashCode();
		byte[] inputBytes = new byte[4];
		inputBytes[0] = (byte)inputFloatInInt;
		inputBytes[1] = (byte)(inputFloatInInt >> 8);
		inputBytes[2] = (byte)(inputFloatInInt >> 16);
		inputBytes[3] = (byte)(inputFloatInInt >> 24);
		Vector4 distByteScale = new Vector4 ();
		distByteScale.x = (float)inputBytes[0]/255f;
		distByteScale.y = (float)inputBytes[1]/255f;
		distByteScale.z = (float)inputBytes[2]/255f;
		distByteScale.w = (float)inputBytes[3]/255f;
		return distByteScale;
	}

	public static void WriteFloatToTexturePixel(float input,ref Texture2D texture,int x,int y){
		Vector4 rgbaInput = FloatToRGBA (input);
		texture.SetPixel (x, y, new Color(rgbaInput.x, rgbaInput.y, rgbaInput.z,rgbaInput.w));
	}
}

