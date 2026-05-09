using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour, IDetailedStoreListener
#else
public class IAPManager : MonoBehaviour
#endif
{
    public static IAPManager I;

    public const string NO_ADS = "com.yourstudio.asuradefence.noads";
    public const string GOLD_PACK_1 = "com.yourstudio.asuradefence.gold500";
    public const string GOLD_PACK_2 = "com.yourstudio.asuradefence.gold1200";
    public const string GOLD_PACK_3 = "com.yourstudio.asuradefence.gold3000";
    public const string TOWER_SKIN = "com.yourstudio.asuradefence.skin_krishna";

#if UNITY_PURCHASING
    IStoreController _store;
    IExtensionProvider _extensions;
#endif

    void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_PURCHASING
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(NO_ADS, ProductType.NonConsumable);
        builder.AddProduct(GOLD_PACK_1, ProductType.Consumable);
        builder.AddProduct(GOLD_PACK_2, ProductType.Consumable);
        builder.AddProduct(GOLD_PACK_3, ProductType.Consumable);
        builder.AddProduct(TOWER_SKIN, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
#else
        Debug.Log("Unity IAP package not installed. IAP is disabled in this build.");
#endif
    }

    public void BuyNoAds() => InitiatePurchase(NO_ADS);
    public void BuyGoldPack1() => InitiatePurchase(GOLD_PACK_1);
    public void BuyGoldPack2() => InitiatePurchase(GOLD_PACK_2);
    public void BuyGoldPack3() => InitiatePurchase(GOLD_PACK_3);
    public void BuyKrishnaSkin() => InitiatePurchase(TOWER_SKIN);

    public void RestorePurchases()
    {
#if UNITY_PURCHASING
#if UNITY_IOS
        _extensions?.GetExtension<IAppleExtensions>()?.RestoreTransactions(_ => { });
#endif
#endif
    }

    public bool IsNoAdsPurchased() => PlayerPrefs.GetInt("NoAds", 0) == 1;

    public string GetPrice(string productId)
    {
#if UNITY_PURCHASING
        if (_store == null) return "...";
        var product = _store.products.WithID(productId);
        return product != null ? product.metadata.localizedPriceString : "N/A";
#else
        return "N/A";
#endif
    }

    void InitiatePurchase(string id)
    {
#if UNITY_PURCHASING
        _store?.InitiatePurchase(id);
#else
        Debug.Log($"IAP disabled. Cannot purchase {id}");
#endif
    }

#if UNITY_PURCHASING
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _store = controller;
        _extensions = extensions;
        Debug.Log("IAP initialised");

        if (IsNoAdsPurchased() && AdsManager.I != null)
            AdsManager.I.HideBanner();
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string id = args.purchasedProduct.definition.id;

        if (id == NO_ADS)
        {
            PlayerPrefs.SetInt("NoAds", 1);
            PlayerPrefs.Save();
            if (AdsManager.I != null) AdsManager.I.HideBanner();
        }
        else if (id == GOLD_PACK_1) GameManager.I.AddGold(500);
        else if (id == GOLD_PACK_2) GameManager.I.AddGold(1200);
        else if (id == GOLD_PACK_3) GameManager.I.AddGold(3000);
        else if (id == TOWER_SKIN)
        {
            PlayerPrefs.SetInt("Skin_Krishna", 1);
            PlayerPrefs.Save();
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription description)
    {
        Debug.LogWarning($"Purchase failed: {product.definition.id} - {description.reason}");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogWarning($"IAP init failed: {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogWarning($"IAP init failed: {error} - {message}");
    }
#endif
}
