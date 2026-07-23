using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public abstract class MovementAbility : MonoBehaviour
{
    public virtual int TickPriority => 0;

    protected PlayerController Controller { get; private set; }
    protected PlayerStats Stats { get; private set; }
    protected PlayerInputHandler Input { get; private set; }

    public virtual void InitializeAbility(PlayerController controller)
    {
        Controller = controller;
        Stats = controller.Stats;
        Input = controller.InputHandler;
    }

    public abstract void TickAbility(float dt);
}
