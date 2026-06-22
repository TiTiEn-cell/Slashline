using System.Collections;
using UnityEngine;

public class RatingService : SingletonMono<RatingService>
{
#if UNITY_ANDROID
    private Google.Play.Review.ReviewManager reviewManager;
    private Google.Play.Review.PlayReviewInfo playReviewInfo;
#endif

    public void Rating(string appId)
    {
#if UNITY_EDITOR
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
        return;
#endif

#if UNITY_ANDROID
        StartCoroutine(RequestReview());
#elif UNITY_IPHONE || UNITY_IOS
        if (!UnityEngine.iOS.Device.RequestStoreReview()) {
            Application.OpenURL("https://apps.apple.com/app/id" + appId);
        }
#endif
    }

    private IEnumerator RequestReview()
    {
#if UNITY_ANDROID
        reviewManager = new Google.Play.Review.ReviewManager();
        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != Google.Play.Review.ReviewErrorCode.NoError) {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        playReviewInfo = requestFlowOperation.GetResult();
        var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchFlowOperation;
        playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != Google.Play.Review.ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
#else
        yield return null;
#endif
    }
}
