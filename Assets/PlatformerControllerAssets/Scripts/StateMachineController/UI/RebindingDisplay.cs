using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindingDisplay : MonoBehaviour {

    [SerializeField] private InputActionReference shootAction = null;
    [SerializeField] private PlayerInputHandler inputHandler = null;
    [SerializeField] private GameObject MenuObject = null;
    [SerializeField] private TMP_Text bindingDisplayNameText = null;
    [SerializeField] private GameObject startRebindObject = null;
    [SerializeField] private GameObject waitingForInputObject = null;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private const string RebindsKey = "rebinds";

    private void Start() {
        // string rebinds = PlayerPrefs.GetString(RebindsKey, string.Empty);

        // if (string.IsNullOrEmpty(rebinds)) { return; }

        // inputHandler.PlayerInput.actions.LoadBindingOverridesFromJson(rebinds);

        // int bindingIndex = shootAction.action.GetBindingIndexForControl(shootAction.action.controls[0]);

        // bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
        //     shootAction.action.bindings[bindingIndex].effectivePath,
        //     InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
    // private void Update() => EscapeInput();

    public void SaveRebindings() {
        string rebinds = inputHandler.PlayerInput.actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(RebindsKey, rebinds);
    }

    public void StartRebinding() {
        startRebindObject.SetActive(false);
        waitingForInputObject.SetActive(true);

        inputHandler.PlayerInput.SwitchCurrentActionMap("Empty");

        rebindingOperation = shootAction.action.PerformInteractiveRebinding()
        .WithControlsExcluding("Mouse")
        .OnMatchWaitForAnother(0.1f)
        .OnComplete(operation => RebindComplete())
        .Start();
    }
    private void RebindComplete() {

        int bindingIndex = shootAction.action.GetBindingIndexForControl(shootAction.action.controls[0]);

        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            shootAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();

        startRebindObject.SetActive(true);
        waitingForInputObject.SetActive(false);

        inputHandler.PlayerInput.SwitchCurrentActionMap("Gameplay");

    }
    public void ResetBindingOverrides() {
        InputActionRebindingExtensions.RemoveAllBindingOverrides(shootAction);

        int bindingIndex = shootAction.action.GetBindingIndexForControl(shootAction.action.controls[0]);

        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            shootAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
    private void EscapeInput() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            MenuObject.SetActive(false);
            inputHandler.PlayerInput.SwitchCurrentActionMap("Gameplay");
        }
    }

}
