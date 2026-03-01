using UnityEngine;

public class SlotFolder : MonoBehaviour
{
    public FolderPos folderPos;

    public bool IsHaveObject()
    {
        return folderPos.IsHaveObj();
    }

    public void SetFolder(FolderHaveSticker folder)
    {
        folderPos.RegisterObj(folder);
    }
}
