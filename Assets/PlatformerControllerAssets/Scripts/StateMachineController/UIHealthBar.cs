using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : Singleton<UIHealthBar> {
    Image mask;
    float originalSize;

    private void Start() {
        mask = transform.GetChild(0).GetComponent<Image>();
        originalSize = mask.rectTransform.rect.height;
    }
    public void SetValue(float value) {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalSize * value);
    }


}
