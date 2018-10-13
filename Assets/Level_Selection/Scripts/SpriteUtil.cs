using UnityEngine;
using System.Collections;

public class SpriteUtil
{
    public static void CenterSpriteInImage(GameObject sprite, GameObject image)
    {
        SpriteRenderer sr = sprite.GetComponent<SpriteRenderer>();
        RectTransform rt = image.GetComponent<RectTransform>();

        sprite.transform.position = image.transform.position;

        float rw = rt.rect.width * rt.lossyScale.x;
        float rh = rt.rect.height * rt.lossyScale.y;

        float sw = sr.bounds.size.x;
        float sh = sr.bounds.size.y;

        sprite.transform.localScale = Vector3.Scale(sprite.transform.localScale,
            new Vector3(rw / sw, rh / sh, 1f));
    }

    public static GameObject MakeSprite(Texture2D image, float width, float height, string name)
    {
        GameObject obj;
        SpriteRenderer sr;

        float pixWidth = (float)image.width;
        float pixHeight = (float)image.height;

        float widthPPU = pixWidth / width;
        float heightPPU = pixHeight / height;

        float ppu = Mathf.Max(widthPPU, heightPPU);

        obj = new GameObject(name);
        sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(image, new Rect(0f, 0f, image.width, image.height),
            new Vector2(0.5f, 0.5f), ppu);

        obj.transform.localScale = new Vector3(ppu / widthPPU, ppu / heightPPU, 1f);

        return obj;
    }

    public static GameObject AddSprite(Texture2D image, Vector2 size, Vector3 position, string name, Transform parent)
    {
        GameObject obj;
        obj = MakeSprite(image, size.x, size.y, name);
        obj.transform.position = position;
        obj.transform.parent = parent;

        TilePulse tp = obj.AddComponent<TilePulse>() as TilePulse; 
        
        return obj;
    }

    public static GameObject AddSprite(Texture2D image, Vector2 size, Vector3 position, string name, GameObject parent)
    {
        return AddSprite(image, size, position, name, parent.transform);
    }

    public static GameObject AddSprite(Texture2D image, Vector2 size, Vector3 position, 
                                        string name, GameObject parent, bool preserveAspectRatio) {
        if (preserveAspectRatio)
        {
            float iWidth = (float)image.width;
            float iHeight = (float)image.height;
            float iAspect = iWidth / iHeight;

            float sAspect = size.x / size.y;

            if (sAspect > iAspect)
            {
                // Width is too large
                size = new Vector2(size.x * (iAspect / sAspect), size.y);
            }
            else
            {
                // Height is too large
                size = new Vector2(size.x, size.y * (sAspect / iAspect));
            }
        }

        return AddSprite(image, size, position, name, parent);
    }
}
