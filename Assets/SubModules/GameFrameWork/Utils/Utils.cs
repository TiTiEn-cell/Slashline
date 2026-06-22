using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GFramework.Utils
{
    public static class Utils
    {
        public static void SetLayer(this GameObject go, string layerName)
        {
            go.layer = LayerMask.NameToLayer(layerName);
            var tran = go.transform;
            var childCount = tran.childCount;
            if (childCount > 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    SetLayer(tran.GetChild(i).gameObject, layerName);
                }
            }
        }

        public static void SetSpriteRendererSortingLayer(this GameObject go, int depth)
        {
            var spriteRenderers = go.GetComponentsInChildren<SpriteRenderer>();
            if (spriteRenderers != null && spriteRenderers.Length > 0)
            {
                foreach (var item in spriteRenderers)
                {
                    item.sortingOrder += depth;
                }
            }
        }

        public static List<Transform> GetAllChilds(this Transform _t)
        {
            List<Transform> ts = new List<Transform>();

            foreach (Transform t in _t)
            {
                ts.Add(t);
                if (t.childCount > 0)
                    ts.AddRange(GetAllChilds(t));
            }

            return ts;
        }

        public static float CalculatePathDistance(Vector3[] path)
        {
            float totalDistance = 0f;

            for (int i = 0; i < path.Length - 1; i++)
            {
                totalDistance += Vector3.Distance(path[i], path[i + 1]);
            }

            return totalDistance;
        }

        public static float CalculateABDistance(Vector3 poit_a, Vector3 point_b)
        {
            return Vector3.Distance(poit_a, point_b);
        }

        public static float CalculateTravelTime(float distance, float speed)
        {
            if (speed <= 0)
            {
                Debug.LogError("Speed must be greater than zero.");
                return 0;
            }

            return distance / speed;
        }

        public static Vector3[] TransformListToVector3Array(List<Transform> transformList)
        {
            Vector3[] positionArray = new Vector3[transformList.Count];
            for (int i = 0; i < transformList.Count; i++)
            {
                positionArray[i] = transformList[i].position;
            }
            return positionArray;
        }

        public static Vector3 ProjectPointOntoLine3D(Vector3 A, Vector3 B, Vector3 C)
        {
            Vector3 AB = B - A;
            Vector3 AC = C - A;
            float scalar_projection = Vector3.Dot(AC, AB) / AB.sqrMagnitude;
            return A + scalar_projection * AB;
        }

        public static Vector3 OffsetPointInLine(Vector3 A, Vector3 B, float offset)
        {
            // offset B toward A
            Vector3 AB = B - A;
            float AB_length = AB.magnitude;
            return B - (AB / AB_length) * offset;
        }

        public static string GetGoogleSheetLink(string idsheet, string idgid)
        {
            string url = string.Format("https://docs.google.com/spreadsheets/d/{0}/export?format=csv&id={0}&gid={1}", idsheet, idgid);
            return url;
        }

        public static bool IsPointerOverEventSystemObject()
        {
#if UNITY_EDITOR
            return EventSystem.current.IsPointerOverGameObject();
#endif
            if (Input.touchCount <= 0) return false;
            return EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId);
        }

        public static void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = UnityEngine.Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        public static List<Vector3> GenerateRandomPositions(Vector3 centerPoin, float radius, int numberOfPoints)
        {
            List<Vector3> positions = new List<Vector3>();
            HashSet<Vector2> usedPositions = new HashSet<Vector2>();

            while (positions.Count < numberOfPoints)
            {
                float angle = UnityEngine.Random.Range(0f, Mathf.PI * 5);
                float distance = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f)) * radius;

                Vector2 position2D = new Vector2(
                    Mathf.Cos(angle) * distance,
                    Mathf.Sin(angle) * distance
                );

                // Round to 2 decimal places to avoid floating-point precision issues
                position2D = new Vector2(
                    Mathf.Round(position2D.x * 100f) / 100f,
                    Mathf.Round(position2D.y * 100f) / 100f
                );

                if (!usedPositions.Contains(position2D))
                {
                    usedPositions.Add(position2D);
                    Vector3 position3D = centerPoin + new Vector3(position2D.x, position2D.y, 0);
                    positions.Add(position3D);
                }
            }
            return positions;
        }



        public static Quaternion ConvertFloatToQuaternion_yAxis(float y_rotationInDegrees)
        {
            return Quaternion.Euler(0f, y_rotationInDegrees, 0f);
        }

        public static float GetAngleInRadianFromVector3(Vector3 direction)
        {
            return Mathf.Atan2(direction.z, direction.x);
        }

        public static float GetAngleIndegreeFromVector3(Vector3 direction)
        {
            return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        }

        public static Vector3 ConvertToScreenPoint(Canvas canvas, RectTransform rectTransform)
        {
            return RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);
        }

        public static Vector3 ConvertToUIPoint(Canvas canvas, Vector3 screenPoint)
        {
            RectTransform rectTransform = canvas.GetComponent<RectTransform>();
            Vector3 worldPoint;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform,
                screenPoint,
                canvas.worldCamera,
                out worldPoint);

            return worldPoint;
        }

        public static string GetMaxNameStr(string name, int length)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "";
            }

            if (name.Length < length)
            {
                return name;
            }

            var str = "";
            var count = 0;
            var maxLength = false;

            foreach (var item in name)
            {
                if (count > length - 3)
                {
                    maxLength = true;
                    break;
                }

                str += item;
                count += 1;
            }

            if (maxLength)
            {
                str += "...";
            }

            return str;
        }

        #region  Time
        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetFirstDayOfWeek(dayInWeek, defaultCultureInfo);
        }

        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek, DayOfWeek firstDay)
        {
            DateTime firstDayInWeek = dayInWeek.Date;

            while (firstDayInWeek.DayOfWeek != firstDay)
            {
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            }

            return firstDayInWeek;
        }

        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek, string startDay)
        {
            DayOfWeek firstDay = ParseEnum<DayOfWeek>(startDay);
            return GetFirstDayOfWeek(dayInWeek, firstDay);
        }

        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            return GetFirstDayOfWeek(dayInWeek, firstDay);
        }

        public static DateTime GetLastDayOfWeek(DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetLastDayOfWeek(dayInWeek, defaultCultureInfo);
        }

        public static DateTime GetLastDayOfWeek(DateTime dayInWeek, DayOfWeek firstDay)
        {
            DateTime firstDayInWeek = GetFirstDayOfWeek(dayInWeek, firstDay);
            return firstDayInWeek.AddDays(7);
        }

        public static DateTime GetLastDayOfWeek(DateTime dayInWeek, string startDay)
        {
            DateTime firstDayInWeek = GetFirstDayOfWeek(dayInWeek, startDay);
            return firstDayInWeek.AddDays(7);
        }

        public static DateTime GetLastDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DateTime firstDayInWeek = GetFirstDayOfWeek(dayInWeek, cultureInfo);
            return firstDayInWeek.AddDays(7);
        }

        public static Tuple<DateTime, DateTime> CampusVueDateRange(DateTime dayInWeek, string startDay)
        {
            DateTime firstDayOfWeek = GetFirstDayOfWeek(dayInWeek, startDay).AddSeconds(1);
            DateTime lastDayOfWeek = GetLastDayOfWeek(dayInWeek, startDay);

            return new Tuple<DateTime, DateTime>(firstDayOfWeek, lastDayOfWeek);
        }
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        #endregion
    }
}
