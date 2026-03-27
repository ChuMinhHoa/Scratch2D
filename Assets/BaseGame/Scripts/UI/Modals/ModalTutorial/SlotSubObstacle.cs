using UnityEngine;

public class SlotSubObstacle : SlotBase<Sprite>
{
    public override void InitData(Sprite data)
    {
        base.InitData(data);
        imgIcon.sprite = data;
    }
}
