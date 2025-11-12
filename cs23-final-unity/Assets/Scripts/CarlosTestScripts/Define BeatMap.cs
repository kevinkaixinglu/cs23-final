using UnityEngine;

[System.Serializable]
public class Beat
{
    public int[] notes = new int[4]; // Elements must range from 0-4
}

[System.Serializable]
public class Measure
{
    public Beat[] beats = new Beat[4];
}
