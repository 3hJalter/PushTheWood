using UnityEngine;

namespace Daivq
{
    public class PlayerAnimationControl : MonoBehaviour
    {
        private static readonly int ANIM_BOOL_IS_MOVING = Animator.StringToHash("IsMoving");
        private static readonly int ANIM_TRIGGER_PUSH = Animator.StringToHash("Push");
        [SerializeField] private Animator _animator;

        public void Idle()
        {
            _animator.SetBool(ANIM_BOOL_IS_MOVING, false);
        }

        public void Run()
        {
            _animator.SetBool(ANIM_BOOL_IS_MOVING, true);
        }

        public void Push()
        {
            _animator.SetTrigger(ANIM_TRIGGER_PUSH);
        }
    }
}
