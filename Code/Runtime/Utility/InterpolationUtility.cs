using System;
using System.Collections;
using UnityEngine;

namespace ShizoGames.UGUIExtended.Utility
{
    internal static class InterpolationUtility
    {
        public static Coroutine Interpolate(MonoBehaviour coroutineRunner, Action<Color> action, Color start, Color end, 
            float duration, AnimationCurve curve = null)
        {
            return coroutineRunner.StartCoroutine(InterpolateCoroutine(action, start, end, duration, curve));
        }
        
        public static Coroutine Interpolate(MonoBehaviour coroutineRunner, Action<float> action, float start, float end, 
            float duration, AnimationCurve curve = null)
        {
            return coroutineRunner.StartCoroutine(InterpolateCoroutine(action, start, end, duration, curve));
        }
        
        private static IEnumerator InterpolateCoroutine(Action<Color> action, Color start, Color end, 
            float duration, AnimationCurve curve = null)
        {
            var elapsed = 0f;
            
            curve = curve ?? AnimationCurve.Linear(0, 0, 1, 1);
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                var t = curve.Evaluate(elapsed / duration);

                var value = Color.Lerp(start, end, t);
                
                action.Invoke(value);

                yield return null;
            }

            action.Invoke(end);
        }
        
        private static IEnumerator InterpolateCoroutine(Action<float> action, float start, float end, 
            float duration, AnimationCurve curve = null)
        {
            var elapsed = 0f;
            
            curve = curve ?? AnimationCurve.Linear(0, 0, 1, 1);
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                var t = curve.Evaluate(elapsed / duration);

                var value = Mathf.Lerp(start, end, t);
                
                action.Invoke(value);

                yield return null;
            }

            action.Invoke(end);
        }
    }
}