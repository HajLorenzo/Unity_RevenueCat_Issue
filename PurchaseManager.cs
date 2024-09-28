using UnityEngine;
using RevenueCat;

public class PurchaseManager : MonoBehaviour
{
    public static PurchaseManager Instance;
    private PurchasesListener purchasesListener;
    private Purchases purchases;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        purchases = GetComponent<Purchases>();
        purchasesListener = GetComponent<PurchasesListener>();
    }

    public void OnPurchaseButtonClick(string purchaseID)
    {
        print("Start purchase "+purchaseID);
        purchases.GetOfferings((offerings, error) =>
        {
            if (error != null)
            {
                GlobalManager.Instance.ShowStatus($"Error fetching offerings: {error.Message}",1);
                print($"Error fetching offerings: {error.Message}");
                GameAnalyticsManager.Instance.CustomEvent("PurchaseManager_Error_"+error.Message);
                return;
            }

            var currentOffering = offerings.Current;

            if (currentOffering != null)
            {
                Purchases.Package selectedPackage = null;

                print("start founding  in "+currentOffering.AvailablePackages.Count+" offering !");
                foreach (var package in currentOffering.AvailablePackages)
                {
                    if (package.Identifier == purchaseID)
                    {
                        print("founded purchase package !");
                        selectedPackage = package;
                        break;
                    }
                }

                if (selectedPackage != null)
                {
                    purchasesListener.BeginPurchase(selectedPackage);
                }
                else
                {
                    GameAnalyticsManager.Instance.CustomEvent($"No package found for product ID: {purchaseID}");
                    GlobalManager.Instance.ShowStatus($"No package found for product ID: {purchaseID}",1);
                }
            }
            else
            {
                GameAnalyticsManager.Instance.CustomEvent("No current offering found.");
                GlobalManager.Instance.ShowStatus("No current offering found.",1);
            }
        });
    }
}
