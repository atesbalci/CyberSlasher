namespace Game
{
    public class Boss : Enemy
    {
        protected override float Damage
        {
            get { return 0.3f; }
        }

        public override void Hit(HitType hitType)
        {
            if (hitType == HitType.Super)
            {
                IsDead = true;
            }
        }
    }
}
