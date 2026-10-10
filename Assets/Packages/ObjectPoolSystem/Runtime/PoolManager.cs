using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Tools.ObjectPoolSystem
{
    public class PoolManager : MonoBehaviour
    {
        [SerializeField] private bool addToDontDestroyOnLoad = false;

        [SerializeField] private int defaultCapacity = 20;
        [SerializeField] private int maxCapacity = 100;
        [SerializeField] private bool collectionCheck = false;

        private static int _defaultCapacity;
        private static int _maxCapacity;
        private static bool _collectionCheck;

        private GameObject emptyHolder;

        private static GameObject particleSystemsPools;
        private static GameObject gameObjectsPools;
        private static GameObject UIPools;
        private static GameObject WorldUIPools;

        private static Dictionary<GameObject, ObjectPool<GameObject>> objectPools;
        private static Dictionary<GameObject, GameObject> cloneToPrefabMap;

        public enum PoolType
        {
            ParticleSystems,
            GameObjects,
            UI,
            WorldUI,
        }

        public static PoolType PoolingType;

        #region Initialization

        private void Awake()
        {
            _defaultCapacity = defaultCapacity;
            _maxCapacity = maxCapacity;
            _collectionCheck = collectionCheck;

            objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
            cloneToPrefabMap = new Dictionary<GameObject, GameObject>();

            SetupPools();
        }

        private void SetupPools()
        {
            emptyHolder = new GameObject("Object Pools");

            particleSystemsPools = new GameObject("Particle Systems");
            particleSystemsPools.transform.SetParent(emptyHolder.transform);

            gameObjectsPools = new GameObject("Game Objects");
            gameObjectsPools.transform.SetParent(emptyHolder.transform);

            UIPools = new GameObject("UI");
            UIPools.transform.SetParent(emptyHolder.transform);
            Canvas canvas = UIPools.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = UIPools.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0;
            scaler.referenceResolution = new Vector2(Screen.width, Screen.height);

            WorldUIPools = new GameObject("World UI");
            WorldUIPools.transform.SetParent(emptyHolder.transform);
            Canvas worldCanvas = WorldUIPools.AddComponent<Canvas>();
            worldCanvas.renderMode = RenderMode.WorldSpace;
            worldCanvas.worldCamera = Camera.main;

            if (addToDontDestroyOnLoad)
            {
                DontDestroyOnLoad(particleSystemsPools.transform.root);
            }
        }

        private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => CreateObject(prefab, pos, rot, poolType),
                actionOnGet: OnGetObject,
                actionOnRelease: OnReleaseObject,
                actionOnDestroy: OnDestroyObject,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxCapacity,
                collectionCheck: _collectionCheck
                );

            objectPools.Add(prefab, pool);
        }

        private static void CreatePool(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => CreateObject(prefab, parent, rot, poolType),
                actionOnGet: OnGetObject,
                actionOnRelease: OnReleaseObject,
                actionOnDestroy: OnDestroyObject,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxCapacity,
                collectionCheck: _collectionCheck
                );

            objectPools.Add(prefab, pool);
        }

        private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
        {
            prefab.SetActive(false);

            GameObject obj = Instantiate(prefab, pos, rot);

            prefab.SetActive(true);

            GameObject parentObject = SetParentObject(poolType);
            obj.transform.SetParent(parentObject.transform);

            return obj;
        }

        private static GameObject CreateObject(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects)
        {
            prefab.SetActive(false);

            GameObject obj = Instantiate(prefab, parent);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = rot;
            obj.transform.localScale = Vector3.one;

            prefab.SetActive(true);

            GameObject parentObject = SetParentObject(poolType);
            obj.transform.SetParent(parentObject.transform);

            return obj;
        }

        private static void OnGetObject(GameObject obj)
        {

        }

        private static void OnReleaseObject(GameObject obj)
        {
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            if (obj.TryGetComponent<CanvasGroup>(out CanvasGroup group))
            {
                group.alpha = 1f;
                group.interactable = true;
                group.blocksRaycasts = true;
            }

            obj.SetActive(false);
        }

        private static void OnDestroyObject(GameObject obj)
        {
            if (cloneToPrefabMap.ContainsKey(obj))
            {
                cloneToPrefabMap.Remove(obj);
            }

#if UNITY_EDITOR
            DestroyImmediate(obj);
#else
            Destroy(obj);
#endif
        }

        #endregion

        #region Utils

        private static GameObject SetParentObject(PoolType poolType)
        {
            switch (poolType)
            {
                case PoolType.ParticleSystems:

                    return particleSystemsPools;

                case PoolType.GameObjects:

                    return gameObjectsPools;

                case PoolType.UI:

                    return UIPools;

                case PoolType.WorldUI:

                    return WorldUIPools;

                default:
                    return null;
            }
        }

        private static T SpawnObject<T>(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : UnityEngine.Object
        {
            if (!objectPools.ContainsKey(objectToSpawn))
            {
                CreatePool(objectToSpawn, spawnPos, spawnRotation, poolType);
            }

            GameObject obj = objectPools[objectToSpawn].Get();

            if (obj != null)
            {
                if (!cloneToPrefabMap.ContainsKey(obj))
                {
                    cloneToPrefabMap.Add(obj, objectToSpawn);
                }

                obj.transform.position = spawnPos;
                obj.transform.rotation = spawnRotation;
                obj.SetActive(true);

                if (typeof(T) == typeof(GameObject))
                {
                    return obj as T;
                }

                T component = obj.GetComponent<T>();

                if (component == null)
                {
                    Debug.LogError($"Object {objectToSpawn.name} doesn't have a component of type {typeof(T)}");
                    return null;
                }

                return component;
            }

            return null;
        }

        private static T SpawnObject<T>(GameObject objectToSpawn, Transform parent, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : UnityEngine.Object
        {
            if (!objectPools.ContainsKey(objectToSpawn))
            {
                CreatePool(objectToSpawn, parent, spawnRotation, poolType);
            }

            GameObject obj = objectPools[objectToSpawn].Get();

            if (obj != null)
            {
                if (!cloneToPrefabMap.ContainsKey(obj))
                {
                    cloneToPrefabMap.Add(obj, objectToSpawn);
                }

                obj.transform.SetParent(parent);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = spawnRotation;
                obj.SetActive(true);

                if (typeof(T) == typeof(GameObject))
                {
                    return obj as T;
                }

                T component = obj.GetComponent<T>();

                if (component == null)
                {
                    Debug.LogError($"Object {objectToSpawn.name} doesn't have a component of type {typeof(T)}");
                    return null;
                }

                return component;
            }

            return null;
        }

        #endregion

        #region Logic

        /// <summary>
        /// Checks if a pool for the given prefab already exists.
        /// </summary>
        /// <param name="prefab">Pool Template</param>
        /// <returns></returns>
        public static bool HasPool(GameObject prefab)
        {
            if (objectPools.ContainsKey(prefab))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a pool for the given prefab with the specified parameters.
        /// </summary>
        /// <param name="prefab">Pool Template</param>
        /// <param name="pos">Position</param>
        /// <param name="rot">Rotation</param>
        /// <param name="poolType">Pool type to ordering the created objects</param>
        /// <param name="defaultCapacity">Start capacity of the pool</param>
        /// <param name="maxCapacity">Max capacity of the pool</param>
        /// <param name="collectionCheck"></param>
        public static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects, int defaultCapacity = 5, int maxCapacity = 20, bool collectionCheck = false)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => CreateObject(prefab, pos, rot, poolType),
                actionOnGet: OnGetObject,
                actionOnRelease: OnReleaseObject,
                actionOnDestroy: OnDestroyObject,
                defaultCapacity: defaultCapacity,
                maxSize: maxCapacity,
                collectionCheck: collectionCheck
                );

            objectPools.Add(prefab, pool);
        }

        /// <summary>
        /// Creates a pool for the given prefab to be the child of given parent transform.
        /// </summary>
        /// <param name="prefab">Pool Template</param>
        /// <param name="pos">Position</param>
        /// <param name="rot">Rotation</param>
        /// <param name="poolType">Pool type to ordering the created objects</param>
        /// <param name="defaultCapacity">Start capacity of the pool</param>
        /// <param name="maxCapacity">Max capacity of the pool</param>
        /// <param name="collectionCheck"></param>
        public static void CreatePool(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects, int defaultCapacity = 5, int maxCapacity = 20, bool collectionCheck = false)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => CreateObject(prefab, parent, rot, poolType),
                actionOnGet: OnGetObject,
                actionOnRelease: OnReleaseObject,
                actionOnDestroy: OnDestroyObject,
                defaultCapacity: defaultCapacity,
                maxSize: maxCapacity,
                collectionCheck: collectionCheck
                );

            objectPools.Add(prefab, pool);
        }

        /// <summary>
        /// Sets the pool size for all pools created after this call. Doesn't affect already created pools.
        /// </summary>
        /// <param name="defaultCapacity">Default capacity of the all pools</param>
        /// <param name="maxCapacity">Max capacity of the all pools</param>
        public static void SetPoolSize(int? defaultCapacity = null, int? maxCapacity = null)
        {
            _defaultCapacity = defaultCapacity ?? _defaultCapacity;
            _maxCapacity = maxCapacity ?? _maxCapacity;
        }

        /// <summary>
        /// Spawn an object with type from the pool. If the pool doesn't exist, it will be created with the given parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typePrefab">Pool Template</param>
        /// <param name="spawnPos">Position</param>
        /// <param name="spawnRotation">Rotation</param>
        /// <param name="poolType"></param>
        /// <returns></returns>
        public static T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : UnityEngine.Component
        {
            return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRotation, poolType);
        }

        /// <summary>
        /// Spawn an Game Object from the pool. If the pool doesn't exist, it will be created with the given parameters.
        /// </summary>
        /// <param name="objectToSpawn">Pool Template</param>
        /// <param name="spawnPos">Position</param>
        /// <param name="spawnRotation">Rotation</param>
        /// <param name="poolType"></param>
        /// <returns></returns>
        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects)
        {
            return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRotation, poolType);
        }

        /// <summary>
        /// Spawn an object with type from the pool. If the pool doesn't exist, it will be created with the given parameters.
        /// </summary>
        /// <param name="typePrefab">Pool Template</param>
        /// <param name="parent">Parent Transform</param>
        /// <param name="spawnRotation">Rotation</param>
        /// <param name="poolType"></param>
        /// <returns></returns>
        public static T SpawnObject<T>(T typePrefab, Transform parent, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : UnityEngine.Component
        {
            return SpawnObject<T>(typePrefab.gameObject, parent, spawnRotation, poolType);
        }

        /// <summary>
        /// Spawn an Game Object from the pool. If the pool doesn't exist, it will be created with the given parameters.
        /// </summary>
        /// <param name="objectToSpawn">Pool Template</param>
        /// <param name="parent">Parent Transform</param>
        /// <param name="spawnRotation">Rotation</param>
        /// <param name="poolType"></param>
        /// <returns></returns>
        public static GameObject SpawnObject(GameObject objectToSpawn, Transform parent, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects)
        {
            return SpawnObject<GameObject>(objectToSpawn, parent, spawnRotation, poolType);
        }

        /// <summary>
        /// Make inactive the given object and return it to the pool. If the object doesn't belong to any pool, a warning will be logged.
        /// </summary>
        /// <param name="obj">Object to return to pool</param>
        /// <param name="poolType">Return to here</param>
        /// <returns></returns>
        public static void Release(GameObject obj, PoolType poolType = PoolType.GameObjects)
        {
            if (cloneToPrefabMap.TryGetValue(obj, out GameObject prefab))
            {
                GameObject parentObject = SetParentObject(poolType);

                if (obj.transform.parent != parentObject.transform)
                {
                    obj.transform.SetParent(parentObject.transform);
                }

                if (objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
                {
                    pool.Release(obj);
                }
            }
            else
            {
                Debug.LogWarning("Trying to return an object that is not pooled: " + obj.name);
            }
        }

        #endregion
    }
}
