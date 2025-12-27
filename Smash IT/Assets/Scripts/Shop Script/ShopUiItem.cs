using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopUiItem : MonoBehaviour
{
    public ShopItem item;

    public TMP_Text ownedText;
    public Button buyButton;

    private void Start()
    {
        buyButton.onClick.AddListener(OnBuyClicked);
        Refresh();
    }

    public void Refresh()
    {
        int owned = ShopManager.Instance.GetConsumableCount(item.powerUpType);
        ownedText.text = owned.ToString();
    }

    private void OnBuyClicked()
    {
        ShopManager.Instance.BuyItem(item, 1);
        Refresh();
    }

    public void UpdateOwnedText(int value)
    {
        ownedText.text = value.ToString();
    }
}
