using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Good
{
    public static class Transform3D
    {
        public static Vector3 SetX(float x, Transform t)
        {
            return new Vector3(x, t.position.y);
        }

        public static Vector3 SetY(float y, Transform t)
        {
            return new Vector3(t.position.x, y);
        }
    }

    public static class MathG
    {
        // 전체값의 몇 퍼센트는 얼마인가
        public static float GetPercent1(float max, float percent)
        {
            return max * percent / max;
        }

        // 전체값에서 일부값이 몇 퍼센트인지
        public static float GetPercent2(float max, float cur)
        {
            return cur * max / max;
        }

        // 가까운 오브젝트 찾기
        public static GameObject FindNearTarget(string tag, GameObject myObj)
        {
            List<GameObject> objects = new List<GameObject>(GameObject.FindGameObjectsWithTag(tag));
            if (objects.Count == 0)
            {
                return null;
            }
            if (tag == myObj.tag && objects.Count > 1)
                objects.Remove(myObj);

            GameObject nearTarget = objects[0];
            float nearDistance = Vector3.Distance(myObj.transform.position, nearTarget.transform.position);
            foreach (var obj in objects)
            {
                float distance = Vector3.Distance(myObj.transform.position, obj.transform.position);
                if (distance < nearDistance)
                {
                    nearTarget = obj;
                    nearDistance = distance;
                }
            }
            return nearTarget;
        }
    }
}
