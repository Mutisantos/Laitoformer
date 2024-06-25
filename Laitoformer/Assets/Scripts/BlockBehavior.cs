using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockBehavior : MonoBehaviour
{
    public Vector2 cellCollisionDetectionThreshold;
    public Tilemap blockTilemap;
    public Tile breakableTile;
    public Tile cookieTile;
    public Tile powerUpTile;
    public Tile blockTile;
    public Collectable CookiePrefab;
    public Collectable PowerUpPrefab;
    public Collectable BreakingBlockPrefab;
    public AudioClip hardCollisionSfx;
    public AudioClip breakBlockSfx;
    public AudioClip cookiePopSfx;
    public AudioClip powerUpPopSfx;

    private Vector3Int GetGridPositionFromTransform(Vector2 transformPosition)
    {
        Vector3Int cellPosition = blockTilemap.WorldToCell(transformPosition);
        cellPosition.y -= Mathf.FloorToInt(cellCollisionDetectionThreshold.y);
        cellPosition.x -= Mathf.FloorToInt(cellCollisionDetectionThreshold.x);
        return cellPosition;
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.transform.tag == "PlayerHead")
        {
            Vector3Int cellPosition = GetGridPositionFromTransform(collider.gameObject.transform.position);
            Vector3 collisionPosition = collider.transform.position;
            collisionPosition.y += 4;
            if (blockTilemap.GetTile(cellPosition) != null)
            {

                if (blockTilemap.GetTile(cellPosition).name == this.cookieTile.name)
                {
                    SoundManager.Instance.PlayEffectOnce(cookiePopSfx);
                    Instantiate(CookiePrefab, collisionPosition, Quaternion.identity);
                    blockTilemap.SetTile(cellPosition, blockTile);
                }
                else if (blockTilemap.GetTile(cellPosition).name == this.powerUpTile.name)
                {
                    SoundManager.Instance.PlayEffectOnce(powerUpPopSfx);
                    Instantiate(PowerUpPrefab, collisionPosition, Quaternion.identity);
                    blockTilemap.SetTile(cellPosition, blockTile);
                }
                else if (blockTilemap.GetTile(cellPosition).name == this.breakableTile.name)
                {
                    if (GameManager.Instance.HasPowerUp())
                    {

                        collisionPosition.y -= 1;
                        Instantiate(BreakingBlockPrefab, collisionPosition, Quaternion.identity);
                        SoundManager.Instance.PlayEffectOnce(breakBlockSfx);
                        GameManager.Instance.ChangeScore(100);
                        blockTilemap.SetTile(cellPosition, null);
                    }
                    else
                    {
                        SoundManager.Instance.PlayEffectOnce(hardCollisionSfx);
                    }
                }
                else if (blockTilemap.GetTile(cellPosition).name == this.blockTile.name)
                {
                    SoundManager.Instance.PlayEffectOnce(hardCollisionSfx);
                }
            }
        }
    }

}
