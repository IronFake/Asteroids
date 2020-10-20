using UnityEngine;

class AnimateTiledTexture : MonoBehaviour
{
    public int uvAnimationTileX = 4;
    public int uvAnimationTileY = 16;

    public float framesPerSecond = 30.0f;

    void Update()
    {
        // Calculate index
        int index = (int)(Time.time * framesPerSecond);
        // repeat when exhausting all frames
        index = index % (uvAnimationTileX * uvAnimationTileY);

        // Size of every tile
        var size = new Vector2(1.0f / uvAnimationTileX, 1.0f / uvAnimationTileY);

        // split into horizontal and vertical index
        var uIndex = index % uvAnimationTileX;
        var vIndex = index / uvAnimationTileX;

        // build offset
        // v coordinate is the bottom of the image in opengl so we need to invert.
        var offset = new Vector2(uIndex * size.x, 1.0f - size.y - vIndex * size.y);

        GetComponent<LineRenderer>().material.SetTextureOffset("_MainTex", offset);
        GetComponent<LineRenderer>().material.SetTextureScale("_MainTex", size);
    }
}
