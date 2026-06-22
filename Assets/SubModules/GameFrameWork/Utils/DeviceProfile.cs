using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace GFramework.Utils
{
    public enum ProfileRange
    {
        Unidentified,
        VeryLow,
        Low,
        Mid,
        High,
        VeryHigh,
        Unknow
    }
    public class DeviceProfile
    {
        public static ProfileRange GetDeviceProfile()
        {
#if UNITY_EDITOR
            return ProfileRange.VeryHigh;
#endif
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return GetDeviceProfileForiOS();
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                return GetDeviceProfileForAndroid();
            }
            return ProfileRange.Unknow;
        }

        private static ProfileRange GetDeviceProfileForiOS()
        {
#if UNITY_IOS
        if (Device.generation > DeviceGeneration.iPhone11)
        {
            return ProfileRange.VeryHigh;
        }

        if (Device.generation > DeviceGeneration.iPhoneX)
        {
            return ProfileRange.High;
        }

        if (Device.generation > DeviceGeneration.iPhone6)
        {
            return ProfileRange.Mid;
        }

        if (Device.generation > DeviceGeneration.iPhone5)
        {
            return ProfileRange.Low;
        }
        else
        {
            return ProfileRange.VeryLow;
        }
#endif

            return ProfileRange.Unknow;
        }

        private static ProfileRange GetDeviceProfileForAndroid()
        {
            HardwareInfo.instance.CalculateHardwareScore();
            var score = HardwareInfo.instance.userHardwareScore;
            return GetDeviceProfileByScore(score);
        }

        public static ProfileRange GetDeviceProfileByScore(float score)
        {
            if (score > 250)
            {
                return ProfileRange.VeryHigh;
            }

            if (score > 170)
            {
                return ProfileRange.High;
            }

            if (score > 150)
            {
                return ProfileRange.Mid;
            }

            if (score > 120)
            {
                return ProfileRange.Low;
            }

            return ProfileRange.VeryLow;
        }

        public static void SetQualitySetting(ProfileRange profileRange)
        {
            var currentLightShadows = LightShadows.Soft;

            switch (profileRange)
            {
                case ProfileRange.VeryHigh:
                    {
                        QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                        QualitySettings.shadows = ShadowQuality.All;
                        QualitySettings.antiAliasing = 8;
                        QualitySettings.skinWeights = SkinWeights.Unlimited;
                        currentLightShadows = LightShadows.Soft;
                        break;
                    }

                case ProfileRange.High:
                    {
                        QualitySettings.shadowResolution = ShadowResolution.Medium;
                        QualitySettings.shadows = ShadowQuality.HardOnly;
                        QualitySettings.antiAliasing = 2;
                        QualitySettings.skinWeights = SkinWeights.FourBones;
                        currentLightShadows = LightShadows.Soft;
                        break;
                    }

                case ProfileRange.Mid:
                    {
                        QualitySettings.shadowResolution = ShadowResolution.Low;
                        QualitySettings.shadows = ShadowQuality.HardOnly;
                        QualitySettings.antiAliasing = 0;
                        QualitySettings.skinWeights = SkinWeights.FourBones;
                        currentLightShadows = LightShadows.Hard;
                        break;
                    }

                case ProfileRange.Low:
                    {
                        QualitySettings.shadowResolution = ShadowResolution.Low;
                        QualitySettings.shadows = ShadowQuality.Disable;
                        QualitySettings.antiAliasing = 0;
                        QualitySettings.skinWeights = SkinWeights.TwoBones;
                        currentLightShadows = LightShadows.None;
                        break;
                    }

                case ProfileRange.VeryLow:
                    {
                        QualitySettings.shadowResolution = ShadowResolution.Low;
                        QualitySettings.shadows = ShadowQuality.Disable;
                        QualitySettings.antiAliasing = 0;
                        QualitySettings.skinWeights = SkinWeights.OneBone;
                        currentLightShadows = LightShadows.None;
                        break;
                    }
            }

            QualitySettings.vSyncCount = 0;

            var lights = GameObject.FindObjectsOfType<Light>() as Light[];
            foreach (Light l in lights) {
                l.shadows = currentLightShadows;
            }
        }
    }
}
