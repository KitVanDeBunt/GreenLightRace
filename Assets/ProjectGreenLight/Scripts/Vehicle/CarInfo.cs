using UnityEngine;

enum CarType
{
    self,
    other,
    aI
}

public enum CarID
{
    joppeHotrod,
    maartenNucleoid,
    thomasCar
}

public class CarInfo : MonoBehaviour
{
    public Transform follow;
}
