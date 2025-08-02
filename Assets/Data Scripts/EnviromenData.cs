using System.Collections.Generic;

[System.Serializable]
public class EnviromenData
{
    public List<string> pickupItems;

    public EnviromenData(List<string> _pickupItems)
    {
        pickupItems = _pickupItems;
    }
}