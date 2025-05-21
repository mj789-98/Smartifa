using UnityEngine;

public static class GameLayers
{
    // Layer names
    public static readonly string PlayerLayer = "Player";
    public static readonly string ObstacleLayer = "Obstacle";
    public static readonly string GroundLayer = "Ground";
    public static readonly string CollectibleLayer = "Collectible";

    // Layer masks
    private static int? playerLayer;
    private static int? obstacleLayer;
    private static int? groundLayer;
    private static int? collectibleLayer;

    // Tag names
    public static readonly string PlayerTag = "Player";
    public static readonly string ObstacleTag = "Obstacle";
    public static readonly string CoinTag = "Coin";
    public static readonly string GroundTag = "Ground";

    // Layer mask properties with caching
    public static int PlayerLayerMask
    {
        get
        {
            if (!playerLayer.HasValue)
                playerLayer = LayerMask.NameToLayer(PlayerLayer);
            return playerLayer.Value;
        }
    }

    public static int ObstacleLayerMask
    {
        get
        {
            if (!obstacleLayer.HasValue)
                obstacleLayer = LayerMask.NameToLayer(ObstacleLayer);
            return obstacleLayer.Value;
        }
    }

    public static int GroundLayerMask
    {
        get
        {
            if (!groundLayer.HasValue)
                groundLayer = LayerMask.NameToLayer(GroundLayer);
            return groundLayer.Value;
        }
    }

    public static int CollectibleLayerMask
    {
        get
        {
            if (!collectibleLayer.HasValue)
                collectibleLayer = LayerMask.NameToLayer(CollectibleLayer);
            return collectibleLayer.Value;
        }
    }

    // Collision matrix helper methods
    public static bool ShouldCollide(GameObject obj1, GameObject obj2)
    {
        // Define collision rules
        if (obj1.CompareTag(PlayerTag) && obj2.CompareTag(ObstacleTag))
            return true;
        if (obj1.CompareTag(PlayerTag) && obj2.CompareTag(GroundTag))
            return true;
        if (obj1.CompareTag(PlayerTag) && obj2.CompareTag(CoinTag))
            return true;
        
        return false;
    }

    // Layer setup helper
    public static void SetupObjectLayers(GameObject obj, string layerName, string tagName)
    {
        // Set layer
        obj.layer = LayerMask.NameToLayer(layerName);
        
        // Set tag
        if (!string.IsNullOrEmpty(tagName))
            obj.tag = tagName;
        
        // Apply to all children
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = obj.layer;
            if (!string.IsNullOrEmpty(tagName))
                child.gameObject.tag = tagName;
        }
    }

    // Validation method
    public static bool ValidateLayerSetup()
    {
        bool isValid = true;

        // Check if all required layers exist
        if (LayerMask.NameToLayer(PlayerLayer) == -1)
        {
            Debug.LogError($"Missing layer: {PlayerLayer}");
            isValid = false;
        }
        if (LayerMask.NameToLayer(ObstacleLayer) == -1)
        {
            Debug.LogError($"Missing layer: {ObstacleLayer}");
            isValid = false;
        }
        if (LayerMask.NameToLayer(GroundLayer) == -1)
        {
            Debug.LogError($"Missing layer: {GroundLayer}");
            isValid = false;
        }
        if (LayerMask.NameToLayer(CollectibleLayer) == -1)
        {
            Debug.LogError($"Missing layer: {CollectibleLayer}");
            isValid = false;
        }

        return isValid;
    }
} 