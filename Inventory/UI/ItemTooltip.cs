using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public Text itemNameText;

    public void UpdateItemName(ItemName itemName)
    {
        itemNameText.text = itemName switch
        {
            ItemName.Key => "Ô¿³×",
            ItemName.paper1 => "Ö½ºýËéÆ¬",
            ItemName.paper2 => "Ö½ºýËéÆ¬",
            ItemName.paper3 => "Ö½ºýËéÆ¬",
            ItemName.paper4 => "Ö½ºýËéÆ¬",
            ItemName.door_key => "Ô¿³×",
            _ => ""

        };
    }
}
