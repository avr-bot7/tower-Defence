using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [Header("Buttons")]
    public TextMeshProUGUI noAdsPrice;
    public TextMeshProUGUI gold1Price;
    public TextMeshProUGUI gold2Price;
    public TextMeshProUGUI gold3Price;
    public TextMeshProUGUI skinPrice;

    public GameObject noAdsButton;

    void OnEnable()
    {
        if (IAPManager.I == null) return;

        if (noAdsPrice != null) noAdsPrice.text = IAPManager.I.GetPrice(IAPManager.NO_ADS);
        if (gold1Price != null) gold1Price.text = IAPManager.I.GetPrice(IAPManager.GOLD_PACK_1);
        if (gold2Price != null) gold2Price.text = IAPManager.I.GetPrice(IAPManager.GOLD_PACK_2);
        if (gold3Price != null) gold3Price.text = IAPManager.I.GetPrice(IAPManager.GOLD_PACK_3);
        if (skinPrice != null) skinPrice.text = IAPManager.I.GetPrice(IAPManager.TOWER_SKIN);

        if (noAdsButton != null)
            noAdsButton.SetActive(!IAPManager.I.IsNoAdsPurchased());
    }

    public void OnNoAds() => IAPManager.I?.BuyNoAds();
    public void OnGold1() => IAPManager.I?.BuyGoldPack1();
    public void OnGold2() => IAPManager.I?.BuyGoldPack2();
    public void OnGold3() => IAPManager.I?.BuyGoldPack3();
    public void OnKrishnaSkin() => IAPManager.I?.BuyKrishnaSkin();
    public void OnRestore() => IAPManager.I?.RestorePurchases();
}
