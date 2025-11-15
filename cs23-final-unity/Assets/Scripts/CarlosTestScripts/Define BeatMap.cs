using UnityEngine;

[System.Serializable]
public class QNote
{
    public int[] sNotes = new int[4]; // Elements must range from 0-4
}

[System.Serializable]
public class Measure
{
    public QNote[] qNotes = new QNote[4];
}
