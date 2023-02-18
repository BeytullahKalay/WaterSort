
namespace BottleCodes.State
{
    public abstract class BottleBaseState
    {
        public abstract void EnterState(BottleStateManager bottleState);

        public abstract void UpdateState(BottleStateManager bottleState);
    }
}
