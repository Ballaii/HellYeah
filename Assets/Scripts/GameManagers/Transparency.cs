using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GLTFast.Schema;
using UnityEngine.UI;
public class Transparency : MonoBehaviour
{

    public RawImage Sprite;
    Color color;
    public float alpha;

    void Start()
    {
        color = Sprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        color.a = alpha / 255;
        Sprite.color = color;
    }
}
