#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

public class ATTService : SingletonMono<ATTService>
{
    public void Init()
    {
#if UNITY_IOS
        if(ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
    }
}
