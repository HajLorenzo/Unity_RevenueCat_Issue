using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PurchasesListener : Purchases.UpdatedCustomerInfoListener
{
    private Purchases purchases;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        purchases=GetComponent<Purchases>();
    }
    public override void CustomerInfoReceived(Purchases.CustomerInfo customerInfo)
    {
        // display new CustomerInfo
    }

    private async void Start()
    {
        await Task.Delay(50);
        purchases.SetDebugLogsEnabled(true);
        purchases.GetOfferings((offerings, error) =>
        {
            if (error != null)
            {
                Debug.LogError($"Error fetching offerings: {error.Message}");
            }
            else if (offerings.Current != null)
            {
                // You can now show available packages to users
                Debug.Log("Offerings loaded successfully");

                // For example, log all available packages in the current offering
                foreach (var package in offerings.Current.AvailablePackages)
                {
                    Debug.Log($"Package available: {package.ToString()}");
                }
            }
            else
            {
                // No offerings available, handle appropriately
                Debug.LogWarning("No current offering found.");
            }
        });
    }


    public void BeginPurchase(Purchases.Package package)
    {
        purchases.PurchasePackage(package, (productIdentifier, customerInfo, userCancelled, error) =>
        {
            if (error != null && !userCancelled)
            {
                Debug.LogError($"Error during purchase: {error.Message}");
            }
            else if (!userCancelled)
            {
                Debug.Log("Purchase successful! Updated Customer Info received.");
                CustomerInfoReceived(customerInfo);
            }
            else
            {
                Debug.Log("Purchase was cancelled by the user.");
            }
        });
    }

    private void RestoreClicked()
    {
        purchases.RestorePurchases((customerInfo, error) =>
        {
            if (error != null)
            {
                Debug.LogError($"Error restoring purchases: {error.Message}");
            }
            else
            {
                Debug.Log("Restored purchases successfully.");
                CustomerInfoReceived(customerInfo);
            }
        });
    }
}