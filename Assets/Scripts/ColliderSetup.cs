using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class ColliderSetup : EditorWindow
{
    private GameObject targetObject;
    private ColliderType colliderType = ColliderType.Box;
    private bool isTrigger = false;
    private string layerName = "Default";
    private string tagName = "Untagged";
    private bool applyToChildren = true;

    private enum ColliderType
    {
        Box,
        Circle,
        Capsule
    }

    [MenuItem("Tools/Gameplay/Collider Setup")]
    public static void ShowWindow()
    {
        GetWindow<ColliderSetup>("Collider Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Collider Setup Tool", EditorStyles.boldLabel);

        targetObject = EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true) as GameObject;
        colliderType = (ColliderType)EditorGUILayout.EnumPopup("Collider Type", colliderType);
        isTrigger = EditorGUILayout.Toggle("Is Trigger", isTrigger);
        layerName = EditorGUILayout.TextField("Layer Name", layerName);
        tagName = EditorGUILayout.TextField("Tag Name", tagName);
        applyToChildren = EditorGUILayout.Toggle("Apply to Children", applyToChildren);

        if (GUILayout.Button("Apply Collider Settings"))
        {
            if (targetObject != null)
            {
                Undo.RecordObject(targetObject, "Apply Collider Settings");
                ApplyColliderSettings(targetObject);
                if (applyToChildren)
                {
                    foreach (Transform child in targetObject.transform)
                    {
                        Undo.RecordObject(child.gameObject, "Apply Collider Settings to Child");
                        ApplyColliderSettings(child.gameObject);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Please select a target object first!");
            }
        }

        if (GUILayout.Button("Setup Common Objects"))
        {
            SetupCommonObjects();
        }
    }

    private void ApplyColliderSettings(GameObject obj)
    {
        // Remove existing colliders
        Collider2D[] existingColliders = obj.GetComponents<Collider2D>();
        foreach (var collider in existingColliders)
        {
            DestroyImmediate(collider);
        }

        // Add new collider
        Collider2D newCollider = null;
        switch (colliderType)
        {
            case ColliderType.Box:
                newCollider = obj.AddComponent<BoxCollider2D>();
                break;
            case ColliderType.Circle:
                newCollider = obj.AddComponent<CircleCollider2D>();
                break;
            case ColliderType.Capsule:
                newCollider = obj.AddComponent<CapsuleCollider2D>();
                break;
        }

        if (newCollider != null)
        {
            newCollider.isTrigger = isTrigger;
        }

        // Set layer and tag
        if (!string.IsNullOrEmpty(layerName))
        {
            obj.layer = LayerMask.NameToLayer(layerName);
        }
        if (!string.IsNullOrEmpty(tagName))
        {
            obj.tag = tagName;
        }
    }

    private void SetupCommonObjects()
    {
        // Setup Player
        GameObject player = GameObject.FindGameObjectWithTag(GameLayers.PlayerTag);
        if (player != null)
        {
            Undo.RecordObject(player, "Setup Player Collider");
            CapsuleCollider2D playerCollider = player.GetComponent<CapsuleCollider2D>();
            if (playerCollider == null)
                playerCollider = player.AddComponent<CapsuleCollider2D>();
            playerCollider.isTrigger = false;
            GameLayers.SetupObjectLayers(player, GameLayers.PlayerLayer, GameLayers.PlayerTag);
        }

        // Setup Obstacles
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag(GameLayers.ObstacleTag);
        foreach (var obstacle in obstacles)
        {
            Undo.RecordObject(obstacle, "Setup Obstacle Collider");
            BoxCollider2D obstacleCollider = obstacle.GetComponent<BoxCollider2D>();
            if (obstacleCollider == null)
                obstacleCollider = obstacle.AddComponent<BoxCollider2D>();
            obstacleCollider.isTrigger = false;
            GameLayers.SetupObjectLayers(obstacle, GameLayers.ObstacleLayer, GameLayers.ObstacleTag);
        }

        // Setup Coins
        GameObject[] coins = GameObject.FindGameObjectsWithTag(GameLayers.CoinTag);
        foreach (var coin in coins)
        {
            Undo.RecordObject(coin, "Setup Coin Collider");
            CircleCollider2D coinCollider = coin.GetComponent<CircleCollider2D>();
            if (coinCollider == null)
                coinCollider = coin.AddComponent<CircleCollider2D>();
            coinCollider.isTrigger = true;
            GameLayers.SetupObjectLayers(coin, GameLayers.CollectibleLayer, GameLayers.CoinTag);
        }

        // Setup Ground
        GameObject[] grounds = GameObject.FindGameObjectsWithTag(GameLayers.GroundTag);
        foreach (var ground in grounds)
        {
            Undo.RecordObject(ground, "Setup Ground Collider");
            BoxCollider2D groundCollider = ground.GetComponent<BoxCollider2D>();
            if (groundCollider == null)
                groundCollider = ground.AddComponent<BoxCollider2D>();
            groundCollider.isTrigger = false;
            GameLayers.SetupObjectLayers(ground, GameLayers.GroundLayer, GameLayers.GroundTag);
        }
    }
}
#endif 