using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : Singleton<UIHealthBar> {

    public Image mask;
    float originalSize;

    private void Start() {
        originalSize = mask.rectTransform.rect.height;
    }
    public void SetValue(float value) {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalSize * value);
    }

}
