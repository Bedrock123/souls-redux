using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public List<Action> actionSlots = new List<Action>();

    StateManager stateManager;

    public void Init(StateManager _stateManager)
    {
        stateManager = _stateManager;

        AssignWeaponActions(false);
    }

    // Assigs the actions slots from the weapon action alots
    public void AssignWeaponActions(bool twoHanded)
    {
        // Get the current equpred qeapon
        Weapon _currentWeapon = stateManager.inventoryManager.currentWeapon;

        // Clears all actions when weapon is assigned
        for (int i = 0; i < 4; i++)
        {
            Action action = GetAction((ActionInput)i);
            action.targetAnimation = null;
        }

        if (twoHanded)
        {
            // Loop through all of the actions for the weapon
            for (int i = 0; i < _currentWeapon.twoHandedActions.Count; i++)
            {
                // Get the matching actions input slot for the weapon action and action managet
                Action action = GetAction(_currentWeapon.twoHandedActions[i].input);

                // Set teh target target animation actio nto the weapon target animation.
                action.targetAnimation = _currentWeapon.twoHandedActions[i].targetAnimation;
            }
        } else
        {
            // Loop through all of the actions for the weapon
            for (int i = 0; i < _currentWeapon.actions.Count; i++)
            {
                // Get the matching actions input slot for the weapon action and action managet
                Action action = GetAction(_currentWeapon.actions[i].input);

                // Set teh target target animation actio nto the weapon target animation.
                action.targetAnimation = _currentWeapon.actions[i].targetAnimation;
            }
        }
       
    }

    // Assgns actions to the actions sltos
    ActionManager()
    {
        // Crea a new action
        for (int i = 0; i < 4; i++)
        {
            Action _action = new Action();
            _action.input = (ActionInput)i;
            actionSlots.Add(_action);
        }
    }

    public Action GetActionSlot(StateManager stateManager)
    {
        ActionInput actionInput = GetActionInput(stateManager);

        return GetAction(actionInput);
    }

    // Gets the action animatino
    public Action GetAction(ActionInput actionInput)
    {
        for (int i = 0; i < actionSlots.Count; i++)
        {
            if (actionSlots[i].input == actionInput)
            {
                return actionSlots[i];
            }
        }
        return null;
    }

    // Assins inputs to the action slots.
    public ActionInput GetActionInput(StateManager stateManager)
    {
        // Get the action input enum base don the state manager
        if (stateManager.rb)
            return ActionInput.rb;
        if (stateManager.rt)
            return ActionInput.rt;
        if (stateManager.lb)
            return ActionInput.lb;
        if (stateManager.lt)
            return ActionInput.lt;

        return ActionInput.rb;


    }
  
}

public enum ActionInput
{
    rb, lb, rt, lt
}

[System.Serializable]
public class Action
{
    public ActionInput input;
    public string targetAnimation;
}