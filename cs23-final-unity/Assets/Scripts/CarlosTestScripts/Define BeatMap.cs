using UnityEngine;

[System.Serializable]
public class Beat
{
    public bool[] notes = new bool[4];
}

[System.Serializable]
public class Measure
{
    public Beat[] beats = new Beat[4];
}
