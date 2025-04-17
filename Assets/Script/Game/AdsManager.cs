using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsShowListener
{
    public static AdsManager instance;
    string gameId = "5836307";
    bool testMode = true;

    private Action onShowCompleted;
    private Action onShowFailed;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Advertisement.Initialize(gameId, testMode, this);
    }

    // call to show inters ads
    public void ShowInters()
    {
        Advertisement.Show("Interstitial_Android", this);
    }

    // call show reward using this function
    public void OnShowRewardVideo(Action onShowCompleted, Action onShowFailed)
    {
        this.onShowCompleted = onShowCompleted;
        this.onShowFailed = onShowFailed;
        Advertisement.Show("Rewarded_Android", this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Init Completed");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log("Init failed");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        onShowFailed?.Invoke();
    }

    public void OnUnityAdsShowStart(string placementId)
    {

    }

    public void OnUnityAdsShowClick(string placementId)
    {

    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Reward collected");
        onShowCompleted?.Invoke();
    }
}
