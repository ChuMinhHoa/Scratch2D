using UnityEngine;
public enum ReviveType
{
    AddNote,
    AddSlot,
    BoosterMagnet
}
public class SlotRevive : SlotBase<ReviveType>
{
    public GameObject[] objRevive;

    public override void InitData(ReviveType data)
    {
        base.InitData(data);
        for (var i = 0; i < objRevive.Length; i++)
        {
            objRevive[i].SetActive(false);
        }
        objRevive[(int)slotData].SetActive(true);
    }
}
