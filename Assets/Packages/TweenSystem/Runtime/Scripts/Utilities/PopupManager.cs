using UnityEngine;
using Tools.ObjectPoolSystem;
using PrimeTween;
using UnityEngine.InputSystem;
using Tools.TweenSystem.Elements;

namespace Tools.TweenSystem.Utilities
{
	public static class PopupManager
	{
        /// <summary>
        /// Create floating element at mouse location and play tween
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="tween"></param>
        public static void Create<T>(GameObject target, System.Func<T, Tween> tween) where T : UIElement
        {
            T element = GetElement(target, Mouse.current.position.ReadValue()) as T;

            tween(element).OnComplete(element.gameObject, onComplete: static target => PoolManager.Release(target.gameObject, PoolManager.PoolType.UI));
        }

        /// <summary>
        /// Create floating element at mouse location and play sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="tween"></param>
        public static void Create<T>(GameObject target, System.Func<T, AnimSequence> tween) where T : UIElement
        {
            T element = GetElement(target, Mouse.current.position.ReadValue()) as T;

            tween(element).OnComplete(element.gameObject, static target => PoolManager.Release(target, PoolManager.PoolType.UI));
        }

        /// <summary>
        /// Create floating element at given location and play tween
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="position">Element creation location</param>
        /// <param name="tween"></param>
        public static void Create<T>(GameObject target, Vector3 position, System.Func<T, Tween> tween) where T : UIElement 
		{
            T element = GetElement(target, position) as T;

            tween(element).OnComplete(element.gameObject, onComplete: static target => PoolManager.Release(target, PoolManager.PoolType.UI));
        }

        /// <summary>
        /// Create floating element at given location and play sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="position">Element creation location</param>
        /// <param name="tween"></param>
        public static void Create<T>(GameObject target, Vector3 position, System.Func<T, AnimSequence> tween) where T : UIElement
        {
            T element = GetElement(target, position) as T;

            tween(element).OnComplete(element.gameObject, static target => PoolManager.Release(target, PoolManager.PoolType.UI));
        }

        private static UIElement GetElement(GameObject target, Vector3 position)
        {
            if (!PoolManager.HasPool(target))
            {
                PoolManager.CreatePool(target, target.transform.position, Quaternion.identity, PoolManager.PoolType.UI);
            }

            GameObject obj = PoolManager.SpawnObject(target, position, Quaternion.identity, PoolManager.PoolType.UI);

            UIElement targetElement = obj.GetComponent<UIElement>();

            return targetElement;
        }
    } 
}
