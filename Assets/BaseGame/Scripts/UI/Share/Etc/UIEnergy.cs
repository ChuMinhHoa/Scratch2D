using UnityEngine;

public class UIEnergy : UIResource
{
}

public class ActionCallOnResource
{
    public virtual void ActionCallOnUIResource()
    {
    }
}

public class ActionCallOnEnergy : ActionCallOnResource
{
    public override void ActionCallOnUIResource()
    {
        base.ActionCallOnUIResource();
        Debug.Log("Action call on energy");
    }
}

public class ActionCallOnMoney : ActionCallOnResource
{
    public override void ActionCallOnUIResource()
    {
        base.ActionCallOnUIResource();
        Debug.Log("Action call on money");
    }
}
