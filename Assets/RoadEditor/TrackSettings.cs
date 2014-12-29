using UnityEngine;

[System.Serializable]
public class TrackSettings{
	public string name = "road";
#if UNITY_EDITOR
	public IconManager.LabelIcon labelType;
#endif
	
	public Material trackMaterial;
	public float pointsPerUnityUnit = 1;
	public int widthDetail = 1;
	
	public bool wallRight = true;
	public float wallRightHeight  = 5;
	public bool wallLeft = true;
	public float WallLefttHeight  = 5;
	
	public bool smoothSphereCollision = true;
	
	[HideInInspector]
	[SerializeField]
	private TrackSettings save;
	
	public bool CheckUpdated(){
		if(save == null){
			save = new TrackSettings();
			return true;
		}
		if(name 				!= save.name){
			return true;
		}
#if UNITY_EDITOR
		if(labelType 			!= save.labelType){
			return true;
		}
#endif
		if(trackMaterial			!= save.trackMaterial){
			return true;
		}
		if(pointsPerUnityUnit		!= save.pointsPerUnityUnit){
			return true;
		}
		if(widthDetail 				!= save.widthDetail){
			return true;
		}
		if(wallRight 				!= save.wallRight){
			return true;
		}
		if(wallRightHeight 			!= save.wallRightHeight){
			return true;
		}
		if(wallLeft 				!= save.wallLeft){
			return true;
		}
		if(WallLefttHeight 			!= save.WallLefttHeight){
			return true;
		}
		if(smoothSphereCollision	!= save.smoothSphereCollision){
			return true;
		}
		return false;
	}
	
	public void Save(){
		save.name = name;
#if UNITY_EDITOR
		save.labelType = labelType;
#endif
		save.trackMaterial = trackMaterial;
		save.pointsPerUnityUnit = pointsPerUnityUnit;
		save.widthDetail = widthDetail;
		save.wallRight = wallRight;
		save.wallRightHeight = wallRightHeight;
		save.wallLeft = wallLeft;
		save.WallLefttHeight = WallLefttHeight;
		save.smoothSphereCollision = smoothSphereCollision;
	}
}