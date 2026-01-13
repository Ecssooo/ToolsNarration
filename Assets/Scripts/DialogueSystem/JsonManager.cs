using UnityEngine;

public class JsonManager : MonoBehaviour
{
    public static Graph JsonToDialogueData(string jsonString)
    {
        return JsonUtility.FromJson<Graph>(jsonString);
    }
}
