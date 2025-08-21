using UnityEngine;

[System.Serializable]
public class Quest
{
    [Header("bool")]
    public bool accepted;
    public bool denied;
    public bool initialDialogCompleted;
    public bool isCompleted;

    public bool hasNoRequirement;

    [Header("Quest Info")]
    public QuestInfo info;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/QuestInfo", order = 1)]  
public class QuestInfo : ScriptableObject
{


}