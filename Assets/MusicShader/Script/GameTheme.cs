using UnityEngine;

public struct GameTheme{
	public Color color;
	public float lightPower;
	public GameTheme(Color initColor,float initLightPower){
		color = initColor;
		lightPower = initLightPower;
	}
	public bool checkSame(GameTheme check){
		if(check.color==this.color&&check.lightPower==this.lightPower){
			return true;
		}else{
			return false;
		}
	}
}