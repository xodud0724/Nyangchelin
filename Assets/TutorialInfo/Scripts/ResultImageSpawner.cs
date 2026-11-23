using UnityEngine;

public class ResultImageSpawner : MonoBehaviour
{
    public ResultImage[] resultImages;
    public Transform canvas;

    public void SpawnResultImage(TapGrade grade)
    {
        int index = (int)grade;
        Instantiate(resultImages[index], canvas, false);
    }
}
